using System;
using System.IO;
using System.Security.Cryptography;

namespace EncryptorDecryptor
{
    public static class EncryptDecrypt
    {
        public static string Run(string[] args)
        {
            var sourceFileName = args[0];
            var destinationFileName = args[1];

            byte[] key = null;
            if (args.Length == 3)
            {
                // If a decryption key was present in the arguments array, assign it to the corresponding variable.
                key = System.Convert.FromBase64String(args[2]);
            }

            if (key == null)
            {
                // If no key is present, the user must wish to encrypt.
                using (var sourceStream = File.OpenRead(sourceFileName))
                using (var destinationStream = File.Create(destinationFileName))
                using (var provider = new AesCryptoServiceProvider())
                using (var cryptoTransform = provider.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    destinationStream.Write(provider.IV, 0, provider.IV.Length);
                    sourceStream.CopyTo(cryptoStream);
                    Console.WriteLine("Encryption was successful.");
                    Console.WriteLine(System.Convert.ToBase64String(provider.Key));
                    return (System.Convert.ToBase64String(provider.Key));
                }
            }
            else
            {
                // If a key IS present, decrypt the source file and write it to the destination file.
                using (var sourceStream = File.OpenRead(sourceFileName))
                using (var destinationStream = File.Create(destinationFileName))
                using (var provider = new AesCryptoServiceProvider())
                {
                    var IV = new byte[provider.IV.Length];
                    sourceStream.Read(IV, 0, IV.Length);
                    using (var cryptoTransform = provider.CreateDecryptor(key, IV))
                    using (var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(destinationStream);
                        Console.WriteLine("Decryption was successful.");
                        return null;
                    }
                }
            }
        }
    }
}
