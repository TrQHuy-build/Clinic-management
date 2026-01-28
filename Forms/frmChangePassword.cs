using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Forms
{
    public partial class frmChangePassword : Form
    {
        public frmChangePassword()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate
            if (string.IsNullOrEmpty(currentPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu hiện tại!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCurrentPassword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            // Kiểm tra độ mạnh password
            var (isValid, errorMessage) = PasswordHasher.ValidatePasswordStrength(newPassword);
            if (!isValid)
            {
                MessageBox.Show(errorMessage, "Mật khẩu không đủ mạnh",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            try
            {
                // Lấy password hash hiện tại
                string query = "SELECT password_hash FROM UserAccount WHERE user_id = @userId";
                SqlParameter[] parameters = {
                    new SqlParameter("@userId", Auth.CurrentUserId)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy tài khoản!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string currentHash = dt.Rows[0]["password_hash"].ToString();

                // Verify mật khẩu hiện tại
                if (!PasswordHasher.VerifyPassword(currentPassword, currentHash))
                {
                    // Nếu là format cũ, thử so sánh plain text
                    if (!(PasswordHasher.IsOldFormat(currentHash) && currentHash == currentPassword))
                    {
                        MessageBox.Show("Mật khẩu hiện tại không đúng!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCurrentPassword.Focus();
                        return;
                    }
                }

                // Hash mật khẩu mới
                string newHash = PasswordHasher.HashPassword(newPassword);

                // Update vào database
                string updateQuery = "UPDATE UserAccount SET password_hash = @newHash WHERE user_id = @userId";
                SqlParameter[] updateParams = {
                    new SqlParameter("@newHash", newHash),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                };

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);

                if (rowsAffected > 0)
                {
                    // Ghi log
                    Logger.LogAction("CHANGE_PASSWORD", $"{Auth.CurrentUserName} đã đổi mật khẩu");

                    MessageBox.Show("Đổi mật khẩu thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể đổi mật khẩu. Vui lòng thử lại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnToggleCurrent_Click(object sender, EventArgs e)
        {
            txtCurrentPassword.UseSystemPasswordChar = !txtCurrentPassword.UseSystemPasswordChar;
            btnToggleCurrent.Text = txtCurrentPassword.UseSystemPasswordChar ? "👁" : "🙈";
        }

        private void BtnToggleNew_Click(object sender, EventArgs e)
        {
            txtNewPassword.UseSystemPasswordChar = !txtNewPassword.UseSystemPasswordChar;
            btnToggleNew.Text = txtNewPassword.UseSystemPasswordChar ? "👁" : "🙈";
        }

        private void BtnToggleConfirm_Click(object sender, EventArgs e)
        {
            txtConfirmPassword.UseSystemPasswordChar = !txtConfirmPassword.UseSystemPasswordChar;
            btnToggleConfirm.Text = txtConfirmPassword.UseSystemPasswordChar ? "👁" : "🙈";
        }
    }
}
