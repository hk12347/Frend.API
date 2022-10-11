using Frend.API.Helpers;

namespace Frend.API.Models
{
    public class Input
    {
        /// <summary>
        /// The HTTP Method to be used with the request.
        /// </summary>
        public Method Method { get; set; }

        /// <summary>
        /// The URL with protocol and path. You can include query parameters directly in the url.
        /// e.g. https://example.org/path/to
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The message text to be sent with the request.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// List of HTTP headers to be added to the request.
        /// </summary>
        public Header[] Headers { get; set; }
    }
}
