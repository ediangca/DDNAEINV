
using System;
using System.Security.Cryptography;
using System.Text;

namespace DDNAEINV.Helper
{
    public class PasswordHasher
    {
        private const int SaltSize = 16; // 128-bit
        private const int KeySize = 32; // 256-bit
        private const int Iterations = 10000; // Number of iterations

        public string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[SaltSize];
                rng.GetBytes(salt);

                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    var key = deriveBytes.GetBytes(KeySize);
                    var hashBytes = new byte[SaltSize + KeySize];
                    Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
                    Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                var key = deriveBytes.GetBytes(KeySize);
                for (int i = 0; i < KeySize; i++)
                {
                    if (hashBytes[SaltSize + i] != key[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string GenerateHash(string password)
        {
            // Example using SHA-256
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // Convert to hex string
            }
        }
        public bool VerifyHashPassword(string inputPassword, string storedHash)
        {
            // Hash the input password
            string inputHash = GenerateHash(inputPassword);

            // Compare the hashes
            return inputHash == storedHash;
        }

    }
}