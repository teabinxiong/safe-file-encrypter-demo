namespace Cryptography.Lib.Models
{

    public class EncryptedFile
    {
        public string client { get; set; }
        public byte[] encryptedSesssionKey { get; set; }
        public byte[] IV { get; set; }
        public byte[] encryptedData { get; set; }
        public string Signature { get; set; }
    }
}
