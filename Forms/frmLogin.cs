using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Forms
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            btnTogglePassword.FlatStyle = FlatStyle.Flat; 
            btnTogglePassword.FlatAppearance.BorderSize = 0; 
            btnTogglePassword.Cursor = Cursors.Hand;
            txtPassword.UseSystemPasswordChar = true;
            btnTogglePassword.Text = "👁";

              

        }
        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;

            if (txtPassword.UseSystemPasswordChar)
                btnTogglePassword.Text = "🙈";   // Đang ẩn
            else
                btnTogglePassword.Text = "👁";  // Đang hiện
        }


        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validate
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                // Kiểm tra kết nối database
                if (!DatabaseHelper.TestConnection())
                {
                    return;
                }

                // Query đăng nhập - CHỈ LẤY USER THEO EMAIL
                string query = @"
                    SELECT u.user_id, u.fullname, u.role, u.email, u.status, u.password_hash,
                           s.staff_id, p.patient_id
                    FROM UserAccount u
                    LEFT JOIN Staff s ON u.user_id = s.user_id
                    LEFT JOIN Patient p ON u.user_id = p.user_id
                    WHERE u.email = @email";

                SqlParameter[] parameters = {
                    new SqlParameter("@email", email)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Email hoặc mật khẩu không đúng!", "Đăng nhập thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = dt.Rows[0];
                string storedHash = row["password_hash"].ToString();

                // VERIFY PASSWORD VỚI HASH
                if (!PasswordHasher.VerifyPassword(password, storedHash))
                {
                    // Nếu verify failed và là format cũ, thử so sánh plain text (backward compatibility)
                    if (PasswordHasher.IsOldFormat(storedHash) && storedHash == password)
                    {
                        // Password đúng nhưng là format cũ - cần update
                        MessageBox.Show(
                            "Mật khẩu của bạn đang dùng định dạng cũ (không an toàn).\n\n" +
                            "Hệ thống sẽ tự động cập nhật sang định dạng mới sau khi đăng nhập.\n\n" +
                            "Lần đăng nhập tiếp theo bạn sẽ dùng mật khẩu này nhưng đã được mã hóa an toàn.",
                            "Cảnh báo bảo mật",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        // Update password sang format mới
                        int updateUserId = Convert.ToInt32(row["user_id"]);
                        string newHash = PasswordHasher.HashPassword(password);
                        string updateQuery = "UPDATE UserAccount SET password_hash = @newHash WHERE user_id = @userId";
                        SqlParameter[] updateParams = {
                            new SqlParameter("@newHash", newHash),
                            new SqlParameter("@userId", updateUserId)
                        };
                        DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);
                    }
                    else
                    {
                        MessageBox.Show("Email hoặc mật khẩu không đúng!", "Đăng nhập thất bại",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string status = row["status"].ToString();

                if (status.ToLower() != "active")
                {
                    MessageBox.Show("Tài khoản đã bị khóa!", "Đăng nhập thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lưu thông tin user
                int userId = Convert.ToInt32(row["user_id"]);
                string fullname = row["fullname"].ToString();
                string role = row["role"].ToString();
                int? staffId = row["staff_id"] != DBNull.Value ? (int?)Convert.ToInt32(row["staff_id"]) : null;
                int? patientId = row["patient_id"] != DBNull.Value ? (int?)Convert.ToInt32(row["patient_id"]) : null;

                Auth.Login(userId, fullname, role, email, staffId, patientId);

                // Ghi log
                string logQuery = "INSERT INTO AuditLog (user_id, action, description) VALUES (@userId, @action, @desc)";
                SqlParameter[] logParams = {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@action", "LOGIN"),
                    new SqlParameter("@desc", $"{fullname} đăng nhập hệ thống")
                };
                DatabaseHelper.ExecuteNonQuery(logQuery, logParams);

                // Mở form chính
                MessageBox.Show($"Chào mừng {fullname}!", "Đăng nhập thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                frmMain mainForm = new frmMain();
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            
            Application.Exit();
        }


        private void lblSubtitle_Click(object sender, EventArgs e)
        {

        }



        private void txtPassword_TextChanged(object sender, EventArgs e)

        {

        }

        private void rightPanel_Paint(object sender, PaintEventArgs e)
        {
           
        }

        private void lblWelcome_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}