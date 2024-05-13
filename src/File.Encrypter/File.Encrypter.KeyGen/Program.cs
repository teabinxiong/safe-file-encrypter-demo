using Cryptography.Lib;
using Cryptography.Lib.Extensions;
using Cryptography.Lib.Models;
using Cryptography.Lib.Utils;
using Microsoft.Extensions.Configuration;

var client = String.Empty;
var password = string.Empty;

Console.WriteLine("Starting Key Generation tool..");

try
{
    var builder = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", false, true)
        .AddJsonFile(@"D:\secrets\FileEncrypterDemo\secrets.json", true, true);

    IConfiguration configuration = builder.Build();

    var outputDir = configuration.GetSection("keyGenPath").Value;


    Console.WriteLine("Please enter client name:");
    client = Console.ReadLine();
    Console.WriteLine("Please enter password:");
    password = new System.Net.NetworkCredential(string.Empty, UtilHelper.GetPassword()).Password;

    Console.WriteLine();

    var rsa = new RsaWithXMLKey();
    rsa.GenerateNewKey($"{outputDir}\\public.xml", $"{outputDir}\\private.xml");

    var privateKeyData = PrivateKeyData.New(
       configuration,
       client,
       rsa.privateKeyXML
       ) ;

    rsa.OutputPrivateFile($"{outputDir}\\Private.key", privateKeyData);

    var publicKeyData =  PublicKeyData.New(
        configuration,
        client,
        password,
        rsa.publicKeyXML
        );

    rsa.OutputPublicFile($"{outputDir}\\Public.key", publicKeyData);

    Console.WriteLine("Key file generated");

}
catch (Exception ex)
{
    Console.WriteLine("Key generated failed!");
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.WriteLine("Press any key to quit...");
    Console.Read();
}

