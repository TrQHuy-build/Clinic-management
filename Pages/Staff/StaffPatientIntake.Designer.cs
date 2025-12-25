namespace DentalClinicManagement.Pages.Staff
{
    partial class StaffPatientIntake
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpAppointment;
        private System.Windows.Forms.Label lblAppointment;
        private System.Windows.Forms.ComboBox cboAppointment;
        private System.Windows.Forms.GroupBox grpBasicInfo;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblExistingPatient;
        private System.Windows.Forms.GroupBox grpPatientDetails;
        private System.Windows.Forms.Label lblDateOfBirth;
        private System.Windows.Forms.DateTimePicker dtpDateOfBirth;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.ComboBox cboGender;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblInsurance;
        private System.Windows.Forms.TextBox txtInsurance;
        private System.Windows.Forms.GroupBox grpDoctor;
        private System.Windows.Forms.Label lblDoctor;
        private System.Windows.Forms.ComboBox cboDoctor;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;

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
            this.grpAppointment = new System.Windows.Forms.GroupBox();
            this.lblAppointment = new System.Windows.Forms.Label();
            this.cboAppointment = new System.Windows.Forms.ComboBox();
            this.grpBasicInfo = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblExistingPatient = new System.Windows.Forms.Label();
            this.grpPatientDetails = new System.Windows.Forms.GroupBox();
            this.lblDateOfBirth = new System.Windows.Forms.Label();
            this.dtpDateOfBirth = new System.Windows.Forms.DateTimePicker();
            this.lblGender = new System.Windows.Forms.Label();
            this.cboGender = new System.Windows.Forms.ComboBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblInsurance = new System.Windows.Forms.Label();
            this.txtInsurance = new System.Windows.Forms.TextBox();
            this.grpDoctor = new System.Windows.Forms.GroupBox();
            this.lblDoctor = new System.Windows.Forms.Label();
            this.cboDoctor = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.grpAppointment.SuspendLayout();
            this.grpBasicInfo.SuspendLayout();
            this.grpPatientDetails.SuspendLayout();
            this.grpDoctor.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1254, 64);
            this.panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(515, 45);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "👤 Kê khai thông tin bệnh nhân";
            // 
            // grpAppointment
            // 
            this.grpAppointment.Controls.Add(this.lblAppointment);
            this.grpAppointment.Controls.Add(this.cboAppointment);
            this.grpAppointment.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpAppointment.Location = new System.Drawing.Point(34, 85);
            this.grpAppointment.Name = "grpAppointment";
            this.grpAppointment.Size = new System.Drawing.Size(1189, 89);
            this.grpAppointment.TabIndex = 1;
            this.grpAppointment.TabStop = false;
            this.grpAppointment.Text = "Chọn lịch hẹn";
            // 
            // lblAppointment
            // 
            this.lblAppointment.AutoSize = true;
            this.lblAppointment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAppointment.Location = new System.Drawing.Point(23, 37);
            this.lblAppointment.Name = "lblAppointment";
            this.lblAppointment.Size = new System.Drawing.Size(78, 23);
            this.lblAppointment.TabIndex = 0;
            this.lblAppointment.Text = "Lịch hẹn:";
            // 
            // cboAppointment
            // 
            this.cboAppointment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAppointment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboAppointment.FormattingEnabled = true;
            this.cboAppointment.Location = new System.Drawing.Point(137, 34);
            this.cboAppointment.Name = "cboAppointment";
            this.cboAppointment.Size = new System.Drawing.Size(1017, 31);
            this.cboAppointment.TabIndex = 1;
            this.cboAppointment.SelectedIndexChanged += new System.EventHandler(this.CboAppointment_SelectedIndexChanged);
            // 
            // grpBasicInfo
            // 
            this.grpBasicInfo.Controls.Add(this.lblName);
            this.grpBasicInfo.Controls.Add(this.txtName);
            this.grpBasicInfo.Controls.Add(this.lblPhone);
            this.grpBasicInfo.Controls.Add(this.txtPhone);
            this.grpBasicInfo.Controls.Add(this.lblEmail);
            this.grpBasicInfo.Controls.Add(this.txtEmail);
            this.grpBasicInfo.Controls.Add(this.lblExistingPatient);
            this.grpBasicInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpBasicInfo.Location = new System.Drawing.Point(34, 195);
            this.grpBasicInfo.Name = "grpBasicInfo";
            this.grpBasicInfo.Size = new System.Drawing.Size(1189, 165);
            this.grpBasicInfo.TabIndex = 2;
            this.grpBasicInfo.TabStop = false;
            this.grpBasicInfo.Text = "Thông tin cơ bản";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblName.Location = new System.Drawing.Point(23, 37);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(88, 23);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Họ và tên:";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.Location = new System.Drawing.Point(137, 34);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(342, 30);
            this.txtName.TabIndex = 1;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPhone.Location = new System.Drawing.Point(514, 37);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(44, 23);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "SĐT:";
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPhone.Location = new System.Drawing.Point(606, 34);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.ReadOnly = true;
            this.txtPhone.Size = new System.Drawing.Size(228, 30);
            this.txtPhone.TabIndex = 3;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEmail.Location = new System.Drawing.Point(23, 106);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(55, 23);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email:";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.Location = new System.Drawing.Point(137, 103);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.ReadOnly = true;
            this.txtEmail.Size = new System.Drawing.Size(342, 30);
            this.txtEmail.TabIndex = 5;
            // 
            // lblExistingPatient
            // 
            this.lblExistingPatient.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblExistingPatient.Location = new System.Drawing.Point(514, 75);
            this.lblExistingPatient.Name = "lblExistingPatient";
            this.lblExistingPatient.Size = new System.Drawing.Size(640, 32);
            this.lblExistingPatient.TabIndex = 6;
            this.lblExistingPatient.Visible = false;
            // 
            // grpPatientDetails
            // 
            this.grpPatientDetails.Controls.Add(this.lblDateOfBirth);
            this.grpPatientDetails.Controls.Add(this.dtpDateOfBirth);
            this.grpPatientDetails.Controls.Add(this.lblGender);
            this.grpPatientDetails.Controls.Add(this.cboGender);
            this.grpPatientDetails.Controls.Add(this.lblAddress);
            this.grpPatientDetails.Controls.Add(this.txtAddress);
            this.grpPatientDetails.Controls.Add(this.lblInsurance);
            this.grpPatientDetails.Controls.Add(this.txtInsurance);
            this.grpPatientDetails.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpPatientDetails.Location = new System.Drawing.Point(34, 363);
            this.grpPatientDetails.Name = "grpPatientDetails";
            this.grpPatientDetails.Size = new System.Drawing.Size(1189, 237);
            this.grpPatientDetails.TabIndex = 3;
            this.grpPatientDetails.TabStop = false;
            this.grpPatientDetails.Text = "Thông tin chi tiết";
            // 
            // lblDateOfBirth
            // 
            this.lblDateOfBirth.AutoSize = true;
            this.lblDateOfBirth.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDateOfBirth.Location = new System.Drawing.Point(23, 37);
            this.lblDateOfBirth.Name = "lblDateOfBirth";
            this.lblDateOfBirth.Size = new System.Drawing.Size(90, 23);
            this.lblDateOfBirth.TabIndex = 0;
            this.lblDateOfBirth.Text = "Ngày sinh:";
            // 
            // dtpDateOfBirth
            // 
            this.dtpDateOfBirth.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpDateOfBirth.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateOfBirth.Location = new System.Drawing.Point(137, 34);
            this.dtpDateOfBirth.Name = "dtpDateOfBirth";
            this.dtpDateOfBirth.Size = new System.Drawing.Size(228, 30);
            this.dtpDateOfBirth.TabIndex = 1;
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblGender.Location = new System.Drawing.Point(400, 37);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(79, 23);
            this.lblGender.TabIndex = 2;
            this.lblGender.Text = "Giới tính:";
            // 
            // cboGender
            // 
            this.cboGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGender.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboGender.FormattingEnabled = true;
            this.cboGender.Items.AddRange(new object[] {
            "Male",
            "Female",
            "Other"});
            this.cboGender.Location = new System.Drawing.Point(503, 34);
            this.cboGender.Name = "cboGender";
            this.cboGender.Size = new System.Drawing.Size(171, 31);
            this.cboGender.TabIndex = 3;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAddress.Location = new System.Drawing.Point(23, 132);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(66, 23);
            this.lblAddress.TabIndex = 4;
            this.lblAddress.Text = "Địa chỉ:";
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtAddress.Location = new System.Drawing.Point(137, 129);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(537, 53);
            this.txtAddress.TabIndex = 5;
            // 
            // lblInsurance
            // 
            this.lblInsurance.AutoSize = true;
            this.lblInsurance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInsurance.Location = new System.Drawing.Point(709, 132);
            this.lblInsurance.Name = "lblInsurance";
            this.lblInsurance.Size = new System.Drawing.Size(54, 23);
            this.lblInsurance.TabIndex = 6;
            this.lblInsurance.Text = "BHYT:";
            // 
            // txtInsurance
            // 
            this.txtInsurance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtInsurance.Location = new System.Drawing.Point(777, 129);
            this.txtInsurance.Name = "txtInsurance";
            this.txtInsurance.Size = new System.Drawing.Size(377, 30);
            this.txtInsurance.TabIndex = 7;
            // 
            // grpDoctor
            // 
            this.grpDoctor.Controls.Add(this.lblDoctor);
            this.grpDoctor.Controls.Add(this.cboDoctor);
            this.grpDoctor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpDoctor.Location = new System.Drawing.Point(34, 606);
            this.grpDoctor.Name = "grpDoctor";
            this.grpDoctor.Size = new System.Drawing.Size(1189, 114);
            this.grpDoctor.TabIndex = 4;
            this.grpDoctor.TabStop = false;
            this.grpDoctor.Text = "Chọn bác sĩ khám";
            // 
            // lblDoctor
            // 
            this.lblDoctor.AutoSize = true;
            this.lblDoctor.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDoctor.Location = new System.Drawing.Point(23, 37);
            this.lblDoctor.Name = "lblDoctor";
            this.lblDoctor.Size = new System.Drawing.Size(57, 23);
            this.lblDoctor.TabIndex = 0;
            this.lblDoctor.Text = "Bác sĩ:";
            // 
            // cboDoctor
            // 
            this.cboDoctor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDoctor.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboDoctor.FormattingEnabled = true;
            this.cboDoctor.Location = new System.Drawing.Point(137, 34);
            this.cboDoctor.Name = "cboDoctor";
            this.cboDoctor.Size = new System.Drawing.Size(1017, 31);
            this.cboDoctor.TabIndex = 1;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(856, 737);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(137, 43);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "💾 Lưu";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.Gray;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(1026, 737);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(197, 43);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "🔄 Làm mới";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // StaffPatientIntake
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpDoctor);
            this.Controls.Add(this.grpPatientDetails);
            this.Controls.Add(this.grpBasicInfo);
            this.Controls.Add(this.grpAppointment);
            this.Controls.Add(this.panelHeader);
            this.Name = "StaffPatientIntake";
            this.Size = new System.Drawing.Size(1254, 796);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.grpAppointment.ResumeLayout(false);
            this.grpAppointment.PerformLayout();
            this.grpBasicInfo.ResumeLayout(false);
            this.grpBasicInfo.PerformLayout();
            this.grpPatientDetails.ResumeLayout(false);
            this.grpPatientDetails.PerformLayout();
            this.grpDoctor.ResumeLayout(false);
            this.grpDoctor.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}