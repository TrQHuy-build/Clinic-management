namespace DentalClinicManagement.Pages.Staff
{
    partial class StaffDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel panelStats;
        private System.Windows.Forms.Panel panelStat1;
        private System.Windows.Forms.Panel panelStat2;
        private System.Windows.Forms.Panel panelStat3;
        private System.Windows.Forms.Panel panelStat4;
        private System.Windows.Forms.Label lblStat1Title;
        private System.Windows.Forms.Label lblTodayAppointments;
        private System.Windows.Forms.Label lblStat2Title;
        private System.Windows.Forms.Label lblPendingAppointments;
        private System.Windows.Forms.Label lblStat3Title;
        private System.Windows.Forms.Label lblUnpaidInvoices;
        private System.Windows.Forms.Label lblStat4Title;
        private System.Windows.Forms.Label lblNewPatients;
        private System.Windows.Forms.GroupBox grpTodayAppointments;
        private System.Windows.Forms.DataGridView dgvTodayAppointments;
        private System.Windows.Forms.GroupBox grpPendingInvoices;
        private System.Windows.Forms.DataGridView dgvPendingInvoices;

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
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelStats = new System.Windows.Forms.Panel();
            this.panelStat1 = new System.Windows.Forms.Panel();
            this.lblStat1Title = new System.Windows.Forms.Label();
            this.lblTodayAppointments = new System.Windows.Forms.Label();
            this.panelStat2 = new System.Windows.Forms.Panel();
            this.lblStat2Title = new System.Windows.Forms.Label();
            this.lblPendingAppointments = new System.Windows.Forms.Label();
            this.panelStat3 = new System.Windows.Forms.Panel();
            this.lblStat3Title = new System.Windows.Forms.Label();
            this.lblUnpaidInvoices = new System.Windows.Forms.Label();
            this.panelStat4 = new System.Windows.Forms.Panel();
            this.lblStat4Title = new System.Windows.Forms.Label();
            this.lblNewPatients = new System.Windows.Forms.Label();
            this.grpTodayAppointments = new System.Windows.Forms.GroupBox();
            this.dgvTodayAppointments = new System.Windows.Forms.DataGridView();
            this.grpPendingInvoices = new System.Windows.Forms.GroupBox();
            this.dgvPendingInvoices = new System.Windows.Forms.DataGridView();
            this.panelHeader.SuspendLayout();
            this.panelStats.SuspendLayout();
            this.panelStat1.SuspendLayout();
            this.panelStat2.SuspendLayout();
            this.panelStat3.SuspendLayout();
            this.panelStat4.SuspendLayout();
            this.grpTodayAppointments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTodayAppointments)).BeginInit();
            this.grpPendingInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendingInvoices)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Controls.Add(this.btnRefresh);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1448, 87);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(23, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(390, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 Dashboard - Staff";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(1109, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(114, 37);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // panelStats
            // 
            this.panelStats.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelStats.Controls.Add(this.panelStat1);
            this.panelStats.Controls.Add(this.panelStat2);
            this.panelStats.Controls.Add(this.panelStat3);
            this.panelStats.Controls.Add(this.panelStat4);
            this.panelStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStats.Location = new System.Drawing.Point(0, 87);
            this.panelStats.Name = "panelStats";
            this.panelStats.Padding = new System.Windows.Forms.Padding(23, 21, 23, 11);
            this.panelStats.Size = new System.Drawing.Size(1448, 133);
            this.panelStats.TabIndex = 1;
            // 
            // panelStat1
            // 
            this.panelStat1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(242)))), ((int)(((byte)(253)))));
            this.panelStat1.Controls.Add(this.lblStat1Title);
            this.panelStat1.Controls.Add(this.lblTodayAppointments);
            this.panelStat1.Location = new System.Drawing.Point(34, 21);
            this.panelStat1.Name = "panelStat1";
            this.panelStat1.Size = new System.Drawing.Size(286, 98);
            this.panelStat1.TabIndex = 0;
            // 
            // lblStat1Title
            // 
            this.lblStat1Title.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStat1Title.Location = new System.Drawing.Point(11, 11);
            this.lblStat1Title.Name = "lblStat1Title";
            this.lblStat1Title.Size = new System.Drawing.Size(263, 27);
            this.lblStat1Title.TabIndex = 0;
            this.lblStat1Title.Text = "📅 Lịch hẹn hôm nay";
            // 
            // lblTodayAppointments
            // 
            this.lblTodayAppointments.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTodayAppointments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblTodayAppointments.Location = new System.Drawing.Point(11, 37);
            this.lblTodayAppointments.Name = "lblTodayAppointments";
            this.lblTodayAppointments.Size = new System.Drawing.Size(263, 43);
            this.lblTodayAppointments.TabIndex = 1;
            this.lblTodayAppointments.Text = "0";
            // 
            // panelStat2
            // 
            this.panelStat2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(243)))), ((int)(((byte)(205)))));
            this.panelStat2.Controls.Add(this.lblStat2Title);
            this.panelStat2.Controls.Add(this.lblPendingAppointments);
            this.panelStat2.Location = new System.Drawing.Point(343, 21);
            this.panelStat2.Name = "panelStat2";
            this.panelStat2.Size = new System.Drawing.Size(286, 98);
            this.panelStat2.TabIndex = 1;
            // 
            // lblStat2Title
            // 
            this.lblStat2Title.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStat2Title.Location = new System.Drawing.Point(11, 11);
            this.lblStat2Title.Name = "lblStat2Title";
            this.lblStat2Title.Size = new System.Drawing.Size(263, 27);
            this.lblStat2Title.TabIndex = 0;
            this.lblStat2Title.Text = "⏳ Chờ BS xác nhận";
            // 
            // lblPendingAppointments
            // 
            this.lblPendingAppointments.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblPendingAppointments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(124)))), ((int)(((byte)(0)))));
            this.lblPendingAppointments.Location = new System.Drawing.Point(11, 37);
            this.lblPendingAppointments.Name = "lblPendingAppointments";
            this.lblPendingAppointments.Size = new System.Drawing.Size(263, 43);
            this.lblPendingAppointments.TabIndex = 1;
            this.lblPendingAppointments.Text = "0";
            // 
            // panelStat3
            // 
            this.panelStat3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(238)))));
            this.panelStat3.Controls.Add(this.lblStat3Title);
            this.panelStat3.Controls.Add(this.lblUnpaidInvoices);
            this.panelStat3.Location = new System.Drawing.Point(651, 21);
            this.panelStat3.Name = "panelStat3";
            this.panelStat3.Size = new System.Drawing.Size(286, 98);
            this.panelStat3.TabIndex = 2;
            // 
            // lblStat3Title
            // 
            this.lblStat3Title.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStat3Title.Location = new System.Drawing.Point(11, 11);
            this.lblStat3Title.Name = "lblStat3Title";
            this.lblStat3Title.Size = new System.Drawing.Size(263, 27);
            this.lblStat3Title.TabIndex = 0;
            this.lblStat3Title.Text = "💰 Hóa đơn chưa TT";
            // 
            // lblUnpaidInvoices
            // 
            this.lblUnpaidInvoices.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblUnpaidInvoices.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.lblUnpaidInvoices.Location = new System.Drawing.Point(11, 37);
            this.lblUnpaidInvoices.Name = "lblUnpaidInvoices";
            this.lblUnpaidInvoices.Size = new System.Drawing.Size(263, 43);
            this.lblUnpaidInvoices.TabIndex = 1;
            this.lblUnpaidInvoices.Text = "0";
            // 
            // panelStat4
            // 
            this.panelStat4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(233)))));
            this.panelStat4.Controls.Add(this.lblStat4Title);
            this.panelStat4.Controls.Add(this.lblNewPatients);
            this.panelStat4.Location = new System.Drawing.Point(960, 21);
            this.panelStat4.Name = "panelStat4";
            this.panelStat4.Size = new System.Drawing.Size(286, 98);
            this.panelStat4.TabIndex = 3;
            // 
            // lblStat4Title
            // 
            this.lblStat4Title.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStat4Title.Location = new System.Drawing.Point(11, 11);
            this.lblStat4Title.Name = "lblStat4Title";
            this.lblStat4Title.Size = new System.Drawing.Size(263, 27);
            this.lblStat4Title.TabIndex = 0;
            this.lblStat4Title.Text = "👥 BN mới tháng này";
            // 
            // lblNewPatients
            // 
            this.lblNewPatients.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblNewPatients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(142)))), ((int)(((byte)(60)))));
            this.lblNewPatients.Location = new System.Drawing.Point(11, 37);
            this.lblNewPatients.Name = "lblNewPatients";
            this.lblNewPatients.Size = new System.Drawing.Size(263, 43);
            this.lblNewPatients.TabIndex = 1;
            this.lblNewPatients.Text = "0";
            // 
            // grpTodayAppointments
            // 
            this.grpTodayAppointments.Controls.Add(this.dgvTodayAppointments);
            this.grpTodayAppointments.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.grpTodayAppointments.Location = new System.Drawing.Point(34, 253);
            this.grpTodayAppointments.Name = "grpTodayAppointments";
            this.grpTodayAppointments.Size = new System.Drawing.Size(682, 416);
            this.grpTodayAppointments.TabIndex = 2;
            this.grpTodayAppointments.TabStop = false;
            this.grpTodayAppointments.Text = "📅 Lịch hẹn hôm nay";
            // 
            // dgvTodayAppointments
            // 
            this.dgvTodayAppointments.AllowUserToAddRows = false;
            this.dgvTodayAppointments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTodayAppointments.BackgroundColor = System.Drawing.Color.White;
            this.dgvTodayAppointments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTodayAppointments.ColumnHeadersHeight = 29;
            this.dgvTodayAppointments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTodayAppointments.Location = new System.Drawing.Point(3, 28);
            this.dgvTodayAppointments.Name = "dgvTodayAppointments";
            this.dgvTodayAppointments.ReadOnly = true;
            this.dgvTodayAppointments.RowHeadersWidth = 51;
            this.dgvTodayAppointments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTodayAppointments.Size = new System.Drawing.Size(676, 385);
            this.dgvTodayAppointments.TabIndex = 0;
            // 
            // grpPendingInvoices
            // 
            this.grpPendingInvoices.Controls.Add(this.dgvPendingInvoices);
            this.grpPendingInvoices.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.grpPendingInvoices.Location = new System.Drawing.Point(742, 253);
            this.grpPendingInvoices.Name = "grpPendingInvoices";
            this.grpPendingInvoices.Size = new System.Drawing.Size(681, 416);
            this.grpPendingInvoices.TabIndex = 3;
            this.grpPendingInvoices.TabStop = false;
            this.grpPendingInvoices.Text = "💰 Hóa đơn chưa thanh toán";
            // 
            // dgvPendingInvoices
            // 
            this.dgvPendingInvoices.AllowUserToAddRows = false;
            this.dgvPendingInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPendingInvoices.BackgroundColor = System.Drawing.Color.White;
            this.dgvPendingInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPendingInvoices.ColumnHeadersHeight = 29;
            this.dgvPendingInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPendingInvoices.Location = new System.Drawing.Point(3, 28);
            this.dgvPendingInvoices.Name = "dgvPendingInvoices";
            this.dgvPendingInvoices.ReadOnly = true;
            this.dgvPendingInvoices.RowHeadersWidth = 51;
            this.dgvPendingInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPendingInvoices.Size = new System.Drawing.Size(675, 385);
            this.dgvPendingInvoices.TabIndex = 0;
            // 
            // StaffDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpPendingInvoices);
            this.Controls.Add(this.grpTodayAppointments);
            this.Controls.Add(this.panelStats);
            this.Controls.Add(this.panelHeader);
            this.Name = "StaffDashboard";
            this.Size = new System.Drawing.Size(1448, 720);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelStats.ResumeLayout(false);
            this.panelStat1.ResumeLayout(false);
            this.panelStat2.ResumeLayout(false);
            this.panelStat3.ResumeLayout(false);
            this.panelStat4.ResumeLayout(false);
            this.grpTodayAppointments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTodayAppointments)).EndInit();
            this.grpPendingInvoices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendingInvoices)).EndInit();
            this.ResumeLayout(false);

        }
    }
}