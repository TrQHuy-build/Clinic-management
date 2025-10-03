using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class ExamForm : Form
    {
        public ExamForm()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT appointment_id, patient_id, staff_id, appointment_date, status FROM Appointment WHERE appointment_date >= @d ORDER BY appointment_date", con);
                cmd.Parameters.AddWithValue("@d", DateTime.Today);
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvAppointments.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCreateRecord_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null) return;
            var apptId = (int)dgvAppointments.CurrentRow.Cells["appointment_id"].Value;
            var diagnosis = txtDiagnosis.Text.Trim();
            var treatment = txtTreatment.Text.Trim();
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment) SELECT patient_id, staff_id, @diag, @treat FROM Appointment WHERE appointment_id = @a", con);
                cmd.Parameters.AddWithValue("@diag", diagnosis);
                cmd.Parameters.AddWithValue("@treat", treatment);
                cmd.Parameters.AddWithValue("@a", apptId);
                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Medical record created");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
