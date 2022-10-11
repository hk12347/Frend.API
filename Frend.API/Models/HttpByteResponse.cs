using System.Net.Http.Headers;

namespace Frend.API.Models
{
    public class HttpByteResponse
    {
        public byte[] BodyBytes { get; set; }
        public double BodySizeInMegaBytes => Math.Round((BodyBytes?.Length / (1024 * 1024d) ?? 0), 3);
        public MediaTypeHeaderValue ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int StatusCode { get; set; }
    }
}
