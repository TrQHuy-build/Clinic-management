using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng Prescription (Toa thuốc)
    /// </summary>
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int RecordId { get; set; }
        public int MedicineId { get; set; }
        public string Dosage { get; set; } // Liều lượng: 2 viên/ngày
        public int Quantity { get; set; } // Số lượng
        public string Notes { get; set; }

        // Thông tin join từ Medicine
        public string MedicineName { get; set; }
        public string Unit { get; set; }
        public decimal MedicinePrice { get; set; }

        public Prescription() { }

        public Prescription(int prescriptionId, int recordId, int medicineId,
            string dosage, int quantity, string notes)
        {
            PrescriptionId = prescriptionId;
            RecordId = recordId;
            MedicineId = medicineId;
            Dosage = dosage;
            Quantity = quantity;
            Notes = notes;
        }

        /// <summary>
        /// Tính tổng tiền cho thuốc này
        /// </summary>
        public decimal GetTotalAmount()
        {
            return MedicinePrice * Quantity;
        }

        /// <summary>
        /// Format tổng tiền
        /// </summary>
        public string GetFormattedTotal()
        {
            return $"{GetTotalAmount():N0}đ";
        }

        public override string ToString()
        {
            return $"{MedicineName} - {Dosage} x {Quantity} {Unit}";
        }
    }
}