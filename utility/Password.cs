using System.Security.Cryptography;

namespace PeyulErp.Utility
{
    public class Password
    {
        public static string HashPassword(string password)
        {
            var sha = SHA256.Create();
            var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hash);
        }

        public static string GenerateRandomPassword(int length = 8)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++) stringChars[i] = chars[random.Next(chars.Length)];

            return new string(stringChars);
        }
    }
}