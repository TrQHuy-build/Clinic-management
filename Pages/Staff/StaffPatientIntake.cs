using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Staff
{
    public partial class StaffPatientIntake : UserControl
    {
        private int? currentAppointmentId = null;

        public StaffPatientIntake()
        {
            InitializeComponent();
            LoadPendingAppointments();
            LoadDoctors();
            InitializeRealtimeValidation();
        }

        private void InitializeRealtimeValidation()
        {
            // Realtime validation cho các field
            txtPhone.TextChanged += TxtPhone_TextChanged;
            txtEmail.TextChanged += TxtEmail_TextChanged;
            txtName.TextChanged += TxtName_TextChanged;
            txtAddress.TextChanged += TxtAddress_TextChanged;
        }

        private void TxtPhone_TextChanged(object sender, EventArgs e)
        {
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone))
            {
                txtPhone.BackColor = Color.White;
                return;
            }

            // Validate phone format (10-11 số, bắt đầu bằng 0)
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9,10}$"))
            {
                txtPhone.BackColor = ColorTranslator.FromHtml("#FFEBEE");
            }
            else
            {
                txtPhone.BackColor = Color.White;
            }
        }

        private void TxtEmail_TextChanged(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                txtEmail.BackColor = Color.White;
                return;
            }

            // Validate email format
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                txtEmail.BackColor = ColorTranslator.FromHtml("#FFEBEE");
            }
            else
            {
                txtEmail.BackColor = Color.White;
            }
        }

        private void TxtName_TextChanged(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                txtName.BackColor = ColorTranslator.FromHtml("#FFEBEE");
            }
            else
            {
                txtName.BackColor = Color.White;
            }
        }

        private void TxtAddress_TextChanged(object sender, EventArgs e)
        {
            string address = txtAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(address) || address.Length < 5)
            {
                txtAddress.BackColor = ColorTranslator.FromHtml("#FFEBEE");
            }
            else
            {
                txtAddress.BackColor = Color.White;
            }
        }

        private void LoadPendingAppointments()
        {
            try
            {
                string query = @"
                    SELECT 
                        appointment_id,
                        patient_name + ' - ' + phone + ' (' + FORMAT(appointment_date, 'dd/MM HH:mm') + ')' AS display
                    FROM Appointment
                    WHERE status = 'booked'
                    ORDER BY appointment_date";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                cboAppointment.DisplayMember = "display";
                cboAppointment.ValueMember = "appointment_id";
                cboAppointment.DataSource = dt;
                cboAppointment.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải lịch hẹn: {ex.Message}");
            }
        }

        private void LoadDoctors()
        {
            try
            {
                string query = @"
                    SELECT s.staff_id, u.fullname + ' - ' + ISNULL(s.specialization, N'Nha khoa') AS display
                    FROM Staff s
                    INNER JOIN UserAccount u ON s.user_id = u.user_id
                    WHERE u.role = 'doctor' AND u.status = 'active'
                    ORDER BY u.fullname";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                cboDoctor.DisplayMember = "display";
                cboDoctor.ValueMember = "staff_id";
                cboDoctor.DataSource = dt;
                cboDoctor.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải danh sách bác sĩ: {ex.Message}");
            }
        }

        private void CboAppointment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAppointment.SelectedValue == null) return;

            try
            {
                int appointmentId = Convert.ToInt32(cboAppointment.SelectedValue);
                currentAppointmentId = appointmentId;

                // Load appointment details
                string query = "SELECT * FROM Appointment WHERE appointment_id = @id";
                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", appointmentId)
                });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtName.Text = row["patient_name"]?.ToString() ?? "";
                    txtPhone.Text = row["phone"]?.ToString() ?? "";
                    txtEmail.Text = row["email"]?.ToString() ?? "";

                    // Clear patient details
                    ClearPatientDetails();

                    // Try to find existing patient by phone
                    SearchExistingPatient(txtPhone.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void SearchExistingPatient(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return;

            try
            {
                string query = @"
                    SELECT p.patient_id, u.fullname, p.date_of_birth, p.gender, p.address, p.insurance
                    FROM Patient p
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE u.phone = @phone";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
                    new SqlParameter("@phone", phone)
                });

                if (dt.Rows.Count > 0)
                {
                    lblExistingPatient.Text = "✅ Tìm thấy bệnh nhân cũ! Thông tin sẽ được điền tự động.";
                    lblExistingPatient.ForeColor = Color.Green;
                    lblExistingPatient.Visible = true;

                    DataRow row = dt.Rows[0];

                    if (row["date_of_birth"] != DBNull.Value)
                        dtpDateOfBirth.Value = Convert.ToDateTime(row["date_of_birth"]);

                    string gender = row["gender"]?.ToString();
                    if (gender == "Male") cboGender.SelectedIndex = 0;
                    else if (gender == "Female") cboGender.SelectedIndex = 1;
                    else cboGender.SelectedIndex = 2;

                    txtAddress.Text = row["address"]?.ToString() ?? "";
                    txtInsurance.Text = row["insurance"]?.ToString() ?? "";
                }
                else
                {
                    lblExistingPatient.Text = "ℹ️ Bệnh nhân mới - vui lòng điền đầy đủ thông tin.";
                    lblExistingPatient.ForeColor = Color.Blue;
                    lblExistingPatient.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tìm kiếm: {ex.Message}");
            }
        }

        private void ClearPatientDetails()
        {
            dtpDateOfBirth.Value = DateTime.Now.AddYears(-25);
            cboGender.SelectedIndex = -1;
            txtAddress.Clear();
            txtInsurance.Clear();
            lblExistingPatient.Visible = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            if (!currentAppointmentId.HasValue)
            {
                MessageBoxHelper.ShowValidationError("Vui lòng chọn lịch hẹn!");
                return;
            }

            if (cboDoctor.SelectedValue == null)
            {
                MessageBoxHelper.ShowValidationError("Vui lòng chọn bác sĩ!");
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        try
                        {
                            int? patientId = null;

                            // Check if patient exists by phone
                            string checkQuery = @"
                                SELECT p.patient_id 
                                FROM Patient p
                                INNER JOIN UserAccount u ON p.user_id = u.user_id
                                WHERE u.phone = @phone";

                            SqlCommand cmdCheck = new SqlCommand(checkQuery, conn, tran);
                            cmdCheck.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                            object existingPatientId = cmdCheck.ExecuteScalar();

                            if (existingPatientId != null)
                            {
                                // Patient exists - update info
                                patientId = Convert.ToInt32(existingPatientId);

                                string updateQuery = @"
                                    UPDATE Patient 
                                    SET date_of_birth = @dob,
                                        gender = @gender,
                                        address = @address,
                                        insurance = @insurance
                                    WHERE patient_id = @patientId";

                                SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn, tran);
                                cmdUpdate.Parameters.AddWithValue("@dob", dtpDateOfBirth.Value.Date);
                                cmdUpdate.Parameters.AddWithValue("@gender", cboGender.SelectedItem.ToString());
                                cmdUpdate.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                                cmdUpdate.Parameters.AddWithValue("@insurance", string.IsNullOrWhiteSpace(txtInsurance.Text) ? (object)DBNull.Value : txtInsurance.Text.Trim());
                                cmdUpdate.Parameters.AddWithValue("@patientId", patientId);
                                cmdUpdate.ExecuteNonQuery();
                            }
                            else
                            {
                                // New patient - check duplicate phone/email first
                                string checkDuplicateQuery = @"
                                    SELECT COUNT(*) FROM UserAccount 
                                    WHERE phone = @phone OR (@email IS NOT NULL AND email = @email)";

                                SqlCommand cmdCheckDup = new SqlCommand(checkDuplicateQuery, conn, tran);
                                cmdCheckDup.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                                cmdCheckDup.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim());

                                int duplicateCount = Convert.ToInt32(cmdCheckDup.ExecuteScalar());
                                if (duplicateCount > 0)
                                {
                                    tran.Rollback();
                                    MessageBoxHelper.ShowError("Số điện thoại hoặc email đã tồn tại trong hệ thống!\nVui lòng kiểm tra lại.");
                                    return;
                                }

                                // Create UserAccount + Patient
                                string insertUser = @"
                                    INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status)
                                    OUTPUT INSERTED.user_id
                                    VALUES (@fullname, @phone, @email, 'default_hash', 'patient', 'active')";

                                SqlCommand cmdUser = new SqlCommand(insertUser, conn, tran);
                                cmdUser.Parameters.AddWithValue("@fullname", txtName.Text.Trim());
                                cmdUser.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                                cmdUser.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim());
                                int userId = Convert.ToInt32(cmdUser.ExecuteScalar());

                                string insertPatient = @"
                                    INSERT INTO Patient (user_id, date_of_birth, gender, address, insurance)
                                    OUTPUT INSERTED.patient_id
                                    VALUES (@userId, @dob, @gender, @address, @insurance)";

                                SqlCommand cmdPatient = new SqlCommand(insertPatient, conn, tran);
                                cmdPatient.Parameters.AddWithValue("@userId", userId);
                                cmdPatient.Parameters.AddWithValue("@dob", dtpDateOfBirth.Value.Date);
                                cmdPatient.Parameters.AddWithValue("@gender", cboGender.SelectedItem.ToString());
                                cmdPatient.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                                cmdPatient.Parameters.AddWithValue("@insurance", string.IsNullOrWhiteSpace(txtInsurance.Text) ? (object)DBNull.Value : txtInsurance.Text.Trim());
                                patientId = Convert.ToInt32(cmdPatient.ExecuteScalar());
                            }

                            // Update Appointment
                            string updateAppointment = @"
                                UPDATE Appointment
                                SET patient_id = @patientId,
                                    assigned_doctor_id = @doctorId,
                                    status = N'pending'
                                WHERE appointment_id = @appointmentId";

                            SqlCommand cmdAppointment = new SqlCommand(updateAppointment, conn, tran);
                            cmdAppointment.Parameters.AddWithValue("@patientId", patientId);
                            cmdAppointment.Parameters.AddWithValue("@doctorId", Convert.ToInt32(cboDoctor.SelectedValue));
                            cmdAppointment.Parameters.AddWithValue("@appointmentId", currentAppointmentId.Value);
                            cmdAppointment.ExecuteNonQuery();

                            tran.Commit();

                            MessageBoxHelper.ShowSuccess("Đã kê khai thông tin và gửi yêu cầu đến bác sĩ!");
                            Logger.LogAction("PATIENT_INTAKE", $"Kê khai BN: {txtName.Text} - Lịch #{currentAppointmentId}");

                            ClearForm();
                            LoadPendingAppointments();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi lưu dữ liệu: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            // Validate Name
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBoxHelper.ShowValidationError("Tên bệnh nhân không được để trống!");
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageBoxHelper.ShowValidationError("Tên bệnh nhân phải có ít nhất 2 ký tự!");
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length > 100)
            {
                MessageBoxHelper.ShowValidationError("Tên bệnh nhân không được vượt quá 100 ký tự!");
                txtName.Focus();
                return false;
            }

            // Validate Phone
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không được để trống!");
                txtPhone.Focus();
                return false;
            }

            string phone = txtPhone.Text.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0\d{9,10}$"))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không hợp lệ!\nVui lòng nhập 10-11 số, bắt đầu bằng 0.");
                txtPhone.Focus();
                return false;
            }

            // Validate Email (optional nhưng nếu có thì phải đúng format)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBoxHelper.ShowValidationError("Email không hợp lệ!");
                    txtEmail.Focus();
                    return false;
                }
            }

            // Validate Gender
            if (cboGender.SelectedIndex < 0)
            {
                MessageBoxHelper.ShowValidationError("Vui lòng chọn giới tính!");
                cboGender.Focus();
                return false;
            }

            // Validate Address
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBoxHelper.ShowValidationError("Địa chỉ không được để trống!");
                txtAddress.Focus();
                return false;
            }

            if (txtAddress.Text.Trim().Length < 5)
            {
                MessageBoxHelper.ShowValidationError("Địa chỉ phải có ít nhất 5 ký tự!");
                txtAddress.Focus();
                return false;
            }

            // Validate Date of Birth
            if (dtpDateOfBirth.Value > DateTime.Now)
            {
                MessageBoxHelper.ShowValidationError("Ngày sinh không thể là ngày trong tương lai!");
                dtpDateOfBirth.Focus();
                return false;
            }

            int age = DateTime.Now.Year - dtpDateOfBirth.Value.Year;
            if (age < 0 || age > 150)
            {
                MessageBoxHelper.ShowValidationError("Ngày sinh không hợp lệ! Tuổi phải từ 0-150.");
                dtpDateOfBirth.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            cboAppointment.SelectedIndex = -1;
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            ClearPatientDetails();
            cboDoctor.SelectedIndex = -1;
            currentAppointmentId = null;

            // Reset background colors
            txtName.BackColor = Color.White;
            txtPhone.BackColor = Color.White;
            txtEmail.BackColor = Color.White;
            txtAddress.BackColor = Color.White;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}