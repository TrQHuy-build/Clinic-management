namespace QL_Nha_Khoa
{
    partial class StaffForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblWelcome;
        private Button btnCheckIn;
        private Button btnInventory;
        private Button btnExam;
        private Button btnPayments;

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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnExam = new System.Windows.Forms.Button();
            this.btnPayments = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(24, 24);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(51, 20);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "label1";
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.Location = new System.Drawing.Point(24, 60);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(120, 30);
            this.btnCheckIn.TabIndex = 1;
            this.btnCheckIn.Text = "Check-in Patients";
            this.btnCheckIn.UseVisualStyleBackColor = true;
            this.btnCheckIn.Click += new System.EventHandler(this.btnCheckIn_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(160, 60);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(120, 30);
            this.btnInventory.TabIndex = 2;
            this.btnInventory.Text = "Manage Inventory";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnExam
            // 
            this.btnExam.Location = new System.Drawing.Point(296, 60);
            this.btnExam.Name = "btnExam";
            this.btnExam.Size = new System.Drawing.Size(120, 30);
            this.btnExam.TabIndex = 3;
            this.btnExam.Text = "Examination";
            this.btnExam.UseVisualStyleBackColor = true;
            this.btnExam.Click += new System.EventHandler(this.btnExam_Click);
            // 
            // btnPayments
            // 
            this.btnPayments.Location = new System.Drawing.Point(432, 60);
            this.btnPayments.Name = "btnPayments";
            this.btnPayments.Size = new System.Drawing.Size(120, 30);
            this.btnPayments.TabIndex = 4;
            this.btnPayments.Text = "Payments";
            this.btnPayments.UseVisualStyleBackColor = true;
            this.btnPayments.Click += new System.EventHandler(this.btnPayments_Click);
            // 
            // StaffForm
            // 
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.btnPayments);
            this.Controls.Add(this.btnExam);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.btnCheckIn);
            this.Controls.Add(this.lblWelcome);
            this.Name = "StaffForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Staff Dashboard";
            this.Load += new System.EventHandler(this.StaffForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
