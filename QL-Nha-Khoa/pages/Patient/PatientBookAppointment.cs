using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientBookAppointment : UserControl
    {
        public PatientBookAppointment()
        {
            InitializeComponent();
            this.Load += PatientBookAppointment_Load;
        }

        private void PatientBookAppointment_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
            LoadServices();
        }

        private void LoadUserInfo()
        {
            if (txtPatientName == null || txtPhone == null || txtEmail == null) return;

            if (!Auth.CurrentUserId.HasValue)
            {
                MessageBoxHelper.ShowError("Không tìm thấy thông tin người dùng!");
                return;
            }

            try
            {
                string query = "SELECT fullname, phone, email FROM UserAccount WHERE user_id = @userId";
                SqlParameter[] parameters = { new SqlParameter("@userId", Auth.CurrentUserId.Value) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    txtPatientName.Text = dt.Rows[0]["fullname"]?.ToString() ?? "";
                    txtPhone.Text = dt.Rows[0]["phone"]?.ToString() ?? "";
                    txtEmail.Text = dt.Rows[0]["email"]?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải thông tin: {ex.Message}");
            }
        }

        private void LoadServices()
        {
            if (cboService == null) return;

            try
            {
                string query = @"
                    SELECT service_id, service_name + ' - ' + CAST(price AS VARCHAR) + 'đ' AS display_name
                    FROM Service
                    WHERE status = N'available'
                    ORDER BY service_name";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                cboService.DisplayMember = "display_name";
                cboService.ValueMember = "service_id";
                cboService.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dịch vụ: {ex.Message}");
            }
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (txtPatientName == null || cboService == null) return;

            if (!Validator.IsNotEmpty(txtPatientName.Text.Trim()))
            {
                MessageBoxHelper.ShowValidationError("Họ và tên");
                txtPatientName.Focus();
                return;
            }

            if (!Validator.IsValidPhone(txtPhone.Text.Trim()))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không hợp lệ!");
                txtPhone.Focus();
                return;
            }

            if (cboService.SelectedValue == null)
            {
                MessageBoxHelper.ShowValidationError("Dịch vụ");
                cboService.Focus();
                return;
            }

            DateTime appointmentDate = dtpDate.Value.Date
                .AddHours(int.Parse(cboHour.SelectedItem.ToString()))
                .AddMinutes(int.Parse(cboMinute.SelectedItem.ToString()));

            if (!Validator.IsValidFutureDateTime(appointmentDate))
            {
                MessageBoxHelper.ShowValidationError("Giờ hẹn phải là thời gian trong tương lai!");
                return;
            }

            try
            {
                string query = @"
                    INSERT INTO Appointment (patient_name, phone, email, service_id, appointment_date, status, notes)
                    VALUES (@name, @phone, @email, @serviceId, @date, N'booked', @notes)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", txtPatientName.Text.Trim()),
                    new SqlParameter("@phone", txtPhone.Text.Trim()),
                    new SqlParameter("@email", txtEmail.Text.Trim()),
                    new SqlParameter("@serviceId", cboService.SelectedValue),
                    new SqlParameter("@date", appointmentDate),
                    new SqlParameter("@notes", txtNotes.Text.Trim())
                };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess(
                        $"Đặt lịch hẹn thành công!\n\n" +
                        $"Thời gian: {appointmentDate:dd/MM/yyyy HH:mm}\n" +
                        $"Dịch vụ: {cboService.Text}\n\n" +
                        $"Vui lòng đến đúng giờ hẹn!");

                    Logger.LogBookAppointment(txtPatientName.Text, appointmentDate);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi đặt lịch: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            if (txtNotes != null) txtNotes.Clear();
            if (dtpDate != null) dtpDate.Value = DateTime.Today.AddDays(1);
            if (cboHour != null) cboHour.SelectedIndex = 0;
            if (cboMinute != null) cboMinute.SelectedIndex = 0;
        }
    }
}