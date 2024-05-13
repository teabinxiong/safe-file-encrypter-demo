using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace Cryptography.Lib.Utils
{
    public class RsaWithXMLKey
    {

        public string publicKeyXML { get; set; }
        public string privateKeyXML { get; set; }

        public void OutputPublicFile<PublicKeyData>(string filepath, PublicKeyData objectToWrite, bool append = false)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filepath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objectToWrite);
            }

        }

        public void OutputPrivateFile<PrivateKeyData>(string filePath, PrivateKeyData objectToWrite, bool append = false)
        {

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objectToWrite);

            }
        }


        public void GenerateNewKey(string publicKeyPath, string privateKeyPath)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                if (File.Exists(privateKeyPath))
                {
                    File.Delete(privateKeyPath);
                }

                if (File.Exists(publicKeyPath))
                {
                    File.Delete(publicKeyPath);
                }

                var publicKeyfolder = Path.GetDirectoryName(publicKeyPath);
                var privateKeyfolder = Path.GetDirectoryName(privateKeyPath);

                if (!Directory.Exists(publicKeyfolder))
                {
                    Directory.CreateDirectory(publicKeyfolder);
                }

                if (!Directory.Exists(privateKeyfolder))
                {
                    Directory.CreateDirectory(privateKeyfolder);
                }


                publicKeyXML = rsa.ToXmlString(true);
                privateKeyXML = rsa.ToXmlString(false);
            }
        }

        public byte[] EncryptData(byte[] dataToEncrypt)
        {
            byte[] cipherbytes;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(privateKeyXML);

                cipherbytes = rsa.Encrypt(dataToEncrypt, false);
            }

            return cipherbytes;
        }

        public byte[] DecryptData(byte[] dataToEncrypt)
        {
            byte[] plain;

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(publicKeyXML);

                plain = rsa.Decrypt(dataToEncrypt, false);
            }

            return plain;
        }


        public void PublicKeyData<EncryptedFile>(string filePath,
           EncryptedFile objectToWrite,
           bool append = false)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objectToWrite);
            }

        }

        public T EncryptedFileReadFromFile<T>(string filePath)
        {
            T p;

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                p = (T)serializer.Deserialize(file, typeof(T));
            }
            return p;

        }
    }
}
