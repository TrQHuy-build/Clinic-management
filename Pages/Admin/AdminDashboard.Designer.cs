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
        private Button btnRefresh;
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
            this.btnRefresh = new System.Windows.Forms.Button();
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
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(469, 54);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard - Tổng quan";
            // 
            // statsPanel
            // 
            this.statsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statsPanel.Location = new System.Drawing.Point(20, 80);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(1500, 130);  // ✅ Giảm từ 1900 → 1500 (fit 4 cards: 240+240+240+300 + margins)
            this.statsPanel.TabIndex = 1;
            // 
            // recentPanel
            // 
            this.recentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));  // ✅ Thêm Anchor để fit
            this.recentPanel.BackColor = System.Drawing.Color.White;
            this.recentPanel.Controls.Add(this.lblRecent);
            this.recentPanel.Controls.Add(this.btnRefresh);
            this.recentPanel.Controls.Add(this.dgvAppointments);
            this.recentPanel.Location = new System.Drawing.Point(20, 220);  // ✅ Điều chỉnh vị trí
            this.recentPanel.Name = "recentPanel";
            this.recentPanel.Padding = new System.Windows.Forms.Padding(20);
            this.recentPanel.Size = new System.Drawing.Size(1500, 620);  // ✅ Giảm từ 1900 → 1500 (đồng bộ)
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
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(1180, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 35);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
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
            this.dgvAppointments.BackgroundColor = System.Drawing.Color.White;
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
            this.dgvAppointments.Size = new System.Drawing.Size(1440, 540);  // ✅ Giảm width từ 1880 → 1440
            this.dgvAppointments.TabIndex = 2;
            this.dgvAppointments.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAppointments_CellContentClick);
            // 
            // AdminDashboard
            // 
            this.AutoScroll = false;  // ✅ Tắt cuộn, fit hoàn toàn
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.recentPanel);
            this.Name = "AdminDashboard";
            this.Padding = new System.Windows.Forms.Padding(20, 20, 20, 0);  // ✅ Giảm padding bottom
            this.Size = new System.Drawing.Size(1540, 860);  // ✅ Giảm width từ 1940 → 1540
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
            // Xác định kích thước card dựa trên loại
            int cardWidth = (iconKey == "Money") ? 300 : 240;  // ✅ Giảm: Money 380→300, các card khác 260→240
            
            Panel card = new Panel
            {
                Size = new Size(cardWidth, 110),
                BackColor = Color.White,
                Margin = new Padding(5)  // ✅ Giảm từ 10 → 5
            };

            // Border nhẹ
            card.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(225, 225, 225), 1),
                    0, 0, card.Width - 1, card.Height - 1);
            };

            // === ✅ ICON ĐÚNG TỪNG CDL2 ASSETS ===
            string iconChar;
            if (iconKey == "Patient")
                iconChar = "\uE77B";      // User icon
            else if (iconKey == "Doctor")
                iconChar = "\uE7F8";      // Medical/Doctor icon
            else if (iconKey == "Calendar")
                iconChar = "\uE787";      // Calendar icon
            else if (iconKey == "Money")
                iconChar = "\uE7C8";      // Money/Wallet icon
            else
                iconChar = "\uE946";      // Default

            Label lblIcon = new Label
            {
                Text = iconChar,
                Font = new Font("Segoe MDL2 Assets", 28F),
                ForeColor = ColorTranslator.FromHtml(color),
                AutoSize = true,
                Location = new Point(12, 16)  // ✅ Điều chỉnh cho card nhỏ hơn
            };

            Label lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9F),  // ✅ Giảm từ 10F → 9F
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(60, 18)
            };

            Label lblValue = new Label
            {
                Name = "value_" + label,
                Text = value,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),  // ✅ Giảm từ 18F → 16F
                ForeColor = ColorTranslator.FromHtml(color),
                AutoSize = true,
                Location = new Point(60, 48)
            };

            card.Controls.Add(lblIcon);
            card.Controls.Add(lblLabel);
            card.Controls.Add(lblValue);

            return card;
        }

        #endregion
    }
}