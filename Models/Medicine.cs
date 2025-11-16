using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Medicine
    /// </summary>
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; } // hộp, viên, ống...
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }

        public Medicine() { }

        public Medicine(int medicineId, string name, string unit,
            string manufacturer, decimal price)
        {
            MedicineId = medicineId;
            Name = name;
            Unit = unit;
            Manufacturer = manufacturer;
            Price = price;
        }

        /// <summary>
        /// Format giá tiền
        /// </summary>
        public string GetFormattedPrice()
        {
            return $"{Price:N0}đ/{Unit}";
        }

        public override string ToString()
        {
            return $"{Name} ({Unit}) - {Manufacturer}";
        }
    }
}