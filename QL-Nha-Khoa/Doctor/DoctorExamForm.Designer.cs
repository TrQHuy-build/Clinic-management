namespace QL_Nha_Khoa
{
    partial class DoctorExamForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvAppointments;
        private System.Windows.Forms.TextBox txtPatientName;
        private System.Windows.Forms.TextBox txtPatientPhone;
        private System.Windows.Forms.TextBox txtPatientEmail;
        private System.Windows.Forms.TextBox txtDOB;
        private System.Windows.Forms.TextBox txtGender;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.ComboBox cmbService;
        private System.Windows.Forms.DataGridView dgvMedicines;
        private System.Windows.Forms.NumericUpDown numPrescribeQty;
        private System.Windows.Forms.Button btnAddPrescription;
        private System.Windows.Forms.DataGridView dgvPrescriptions;
        private System.Windows.Forms.TextBox txtDiagnosis;
        private System.Windows.Forms.TextBox txtTreatment;
        private System.Windows.Forms.Button btnSave;

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
            this.dgvAppointments = new System.Windows.Forms.DataGridView();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.txtPatientPhone = new System.Windows.Forms.TextBox();
            this.txtPatientEmail = new System.Windows.Forms.TextBox();
            this.txtDOB = new System.Windows.Forms.TextBox();
            this.txtGender = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.cmbService = new System.Windows.Forms.ComboBox();
            this.dgvMedicines = new System.Windows.Forms.DataGridView();
            this.numPrescribeQty = new System.Windows.Forms.NumericUpDown();
            this.btnAddPrescription = new System.Windows.Forms.Button();
            this.dgvPrescriptions = new System.Windows.Forms.DataGridView();
            this.txtDiagnosis = new System.Windows.Forms.TextBox();
            this.txtTreatment = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrescribeQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrescriptions)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAppointments
            // 
            this.dgvAppointments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAppointments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAppointments.Location = new System.Drawing.Point(12, 12);
            this.dgvAppointments.Name = "dgvAppointments";
            this.dgvAppointments.RowTemplate.Height = 24;
            this.dgvAppointments.Size = new System.Drawing.Size(760, 150);
            this.dgvAppointments.TabIndex = 0;
            this.dgvAppointments.SelectionChanged += new System.EventHandler(this.dgvAppointments_SelectionChanged);
            // 
            // txtPatientName
            // 
            this.txtPatientName.Location = new System.Drawing.Point(12, 170);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.ReadOnly = true;
            this.txtPatientName.Size = new System.Drawing.Size(200, 22);
            this.txtPatientName.TabIndex = 1;
            // 
            // txtPatientPhone
            // 
            this.txtPatientPhone.Location = new System.Drawing.Point(220, 170);
            this.txtPatientPhone.Name = "txtPatientPhone";
            this.txtPatientPhone.ReadOnly = true;
            this.txtPatientPhone.Size = new System.Drawing.Size(150, 22);
            this.txtPatientPhone.TabIndex = 2;
            // 
            // txtPatientEmail
            // 
            this.txtPatientEmail.Location = new System.Drawing.Point(380, 170);
            this.txtPatientEmail.Name = "txtPatientEmail";
            this.txtPatientEmail.ReadOnly = true;
            this.txtPatientEmail.Size = new System.Drawing.Size(200, 22);
            this.txtPatientEmail.TabIndex = 3;
            // 
            // txtDOB
            // 
            this.txtDOB.Location = new System.Drawing.Point(12, 200);
            this.txtDOB.Name = "txtDOB";
            this.txtDOB.ReadOnly = true;
            this.txtDOB.Size = new System.Drawing.Size(120, 22);
            this.txtDOB.TabIndex = 4;
            // 
            // txtGender
            // 
            this.txtGender.Location = new System.Drawing.Point(140, 200);
            this.txtGender.Name = "txtGender";
            this.txtGender.ReadOnly = true;
            this.txtGender.Size = new System.Drawing.Size(120, 22);
            this.txtGender.TabIndex = 5;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(270, 200);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(380, 22);
            this.txtAddress.TabIndex = 6;
            // 
            // cmbService
            // 
            this.cmbService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbService.Location = new System.Drawing.Point(12, 230);
            this.cmbService.Name = "cmbService";
            this.cmbService.Size = new System.Drawing.Size(300, 24);
            this.cmbService.TabIndex = 7;
            // 
            // dgvMedicines
            // 
            this.dgvMedicines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMedicines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMedicines.Location = new System.Drawing.Point(12, 260);
            this.dgvMedicines.Name = "dgvMedicines";
            this.dgvMedicines.RowTemplate.Height = 24;
            this.dgvMedicines.Size = new System.Drawing.Size(400, 150);
            this.dgvMedicines.TabIndex = 8;
            // 
            // numPrescribeQty
            // 
            this.numPrescribeQty.Location = new System.Drawing.Point(420, 260);
            this.numPrescribeQty.Minimum = 1;
            this.numPrescribeQty.Maximum = 1000;
            this.numPrescribeQty.Name = "numPrescribeQty";
            this.numPrescribeQty.Size = new System.Drawing.Size(80, 22);
            this.numPrescribeQty.TabIndex = 9;
            // 
            // btnAddPrescription
            // 
            this.btnAddPrescription.Location = new System.Drawing.Point(420, 290);
            this.btnAddPrescription.Name = "btnAddPrescription";
            this.btnAddPrescription.Size = new System.Drawing.Size(120, 28);
            this.btnAddPrescription.TabIndex = 10;
            this.btnAddPrescription.Text = "Add Prescription";
            this.btnAddPrescription.UseVisualStyleBackColor = true;
            this.btnAddPrescription.Click += new System.EventHandler(this.btnAddPrescription_Click);
            // 
            // dgvPrescriptions
            // 
            this.dgvPrescriptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPrescriptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrescriptions.Location = new System.Drawing.Point(12, 420);
            this.dgvPrescriptions.Name = "dgvPrescriptions";
            this.dgvPrescriptions.RowTemplate.Height = 24;
            this.dgvPrescriptions.Size = new System.Drawing.Size(760, 150);
            this.dgvPrescriptions.TabIndex = 11;
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Location = new System.Drawing.Point(12, 600);
            this.txtDiagnosis.Multiline = true;
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Size = new System.Drawing.Size(360, 80);
            this.txtDiagnosis.TabIndex = 12;
            // 
            // txtTreatment
            // 
            this.txtTreatment.Location = new System.Drawing.Point(392, 600);
            this.txtTreatment.Multiline = true;
            this.txtTreatment.Name = "txtTreatment";
            this.txtTreatment.Size = new System.Drawing.Size(380, 80);
            this.txtTreatment.TabIndex = 13;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(680, 700);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 28);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // DoctorExamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 761);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtTreatment);
            this.Controls.Add(this.txtDiagnosis);
            this.Controls.Add(this.dgvPrescriptions);
            this.Controls.Add(this.btnAddPrescription);
            this.Controls.Add(this.numPrescribeQty);
            this.Controls.Add(this.dgvMedicines);
            this.Controls.Add(this.cmbService);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtGender);
            this.Controls.Add(this.txtDOB);
            this.Controls.Add(this.txtPatientEmail);
            this.Controls.Add(this.txtPatientPhone);
            this.Controls.Add(this.txtPatientName);
            this.Controls.Add(this.dgvAppointments);
            this.Name = "DoctorExamForm";
            this.Text = "Doctor Examination";
            this.Load += new System.EventHandler(this.DoctorExamForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrescribeQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrescriptions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
