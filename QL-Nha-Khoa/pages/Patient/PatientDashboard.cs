using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq; // ← THÊM DÒNG NÀY
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientDashboard : UserControl
    {
        public PatientDashboard()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // Sửa: Dùng int? thay vì int
            if (!Auth.CurrentPatientId.HasValue || !Auth.CurrentUserId.HasValue)
            {
                MessageBoxHelper.ShowError("Không tìm thấy thông tin bệnh nhân!");
                return;
            }

            try
            {
                int patientId = Auth.CurrentPatientId.Value; // ← .Value
                int userId = Auth.CurrentUserId.Value;       // ← .Value

                // Lấy tên bệnh nhân từ DB
                string getNameQuery = "SELECT fullname FROM UserAccount WHERE user_id = @userId";
                object nameResult = DatabaseHelper.ExecuteScalar(getNameQuery, new[] { new SqlParameter("@userId", userId) });
                string patientName = nameResult?.ToString() ?? Auth.CurrentUserName;

                // === 1. Đếm lịch hẹn sắp tới ===
                string queryUpcoming = @"
                    SELECT COUNT(*) FROM Appointment
                    WHERE patient_name = @patientName
                    AND appointment_date >= GETDATE()
                    AND status = N'booked'";

                object upcomingCount = DatabaseHelper.ExecuteScalar(queryUpcoming, new[] { new SqlParameter("@patientName", patientName) });
                UpdateStatValue("Lịch hẹn sắp tới", upcomingCount?.ToString() ?? "0");

                // === 2. Đếm hóa đơn chưa thanh toán ===
                string queryUnpaid = @"
                    SELECT COUNT(*) FROM Invoice
                    WHERE patient_id = @patientId AND status = N'unpaid'";

                object unpaidCount = DatabaseHelper.ExecuteScalar(queryUnpaid, new[] { new SqlParameter("@patientId", patientId) });
                UpdateStatValue("Hóa đơn chưa thanh toán", unpaidCount?.ToString() ?? "0");

                // === 3. Đếm hồ sơ khám bệnh ===
                string queryRecords = @"
                    SELECT COUNT(*) FROM MedicalRecord WHERE patient_id = @patientId";

                object recordCount = DatabaseHelper.ExecuteScalar(queryRecords, new[] { new SqlParameter("@patientId", patientId) });
                UpdateStatValue("Hồ sơ khám bệnh", recordCount?.ToString() ?? "0");

                // === 4. Chi tiết lịch hẹn ===
                string queryAppointmentDetails = @"
                    SELECT TOP 5
                        FORMAT(appointment_date, 'dd/MM/yyyy HH:mm') AS [Ngày giờ],
                        s.service_name AS [Dịch vụ],
                        status AS [Trạng thái]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE patient_name = @patientName
                    AND appointment_date >= GETDATE()
                    ORDER BY appointment_date";

                DataTable dtAppointments = DatabaseHelper.ExecuteQuery(queryAppointmentDetails, new[] { new SqlParameter("@patientName", patientName) });
                dgvUpcoming.DataSource = dtAppointments;

                // === 5. Chi tiết hóa đơn ===
                string queryInvoiceDetails = @"
                    SELECT
                        invoice_id AS [Mã HĐ],
                        FORMAT(invoice_date, 'dd/MM/yyyy') AS [Ngày],
                        FORMAT(total_amount, 'N0') + 'đ' AS [Số tiền]
                    FROM Invoice
                    WHERE patient_id = @patientId AND status = N'unpaid'
                    ORDER BY invoice_date DESC";

                DataTable dtInvoices = DatabaseHelper.ExecuteQuery(queryInvoiceDetails, new[] { new SqlParameter("@patientId", patientId) });
                dgvInvoices.DataSource = dtInvoices;

                // Cập nhật tiêu đề
                lblTitle.Text = $"Xin chào, {patientName}";
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void UpdateStatValue(string label, string value)
        {
            var card = statsPanel.Controls
                .OfType<Panel>()
                .FirstOrDefault(p => p.Controls.OfType<Label>().Any(l => l.Text == label));

            if (card != null)
            {
                var valueLabel = card.Controls
                    .OfType<Label>()
                    .FirstOrDefault(l => l.Name == "value_" + label);

                if (valueLabel != null)
                    valueLabel.Text = value;
            }
        }
    }
}