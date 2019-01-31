using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SenseApiLibrary
{
    internal class CertSupport
    {
        private const string QV_CERT_EXTENSION_OID = "1.3.6.1.5.5.7.13.3";
        private const StoreLocation CLIENT_STORE_LOCATION = StoreLocation.CurrentUser;
        private const StoreName CLIENT_STORE_NAME = StoreName.My;
        private const string CLIENT_SUBJECT = "CN=QlikClient";

        public IEnumerable<X509Certificate2> GetAllSenseClientCertificates()
        {
            X509Store store = null;

            try
            {
                store = new X509Store(CLIENT_STORE_NAME, CLIENT_STORE_LOCATION);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                return store.Certificates
                    .Find(X509FindType.FindByExtension, QV_CERT_EXTENSION_OID, true)
                    .OfType<X509Certificate2>()
                    .Where(cert => cert.Subject == CLIENT_SUBJECT)
                    .ToArray();
            }
            catch (Exception)
            {
                return new X509Certificate2[0];
            }
            finally
            {
                store?.Close();
            }
        }
    }
}