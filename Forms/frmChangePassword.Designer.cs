namespace DentalClinicManagement.Forms
{
    partial class frmChangePassword
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCurrentPassword;
        private System.Windows.Forms.TextBox txtCurrentPassword;
        private System.Windows.Forms.Button btnToggleCurrent;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Button btnToggleNew;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Button btnToggleConfirm;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPasswordHint;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCurrentPassword = new System.Windows.Forms.Label();
            this.txtCurrentPassword = new System.Windows.Forms.TextBox();
            this.btnToggleCurrent = new System.Windows.Forms.Button();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.btnToggleNew = new System.Windows.Forms.Button();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.btnToggleConfirm = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPasswordHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(198, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔐 Đổi mật khẩu";
            // 
            // lblCurrentPassword
            // 
            this.lblCurrentPassword.AutoSize = true;
            this.lblCurrentPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblCurrentPassword.Location = new System.Drawing.Point(30, 90);
            this.lblCurrentPassword.Name = "lblCurrentPassword";
            this.lblCurrentPassword.Size = new System.Drawing.Size(134, 20);
            this.lblCurrentPassword.TabIndex = 1;
            this.lblCurrentPassword.Text = "Mật khẩu hiện tại:";
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCurrentPassword.Location = new System.Drawing.Point(30, 115);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.Size = new System.Drawing.Size(380, 27);
            this.txtCurrentPassword.TabIndex = 2;
            this.txtCurrentPassword.UseSystemPasswordChar = true;
            // 
            // btnToggleCurrent
            // 
            this.btnToggleCurrent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleCurrent.FlatAppearance.BorderSize = 0;
            this.btnToggleCurrent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleCurrent.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnToggleCurrent.Location = new System.Drawing.Point(415, 115);
            this.btnToggleCurrent.Name = "btnToggleCurrent";
            this.btnToggleCurrent.Size = new System.Drawing.Size(40, 27);
            this.btnToggleCurrent.TabIndex = 3;
            this.btnToggleCurrent.Text = "👁";
            this.btnToggleCurrent.UseVisualStyleBackColor = true;
            this.btnToggleCurrent.Click += new System.EventHandler(this.BtnToggleCurrent_Click);
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNewPassword.Location = new System.Drawing.Point(30, 160);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(114, 20);
            this.lblNewPassword.TabIndex = 4;
            this.lblNewPassword.Text = "Mật khẩu mới:";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNewPassword.Location = new System.Drawing.Point(30, 185);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(380, 27);
            this.txtNewPassword.TabIndex = 5;
            this.txtNewPassword.UseSystemPasswordChar = true;
            // 
            // btnToggleNew
            // 
            this.btnToggleNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleNew.FlatAppearance.BorderSize = 0;
            this.btnToggleNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleNew.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnToggleNew.Location = new System.Drawing.Point(415, 185);
            this.btnToggleNew.Name = "btnToggleNew";
            this.btnToggleNew.Size = new System.Drawing.Size(40, 27);
            this.btnToggleNew.TabIndex = 6;
            this.btnToggleNew.Text = "👁";
            this.btnToggleNew.UseVisualStyleBackColor = true;
            this.btnToggleNew.Click += new System.EventHandler(this.BtnToggleNew_Click);
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblConfirmPassword.Location = new System.Drawing.Point(30, 230);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(158, 20);
            this.lblConfirmPassword.TabIndex = 7;
            this.lblConfirmPassword.Text = "Xác nhận mật khẩu:";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(30, 255);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(380, 27);
            this.txtConfirmPassword.TabIndex = 8;
            this.txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // btnToggleConfirm
            // 
            this.btnToggleConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleConfirm.FlatAppearance.BorderSize = 0;
            this.btnToggleConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleConfirm.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnToggleConfirm.Location = new System.Drawing.Point(415, 255);
            this.btnToggleConfirm.Name = "btnToggleConfirm";
            this.btnToggleConfirm.Size = new System.Drawing.Size(40, 27);
            this.btnToggleConfirm.TabIndex = 9;
            this.btnToggleConfirm.Text = "👁";
            this.btnToggleConfirm.UseVisualStyleBackColor = true;
            this.btnToggleConfirm.Click += new System.EventHandler(this.BtnToggleConfirm_Click);
            // 
            // lblPasswordHint
            // 
            this.lblPasswordHint.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPasswordHint.ForeColor = System.Drawing.Color.Gray;
            this.lblPasswordHint.Location = new System.Drawing.Point(30, 295);
            this.lblPasswordHint.Name = "lblPasswordHint";
            this.lblPasswordHint.Size = new System.Drawing.Size(425, 60);
            this.lblPasswordHint.TabIndex = 10;
            this.lblPasswordHint.Text = "💡 Yêu cầu mật khẩu:\r\n• Tối thiểu 8 ký tự\r\n• Chứa ít nhất 3 trong 4: chữ hoa, chữ thường, số, ký tự đặc biệt";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(30, 370);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 45);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "💾 Lưu thay đổi";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Gray;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(255, 370);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(200, 45);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "❌ Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // frmChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 441);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblPasswordHint);
            this.Controls.Add(this.btnToggleConfirm);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.lblConfirmPassword);
            this.Controls.Add(this.btnToggleNew);
            this.Controls.Add(this.txtNewPassword);
            this.Controls.Add(this.lblNewPassword);
            this.Controls.Add(this.btnToggleCurrent);
            this.Controls.Add(this.txtCurrentPassword);
            this.Controls.Add(this.lblCurrentPassword);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChangePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đổi mật khẩu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
