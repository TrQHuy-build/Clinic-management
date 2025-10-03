using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class CheckInForm : Form
    {
        public CheckInForm()
        {
            InitializeComponent();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT p.patient_id, ua.fullname FROM Patient p JOIN UserAccount ua ON p.user_id = ua.user_id WHERE ua.phone = @q OR ua.email = @q", con);
                cmd.Parameters.AddWithValue("@q", txtQuery.Text.Trim());
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvResults.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCheckin_Click(object sender, EventArgs e)
        {
            if (dgvResults.CurrentRow == null) return;
            var patientId = (int)dgvResults.CurrentRow.Cells["patient_id"].Value;
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("INSERT INTO Appointment (patient_id, staff_id, appointment_date, status) VALUES (@p, @s, @d, N'checked-in')", con);
                cmd.Parameters.AddWithValue("@p", patientId);
                cmd.Parameters.AddWithValue("@s", CurrentUser.Instance.UserId);
                cmd.Parameters.AddWithValue("@d", DateTime.Now);
                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Check-in successful");
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
