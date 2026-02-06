using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SteamHub.Helper
{
    public static class EncryptionHelper
    {
        private static readonly byte[] AesKey = Encoding.UTF8.GetBytes("39F671BDEEC80A38FE42E77EF946F55B");

        /// <summary>
        /// AES 加密
        /// </summary>
        public static byte[] AesEncrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = AesKey;
                aes.GenerateIV();
                var iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(iv, 0, iv.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        public static string AesDecrypt(byte[] cipherText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = AesKey;

                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    var iv = new byte[aes.BlockSize / 8];
                    msDecrypt.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}