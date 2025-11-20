using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    partial class AdminDashboard
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Declare controls
        private Label lblTitle;
        private FlowLayoutPanel statsPanel;
        private Panel cardPatients;
        private Panel cardDoctors;
        private Panel cardAppointments;
        private Panel cardRevenue;
        private Panel recentPanel;
        private Label lblRecent;
        private DataGridView dgvAppointments;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitle = new System.Windows.Forms.Label();
            this.statsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.recentPanel = new System.Windows.Forms.Panel();
            this.lblRecent = new System.Windows.Forms.Label();
            this.dgvAppointments = new System.Windows.Forms.DataGridView();
            this.recentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(469, 54);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard - Tổng quan";
            // 
            // statsPanel
            // 
            this.statsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statsPanel.Location = new System.Drawing.Point(0, 57);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1318, 167);
            this.statsPanel.TabIndex = 1;
            // 
            // recentPanel
            // 
            this.recentPanel.BackColor = System.Drawing.Color.White;
            this.recentPanel.Controls.Add(this.lblRecent);
            this.recentPanel.Controls.Add(this.dgvAppointments);
            this.recentPanel.Location = new System.Drawing.Point(0, 225);
            this.recentPanel.Name = "recentPanel";
            this.recentPanel.Padding = new System.Windows.Forms.Padding(20);
            this.recentPanel.Size = new System.Drawing.Size(1318, 602);
            this.recentPanel.TabIndex = 2;
            // 
            // lblRecent
            // 
            this.lblRecent.AutoSize = true;
            this.lblRecent.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblRecent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblRecent.Location = new System.Drawing.Point(20, 12);
            this.lblRecent.Name = "lblRecent";
            this.lblRecent.Size = new System.Drawing.Size(231, 37);
            this.lblRecent.TabIndex = 0;
            this.lblRecent.Text = "Lịch hẹn gần đây";
            // 
            // dgvAppointments
            // 
            this.dgvAppointments.AllowUserToAddRows = false;
            this.dgvAppointments.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dgvAppointments.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAppointments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAppointments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAppointments.BackgroundColor = System.Drawing.Color.SeaShell;
            this.dgvAppointments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvAppointments.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvAppointments.ColumnHeadersHeight = 40;
            this.dgvAppointments.EnableHeadersVisualStyles = false;
            this.dgvAppointments.Location = new System.Drawing.Point(9, 60);
            this.dgvAppointments.Name = "dgvAppointments";
            this.dgvAppointments.ReadOnly = true;
            this.dgvAppointments.RowHeadersWidth = 51;
            this.dgvAppointments.RowTemplate.Height = 35;
            this.dgvAppointments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAppointments.Size = new System.Drawing.Size(1299, 526);
            this.dgvAppointments.TabIndex = 1;
            this.dgvAppointments.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAppointments_CellContentClick);
            // 
            // AdminDashboard
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.recentPanel);
            this.Name = "AdminDashboard";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1318, 834);
            this.recentPanel.ResumeLayout(false);
            this.recentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAppointments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

            this.statsPanel.FlowDirection = FlowDirection.LeftToRight;
            this.statsPanel.WrapContents = true;
            this.statsPanel.AutoSize = false;

        }

        /// <summary>
        /// Create a stat card panel
        /// </summary>
        private Panel CreateStatCard(string iconKey, string label, string value, string color)
        {
            Panel card = new Panel
            {
                Size = new Size(260, 130),
                BackColor = Color.White,
                Margin = new Padding(15)
            };

            // Border nhẹ
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(225, 225, 225), 1),
                    0, 0, card.Width - 1, card.Height - 1);
            };

            // === DÙNG IF-ELSE THAY SWITCH EXPRESSION ===
            string iconChar;
            if (iconKey == "Patient")
                iconChar = "\uE77B";      // User
            else if (iconKey == "Doctor")
                iconChar = "\uE8C3";      // Doctor
            else if (iconKey == "Calendar")
                iconChar = "\uE787";      // Calendar
            else if (iconKey == "Money")
                iconChar = "\uE7C8";      // Money
            else
                iconChar = "\uE946";      // Default

            Label lblIcon = new Label
            {
                Text = iconChar,
                Font = new Font("Segoe MDL2 Assets", 36F),
                ForeColor = ColorTranslator.FromHtml(color),
                AutoSize = true,
                Location = new Point(20, 25)
            };

            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(90, 30)
            };

            Label lblValue = new Label
            {
                Name = "value_" + label,
                Text = value,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml(color),
                AutoSize = true,
                Location = new Point(90, 60)
            };

            card.Controls.Add(lblIcon);
            card.Controls.Add(lblLabel);
            card.Controls.Add(lblValue);

            return card;
        }

        #endregion
    }
}