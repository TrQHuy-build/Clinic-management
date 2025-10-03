namespace QL_Nha_Khoa
{
    partial class ShiftEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cmbStaff;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbStaff = new System.Windows.Forms.ComboBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbStaff
            // 
            this.cmbStaff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStaff.Location = new System.Drawing.Point(24, 24);
            this.cmbStaff.Size = new System.Drawing.Size(300, 28);
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(24, 64);
            this.dtpDate.Size = new System.Drawing.Size(300, 27);
            // 
            // dtpStart
            // 
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpStart.Location = new System.Drawing.Point(24, 104);
            this.dtpStart.Size = new System.Drawing.Size(140, 27);
            // 
            // dtpEnd
            // 
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEnd.Location = new System.Drawing.Point(184, 104);
            this.dtpEnd.Size = new System.Drawing.Size(140, 27);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(24, 148);
            this.btnSave.Size = new System.Drawing.Size(94, 29);
            this.btnSave.Text = "Save";
            this.btnSave.Click += btnSave_Click;
            // 
            // ShiftEditForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 200);
            this.Controls.Add(this.cmbStaff);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shift";
            this.ResumeLayout(false);
        }
    }
}
