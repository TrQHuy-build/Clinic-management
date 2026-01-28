// AdminInvoices.Designer.cs
using System;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    partial class AdminInvoices
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            // ✅ PHÂN TRANG
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.txtPageNumber = new System.Windows.Forms.TextBox();
            this.btnGoToPage = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right));
            this.dgvInvoices.BackgroundColor = System.Drawing.Color.White;
            this.dgvInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInvoices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvInvoices.ColumnHeadersHeight = 40;
            this.dgvInvoices.EnableHeadersVisualStyles = false;
            this.dgvInvoices.Location = new System.Drawing.Point(20, 145);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.ReadOnly = true;
            this.dgvInvoices.RowHeadersWidth = 51;
            this.dgvInvoices.RowTemplate.Height = 35;
            this.dgvInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoices.Size = new System.Drawing.Size(1160, 495);
            this.dgvInvoices.TabIndex = 0;
            this.dgvInvoices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvInvoices_CellClick);
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(109, 18);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(239, 22);
            this.dtpFrom.TabIndex = 1;
            this.dtpFrom.Value = new System.DateTime(2025, 10, 15, 8, 51, 21, 98);
            this.dtpFrom.ValueChanged += new System.EventHandler(this.dtpFrom_ValueChanged);
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(514, 18);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(236, 22);
            this.dtpTo.TabIndex = 3;
            this.dtpTo.Value = new System.DateTime(2025, 11, 15, 8, 51, 21, 106);
            this.dtpTo.ValueChanged += new System.EventHandler(this.dtpTo_ValueChanged);
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] {
            "Tất cả",
            "Đã thanh toán",
            "Chưa thanh toán",
            "Đã hủy"});
            this.cboStatus.Location = new System.Drawing.Point(937, 18);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(137, 24);
            this.cboStatus.TabIndex = 5;
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.cboStatus_SelectedIndexChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(314, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý Hóa đơn";
            // 
            // filterPanel
            // 
            this.filterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterPanel.BackColor = System.Drawing.Color.White;
            this.filterPanel.Controls.Add(this.lblFrom);
            this.filterPanel.Controls.Add(this.dtpFrom);
            this.filterPanel.Controls.Add(this.lblTo);
            this.filterPanel.Controls.Add(this.dtpTo);
            this.filterPanel.Controls.Add(this.lblStatus);
            this.filterPanel.Controls.Add(this.cboStatus);
            this.filterPanel.Controls.Add(this.btnRefresh);
            this.filterPanel.Location = new System.Drawing.Point(20, 75);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Size = new System.Drawing.Size(1160, 60);
            this.filterPanel.TabIndex = 1;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(26, 21);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(59, 16);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Từ ngày:";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(459, 21);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(34, 16);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "Đến:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(848, 21);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(70, 16);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(1117, 18);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(191, 35);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            
            // ========================================
            // Pagination Panel
            // ========================================
            this.pnlPagination = new System.Windows.Forms.Panel();
            this.pnlPagination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right));
            this.pnlPagination.BackColor = System.Drawing.Color.White;
            this.pnlPagination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPagination.Location = new System.Drawing.Point(20, 640);
            this.pnlPagination.Name = "pnlPagination";
            this.pnlPagination.Size = new System.Drawing.Size(1160, 50);

            // ========================================
            // Page Info Label
            // ========================================
            this.lblPageInfo.AutoSize = true;
            this.lblPageInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPageInfo.Location = new System.Drawing.Point(15, 15);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Text = "Trang 1/1 (Hiển thị 1-50 / 100 bản ghi)";

            // ========================================
            // Page Size Label & ComboBox
            // ========================================
            System.Windows.Forms.Label lblPageSize = new System.Windows.Forms.Label();
            lblPageSize.AutoSize = true;
            lblPageSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblPageSize.Location = new System.Drawing.Point(350, 17);
            lblPageSize.Name = "lblPageSize";
            lblPageSize.Text = "Hiển thị:";

            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboPageSize.Location = new System.Drawing.Point(415, 12);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(80, 25);
            this.cboPageSize.Items.AddRange(new object[] { 25, 50, 100, 200 });
            this.cboPageSize.SelectedIndex = 1; // Default 50
            this.cboPageSize.SelectedIndexChanged += new System.EventHandler(this.CboPageSize_SelectedIndexChanged);

            System.Windows.Forms.Label lblRecords = new System.Windows.Forms.Label();
            lblRecords.AutoSize = true;
            lblRecords.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblRecords.Location = new System.Drawing.Point(500, 17);
            lblRecords.Name = "lblRecords";
            lblRecords.Text = "bản ghi/trang";

            // ========================================
            // Navigation Buttons (Center)
            // ========================================
            int centerX = 750;
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
            lblGoTo.Name = "lblGoTo";
            lblGoTo.Text = "Đến trang:";

            this.txtPageNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPageNumber.Location = new System.Drawing.Point(970, 13);
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.Size = new System.Drawing.Size(60, 25);
            this.txtPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPageNumber.KeyPress += (s, e) =>
            {
                // Only allow numbers and control keys
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                // Press Enter to go to page
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
            this.pnlPagination.Controls.Add(this.lblPageInfo);
            this.pnlPagination.Controls.Add(lblPageSize);
            this.pnlPagination.Controls.Add(this.cboPageSize);
            this.pnlPagination.Controls.Add(lblRecords);
            this.pnlPagination.Controls.Add(this.btnFirst);
            this.pnlPagination.Controls.Add(this.btnPrevious);
            this.pnlPagination.Controls.Add(this.btnNext);
            this.pnlPagination.Controls.Add(this.btnLast);
            this.pnlPagination.Controls.Add(lblGoTo);
            this.pnlPagination.Controls.Add(this.txtPageNumber);
            this.pnlPagination.Controls.Add(this.btnGoToPage);

            // ========================================
            // UserControl
            // ========================================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F2F5");
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.filterPanel);
            this.Controls.Add(this.dgvInvoices);
            this.Controls.Add(this.pnlPagination);
            this.Name = "AdminInvoices";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1200, 720);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.pnlPagination.ResumeLayout(false);
            this.pnlPagination.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvInvoices;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.ComboBox cboStatus;
        private Label lblTitle;
        private Panel filterPanel;
        private Label lblFrom;
        private Label lblTo;
        private Label lblStatus;
        private Button btnRefresh;
        // ✅ PHÂN TRANG
        private Label lblPageInfo;
        private Button btnFirst;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnLast;
        private ComboBox cboPageSize;
        private TextBox txtPageNumber;
        private Button btnGoToPage;
        private System.Windows.Forms.Panel pnlPagination;
    }
}