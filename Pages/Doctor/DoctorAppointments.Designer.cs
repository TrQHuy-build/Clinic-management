using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    partial class DoctorAppointments
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
            this.dgvAppointments = new System.Windows.Forms.DataGridView();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnToday = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAppointments
            // 
            this.dgvAppointments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAppointments.BackgroundColor = System.Drawing.Color.White;
            this.dgvAppointments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAppointments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAppointments.ColumnHeadersHeight = 40;
            this.dgvAppointments.EnableHeadersVisualStyles = false;
            this.dgvAppointments.Location = new System.Drawing.Point(15, 130);
            this.dgvAppointments.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvAppointments.Name = "dgvAppointments";
            this.dgvAppointments.ReadOnly = true;
            this.dgvAppointments.RowHeadersWidth = 51;
            this.dgvAppointments.RowTemplate.Height = 35;
            this.dgvAppointments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppointments.Size = new System.Drawing.Size(870, 422);
            this.dgvAppointments.TabIndex = 2;
            this.dgvAppointments.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvAppointments_CellClick);
            // 
            // dtpDate
            // 
            this.dtpDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDate.Location = new System.Drawing.Point(104, 14);
            this.dtpDate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(247, 23);
            this.dtpDate.TabIndex = 1;
            this.dtpDate.Value = new System.DateTime(2025, 11, 15, 0, 0, 0, 0);
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 16);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(237, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Lịch hẹn của tôi";
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.White;
            this.panel.Controls.Add(this.lblDate);
            this.panel.Controls.Add(this.dtpDate);
            this.panel.Controls.Add(this.btnToday);
            this.panel.Location = new System.Drawing.Point(15, 65);
            this.panel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(870, 49);
            this.panel.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.Location = new System.Drawing.Point(15, 16);
            this.lblDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(80, 17);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Chọn ngày:";
            // 
            // btnToday
            // 
            this.btnToday.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnToday.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToday.FlatAppearance.BorderSize = 0;
            this.btnToday.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToday.ForeColor = System.Drawing.Color.White;
            this.btnToday.Location = new System.Drawing.Point(424, 8);
            this.btnToday.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(94, 33);
            this.btnToday.TabIndex = 2;
            this.btnToday.Text = "Hôm nay";
            this.btnToday.UseVisualStyleBackColor = false;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            // 
            // DoctorAppointments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.dgvAppointments);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DoctorAppointments";
            this.Padding = new System.Windows.Forms.Padding(15, 16, 15, 16);
            this.Size = new System.Drawing.Size(900, 569);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAppointments;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private Label lblTitle;
        private Panel panel;
        private Label lblDate;
        private Button btnToday;
    }
}