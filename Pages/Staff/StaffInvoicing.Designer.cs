namespace DentalClinicManagement.Pages.Staff
{
    partial class StaffInvoicing
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgvInvoices;

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
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.panelHeader.SuspendLayout();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1257, 64);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(341, 46);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "💰 Quản lý hóa đơn";
            // 
            // panelFilter
            // 
            this.panelFilter.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelFilter.Controls.Add(this.lblStatus);
            this.panelFilter.Controls.Add(this.cboStatus);
            this.panelFilter.Controls.Add(this.btnRefresh);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 64);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1257, 64);
            this.panelFilter.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStatus.Location = new System.Drawing.Point(23, 21);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(91, 23);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Items.AddRange(new object[] {
            "Tất cả",
            "Chưa thanh toán",
            "Đã thanh toán",
            "Đã hủy"});
            this.cboStatus.Location = new System.Drawing.Point(120, 18);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(228, 31);
            this.cboStatus.TabIndex = 1;
            this.cboStatus.SelectedIndexChanged += new System.EventHandler(this.CboStatus_SelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(409, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(114, 32);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.AllowUserToAddRows = false;
            this.dgvInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInvoices.BackgroundColor = System.Drawing.Color.White;
            this.dgvInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvInvoices.ColumnHeadersHeight = 29;
            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoices.Location = new System.Drawing.Point(0, 128);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.ReadOnly = true;
            this.dgvInvoices.RowHeadersWidth = 51;
            this.dgvInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoices.Size = new System.Drawing.Size(1257, 565);
            this.dgvInvoices.TabIndex = 2;
            this.dgvInvoices.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvInvoices_CellClick);
            // 
            // StaffInvoicing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.dgvInvoices);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.panelHeader);
            this.Name = "StaffInvoicing";
            this.Size = new System.Drawing.Size(1257, 693);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.ResumeLayout(false);

        }
    }
}