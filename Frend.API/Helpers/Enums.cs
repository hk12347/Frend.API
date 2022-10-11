namespace Frend.API.Helpers
{
    public enum Method
    {
        GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS, CONNECT
    }

    /// <summary>
    /// Allowed methods for sending content
    /// </summary>
    public enum SendMethod
    {
        POST, PUT, PATCH, DELETE
    }

    public enum Authentication
    {
        None, Basic, WindowsAuthentication, WindowsIntegratedSecurity, OAuth, ClientCertificate
    }

    public enum CertificateSource
    {
        CertificateStore,
        File,
        String
    }
}