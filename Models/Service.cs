using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Service
    /// </summary>
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } // available, unavailable

        public Service() { }

        public Service(int serviceId, string serviceName, string description,
            decimal price, string status)
        {
            ServiceId = serviceId;
            ServiceName = serviceName;
            Description = description;
            Price = price;
            Status = status;
        }

        /// <summary>
        /// Kiểm tra dịch vụ có khả dụng không
        /// </summary>
        public bool IsAvailable()
        {
            return Status?.ToLower() == "available";
        }

        /// <summary>
        /// Format giá tiền
        /// </summary>
        public string GetFormattedPrice()
        {
            return $"{Price:N0}đ";
        }

        public override string ToString()
        {
            return $"{ServiceName} - {GetFormattedPrice()}";
        }
    }
}