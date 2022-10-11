using Frend.API.Models;

namespace Frend.API.Helpers
{
    /// <summary>
    /// HttpClientFactory
    /// </summary>
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(Options options);
    }

    public class HttpClientFactory: IHttpClientFactory
    {
        public HttpClient CreateClient(Options options)
        {
            var handler = new HttpClientHandler();
            handler.SetHandlerSettingsBasedOnOptions(options);
            return new HttpClient(handler);
        }
    }
}