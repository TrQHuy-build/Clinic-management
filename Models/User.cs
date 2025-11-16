using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng UserAccount
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // admin, doctor, staff, patient
        public string Status { get; set; } // active, inactive
        public DateTime CreatedAt { get; set; }

        public User() { }

        public User(int userId, string fullname, string phone, string email,
            string passwordHash, string role, string status, DateTime createdAt)
        {
            UserId = userId;
            Fullname = fullname;
            Phone = phone;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            Status = status;
            CreatedAt = createdAt;
        }

        public override string ToString()
        {
            return $"{Fullname} ({Email}) - {Role}";
        }
    }
}