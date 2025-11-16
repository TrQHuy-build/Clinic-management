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
                    cboGender.SelectedItem = dt.Rows[0]["gender"].ToString();
                    txtAddress.Text = dt.Rows[0]["address"].ToString();
                    txtInsurance.Text = dt.Rows[0]["insurance"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Validator.IsNotEmpty(txtFullname.Text) || !Validator.IsValidPhone(txtPhone.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập đầy đủ thông tin!");
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