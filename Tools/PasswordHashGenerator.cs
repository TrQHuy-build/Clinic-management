using System;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tools
{
    /// <summary>
    /// Console tool để generate password hash cho admin
    /// Sử dụng để tạo hash cho SQL scripts hoặc testing
    /// </summary>
    class PasswordHashGenerator
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("🔐 PASSWORD HASH GENERATOR TOOL");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Chọn chức năng:");
                Console.WriteLine("1. Generate hash cho password mới");
                Console.WriteLine("2. Verify password với hash");
                Console.WriteLine("3. Kiểm tra độ mạnh password");
                Console.WriteLine("0. Thoát");
                Console.Write("\nNhập lựa chọn: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        GenerateHash();
                        break;
                    case "2":
                        VerifyHash();
                        break;
                    case "3":
                        CheckStrength();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!\n");
                        break;
                }

                Console.WriteLine("\n───────────────────────────────────────────────────\n");
            }
        }

        static void GenerateHash()
        {
            Console.Write("Nhập password cần hash: ");
            string password = ReadPassword();
            Console.WriteLine();

            try
            {
                // Kiểm tra độ mạnh
                var (isValid, errorMessage) = PasswordHasher.ValidatePasswordStrength(password);
                if (!isValid)
                {
                    Console.WriteLine($"⚠️  WARNING: {errorMessage}");
                    Console.Write("Tiếp tục? (y/n): ");
                    if (Console.ReadLine()?.ToLower() != "y")
                        return;
                }

                string hash = PasswordHasher.HashPassword(password);
                
                Console.WriteLine("\n✅ Hash đã được tạo!");
                Console.WriteLine("\n📋 Sao chép hash dưới đây:");
                Console.WriteLine("─────────────────────────────────────────────");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(hash);
                Console.ResetColor();
                Console.WriteLine("─────────────────────────────────────────────");
                
                Console.WriteLine("\n📝 SQL UPDATE statement:");
                Console.WriteLine("─────────────────────────────────────────────");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"UPDATE UserAccount SET password_hash = '{hash}' WHERE user_id = ?;");
                Console.ResetColor();
                Console.WriteLine("─────────────────────────────────────────────");

                // Verify luôn
                bool verified = PasswordHasher.VerifyPassword(password, hash);
                Console.WriteLine($"\n🔍 Verification test: {(verified ? "✅ PASSED" : "❌ FAILED")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static void VerifyHash()
        {
            Console.Write("Nhập password: ");
            string password = ReadPassword();
            Console.WriteLine();

            Console.Write("Nhập hash: ");
            string hash = Console.ReadLine();

            try
            {
                bool isValid = PasswordHasher.VerifyPassword(password, hash);
                
                if (isValid)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✅ PASSWORD KHỚP!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n❌ PASSWORD KHÔNG KHỚP!");
                }
                Console.ResetColor();

                // Check format
                if (PasswordHasher.IsOldFormat(hash))
                {
                    Console.WriteLine("⚠️  Hash này là định dạng cũ (không an toàn)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static void CheckStrength()
        {
            Console.Write("Nhập password cần kiểm tra: ");
            string password = ReadPassword();
            Console.WriteLine();

            var (isValid, errorMessage) = PasswordHasher.ValidatePasswordStrength(password);
            
            Console.WriteLine("\n📊 KẾT QUẢ KIỂM TRA:");
            Console.WriteLine("─────────────────────────────────────────────");
            
            // Check length
            Console.Write($"Độ dài: {password.Length} ký tự ");
            if (password.Length >= 8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ (Tối thiểu 8)");
            }
            Console.ResetColor();

            // Check components
            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            PrintCheck("Chữ hoa (A-Z)", hasUpper);
            PrintCheck("Chữ thường (a-z)", hasLower);
            PrintCheck("Số (0-9)", hasDigit);
            PrintCheck("Ký tự đặc biệt (!@#$...)", hasSpecial);

            Console.WriteLine("─────────────────────────────────────────────");
            
            if (isValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ PASSWORD ĐỦ MẠNH!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ {errorMessage}");
            }
            Console.ResetColor();
        }

        static void PrintCheck(string label, bool passed)
        {
            Console.Write($"{label}: ");
            if (passed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌");
            }
            Console.ResetColor();
        }

        // Read password with masking (*)
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter && !char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password;
        }
    }
}
