namespace QL_Nha_Khoa
{
    partial class InventoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.NumericUpDown numAdjust;
        private System.Windows.Forms.Button btnAdjust;

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
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.numAdjust = new System.Windows.Forms.NumericUpDown();
            this.btnAdjust = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdjust)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvInventory
            // 
            this.dgvInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Location = new System.Drawing.Point(12, 12);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.RowTemplate.Height = 24;
            this.dgvInventory.Size = new System.Drawing.Size(560, 300);
            this.dgvInventory.TabIndex = 0;
            // 
            // numAdjust
            // 
            this.numAdjust.Location = new System.Drawing.Point(12, 320);
            this.numAdjust.Minimum = -1000;
            this.numAdjust.Maximum = 1000000;
            this.numAdjust.Name = "numAdjust";
            this.numAdjust.Size = new System.Drawing.Size(120, 22);
            this.numAdjust.TabIndex = 1;
            // 
            // btnAdjust
            // 
            this.btnAdjust.Location = new System.Drawing.Point(140, 318);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(100, 25);
            this.btnAdjust.TabIndex = 2;
            this.btnAdjust.Text = "Adjust";
            this.btnAdjust.UseVisualStyleBackColor = true;
            this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
            // 
            // InventoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.btnAdjust);
            this.Controls.Add(this.numAdjust);
            this.Controls.Add(this.dgvInventory);
            this.Name = "InventoryForm";
            this.Text = "Inventory";
            this.Load += new System.EventHandler(this.InventoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAdjust)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
