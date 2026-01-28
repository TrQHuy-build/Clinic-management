using System;
using System.Text.RegularExpressions;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Helper class untuk validasi input data
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Kiểm tra tên (chỉ chứa chữ cái và khoảng trắng)
        /// </summary>
        public static bool IsValidFullname(string fullname)
        {
            if (string.IsNullOrWhiteSpace(fullname))
                return false;

            // Chỉ cho phép chữ cái (bao gồm tiếng Việt), khoảng trắng, dấu chấm
            string pattern = @"^[\p{L}\s.'-]+$";
            return Regex.IsMatch(fullname, pattern);
        }

        /// <summary>
        /// Kiểm tra số điện thoại (chỉ số, 10-11 chữ số)
        /// </summary>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Loại bỏ khoảng trắng, dấu gạch ngang, ngoặc đơn
            string cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Kiểm tra chỉ chứa số và độ dài 10-11 chữ số
            string pattern = @"^[0-9]{10,11}$";
            return Regex.IsMatch(cleanPhone, pattern);
        }

        /// <summary>
        /// Kiểm tra số điện thoại Việt Nam (bắt đầu bằng 0)
        /// </summary>
        public static bool IsValidVietnamPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Phải bắt đầu bằng 0 và có 10-11 chữ số
            string pattern = @"^0[0-9]{9,10}$";
            return Regex.IsMatch(cleanPhone, pattern);
        }

        /// <summary>
        /// Kiểm tra ngày sinh hợp lệ (không quá trẻ, không quá già)
        /// </summary>
        public static bool IsValidDateOfBirth(DateTime dob)
        {
            // Người dùng phải ít nhất 10 tuổi
            DateTime minDate = DateTime.Now.AddYears(-150);
            DateTime maxDate = DateTime.Now.AddYears(-10);

            return dob >= minDate && dob <= maxDate;
        }

        /// <summary>
        /// Lấy thông báo lỗi chi tiết cho ngày sinh không hợp lệ
        /// </summary>
        public static string GetDateOfBirthErrorMessage(DateTime dob)
        {
            DateTime maxDate = DateTime.Now.AddYears(-10);
            DateTime minDate = DateTime.Now.AddYears(-150);

            if (dob > maxDate)
                return "Bạn phải ít nhất 10 tuổi để sử dụng dịch vụ!";

            if (dob < minDate)
                return "Ngày sinh không hợp lệ (quá cũ)!";

            return "Ngày sinh không hợp lệ!";
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra chuỗi không trống
        /// </summary>
        public static bool IsNotEmpty(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// Kiểm tra độ dài chuỗi
        /// </summary>
        public static bool IsValidLength(string text, int minLength, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            return text.Length >= minLength && text.Length <= maxLength;
        }
    }
}
