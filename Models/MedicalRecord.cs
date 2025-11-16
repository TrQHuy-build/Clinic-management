using System;

namespace DentalClinicManagement.Models
{
    /// <summary>
    /// Model cho bảng MedicalRecord (Hồ sơ bệnh án)
    /// </summary>
    public class MedicalRecord
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public int StaffId { get; set; }
        public string Diagnosis { get; set; } // Chẩn đoán
        public string Treatment { get; set; } // Điều trị
        public DateTime RecordDate { get; set; }

        // Thông tin join
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }

        public MedicalRecord() { }

        public MedicalRecord(int recordId, int patientId, int staffId,
            string diagnosis, string treatment, DateTime recordDate)
        {
            RecordId = recordId;
            PatientId = patientId;
            StaffId = staffId;
            Diagnosis = diagnosis;
            Treatment = treatment;
            RecordDate = recordDate;
        }

        public override string ToString()
        {
            return $"#{RecordId} - {PatientName} - {RecordDate:dd/MM/yyyy}";
        }
    }
}