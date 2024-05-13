using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Cryptography.Lib.Extensions;
using Cryptography.Lib.Utils;

namespace Cryptography.Lib.Models
{
    public sealed class PublicKeyData
    {
        public string Client { get; private set; }
        public string KeyXML { get; private set; }
        public string ExpiryDate { get; private set; }
        public string Checksum { get; private set; }
        public string Iv { get; private set; }

        public string Signature { get; private set; }

        private PublicKeyData()
        {
            
        }

        private PublicKeyData(string client, string keyXML, string expiryDate, string checksum, string iv, string signature)
        {
            this.Client = client;
            this.KeyXML = keyXML;
            this.ExpiryDate = expiryDate;
            this.Checksum = checksum;
            this.Iv = iv;
            this.Signature = signature;
        }

        public static PublicKeyData New(IConfiguration configuration, string client, string password, string privateKeyXML)
        {

            var expiryDate = DateTime.Now.AddMonths(Convert.ToInt32(configuration.GetSection("expiryPeriod").Value)).ToString("yyyy-MM-dd");
            var aes = new AesEncryption();
            var iv = aes.GenerateRandomNumber(24);

            var checksum = UtilHelper.Sha256Hash(client + expiryDate + iv + configuration.GetSection("secret").Value);

            var signature = UtilHelper.Sha256Hash(client + expiryDate + iv + password +configuration.GetSection("secret").Value);

            return new PublicKeyData(
                client,
                privateKeyXML,
                expiryDate,
                checksum,
                iv.BytesArrayToString(),
                signature
                );
        }
    }
}
