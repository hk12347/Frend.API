using Frend.API.Helpers;

namespace Frend.API.Models
{
    public class ByteInput
    {
        /// <summary>
        /// The HTTP Method to be used with the request.
        /// </summary>
        public SendMethod Method { get; set; }

        /// <summary>
        /// The URL with protocol and path. You can include query parameters directly in the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The content to send as byte array
        /// </summary>
        public byte[] ContentBytes { get; set; }

        /// <summary>
        /// List of HTTP headers to be added to the request.
        /// </summary>
        public Header[] Headers { get; set; }
    }
}
