using System;
using System.Security.Cryptography;
using System.Text;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Utility class để xử lý mã hóa mật khẩu an toàn
    /// Sử dụng PBKDF2 với salt - đó là best practice
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 20; // 160 bits
        private const int Iterations = 10000; // PBKDF2 iterations
        private const char Delimiter = ':'; // Tách biệt salt và hash

        /// <summary>
        /// Mã hóa mật khẩu với salt ngẫu nhiên
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Mật khẩu không được rỗng");

            // Kiểm tra độ dài
            if (password.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự");

            if (password.Length > 128)
                throw new ArgumentException("Mật khẩu không được vượt quá 128 ký tự");

            // Tạo salt ngẫu nhiên
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);

                // Mã hóa mật khẩu với salt
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] hashBytes = pbkdf2.GetBytes(HashSize);

                    // Kết hợp salt + hash và mã hóa Base64
                    byte[] combined = new byte[SaltSize + HashSize];
                    Buffer.BlockCopy(saltBytes, 0, combined, 0, SaltSize);
                    Buffer.BlockCopy(hashBytes, 0, combined, SaltSize, HashSize);

                    string hash = Convert.ToBase64String(combined);
                    return hash;
                }
            }
        }

        /// <summary>
        /// Xác minh mật khẩu nhập vào có khớp với hash không
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                // Giải mã Base64
                byte[] combined = Convert.FromBase64String(hash);

                // Tách salt từ combined
                byte[] saltBytes = new byte[SaltSize];
                Buffer.BlockCopy(combined, 0, saltBytes, 0, SaltSize);

                // Mã hóa mật khẩu nhập vào với salt cũ
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] hashBytes = pbkdf2.GetBytes(HashSize);

                    // So sánh hash
                    for (int i = 0; i < HashSize; i++)
                    {
                        if (combined[i + SaltSize] != hashBytes[i])
                            return false;
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}