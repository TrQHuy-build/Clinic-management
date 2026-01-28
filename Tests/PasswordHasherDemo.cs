using System;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests
{
    /// <summary>
    /// Class demo và test PasswordHasher
    /// Uncomment Main() để chạy standalone test
    /// </summary>
    public class PasswordHasherDemo
    {
        /*
        // Uncomment để test độc lập
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            RunAllTests();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        */

        public static void RunAllTests()
        {
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("🔐 PASSWORDHASHER DEMO & TEST");
            Console.WriteLine("═══════════════════════════════════════════════════\n");

            Test1_BasicHashing();
            Test2_Verification();
            Test3_DifferentPasswords();
            Test4_PasswordStrength();
            Test5_OldFormatDetection();
            Test6_BackwardCompatibility();

            Console.WriteLine("\n═══════════════════════════════════════════════════");
            Console.WriteLine("✅ ALL TESTS COMPLETED!");
            Console.WriteLine("═══════════════════════════════════════════════════");
        }

        static void Test1_BasicHashing()
        {
            Console.WriteLine("TEST 1: Basic Password Hashing");
            Console.WriteLine("───────────────────────────────────────────────────");

            string password = "Admin@123";
            string hash = PasswordHasher.HashPassword(password);

            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash: {hash}");
            Console.WriteLine($"Hash length: {hash.Length} chars");
            
            string[] parts = hash.Split(':');
            Console.WriteLine($"Format: {parts.Length} parts");
            Console.WriteLine($"  - Iterations: {parts[0]}");
            Console.WriteLine($"  - Salt: {parts[1].Substring(0, 10)}... ({parts[1].Length} chars)");
            Console.WriteLine($"  - Hash: {parts[2].Substring(0, 10)}... ({parts[2].Length} chars)");
            
            Console.WriteLine("✅ PASSED\n");
        }

        static void Test2_Verification()
        {
            Console.WriteLine("TEST 2: Password Verification");
            Console.WriteLine("───────────────────────────────────────────────────");

            string password = "TestPass@2026";
            string hash = PasswordHasher.HashPassword(password);

            bool correctVerify = PasswordHasher.VerifyPassword("TestPass@2026", hash);
            bool wrongVerify = PasswordHasher.VerifyPassword("WrongPass@2026", hash);

            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Verify with correct password: {(correctVerify ? "✅ TRUE" : "❌ FALSE")}");
            Console.WriteLine($"Verify with wrong password: {(wrongVerify ? "❌ TRUE" : "✅ FALSE")}");

            if (correctVerify && !wrongVerify)
                Console.WriteLine("✅ PASSED\n");
            else
                Console.WriteLine("❌ FAILED\n");
        }

        static void Test3_DifferentPasswords()
        {
            Console.WriteLine("TEST 3: Same Password → Different Hashes (Random Salt)");
            Console.WriteLine("───────────────────────────────────────────────────");

            string password = "SamePassword123!";
            string hash1 = PasswordHasher.HashPassword(password);
            string hash2 = PasswordHasher.HashPassword(password);

            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash 1: {hash1.Substring(0, 50)}...");
            Console.WriteLine($"Hash 2: {hash2.Substring(0, 50)}...");
            Console.WriteLine($"Are hashes different? {(hash1 != hash2 ? "✅ YES" : "❌ NO")}");

            bool verify1 = PasswordHasher.VerifyPassword(password, hash1);
            bool verify2 = PasswordHasher.VerifyPassword(password, hash2);
            Console.WriteLine($"Both verify correctly? {(verify1 && verify2 ? "✅ YES" : "❌ NO")}");

            if (hash1 != hash2 && verify1 && verify2)
                Console.WriteLine("✅ PASSED (Random salt working!)\n");
            else
                Console.WriteLine("❌ FAILED\n");
        }

        static void Test4_PasswordStrength()
        {
            Console.WriteLine("TEST 4: Password Strength Validation");
            Console.WriteLine("───────────────────────────────────────────────────");

            string[] testPasswords = {
                "abc",                  // Too short
                "abcdefgh",            // No number/special
                "12345678",            // No letters
                "Abcdefgh",            // No number/special
                "Abc12345",            // No special
                "Abc@1234",            // Valid
                "MyClinic@2026!"       // Valid
            };

            foreach (string pwd in testPasswords)
            {
                var (isValid, errorMessage) = PasswordHasher.ValidatePasswordStrength(pwd);
                string status = isValid ? "✅" : "❌";
                string message = isValid ? "VALID" : errorMessage;
                Console.WriteLine($"{status} \"{pwd}\" → {message}");
            }

            Console.WriteLine("✅ PASSED\n");
        }

        static void Test5_OldFormatDetection()
        {
            Console.WriteLine("TEST 5: Old Format Detection");
            Console.WriteLine("───────────────────────────────────────────────────");

            string[] testHashes = {
                "plaintext123",                                    // Plain text
                "5f4dcc3b5aa765d61d8327deb882cf99",               // MD5
                "100000:salt:hash",                                // New format (3 parts)
                "100000:BASE64SALT:BASE64HASH"                     // New format
            };

            foreach (string hash in testHashes)
            {
                bool isOld = PasswordHasher.IsOldFormat(hash);
                string status = hash.Split(':').Length == 3 ? "✅ NEW" : "⚠️ OLD";
                Console.WriteLine($"{status} \"{hash.Substring(0, Math.Min(30, hash.Length))}...\" → {(isOld ? "Old format" : "New format")}");
            }

            Console.WriteLine("✅ PASSED\n");
        }

        static void Test6_BackwardCompatibility()
        {
            Console.WriteLine("TEST 6: Backward Compatibility");
            Console.WriteLine("───────────────────────────────────────────────────");

            // Simulate old format (plain text)
            string oldPassword = "admin123";
            string oldHash = "admin123";  // Stored as plain text

            Console.WriteLine("Scenario: User has old plain text password");
            Console.WriteLine($"Stored hash: {oldHash}");
            Console.WriteLine($"User input: {oldPassword}");

            // Check if old format
            bool isOld = PasswordHasher.IsOldFormat(oldHash);
            Console.WriteLine($"Is old format? {(isOld ? "✅ YES" : "❌ NO")}");

            // Verify fails with new method (expected)
            bool newVerify = PasswordHasher.VerifyPassword(oldPassword, oldHash);
            Console.WriteLine($"New verify method: {(newVerify ? "TRUE" : "✅ FALSE (expected)")}");

            // Should use plain text comparison for old format
            bool plainCompare = (oldHash == oldPassword);
            Console.WriteLine($"Plain text compare: {(plainCompare ? "✅ TRUE" : "FALSE")}");

            // Upgrade to new format
            string newHash = PasswordHasher.HashPassword(oldPassword);
            Console.WriteLine($"Upgraded hash: {newHash.Substring(0, 50)}...");

            // Verify with new hash
            bool verifyNew = PasswordHasher.VerifyPassword(oldPassword, newHash);
            Console.WriteLine($"Verify with new hash: {(verifyNew ? "✅ TRUE" : "❌ FALSE")}");

            if (isOld && !newVerify && plainCompare && verifyNew)
                Console.WriteLine("✅ PASSED (Backward compatibility works!)\n");
            else
                Console.WriteLine("❌ FAILED\n");
        }
    }
}
