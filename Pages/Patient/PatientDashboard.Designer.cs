using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Patient
{
    partial class PatientDashboard
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitle = new System.Windows.Forms.Label();
            this.statsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.cardAppointments = new System.Windows.Forms.Panel();
            this.iconAppointments = new System.Windows.Forms.Label();
            this.lblAppointments = new System.Windows.Forms.Label();
            this.valueAppointments = new System.Windows.Forms.Label();
            this.cardInvoices = new System.Windows.Forms.Panel();
            this.iconInvoices = new System.Windows.Forms.Label();
            this.lblInvoices = new System.Windows.Forms.Label();
            this.valueInvoices = new System.Windows.Forms.Label();
            this.cardRecords = new System.Windows.Forms.Panel();
            this.iconRecords = new System.Windows.Forms.Label();
            this.lblRecords = new System.Windows.Forms.Label();
            this.valueRecords = new System.Windows.Forms.Label();
            this.appointmentPanel = new System.Windows.Forms.Panel();
            this.lblUpcoming = new System.Windows.Forms.Label();
            this.dgvUpcoming = new System.Windows.Forms.DataGridView();
            this.invoicePanel = new System.Windows.Forms.Panel();
            this.lblInvoiceTitle = new System.Windows.Forms.Label();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.statsPanel.SuspendLayout();
            this.cardAppointments.SuspendLayout();
            this.cardInvoices.SuspendLayout();
            this.cardRecords.SuspendLayout();
            this.appointmentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUpcoming)).BeginInit();
            this.invoicePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(301, 54);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Xin chào, [Tên]";
            // 
            // statsPanel
            // 
            this.statsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statsPanel.BackColor = System.Drawing.Color.Transparent;
            this.statsPanel.Controls.Add(this.cardAppointments);
            this.statsPanel.Controls.Add(this.cardInvoices);
            this.statsPanel.Controls.Add(this.cardRecords);
            this.statsPanel.Location = new System.Drawing.Point(20, 90);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1163, 150);
            this.statsPanel.TabIndex = 1;
            // 
            // cardAppointments
            // 
            this.cardAppointments.BackColor = System.Drawing.Color.White;
            this.cardAppointments.Controls.Add(this.iconAppointments);
            this.cardAppointments.Controls.Add(this.lblAppointments);
            this.cardAppointments.Controls.Add(this.valueAppointments);
            this.cardAppointments.Location = new System.Drawing.Point(3, 3);
            this.cardAppointments.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.cardAppointments.Name = "cardAppointments";
            this.cardAppointments.Size = new System.Drawing.Size(280, 130);
            this.cardAppointments.TabIndex = 0;
            // 
            // iconAppointments
            // 
            this.iconAppointments.AutoSize = true;
            this.iconAppointments.Font = new System.Drawing.Font("Segoe MDL2 Assets", 32F);
            this.iconAppointments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.iconAppointments.Location = new System.Drawing.Point(20, 35);
            this.iconAppointments.Name = "iconAppointments";
            this.iconAppointments.Size = new System.Drawing.Size(77, 54);
            this.iconAppointments.TabIndex = 0;
            this.iconAppointments.Text = "";
            // 
            // lblAppointments
            // 
            this.lblAppointments.AutoSize = true;
            this.lblAppointments.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblAppointments.ForeColor = System.Drawing.Color.Gray;
            this.lblAppointments.Location = new System.Drawing.Point(100, 35);
            this.lblAppointments.Name = "lblAppointments";
            this.lblAppointments.Size = new System.Drawing.Size(144, 25);
            this.lblAppointments.TabIndex = 1;
            this.lblAppointments.Text = "Lịch hẹn sắp tới";
            // 
            // valueAppointments
            // 
            this.valueAppointments.AutoSize = true;
            this.valueAppointments.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.valueAppointments.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.valueAppointments.Location = new System.Drawing.Point(100, 60);
            this.valueAppointments.Name = "valueAppointments";
            this.valueAppointments.Size = new System.Drawing.Size(40, 46);
            this.valueAppointments.TabIndex = 2;
            this.valueAppointments.Text = "0";
            // 
            // cardInvoices
            // 
            this.cardInvoices.BackColor = System.Drawing.Color.White;
            this.cardInvoices.Controls.Add(this.iconInvoices);
            this.cardInvoices.Controls.Add(this.lblInvoices);
            this.cardInvoices.Controls.Add(this.valueInvoices);
            this.cardInvoices.Location = new System.Drawing.Point(306, 3);
            this.cardInvoices.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.cardInvoices.Name = "cardInvoices";
            this.cardInvoices.Size = new System.Drawing.Size(343, 130);
            this.cardInvoices.TabIndex = 1;
            // 
            // iconInvoices
            // 
            this.iconInvoices.AutoSize = true;
            this.iconInvoices.Font = new System.Drawing.Font("Segoe MDL2 Assets", 32F);
            this.iconInvoices.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.iconInvoices.Location = new System.Drawing.Point(22, 35);
            this.iconInvoices.Name = "iconInvoices";
            this.iconInvoices.Size = new System.Drawing.Size(77, 54);
            this.iconInvoices.TabIndex = 0;
            this.iconInvoices.Text = "";
            // 
            // lblInvoices
            // 
            this.lblInvoices.AutoSize = true;
            this.lblInvoices.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblInvoices.ForeColor = System.Drawing.Color.Gray;
            this.lblInvoices.Location = new System.Drawing.Point(111, 35);
            this.lblInvoices.Name = "lblInvoices";
            this.lblInvoices.Size = new System.Drawing.Size(227, 25);
            this.lblInvoices.TabIndex = 1;
            this.lblInvoices.Text = "Hóa đơn chưa thanh toán";
            // 
            // valueInvoices
            // 
            this.valueInvoices.AutoSize = true;
            this.valueInvoices.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.valueInvoices.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.valueInvoices.Location = new System.Drawing.Point(108, 60);
            this.valueInvoices.Name = "valueInvoices";
            this.valueInvoices.Size = new System.Drawing.Size(40, 46);
            this.valueInvoices.TabIndex = 2;
            this.valueInvoices.Text = "0";
            // 
            // cardRecords
            // 
            this.cardRecords.BackColor = System.Drawing.Color.White;
            this.cardRecords.Controls.Add(this.iconRecords);
            this.cardRecords.Controls.Add(this.lblRecords);
            this.cardRecords.Controls.Add(this.valueRecords);
            this.cardRecords.Location = new System.Drawing.Point(672, 3);
            this.cardRecords.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.cardRecords.Name = "cardRecords";
            this.cardRecords.Size = new System.Drawing.Size(280, 130);
            this.cardRecords.TabIndex = 2;
            // 
            // iconRecords
            // 
            this.iconRecords.AutoSize = true;
            this.iconRecords.Font = new System.Drawing.Font("Segoe MDL2 Assets", 32F);
            this.iconRecords.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.iconRecords.Location = new System.Drawing.Point(17, 35);
            this.iconRecords.Name = "iconRecords";
            this.iconRecords.Size = new System.Drawing.Size(77, 54);
            this.iconRecords.TabIndex = 0;
            this.iconRecords.Text = "";
            // 
            // lblRecords
            // 
            this.lblRecords.AutoSize = true;
            this.lblRecords.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblRecords.ForeColor = System.Drawing.Color.Gray;
            this.lblRecords.Location = new System.Drawing.Point(100, 35);
            this.lblRecords.Name = "lblRecords";
            this.lblRecords.Size = new System.Drawing.Size(159, 25);
            this.lblRecords.TabIndex = 1;
            this.lblRecords.Text = "Hồ sơ khám bệnh";
            // 
            // valueRecords
            // 
            this.valueRecords.AutoSize = true;
            this.valueRecords.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.valueRecords.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.valueRecords.Location = new System.Drawing.Point(100, 60);
            this.valueRecords.Name = "valueRecords";
            this.valueRecords.Size = new System.Drawing.Size(40, 46);
            this.valueRecords.TabIndex = 2;
            this.valueRecords.Text = "0";
            // 
            // appointmentPanel
            // 
            this.appointmentPanel.BackColor = System.Drawing.Color.White;
            this.appointmentPanel.Controls.Add(this.lblUpcoming);
            this.appointmentPanel.Controls.Add(this.dgvUpcoming);
            this.appointmentPanel.Location = new System.Drawing.Point(20, 260);
            this.appointmentPanel.Name = "appointmentPanel";
            this.appointmentPanel.Padding = new System.Windows.Forms.Padding(15);
            this.appointmentPanel.Size = new System.Drawing.Size(570, 393);
            this.appointmentPanel.TabIndex = 2;
            // 
            // lblUpcoming
            // 
            this.lblUpcoming.AutoSize = true;
            this.lblUpcoming.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblUpcoming.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblUpcoming.Location = new System.Drawing.Point(15, 15);
            this.lblUpcoming.Name = "lblUpcoming";
            this.lblUpcoming.Size = new System.Drawing.Size(192, 32);
            this.lblUpcoming.TabIndex = 0;
            this.lblUpcoming.Text = "Lịch hẹn sắp tới";
            // 
            // dgvUpcoming
            // 
            this.dgvUpcoming.AllowUserToAddRows = false;
            this.dgvUpcoming.AllowUserToDeleteRows = false;
            this.dgvUpcoming.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUpcoming.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUpcoming.BackgroundColor = System.Drawing.Color.White;
            this.dgvUpcoming.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUpcoming.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvUpcoming.ColumnHeadersHeight = 35;
            this.dgvUpcoming.EnableHeadersVisualStyles = false;
            this.dgvUpcoming.Location = new System.Drawing.Point(15, 50);
            this.dgvUpcoming.Name = "dgvUpcoming";
            this.dgvUpcoming.ReadOnly = true;
            this.dgvUpcoming.RowHeadersWidth = 51;
            this.dgvUpcoming.RowTemplate.Height = 30;
            this.dgvUpcoming.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUpcoming.Size = new System.Drawing.Size(540, 328);
            this.dgvUpcoming.TabIndex = 1;
            // 
            // invoicePanel
            // 
            this.invoicePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.invoicePanel.BackColor = System.Drawing.Color.White;
            this.invoicePanel.Controls.Add(this.lblInvoiceTitle);
            this.invoicePanel.Controls.Add(this.dgvInvoices);
            this.invoicePanel.Location = new System.Drawing.Point(613, 260);
            this.invoicePanel.Name = "invoicePanel";
            this.invoicePanel.Padding = new System.Windows.Forms.Padding(15);
            this.invoicePanel.Size = new System.Drawing.Size(570, 393);
            this.invoicePanel.TabIndex = 3;
            // 
            // lblInvoiceTitle
            // 
            this.lblInvoiceTitle.AutoSize = true;
            this.lblInvoiceTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblInvoiceTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblInvoiceTitle.Location = new System.Drawing.Point(15, 15);
            this.lblInvoiceTitle.Name = "lblInvoiceTitle";
            this.lblInvoiceTitle.Size = new System.Drawing.Size(305, 32);
            this.lblInvoiceTitle.TabIndex = 0;
            this.lblInvoiceTitle.Text = "Hóa đơn chưa thanh toán";
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.AllowUserToAddRows = false;
            this.dgvInvoices.AllowUserToDeleteRows = false;
            this.dgvInvoices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInvoices.BackgroundColor = System.Drawing.Color.White;
            this.dgvInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvInvoices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvInvoices.ColumnHeadersHeight = 35;
            this.dgvInvoices.EnableHeadersVisualStyles = false;
            this.dgvInvoices.Location = new System.Drawing.Point(15, 50);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.ReadOnly = true;
            this.dgvInvoices.RowHeadersWidth = 51;
            this.dgvInvoices.RowTemplate.Height = 30;
            this.dgvInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoices.Size = new System.Drawing.Size(540, 328);
            this.dgvInvoices.TabIndex = 1;
            // 
            // PatientDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.invoicePanel);
            this.Controls.Add(this.appointmentPanel);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.lblTitle);
            this.Name = "PatientDashboard";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1203, 685);
            this.statsPanel.ResumeLayout(false);
            this.cardAppointments.ResumeLayout(false);
            this.cardAppointments.PerformLayout();
            this.cardInvoices.ResumeLayout(false);
            this.cardInvoices.PerformLayout();
            this.cardRecords.ResumeLayout(false);
            this.cardRecords.PerformLayout();
            this.appointmentPanel.ResumeLayout(false);
            this.appointmentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUpcoming)).EndInit();
            this.invoicePanel.ResumeLayout(false);
            this.invoicePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel statsPanel;
        private System.Windows.Forms.Panel cardAppointments;
        private System.Windows.Forms.Label iconAppointments;
        private System.Windows.Forms.Label lblAppointments;
        private System.Windows.Forms.Label valueAppointments;
        private System.Windows.Forms.Panel cardInvoices;
        private System.Windows.Forms.Label iconInvoices;
        private System.Windows.Forms.Label lblInvoices;
        private System.Windows.Forms.Label valueInvoices;
        private System.Windows.Forms.Panel cardRecords;
        private System.Windows.Forms.Label iconRecords;
        private System.Windows.Forms.Label lblRecords;
        private System.Windows.Forms.Label valueRecords;
        private System.Windows.Forms.Panel appointmentPanel;
        private System.Windows.Forms.Label lblUpcoming;
        private System.Windows.Forms.DataGridView dgvUpcoming;
        private System.Windows.Forms.Panel invoicePanel;
        private System.Windows.Forms.Label lblInvoiceTitle;
        private System.Windows.Forms.DataGridView dgvInvoices;
    }
}