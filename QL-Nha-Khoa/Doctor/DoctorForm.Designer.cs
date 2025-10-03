namespace QL_Nha_Khoa
{
    partial class DoctorForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblWelcome;
    private System.Windows.Forms.Button btnExam;

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
            lblWelcome = new Label();
            btnExam = new Button();
            SuspendLayout();
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Location = new Point(24, 24);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(50, 20);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "label1";
            // 
            // btnExam
            // 
            btnExam.Location = new Point(227, 256);
            btnExam.Name = "btnExam";
            btnExam.Size = new Size(120, 30);
            btnExam.TabIndex = 1;
            btnExam.Text = "Open Exam";
            btnExam.UseVisualStyleBackColor = true;
            btnExam.Click += btnExam_Click;
            // 
            // DoctorForm
            // 
            ClientSize = new Size(600, 400);
            Controls.Add(lblWelcome);
            Controls.Add(btnExam);
            Name = "DoctorForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Doctor Dashboard";
            Load += DoctorForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
