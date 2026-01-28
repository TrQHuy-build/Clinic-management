using System;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Forms
{
    public partial class frmMigration : Form
    {
        private Label lblStatus;
        private Label lblInfo;
        private Button btnMigratePasswords;
        private Button btnClose;

        public frmMigration()
        {
            InitializeComponent();
        }

        private void frmMigration_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "⚠️ Chú ý: Chỉ chạy migration này một lần duy nhất!\n\nNhấn nút bên dưới để mã hóa tất cả mật khẩu.";
        }

        private void btnMigratePasswords_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn chắc chắn muốn mã hóa tất cả mật khẩu trong database?\n\nHành động này không thể hoàn tác!",
                "Xác nhận Migration",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            btnMigratePasswords.Enabled = false;
            lblStatus.Text = "Đang xử lý...";
            this.Refresh();

            try
            {
                MigrationHelper.MigratePasswordsToHash();
                lblStatus.Text = "✅ Migration thành công!";
                MessageBox.Show("Migration thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Lỗi: " + ex.Message;
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnMigratePasswords.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.btnMigratePasswords = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnMigratePasswords
            // 
            this.btnMigratePasswords.Location = new System.Drawing.Point(150, 150);
            this.btnMigratePasswords.Name = "btnMigratePasswords";
            this.btnMigratePasswords.Size = new System.Drawing.Size(200, 40);
            this.btnMigratePasswords.TabIndex = 0;
            this.btnMigratePasswords.Text = "🔐 Mã hóa tất cả mật khẩu";
            this.btnMigratePasswords.UseVisualStyleBackColor = true;
            this.btnMigratePasswords.Click += new System.EventHandler(this.btnMigratePasswords_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(150, 200);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(200, 40);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(50, 260);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 16);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = false;
            this.lblInfo.Location = new System.Drawing.Point(30, 20);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(450, 100);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "";
            // 
            // frmMigration
            // 
            this.ClientSize = new System.Drawing.Size(514, 340);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnMigratePasswords);
            this.Controls.Add(this.btnClose);
            this.Name = "frmMigration";
            this.Text = "Migration - Mã hóa mật khẩu";
            this.Load += new System.EventHandler(this.frmMigration_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}