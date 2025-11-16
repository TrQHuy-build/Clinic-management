using System;
using System.Data;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminDashboard : UserControl
    {
        public AdminDashboard()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                statsPanel.Controls.Clear();

                var cardPatients = CreateStatCard("Patient", "Bệnh nhân", "0", "#007ACC");
                var cardDoctors = CreateStatCard("Doctor", "Bác sĩ", "0", "#28A745");
                var cardAppointments = CreateStatCard("Calendar", "Lịch hẹn hôm nay", "0", "#FFC107");
                var cardRevenue = CreateStatCard("Money", "Doanh thu tháng", "0đ", "#DC3545");

                statsPanel.Controls.Add(cardPatients);
                statsPanel.Controls.Add(cardDoctors);
                statsPanel.Controls.Add(cardAppointments);
                statsPanel.Controls.Add(cardRevenue);

                // === TẢI DỮ LIỆU ===
                UpdateStatValue("Bệnh nhân", GetScalar("SELECT COUNT(*) FROM Patient")?.ToString() ?? "0");
                UpdateStatValue("Bác sĩ", GetScalar("SELECT COUNT(*) FROM Staff WHERE position = N'Doctor'")?.ToString() ?? "0");
                UpdateStatValue("Lịch hẹn hôm nay", GetScalar(@"
            SELECT COUNT(*) FROM Appointment
            WHERE CAST(appointment_date AS DATE) = CAST(GETDATE() AS DATE)")?.ToString() ?? "0");

                var revenue = GetScalar(@"
            SELECT ISNULL(SUM(total_amount), 0) FROM Invoice
            WHERE MONTH(invoice_date) = MONTH(GETDATE())
            AND YEAR(invoice_date) = YEAR(GETDATE())
            AND status = N'paid'");
                decimal rev = revenue != null ? Convert.ToDecimal(revenue) : 0;
                UpdateStatValue("Doanh thu tháng", $"{rev:N0}đ");

                // === LỊCH HẸN ===
                DataTable dt = DatabaseHelper.ExecuteQuery(@"
            SELECT TOP 10
                a.appointment_id AS [Mã],
                a.patient_name AS [Bệnh nhân],
                s.service_name AS [Dịch vụ],
                FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Ngày hẹn],
                a.status AS [Trạng thái]
            FROM Appointment a
            LEFT JOIN Service s ON a.service_id = s.service_id
            ORDER BY a.appointment_date DESC");

                dgvAppointments.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private object GetScalar(string query)
        {
            try { return DatabaseHelper.ExecuteScalar(query); }
            catch { return null; }
        }

        private void UpdateStatValue(string label, string value)
        {
            Control[] controls = this.Controls.Find("value_" + label, true);
            if (controls.Length > 0 && controls[0] is Label)
            {
                ((Label)controls[0]).Text = value;
            }
        }

        private void dgvAppointments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}