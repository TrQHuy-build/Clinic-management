using System;
using System.Data;
using System.Data.SqlClient;
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

                // Query đăng nhập
                string query = @"
                    SELECT u.user_id, u.fullname, u.role, u.email, u.status,
                           s.staff_id, p.patient_id
                    FROM UserAccount u
                    LEFT JOIN Staff s ON u.user_id = s.user_id
                    LEFT JOIN Patient p ON u.user_id = p.user_id
                    WHERE u.email = @email AND u.password_hash = @password";

                SqlParameter[] parameters = {
                    new SqlParameter("@email", email),
                    new SqlParameter("@password", password)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Email hoặc mật khẩu không đúng!", "Đăng nhập thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = dt.Rows[0];
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
    }
}