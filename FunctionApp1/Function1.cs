using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly); 
            log.LogInformation(certStore.Certificates.Count + "");
            foreach (var certStoreCertificate in certStore.Certificates)
            { 
                log.LogInformation(certStoreCertificate.Thumbprint + " " + certStoreCertificate.FriendlyName + " " + certStoreCertificate.HasPrivateKey);
            }
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                X509FindType.FindByThumbprint,
                "827C84F23137042D1CA90D05BB6C47511BF6FC28",
                false);
            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                X509Certificate2 cert = certCollection[0];
                // Use certificate
                log.LogInformation(" cert find " + certCollection[0].Thumbprint + " " + certCollection.Count);
                using (var csp = cert.PrivateKey)
                {
                    log.LogInformation(csp.KeySize + "");
                }
            }
            certStore.Close();
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
