using Cryptography.Lib.Extensions;
using Cryptography.Lib.Utils;
using Microsoft.Extensions.Configuration;

namespace Cryptography.Lib.Models
{
    public class PrivateKeyData
    {
        public string Client { get;  set; }
        public string KeyXML { get;  set; }
        public string ExpiryDate { get;  set; }
        public string Iv { get;  set; }
        public string Checksum { get;  set; }

        private PrivateKeyData()
        {

        }

        private PrivateKeyData(string client, string keyXML, string expiryDate, string iv, string checksum)
        {
            this.Client = client;
            KeyXML = keyXML;
            this.ExpiryDate = expiryDate;
            this.Iv = iv;
            this.Checksum = checksum;
        }

        public static PrivateKeyData New(IConfiguration configuration, string? client, string privateKeyXML)
        {
            var expiryDate = DateTime.Now.AddMonths(Convert.ToInt32(configuration.GetSection("expiryPeriod").Value)).ToString("yyyy-MM-dd");
            var aes = new AesEncryption();
            var iv = aes.GenerateRandomNumber(24);
            return new PrivateKeyData(
                client,
                privateKeyXML,
                expiryDate,
                iv.BytesArrayToString(),
                UtilHelper.Sha256Hash(client + expiryDate + iv.BytesArrayToString() + configuration.GetSection("secret").Value)
                );
        }
    }
}
