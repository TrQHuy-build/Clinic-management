using System;
using System.Text.RegularExpressions;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Class hỗ trợ validate dữ liệu input
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại Việt Nam hợp lệ
        /// </summary>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove spaces and dashes
            phone = phone.Replace(" ", "").Replace("-", "");

            // Vietnamese phone: 10-11 digits, starts with 0
            string pattern = @"^0\d{9,10}$";
            return Regex.IsMatch(phone, pattern);
        }

        /// <summary>
        /// Kiểm tra chuỗi không rỗng
        /// </summary>
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Kiểm tra số dương
        /// </summary>
        public static bool IsPositiveNumber(decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Kiểm tra số nguyên dương
        /// </summary>
        public static bool IsPositiveInteger(int value)
        {
            return value > 0;
        }

        /// <summary>
        /// Kiểm tra độ dài chuỗi
        /// </summary>
        public static bool IsValidLength(string value, int minLength, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            int length = value.Trim().Length;
            return length >= minLength && length <= maxLength;
        }

        /// <summary>
        /// Kiểm tra ngày hợp lệ (không phải ngày tương lai)
        /// </summary>
        public static bool IsValidPastDate(DateTime date)
        {
            return date <= DateTime.Now;
        }

        /// <summary>
        /// Kiểm tra ngày sinh hợp lệ (từ 0-120 tuổi)
        /// </summary>
        public static bool IsValidDateOfBirth(DateTime date)
        {
            int age = DateTime.Now.Year - date.Year;
            if (DateTime.Now.DayOfYear < date.DayOfYear)
                age--;

            return age >= 0 && age <= 120;
        }

        /// <summary>
        /// Kiểm tra giờ hẹn hợp lệ (phải là giờ tương lai)
        /// </summary>
        public static bool IsValidFutureDateTime(DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }

        /// <summary>
        /// Validate mật khẩu (tối thiểu 6 ký tự)
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        /// <summary>
        /// Kiểm tra số CMND/CCCD (9 hoặc 12 số)
        /// </summary>
        public static bool IsValidIdCard(string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
                return false;

            idCard = idCard.Replace(" ", "");
            return Regex.IsMatch(idCard, @"^\d{9}$") || Regex.IsMatch(idCard, @"^\d{12}$");
        }
    }
}