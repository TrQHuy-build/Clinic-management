namespace DentalClinicManagement.Forms
{
    partial class frmQuenMatKhau
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlEmail = new System.Windows.Forms.Panel();
            this.btnGuiOTP = new System.Windows.Forms.Button();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.pnlOTP = new System.Windows.Forms.Panel();
            this.btnXacNhanOTP = new System.Windows.Forms.Button();
            this.txtOTP = new System.Windows.Forms.TextBox();
            this.lblOTP = new System.Windows.Forms.Label();
            this.pnlPasswordReset = new System.Windows.Forms.Panel();
            this.pnlNewPassword = new System.Windows.Forms.Panel();
            this.btnToggleNewPassword = new System.Windows.Forms.Button();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.pnlConfirmPassword = new System.Windows.Forms.Panel();
            this.btnToggleConfirmPassword = new System.Windows.Forms.Button();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlEmail.SuspendLayout();
            this.pnlOTP.SuspendLayout();
            this.pnlPasswordReset.SuspendLayout();
            this.pnlNewPassword.SuspendLayout();
            this.pnlConfirmPassword.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();

            // pnlMain
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlMain.Controls.Add(this.pnlButtons);
            this.pnlMain.Controls.Add(this.lblStatus);
            this.pnlMain.Controls.Add(this.pnlPasswordReset);
            this.pnlMain.Controls.Add(this.pnlOTP);
            this.pnlMain.Controls.Add(this.pnlEmail);
            this.pnlMain.Controls.Add(this.pnlHeader);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(30);
            this.pnlMain.Size = new System.Drawing.Size(500, 600);
            this.pnlMain.TabIndex = 0;

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(0, 0, 0);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 70;
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quên Mật Khẩu?";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // pnlEmail
            this.pnlEmail.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlEmail.Controls.Add(this.btnGuiOTP);
            this.pnlEmail.Controls.Add(this.txtEmail);
            this.pnlEmail.Controls.Add(this.lblEmail);
            this.pnlEmail.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEmail.Height = 120;
            this.pnlEmail.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.pnlEmail.Name = "pnlEmail";
            this.pnlEmail.TabIndex = 1;

            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblEmail.ForeColor = System.Drawing.Color.Black;
            this.lblEmail.Location = new System.Drawing.Point(0, 0);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(50, 25);
            this.lblEmail.TabIndex = 0;
            this.lblEmail.Text = "Email";

            // txtEmail
            this.txtEmail.BackColor = System.Drawing.Color.White;
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtEmail.ForeColor = System.Drawing.Color.Black;
            this.txtEmail.Location = new System.Drawing.Point(0, 30);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(350, 35);
            this.txtEmail.TabIndex = 1;

            // btnGuiOTP
            this.btnGuiOTP.BackColor = System.Drawing.Color.Black;
            this.btnGuiOTP.ForeColor = System.Drawing.Color.White;
            this.btnGuiOTP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGuiOTP.Location = new System.Drawing.Point(360, 30);
            this.btnGuiOTP.Name = "btnGuiOTP";
            this.btnGuiOTP.Size = new System.Drawing.Size(120, 35);
            this.btnGuiOTP.TabIndex = 2;
            this.btnGuiOTP.Text = "Gửi OTP";
            this.btnGuiOTP.UseVisualStyleBackColor = false;
            this.btnGuiOTP.Click += new System.EventHandler(this.btnGuiOTP_Click);

            // pnlOTP
            this.pnlOTP.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlOTP.Controls.Add(this.btnXacNhanOTP);
            this.pnlOTP.Controls.Add(this.txtOTP);
            this.pnlOTP.Controls.Add(this.lblOTP);
            this.pnlOTP.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOTP.Height = 120;
            this.pnlOTP.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.pnlOTP.Name = "pnlOTP";
            this.pnlOTP.TabIndex = 2;

            // lblOTP
            this.lblOTP.AutoSize = true;
            this.lblOTP.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.lblOTP.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblOTP.ForeColor = System.Drawing.Color.Black;
            this.lblOTP.Location = new System.Drawing.Point(0, 0);
            this.lblOTP.Name = "lblOTP";
            this.lblOTP.Size = new System.Drawing.Size(70, 25);
            this.lblOTP.TabIndex = 0;
            this.lblOTP.Text = "Mã OTP";

            // txtOTP
            this.txtOTP.BackColor = System.Drawing.Color.White;
            this.txtOTP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOTP.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtOTP.ForeColor = System.Drawing.Color.Black;
            this.txtOTP.Location = new System.Drawing.Point(0, 30);
            this.txtOTP.Name = "txtOTP";
            this.txtOTP.Size = new System.Drawing.Size(350, 35);
            this.txtOTP.TabIndex = 1;
            this.txtOTP.Enabled = false;

            // btnXacNhanOTP
            this.btnXacNhanOTP.BackColor = System.Drawing.Color.Black;
            this.btnXacNhanOTP.ForeColor = System.Drawing.Color.White;
            this.btnXacNhanOTP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXacNhanOTP.Location = new System.Drawing.Point(360, 30);
            this.btnXacNhanOTP.Name = "btnXacNhanOTP";
            this.btnXacNhanOTP.Size = new System.Drawing.Size(120, 35);
            this.btnXacNhanOTP.TabIndex = 2;
            this.btnXacNhanOTP.Text = "Xác nhận";
            this.btnXacNhanOTP.Enabled = false;
            this.btnXacNhanOTP.UseVisualStyleBackColor = false;
            this.btnXacNhanOTP.Click += new System.EventHandler(this.btnXacNhanOTP_Click);

            // pnlPasswordReset
            this.pnlPasswordReset.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlPasswordReset.Controls.Add(this.pnlConfirmPassword);
            this.pnlPasswordReset.Controls.Add(this.pnlNewPassword);
            this.pnlPasswordReset.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPasswordReset.Height = 250;
            this.pnlPasswordReset.Visible = false;
            this.pnlPasswordReset.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.pnlPasswordReset.Name = "pnlPasswordReset";
            this.pnlPasswordReset.TabIndex = 3;

            // pnlNewPassword
            this.pnlNewPassword.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlNewPassword.Controls.Add(this.btnToggleNewPassword);
            this.pnlNewPassword.Controls.Add(this.txtNewPassword);
            this.pnlNewPassword.Controls.Add(this.lblNewPassword);
            this.pnlNewPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNewPassword.Height = 100;
            this.pnlNewPassword.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.pnlNewPassword.Name = "pnlNewPassword";
            this.pnlNewPassword.TabIndex = 0;

            // lblNewPassword
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblNewPassword.ForeColor = System.Drawing.Color.Black;
            this.lblNewPassword.Location = new System.Drawing.Point(0, 0);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(100, 25);
            this.lblNewPassword.TabIndex = 0;
            this.lblNewPassword.Text = "Mật khẩu mới";

            // txtNewPassword
            this.txtNewPassword.BackColor = System.Drawing.Color.White;
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtNewPassword.ForeColor = System.Drawing.Color.Black;
            this.txtNewPassword.Location = new System.Drawing.Point(0, 30);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(410, 35);
            this.txtNewPassword.TabIndex = 1;
            this.txtNewPassword.UseSystemPasswordChar = true;

            // btnToggleNewPassword
            this.btnToggleNewPassword.BackColor = System.Drawing.Color.White;
            //this.btnToggleNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnToggleNewPassword.Location = new System.Drawing.Point(410, 30);
            this.btnToggleNewPassword.Name = "btnToggleNewPassword";
            this.btnToggleNewPassword.Size = new System.Drawing.Size(35, 35);
            this.btnToggleNewPassword.TabIndex = 2;
            this.btnToggleNewPassword.Text = "🙈";
            this.btnToggleNewPassword.UseVisualStyleBackColor = false;
            this.btnToggleNewPassword.Click += new System.EventHandler(this.btnToggleNewPassword_Click);

            // pnlConfirmPassword
            this.pnlConfirmPassword.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlConfirmPassword.Controls.Add(this.btnToggleConfirmPassword);
            this.pnlConfirmPassword.Controls.Add(this.txtConfirmPassword);
            this.pnlConfirmPassword.Controls.Add(this.lblConfirmPassword);
            this.pnlConfirmPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlConfirmPassword.Height = 100;
            this.pnlConfirmPassword.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.pnlConfirmPassword.Name = "pnlConfirmPassword";
            this.pnlConfirmPassword.TabIndex = 1;

            // lblConfirmPassword
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.Black;
            this.lblConfirmPassword.Location = new System.Drawing.Point(0, 0);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(150, 25);
            this.lblConfirmPassword.TabIndex = 0;
            this.lblConfirmPassword.Text = "Xác nhận mật khẩu";

            // txtConfirmPassword
            this.txtConfirmPassword.BackColor = System.Drawing.Color.White;
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtConfirmPassword.ForeColor = System.Drawing.Color.Black;
            this.txtConfirmPassword.Location = new System.Drawing.Point(0, 30);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(410, 35);
            this.txtConfirmPassword.TabIndex = 1;
            this.txtConfirmPassword.UseSystemPasswordChar = true;

            // btnToggleConfirmPassword
            this.btnToggleConfirmPassword.BackColor = System.Drawing.Color.White;
            //this.btnToggleConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btnToggleConfirmPassword.Location = new System.Drawing.Point(410, 30);
            this.btnToggleConfirmPassword.Name = "btnToggleConfirmPassword";
            this.btnToggleConfirmPassword.Size = new System.Drawing.Size(35, 35);
            this.btnToggleConfirmPassword.TabIndex = 2;
            this.btnToggleConfirmPassword.Text = "🙈";
            this.btnToggleConfirmPassword.UseVisualStyleBackColor = false;
            this.btnToggleConfirmPassword.Click += new System.EventHandler(this.btnToggleConfirmPassword_Click);

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblStatus.ForeColor = System.Drawing.Color.Black;
            this.lblStatus.Location = new System.Drawing.Point(30, 450);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 25);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "";

            // pnlButtons
            this.pnlButtons.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnResetPassword);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Height = 60;
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.TabIndex = 4;

            // btnResetPassword
            this.btnResetPassword.BackColor = System.Drawing.Color.Black;
            this.btnResetPassword.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnResetPassword.Location = new System.Drawing.Point(20, 10);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(200, 40);
            this.btnResetPassword.TabIndex = 0;
            this.btnResetPassword.Text = "ĐẶT LẠI MẬT KHẨU";
            this.btnResetPassword.Visible = false;
            this.btnResetPassword.UseVisualStyleBackColor = false;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);

            // btnCancel
            this.btnCancel.BackColor = System.Drawing.Color.LightGray;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(230, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(200, 40);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "HUỶ";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // frmQuenMatKhau
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(255, 241, 118);
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQuenMatKhau";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quên Mật Khẩu";
            this.Load += new System.EventHandler(this.frmQuenMatKhau_Load);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlEmail.ResumeLayout(false);
            this.pnlEmail.PerformLayout();
            this.pnlOTP.ResumeLayout(false);
            this.pnlOTP.PerformLayout();
            this.pnlPasswordReset.ResumeLayout(false);
            this.pnlNewPassword.ResumeLayout(false);
            this.pnlNewPassword.PerformLayout();
            this.pnlConfirmPassword.ResumeLayout(false);
            this.pnlConfirmPassword.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnGuiOTP;
        private System.Windows.Forms.Panel pnlOTP;
        private System.Windows.Forms.Label lblOTP;
        private System.Windows.Forms.TextBox txtOTP;
        private System.Windows.Forms.Button btnXacNhanOTP;
        private System.Windows.Forms.Panel pnlPasswordReset;
        private System.Windows.Forms.Panel pnlNewPassword;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Button btnToggleNewPassword;
        private System.Windows.Forms.Panel pnlConfirmPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnToggleConfirmPassword;
        private System.Windows.Forms.Button btnResetPassword;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlButtons;
    }
}