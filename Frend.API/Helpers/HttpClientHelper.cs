using Frend.API.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Frend.API.Helpers
{
    /// <summary>
    ///  HttpClientHelper 
    ///  
    ///  Note. Depency Injection (.Net Core memorycache) doesn't work with static
    ///  https://stackoverflow.com/questions/40946583/imemorycache-dependency-injection-outside-controllers
    /// </summary>
    public class HttpClientHelper
    {
        private readonly IMemoryCache _memoryCache;

        public HttpClientHelper(IMemoryCache cache)
        {
            _memoryCache = cache;
        }

        public HttpClient GetHttpClientForOptions(Options options)
        {
            var cacheKey = GetHttpClientCacheKey(options);

            if (_memoryCache.TryGetValue(cacheKey, out HttpClient _httpclient))
            {
                return _httpclient;
            }

            var httpClient = new HttpClientFactory().CreateClient(options);
            httpClient.SetDefaultRequestHeadersBasedOnOptions(options);

            //sliding expiration (evict if not accessed for 7 days)
            _memoryCache.Set("key", httpClient, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(7)
            });

            _memoryCache.Set(cacheKey, httpClient);

            return httpClient;
        }

        public Dictionary<string, string> GetResponseHeaderDictionary(HttpResponseHeaders responseMessageHeaders, HttpContentHeaders contentHeaders)
        {
            var responseHeaders = responseMessageHeaders.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            var allHeaders = contentHeaders?.ToDictionary(h => h.Key, h => string.Join(";", h.Value)) ?? new Dictionary<string, string>();
            responseHeaders.ToList().ForEach(x => allHeaders[x.Key] = x.Value);
            return allHeaders;
        }


        public IDictionary<string, string> GetHeaderDictionary(Header[] headers, Options options)
        {
            if (!headers.Any(header => header.Name.ToLower().Equals("authorization")))
            {

                var authHeader = new Header { Name = "Authorization" };
                switch (options.Authentication)
                {
                    case Authentication.Basic:
                        authHeader.Value = $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{options.Username}:{options.Password}"))}";
                        headers = headers.Concat(new[] { authHeader }).ToArray();
                        break;
                    case Authentication.OAuth:
                        authHeader.Value = $"Bearer {options.Token}";
                        headers = headers.Concat(new[] { authHeader }).ToArray();
                        break;
                }
            }

            //Ignore case for headers and key comparison
            return headers.ToDictionary(key => key.Name, value => value.Value, StringComparer.InvariantCultureIgnoreCase);
        }

        public HttpContent GetContent(Input input, IDictionary<string, string> headers)
        {
            //Check if Content-Type exists and is set and valid
            var contentTypeIsSetAndValid = false;
            MediaTypeWithQualityHeaderValue validContentType = null;
            if (headers.TryGetValue("content-type", out string contentTypeValue))
            {
                contentTypeIsSetAndValid = MediaTypeWithQualityHeaderValue.TryParse(contentTypeValue, out validContentType);
            }

            return contentTypeIsSetAndValid
                ? new StringContent(input.Message ?? "", Encoding.GetEncoding(validContentType.CharSet ?? Encoding.UTF8.WebName))
                : new StringContent(input.Message ?? "");
        }

        public void ClearClientCache()
        {
            if (_memoryCache is MemoryCache memoryCache)
                memoryCache.Compact(1);
        }

        #region "Static methods"

        private static string GetHttpClientCacheKey(Options options)
        {
            // Includes everything except for options.Token, which is used on request level, not http client level
            return $"{options.Authentication}:{options.Username}:{options.Password}:{options.ClientCertificateSource}"
                   + $":{options.ClientCertificateFilePath}:{options.ClientCertificateInBase64}:{options.ClientCertificateKeyPhrase}"
                   + $":{options.CertificateThumbprint}:{options.LoadEntireChainForCertificate}:{options.ConnectionTimeoutSeconds}"
                   + $":{options.FollowRedirects}:{options.AllowInvalidCertificate}:{options.AllowInvalidResponseContentTypeCharSet}"
                   + $":{options.ThrowExceptionOnErrorResponse}:{options.AutomaticCookieHandling}";
        }

        public static HttpContent GetContent(ByteInput input)
        {
            return new ByteArrayContent(input.ContentBytes);
        }

        public static object TryParseRequestStringResultAsJToken(string response)
        {
            try
            {
                return string.IsNullOrWhiteSpace(response) ? new JValue("") : JToken.Parse(response);
            }
            catch (JsonReaderException)
            {
                throw new JsonReaderException($"Unable to read response message as json: {response}");
            }
        }
        #endregion
    }
}
