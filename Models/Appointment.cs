using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Appointment
    /// </summary>
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // booked, completed, cancelled
        public string Notes { get; set; }

        // Thông tin join từ Service
        public string ServiceName { get; set; }
        public decimal ServicePrice { get; set; }

        public Appointment() { }

        public Appointment(int appointmentId, string patientName, string phone,
            string email, int? serviceId, DateTime appointmentDate, string status, string notes)
        {
            AppointmentId = appointmentId;
            PatientName = patientName;
            Phone = phone;
            Email = email;
            ServiceId = serviceId;
            AppointmentDate = appointmentDate;
            Status = status;
            Notes = notes;
        }

        /// <summary>
        /// Kiểm tra có thể hủy hẹn không (chỉ hủy được nếu chưa đến giờ hẹn)
        /// </summary>
        public bool CanCancel()
        {
            return Status?.ToLower() == "booked" && AppointmentDate > DateTime.Now;
        }

        /// <summary>
        /// Lấy trạng thái với màu sắc
        /// </summary>
        public string GetStatusDisplay()
        {
            switch (Status?.ToLower())
            {
                case "booked": return "Đã đặt";
                case "completed": return "Hoàn thành";
                case "cancelled": return "Đã hủy";
                default: return Status;
            }
        }

        public override string ToString()
        {
            return $"{PatientName} - {AppointmentDate:dd/MM/yyyy HH:mm}";
        }
    }
}