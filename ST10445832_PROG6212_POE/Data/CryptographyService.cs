using ST10445832_PROG6212_POE.Models;
using System.Security.Cryptography;

namespace ST10445832_PROG6212_POE.Data
{
    public class CryptographyService
    {
        private static readonly byte[] _key = Convert.FromHexString("f7b9a3c4d2e1f5067890abcd1234567890fedcba0987654321abcdef01234567");

        //====================================================//
        // returns encrypted byte array and random IV
        public static FileEncryptionResponse EncryptFile(byte[] fileContent)
        {
            using (Aes aes = Aes.Create())
            {
                // set key
                aes.Key = _key;
                // generate random IV
                aes.GenerateIV();

                using (var memoryStream = new MemoryStream())
                {
                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(fileContent, 0, fileContent.Length);
                            cryptoStream.FlushFinalBlock();
                            var encryptedContent = memoryStream.ToArray();
                            return new FileEncryptionResponse { Content = encryptedContent, IV = aes.IV };
                        }
                    }
                }
            }
        }

        //=====================================================//
        // return decrypted byte array
        public static byte[] DecryptFile(byte[] encryptedFileContent, byte[] IV)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = IV;

                using (var memoryStream = new MemoryStream())
                {
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(encryptedFileContent, 0, encryptedFileContent.Length);
                            cryptoStream.FlushFinalBlock();
                            var decryptedContent = memoryStream.ToArray();
                            return decryptedContent;
                        }
                    }
                }
            }
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\