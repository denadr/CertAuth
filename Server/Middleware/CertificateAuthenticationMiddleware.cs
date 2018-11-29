using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Server.Middleware
{
    public class CertificateAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public CertificateAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var certificateHeader = context.Request.Headers["X-ARR-ClientCert"];

                var certificate = new X509Certificate2(Convert.FromBase64String(certificateHeader));

                if (ValidateCertificate(certificate))
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }

        private bool ValidateCertificate(X509Certificate2 certificate)
        {
            return true;
        }
    }

    public static class CertificateAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCertificateAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CertificateAuthenticationMiddleware>();
        }
    }
}
