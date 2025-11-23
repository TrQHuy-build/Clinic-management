using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Staff
{
    public partial class StaffDashboard : UserControl
    {
        public StaffDashboard()
        {
            InitializeComponent();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                LoadStatistics();
                LoadTodayAppointments();
                LoadPendingInvoices();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dashboard: {ex.Message}");
            }
        }

        private void LoadStatistics()
        {
            try
            {
                // Tổng lịch hẹn hôm nay
                string queryToday = "SELECT COUNT(*) FROM Appointment WHERE CAST(appointment_date AS DATE) = @today AND status = 'booked'";
                object todayCount = DatabaseHelper.ExecuteScalar(queryToday, new SqlParameter[] {
                    new SqlParameter("@today", DateTime.Today)
                });
                lblTodayAppointments.Text = todayCount?.ToString() ?? "0";

                // Lịch hẹn chờ xác nhận
                string queryPending = "SELECT COUNT(*) FROM Appointment WHERE status = 'pending'";
                object pendingCount = DatabaseHelper.ExecuteScalar(queryPending);
                lblPendingAppointments.Text = pendingCount?.ToString() ?? "0";

                // Hóa đơn chưa thanh toán
                string queryUnpaid = "SELECT COUNT(*) FROM Invoice WHERE status = 'unpaid'";
                object unpaidCount = DatabaseHelper.ExecuteScalar(queryUnpaid);
                lblUnpaidInvoices.Text = unpaidCount?.ToString() ?? "0";

                // Bệnh nhân mới trong tháng
                string queryNewPatients = @"
                    SELECT COUNT(*) FROM Patient p
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE MONTH(u.created_at) = MONTH(GETDATE())
                    AND YEAR(u.created_at) = YEAR(GETDATE())";
                object newPatients = DatabaseHelper.ExecuteScalar(queryNewPatients);
                lblNewPatients.Text = newPatients?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải thống kê: {ex.Message}");
            }
        }

        private void LoadTodayAppointments()
        {
            try
            {
                string query = @"
                    SELECT
                        a.appointment_id AS [ID],
                        a.patient_name AS [Bệnh nhân],
                        a.phone AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'HH:mm') AS [Giờ hẹn],
                        CASE a.status
                            WHEN 'booked' THEN N'Đã đặt'
                            WHEN 'pending' THEN N'Chờ BS xác nhận'
                            WHEN 'confirmed' THEN N'Đã xác nhận'
                            WHEN 'completed' THEN N'Hoàn thành'
                            ELSE a.status
                        END AS [Trạng thái]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE CAST(a.appointment_date AS DATE) = @today
                    AND a.status IN ('booked', 'pending', 'confirmed')
                    ORDER BY a.appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@today", DateTime.Today)
                });

                dgvTodayAppointments.DataSource = dt;

                if (dgvTodayAppointments.Columns.Contains("ID"))
                    dgvTodayAppointments.Columns["ID"].Visible = false;

                // Color coding
                foreach (DataGridViewRow row in dgvTodayAppointments.Rows)
                {
                    if (row.IsNewRow) continue;
                    string status = row.Cells["Trạng thái"].Value?.ToString();

                    if (status == "Đã đặt")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFF3CD");
                    else if (status == "Chờ BS xác nhận")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D1ECF1");
                    else if (status == "Đã xác nhận")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D4EDDA");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải lịch hẹn: {ex.Message}");
            }
        }

        private void LoadPendingInvoices()
        {
            try
            {
                string query = @"
                    SELECT
                        i.invoice_id AS [ID],
                        u.fullname AS [Bệnh nhân],
                        FORMAT(i.invoice_date, 'dd/MM/yyyy') AS [Ngày],
                        i.total_amount AS [Tổng tiền],
                        CASE i.status
                            WHEN 'unpaid' THEN N'Chưa thanh toán'
                            WHEN 'paid' THEN N'Đã thanh toán'
                            ELSE i.status
                        END AS [Trạng thái]
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE i.status = 'unpaid'
                    ORDER BY i.invoice_date DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                dgvPendingInvoices.DataSource = dt;

                if (dgvPendingInvoices.Columns.Contains("ID"))
                    dgvPendingInvoices.Columns["ID"].Visible = false;

                if (dgvPendingInvoices.Columns.Contains("Tổng tiền"))
                    dgvPendingInvoices.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";

                foreach (DataGridViewRow row in dgvPendingInvoices.Rows)
                {
                    if (!row.IsNewRow)
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải hóa đơn: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboard();
        }
    }
}