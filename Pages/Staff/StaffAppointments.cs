using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Staff
{
    public partial class StaffAppointments : UserControl
    {
        public StaffAppointments()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                string statusFilter = cboStatus.SelectedItem?.ToString() ?? "Tất cả";
                string searchText = txtSearch.Text.Trim();

                string query = @"
                    SELECT
                        a.appointment_id AS [ID],
                        a.patient_name AS [Bệnh nhân],
                        ISNULL(a.phone, 'N/A') AS [SĐT],
                        ISNULL(a.email, 'N/A') AS [Email],
                        s.service_name AS [Dịch vụ],
                        FORMAT(a.appointment_date, 'dd/MM/yyyy HH:mm') AS [Thời gian],
                        CASE a.status
                            WHEN 'booked' THEN N'Đã đặt'
                            WHEN 'pending' THEN N'Chờ BS xác nhận'
                            WHEN 'rejected' THEN N'Bị từ chối'
                            WHEN 'confirmed' THEN N'Đã xác nhận'
                            WHEN 'completed' THEN N'Hoàn thành'
                            WHEN 'cancelled' THEN N'Đã hủy'
                            ELSE a.status
                        END AS [Trạng thái],
                        ISNULL(st.fullname, N'Chưa assign') AS [Bác sĩ],
                        ISNULL(a.notes, '') AS [Ghi chú]
                    FROM Appointment a
                    LEFT JOIN Service s ON a.service_id = s.service_id
                    LEFT JOIN Staff staff ON a.assigned_doctor_id = staff.staff_id
                    LEFT JOIN UserAccount st ON staff.user_id = st.user_id
                    WHERE 1=1";

                System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();

                if (statusFilter != "Tất cả")
                {
                    string dbStatus = ConvertStatusToDb(statusFilter);
                    query += " AND a.status = @status";
                    parameters.Add(new SqlParameter("@status", dbStatus));
                }

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query += " AND (a.patient_name LIKE @search OR a.phone LIKE @search OR a.email LIKE @search)";
                    parameters.Add(new SqlParameter("@search", $"%{searchText}%"));
                }

                query += " ORDER BY a.appointment_date DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
                dgvAppointments.DataSource = dt;

                if (dgvAppointments.Columns.Contains("ID"))
                    dgvAppointments.Columns["ID"].Visible = false;

                AddActionColumns();
                ApplyRowColors();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private string ConvertStatusToDb(string displayStatus)
        {
            switch (displayStatus)
            {
                case "Đã đặt": return "booked";
                case "Chờ BS xác nhận": return "pending";
                case "Bị từ chối": return "rejected";
                case "Đã xác nhận": return "confirmed";
                case "Hoàn thành": return "completed";
                case "Đã hủy": return "cancelled";
                default: return "booked";
            }
        }

        private void AddActionColumns()
        {
            if (!dgvAppointments.Columns.Contains("Process"))
            {
                dgvAppointments.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Process",
                    HeaderText = "",
                    Text = "Kê khai",
                    UseColumnTextForButtonValue = true,
                    Width = 100
                });
            }

            if (!dgvAppointments.Columns.Contains("Cancel"))
            {
                dgvAppointments.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Cancel",
                    HeaderText = "",
                    Text = "Hủy",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                });
            }
        }

        private void ApplyRowColors()
        {
            foreach (DataGridViewRow row in dgvAppointments.Rows)
            {
                if (row.IsNewRow) continue;

                string status = row.Cells["Trạng thái"].Value?.ToString();

                switch (status)
                {
                    case "Đã đặt":
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFF3CD");
                        break;
                    case "Chờ BS xác nhận":
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D1ECF1");
                        break;
                    case "Đã xác nhận":
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#D4EDDA");
                        break;
                    case "Hoàn thành":
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                        break;
                    case "Bị từ chối":
                    case "Đã hủy":
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                        break;
                }
            }
        }

        private void DgvAppointments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                // Validate cell value trước khi convert
                var idCell = dgvAppointments.Rows[e.RowIndex].Cells["ID"];
                if (idCell == null || idCell.Value == null || idCell.Value == DBNull.Value)
                {
                    MessageBoxHelper.ShowError("Không thể xác định lịch hẹn!");
                    return;
                }

                int appointmentId = Convert.ToInt32(idCell.Value);
                string status = dgvAppointments.Rows[e.RowIndex].Cells["Trạng thái"].Value?.ToString();
                string columnName = dgvAppointments.Columns[e.ColumnIndex].Name;

                if (columnName == "Process")
                {
                    if (status != "Đã đặt")
                    {
                        MessageBoxHelper.ShowWarning("Chỉ có thể kê khai cho lịch hẹn có trạng thái 'Đã đặt'!");
                        return;
                    }
                    ProcessAppointment(appointmentId);
                }
                else if (columnName == "Cancel")
                {
                    // Chỉ cho phép hủy lịch hẹn ở trạng thái 'Đã đặt' và 'Chờ BS xác nhận'
                    if (status != "Đã đặt" && status != "Chờ BS xác nhận")
                    {
                        MessageBoxHelper.ShowWarning("Chỉ có thể hủy lịch hẹn ở trạng thái 'Đã đặt' hoặc 'Chờ BS xác nhận'!\n\n" +
                                                   "Lịch đã xác nhận/đang khám/hoàn thành không được hủy.");
                        return;
                    }
                    CancelAppointment(appointmentId);
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi xử lý thao tác: {ex.Message}");
            }
        }

        private void ProcessAppointment(int appointmentId)
        {
            // Navigate to StaffPatientIntake with appointmentId
            // For now, show message
            MessageBoxHelper.ShowInfo($"Chuyển đến trang Kê khai bệnh nhân cho lịch hẹn #{appointmentId}\n\n" +
                                     "Chức năng này sẽ mở trang StaffPatientIntake.");

            // TODO: Implement navigation
            // var intakePage = new StaffPatientIntake(appointmentId);
            // NavigateToPage(intakePage);
        }

        private void CancelAppointment(int appointmentId)
        {
            if (!MessageBoxHelper.ShowConfirm("Xác nhận hủy lịch hẹn này?"))
                return;

            try
            {
                string query = "UPDATE Appointment SET status = N'cancelled' WHERE appointment_id = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", appointmentId)
                });

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đã hủy lịch hẹn!");
                    Logger.LogAction("CANCEL_APPOINTMENT", $"Staff hủy lịch hẹn #{appointmentId}");
                    LoadAppointments();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadAppointments();
        }

        private void CboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAppointments();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboStatus.SelectedIndex = 0;
            LoadAppointments();
        }
    }
}