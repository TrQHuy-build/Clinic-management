using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Patient
{
    partial class PatientBookAppointment
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Declare controls
        private Label lblTitle;
        private Panel formPanel;
        private Label lblPatientName;
        private TextBox txtPatientName;
        private Label lblPhone;
        private TextBox txtPhone;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblService;
        private ComboBox cboService;
        private Label lblDate;
        private DateTimePicker dtpDate;
        private Label lblTime;
        private ComboBox cboHour;
        private Label lblColon;
        private ComboBox cboMinute;
        private Label lblNotes;
        private TextBox txtNotes;
        private Button btnBook;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.formPanel = new System.Windows.Forms.Panel();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.txtPatientName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblService = new System.Windows.Forms.Label();
            this.cboService = new System.Windows.Forms.ComboBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblTime = new System.Windows.Forms.Label();
            this.cboHour = new System.Windows.Forms.ComboBox();
            this.lblColon = new System.Windows.Forms.Label();
            this.cboMinute = new System.Windows.Forms.ComboBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnBook = new System.Windows.Forms.Button();
            this.formPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(14, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(229, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Đặt lịch hẹn";
            // 
            // formPanel
            // 
            this.formPanel.BackColor = System.Drawing.Color.White;
            this.formPanel.Controls.Add(this.lblPatientName);
            this.formPanel.Controls.Add(this.txtPatientName);
            this.formPanel.Controls.Add(this.lblPhone);
            this.formPanel.Controls.Add(this.txtPhone);
            this.formPanel.Controls.Add(this.lblEmail);
            this.formPanel.Controls.Add(this.txtEmail);
            this.formPanel.Controls.Add(this.lblService);
            this.formPanel.Controls.Add(this.cboService);
            this.formPanel.Controls.Add(this.lblDate);
            this.formPanel.Controls.Add(this.dtpDate);
            this.formPanel.Controls.Add(this.lblTime);
            this.formPanel.Controls.Add(this.cboHour);
            this.formPanel.Controls.Add(this.lblColon);
            this.formPanel.Controls.Add(this.cboMinute);
            this.formPanel.Controls.Add(this.lblNotes);
            this.formPanel.Controls.Add(this.txtNotes);
            this.formPanel.Controls.Add(this.btnBook);
            this.formPanel.Location = new System.Drawing.Point(23, 65);
            this.formPanel.Name = "formPanel";
            this.formPanel.Padding = new System.Windows.Forms.Padding(30);
            this.formPanel.Size = new System.Drawing.Size(700, 550);
            this.formPanel.TabIndex = 1;
            // 
            // lblPatientName
            // 
            this.lblPatientName.AutoSize = true;
            this.lblPatientName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPatientName.Location = new System.Drawing.Point(30, 33);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(88, 23);
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "Họ và tên:";
            // 
            // txtPatientName
            // 
            this.txtPatientName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPatientName.Location = new System.Drawing.Point(180, 30);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Size = new System.Drawing.Size(400, 32);
            this.txtPatientName.TabIndex = 1;
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPhone.Location = new System.Drawing.Point(30, 78);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(115, 23);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "Số điện thoại:";
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPhone.Location = new System.Drawing.Point(180, 75);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(400, 32);
            this.txtPhone.TabIndex = 3;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEmail.Location = new System.Drawing.Point(30, 123);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(55, 23);
            this.lblEmail.TabIndex = 4;
            this.lblEmail.Text = "Email:";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtEmail.Location = new System.Drawing.Point(180, 120);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(400, 32);
            this.txtEmail.TabIndex = 5;
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblService.Location = new System.Drawing.Point(30, 168);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(71, 23);
            this.lblService.TabIndex = 6;
            this.lblService.Text = "Dịch vụ:";
            // 
            // cboService
            // 
            this.cboService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboService.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboService.Location = new System.Drawing.Point(180, 165);
            this.cboService.Name = "cboService";
            this.cboService.Size = new System.Drawing.Size(400, 33);
            this.cboService.TabIndex = 7;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDate.Location = new System.Drawing.Point(30, 213);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(88, 23);
            this.lblDate.TabIndex = 8;
            this.lblDate.Text = "Ngày hẹn:";
            // 
            // dtpDate
            // 
            this.dtpDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(180, 210);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(200, 32);
            this.dtpDate.TabIndex = 9;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTime.Location = new System.Drawing.Point(30, 258);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(74, 23);
            this.lblTime.TabIndex = 10;
            this.lblTime.Text = "Giờ hẹn:";
            // 
            // cboHour
            // 
            this.cboHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHour.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboHour.Items.AddRange(new object[] {
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18"});
            this.cboHour.Location = new System.Drawing.Point(180, 255);
            this.cboHour.Name = "cboHour";
            this.cboHour.Size = new System.Drawing.Size(80, 33);
            this.cboHour.TabIndex = 11;
            // 
            // lblColon
            // 
            this.lblColon.AutoSize = true;
            this.lblColon.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblColon.Location = new System.Drawing.Point(265, 258);
            this.lblColon.Name = "lblColon";
            this.lblColon.Size = new System.Drawing.Size(21, 32);
            this.lblColon.TabIndex = 12;
            this.lblColon.Text = ":";
            // 
            // cboMinute
            // 
            this.cboMinute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMinute.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboMinute.Items.AddRange(new object[] {
            "00",
            "15",
            "30",
            "45"});
            this.cboMinute.Location = new System.Drawing.Point(289, 255);
            this.cboMinute.Name = "cboMinute";
            this.cboMinute.Size = new System.Drawing.Size(85, 33);
            this.cboMinute.TabIndex = 13;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNotes.Location = new System.Drawing.Point(30, 303);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(73, 23);
            this.lblNotes.TabIndex = 14;
            this.lblNotes.Text = "Ghi chú:";
            // 
            // txtNotes
            // 
            this.txtNotes.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNotes.Location = new System.Drawing.Point(180, 300);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(400, 80);
            this.txtNotes.TabIndex = 15;
            // 
            // btnBook
            // 
            this.btnBook.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnBook.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBook.FlatAppearance.BorderSize = 0;
            this.btnBook.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBook.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnBook.ForeColor = System.Drawing.Color.White;
            this.btnBook.Location = new System.Drawing.Point(180, 400);
            this.btnBook.Name = "btnBook";
            this.btnBook.Size = new System.Drawing.Size(200, 45);
            this.btnBook.TabIndex = 16;
            this.btnBook.Text = "📅 Đặt lịch hẹn";
            this.btnBook.UseVisualStyleBackColor = false;
            this.btnBook.Click += new System.EventHandler(this.BtnBook_Click);
            // 
            // PatientBookAppointment
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.formPanel);
            this.Name = "PatientBookAppointment";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1565, 834);
            this.formPanel.ResumeLayout(false);
            this.formPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}