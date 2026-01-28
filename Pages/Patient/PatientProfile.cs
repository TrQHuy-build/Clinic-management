using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientProfile : UserControl
    {
        public PatientProfile()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            if (!Auth.CurrentPatientId.HasValue) return;

            try
            {
                string query = @"
                    SELECT u.*, p.* 
                    FROM Patient p 
                    INNER JOIN UserAccount u ON p.user_id = u.user_id 
                    WHERE p.patient_id = @id";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", Auth.CurrentPatientId.Value) });

                if (dt.Rows.Count > 0)
                {
                    txtFullname.Text = dt.Rows[0]["fullname"].ToString();
                    txtPhone.Text = dt.Rows[0]["phone"].ToString();
                    if (dt.Rows[0]["date_of_birth"] != DBNull.Value)
                        dtpDOB.Value = Convert.ToDateTime(dt.Rows[0]["date_of_birth"]);
                    if (dt.Rows[0]["gender"] != DBNull.Value)
                        cboGender.SelectedItem = dt.Rows[0]["gender"].ToString();
                    txtAddress.Text = dt.Rows[0]["address"] != DBNull.Value ? dt.Rows[0]["address"].ToString() : "";
                    txtInsurance.Text = dt.Rows[0]["insurance"] != DBNull.Value ? dt.Rows[0]["insurance"].ToString() : "";
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // ✅ Validate Fullname
            if (!ValidationHelper.IsNotEmpty(txtFullname.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập họ tên!");
                txtFullname.Focus();
                return;
            }

            if (!ValidationHelper.IsValidFullname(txtFullname.Text))
            {
                MessageBoxHelper.ShowValidationError("Họ tên chỉ được chứa chữ cái, khoảng trắng và dấu chấm!");
                txtFullname.Focus();
                return;
            }

            // ✅ Validate Phone
            if (!ValidationHelper.IsNotEmpty(txtPhone.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập số điện thoại!");
                txtPhone.Focus();
                return;
            }

            if (!ValidationHelper.IsValidVietnamPhone(txtPhone.Text))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại phải là 10-11 chữ số và bắt đầu bằng 0 (VD: 0901234567)!");
                txtPhone.Focus();
                return;
            }

            // ✅ Validate Date of Birth
            if (!ValidationHelper.IsValidDateOfBirth(dtpDOB.Value))
            {
                MessageBoxHelper.ShowValidationError(ValidationHelper.GetDateOfBirthErrorMessage(dtpDOB.Value));
                return;
            }

            // ✅ Validate Gender
            if (cboGender.SelectedItem == null || string.IsNullOrWhiteSpace(cboGender.SelectedItem.ToString()))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng chọn giới tính!");
                cboGender.Focus();
                return;
            }

            try
            {
                // Update UserAccount
                string updateUser = "UPDATE UserAccount SET fullname=@name, phone=@phone WHERE user_id=@userId";
                DatabaseHelper.ExecuteNonQuery(updateUser, new SqlParameter[] {
                    new SqlParameter("@name", txtFullname.Text.Trim()),
                    new SqlParameter("@phone", txtPhone.Text.Trim()),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                // Update Patient
                string updatePatient = "UPDATE Patient SET date_of_birth=@dob, gender=@gender, address=@addr, insurance=@ins WHERE patient_id=@id";
                DatabaseHelper.ExecuteNonQuery(updatePatient, new SqlParameter[] {
                    new SqlParameter("@dob", dtpDOB.Value),
                    new SqlParameter("@gender", cboGender.SelectedItem.ToString()),
                    new SqlParameter("@addr", txtAddress.Text.Trim()),
                    new SqlParameter("@ins", txtInsurance.Text.Trim()),
                    new SqlParameter("@id", Auth.CurrentPatientId.Value)
                });

                MessageBoxHelper.ShowUpdateSuccess("thông tin cá nhân");
                Logger.LogUpdate("PatientProfile", txtFullname.Text);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}