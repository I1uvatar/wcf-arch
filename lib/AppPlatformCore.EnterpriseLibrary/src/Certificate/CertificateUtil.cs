using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Certificate
{
    /// <summary>
    /// A utility class which helps to retrieve an x509 certificate
    /// </summary>
    public class CertificateUtil
    {
        /// <summary>
        /// Get certificate
        /// </summary>
        /// <param name="name">Name of the cert store</param>
        /// <param name="location">Store location</param>
        /// <param name="subjectName">Cert subject name</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, string subjectName)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    if (cert.SubjectName.Name.ToLower().Contains(subjectName.ToLower()))
                    {
                        if (result != null)
                            throw new ApplicationException(string.Format("There is more than one certificate found for subject Name {0}", subjectName));

                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                }
                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    for (int i = 0; i < certificates.Count; i++)
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }

        /// <summary>
        /// Create cert from RAW dara
        /// </summary>
        /// <param name="rawData">Certificate data</param>
        /// <returns></returns>
        public static X509Certificate2 GetCertificateFromRawData(string rawData)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            return new X509Certificate2(encoding.GetBytes(rawData));
        }

        public static X509Certificate2 SelectCertificateGUI(bool allowCancel)
        {
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var collection = store.Certificates;
            var fcollection = collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(
                fcollection,
                "Izbira digitalnega potrdila",
                "Izberite digitalno potrdilo iz seznama za prikaz podatkov potrdila.",
                X509SelectionFlag.SingleSelection);
            if (scollection.Count != 1)
            {
                if (allowCancel)
                {
                    store.Close();
                    return null;
                }

                throw new Exception("Izbrati morate digitalno potrdilo.");
            }

            var cert = scollection[0];
            store.Close();
            return cert;
        }
    }
}