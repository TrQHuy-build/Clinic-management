using System;
using System.Data;
using System.Data.SqlClient;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Class hỗ trợ migrate dữ liệu cũ (mã hóa mật khẩu)
    /// Chỉ chạy một lần lúc upgrade
    /// </summary>
    public static class MigrationHelper
    {
        /// <summary>
        /// Mã hóa tất cả mật khẩu trong database
        /// Chỉ chạy một lần!
        /// </summary>
        public static void MigratePasswordsToHash()
        {
            try
            {
                // Lấy tất cả user có mật khẩu plain text
                string query = "SELECT user_id, password_hash FROM UserAccount WHERE password_hash IS NOT NULL";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    Console.WriteLine("❌ Không có dữ liệu user để migrate");
                    return;
                }

                int successCount = 0;
                int failCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        int userId = Convert.ToInt32(row["user_id"]);
                        string plainPassword = row["password_hash"].ToString();

                        // Nếu đã là hash (dài > 100 và là base64), bỏ qua
                        if (IsAlreadyHashed(plainPassword))
                        {
                            Console.WriteLine($"✅ User {userId} đã được hash sẵn, bỏ qua");
                            continue;
                        }

                        // Mã hóa mật khẩu
                        string hashedPassword = PasswordHelper.HashPassword(plainPassword);

                        // Cập nhật vào database
                        string updateQuery = "UPDATE UserAccount SET password_hash = @hash WHERE user_id = @userId";
                        SqlParameter[] parameters = {
                            new SqlParameter("@hash", hashedPassword),
                            new SqlParameter("@userId", userId)
                        };

                        DatabaseHelper.ExecuteNonQuery(updateQuery, parameters);
                        successCount++;
                        Console.WriteLine($"✅ User {userId}: Mã hóa thành công");
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        Console.WriteLine($"❌ Lỗi user {row["user_id"]}: {ex.Message}");
                    }
                }

                Console.WriteLine($"\n✅ Hoàn tất migration:");
                Console.WriteLine($"   - Thành công: {successCount}");
                Console.WriteLine($"   - Lỗi: {failCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi migration: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra xem password đã được hash hay không
        /// </summary>
        private static bool IsAlreadyHashed(string password)
        {
            if (string.IsNullOrEmpty(password))
                return true;

            // Hash từ PBKDF2 sẽ dài khoảng 70-80 ký tự Base64
            if (password.Length > 60)
            {
                try
                {
                    // Thử decode Base64
                    byte[] data = Convert.FromBase64String(password);
                    // Nếu thành công và dài đúng, đó là hash
                    return data.Length > 30;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Reset password của một user về mặc định (cho admin)
        /// </summary>
        public static bool ResetUserPassword(int userId, string newPassword = "123456")
        {
            try
            {
                string hashedPassword = PasswordHelper.HashPassword(newPassword);
                string query = "UPDATE UserAccount SET password_hash = @hash WHERE user_id = @userId";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@hash", hashedPassword),
                    new SqlParameter("@userId", userId)
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}