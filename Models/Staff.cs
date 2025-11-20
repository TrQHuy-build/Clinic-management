using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Staff (bao gồm cả Doctor)
    /// </summary>
    public class Staff
    {
        public int StaffId { get; set; }
        public int UserId { get; set; }
        public string Position { get; set; } // Doctor, Staff
        public string Specialization { get; set; } // Chỉ dùng cho Doctor
        public decimal Salary { get; set; }
        public DateTime? HireDate { get; set; }

        // Thông tin từ UserAccount (join)
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

        public Staff() { }

        public Staff(int staffId, int userId, string position, string specialization,
            decimal salary, DateTime? hireDate)
        {
            StaffId = staffId;
            UserId = userId;
            Position = position;
            Specialization = specialization;
            Salary = salary;
            HireDate = hireDate;
        }

        /// <summary>
        /// Kiểm tra có phải bác sĩ không
        /// </summary>
        public bool IsDoctor()
        {
            return Position?.ToLower() == "doctor";
        }

        /// <summary>
        /// Số năm làm việc
        /// </summary>
        public int GetYearsOfService()
        {
            if (!HireDate.HasValue) return 0;

            int years = DateTime.Now.Year - HireDate.Value.Year;
            if (DateTime.Now.DayOfYear < HireDate.Value.DayOfYear)
                years--;

            return years;
        }

        public override string ToString()
        {
            if (IsDoctor())
                return $"BS. {Fullname} - {Specialization}";
            return $"{Fullname} - {Position}";
        }
    }
}