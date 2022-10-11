using FluentAssertions;
using Frend.API.Helpers;
using Frend.API.Models;
using Frend.API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Reflection;
using System.Text;

namespace Frend.API.Tests.System.Controllers
{
    /// <summary>
    /// WebApplicationFactory for Functional tests.
    /// </summary>
    internal class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config => { });
            builder.ConfigureTestServices(services => { });
        }
    }

    public class FrendAPIControllerTest
    {
        private readonly IFrendAPIService _service;

        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private readonly MockHttpClientFactory _mockClientFactory;

        public FrendAPIControllerTest()
        {
            _service = new FrendAPIService();
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _mockClientFactory = new MockHttpClientFactory(_mockHttpMessageHandler);
        }

        public class MockHttpClientFactory : API.Helpers.IHttpClientFactory
        {
            private readonly MockHttpMessageHandler _mockHttpMessageHandler;

            public MockHttpClientFactory(MockHttpMessageHandler mockHttpMessageHandler)
            {
                _mockHttpMessageHandler = mockHttpMessageHandler;
            }
            public HttpClient CreateClient(Options options)
            {
                return _mockHttpMessageHandler.ToHttpClient();

            }
        }

        #region "Unit tests"
        [Fact]
        public async Task HttpRequestBytesReturnShoulReturnEmpty()
        {
            var input = new Input { Method = Method.GET, Url = "http://localhost:5298/api", Headers = new Header[0], Message = "" };
            var options = new Options { ConnectionTimeoutSeconds = 60 };

            _mockHttpMessageHandler.When(input.Url)
                .Respond("application/octet-stream", String.Empty);

            var httpClient = _mockClientFactory.CreateClient(options);

            using (var responseMessage = await _service.GetHttpRequestResponseAsync(
                        httpClient,
                        input.Method.ToString(),
                        input.Url,
                        new StringContent(""),
                        new Dictionary<string, string>(),
                        options,
                        default)
                    .ConfigureAwait(false))
            {
                var response = new Models.HttpByteResponse()
                {
                    BodyBytes = await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false),
                };

                Assert.Equal(0, response.BodySizeInMegaBytes);
                Assert.Empty(response.BodyBytes);
            }
        }

        [Fact]
        public async Task HttpRequestBodyReturnShouldBeOfTypeString()
        {
            const string expectedReturn = "<foo>BAR</foo>";

            var input = new Input
            { Method = Method.GET, Url = "http://localhost:5298/", Headers = new Header[0], Message = "" };
            var options = new Options { ConnectionTimeoutSeconds = 60};

            _mockHttpMessageHandler.When(input.Url)
            .Respond("text/plain", expectedReturn);

            var httpClient = _mockClientFactory.CreateClient(options);

            using (var responseMessage = await _service.GetHttpRequestResponseAsync(
                        httpClient,
                        input.Method.ToString(),
                        input.Url,
                        new StringContent(""),
                        new Dictionary<string,string>(),
                        options,
                        default)
                    .ConfigureAwait(false))
                {   
                var response = new Models.HttpResponse()
                {
                    Body = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false),
                };

                expectedReturn.Should().Be(response.Body);
            }
        }
        #endregion

        #region "Functional tests"
        [Fact]
        public async Task ReturnOkForHealthCheck()
        {
            var api = new ApiWebApplicationFactory();

            using (var client = api.CreateClient())
            {
                var response = await client.GetAsync("http://localhost:5298/api/healthcheck");
                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);

            }
        }

        [Fact]
        public async Task HttpRequestBytesShouldReturnBinary()
        {
            var testFileUriPath = "https://github.githubassets.com/favicons/favicon.png";

            var options = new Options 
            { ConnectionTimeoutSeconds = 60, Url = testFileUriPath };

            var testFileUriLocalPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "MockData\\favicon.png");
            var actualFileBytes = File.ReadAllBytes(new Uri(testFileUriLocalPath).LocalPath);

            var api = new ApiWebApplicationFactory();

            using (var client = api.CreateClient())
            {
                var json = JsonConvert.SerializeObject(options);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var post = await client.PostAsync("http://localhost:5298/api/HttpRequestBytes", data);
                string result = post.Content.ReadAsStringAsync().Result;

                dynamic jToken = JToken.Parse(result);

                result.Should().NotBeNull();
                actualFileBytes.Should().BeEquivalentTo(Convert.FromBase64String((string)jToken.bodyBytes));
            }
        }
        #endregion
    }
}
