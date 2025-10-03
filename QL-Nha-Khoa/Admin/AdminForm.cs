using System;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Chào {CurrentUser.Instance.Fullname} (Admin)";
            LoadUsers();
            LoadShifts();
            LoadSalaries();
            LoadAudit();
        }

        private void LoadUsers()
        {
            try
            {
                using var con = new System.Data.SqlClient.SqlConnection(AppConfig.ConnectionString);
                using var cmd = new System.Data.SqlClient.SqlCommand("SELECT user_id, fullname, phone, email, role, status, created_at FROM UserAccount", con);
                var dt = new System.Data.DataTable();
                using var adt = new System.Data.SqlClient.SqlDataAdapter(cmd);
                adt.Fill(dt);
                dgvUsers.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefreshUsers_Click(object sender, EventArgs e) => LoadUsers();

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            using var f = new UserEditForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void btnEditUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;
            var id = (int)dgvUsers.CurrentRow.Cells["user_id"].Value;
            using var f = new UserEditForm(id);
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;
            var id = (int)dgvUsers.CurrentRow.Cells["user_id"].Value;
            if (MessageBox.Show("Xoá user này?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                using var con = new System.Data.SqlClient.SqlConnection(AppConfig.ConnectionString);
                con.Open();
                using var cmd = new System.Data.SqlClient.SqlCommand("DELETE FROM UserAccount WHERE user_id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadShifts()
        {
            try
            {
                using var con = new System.Data.SqlClient.SqlConnection(AppConfig.ConnectionString);
                using var cmd = new System.Data.SqlClient.SqlCommand("SELECT s.shift_id, sa.fullname AS staff_name, s.shift_date, s.start_time, s.end_time FROM Shift s JOIN Staff st ON s.staff_id = st.staff_id JOIN UserAccount sa ON st.user_id = sa.user_id", con);
                var dt = new System.Data.DataTable();
                using var adt = new System.Data.SqlClient.SqlDataAdapter(cmd);
                adt.Fill(dt);
                dgvShifts.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefreshShifts_Click(object sender, EventArgs e) => LoadShifts();

        private void btnAddShift_Click(object sender, EventArgs e)
        {
            using var f = new ShiftEditForm();
            if (f.ShowDialog() == DialogResult.OK) LoadShifts();
        }

        private void LoadSalaries()
        {
            try
            {
                using var con = new System.Data.SqlClient.SqlConnection(AppConfig.ConnectionString);
                using var cmd = new System.Data.SqlClient.SqlCommand("SELECT salary_id, staff_id, month, year, base_salary, bonus, deduction, total_salary FROM Salary", con);
                var dt = new System.Data.DataTable();
                using var adt = new System.Data.SqlClient.SqlDataAdapter(cmd);
                adt.Fill(dt);
                dgvSalaries.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefreshSalaries_Click(object sender, EventArgs e) => LoadSalaries();

        private void btnAddSalary_Click(object sender, EventArgs e)
        {
            using var f = new SalaryEditForm();
            if (f.ShowDialog() == DialogResult.OK) LoadSalaries();
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                using var con = new System.Data.SqlClient.SqlConnection(AppConfig.ConnectionString);
                using var cmd = new System.Data.SqlClient.SqlCommand("SELECT invoice_id, patient_id, staff_id, invoice_date, total_amount, status FROM Invoice WHERE invoice_date BETWEEN @from AND @to", con);
                cmd.Parameters.AddWithValue("@from", dtpFrom.Value.Date);
                cmd.Parameters.AddWithValue("@to", dtpTo.Value.Date.AddDays(1).AddTicks(-1));
                var dt = new System.Data.DataTable();
                using var adt = new System.Data.SqlClient.SqlDataAdapter(cmd);
                adt.Fill(dt);
                dgvReport.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadAudit()
        {
            try
            {
                using var con = new System.Data.SqlClient.SqlConnection(AppConfig.ConnectionString);
                using var cmd = new System.Data.SqlClient.SqlCommand("SELECT log_id, user_id, action, description, timestamp FROM AuditLog ORDER BY timestamp DESC", con);
                var dt = new System.Data.DataTable();
                using var adt = new System.Data.SqlClient.SqlDataAdapter(cmd);
                adt.Fill(dt);
                dgvAudit.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefreshAudit_Click(object sender, EventArgs e) => LoadAudit();
    }
}
