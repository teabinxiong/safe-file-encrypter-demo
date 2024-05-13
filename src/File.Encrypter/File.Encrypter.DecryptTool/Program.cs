using Cryptography.Lib;
using Cryptography.Lib.Models;
using Cryptography.Lib.Utils;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json;
using System.Text;
using Cryptography.Lib.Extensions;

string keyPath = @"D:\work\personal\file-ecrypter\keygen\Public.key";
string filePath = @"D:\work\personal\file-ecrypter\encryptedfiles\sample-file.enc";


var builder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile(@"D:\secrets\FileEncrypterDemo\secrets.json", true, true);
IConfiguration configuration = builder.Build();

Console.WriteLine("Starting decryption tool..");
var outputDir = configuration.GetSection("EncryptedFilePath").Value;


try { 
    var rsa = new RsaWithXMLKey();
    PublicKeyData publicKeyData = rsa.EncryptedFileReadFromFile<PublicKeyData>(keyPath);
    var vendor = publicKeyData.Client;
    rsa.publicKeyXML = publicKeyData.KeyXML;
    var expirydatestring = publicKeyData.ExpiryDate;

    var inClient = string.Empty;
    var inPassword = string.Empty;

    Console.WriteLine("Please enter password:");
    inPassword = new System.Net.NetworkCredential(string.Empty, UtilHelper.GetPassword()).Password;

    Console.WriteLine();

    #region "Expiry Date & Checksum validation"
    DateTime expirydate;
    if (!string.IsNullOrEmpty(expirydatestring))
    {
        if (!DateTime.TryParseExact(expirydatestring, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out expirydate))
        {
            throw new Exception("Invalid Expiry Date!");
        }
        else
        {
            if (expirydate < DateTime.Now)
            {
                throw new Exception("Key expired!");
            }
        }
    }
    else
    {
        throw new Exception("Expiry Date missing!");
    }

    // verify the checksum
    if (UtilHelper.Sha256Hash(publicKeyData.Client + publicKeyData.ExpiryDate + publicKeyData.Iv + configuration.GetSection("secret").Value) != publicKeyData.Checksum)
    {
        throw new Exception("Invalid checksum!");
    }


    // verify the signature
    if (UtilHelper.Sha256Hash(publicKeyData.Client + publicKeyData.ExpiryDate + publicKeyData.Iv + inPassword + configuration.GetSection("secret").Value) != publicKeyData.Signature)
    {
        throw new Exception("Invalid signature!");
    }


    #endregion

    EncryptedFile encryptedFile = rsa.EncryptedFileReadFromFile<EncryptedFile>(filePath);
    var encSesssion = encryptedFile.EncryptedSesssionKey;
    var encryptedData = encryptedFile.EncryptedData;
    var IV = encryptedFile.IV;
    var evendor = encryptedFile.Client;
    var signature = encryptedFile.Signature;

    #region "Signature validation"
    if (signature != configuration.GetSection("signature").Value)
    {
        throw new Exception("Invalid signature!");
    }
    #endregion


    var session = rsa.DecryptData(encSesssion);

    var aes = new AesEncryption();
    var dataInBytes = aes.Decrypt(encryptedData, session, IV);

   // var valueBytes = System.Convert.FromBase64String(dataInBytes.BytesArrayToString());
    var data = System.Text.Encoding.UTF8.GetString(dataInBytes);

    File.WriteAllText($"{outputDir}//" + encryptedFile.Filename, data.Trim('"')) ;

    Console.WriteLine("Decrypt successfully!");

}
catch (Exception ex)
{
    Console.WriteLine("Failed to decrypt file!");
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.WriteLine("Press any key to quit...");
    Console.Read();
}