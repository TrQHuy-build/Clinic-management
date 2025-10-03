using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace QL_Nha_Khoa
{
    public partial class DoctorExamForm : Form
    {
        public DoctorExamForm()
        {
            InitializeComponent();
        }
        
    private void DoctorExamForm_Load(object sender, EventArgs e)
        {
            LoadServices();
            LoadMedicines();
            LoadAppointments();
        }

    private void LoadServices()
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT service_id, service_name, price FROM Service WHERE status = N'available'", con);
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                cmbService.DisplayMember = "service_name";
                cmbService.ValueMember = "service_id";
                cmbService.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    private void LoadMedicines()
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT medicine_id, name, price FROM Medicine", con);
                var dt = new DataTable();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dgvMedicines.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    private void LoadAppointments()
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT a.appointment_id, a.patient_id, a.appointment_date, a.status, ua.fullname FROM Appointment a JOIN Patient p ON a.patient_id = p.patient_id JOIN UserAccount ua ON p.user_id = ua.user_id WHERE a.staff_id = (SELECT staff_id FROM Staff WHERE user_id = @uid) AND CONVERT(date, a.appointment_date) = CONVERT(date, GETDATE()) ORDER BY a.appointment_date", con);
                cmd.Parameters.AddWithValue("@uid", CurrentUser.Instance.UserId);
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

    private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null) return;
            var value = dgvAppointments.CurrentRow.Cells["patient_id"].Value;
            if (value == DBNull.Value) return;
            var pid = (int)value;
            LoadPatientInfo(pid);
        }

    private void LoadPatientInfo(int patientId)
        {
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                using var cmd = new SqlCommand("SELECT ua.fullname, ua.phone, ua.email, p.date_of_birth, p.gender, p.address FROM Patient p JOIN UserAccount ua ON p.user_id = ua.user_id WHERE p.patient_id = @pid", con);
                cmd.Parameters.AddWithValue("@pid", patientId);
                con.Open();
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    txtPatientName.Text = r.GetString(0);
                    txtPatientPhone.Text = r.GetString(1);
                    txtPatientEmail.Text = r.GetString(2);
                    txtDOB.Text = r.IsDBNull(3) ? "" : r.GetDateTime(3).ToShortDateString();
                    txtGender.Text = r.IsDBNull(4) ? "" : r.GetString(4);
                    txtAddress.Text = r.IsDBNull(5) ? "" : r.GetString(5);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    private void btnAddPrescription_Click(object sender, EventArgs e)
        {
            if (dgvMedicines.CurrentRow == null) return;
            var id = (int)dgvMedicines.CurrentRow.Cells["medicine_id"].Value;
            var name = dgvMedicines.CurrentRow.Cells["name"].Value.ToString();
            var qty = (int)numPrescribeQty.Value;
            var row = new DataGridViewRow();
            row.CreateCells(dgvPrescriptions, id, name, qty);
            dgvPrescriptions.Rows.Add(row);
        }

    private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null) { MessageBox.Show("Chọn lịch hẹn trước"); return; }
            var apptId = (int)dgvAppointments.CurrentRow.Cells["appointment_id"].Value;
            var patientId = (int)dgvAppointments.CurrentRow.Cells["patient_id"].Value;
            var diagnosis = txtDiagnosis.Text.Trim();
            var treatment = txtTreatment.Text.Trim();
            var serviceId = cmbService.SelectedValue == null ? (int?)null : Convert.ToInt32(cmbService.SelectedValue);
            try
            {
                using var con = new SqlConnection(AppConfig.ConnectionString);
                con.Open();
                using var tran = con.BeginTransaction();
                try
                {
                    // Insert medical record
                    using var cmd = new SqlCommand("INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment) VALUES (@p, (SELECT staff_id FROM Staff WHERE user_id = @uid), @diag, @t); SELECT SCOPE_IDENTITY();", con, tran);
                    cmd.Parameters.AddWithValue("@p", patientId);
                    cmd.Parameters.AddWithValue("@uid", CurrentUser.Instance.UserId);
                    cmd.Parameters.AddWithValue("@diag", diagnosis);
                    cmd.Parameters.AddWithValue("@t", treatment);
                    var recIdObj = cmd.ExecuteScalar();
                    var recId = Convert.ToInt32(recIdObj);

                    // Insert prescriptions and collect total medicine cost
                    decimal medsTotal = 0m;
                    foreach (DataGridViewRow r in dgvPrescriptions.Rows)
                    {
                        if (r.IsNewRow) continue;
                        var medId = Convert.ToInt32(r.Cells[0].Value);
                        var medQty = Convert.ToInt32(r.Cells[2].Value);
                        using var pcmd = new SqlCommand("INSERT INTO Prescription (record_id, medicine_id, dosage, quantity, notes) VALUES (@rid, @mid, @dos, @qty, @notes)", con, tran);
                        pcmd.Parameters.AddWithValue("@rid", recId);
                        pcmd.Parameters.AddWithValue("@mid", medId);
                        pcmd.Parameters.AddWithValue("@dos", "-");
                        pcmd.Parameters.AddWithValue("@qty", medQty);
                        pcmd.Parameters.AddWithValue("@notes", "");
                        pcmd.ExecuteNonQuery();
                        // fetch price
                        using var priceCmd = new SqlCommand("SELECT price FROM Medicine WHERE medicine_id = @mid", con, tran);
                        priceCmd.Parameters.AddWithValue("@mid", medId);
                        var p = priceCmd.ExecuteScalar();
                        medsTotal += Convert.ToDecimal(p) * medQty;
                    }

                    // Create invoice for service + medicines
                    decimal servicePrice = 0m;
                    int invoiceId;
                    if (serviceId.HasValue)
                    {
                        using var scmd = new SqlCommand("SELECT price FROM Service WHERE service_id = @sid", con, tran);
                        scmd.Parameters.AddWithValue("@sid", serviceId.Value);
                        var sp = scmd.ExecuteScalar();
                        servicePrice = sp == DBNull.Value ? 0m : Convert.ToDecimal(sp);
                    }
                    var total = servicePrice + medsTotal;
                    using var icmd = new SqlCommand("INSERT INTO Invoice (patient_id, staff_id, total_amount, status) VALUES (@p, (SELECT staff_id FROM Staff WHERE user_id = @uid), @total, N'unpaid'); SELECT SCOPE_IDENTITY();", con, tran);
                    icmd.Parameters.AddWithValue("@p", patientId);
                    icmd.Parameters.AddWithValue("@uid", CurrentUser.Instance.UserId);
                    icmd.Parameters.AddWithValue("@total", total);
                    var invObj = icmd.ExecuteScalar();
                    invoiceId = Convert.ToInt32(invObj);

                    // Insert service usage
                    if (serviceId.HasValue)
                    {
                        using var suc = new SqlCommand("INSERT INTO ServiceUsage (invoice_id, service_id, quantity) VALUES (@iid, @sid, 1)", con, tran);
                        suc.Parameters.AddWithValue("@iid", invoiceId);
                        suc.Parameters.AddWithValue("@sid", serviceId.Value);
                        suc.ExecuteNonQuery();
                    }

                    // Update appointment status to 'done'
                    using var acmd = new SqlCommand("UPDATE Appointment SET status = N'done' WHERE appointment_id = @a", con, tran);
                    acmd.Parameters.AddWithValue("@a", apptId);
                    acmd.ExecuteNonQuery();

                    tran.Commit();
                    MessageBox.Show("Saved medical record and invoice created.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
