using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    partial class AdminInventory
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Declare controls
        private Label lblTitle;
        private Panel searchPanel;
        private Label lblSearch;
        private TextBox txtSearch;
        private Label lblType;
        private ComboBox cboType;
        private Button btnAdd;
        private Button btnImport;
        private Button btnRefresh;
        private DataGridView dgvInventory;

        // ✅ PHÂN TRANG
        private Label lblPageInfo;
        private Button btnFirst;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnLast;
        private ComboBox cboPageSize;
        private TextBox txtPageNumber;
        private Button btnGoToPage;
        private Panel pnlPagination;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitle = new System.Windows.Forms.Label();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.cboPageSize = new System.Windows.Forms.ComboBox();
            this.txtPageNumber = new System.Windows.Forms.TextBox();
            this.btnGoToPage = new System.Windows.Forms.Button();
            this.pnlPagination = new System.Windows.Forms.Panel();
            this.searchPanel.SuspendLayout();
            this.pnlPagination.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(0, -1);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(516, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý Kho vật tư / Thiết bị";
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.White;
            this.searchPanel.Controls.Add(this.lblSearch);
            this.searchPanel.Controls.Add(this.txtSearch);
            this.searchPanel.Controls.Add(this.lblType);
            this.searchPanel.Controls.Add(this.cboType);
            this.searchPanel.Controls.Add(this.btnAdd);
            this.searchPanel.Controls.Add(this.btnRefresh);
            this.searchPanel.Controls.Add(this.btnImport);
            this.searchPanel.Location = new System.Drawing.Point(21, 60);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(1227, 60);
            this.searchPanel.TabIndex = 1;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSearch.Location = new System.Drawing.Point(20, 20);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(83, 23);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Tìm kiếm:";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSearch.Location = new System.Drawing.Point(116, 17);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 32);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblType.Location = new System.Drawing.Point(380, 20);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(45, 23);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Loại:";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboType.Items.AddRange(new object[] {
            "Tất cả",
            "Material",
            "Equipment"});
            this.cboType.Location = new System.Drawing.Point(431, 17);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(150, 33);
            this.cboType.TabIndex = 3;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.CboType_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(815, 13);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 35);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "+ Thêm mới";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(954, 13);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 35);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnImport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnImport.FlatAppearance.BorderSize = 0;
            this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImport.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnImport.ForeColor = System.Drawing.Color.White;
            this.btnImport.Location = new System.Drawing.Point(1086, 13);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(120, 35);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "📦 Nhập kho";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.BtnImport_Click);
            // 
            // dgvInventory
            // 
            this.dgvInventory.AllowUserToAddRows = false;
            this.dgvInventory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dgvInventory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInventory.BackgroundColor = System.Drawing.Color.White;
            this.dgvInventory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInventory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvInventory.ColumnHeadersHeight = 40;
            this.dgvInventory.EnableHeadersVisualStyles = false;
            this.dgvInventory.Location = new System.Drawing.Point(23, 131);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.ReadOnly = true;
            this.dgvInventory.RowHeadersWidth = 51;
            this.dgvInventory.RowTemplate.Height = 35;
            this.dgvInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInventory.Size = new System.Drawing.Size(1407, 623);
            this.dgvInventory.TabIndex = 2;
            
            // ========================================
            // Pagination Panel
            // ========================================
            this.pnlPagination = new System.Windows.Forms.Panel();
            this.pnlPagination.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlPagination.BackColor = System.Drawing.Color.White;
            this.pnlPagination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPagination.Location = new System.Drawing.Point(23, 765);
            this.pnlPagination.Name = "pnlPagination";
            this.pnlPagination.Size = new System.Drawing.Size(1407, 50);

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
            lblPageSize.Location = new System.Drawing.Point(350, 15);
            lblPageSize.Text = "Hiển thị:";

            this.cboPageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPageSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboPageSize.Location = new System.Drawing.Point(415, 12);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(80, 25);
            this.cboPageSize.Items.AddRange(new object[] { 25, 50, 100, 200 });
            this.cboPageSize.SelectedIndex = 1;
            this.cboPageSize.SelectedIndexChanged += new System.EventHandler(this.CboPageSize_SelectedIndexChanged);

            System.Windows.Forms.Label lblRecords = new System.Windows.Forms.Label();
            lblRecords.AutoSize = true;
            lblRecords.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblRecords.Location = new System.Drawing.Point(500, 15);
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
            lblGoTo.Location = new System.Drawing.Point(1200, 17);
            lblGoTo.Text = "Đến trang:";

            this.txtPageNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPageNumber.Location = new System.Drawing.Point(1280, 13);
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.Size = new System.Drawing.Size(60, 25);
            this.txtPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPageNumber.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (char)Keys.Enter)
                {
                    BtnGoToPage_Click(this.btnGoToPage, null);
                    e.Handled = true;
                }
            };

            this.btnGoToPage.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.btnGoToPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGoToPage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnGoToPage.ForeColor = System.Drawing.Color.White;
            this.btnGoToPage.Location = new System.Drawing.Point(1345, 10);
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
            
            // Update dgvInventory height and location
            this.dgvInventory.Size = new System.Drawing.Size(1407, 620);
            this.dgvInventory.Location = new System.Drawing.Point(23, 131);
            // 
            // AdminInventory
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.pnlPagination);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.searchPanel);
            this.Controls.Add(this.dgvInventory);
            this.Name = "AdminInventory";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1453, 830);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.pnlPagination.ResumeLayout(false);
            this.pnlPagination.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}