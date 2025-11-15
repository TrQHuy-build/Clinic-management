using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Forms
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Declare controls
        private Panel panelSidebar;
        private Panel panelHeader;
        private Panel panelContent;
        private Panel panelLogo;
        private Label lblLogo;
        private Label lblUserInfo;
        private Button btnLogout;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelSidebar = new Panel();
            this.panelHeader = new Panel();
            this.panelContent = new Panel();
            this.panelLogo = new Panel();
            this.lblLogo = new Label();
            this.lblUserInfo = new Label();
            this.btnLogout = new Button();

            this.SuspendLayout();

            // 
            // Form Settings
            // 
            this.Text = "Dental Clinic Management System";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = ColorTranslator.FromHtml("#F0F2F5");
            this.Name = "frmMain";

            // 
            // panelSidebar
            // 
            this.panelSidebar.Dock = DockStyle.Left;
            this.panelSidebar.Width = 250;
            this.panelSidebar.BackColor = ColorTranslator.FromHtml("#007ACC");
            this.panelSidebar.Name = "panelSidebar";

            // 
            // panelLogo
            // 
            this.panelLogo.Dock = DockStyle.Top;
            this.panelLogo.Height = 100;
            this.panelLogo.BackColor = ColorTranslator.FromHtml("#005A9E");
            this.panelLogo.Name = "panelLogo";

            // 
            // lblLogo
            // 
            this.lblLogo.Text = "🦷 DENTAL\n   CLINIC";
            this.lblLogo.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            this.lblLogo.ForeColor = Color.White;
            this.lblLogo.AutoSize = false;
            this.lblLogo.Size = new Size(250, 100);
            this.lblLogo.Dock = DockStyle.Fill;
            this.lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            this.lblLogo.Name = "lblLogo";

            // 
            // panelHeader
            // 
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 70;
            this.panelHeader.BackColor = Color.White;
            this.panelHeader.Name = "panelHeader";

            // 
            // lblUserInfo
            // 
            this.lblUserInfo.Text = "";  // Sẽ set trong constructor
            this.lblUserInfo.Font = new Font("Segoe UI", 11);
            this.lblUserInfo.ForeColor = ColorTranslator.FromHtml("#007ACC");
            this.lblUserInfo.AutoSize = false;
            this.lblUserInfo.Size = new Size(400, 70);
            this.lblUserInfo.Location = new Point(20, 0);
            this.lblUserInfo.TextAlign = ContentAlignment.MiddleLeft;
            this.lblUserInfo.Name = "lblUserInfo";

            // 
            // btnLogout
            // 
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnLogout.Size = new Size(120, 40);
            this.btnLogout.Location = new Point(1240, 15);  // Default position, sẽ adjust trong Load
            this.btnLogout.BackColor = ColorTranslator.FromHtml("#DC3545");
            this.btnLogout.ForeColor = Color.White;
            this.btnLogout.FlatStyle = FlatStyle.Flat;
            this.btnLogout.Cursor = Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);

            // 
            // panelContent
            // 
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.BackColor = ColorTranslator.FromHtml("#F0F2F5");
            this.panelContent.Padding = new Padding(20);
            this.panelContent.Name = "panelContent";

            // 
            // Add controls to panels
            // 
            this.panelLogo.Controls.Add(this.lblLogo);
            this.panelSidebar.Controls.Add(this.panelLogo);

            this.panelHeader.Controls.Add(this.lblUserInfo);
            this.panelHeader.Controls.Add(this.btnLogout);

            // 
            // Add panels to form
            // 
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelSidebar);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}