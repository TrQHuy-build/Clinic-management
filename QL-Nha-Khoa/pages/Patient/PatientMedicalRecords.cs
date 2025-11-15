using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientMedicalRecords : UserControl
    {
        public PatientMedicalRecords()
        {
            InitializeComponent();
            LoadRecords();
        }

        private void LoadRecords()
        {
            if (!Auth.CurrentPatientId.HasValue) return;

            try
            {
                string query = @"
                    SELECT 
                        u.fullname AS [Bác sĩ],
                        m.diagnosis AS [Chẩn đoán],
                        m.treatment AS [Điều trị],
                        FORMAT(m.record_date, 'dd/MM/yyyy') AS [Ngày khám]
                    FROM MedicalRecord m
                    INNER JOIN Staff s ON m.staff_id = s.staff_id
                    INNER JOIN UserAccount u ON s.user_id = u.user_id
                    WHERE m.patient_id = @patientId
                    ORDER BY m.record_date DESC";

                SqlParameter[] parameters = { new SqlParameter("@patientId", Auth.CurrentPatientId.Value) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvRecords.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}