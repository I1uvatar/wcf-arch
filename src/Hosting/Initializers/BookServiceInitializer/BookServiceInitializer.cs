using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace BookServiceInitializer
{
    public class BookServiceInitializer : MarshalByRefObject
    {
        private readonly string serviceIdentificatorName;

        public BookServiceInitializer(string serviceIdentificatorName)
        {
            this.serviceIdentificatorName = serviceIdentificatorName;
        }

        public void Initialize()
        {
            //TODO: initialize all ioc stuff here

            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
        }

        private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            if (sslpolicyerrors != SslPolicyErrors.None)
            {

                if ((sslpolicyerrors | SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    string chainStatus = string.Empty;
                    foreach (X509ChainStatus objChainStatus in chain.ChainStatus)
                    {
                        chainStatus += objChainStatus.Status.ToString() + " - " + objChainStatus.StatusInformation
                                       + "; ";
                    }

                  return false;
                }
            }

            if (certificate != null && (sslpolicyerrors | SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                // used for testing purposes (test service certificate does not match the actual WCF endpoint name.)
                if (certificate.Subject.Contains("somecert.si"))
                {
                    return true;
                }
            }

            return sslpolicyerrors == SslPolicyErrors.None;
        }
    }
}
