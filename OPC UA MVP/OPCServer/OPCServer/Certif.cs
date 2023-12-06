using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OPCServer
{
    public class Certif
    {
        public static X509Certificate2 GenerationDefoltSerteficate()
        {
            var certificateRequest = new CertificateRequest("cn=OPSServer", RSA.Create(2048), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DataEncipherment, false));
            certificateRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.3") }, false));
            return certificateRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(365));
        }
    }
}
