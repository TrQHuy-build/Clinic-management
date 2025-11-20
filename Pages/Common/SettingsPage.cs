using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Common
{
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            try
            {
                string query = "SELECT fullname, phone, email FROM UserAccount WHERE user_id = @userId";
                var dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@userId", Auth.CurrentUserId) });
                if (dt.Rows.Count > 0)
                {
                    txtFullname.Text = dt.Rows[0]["fullname"].ToString();
                    txtPhone.Text = dt.Rows[0]["phone"].ToString();
                    txtEmail.Text = dt.Rows[0]["email"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải thông tin: {ex.Message}");
            }
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            string fullname = txtFullname.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (!Validator.IsNotEmpty(fullname))
            {
                MessageBoxHelper.ShowValidationError("họ và tên");
                txtFullname.Focus();
                return;
            }

            if (!Validator.IsNotEmpty(phone))
            {
                MessageBoxHelper.ShowValidationError("số điện thoại");
                txtPhone.Focus();
                return;
            }

            if (!Validator.IsValidPhone(phone))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không hợp lệ!");
                txtPhone.Focus();
                return;
            }

            try
            {
                string checkQuery = "SELECT COUNT(*) FROM UserAccount WHERE phone = @phone AND user_id != @userId";
                object count = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter[]
                {
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (Convert.ToInt32(count) > 0)
                {
                    MessageBoxHelper.ShowDuplicateError("Số điện thoại");
                    return;
                }

                string updateQuery = "UPDATE UserAccount SET fullname = @fullname, phone = @phone WHERE user_id = @userId";
                int result = DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[]
                {
                    new SqlParameter("@fullname", fullname),
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (result > 0)
                {
                    Auth.CurrentUserName = fullname;
                    MessageBoxHelper.ShowUpdateSuccess("thông tin cá nhân");
                    Logger.LogUpdate("UserProfile", $"Cập nhật: {fullname} - {phone}");
                }
                else
                {
                    MessageBoxHelper.ShowDatabaseError("cập nhật thông tin");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi cập nhật: {ex.Message}");
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            string oldPassword = txtOldPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (!Validator.IsNotEmpty(oldPassword))
            {
                MessageBoxHelper.ShowValidationError("mật khẩu cũ");
                txtOldPassword.Focus();
                return;
            }

            if (!Validator.IsValidPassword(newPassword))
            {
                MessageBoxHelper.ShowValidationError("Mật khẩu mới phải có ít nhất 6 ký tự!");
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBoxHelper.ShowValidationError("Xác nhận mật khẩu không khớp!");
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                string checkQuery = "SELECT COUNT(*) FROM UserAccount WHERE user_id = @userId AND password_hash = @oldPassword";
                object count = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter[]
                {
                    new SqlParameter("@userId", Auth.CurrentUserId),
                    new SqlParameter("@oldPassword", oldPassword)
                });

                if (Convert.ToInt32(count) == 0)
                {
                    MessageBoxHelper.ShowError("Mật khẩu cũ không đúng!");
                    txtOldPassword.Focus();
                    return;
                }

                string updateQuery = "UPDATE UserAccount SET password_hash = @newPassword WHERE user_id = @userId";
                int result = DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[]
                {
                    new SqlParameter("@newPassword", newPassword),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đổi mật khẩu thành công!");
                    Logger.LogPasswordChange();
                    txtOldPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmPassword.Clear();
                }
                else
                {
                    MessageBoxHelper.ShowDatabaseError("đổi mật khẩu");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi đổi mật khẩu: {ex.Message}");
            }
        }
    }
}