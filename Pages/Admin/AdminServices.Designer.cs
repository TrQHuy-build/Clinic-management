using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DentalClinicManagement.Pages.Admin
{
    partial class AdminServices
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.SuspendLayout();

            // Title
            System.Windows.Forms.Label lblTitle = new System.Windows.Forms.Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            lblTitle.Location = new System.Drawing.Point(20, 20);
            lblTitle.Text = "Quản lý Dịch vụ";

            // Search Panel
            System.Windows.Forms.Panel searchPanel = new System.Windows.Forms.Panel();
            searchPanel.BackColor = System.Drawing.Color.White;
            searchPanel.Location = new System.Drawing.Point(20, 80);
            searchPanel.Size = new System.Drawing.Size(1160, 60);

            // txtSearch
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSearch.Location = new System.Drawing.Point(20, 17);
            this.txtSearch.Size = new System.Drawing.Size(300, 27);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // btnAdd
            System.Windows.Forms.Button btnAdd = new System.Windows.Forms.Button();
            btnAdd.BackColor = System.Drawing.ColorTranslator.FromHtml("#28A745");
            btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnAdd.ForeColor = System.Drawing.Color.White;
            btnAdd.Location = new System.Drawing.Point(980, 15);
            btnAdd.Size = new System.Drawing.Size(160, 35);
            btnAdd.Text = "+ Thêm dịch vụ mới";
            btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);

            searchPanel.Controls.Add(this.txtSearch);
            searchPanel.Controls.Add(btnAdd);

            // dgvServices
            this.dgvServices.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvServices.BackgroundColor = System.Drawing.Color.White;
            this.dgvServices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvServices.ColumnHeadersHeight = 40;
            this.dgvServices.EnableHeadersVisualStyles = false;
            this.dgvServices.Location = new System.Drawing.Point(20, 160);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.ReadOnly = true;
            this.dgvServices.RowTemplate.Height = 35;
            this.dgvServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServices.Size = new System.Drawing.Size(1160, 520);
            this.dgvServices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvServices_CellClick);

            this.dgvServices.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.dgvServices.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvServices.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            // UserControl
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F2F5");
            this.Controls.Add(lblTitle);
            this.Controls.Add(searchPanel);
            this.Controls.Add(this.dgvServices);
            this.Name = "AdminServices";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1200, 700);
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.TextBox txtSearch;

        // CUEBANNER: Gọi sau khi Handle được tạo
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!this.DesignMode)
            {
                SetCueBanner(txtSearch, "Tìm theo tên hoặc mô tả dịch vụ...");
            }
        }

        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        private void SetCueBanner(TextBox textBox, string text)
        {
            if (textBox.IsHandleCreated)
            {
                SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, text);
            }
        }
    }
}