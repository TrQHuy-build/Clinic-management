using System;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Lớp quản lý thông tin người dùng đang đăng nhập
    /// </summary>
    public static class Auth
    {
        public static int? CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; }
        public static string CurrentUserRole { get; set; }
        public static string CurrentUserEmail { get; set; }
        public static int? CurrentStaffId { get; set; } // Chỉ có khi role = doctor/staff
        public static int? CurrentPatientId { get; set; } // Chỉ có khi role = patient

        /// <summary>
        /// Đăng nhập user
        /// </summary>
        public static void Login(int userId, string userName, string role, string email,
            int? staffId = null, int? patientId = null)
        {
            CurrentUserId = userId;
            CurrentUserName = userName;
            CurrentUserRole = role;
            CurrentUserEmail = email;
            CurrentStaffId = staffId;
            CurrentPatientId = patientId;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public static void Logout()
        {
            CurrentUserId = 0;
            CurrentUserName = null;
            CurrentUserRole = null;
            CurrentUserEmail = null;
            CurrentStaffId = null;
            CurrentPatientId = null;
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public static bool IsAuthenticated()
        {
            return CurrentUserId > 0 && !string.IsNullOrEmpty(CurrentUserRole);
        }

        /// <summary>
        /// Kiểm tra role
        /// </summary>
        public static bool IsAdmin()
        {
            return CurrentUserRole?.ToLower() == "admin";
        }

        public static bool IsDoctor()
        {
            return CurrentUserRole?.ToLower() == "doctor";
        }

        public static bool IsPatient()
        {
            return CurrentUserRole?.ToLower() == "patient";
        }

        public static bool IsStaff()
        {
            return CurrentUserRole?.ToLower() == "staff";
        }
    }
}