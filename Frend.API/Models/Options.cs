using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Frend.API.Helpers;

namespace Frend.API.Models
{
    public class Options
    {
        /// https://learn.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api#using-frombody
        /// Input parameters - 2nd [FromBody] doesn't work e.g. "[FromBody] Options options, [FromBody] Input input" 

        #region "Input parameters"

        /// <summary>
        /// The HTTP Method to be used with the request.
        /// </summary>
        public Method Method { get; set; } = Method.GET;

        /// <summary>
        /// The URL with protocol and path. You can include query parameters directly in the url.
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// The message text to be sent with the request.
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// List of HTTP headers to be added to the request.
        /// </summary>
        public Header[] Headers { get; set; } = Array.Empty<Header>();
        #endregion

        #region "Option parameters"

        /// <summary>
        /// Method of authenticating request
        /// </summary>
        public Authentication Authentication { get; set; } = Authentication.None;

        /// <summary>
        /// If WindowsAuthentication is selected you should use domain\username
        /// </summary>
        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        /// <summary>
        /// Bearer token to be used for request. Token will be added as Authorization header.
        /// </summary>
        public string Token { get; set; } = "";

        /// <summary>
        /// Specifies where the Client Certificate should be loaded from.
        /// </summary>

        public CertificateSource ClientCertificateSource { get; set; } = CertificateSource.CertificateStore;

        /// <summary>
        /// Path to the Client Certificate when using a file as the Certificate Source, pfx (pkcs12) files are recommended. For other supported formats, see
        /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2collection.import?view=netframework-4.7.1
        /// </summary>
        public string ClientCertificateFilePath { get; set; } = "";

        /// <summary>
        /// Client certificate bytes as a base64 encoded string when using a string as the Certificate Source , pfx (pkcs12) format is recommended. For other supported formates, see
        /// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2collection.import?view=netframework-4.7.1
        /// </summary>
        public string ClientCertificateInBase64 { get; set; } = "";

        /// <summary>
        /// Key phrase (password) to access the certificate data when using a string or file as the Certificate Source
        /// </summary>
        public string ClientCertificateKeyPhrase { get; set; } = "";

        /// <summary>
        /// Thumbprint for using client certificate authentication.
        /// </summary>
        public string CertificateThumbprint { get; set; } = "";

        /// <summary>
        /// Should the entire certificate chain be loaded from the certificate store and included in the request. Only valid when using Certificate Store as the Certificate Source 
        /// </summary>
        public bool LoadEntireChainForCertificate { get; set; } = true;

        /// <summary>
        /// Timeout in seconds to be used for the connection and operation.
        /// </summary>
        public int ConnectionTimeoutSeconds { get; set; } = 60;

        /// <summary>
        /// If FollowRedirects is set to false, all responses with an HTTP status code from 300 to 399 is returned to the application.
        /// </summary>
        public bool FollowRedirects { get; set; } = true;

        /// <summary>
        /// Do not throw an exception on certificate error.
        /// </summary>
        public bool AllowInvalidCertificate { get; set; } = true;

        /// <summary>
        /// Some Api's return faulty content-type charset header. This setting overrides the returned charset.
        /// </summary>
        public bool AllowInvalidResponseContentTypeCharSet { get; set; } = true;
        /// <summary>
        /// Throw exception if return code of request is not successfull
        /// </summary>
        public bool ThrowExceptionOnErrorResponse { get; set; } = true;

        /// <summary>
        /// If set to false, cookies must be handled manually. Defaults to true.
        /// </summary>
        public bool AutomaticCookieHandling { get; set; } = true;

        #endregion
    }
}
