using System;
using System.Security.Cryptography;
using System.Text;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Class mã hóa mật khẩu sử dụng PBKDF2 với SHA256
    /// Chuẩn bảo mật cao, chống brute-force attacks
    /// </summary>
    public static class PasswordHasher
    {
        // Số vòng lặp - càng cao càng an toàn (nhưng chậm hơn)
        // 10000 là mức khuyến nghị của OWASP cho PBKDF2
        private const int ITERATIONS = 100000;
        
        // Độ dài salt (bytes)
        private const int SALT_SIZE = 32;
        
        // Độ dài hash (bytes)
        private const int HASH_SIZE = 32;

        /// <summary>
        /// Mã hóa password thành hash string an toàn
        /// Format: [iterations]:[salt]:[hash]
        /// </summary>
        /// <param name="password">Mật khẩu gốc</param>
        /// <returns>Chuỗi hash đã mã hóa</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Tạo salt ngẫu nhiên
            byte[] salt = new byte[SALT_SIZE];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash password với salt
            byte[] hash = HashPasswordWithSalt(password, salt, ITERATIONS);

            // Format: iterations:salt:hash (tất cả dùng Base64)
            return $"{ITERATIONS}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verify password có khớp với hash không
        /// </summary>
        /// <param name="password">Mật khẩu cần kiểm tra</param>
        /// <param name="hashedPassword">Hash đã lưu trong database</param>
        /// <returns>True nếu khớp, False nếu không</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                // Parse hash string
                string[] parts = hashedPassword.Split(':');
                if (parts.Length != 3)
                {
                    // Format cũ (plain text hoặc hash yếu) - return false để bắt đổi password
                    return false;
                }

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hash = Convert.FromBase64String(parts[2]);

                // Hash password đầu vào với cùng salt
                byte[] testHash = HashPasswordWithSalt(password, salt, iterations);

                // So sánh 2 hash (dùng constant-time comparison để chống timing attacks)
                return SlowEquals(hash, testHash);
            }
            catch (Exception)
            {
                // Nếu có lỗi parse (format cũ), return false
                return false;
            }
        }

        /// <summary>
        /// Hash password với salt sử dụng PBKDF2-SHA256
        /// </summary>
        private static byte[] HashPasswordWithSalt(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HASH_SIZE);
            }
        }

        /// <summary>
        /// So sánh 2 mảng byte theo constant-time (chống timing attacks)
        /// </summary>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            uint diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        /// <summary>
        /// Kiểm tra password có đủ mạnh không
        /// </summary>
        /// <param name="password">Password cần kiểm tra</param>
        /// <returns>Tuple (isValid, errorMessage)</returns>
        public static (bool isValid, string errorMessage) ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, "Mật khẩu không được để trống");

            if (password.Length < 8)
                return (false, "Mật khẩu phải có ít nhất 8 ký tự");

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            // Yêu cầu ít nhất 3/4 điều kiện
            int score = (hasUpper ? 1 : 0) + (hasLower ? 1 : 0) + (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);
            
            if (score < 3)
                return (false, "Mật khẩu phải chứa ít nhất 3 trong 4 loại: chữ hoa, chữ thường, số, ký tự đặc biệt");

            return (true, string.Empty);
        }

        /// <summary>
        /// Kiểm tra password có phải format cũ (plain text hoặc hash yếu) không
        /// </summary>
        public static bool IsOldFormat(string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                return true;

            string[] parts = hashedPassword.Split(':');
            return parts.Length != 3;
        }
    }
}
