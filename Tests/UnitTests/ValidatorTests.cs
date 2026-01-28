using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for Validator utility class
    /// Tests all validation methods
    /// </summary>
    [TestClass]
    public class ValidatorTests
    {
        #region Email Validation Tests

        [TestMethod]
        [DataRow("test@example.com", true)]
        [DataRow("user.name@example.co.uk", true)]
        [DataRow("user+tag@example.com", true)]
        [DataRow("test123@test-domain.com", true)]
        public void IsValidEmail_ValidEmails_ReturnsTrue(string email, bool expected)
        {
            // Act
            bool result = Validator.IsValidEmail(email);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("invalid", false)]
        [DataRow("@example.com", false)]
        [DataRow("test@", false)]
        [DataRow("test @example.com", false)]
        [DataRow("test..name@example.com", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsValidEmail_InvalidEmails_ReturnsFalse(string email, bool expected)
        {
            // Act
            bool result = Validator.IsValidEmail(email);

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Phone Validation Tests

        [TestMethod]
        [DataRow("0123456789", true)]
        [DataRow("0987654321", true)]
        [DataRow("+84123456789", true)]
        [DataRow("84123456789", true)]
        public void IsValidPhone_ValidPhones_ReturnsTrue(string phone, bool expected)
        {
            // Act
            bool result = Validator.IsValidPhone(phone);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("123", false)]
        [DataRow("abcdefghij", false)]
        [DataRow("012-345-6789", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsValidPhone_InvalidPhones_ReturnsFalse(string phone, bool expected)
        {
            // Act
            bool result = Validator.IsValidPhone(phone);

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Required Field Tests

        [TestMethod]
        public void IsRequired_NotEmpty_ReturnsTrue()
        {
            // Act
            bool result = Validator.IsRequired("test");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("   ")]
        public void IsRequired_EmptyOrNull_ReturnsFalse(string value)
        {
            // Act
            bool result = Validator.IsRequired(value);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Length Validation Tests

        [TestMethod]
        public void IsValidLength_WithinRange_ReturnsTrue()
        {
            // Act
            bool result = Validator.IsValidLength("test", 1, 10);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidLength_TooShort_ReturnsFalse()
        {
            // Act
            bool result = Validator.IsValidLength("ab", 3, 10);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidLength_TooLong_ReturnsFalse()
        {
            // Act
            bool result = Validator.IsValidLength("abcdefghijk", 1, 10);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidLength_ExactMin_ReturnsTrue()
        {
            // Act
            bool result = Validator.IsValidLength("abc", 3, 10);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidLength_ExactMax_ReturnsTrue()
        {
            // Act
            bool result = Validator.IsValidLength("abcdefghij", 1, 10);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region Numeric Validation Tests

        [TestMethod]
        [DataRow("123")]
        [DataRow("0")]
        [DataRow("999999")]
        public void IsNumeric_ValidNumbers_ReturnsTrue(string value)
        {
            // Act
            bool result = Validator.IsNumeric(value);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("abc")]
        [DataRow("12.34")]
        [DataRow("-123")]
        [DataRow("")]
        [DataRow(null)]
        public void IsNumeric_InvalidNumbers_ReturnsFalse(string value)
        {
            // Act
            bool result = Validator.IsNumeric(value);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Decimal Validation Tests

        [TestMethod]
        [DataRow("123", true)]
        [DataRow("123.45", true)]
        [DataRow("0.5", true)]
        [DataRow("-123.45", true)]
        public void IsDecimal_ValidDecimals_ReturnsTrue(string value, bool expected)
        {
            // Act
            bool result = Validator.IsDecimal(value);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("abc", false)]
        [DataRow("12.34.56", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsDecimal_InvalidDecimals_ReturnsFalse(string value, bool expected)
        {
            // Act
            bool result = Validator.IsDecimal(value);

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Date Validation Tests

        [TestMethod]
        public void IsValidDate_ValidDate_ReturnsTrue()
        {
            // Act
            bool result = Validator.IsValidDate("01/28/2026");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("31/02/2026")] // Invalid day
        [DataRow("13/01/2026")] // Invalid month
        [DataRow("abc")]
        [DataRow("")]
        [DataRow(null)]
        public void IsValidDate_InvalidDate_ReturnsFalse(string value)
        {
            // Act
            bool result = Validator.IsValidDate(value);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsDateInPast_PastDate_ReturnsTrue()
        {
            // Arrange
            DateTime pastDate = DateTime.Now.AddDays(-1);

            // Act
            bool result = Validator.IsDateInPast(pastDate);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsDateInPast_FutureDate_ReturnsFalse()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1);

            // Act
            bool result = Validator.IsDateInPast(futureDate);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsDateInFuture_FutureDate_ReturnsTrue()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1);

            // Act
            bool result = Validator.IsDateInFuture(futureDate);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region Age Validation Tests

        [TestMethod]
        public void IsValidAge_AdultAge_ReturnsTrue()
        {
            // Arrange
            DateTime birthDate = DateTime.Now.AddYears(-25);

            // Act
            bool result = Validator.IsValidAge(birthDate, 18, 100);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidAge_TooYoung_ReturnsFalse()
        {
            // Arrange
            DateTime birthDate = DateTime.Now.AddYears(-10);

            // Act
            bool result = Validator.IsValidAge(birthDate, 18, 100);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidAge_TooOld_ReturnsFalse()
        {
            // Arrange
            DateTime birthDate = DateTime.Now.AddYears(-150);

            // Act
            bool result = Validator.IsValidAge(birthDate, 18, 100);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Range Validation Tests

        [TestMethod]
        public void IsInRange_WithinRange_ReturnsTrue()
        {
            // Act
            bool result = Validator.IsInRange(5, 1, 10);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsInRange_BelowMin_ReturnsFalse()
        {
            // Act
            bool result = Validator.IsInRange(0, 1, 10);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsInRange_AboveMax_ReturnsFalse()
        {
            // Act
            bool result = Validator.IsInRange(11, 1, 10);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsInRange_ExactBoundaries_ReturnsTrue()
        {
            // Act
            bool resultMin = Validator.IsInRange(1, 1, 10);
            bool resultMax = Validator.IsInRange(10, 1, 10);

            // Assert
            Assert.IsTrue(resultMin);
            Assert.IsTrue(resultMax);
        }

        #endregion

        #region Special Characters Tests

        [TestMethod]
        [DataRow("Hello123", false)]
        [DataRow("Hello@123", true)]
        [DataRow("Test#Special", true)]
        [DataRow("No_Special$", true)]
        public void ContainsSpecialCharacter_VariousInputs_ReturnsExpected(string value, bool expected)
        {
            // Act
            bool result = Validator.ContainsSpecialCharacter(value);

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Combination Tests

        [TestMethod]
        public void ValidateUser_AllFieldsValid_ReturnsTrue()
        {
            // Arrange
            string username = "testuser";
            string email = "test@example.com";
            string phone = "0123456789";

            // Act
            bool usernameValid = Validator.IsRequired(username) && Validator.IsValidLength(username, 3, 50);
            bool emailValid = Validator.IsValidEmail(email);
            bool phoneValid = Validator.IsValidPhone(phone);

            // Assert
            Assert.IsTrue(usernameValid);
            Assert.IsTrue(emailValid);
            Assert.IsTrue(phoneValid);
        }

        #endregion
    }
}
