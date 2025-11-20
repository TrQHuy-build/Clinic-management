using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Patient
    /// </summary>
    public class Patient
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } // Male, Female, Other
        public string Address { get; set; }
        public string Insurance { get; set; }

        // Thông tin từ UserAccount (join)
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Patient() { }

        public Patient(int patientId, int userId, DateTime? dateOfBirth,
            string gender, string address, string insurance)
        {
            PatientId = patientId;
            UserId = userId;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Address = address;
            Insurance = insurance;
        }

        /// <summary>
        /// Tính tuổi từ ngày sinh
        /// </summary>
        public int GetAge()
        {
            if (!DateOfBirth.HasValue) return 0;

            int age = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (DateTime.Now.DayOfYear < DateOfBirth.Value.DayOfYear)
                age--;

            return age;
        }

        public override string ToString()
        {
            return $"{Fullname} - {Phone}";
        }
    }
}