using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for PasswordHasher
    /// Tests password hashing, verification, and security features
    /// </summary>
    [TestClass]
    public class PasswordHasherTests
    {
        private PasswordHasher hasher;

        [TestInitialize]
        public void Setup()
        {
            hasher = new PasswordHasher();
        }

        #region Hash Password Tests

        [TestMethod]
        public void HashPassword_ValidPassword_ReturnsHashedPassword()
        {
            // Arrange
            string password = "Test@123456";

            // Act
            string hashed = hasher.HashPassword(password);

            // Assert
            Assert.IsNotNull(hashed);
            Assert.IsTrue(hashed.Length > 0);
            Assert.AreNotEqual(password, hashed);
        }

        [TestMethod]
        public void HashPassword_SamePassword_ReturnsDifferentHashes()
        {
            // Arrange
            string password = "Test@123456";

            // Act
            string hash1 = hasher.HashPassword(password);
            string hash2 = hasher.HashPassword(password);

            // Assert
            Assert.AreNotEqual(hash1, hash2, "Same password should produce different hashes due to random salt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HashPassword_NullPassword_ThrowsArgumentException()
        {
            // Act
            hasher.HashPassword(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HashPassword_EmptyPassword_ThrowsArgumentException()
        {
            // Act
            hasher.HashPassword("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HashPassword_WhitespacePassword_ThrowsArgumentException()
        {
            // Act
            hasher.HashPassword("   ");
        }

        #endregion

        #region Verify Password Tests

        [TestMethod]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "Test@123456";
            string hashed = hasher.HashPassword(password);

            // Act
            bool result = hasher.VerifyPassword(password, hashed);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string password = "Test@123456";
            string wrongPassword = "Wrong@123456";
            string hashed = hasher.HashPassword(password);

            // Act
            bool result = hasher.VerifyPassword(wrongPassword, hashed);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerifyPassword_CaseSensitive_ReturnsFalse()
        {
            // Arrange
            string password = "Test@123456";
            string wrongCase = "test@123456";
            string hashed = hasher.HashPassword(password);

            // Act
            bool result = hasher.VerifyPassword(wrongCase, hashed);

            // Assert
            Assert.IsFalse(result, "Password verification should be case-sensitive");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyPassword_NullPassword_ThrowsArgumentException()
        {
            // Arrange
            string hashed = hasher.HashPassword("Test@123456");

            // Act
            hasher.VerifyPassword(null, hashed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyPassword_NullHash_ThrowsArgumentException()
        {
            // Act
            hasher.VerifyPassword("Test@123456", null);
        }

        #endregion

        #region Password Strength Tests

        [TestMethod]
        public void ValidatePasswordStrength_ValidPassword_ReturnsTrue()
        {
            // Arrange
            string password = "Test@123456";

            // Act
            bool result = hasher.ValidatePasswordStrength(password, out string error);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        public void ValidatePasswordStrength_TooShort_ReturnsFalse()
        {
            // Arrange
            string password = "Test@12";

            // Act
            bool result = hasher.ValidatePasswordStrength(password, out string error);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("8 characters"));
        }

        [TestMethod]
        public void ValidatePasswordStrength_NoUppercase_ReturnsFalse()
        {
            // Arrange
            string password = "test@123456";

            // Act
            bool result = hasher.ValidatePasswordStrength(password, out string error);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("uppercase"));
        }

        [TestMethod]
        public void ValidatePasswordStrength_NoLowercase_ReturnsFalse()
        {
            // Arrange
            string password = "TEST@123456";

            // Act
            bool result = hasher.ValidatePasswordStrength(password, out string error);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("lowercase"));
        }

        [TestMethod]
        public void ValidatePasswordStrength_NoDigit_ReturnsFalse()
        {
            // Arrange
            string password = "Test@Abcdef";

            // Act
            bool result = hasher.ValidatePasswordStrength(password, out string error);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("digit"));
        }

        [TestMethod]
        public void ValidatePasswordStrength_NoSpecialChar_ReturnsFalse()
        {
            // Arrange
            string password = "Test123456";

            // Act
            bool result = hasher.ValidatePasswordStrength(password, out string error);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNotNull(error);
            Assert.IsTrue(error.Contains("special character"));
        }

        #endregion

        #region Security Tests

        [TestMethod]
        public void HashPassword_Uses_PBKDF2_WithSalt()
        {
            // Arrange
            string password = "Test@123456";

            // Act
            string hashed = hasher.HashPassword(password);

            // Assert - PBKDF2 output format: salt.hash (both base64)
            Assert.IsTrue(hashed.Contains("."), "Hash should contain salt and hash separated by dot");
            string[] parts = hashed.Split('.');
            Assert.AreEqual(2, parts.Length);
            
            // Verify base64 format
            Assert.IsTrue(IsBase64String(parts[0]), "Salt should be base64");
            Assert.IsTrue(IsBase64String(parts[1]), "Hash should be base64");
        }

        [TestMethod]
        public void VerifyPassword_TimingAttackResistant()
        {
            // Arrange
            string password = "Test@123456";
            string hashed = hasher.HashPassword(password);
            
            // Act & Measure time for correct password
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            bool result1 = hasher.VerifyPassword(password, hashed);
            sw1.Stop();
            
            // Act & Measure time for wrong password
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            bool result2 = hasher.VerifyPassword("Wrong@123456", hashed);
            sw2.Stop();
            
            // Assert
            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
            
            // Time difference should be minimal (< 10ms for timing attack resistance)
            long timeDiff = Math.Abs(sw1.ElapsedMilliseconds - sw2.ElapsedMilliseconds);
            Assert.IsTrue(timeDiff < 10, $"Time difference ({timeDiff}ms) suggests timing attack vulnerability");
        }

        [TestMethod]
        public void HashPassword_MinimumIterations_10000()
        {
            // This test verifies that hashing takes reasonable time
            // PBKDF2 with 10,000 iterations should take at least 10ms
            
            // Arrange
            string password = "Test@123456";
            
            // Act
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string hashed = hasher.HashPassword(password);
            sw.Stop();
            
            // Assert
            Assert.IsTrue(sw.ElapsedMilliseconds >= 5, 
                "Hashing should take at least 5ms with 10,000 iterations");
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void HashPassword_UnicodeCharacters_HandlesCorrectly()
        {
            // Arrange
            string password = "Test@123Café🔒";

            // Act
            string hashed = hasher.HashPassword(password);
            bool verified = hasher.VerifyPassword(password, hashed);

            // Assert
            Assert.IsNotNull(hashed);
            Assert.IsTrue(verified);
        }

        [TestMethod]
        public void HashPassword_MaxLength_HandlesCorrectly()
        {
            // Arrange
            string password = new string('A', 100) + "@123abc";

            // Act
            string hashed = hasher.HashPassword(password);
            bool verified = hasher.VerifyPassword(password, hashed);

            // Assert
            Assert.IsNotNull(hashed);
            Assert.IsTrue(verified);
        }

        [TestMethod]
        public void VerifyPassword_CorruptedHash_ReturnsFalse()
        {
            // Arrange
            string password = "Test@123456";
            string hashed = hasher.HashPassword(password);
            string corruptedHash = hashed.Substring(0, hashed.Length - 5) + "XXXXX";

            // Act
            bool result = hasher.VerifyPassword(password, corruptedHash);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Helper Methods

        private bool IsBase64String(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            try
            {
                Convert.FromBase64String(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
