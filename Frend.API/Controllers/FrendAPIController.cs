using Frend.API.Models;
using Frend.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Frend.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class FrendAPIController : ControllerBase
    {
        private readonly ILogger<FrendAPIController> _logger;
        private readonly Helpers.HttpClientHelper _httpClient;

        private IFrendAPIService _service;

        public FrendAPIController(IFrendAPIService frendAPIService, Helpers.HttpClientHelper httpClient, ILogger<FrendAPIController> logger)
        {
            _service = frendAPIService;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// HTTP request
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="options">Optional parameters with default values</param>
        /// <returns>Object with the following properties: string Body, nDictionary(string,string) Headers. int StatusCode</returns>
        [HttpPost]
        [Route("HttpRequest")]
        public async Task<ActionResult<Models.HttpResponse>> HttpRequest([FromBody] Options options, CancellationToken cancellationToken = default)
        {
            Input input = new()
            {
                Url = options.Url,
                Headers = options.Headers,
                Message = options.Message,
                Method = options.Method
            };

            var httpClient = _httpClient.GetHttpClientForOptions(options);
            var headers = _httpClient.GetHeaderDictionary(input.Headers, options);

            try
            {
                using (var content = _httpClient.GetContent(input, headers))
                {
                    using (var responseMessage = await _service.GetHttpRequestResponseAsync(
                            httpClient,
                            input.Method.ToString(),
                            input.Url,
                            content,
                            headers,
                            options,
                            cancellationToken)
                        .ConfigureAwait(false))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var response = new Models.HttpResponse()
                        {
                            Body = responseMessage.Content != null ? await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null,
                            StatusCode = (int)responseMessage.StatusCode,
                            Headers = _httpClient.GetResponseHeaderDictionary(responseMessage.Headers, responseMessage.Content?.Headers)
                        };

                        if (!responseMessage.IsSuccessStatusCode && options.ThrowExceptionOnErrorResponse)
                        {
                            throw new WebException(
                                $"Request to '{input.Url}' failed with status code {(int)responseMessage.StatusCode}. Response body: {response.Body}");
                        }

                        return response;
                    }
                }

            }
            catch (Exception ex) {
                return new JsonResult(new { message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }

        /// <summary>
        /// HTTP request with byte (binary) return type
        /// </summary>
        /// <param name="input">Input parameters</param>
        /// <param name="options">Optional parameters with default values</param>
        /// <returns>Object with the following properties: string BodyBytes, Dictionary(string,string) Headers. int StatusCode</returns>
        [HttpPost]
        [Route("HttpRequestBytes")]
        public async Task<object> HttpRequestBytes([FromBody] Options options, CancellationToken cancellationToken = default)
        {
            Input input = new()
            {
                Url = options.Url,
                Headers = options.Headers,
                Message = options.Message,
                Method = options.Method
            };

            var httpClient = _httpClient.GetHttpClientForOptions(options);
            var headers = _httpClient.GetHeaderDictionary(input.Headers, options);

            try
            {
                using (var content = _httpClient.GetContent(input, headers))
                {
                    using (var responseMessage = await _service.GetHttpRequestResponseAsync(
                            httpClient,
                            input.Method.ToString(),
                            input.Url,
                            content,
                            headers,
                            options,
                            cancellationToken)
                        .ConfigureAwait(false))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var response = new HttpByteResponse()
                        {
                            BodyBytes = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false),
                            ContentType = responseMessage.Content.Headers.ContentType,
                            StatusCode = (int)responseMessage.StatusCode,
                            Headers = _httpClient.GetResponseHeaderDictionary(responseMessage.Headers, responseMessage.Content.Headers)
                        };

                        if (!responseMessage.IsSuccessStatusCode && options.ThrowExceptionOnErrorResponse)
                        {
                            throw new WebException($"Request to '{input.Url}' failed with status code {(int)responseMessage.StatusCode}.");
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }

        /// <summary>
        /// Check for Web API Health
        /// </summary>
        /// <returns>StatusCode 200 OK</returns>        
        [Route("healthcheck")]
        [HttpGet]
        public IActionResult HealthCheckGet()
        {
            return Ok();
        }
    }
}
