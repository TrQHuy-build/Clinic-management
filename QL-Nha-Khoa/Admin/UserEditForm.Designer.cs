namespace QL_Nha_Khoa
{
    partial class UserEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtFullname;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtFullname = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFullname
            // 
            this.txtFullname.Location = new System.Drawing.Point(24, 24);
            this.txtFullname.Name = "txtFullname";
            this.txtFullname.PlaceholderText = "Fullname";
            this.txtFullname.Size = new System.Drawing.Size(300, 27);
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(24, 64);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.PlaceholderText = "Phone";
            this.txtPhone.Size = new System.Drawing.Size(300, 27);
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(24, 104);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.PlaceholderText = "Email";
            this.txtEmail.Size = new System.Drawing.Size(300, 27);
            // 
            // cmbRole
            // 
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.Items.AddRange(new object[] { "admin", "doctor", "staff", "patient" });
            this.cmbRole.Location = new System.Drawing.Point(24, 144);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(300, 28);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(24, 188);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 29);
            this.btnSave.Text = "Save";
            this.btnSave.Click += btnSave_Click;
            // 
            // UserEditForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 240);
            this.Controls.Add(this.txtFullname);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.cmbRole);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
