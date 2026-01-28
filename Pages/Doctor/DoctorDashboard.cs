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

            // Thêm event để xử lý resize
            this.Resize += DoctorDashboard_Resize;

            LoadDashboardData();
        }

        private void DoctorDashboard_Load(object sender, EventArgs e)
        {
            // Điều chỉnh layout ngay khi load
            AdjustLayout();
        }

        private void DoctorDashboard_Resize(object sender, EventArgs e)
        {
            // Tính toán lại kích thước và vị trí các controls
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            // Padding
            int padding = 20;
            int cardSpacing = 20;
            int availableWidth = this.Width - (padding * 2);

            // Điều chỉnh stats panel
            statsPanel.Width = availableWidth;

            // Tính toán kích thước card động
            int cardWidth = (availableWidth - (cardSpacing * 2)) / 3;

            // Card 1
            card1.Width = cardWidth;
            card1.Location = new Point(0, 0);

            // Card 2 - Đặt riêng lẻ vì không nằm trong statsPanel
            card2.Width = cardWidth;
            card2.Location = new Point(padding + cardWidth + cardSpacing, 80);

            // Card 3 - Đặt riêng lẻ vì không nằm trong statsPanel
            card3.Width = cardWidth;
            card3.Location = new Point(padding + (cardWidth + cardSpacing) * 2, 80);

            // Điều chỉnh appointment panel
            appointmentPanel.Width = availableWidth;

            // Điều chỉnh DataGridView trong panel (tự động vì đã set Anchor)
            dgvAppointments.Width = appointmentPanel.Width - 40; // Trừ padding
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
                string queryToday = @"
                    SELECT COUNT(*) FROM Appointment 
                    WHERE assigned_doctor_id = @staffId 
                      AND CAST(appointment_date AS DATE) = CAST(GETDATE() AS DATE)
                      AND status IN ('Booked', 'Confirmed')";
                object todayCount = DatabaseHelper.ExecuteScalar(queryToday, 
                    new SqlParameter[] { new SqlParameter("@staffId", staffId) });
                UpdateStatValue("Lịch hẹn hôm nay", todayCount?.ToString() ?? "0");

                // 2. Bệnh nhân tuần này (đã khám)
                string queryWeek = @"
                    SELECT COUNT(DISTINCT patient_id)
                    FROM MedicalRecord
                    WHERE staff_id = @staffId
                      AND record_date >= DATEADD(DAY, -7, GETDATE())";
                object weekCount = DatabaseHelper.ExecuteScalar(queryWeek, 
                    new SqlParameter[] { new SqlParameter("@staffId", staffId) });
                UpdateStatValue("Bệnh nhân tuần này", weekCount?.ToString() ?? "0");

                // 3. Đã khám hôm nay
                string queryCompleted = @"
                    SELECT COUNT(*)
                    FROM MedicalRecord
                    WHERE staff_id = @staffId
                      AND CAST(record_date AS DATE) = CAST(GETDATE() AS DATE)";
                object completedCount = DatabaseHelper.ExecuteScalar(queryCompleted, 
                    new SqlParameter[] { new SqlParameter("@staffId", staffId) });
                UpdateStatValue("Đã khám hôm nay", completedCount?.ToString() ?? "0");

                // 4. Danh sách lịch hẹn
                string queryAppointments = @"
                    SELECT
                        a.appointment_id AS [Mã],
                        CASE 
                            WHEN p.patient_id IS NOT NULL THEN u.fullname
                            ELSE a.patient_name
                        END AS [Bệnh nhân],
                        ISNULL(a.phone, 'N/A') AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'HH:mm') AS [Giờ hẹn],
                        CASE a.status
                            WHEN 'Booked' THEN N'Đã đặt'
                            WHEN 'Confirmed' THEN N'Đã xác nhận'
                            WHEN 'Completed' THEN N'Hoàn thành'
                            WHEN 'Cancelled' THEN N'Đã hủy'
                            ELSE a.status
                        END AS [Trạng thái],
                        ISNULL(a.notes, '') AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    LEFT JOIN Patient p ON a.patient_id = p.patient_id
                    LEFT JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE a.assigned_doctor_id = @staffId
                    AND CAST(a.appointment_date AS DATE) = CAST(GETDATE() AS DATE)
                    ORDER BY a.appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(queryAppointments, 
                    new SqlParameter[] { new SqlParameter("@staffId", staffId) });
                dgvAppointments.DataSource = dt;

                // Điều chỉnh layout sau khi load data
                AdjustLayout();

                // Auto-resize columns
                if (dgvAppointments.Columns.Count > 0)
                {
                    dgvAppointments.Columns["Mã"].Width = 80;
                    dgvAppointments.Columns["Bệnh nhân"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvAppointments.Columns["SĐT"].Width = 120;
                    dgvAppointments.Columns["Dịch vụ"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvAppointments.Columns["Giờ hẹn"].Width = 100;
                    dgvAppointments.Columns["Trạng thái"].Width = 120;
                    dgvAppointments.Columns["Ghi chú"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                // ✅ FIX: Color code theo trạng thái
                foreach (DataGridViewRow row in dgvAppointments.Rows)
                {
                    if (row.IsNewRow) continue;
                    
                    string status = row.Cells["Trạng thái"].Value?.ToString();
                    switch (status)
                    {
                        case "Đã đặt":
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFF3CD"); // Yellow
                            break;
                        case "Đã xác nhận":
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D1ECF1"); // Blue
                            break;
                        case "Hoàn thành":
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D4EDDA"); // Green
                            break;
                        case "Đã hủy":
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8D7DA"); // Red
                            break;
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