
using Cryptography.Lib.Models;
using Cryptography.Lib.Utils;
using Microsoft.Extensions.Configuration;
using NPOI.POIFS.Crypt.Agile;
using System.Text.Json;
using System.Text;
using System.IO;




var builder = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json", true, true)
    .AddJsonFile(@"D:\secrets\FileEncrypterDemo\secrets.json", true, true);
IConfiguration configuration = builder.Build();

var outputDir = configuration.GetSection("encryptedFilePath").Value;
var keyGenDir = configuration.GetSection("keyGenPath").Value;

string? filePath = string.Empty;
Console.WriteLine("Please enter the path to the file you wish to encrypt.");
filePath = Console.ReadLine();
Console.WriteLine();

string keyPath = keyGenDir + @"\Private.key";
Console.WriteLine("Starting encyption tool..");

try
{
    var rsa = new RsaWithXMLKey();
    var privateKeyData = rsa.EncryptedFileReadFromFile<PrivateKeyData>(keyPath);
    var client = privateKeyData.Client;
    rsa.privateKeyXML = privateKeyData.KeyXML;
    var expirydatestring = privateKeyData.ExpiryDate;
    var iv = privateKeyData.Iv;

    var aes = new AesEncryption();
    byte[] session = aes.GenerateRandomNumber(32);
    byte[] IV = aes.GenerateRandomNumber(16);
    byte[] encryptedSession = rsa.EncryptData(session);

    // read from file
    var filename = Path.GetFileName(filePath);
    string fileContents = File.ReadAllText(filePath);


    byte[] data = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(fileContents, new JsonSerializerOptions { WriteIndented = true }));
    var encryptedData = aes.Encrypt(data, session, IV);

    Console.WriteLine("Writing Encrypted File..");

    var encryptedFile = new EncryptedFile();
    encryptedFile.Filename = filename;
    encryptedFile.Client = client;
    encryptedFile.IV = IV;
    encryptedFile.EncryptedSesssionKey = encryptedSession;
    encryptedFile.EncryptedData = encryptedData;
    encryptedFile.Signature = configuration.GetSection("signature").Value;
    rsa.PublicKeyData($"{outputDir}//{Path.GetFileNameWithoutExtension(filePath)}.enc", encryptedFile);
    Console.WriteLine("Successfully encrypted!");
}
catch (Exception ex)
{
    Console.WriteLine("Failed to encrypt file!");
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.WriteLine("Press any key to quit...");
    Console.Read();
}