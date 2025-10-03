namespace QL_Nha_Khoa
{
    partial class PaymentForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvInvoices;
        private System.Windows.Forms.Button btnLoadUnpaid;
        private System.Windows.Forms.Button btnMarkPaid;

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
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.btnLoadUnpaid = new System.Windows.Forms.Button();
            this.btnMarkPaid = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInvoices.Location = new System.Drawing.Point(12, 45);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.RowTemplate.Height = 24;
            this.dgvInvoices.Size = new System.Drawing.Size(560, 300);
            this.dgvInvoices.TabIndex = 0;
            // 
            // btnLoadUnpaid
            // 
            this.btnLoadUnpaid.Location = new System.Drawing.Point(12, 12);
            this.btnLoadUnpaid.Name = "btnLoadUnpaid";
            this.btnLoadUnpaid.Size = new System.Drawing.Size(100, 25);
            this.btnLoadUnpaid.TabIndex = 1;
            this.btnLoadUnpaid.Text = "Load Unpaid";
            this.btnLoadUnpaid.UseVisualStyleBackColor = true;
            this.btnLoadUnpaid.Click += new System.EventHandler(this.btnLoadUnpaid_Click);
            // 
            // btnMarkPaid
            // 
            this.btnMarkPaid.Location = new System.Drawing.Point(120, 12);
            this.btnMarkPaid.Name = "btnMarkPaid";
            this.btnMarkPaid.Size = new System.Drawing.Size(100, 25);
            this.btnMarkPaid.TabIndex = 2;
            this.btnMarkPaid.Text = "Mark Paid";
            this.btnMarkPaid.UseVisualStyleBackColor = true;
            this.btnMarkPaid.Click += new System.EventHandler(this.btnMarkPaid_Click);
            // 
            // PaymentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.btnMarkPaid);
            this.Controls.Add(this.btnLoadUnpaid);
            this.Controls.Add(this.dgvInvoices);
            this.Name = "PaymentForm";
            this.Text = "Payments";
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
