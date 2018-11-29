using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var certificate = GetCertificate();

            using (var httpHandler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                UseProxy = false
            })
            {
                httpHandler.ServerCertificateCustomValidationCallback = (req, cert, chain, err) => true;
                httpHandler.ClientCertificates.Add(certificate);

                using (var client = new HttpClient(httpHandler)
                {
                    BaseAddress = new Uri("https://localhost:44368")
                })
                {
                    using (var response = client.GetAsync("/api/values").Result)
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }

        static X509Certificate GetCertificate()
        {
            X509Store certificateStore = null;
            try
            {
                certificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine, OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                var certificates = certificateStore.Certificates.Find(X509FindType.FindBySerialNumber, "3a6cc87f931ceda2422abc0d6e8edba0", true);

                var certificate = certificates.Count == 0 ? null : certificates[0];

                return certificate;
            }
            finally
            {
                certificateStore?.Close();
            }
        }
    }
}
