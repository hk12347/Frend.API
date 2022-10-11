using Frend.API.Helpers;
using Frend.API.Models;
using System.Diagnostics;

namespace Frend.API.Services
{
    #region "Interface"
    public interface IFrendAPIService
    {
        Task<HttpResponseMessage> GetHttpRequestResponseAsync(
            System.Net.Http.HttpClient httpClient, 
            string method, 
            string url,
            HttpContent content,
            IDictionary<string, string> headers,
            Options options, 
            CancellationToken cancellationToken);
    }
    #endregion

    #region "Service"
    public class FrendAPIService : IFrendAPIService
    {
        public async Task<HttpResponseMessage> GetHttpRequestResponseAsync(
                    System.Net.Http.HttpClient httpClient, 
                    string method, 
                    string url,
                    HttpContent content,
                    IDictionary<string, string> headers,
                    Options options,
                    CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Only POST, PUT, PATCH and DELETE can have content, otherwise the HttpClient will fail
            var isContentAllowed = Enum.TryParse(method, ignoreCase: true, result: out SendMethod _);

            using (var request = new HttpRequestMessage(new HttpMethod(method), new Uri(url))
            {
                Content = isContentAllowed ? content : null,
            })
            {
                //Clear default headers
                content.Headers.Clear();
                foreach (var header in headers)
                {
                    var requestHeaderAddedSuccessfully = request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    if (!requestHeaderAddedSuccessfully && request.Content != null)
                    {
                        //Could not add to request headers try to add to content headers
                        // this check is probably not needed anymore as the new HttpClient does not seem fail on malformed headers
                        var contentHeaderAddedSuccessfully = content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        if (!contentHeaderAddedSuccessfully)
                        {
                            Trace.TraceWarning($"Could not add header {header.Key}:{header.Value}");
                        }
                    }
                }

                HttpResponseMessage response;
                try
                {
                    response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException) 
                {
                     // Cancellation is from outside -> Just throw 
                     throw;
                 }
                catch (Exception) {
                    // Cancellation is from inside of the request, mostly likely a timeout
                    throw new Exception("HttpRequest was canceled, most likely due to a timeout.");
                }

                // this check is probably not needed anymore as the new HttpClient does not fail on invalid charsets
                if (options.AllowInvalidResponseContentTypeCharSet && response.Content.Headers?.ContentType != null)
                {
                    response.Content.Headers.ContentType.CharSet = null;
                }
                return response;
            }
        }
    }
    #endregion
}