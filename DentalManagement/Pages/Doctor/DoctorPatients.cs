using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorPatients : UserControl
    {
        public DoctorPatients()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            if (!Auth.CurrentStaffId.HasValue) return;

            try
            {
                string query = @"
                    SELECT DISTINCT
                        u.fullname AS [Họ tên],
                        u.phone AS [SĐT],
                        u.email AS [Email],
                        p.gender AS [Giới tính],
                        COUNT(m.record_id) AS [Số lần khám]
                    FROM MedicalRecord m
                    INNER JOIN Patient p ON m.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE m.staff_id = @staffId
                    GROUP BY u.fullname, u.phone, u.email, p.gender
                    ORDER BY COUNT(m.record_id) DESC";

                var parameters = new[] { new SqlParameter("@staffId", Auth.CurrentStaffId.Value) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvPatients.DataSource = dt;

                foreach (DataGridViewRow row in dgvPatients.Rows)
                {
                    if (row.Cells["Giới tính"].Value is string gender)
                    {
                        row.Cells["Giới tính"].Value = Formatter.FormatGender(gender);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải danh sách bệnh nhân: {ex.Message}");
            }
        }
    }
}