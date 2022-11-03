using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;

namespace ZebuPairOrder.Helper
{
    public static class Helper
    {
        public static void LogExceptiontoConsole(Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }

        internal static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        internal static string GetBankNiftyExpiryScript(string scriptName)
        {
            var expiryDate = Preferences.Get("ExpiryDate", "");
            var expiryMonth = Preferences.Get("ExpiryMonth", "");
            var expiryYear = Preferences.Get("Year", "");
            if(expiryYear.Length==4)
                expiryYear = expiryYear.Substring(2, 2);
            
            return $"{ scriptName.Trim()}{expiryDate.Trim()}{expiryMonth.Trim().ToUpper()}{expiryYear.Trim()}";
        }

    }
}
