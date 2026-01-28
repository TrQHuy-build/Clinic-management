using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Forms
{
    public partial class frmQuenMatKhau : Form
    {
        public frmQuenMatKhau()
        {
            InitializeComponent();
        }

        private void frmQuenMatKhau_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            lblStatus.ForeColor = System.Drawing.Color.Black;
        }

        private void btnGuiOTP_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            // Validate email
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            try
            {
                // Kiểm tra email có tồn tại trong database
                string query = "SELECT user_id, fullname FROM UserAccount WHERE email = @email";
                SqlParameter[] parameters = {
                    new SqlParameter("@email", email)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Email này không được đăng ký trong hệ thống!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int userId = Convert.ToInt32(dt.Rows[0]["user_id"]);
                string fullname = dt.Rows[0]["fullname"].ToString();

                // Generate OTP (6 digits)
                string otp = GenerateOTP();
                
                // Store OTP in session (in real app, should store with expiration time in DB)
                Session.OTP = otp;
                Session.OTPEmail = email;
                Session.OTPUserId = userId;

                // TODO: Send OTP to email (implement email service)
                // For now, show OTP in label for testing
                lblStatus.Text = $"✓ Mã OTP đã được gửi đến {email}\n(Mã OTP: {otp})";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                // Enable OTP input controls
                txtOTP.Enabled = true;
                btnXacNhanOTP.Enabled = true;
                btnGuiOTP.Enabled = false;
                txtEmail.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXacNhanOTP_Click(object sender, EventArgs e)
        {
            string enteredOTP = txtOTP.Text.Trim();

            // Validate OTP
            if (string.IsNullOrEmpty(enteredOTP))
            {
                MessageBox.Show("Vui lòng nhập mã OTP!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOTP.Focus();
                return;
            }

            if (enteredOTP != Session.OTP)
            {
                MessageBox.Show("Mã OTP không chính xác!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtOTP.Clear();
                txtOTP.Focus();
                return;
            }

            // Show password reset section
            pnlPasswordReset.Visible = true;
            lblStatus.Text = "✓ Xác nhận OTP thành công!";
            lblStatus.ForeColor = System.Drawing.Color.Green;
            txtOTP.Enabled = false;
            btnXacNhanOTP.Enabled = false;
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Validate passwords
            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Hash the new password
                string passwordHash = PasswordHasher.HashPassword(newPassword);

                // Update password in database
                string query = "UPDATE UserAccount SET password_hash = @passwordHash WHERE user_id = @userId";
                SqlParameter[] parameters = {
                    new SqlParameter("@passwordHash", passwordHash),
                    new SqlParameter("@userId", Session.OTPUserId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);

                // Clear session
                Session.OTP = null;
                Session.OTPEmail = null;
                Session.OTPUserId = 0;

                MessageBox.Show("Mật khẩu đã được thay đổi thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật mật khẩu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.OTP = null;
            Session.OTPEmail = null;
            Session.OTPUserId = 0;
            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnToggleNewPassword_Click(object sender, EventArgs e)
        {
            txtNewPassword.UseSystemPasswordChar = !txtNewPassword.UseSystemPasswordChar;
            btnToggleNewPassword.Text = txtNewPassword.UseSystemPasswordChar ? "🙈" : "👁";
        }

        private void btnToggleConfirmPassword_Click(object sender, EventArgs e)
        {
            txtConfirmPassword.UseSystemPasswordChar = !txtConfirmPassword.UseSystemPasswordChar;
            btnToggleConfirmPassword.Text = txtConfirmPassword.UseSystemPasswordChar ? "🙈" : "👁";
        }

        // Generate 6-digit OTP
        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}