using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    partial class DoctorExamine
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelPatient = new System.Windows.Forms.Panel();
            this.lblPatientTitle = new System.Windows.Forms.Label();
            this.cboPatient = new System.Windows.Forms.ComboBox();
            this.panelDiagnosis = new System.Windows.Forms.Panel();
            this.lblDiagnosisTitle = new System.Windows.Forms.Label();
            this.lblTreat = new System.Windows.Forms.Label();
            this.txtTreatment = new System.Windows.Forms.TextBox();
            this.lblDiag = new System.Windows.Forms.Label();
            this.txtDiagnosis = new System.Windows.Forms.TextBox();
            this.panelServices = new System.Windows.Forms.Panel();
            this.lblServicesTitle = new System.Windows.Forms.Label();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.panelMedicines = new System.Windows.Forms.Panel();
            this.lblMedicinesTitle = new System.Windows.Forms.Label();
            this.btnAddMedicine = new System.Windows.Forms.Button();
            this.dgvMedicines = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelPatient.SuspendLayout();
            this.panelDiagnosis.SuspendLayout();
            this.panelServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.panelMedicines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicines)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(219, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Khám bệnh";
            // 
            // panelPatient
            // 
            this.panelPatient.BackColor = System.Drawing.Color.White;
            this.panelPatient.Controls.Add(this.lblPatientTitle);
            this.panelPatient.Controls.Add(this.cboPatient);
            this.panelPatient.Location = new System.Drawing.Point(20, 90);
            this.panelPatient.Name = "panelPatient";
            this.panelPatient.Size = new System.Drawing.Size(700, 100);
            this.panelPatient.TabIndex = 1;
            // 
            // lblPatientTitle
            // 
            this.lblPatientTitle.AutoSize = true;
            this.lblPatientTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblPatientTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblPatientTitle.Location = new System.Drawing.Point(15, 9);
            this.lblPatientTitle.Name = "lblPatientTitle";
            this.lblPatientTitle.Size = new System.Drawing.Size(201, 32);
            this.lblPatientTitle.TabIndex = 1;
            this.lblPatientTitle.Text = "Chọn bệnh nhân";
            // 
            // cboPatient
            // 
            this.cboPatient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPatient.FormattingEnabled = true;
            this.cboPatient.Location = new System.Drawing.Point(22, 50);
            this.cboPatient.Name = "cboPatient";
            this.cboPatient.Size = new System.Drawing.Size(400, 24);
            this.cboPatient.TabIndex = 0;
            // 
            // panelDiagnosis
            // 
            this.panelDiagnosis.BackColor = System.Drawing.Color.White;
            this.panelDiagnosis.Controls.Add(this.lblDiagnosisTitle);
            this.panelDiagnosis.Controls.Add(this.lblTreat);
            this.panelDiagnosis.Controls.Add(this.txtTreatment);
            this.panelDiagnosis.Controls.Add(this.lblDiag);
            this.panelDiagnosis.Controls.Add(this.txtDiagnosis);
            this.panelDiagnosis.Location = new System.Drawing.Point(20, 210);
            this.panelDiagnosis.Name = "panelDiagnosis";
            this.panelDiagnosis.Size = new System.Drawing.Size(700, 200);
            this.panelDiagnosis.TabIndex = 2;
            // 
            // lblDiagnosisTitle
            // 
            this.lblDiagnosisTitle.AutoSize = true;
            this.lblDiagnosisTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblDiagnosisTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblDiagnosisTitle.Location = new System.Drawing.Point(15, 11);
            this.lblDiagnosisTitle.Name = "lblDiagnosisTitle";
            this.lblDiagnosisTitle.Size = new System.Drawing.Size(236, 32);
            this.lblDiagnosisTitle.TabIndex = 4;
            this.lblDiagnosisTitle.Text = "Chẩn đoán & Điều trị";
            // 
            // lblTreat
            // 
            this.lblTreat.AutoSize = true;
            this.lblTreat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTreat.Location = new System.Drawing.Point(23, 142);
            this.lblTreat.Name = "lblTreat";
            this.lblTreat.Size = new System.Drawing.Size(78, 25);
            this.lblTreat.TabIndex = 2;
            this.lblTreat.Text = "Điều trị:";
            // 
            // txtTreatment
            // 
            this.txtTreatment.Location = new System.Drawing.Point(140, 117);
            this.txtTreatment.Multiline = true;
            this.txtTreatment.Name = "txtTreatment";
            this.txtTreatment.Size = new System.Drawing.Size(530, 60);
            this.txtTreatment.TabIndex = 3;
            // 
            // lblDiag
            // 
            this.lblDiag.AutoSize = true;
            this.lblDiag.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiag.Location = new System.Drawing.Point(19, 68);
            this.lblDiag.Name = "lblDiag";
            this.lblDiag.Size = new System.Drawing.Size(115, 25);
            this.lblDiag.TabIndex = 0;
            this.lblDiag.Text = "Chẩn đoán:";
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Location = new System.Drawing.Point(140, 47);
            this.txtDiagnosis.Multiline = true;
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Size = new System.Drawing.Size(530, 60);
            this.txtDiagnosis.TabIndex = 1;
            // 
            // panelServices
            // 
            this.panelServices.BackColor = System.Drawing.Color.White;
            this.panelServices.Controls.Add(this.lblServicesTitle);
            this.panelServices.Controls.Add(this.dgvServices);
            this.panelServices.Location = new System.Drawing.Point(20, 430);
            this.panelServices.Name = "panelServices";
            this.panelServices.Size = new System.Drawing.Size(700, 250);
            this.panelServices.TabIndex = 3;
            // 
            // lblServicesTitle
            // 
            this.lblServicesTitle.AutoSize = true;
            this.lblServicesTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblServicesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblServicesTitle.Location = new System.Drawing.Point(20, 10);
            this.lblServicesTitle.Name = "lblServicesTitle";
            this.lblServicesTitle.Size = new System.Drawing.Size(201, 32);
            this.lblServicesTitle.TabIndex = 1;
            this.lblServicesTitle.Text = "Dịch vụ sử dụng";
            // 
            // dgvServices
            // 
            this.dgvServices.AllowUserToAddRows = false;
            this.dgvServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServices.BackgroundColor = System.Drawing.Color.White;
            this.dgvServices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new System.Drawing.Point(20, 50);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.RowHeadersWidth = 51;
            this.dgvServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServices.Size = new System.Drawing.Size(650, 180);
            this.dgvServices.TabIndex = 0;
            // 
            // panelMedicines
            // 
            this.panelMedicines.BackColor = System.Drawing.Color.White;
            this.panelMedicines.Controls.Add(this.lblMedicinesTitle);
            this.panelMedicines.Controls.Add(this.btnAddMedicine);
            this.panelMedicines.Controls.Add(this.dgvMedicines);
            this.panelMedicines.Location = new System.Drawing.Point(20, 700);
            this.panelMedicines.Name = "panelMedicines";
            this.panelMedicines.Size = new System.Drawing.Size(700, 300);
            this.panelMedicines.TabIndex = 4;
            // 
            // lblMedicinesTitle
            // 
            this.lblMedicinesTitle.AutoSize = true;
            this.lblMedicinesTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblMedicinesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblMedicinesTitle.Location = new System.Drawing.Point(20, 15);
            this.lblMedicinesTitle.Name = "lblMedicinesTitle";
            this.lblMedicinesTitle.Size = new System.Drawing.Size(167, 32);
            this.lblMedicinesTitle.TabIndex = 2;
            this.lblMedicinesTitle.Text = "Kê đơn thuốc";
            // 
            // btnAddMedicine
            // 
            this.btnAddMedicine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAddMedicine.FlatAppearance.BorderSize = 0;
            this.btnAddMedicine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddMedicine.ForeColor = System.Drawing.Color.White;
            this.btnAddMedicine.Location = new System.Drawing.Point(23, 246);
            this.btnAddMedicine.Name = "btnAddMedicine";
            this.btnAddMedicine.Size = new System.Drawing.Size(120, 35);
            this.btnAddMedicine.TabIndex = 1;
            this.btnAddMedicine.Text = "+ Thêm thuốc";
            this.btnAddMedicine.UseVisualStyleBackColor = false;
            // 
            // dgvMedicines
            // 
            this.dgvMedicines.AllowUserToAddRows = false;
            this.dgvMedicines.BackgroundColor = System.Drawing.Color.White;
            this.dgvMedicines.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMedicines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMedicines.Location = new System.Drawing.Point(20, 57);
            this.dgvMedicines.Name = "dgvMedicines";
            this.dgvMedicines.RowHeadersWidth = 51;
            this.dgvMedicines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMedicines.Size = new System.Drawing.Size(650, 171);
            this.dgvMedicines.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(20, 1020);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(300, 50);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Lưu hồ sơ & Tạo hóa đơn";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // DoctorExamine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panelMedicines);
            this.Controls.Add(this.panelServices);
            this.Controls.Add(this.panelDiagnosis);
            this.Controls.Add(this.panelPatient);
            this.Controls.Add(this.lblTitle);
            this.Name = "DoctorExamine";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(765, 1090);
            this.panelPatient.ResumeLayout(false);
            this.panelPatient.PerformLayout();
            this.panelDiagnosis.ResumeLayout(false);
            this.panelDiagnosis.PerformLayout();
            this.panelServices.ResumeLayout(false);
            this.panelServices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.panelMedicines.ResumeLayout(false);
            this.panelMedicines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedicines)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelPatient;
        private System.Windows.Forms.ComboBox cboPatient;
        private System.Windows.Forms.Label lblPatientTitle;
        private System.Windows.Forms.Panel panelDiagnosis;
        private System.Windows.Forms.Label lblDiag;
        private System.Windows.Forms.TextBox txtDiagnosis;
        private System.Windows.Forms.Label lblTreat;
        private System.Windows.Forms.TextBox txtTreatment;
        private System.Windows.Forms.Label lblDiagnosisTitle;
        private System.Windows.Forms.Panel panelServices;
        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.Label lblServicesTitle;
        private System.Windows.Forms.Panel panelMedicines;
        private System.Windows.Forms.DataGridView dgvMedicines;
        private System.Windows.Forms.Button btnAddMedicine;
        private System.Windows.Forms.Label lblMedicinesTitle;
        private System.Windows.Forms.Button btnSave;
    }
}