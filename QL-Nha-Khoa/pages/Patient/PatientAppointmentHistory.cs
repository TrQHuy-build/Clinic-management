using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientAppointmentHistory : UserControl
    {
        public PatientAppointmentHistory()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                // Get patient name from UserAccount
                string getNameQuery = "SELECT fullname FROM UserAccount WHERE user_id = @userId";
                object nameResult = DatabaseHelper.ExecuteScalar(getNameQuery, new SqlParameter[] { new SqlParameter("@userId", Auth.CurrentUserId) });
                string patientName = nameResult?.ToString() ?? Auth.CurrentUserName;

                string query = @"
                    SELECT 
                        a.appointment_id AS [ID],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Ngày giờ hẹn],
                        a.status AS [Trạng thái],
                        a.notes AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    WHERE a.patient_name = @patientName
                    ORDER BY a.appointment_date DESC";

                SqlParameter[] parameters = { new SqlParameter("@patientName", patientName) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvHistory.DataSource = dt;

                if (dgvHistory.Columns["ID"] != null)
                    dgvHistory.Columns["ID"].Visible = false;

                if (!dgvHistory.Columns.Contains("Cancel"))
                {
                    dgvHistory.Columns.Add(new DataGridViewButtonColumn
                    {
                        Name = "Cancel",
                        Text = "Hủy",
                        UseColumnTextForButtonValue = true,
                        Width = 70
                    });
                }

                foreach (DataGridViewRow row in dgvHistory.Rows)
                {
                    string status = row.Cells["Trạng thái"].Value?.ToString();
                    row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);

                    if (status?.ToLower() == "cancelled")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                    else if (status?.ToLower() == "completed")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void DgvHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int appointmentId = Convert.ToInt32(dgvHistory.Rows[e.RowIndex].Cells["ID"].Value);
            string status = dgvHistory.Rows[e.RowIndex].Cells["Trạng thái"].Value?.ToString();

            if (e.ColumnIndex == dgvHistory.Columns["Cancel"].Index)
            {
                if (status != "Đã đặt")
                {
                    MessageBoxHelper.ShowWarning("Chỉ có thể hủy lịch hẹn đang chờ!");
                    return;
                }

                CancelAppointment(appointmentId);
            }
        }

        private void CancelAppointment(int appointmentId)
        {
            if (!MessageBoxHelper.ShowConfirm("Bạn có chắc muốn hủy lịch hẹn này?"))
                return;

            try
            {
                string query = "UPDATE Appointment SET status = N'cancelled' WHERE appointment_id = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", appointmentId) };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đã hủy lịch hẹn!");
                    Logger.LogAction("CANCEL_APPOINTMENT", $"Hủy lịch hẹn #{appointmentId}");
                    LoadHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}