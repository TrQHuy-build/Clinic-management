using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Forms
{
    public partial class frmQuenMatKhau : Form
    {
        private string resetToken = "";
        private string resetEmail = "";
        private DateTime tokenExpiry = DateTime.MinValue;

        public frmQuenMatKhau()
        {
            InitializeComponent();
        }

        private void frmQuenMatKhau_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "🔐 Quên Mật Khẩu";
            lblStep.Text = "Bước 1: Xác minh Email";
            
            // ✅ FIX: Đặt visibility cho tất cả panels
            panelStep1.Visible = true;
            panelStep2.Visible = false;
            panelStep3.Visible = false;
            panelStep1.BringToFront();
            
            btnSendOTP.Focus();
        }

        private void btnSendOTP_Click(object sender, EventArgs e)
        {
            string email = txtEmailStep1.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmailStep1.Focus();
                return;
            }

            if (!Validator.IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmailStep1.Focus();
                return;
            }

            try
            {
                // Kiểm tra email có tồn tại không
                string query = "SELECT user_id FROM UserAccount WHERE email = @email";
                object userId = DatabaseHelper.ExecuteScalar(query, new SqlParameter[] {
                    new SqlParameter("@email", email)
                });

                if (userId == null)
                {
                    MessageBox.Show("Email này không tồn tại trong hệ thống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo OTP (6 ký tự số ngẫu nhiên)
                Random rnd = new Random();
                resetToken = rnd.Next(100000, 999999).ToString();
                resetEmail = email;
                tokenExpiry = DateTime.Now.AddMinutes(5); // OTP hết hạn sau 5 phút

                // Lưu OTP vào database
                string insertQuery = @"
                    INSERT INTO PasswordReset (email, otp, expires_at, used)
                    VALUES (@email, @otp, @expires, 0)";

                SqlParameter[] parameters = {
                    new SqlParameter("@email", email),
                    new SqlParameter("@otp", resetToken),
                    new SqlParameter("@expires", tokenExpiry)
                };

                DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);

                // Hiển thị OTP (trong demo)
                MessageBox.Show(
                    $"OTP của bạn là: {resetToken}\n\n(Có hiệu lực trong 5 phút)\n\nNhập OTP ở bước tiếp theo.",
                    "OTP được gửi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // ✅ FIX: Hiển thị panelStep2 TRƯỚC khi gọi BringToFront()
                panelStep1.Visible = false;
                panelStep2.Visible = true;
                panelStep3.Visible = false;
                panelStep2.BringToFront();
                
                // Chuyển sang Bước 2
                lblStep.Text = "Bước 2: Xác minh OTP";
                txtOTP.Focus();
                lblTimeRemaining.Text = "Hết hạn trong: 5:00";
                StartOTPTimer();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Timer otpTimer;
        private int secondsRemaining = 300; // 5 phút

        private void StartOTPTimer()
        {
            secondsRemaining = 300;
            otpTimer = new Timer();
            otpTimer.Interval = 1000; // 1 giây
            otpTimer.Tick += (s, e) =>
            {
                secondsRemaining--;
                int minutes = secondsRemaining / 60;
                int seconds = secondsRemaining % 60;
                lblTimeRemaining.Text = $"Hết hạn trong: {minutes}:{seconds:D2}";

                if (secondsRemaining <= 0)
                {
                    otpTimer.Stop();
                    MessageBox.Show("OTP đã hết hạn! Vui lòng yêu cầu OTP mới.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetForm();
                }
            };
            otpTimer.Start();
        }

        private void btnVerifyOTP_Click(object sender, EventArgs e)
        {
            string otp = txtOTP.Text.Trim();

            if (string.IsNullOrEmpty(otp))
            {
                MessageBox.Show("Vui lòng nhập OTP!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOTP.Focus();
                return;
            }

            if (otp != resetToken)
            {
                MessageBox.Show("OTP không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtOTP.Clear();
                txtOTP.Focus();
                return;
            }

            if (DateTime.Now > tokenExpiry)
            {
                MessageBox.Show("OTP đã hết hạn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetForm();
                return;
            }

            // Đánh dấu OTP đã được sử dụng
            try
            {
                string updateQuery = "UPDATE PasswordReset SET used = 1 WHERE email = @email AND otp = @otp";
                DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[] {
                    new SqlParameter("@email", resetEmail),
                    new SqlParameter("@otp", resetToken)
                });
            }
            catch { }

            otpTimer.Stop();

            // ✅ FIX: Hiển thị panelStep3
            panelStep1.Visible = false;
            panelStep2.Visible = false;
            panelStep3.Visible = true;
            panelStep3.BringToFront();
            
            // Chuyển sang Bước 3 - Đặt lại mật khẩu
            lblStep.Text = "Bước 3: Đặt lại Mật khẩu";
            txtNewPassword.Focus();
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPassword.Focus();
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNewPassword.Focus();
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            try
            {
                // Mã hóa mật khẩu mới
                string hashedPassword = PasswordHelper.HashPassword(newPassword);

                // Cập nhật mật khẩu
                string updateQuery = "UPDATE UserAccount SET password_hash = @password WHERE email = @email";
                int result = DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[] {
                    new SqlParameter("@password", hashedPassword),
                    new SqlParameter("@email", resetEmail)
                });

                if (result > 0)
                {
                    MessageBox.Show("Mật khẩu đã được đặt lại thành công! Vui lòng đăng nhập.", 
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Ghi log
                    string logQuery = "INSERT INTO AuditLog (action, description) VALUES (@action, @desc)";
                    DatabaseHelper.ExecuteNonQuery(logQuery, new SqlParameter[] {
                        new SqlParameter("@action", "PASSWORD_RESET"),
                        new SqlParameter("@desc", $"Đặt lại mật khẩu cho email: {resetEmail}")
                    });

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackStep1_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void btnBackStep2_Click(object sender, EventArgs e)
        {
            otpTimer?.Stop();
            
            // ✅ FIX: Hiển thị panelStep1
            panelStep1.Visible = true;
            panelStep2.Visible = false;
            panelStep3.Visible = false;
            panelStep1.BringToFront();
            
            lblStep.Text = "Bước 1: Xác minh Email";
            txtEmailStep1.Focus();
        }

        private void btnBackStep3_Click(object sender, EventArgs e)
        {
            // ✅ FIX: Hiển thị panelStep2
            panelStep1.Visible = false;
            panelStep2.Visible = true;
            panelStep3.Visible = false;
            panelStep2.BringToFront();
            
            lblStep.Text = "Bước 2: Xác minh OTP";
            txtOTP.Focus();
        }

        private void ResetForm()
        {
            otpTimer?.Stop();
            resetToken = "";
            resetEmail = "";
            txtEmailStep1.Clear();
            txtOTP.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
            
            // ✅ FIX: Reset visibility
            panelStep1.Visible = true;
            panelStep2.Visible = false;
            panelStep3.Visible = false;
            panelStep1.BringToFront();
            
            lblStep.Text = "Bước 1: Xác minh Email";
            txtEmailStep1.Focus();
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStep = new System.Windows.Forms.Label();
            this.panelStep1 = new System.Windows.Forms.Panel();
            this.panelStep2 = new System.Windows.Forms.Panel();
            this.panelStep3 = new System.Windows.Forms.Panel();
            this.SuspendLayout();

            // Label Title
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.Size = new System.Drawing.Size(450, 50);
            this.lblTitle.Text = "";
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(78, 205, 196);

            // Label Step
            this.lblStep.AutoSize = false;
            this.lblStep.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            this.lblStep.Location = new System.Drawing.Point(30, 75);
            this.lblStep.Size = new System.Drawing.Size(450, 25);
            this.lblStep.Text = "";
            this.lblStep.ForeColor = System.Drawing.Color.Gray;

            // Panel Step 1
            this.panelStep1 = CreateStep1Panel();

            // Panel Step 2
            this.panelStep2 = CreateStep2Panel();

            // Panel Step 3
            this.panelStep3 = CreateStep3Panel();

            // Form
            this.ClientSize = new System.Drawing.Size(520, 450);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStep);
            this.Controls.Add(this.panelStep1);
            this.Controls.Add(this.panelStep2);
            this.Controls.Add(this.panelStep3);
            this.Name = "frmQuenMatKhau";
            this.Text = "Quên Mật Khẩu - Phòng khám nha khoa";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Load += new System.EventHandler(this.frmQuenMatKhau_Load);
            this.ResumeLayout(false);
        }

        private Panel CreateStep1Panel()
        {
            Panel panel = new Panel();
            panel.Location = new System.Drawing.Point(30, 110);
            panel.Size = new System.Drawing.Size(450, 300);

            Label label = new Label() { Text = "Nhập email đăng ký:", Location = new System.Drawing.Point(0, 0), AutoSize = true };
            label.Font = new System.Drawing.Font("Segoe UI", 10);

            this.txtEmailStep1 = new System.Windows.Forms.TextBox();
            this.txtEmailStep1.Location = new System.Drawing.Point(0, 30);
            this.txtEmailStep1.Size = new System.Drawing.Size(450, 40);
            this.txtEmailStep1.Multiline = true;
            this.txtEmailStep1.Font = new System.Drawing.Font("Segoe UI", 12);

            this.btnSendOTP = new System.Windows.Forms.Button();
            this.btnSendOTP.Location = new System.Drawing.Point(0, 80);
            this.btnSendOTP.Size = new System.Drawing.Size(450, 40);
            this.btnSendOTP.Text = "📧 Gửi OTP";
            this.btnSendOTP.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            this.btnSendOTP.BackColor = System.Drawing.Color.FromArgb(78, 205, 196);
            this.btnSendOTP.ForeColor = System.Drawing.Color.White;
            this.btnSendOTP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendOTP.Click += new System.EventHandler(this.btnSendOTP_Click);

            Button btnClose = new Button() { Text = "Đóng", Location = new System.Drawing.Point(0, 130), Size = new System.Drawing.Size(450, 40) };
            btnClose.Font = new System.Drawing.Font("Segoe UI", 11);
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            //btnClose.Click += new System.EventHandler(this.btnClose_Click);

            panel.Controls.Add(label);
            panel.Controls.Add(this.txtEmailStep1);
            panel.Controls.Add(this.btnSendOTP);
            panel.Controls.Add(btnClose);

            return panel;
        }

        private Panel CreateStep2Panel()
        {
            Panel panel = new Panel();
            panel.Location = new System.Drawing.Point(30, 110);
            panel.Size = new System.Drawing.Size(450, 300);
            panel.Visible = false;

            Label label1 = new Label() { Text = "Nhập OTP (mã 6 chữ số):", Location = new System.Drawing.Point(0, 0), AutoSize = true };
            label1.Font = new System.Drawing.Font("Segoe UI", 10);

            this.txtOTP = new System.Windows.Forms.TextBox();
            this.txtOTP.Location = new System.Drawing.Point(0, 30);
            this.txtOTP.Size = new System.Drawing.Size(450, 40);
            this.txtOTP.Multiline = true;
            this.txtOTP.Font = new System.Drawing.Font("Segoe UI", 12);
            this.txtOTP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.lblTimeRemaining.Location = new System.Drawing.Point(0, 75);
            this.lblTimeRemaining.AutoSize = true;
            this.lblTimeRemaining.Font = new System.Drawing.Font("Segoe UI", 9);
            this.lblTimeRemaining.ForeColor = System.Drawing.Color.Red;

            this.btnVerifyOTP = new System.Windows.Forms.Button();
            this.btnVerifyOTP.Location = new System.Drawing.Point(0, 105);
            this.btnVerifyOTP.Size = new System.Drawing.Size(450, 40);
            this.btnVerifyOTP.Text = "✓ Xác minh OTP";
            this.btnVerifyOTP.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            this.btnVerifyOTP.BackColor = System.Drawing.Color.FromArgb(78, 205, 196);
            this.btnVerifyOTP.ForeColor = System.Drawing.Color.White;
            this.btnVerifyOTP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerifyOTP.Click += new System.EventHandler(this.btnVerifyOTP_Click);

            this.btnBackStep2 = new System.Windows.Forms.Button();
            this.btnBackStep2.Location = new System.Drawing.Point(0, 155);
            this.btnBackStep2.Size = new System.Drawing.Size(450, 40);
            this.btnBackStep2.Text = "← Quay lại";
            this.btnBackStep2.Font = new System.Drawing.Font("Segoe UI", 11);
            this.btnBackStep2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackStep2.Click += new System.EventHandler(this.btnBackStep2_Click);

            panel.Controls.Add(label1);
            panel.Controls.Add(this.txtOTP);
            panel.Controls.Add(this.lblTimeRemaining);
            panel.Controls.Add(this.btnVerifyOTP);
            panel.Controls.Add(this.btnBackStep2);

            return panel;
        }

        private Panel CreateStep3Panel()
        {
            Panel panel = new Panel();
            panel.Location = new System.Drawing.Point(30, 110);
            panel.Size = new System.Drawing.Size(450, 300);
            panel.Visible = false;

            Label label1 = new Label() { Text = "Mật khẩu mới:", Location = new System.Drawing.Point(0, 0), AutoSize = true };
            label1.Font = new System.Drawing.Font("Segoe UI", 10);

            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.txtNewPassword.Location = new System.Drawing.Point(0, 30);
            this.txtNewPassword.Size = new System.Drawing.Size(450, 35);
            this.txtNewPassword.UseSystemPasswordChar = true;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 12);

            Label label2 = new Label() { Text = "Xác nhận mật khẩu:", Location = new System.Drawing.Point(0, 75), AutoSize = true };
            label2.Font = new System.Drawing.Font("Segoe UI", 10);

            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.txtConfirmPassword.Location = new System.Drawing.Point(0, 105);
            this.txtConfirmPassword.Size = new System.Drawing.Size(450, 35);
            this.txtConfirmPassword.UseSystemPasswordChar = true;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 12);

            this.btnResetPassword = new System.Windows.Forms.Button();
            this.btnResetPassword.Location = new System.Drawing.Point(0, 150);
            this.btnResetPassword.Size = new System.Drawing.Size(450, 40);
            this.btnResetPassword.Text = "🔄 Đặt lại Mật khẩu";
            this.btnResetPassword.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            this.btnResetPassword.BackColor = System.Drawing.Color.FromArgb(78, 205, 196);
            this.btnResetPassword.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);

            this.btnBackStep3 = new System.Windows.Forms.Button();
            this.btnBackStep3.Location = new System.Drawing.Point(0, 200);
            this.btnBackStep3.Size = new System.Drawing.Size(450, 40);
            this.btnBackStep3.Text = "← Quay lại";
            this.btnBackStep3.Font = new System.Drawing.Font("Segoe UI", 11);
            this.btnBackStep3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackStep3.Click += new System.EventHandler(this.btnBackStep3_Click);

            panel.Controls.Add(label1);
            panel.Controls.Add(this.txtNewPassword);
            panel.Controls.Add(label2);
            panel.Controls.Add(this.txtConfirmPassword);
            panel.Controls.Add(this.btnResetPassword);
            panel.Controls.Add(this.btnBackStep3);

            return panel;
        }

        private Label lblTitle;
        private Label lblStep;
        private Panel panelStep1;
        private Panel panelStep2;
        private Panel panelStep3;
        private TextBox txtEmailStep1;
        private TextBox txtOTP;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnSendOTP;
        private Button btnVerifyOTP;
        private Button btnResetPassword;
        private Button btnBackStep2;
        private Button btnBackStep3;
        private Label lblTimeRemaining;
    }
}