using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập email và mật khẩu");
                return;
            }

            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                con.Open();
                using var cmd = new SqlCommand("SELECT user_id, fullname, email, password_hash, role FROM UserAccount WHERE email = @email AND status = N'active'", con);
                cmd.Parameters.AddWithValue("@email", email);
                using var rdr = cmd.ExecuteReader();
                if (!rdr.Read())
                {
                    MessageBox.Show("Tài khoản không tồn tại hoặc đã bị khoá");
                    return;
                }

                var userId = rdr.GetInt32(rdr.GetOrdinal("user_id"));
                var fullname = rdr.GetString(rdr.GetOrdinal("fullname"));
                var dbEmail = rdr.GetString(rdr.GetOrdinal("email"));
                var dbHash = rdr.GetString(rdr.GetOrdinal("password_hash"));
                var role = rdr.GetString(rdr.GetOrdinal("role"));

                // Try SHA256 of password first, else compare raw (for placeholder hashes like 'hash_admin')
                var sha = ComputeSha256Hash(password);
                if (sha == dbHash || password == dbHash)
                {
                    CurrentUser.Instance.UserId = userId;
                    CurrentUser.Instance.Fullname = fullname;
                    CurrentUser.Instance.Email = dbEmail;
                    CurrentUser.Instance.Role = role;

#if DEBUG
                    MessageBox.Show($"Đăng nhập thành công: {fullname}\nRole='{role}'", "DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif

                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                MessageBox.Show("Email hoặc mật khẩu không chính xác");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
