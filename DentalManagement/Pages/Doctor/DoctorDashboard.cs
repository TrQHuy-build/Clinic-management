using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorDashboard : UserControl
    {
        public DoctorDashboard()
        {
            InitializeComponent();

            // GÁN SAU InitializeComponent
            lblTitle.Text = $"Chào mừng, BS. {Auth.CurrentUserName}";

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            if (!Auth.CurrentStaffId.HasValue)
            {
                MessageBoxHelper.ShowError("Không tìm thấy thông tin bác sĩ!");
                return;
            }

            int staffId = Auth.CurrentStaffId.Value;

            try
            {
                // 1. Lịch hẹn hôm nay
                string queryToday = "SELECT COUNT(*) FROM Appointment WHERE CAST(appointment_date AS DATE) = CAST(GETDATE() AS DATE)";
                object todayCount = DatabaseHelper.ExecuteScalar(queryToday);
                UpdateStatValue("Lịch hẹn hôm nay", todayCount?.ToString() ?? "0");

                // 2. Bệnh nhân tuần này
                string queryWeek = @"
                    SELECT COUNT(DISTINCT patient_id)
                    FROM MedicalRecord
                    WHERE staff_id = @staffId
                      AND record_date >= DATEADD(DAY, -7, GETDATE())";
                object weekCount = DatabaseHelper.ExecuteScalar(queryWeek, new SqlParameter[] { new SqlParameter("@staffId", staffId) });
                UpdateStatValue("Bệnh nhân tuần này", weekCount?.ToString() ?? "0");

                // 3. Đã khám hôm nay
                string queryCompleted = @"
                    SELECT COUNT(*)
                    FROM MedicalRecord
                    WHERE staff_id = @staffId
                      AND CAST(record_date AS DATE) = CAST(GETDATE() AS DATE)";
                object completedCount = DatabaseHelper.ExecuteScalar(queryCompleted, new SqlParameter[] { new SqlParameter("@staffId", staffId) });
                UpdateStatValue("Đã khám hôm nay", completedCount?.ToString() ?? "0");

                // 4. Danh sách lịch hẹn
                string queryAppointments = @"
                    SELECT
                        a.appointment_id AS [Mã],
                        a.patient_name AS [Bệnh nhân],
                        ISNULL(a.phone, 'N/A') AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'HH:mm') AS [Giờ hẹn],
                        a.status AS [Trạng thái],
                        ISNULL(a.notes, '') AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE CAST(a.appointment_date AS DATE) = CAST(GETDATE() AS DATE)
                    ORDER BY a.appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(queryAppointments);
                dgvAppointments.DataSource = dt;

                // Format trạng thái
                foreach (DataGridViewRow row in dgvAppointments.Rows)
                {
                    if (row.Cells["Trạng thái"].Value is string status)
                    {
                        row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        // CHỈ 1 HÀM UpdateStatValue
        private void UpdateStatValue(string label, string value)
        {
            switch (label)
            {
                case "Lịch hẹn hôm nay":
                    valueToday.Text = value;
                    break;
                case "Bệnh nhân tuần này":
                    valueWeek.Text = value;
                    break;
                case "Đã khám hôm nay":
                    valueCompleted.Text = value;
                    break;
            }
        }
    }
}