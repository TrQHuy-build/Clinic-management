using System;
using System.Globalization;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Class hỗ trợ format dữ liệu hiển thị
    /// </summary>
    public static class Formatter
    {
        private static readonly CultureInfo vnCulture = new CultureInfo("vi-VN");

        /// <summary>
        /// Format số tiền VND
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return $"{amount:N0}đ";
        }

        /// <summary>
        /// Format số tiền VND với chi tiết
        /// </summary>
        public static string FormatCurrencyDetail(decimal amount)
        {
            return amount.ToString("C0", vnCulture).Replace("₫", "VNĐ");
        }

        /// <summary>
        /// Format ngày giờ đầy đủ
        /// </summary>
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Format chỉ ngày
        /// </summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Format chỉ giờ
        /// </summary>
        public static string FormatTime(DateTime time)
        {
            return time.ToString("HH:mm");
        }

        /// <summary>
        /// Format số điện thoại (xxxx xxx xxx)
        /// </summary>
        public static string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "";

            phone = phone.Replace(" ", "").Replace("-", "");

            if (phone.Length == 10)
            {
                return $"{phone.Substring(0, 4)} {phone.Substring(4, 3)} {phone.Substring(7, 3)}";
            }
            else if (phone.Length == 11)
            {
                return $"{phone.Substring(0, 4)} {phone.Substring(4, 4)} {phone.Substring(8, 3)}";
            }

            return phone;
        }

        /// <summary>
        /// Format tên đầy đủ (Viết hoa chữ cái đầu)
        /// </summary>
        public static string FormatFullName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            string[] words = name.Trim().ToLower().Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }

            return string.Join(" ", words);
        }

        /// <summary>
        /// Format giới tính
        /// </summary>
        public static string FormatGender(string gender)
        {
            if (string.IsNullOrWhiteSpace(gender))
                return "";

            switch (gender.ToLower())
            {
                case "male": return "Nam";
                case "female": return "Nữ";
                case "other": return "Khác";
                default: return gender;
            }
        }

        /// <summary>
        /// Format trạng thái
        /// </summary>
        public static string FormatStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "";

            switch (status.ToLower())
            {
                case "active": return "Hoạt động";
                case "inactive": return "Không hoạt động";
                case "booked": return "Đã đặt";
                case "completed": return "Hoàn thành";
                case "cancelled": return "Đã hủy";
                case "paid": return "Đã thanh toán";
                case "unpaid": return "Chưa thanh toán";
                case "available": return "Khả dụng";
                case "unavailable": return "Không khả dụng";
                default: return status;
            }
        }

        /// <summary>
        /// Format số lượng với đơn vị
        /// </summary>
        public static string FormatQuantity(int quantity, string unit)
        {
            return $"{quantity} {unit}";
        }

        /// <summary>
        /// Cắt ngắn chuỗi và thêm ...
        /// </summary>
        public static string TruncateString(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }

        /// <summary>
        /// Format tuổi từ ngày sinh
        /// </summary>
        public static string FormatAge(DateTime dateOfBirth)
        {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age--;

            return $"{age} tuổi";
        }

        /// <summary>
        /// Format phần trăm
        /// </summary>
        public static string FormatPercentage(decimal value)
        {
            return $"{value:N2}%";
        }
    }
}