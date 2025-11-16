using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorAppointments : UserControl
    {
        public DoctorAppointments()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                string query = @"
                    SELECT
                        a.appointment_id AS [ID],
                        a.patient_name AS [Bệnh nhân],
                        ISNULL(a.phone, 'N/A') AS [SĐT],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'HH:mm') AS [Giờ hẹn],
                        a.status AS [Trạng thái],
                        ISNULL(a.notes, '') AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE CAST(a.appointment_date AS DATE) = @date
                    ORDER BY a.appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@date", dtpDate.Value.Date)
                });

                dgvAppointments.DataSource = dt;

                if (dgvAppointments.Columns["ID"] != null)
                    dgvAppointments.Columns["ID"].Visible = false;

                AddActionColumn();

                foreach (DataGridViewRow row in dgvAppointments.Rows)
                {
                    string rawStatus = row.Cells["Trạng thái"].Value?.ToString();
                    row.Cells["Trạng thái"].Value = Formatter.FormatStatus(rawStatus);

                    if (rawStatus?.ToLower() == "completed")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void AddActionColumn()
        {
            if (!dgvAppointments.Columns.Contains("Complete"))
            {
                dgvAppointments.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Complete",
                    HeaderText = "",
                    Text = "Hoàn thành",
                    UseColumnTextForButtonValue = true,
                    Width = 100
                });
            }
        }

        private void DgvAppointments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int appointmentId = Convert.ToInt32(dgvAppointments.Rows[e.RowIndex].Cells["ID"].Value);
            string status = dgvAppointments.Rows[e.RowIndex].Cells["Trạng thái"].Value?.ToString();

            if (e.ColumnIndex == dgvAppointments.Columns["Complete"].Index)
            {
                if (status == "Hoàn thành")
                {
                    MessageBoxHelper.ShowWarning("Lịch hẹn này đã hoàn thành!");
                    return;
                }
                CompleteAppointment(appointmentId);
            }
        }

        private void CompleteAppointment(int appointmentId)
        {
            if (!MessageBoxHelper.ShowConfirm("Xác nhận hoàn thành lịch hẹn này?"))
                return;

            try
            {
                string query = "UPDATE Appointment SET status = N'completed' WHERE appointment_id = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", appointmentId)
                });

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đã hoàn thành lịch hẹn!");
                    Logger.LogAction("COMPLETE_APPOINTMENT", $"Hoàn thành lịch hẹn #{appointmentId}");
                    LoadAppointments();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        // Event handlers
        private void dtpDate_ValueChanged(object sender, EventArgs e) => LoadAppointments();
        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpDate.Value = DateTime.Today;
            LoadAppointments();
        }
    }
}