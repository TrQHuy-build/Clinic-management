namespace QL_Nha_Khoa
{
    partial class SalaryEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbStaff;
        private System.Windows.Forms.NumericUpDown numBase;
        private System.Windows.Forms.NumericUpDown numBonus;
        private System.Windows.Forms.NumericUpDown numDeduction;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbStaff = new System.Windows.Forms.ComboBox();
            this.numBase = new System.Windows.Forms.NumericUpDown();
            this.numBonus = new System.Windows.Forms.NumericUpDown();
            this.numDeduction = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDeduction)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbStaff
            // 
            this.cmbStaff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStaff.Location = new System.Drawing.Point(24, 24);
            this.cmbStaff.Size = new System.Drawing.Size(300, 28);
            // 
            // numBase
            // 
            this.numBase.Location = new System.Drawing.Point(24, 64);
            this.numBase.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numBase.Size = new System.Drawing.Size(140, 27);
            // 
            // numBonus
            // 
            this.numBonus.Location = new System.Drawing.Point(184, 64);
            this.numBonus.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numBonus.Size = new System.Drawing.Size(140, 27);
            // 
            // numDeduction
            // 
            this.numDeduction.Location = new System.Drawing.Point(24, 104);
            this.numDeduction.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.numDeduction.Size = new System.Drawing.Size(140, 27);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(24, 148);
            this.btnSave.Size = new System.Drawing.Size(94, 29);
            this.btnSave.Text = "Save";
            this.btnSave.Click += btnSave_Click;
            // 
            // SalaryEditForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 200);
            this.Controls.Add(this.cmbStaff);
            this.Controls.Add(this.numBase);
            this.Controls.Add(this.numBonus);
            this.Controls.Add(this.numDeduction);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Salary";
            ((System.ComponentModel.ISupportInitialize)(this.numBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDeduction)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
