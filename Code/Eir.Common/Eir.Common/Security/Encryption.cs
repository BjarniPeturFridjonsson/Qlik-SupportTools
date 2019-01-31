using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Eir.Common.Security
{


    /// <summary>
    /// Wrapper for the AES algorithm
    /// Make sure that you store your key :)
    /// <para>This uses the default ciphers and padding which are good today (2015-05-01) and are updated by
    /// microsoft as the framework moves forward, keeping it safe for future use.</para>
    /// </summary>
    public class Encryption
    {
        private const int SALT_LEN = 32;

        /// <summary>
        /// Encrypts the plainText input using the given Key.
        /// A random salt and vector is generated and prepended to the encrypted text.
        /// <para>This uses the default ciphers and padding which are good today (2015-05-01) and are updated by
        /// microsoft as the framework moves forward, keeping it safe for future use.</para>
        /// </summary>
        /// <param name="stringToEncrypt">Plain text to encrypt</param>
        /// <param name="encryptionKey">The encryption key</param>
        /// <returns>The ciphertext and salt in Base64</returns>
        public string Encrypt(string stringToEncrypt, string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));

            if (string.IsNullOrEmpty(stringToEncrypt))
                throw new ArgumentNullException(nameof(stringToEncrypt));

            
            using (var derive = new Rfc2898DeriveBytes(encryptionKey, SALT_LEN))
            {
                var salt = derive.Salt;
                var rgbKey = derive.GetBytes(32);
                var rgbVector = derive.GetBytes(16);

                
                using (var aesManaged = new AesManaged())
                using (var encryptor = aesManaged.CreateEncryptor(rgbKey, rgbVector))
                using (var memoryStream = new MemoryStream())
                {
                    using (var stream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(stringToEncrypt);
                    }

                    // Get the encrypted text
                    var cipherBytes = memoryStream.ToArray();
                    Array.Resize(ref salt, salt.Length + cipherBytes.Length);
                    Array.Copy(cipherBytes, 0, salt, SALT_LEN, cipherBytes.Length);

                    return Convert.ToBase64String(salt);
                }
            }
        }

        /// <summary>
        /// Decrypts AES encrypted text
        /// </summary>
        /// <param name="stringToDecrypt">The text to decrypt</param>
        /// <param name="encryptionKey">The encryption key</param>
        /// <returns>decrypted text.</returns>
        public string Decrypt(string stringToDecrypt, string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentNullException(nameof(encryptionKey));

            if (string.IsNullOrEmpty(stringToDecrypt))
                throw new ArgumentNullException(nameof(stringToDecrypt));
          

            
            var byteString = Convert.FromBase64String(stringToDecrypt);
            var saltBytes = byteString.Take(SALT_LEN).ToArray();
            var ciphertextBytes = byteString.Skip(SALT_LEN).Take(byteString.Length - SALT_LEN).ToArray();

            using (var val = new Rfc2898DeriveBytes(encryptionKey, saltBytes))
            {
                // Derive vector 
                var rgbKey = val.GetBytes(32);
                var rgbVector = val.GetBytes(16);

                using (var aesManaged = new AesManaged())
                using (var decryptor = aesManaged.CreateDecryptor(rgbKey, rgbVector))
                {
                    using (var memoryStream = new MemoryStream(ciphertextBytes))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}

