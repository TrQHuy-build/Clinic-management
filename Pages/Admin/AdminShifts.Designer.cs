using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Admin
{
    partial class AdminShifts
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
            this.components = new System.ComponentModel.Container();
            this.dgvShifts = new System.Windows.Forms.DataGridView();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShifts)).BeginInit();
            this.SuspendLayout();

            // Title
            System.Windows.Forms.Label lblTitle = new System.Windows.Forms.Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            lblTitle.Location = new System.Drawing.Point(20, 20);
            lblTitle.Text = "Quản lý Lịch trực";

            // Filter Panel
            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            panel.BackColor = System.Drawing.Color.White;
            panel.Location = new System.Drawing.Point(20, 80);
            panel.Size = new System.Drawing.Size(1160, 60);

            // dtpDate
            this.dtpDate.Location = new System.Drawing.Point(20, 17);
            this.dtpDate.Size = new System.Drawing.Size(200, 23);
            this.dtpDate.Value = DateTime.Today;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);

            // btnAdd
            System.Windows.Forms.Button btnAdd = new System.Windows.Forms.Button();
            btnAdd.BackColor = System.Drawing.ColorTranslator.FromHtml("#28A745");
            btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnAdd.ForeColor = System.Drawing.Color.White;
            btnAdd.Location = new System.Drawing.Point(1030, 15);
            btnAdd.Size = new System.Drawing.Size(130, 35);
            btnAdd.Text = "+ Thêm ca trực";
            btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);

            panel.Controls.Add(this.dtpDate);
            panel.Controls.Add(btnAdd);

            // dgvShifts
            this.dgvShifts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvShifts.BackgroundColor = System.Drawing.Color.White;
            this.dgvShifts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvShifts.ColumnHeadersHeight = 40;
            this.dgvShifts.EnableHeadersVisualStyles = false;
            this.dgvShifts.Location = new System.Drawing.Point(20, 160);
            this.dgvShifts.Name = "dgvShifts";
            this.dgvShifts.ReadOnly = true;
            this.dgvShifts.RowTemplate.Height = 35;
            this.dgvShifts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvShifts.Size = new System.Drawing.Size(1160, 520);

            this.dgvShifts.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#007ACC");
            this.dgvShifts.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvShifts.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            // UserControl
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F2F5");
            this.Controls.Add(lblTitle);
            this.Controls.Add(panel);
            this.Controls.Add(this.dgvShifts);
            this.Name = "AdminShifts";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(1200, 700);
            ((System.ComponentModel.ISupportInitialize)(this.dgvShifts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvShifts;
        private System.Windows.Forms.DateTimePicker dtpDate;
    }
}