using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    partial class AdminLogs
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
            this.dgvLogs = new System.Windows.Forms.DataGridView();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.txtPageNumber = new System.Windows.Forms.TextBox();
            this.btnGoToPage = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).BeginInit();
            this.SuspendLayout();

            // ========================================
            // Title Label
            // ========================================
            System.Windows.Forms.Label lblTitle = new System.Windows.Forms.Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            lblTitle.Location = new System.Drawing.Point(20, 20);
            lblTitle.Text = "Nhật ký hệ thống";

            // ========================================
            // Page Size Label & ComboBox
            // ========================================
            System.Windows.Forms.Label lblPageSize = new System.Windows.Forms.Label();
            lblPageSize.AutoSize = true;
            lblPageSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblPageSize.Location = new System.Drawing.Point(20, 75);
            lblPageSize.Text = "Hiển thị:";

            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboPageSize.Location = new System.Drawing.Point(85, 72);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(80, 25);
            this.cboPageSize.Items.AddRange(new object[] { 25, 50, 100, 200 });
            this.cboPageSize.SelectedIndex = 1; // Mặc định 50
            this.cboPageSize.SelectedIndexChanged += new System.EventHandler(this.CboPageSize_SelectedIndexChanged);

            System.Windows.Forms.Label lblRecords = new System.Windows.Forms.Label();
            lblRecords.AutoSize = true;
            lblRecords.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblRecords.Location = new System.Drawing.Point(170, 75);
            lblRecords.Text = "bản ghi/trang";

            // ========================================
            // Refresh Button
            // ========================================
            this.btnRefresh.BackColor = System.Drawing.ColorTranslator.FromHtml("#28A745");
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(1070, 68);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 32);
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);

            // ========================================
            // DataGridView
            // ========================================
            this.dgvLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvLogs.BackgroundColor = System.Drawing.Color.White;
            this.dgvLogs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvLogs.ColumnHeadersHeight = 40;
            this.dgvLogs.EnableHeadersVisualStyles = false;
            this.dgvLogs.Location = new System.Drawing.Point(20, 110);
            this.dgvLogs.Name = "dgvLogs";
            this.dgvLogs.ReadOnly = true;
            this.dgvLogs.RowTemplate.Height = 35;
            this.dgvLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLogs.Size = new System.Drawing.Size(1160, 520);
            this.dgvLogs.AllowUserToAddRows = false;
            this.dgvLogs.AllowUserToDeleteRows = false;
            this.dgvLogs.RowHeadersVisible = false;
            this.dgvLogs.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.dgvLogs.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvLogs.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dgvLogs.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvLogs.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvLogs.DefaultCellStyle.SelectionBackColor = System.Drawing.ColorTranslator.FromHtml("#D1E7F5");
            this.dgvLogs.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            // ========================================
            // Pagination Panel
            // ========================================
            System.Windows.Forms.Panel pnlPagination = new System.Windows.Forms.Panel();
            pnlPagination.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlPagination.BackColor = System.Drawing.Color.White;
            pnlPagination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pnlPagination.Location = new System.Drawing.Point(20, 640);
            pnlPagination.Name = "pnlPagination";
            pnlPagination.Size = new System.Drawing.Size(1160, 50);

            // ========================================
            // Page Info Label
            // ========================================
            this.lblPageInfo.AutoSize = true;
            this.lblPageInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPageInfo.Location = new System.Drawing.Point(15, 15);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Text = "Trang 1/1 (Hiển thị 1-50 / 100 bản ghi)";

            // ========================================
            // Navigation Buttons (Center)
            // ========================================
            int centerX = 450;
            int buttonY = 10;
            int buttonWidth = 80;
            int buttonHeight = 32;
            int spacing = 5;

            this.btnFirst.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFirst.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFirst.ForeColor = System.Drawing.Color.White;
            this.btnFirst.Location = new System.Drawing.Point(centerX, buttonY);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnFirst.Text = "⏮ Đầu";
            this.btnFirst.UseVisualStyleBackColor = false;
            this.btnFirst.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFirst.Click += new System.EventHandler(this.BtnFirst_Click);

            this.btnPrevious.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.btnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevious.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnPrevious.ForeColor = System.Drawing.Color.White;
            this.btnPrevious.Location = new System.Drawing.Point(centerX + buttonWidth + spacing, buttonY);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnPrevious.Text = "◀ Trước";
            this.btnPrevious.UseVisualStyleBackColor = false;
            this.btnPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevious.Click += new System.EventHandler(this.BtnPrevious_Click);

            this.btnNext.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.Location = new System.Drawing.Point(centerX + (buttonWidth + spacing) * 2, buttonY);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnNext.Text = "Sau ▶";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);

            this.btnLast.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLast.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnLast.ForeColor = System.Drawing.Color.White;
            this.btnLast.Location = new System.Drawing.Point(centerX + (buttonWidth + spacing) * 3, buttonY);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnLast.Text = "Cuối ⏭";
            this.btnLast.UseVisualStyleBackColor = false;
            this.btnLast.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLast.Click += new System.EventHandler(this.BtnLast_Click);

            // ========================================
            // Go To Page (Right side)
            // ========================================
            System.Windows.Forms.Label lblGoTo = new System.Windows.Forms.Label();
            lblGoTo.AutoSize = true;
            lblGoTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblGoTo.Location = new System.Drawing.Point(900, 17);
            lblGoTo.Text = "Đến trang:";

            this.txtPageNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPageNumber.Location = new System.Drawing.Point(970, 13);
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.Size = new System.Drawing.Size(60, 25);
            this.txtPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPageNumber.KeyPress += (s, e) =>
            {
                // Chỉ cho phép nhập số và phím điều khiển
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                // Enter để go to page
                if (e.KeyChar == (char)Keys.Enter)
                {
                    BtnGoToPage_Click(this.btnGoToPage, EventArgs.Empty);
                    e.Handled = true;
                }
            };

            this.btnGoToPage.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.btnGoToPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGoToPage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGoToPage.ForeColor = System.Drawing.Color.White;
            this.btnGoToPage.Location = new System.Drawing.Point(1035, 10);
            this.btnGoToPage.Name = "btnGoToPage";
            this.btnGoToPage.Size = new System.Drawing.Size(50, 32);
            this.btnGoToPage.Text = "→";
            this.btnGoToPage.UseVisualStyleBackColor = false;
            this.btnGoToPage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoToPage.Click += new System.EventHandler(this.BtnGoToPage_Click);

            // ========================================
            // Add controls to Pagination Panel
            // ========================================
            pnlPagination.Controls.Add(this.lblPageInfo);
            pnlPagination.Controls.Add(this.btnFirst);
            pnlPagination.Controls.Add(this.btnPrevious);
            pnlPagination.Controls.Add(this.btnNext);
            pnlPagination.Controls.Add(this.btnLast);
            pnlPagination.Controls.Add(lblGoTo);
            pnlPagination.Controls.Add(this.txtPageNumber);
            pnlPagination.Controls.Add(this.btnGoToPage);

            // ========================================
            // UserControl
            // ========================================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F2F5");
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPageSize);
            this.Controls.Add(this.cboPageSize);
            this.Controls.Add(lblRecords);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.dgvLogs);
            this.Controls.Add(pnlPagination);
            this.Name = "AdminLogs";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1200, 710);

            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLogs;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.ComboBox cboPageSize;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TextBox txtPageNumber;
        private System.Windows.Forms.Button btnGoToPage;
    }
}