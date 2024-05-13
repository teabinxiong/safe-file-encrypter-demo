namespace Cryptography.Lib.Models
{

    public class EncryptedFile
    {
        public string Client { get; set; }
        public string Filename { get; set; }
        public byte[] EncryptedSesssionKey { get; set; }
        public byte[] IV { get; set; }
        public byte[] EncryptedData { get; set; }
        public string Signature { get; set; }
    }
}
