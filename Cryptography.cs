using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AlgoModSimpleWPF
{
    internal class Cryptography
    {
        /// <summary>
        /// Crypts obfuscated IDs into plain text
        /// </summary>
        /// <param name="encryptedText">Text to be decrypted</param>
        /// <returns>Returns plain text</returns>
        public static string DecryptIDS(string encryptedText)
        {
            string decryptedText = string.Empty;

            try
            {
                string key = SensitiveData.IDsKey;
                int ki = 0;

                // Processes text until it is all parsed
                while (!string.IsNullOrEmpty(encryptedText))
                {
                    if (ki >= key.Length)
                    {
                        ki = 0;
                    }

                    // Gets current key value and adds one to account for index
                    int currentKey = int.Parse(key[ki].ToString()) + 1;

                    // Removes chars from the beginning of text based on the key int
                    encryptedText = encryptedText[currentKey..];

                    // Adds the first char of remaining string to output
                    decryptedText += encryptedText[0].ToString();

                    // Removes the first char it just added to output
                    encryptedText = encryptedText[1..];

                    ki++;
                }
            }
            catch (Exception ex)
            {
                MethodsPopup.Popup(1, "Fatal Error!", $"Error DecryptIDS: {ex.Message}", "Exit");
            }

            return decryptedText;
        }


        /// <summary>
        /// Encrypts the message the client is sending to the server
        /// </summary>
        /// <param name="input">Client's message to server</param>
        /// <returns>Returns encrypted message</returns>
        /// <remarks>If the message is "INFO", don't encrypt because sensitive data hasn't been recieved</remarks>
        public static string EncryptMessage(string input)
        {
            // Ensures input isn't empty
            if (input.Trim() != string.Empty)
            {
                // Ensures the client isn't asking for info (check remarks)
                if (input != "INFO")
                {
                    try
                    {
                        // Creates a new AES algorithm
                        using Aes aesAlg = Aes.Create();

                        // Encode Key and IV into byte arrays
                        byte[] key = Encoding.ASCII.GetBytes(SensitiveData.MessageKey);
                        byte[] IV = Encoding.ASCII.GetBytes(SensitiveData.MessageIV);

                        // Set crypto transform of algorithm using encoded byte arrays
                        ICryptoTransform encryptor = aesAlg.CreateEncryptor(key, IV);

                        // Builds steam writer to encrypt input
                        using MemoryStream memoryStream = new();
                        using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
                        using StreamWriter streamWriter = new(cryptoStream);

                        // Writes input as encrypted text to memory stream
                        streamWriter.Write(input);

                        // Return encrypted message as string
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                    catch (Exception ex)
                    {
                        MethodsPopup.Popup(1, "Fatal Error!", $"Error EncryptMessage: {ex.Message}", "Exit");
                    }
                }
            }
            else
            {
                TabMore.Log("Input is null");
            }

            // Return plain text input if encryption failed or key isn't obtainable
            return input;
        }
    }
}
