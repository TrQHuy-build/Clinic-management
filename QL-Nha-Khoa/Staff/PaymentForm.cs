using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class PaymentForm : Form
    {
        public PaymentForm()
        {
            InitializeComponent();
        }

        private void btnLoadUnpaid_Click(object sender, EventArgs e)
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT invoice_id, patient_id, staff_id, invoice_date, total_amount, status FROM Invoice WHERE status <> N'paid'", con);
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvInvoices.DataSource = dt;
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            
            }
        }

        private void btnMarkPaid_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.CurrentRow == null) return;
            var id = (int)dgvInvoices.CurrentRow.Cells["invoice_id"].Value;
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("UPDATE Invoice SET status = N'paid' WHERE invoice_id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Marked paid");
                btnLoadUnpaid_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
