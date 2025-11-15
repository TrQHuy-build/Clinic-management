using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Invoice (Hóa đơn)
    /// </summary>
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int PatientId { get; set; }
        public int StaffId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // paid, unpaid

        // Thông tin join
        public string PatientName { get; set; }
        public string StaffName { get; set; }

        public Invoice() { }

        public Invoice(int invoiceId, int patientId, int staffId,
            DateTime invoiceDate, decimal totalAmount, string status)
        {
            InvoiceId = invoiceId;
            PatientId = patientId;
            StaffId = staffId;
            InvoiceDate = invoiceDate;
            TotalAmount = totalAmount;
            Status = status;
        }

        /// <summary>
        /// Kiểm tra đã thanh toán chưa
        /// </summary>
        public bool IsPaid()
        {
            return Status?.ToLower() == "paid";
        }

        /// <summary>
        /// Format tổng tiền
        /// </summary>
        public string GetFormattedTotal()
        {
            return $"{TotalAmount:N0}đ";
        }

        /// <summary>
        /// Lấy trạng thái hiển thị
        /// </summary>
        public string GetStatusDisplay()
        {
            return IsPaid() ? "Đã thanh toán" : "Chưa thanh toán";
        }

        public override string ToString()
        {
            return $"HD#{InvoiceId} - {PatientName} - {GetFormattedTotal()}";
        }
    }
}