namespace QL_Nha_Khoa
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabUsers;
        private System.Windows.Forms.Button btnRefreshUsers;
        private System.Windows.Forms.Button btnDeleteUser;
        private System.Windows.Forms.Button btnEditUser;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.TabPage tabShifts;
        private System.Windows.Forms.Button btnRefreshShifts;
        private System.Windows.Forms.Button btnAddShift;
        private System.Windows.Forms.DataGridView dgvShifts;
        private System.Windows.Forms.TabPage tabSalaries;
        private System.Windows.Forms.Button btnRefreshSalaries;
        private System.Windows.Forms.Button btnAddSalary;
        private System.Windows.Forms.DataGridView dgvSalaries;
        private System.Windows.Forms.TabPage tabReports;
        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DataGridView dgvReport;
        private System.Windows.Forms.TabPage tabAudit;
        private System.Windows.Forms.Button btnRefreshAudit;
        private System.Windows.Forms.DataGridView dgvAudit;

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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.btnRefreshUsers = new System.Windows.Forms.Button();
            this.btnDeleteUser = new System.Windows.Forms.Button();
            this.btnEditUser = new System.Windows.Forms.Button();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.tabShifts = new System.Windows.Forms.TabPage();
            this.btnRefreshShifts = new System.Windows.Forms.Button();
            this.btnAddShift = new System.Windows.Forms.Button();
            this.dgvShifts = new System.Windows.Forms.DataGridView();
            this.tabSalaries = new System.Windows.Forms.TabPage();
            this.btnRefreshSalaries = new System.Windows.Forms.Button();
            this.btnAddSalary = new System.Windows.Forms.Button();
            this.dgvSalaries = new System.Windows.Forms.DataGridView();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.tabAudit = new System.Windows.Forms.TabPage();
            this.btnRefreshAudit = new System.Windows.Forms.Button();
            this.dgvAudit = new System.Windows.Forms.DataGridView();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(16, 16);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(51, 20);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "label1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabUsers);
            this.tabControl1.Controls.Add(this.tabShifts);
            this.tabControl1.Controls.Add(this.tabSalaries);
            this.tabControl1.Controls.Add(this.tabReports);
            this.tabControl1.Controls.Add(this.tabAudit);
            this.tabControl1.Location = new System.Drawing.Point(16, 48);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 500);
            this.tabControl1.TabIndex = 1;
            // 
            // tabUsers
            // 
            this.tabUsers.Controls.Add(this.btnRefreshUsers);
            this.tabUsers.Controls.Add(this.btnDeleteUser);
            this.tabUsers.Controls.Add(this.btnEditUser);
            this.tabUsers.Controls.Add(this.btnAddUser);
            this.tabUsers.Controls.Add(this.dgvUsers);
            this.tabUsers.Location = new System.Drawing.Point(4, 29);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabUsers.Size = new System.Drawing.Size(752, 467);
            this.tabUsers.TabIndex = 0;
            this.tabUsers.Text = "Users";
            this.tabUsers.UseVisualStyleBackColor = true;
            // 
            // btnRefreshUsers
            // 
            this.btnRefreshUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshUsers.Location = new System.Drawing.Point(623, 12);
            this.btnRefreshUsers.Name = "btnRefreshUsers";
            this.btnRefreshUsers.Size = new System.Drawing.Size(112, 30);
            this.btnRefreshUsers.TabIndex = 4;
            this.btnRefreshUsers.Text = "Refresh";
            this.btnRefreshUsers.UseVisualStyleBackColor = true;
            this.btnRefreshUsers.Click += new System.EventHandler(this.btnRefreshUsers_Click);
            // 
            // btnDeleteUser
            // 
            this.btnDeleteUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteUser.Location = new System.Drawing.Point(505, 12);
            this.btnDeleteUser.Name = "btnDeleteUser";
            this.btnDeleteUser.Size = new System.Drawing.Size(112, 30);
            this.btnDeleteUser.TabIndex = 3;
            this.btnDeleteUser.Text = "Delete";
            this.btnDeleteUser.UseVisualStyleBackColor = true;
            this.btnDeleteUser.Click += new System.EventHandler(this.btnDeleteUser_Click);
            // 
            // btnEditUser
            // 
            this.btnEditUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditUser.Location = new System.Drawing.Point(387, 12);
            this.btnEditUser.Name = "btnEditUser";
            this.btnEditUser.Size = new System.Drawing.Size(112, 30);
            this.btnEditUser.TabIndex = 2;
            this.btnEditUser.Text = "Edit";
            this.btnEditUser.UseVisualStyleBackColor = true;
            this.btnEditUser.Click += new System.EventHandler(this.btnEditUser_Click);
            // 
            // btnAddUser
            // 
            this.btnAddUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddUser.Location = new System.Drawing.Point(269, 12);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(112, 30);
            this.btnAddUser.TabIndex = 1;
            this.btnAddUser.Text = "Add";
            this.btnAddUser.UseVisualStyleBackColor = true;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // dgvUsers
            // 
            this.dgvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsers.Location = new System.Drawing.Point(6, 52);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.RowHeadersWidth = 51;
            this.dgvUsers.Size = new System.Drawing.Size(740, 400);
            this.dgvUsers.TabIndex = 0;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // tabShifts
            // 
            this.tabShifts.Controls.Add(this.btnRefreshShifts);
            this.tabShifts.Controls.Add(this.btnAddShift);
            this.tabShifts.Controls.Add(this.dgvShifts);
            this.tabShifts.Location = new System.Drawing.Point(4, 29);
            this.tabShifts.Name = "tabShifts";
            this.tabShifts.Padding = new System.Windows.Forms.Padding(3);
            this.tabShifts.Size = new System.Drawing.Size(752, 467);
            this.tabShifts.TabIndex = 1;
            this.tabShifts.Text = "Shifts";
            this.tabShifts.UseVisualStyleBackColor = true;
            // 
            // btnRefreshShifts
            // 
            this.btnRefreshShifts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshShifts.Location = new System.Drawing.Point(623, 12);
            this.btnRefreshShifts.Name = "btnRefreshShifts";
            this.btnRefreshShifts.Size = new System.Drawing.Size(112, 30);
            this.btnRefreshShifts.TabIndex = 2;
            this.btnRefreshShifts.Text = "Refresh";
            this.btnRefreshShifts.UseVisualStyleBackColor = true;
            this.btnRefreshShifts.Click += new System.EventHandler(this.btnRefreshShifts_Click);
            // 
            // btnAddShift
            // 
            this.btnAddShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddShift.Location = new System.Drawing.Point(505, 12);
            this.btnAddShift.Name = "btnAddShift";
            this.btnAddShift.Size = new System.Drawing.Size(112, 30);
            this.btnAddShift.TabIndex = 1;
            this.btnAddShift.Text = "Add";
            this.btnAddShift.UseVisualStyleBackColor = true;
            this.btnAddShift.Click += new System.EventHandler(this.btnAddShift_Click);
            // 
            // dgvShifts
            // 
            this.dgvShifts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvShifts.Location = new System.Drawing.Point(6, 52);
            this.dgvShifts.Name = "dgvShifts";
            this.dgvShifts.RowHeadersWidth = 51;
            this.dgvShifts.Size = new System.Drawing.Size(740, 400);
            this.dgvShifts.TabIndex = 0;
            this.dgvShifts.ReadOnly = true;
            this.dgvShifts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // tabSalaries
            // 
            this.tabSalaries.Controls.Add(this.btnRefreshSalaries);
            this.tabSalaries.Controls.Add(this.btnAddSalary);
            this.tabSalaries.Controls.Add(this.dgvSalaries);
            this.tabSalaries.Location = new System.Drawing.Point(4, 29);
            this.tabSalaries.Name = "tabSalaries";
            this.tabSalaries.Padding = new System.Windows.Forms.Padding(3);
            this.tabSalaries.Size = new System.Drawing.Size(752, 467);
            this.tabSalaries.TabIndex = 2;
            this.tabSalaries.Text = "Salaries";
            this.tabSalaries.UseVisualStyleBackColor = true;
            // 
            // btnRefreshSalaries
            // 
            this.btnRefreshSalaries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshSalaries.Location = new System.Drawing.Point(623, 12);
            this.btnRefreshSalaries.Name = "btnRefreshSalaries";
            this.btnRefreshSalaries.Size = new System.Drawing.Size(112, 30);
            this.btnRefreshSalaries.TabIndex = 2;
            this.btnRefreshSalaries.Text = "Refresh";
            this.btnRefreshSalaries.UseVisualStyleBackColor = true;
            this.btnRefreshSalaries.Click += new System.EventHandler(this.btnRefreshSalaries_Click);
            // 
            // btnAddSalary
            // 
            this.btnAddSalary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSalary.Location = new System.Drawing.Point(505, 12);
            this.btnAddSalary.Name = "btnAddSalary";
            this.btnAddSalary.Size = new System.Drawing.Size(112, 30);
            this.btnAddSalary.TabIndex = 1;
            this.btnAddSalary.Text = "Add";
            this.btnAddSalary.UseVisualStyleBackColor = true;
            this.btnAddSalary.Click += new System.EventHandler(this.btnAddSalary_Click);
            // 
            // dgvSalaries
            // 
            this.dgvSalaries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSalaries.Location = new System.Drawing.Point(6, 52);
            this.dgvSalaries.Name = "dgvSalaries";
            this.dgvSalaries.RowHeadersWidth = 51;
            this.dgvSalaries.Size = new System.Drawing.Size(740, 400);
            this.dgvSalaries.TabIndex = 0;
            this.dgvSalaries.ReadOnly = true;
            this.dgvSalaries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // tabReports
            // 
            this.tabReports.Controls.Add(this.btnGenerateReport);
            this.tabReports.Controls.Add(this.dtpTo);
            this.tabReports.Controls.Add(this.dtpFrom);
            this.tabReports.Controls.Add(this.dgvReport);
            this.tabReports.Location = new System.Drawing.Point(4, 29);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabReports.Size = new System.Drawing.Size(752, 467);
            this.tabReports.TabIndex = 3;
            this.tabReports.Text = "Reports";
            this.tabReports.UseVisualStyleBackColor = true;
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateReport.Location = new System.Drawing.Point(623, 12);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(112, 30);
            this.btnGenerateReport.TabIndex = 3;
            this.btnGenerateReport.Text = "Generate";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // dtpTo
            // 
            this.dtpTo.Location = new System.Drawing.Point(255, 14);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(240, 27);
            this.dtpTo.TabIndex = 2;
            // 
            // dtpFrom
            // 
            this.dtpFrom.Location = new System.Drawing.Point(9, 14);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(240, 27);
            this.dtpFrom.TabIndex = 1;
            // 
            // dgvReport
            // 
            this.dgvReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReport.Location = new System.Drawing.Point(6, 52);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.RowHeadersWidth = 51;
            this.dgvReport.Size = new System.Drawing.Size(740, 400);
            this.dgvReport.TabIndex = 0;
            this.dgvReport.ReadOnly = true;
            // 
            // tabAudit
            // 
            this.tabAudit.Controls.Add(this.btnRefreshAudit);
            this.tabAudit.Controls.Add(this.dgvAudit);
            this.tabAudit.Location = new System.Drawing.Point(4, 29);
            this.tabAudit.Name = "tabAudit";
            this.tabAudit.Padding = new System.Windows.Forms.Padding(3);
            this.tabAudit.Size = new System.Drawing.Size(752, 467);
            this.tabAudit.TabIndex = 4;
            this.tabAudit.Text = "Audit";
            this.tabAudit.UseVisualStyleBackColor = true;
            // 
            // btnRefreshAudit
            // 
            this.btnRefreshAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshAudit.Location = new System.Drawing.Point(623, 12);
            this.btnRefreshAudit.Name = "btnRefreshAudit";
            this.btnRefreshAudit.Size = new System.Drawing.Size(112, 30);
            this.btnRefreshAudit.TabIndex = 1;
            this.btnRefreshAudit.Text = "Refresh";
            this.btnRefreshAudit.UseVisualStyleBackColor = true;
            this.btnRefreshAudit.Click += new System.EventHandler(this.btnRefreshAudit_Click);
            // 
            // dgvAudit
            // 
            this.dgvAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAudit.Location = new System.Drawing.Point(6, 52);
            this.dgvAudit.Name = "dgvAudit";
            this.dgvAudit.RowHeadersWidth = 51;
            this.dgvAudit.Size = new System.Drawing.Size(740, 400);
            this.dgvAudit.TabIndex = 0;
            this.dgvAudit.ReadOnly = true;

            // 
            // AdminForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 570);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblWelcome);
            this.Name = "AdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin Dashboard";
            this.Load += new System.EventHandler(this.AdminForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
