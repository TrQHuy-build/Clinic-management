using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    partial class DoctorDashboard
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.statsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.card1 = new System.Windows.Forms.Panel();
            this.icon1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.valueToday = new System.Windows.Forms.Label();
            this.card2 = new System.Windows.Forms.Panel();
            this.icon2 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.valueWeek = new System.Windows.Forms.Label();
            this.card3 = new System.Windows.Forms.Panel();
            this.icon3 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.valueCompleted = new System.Windows.Forms.Label();
            this.appointmentPanel = new System.Windows.Forms.Panel();
            this.lblAppointments = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).BeginInit();
            this.statsPanel.SuspendLayout();
            this.card1.SuspendLayout();
            this.card2.SuspendLayout();
            this.card3.SuspendLayout();
            this.appointmentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAppointments
            // 
            this.dgvAppointments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAppointments.BackgroundColor = System.Drawing.Color.White;
            this.dgvAppointments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            this.dgvAppointments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAppointments.ColumnHeadersHeight = 40;
            this.dgvAppointments.EnableHeadersVisualStyles = false;
            this.dgvAppointments.Location = new System.Drawing.Point(20, 60);
            this.dgvAppointments.Name = "dgvAppointments";
            this.dgvAppointments.ReadOnly = true;
            this.dgvAppointments.RowHeadersWidth = 51;
            this.dgvAppointments.RowTemplate.Height = 35;
            this.dgvAppointments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppointments.Size = new System.Drawing.Size(1120, 350);
            this.dgvAppointments.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 54);
            this.lblTitle.TabIndex = 0;
            // 
            // statsPanel
            // 
            this.statsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statsPanel.BackColor = System.Drawing.Color.Transparent;
            this.statsPanel.Controls.Add(this.card1);
            this.statsPanel.Controls.Add(this.card2);
            this.statsPanel.Controls.Add(this.card3);
            this.statsPanel.Location = new System.Drawing.Point(20, 80);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1160, 150);
            this.statsPanel.TabIndex = 1;
            // 
            // card1
            // 
            this.card1.BackColor = System.Drawing.Color.White;
            this.card1.Controls.Add(this.icon1);
            this.card1.Controls.Add(this.label1);
            this.card1.Controls.Add(this.valueToday);
            this.card1.Location = new System.Drawing.Point(0, 0);
            this.card1.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.card1.Name = "card1";
            this.card1.Size = new System.Drawing.Size(280, 130);
            this.card1.TabIndex = 0;
            // 
            // icon1
            // 
            this.icon1.AutoSize = true;
            this.icon1.Font = new System.Drawing.Font("Segoe MDL2 Assets", 32F);
            this.icon1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.icon1.Location = new System.Drawing.Point(11, 35);
            this.icon1.Name = "icon1";
            this.icon1.Size = new System.Drawing.Size(77, 54);
            this.icon1.TabIndex = 0;
            this.icon1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(90, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Lịch hẹn hôm nay";
            // 
            // valueToday
            // 
            this.valueToday.AutoSize = true;
            this.valueToday.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.valueToday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.valueToday.Location = new System.Drawing.Point(90, 60);
            this.valueToday.Name = "valueToday";
            this.valueToday.Size = new System.Drawing.Size(40, 46);
            this.valueToday.TabIndex = 2;
            this.valueToday.Text = "0";
            // 
            // card2
            // 
            this.card2.BackColor = System.Drawing.Color.White;
            this.card2.Controls.Add(this.icon2);
            this.card2.Controls.Add(this.label2);
            this.card2.Controls.Add(this.valueWeek);
            this.card2.Location = new System.Drawing.Point(300, 0);
            this.card2.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.card2.Name = "card2";
            this.card2.Size = new System.Drawing.Size(280, 130);
            this.card2.TabIndex = 1;
            // 
            // icon2
            // 
            this.icon2.AutoSize = true;
            this.icon2.Font = new System.Drawing.Font("Segoe MDL2 Assets", 32F);
            this.icon2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.icon2.Location = new System.Drawing.Point(7, 35);
            this.icon2.Name = "icon2";
            this.icon2.Size = new System.Drawing.Size(77, 54);
            this.icon2.TabIndex = 0;
            this.icon2.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(90, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Bệnh nhân tuần này";
            // 
            // valueWeek
            // 
            this.valueWeek.AutoSize = true;
            this.valueWeek.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.valueWeek.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.valueWeek.Location = new System.Drawing.Point(90, 60);
            this.valueWeek.Name = "valueWeek";
            this.valueWeek.Size = new System.Drawing.Size(40, 46);
            this.valueWeek.TabIndex = 2;
            this.valueWeek.Text = "0";
            // 
            // card3
            // 
            this.card3.BackColor = System.Drawing.Color.White;
            this.card3.Controls.Add(this.icon3);
            this.card3.Controls.Add(this.label3);
            this.card3.Controls.Add(this.valueCompleted);
            this.card3.Location = new System.Drawing.Point(600, 0);
            this.card3.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.card3.Name = "card3";
            this.card3.Size = new System.Drawing.Size(280, 130);
            this.card3.TabIndex = 2;
            // 
            // icon3
            // 
            this.icon3.AutoSize = true;
            this.icon3.Font = new System.Drawing.Font("Segoe MDL2 Assets", 32F);
            this.icon3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.icon3.Location = new System.Drawing.Point(26, 35);
            this.icon3.Name = "icon3";
            this.icon3.Size = new System.Drawing.Size(58, 54);
            this.icon3.TabIndex = 0;
            this.icon3.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(90, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(164, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "Đã khám hôm nay";
            // 
            // valueCompleted
            // 
            this.valueCompleted.AutoSize = true;
            this.valueCompleted.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.valueCompleted.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.valueCompleted.Location = new System.Drawing.Point(90, 60);
            this.valueCompleted.Name = "valueCompleted";
            this.valueCompleted.Size = new System.Drawing.Size(40, 46);
            this.valueCompleted.TabIndex = 2;
            this.valueCompleted.Text = "0";
            // 
            // appointmentPanel
            // 
            this.appointmentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appointmentPanel.BackColor = System.Drawing.Color.White;
            this.appointmentPanel.Controls.Add(this.lblAppointments);
            this.appointmentPanel.Controls.Add(this.dgvAppointments);
            this.appointmentPanel.Location = new System.Drawing.Point(20, 250);
            this.appointmentPanel.Name = "appointmentPanel";
            this.appointmentPanel.Padding = new System.Windows.Forms.Padding(20);
            this.appointmentPanel.Size = new System.Drawing.Size(1160, 430);
            this.appointmentPanel.TabIndex = 2;
            // 
            // lblAppointments
            // 
            this.lblAppointments.AutoSize = true;
            this.lblAppointments.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppointments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblAppointments.Location = new System.Drawing.Point(20, 20);
            this.lblAppointments.Name = "lblAppointments";
            this.lblAppointments.Size = new System.Drawing.Size(240, 37);
            this.lblAppointments.TabIndex = 0;
            this.lblAppointments.Text = "Lịch hẹn hôm nay";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.label4.Location = new System.Drawing.Point(23, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(469, 54);
            this.label4.TabIndex = 2;
            this.label4.Text = "Dashboard - Tổng quan";
            // 
            // DoctorDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.appointmentPanel);
            this.Name = "DoctorDashboard";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1200, 700);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            this.statsPanel.ResumeLayout(false);
            this.card1.ResumeLayout(false);
            this.card1.PerformLayout();
            this.card2.ResumeLayout(false);
            this.card2.PerformLayout();
            this.card3.ResumeLayout(false);
            this.card3.PerformLayout();
            this.appointmentPanel.ResumeLayout(false);
            this.appointmentPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgvAppointments;
        private System.Windows.Forms.Label valueToday;
        private System.Windows.Forms.Label valueWeek;
        private System.Windows.Forms.Label valueCompleted;
        private FlowLayoutPanel statsPanel;
        private Panel card1;
        private Label icon1;
        private Label label1;
        private Panel card2;
        private Label icon2;
        private Panel card3;
        private Label icon3;
        private Label label3;
        private Label label2;
        private Panel appointmentPanel;
        private Label lblAppointments;
        private Label label4;
    }
}