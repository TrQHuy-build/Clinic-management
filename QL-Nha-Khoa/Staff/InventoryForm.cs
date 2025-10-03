using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class InventoryForm : Form
    {
        public InventoryForm()
        {
            InitializeComponent();
        }

        private void InventoryForm_Load(object sender, EventArgs e)
        {
            LoadInventory();
        }

        private void LoadInventory()
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT item_id, item_name, type, quantity, unit, supplier FROM Inventory", con);
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvInventory.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdjust_Click(object sender, EventArgs e)
        {
            if (dgvInventory.CurrentRow == null) return;
            var id = (int)dgvInventory.CurrentRow.Cells["item_id"].Value;
            var delta = (int)numAdjust.Value;
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("UPDATE Inventory SET quantity = quantity + @d WHERE item_id = @id", con);
                cmd.Parameters.AddWithValue("@d", delta);
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
                LoadInventory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
