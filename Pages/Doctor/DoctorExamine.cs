using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorExamine : UserControl
    {
        private List<PrescriptionItem> prescriptions = new List<PrescriptionItem>();

        public DoctorExamine()
        {
            InitializeComponent();
            InitializeEvents();
            LoadPatients();
            LoadServices();
            ConfigureDataGridViews();
        }

        private void InitializeEvents()
        {
            btnAddMedicine.Click += BtnAddMedicine_Click;
            btnSave.Click += BtnSave_Click;
        }

        private void ConfigureDataGridViews()
        {
            dgvMedicines.Columns.Clear();
            dgvMedicines.Columns.Add("MedicineId", "ID");
            dgvMedicines.Columns.Add("Name", "Tên thuốc");
            dgvMedicines.Columns.Add("Dosage", "Liều lượng");
            dgvMedicines.Columns.Add("Quantity", "Số lượng");
            dgvMedicines.Columns.Add("Notes", "Ghi chú");
            dgvMedicines.Columns["MedicineId"].Visible = false;
        }

        #region Data Loading

        private void LoadPatients()
        {
            try
            {
                string query = @"
                    SELECT p.patient_id, u.fullname + ' - ' + u.phone AS display_name
                    FROM Patient p
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    ORDER BY u.fullname";

                DataTable dt = DatabaseHelper.ExecuteQuery(query);
                cboPatient.DisplayMember = "display_name";
                cboPatient.ValueMember = "patient_id";
                cboPatient.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải danh sách bệnh nhân: {ex.Message}");
            }
        }

        private void LoadServices()
        {
            try
            {
                string query = "SELECT service_id AS [ID], service_name AS [Dịch vụ], price AS [Giá] FROM Service WHERE status = N'available'";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                dgvServices.DataSource = null;
                dgvServices.Columns.Clear();

                // Checkbox column
                DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
                {
                    Name = "Selected",
                    HeaderText = "Chọn",
                    Width = 50,
                    FalseValue = false,
                    TrueValue = true
                };
                dgvServices.Columns.Add(chkCol);

                // Data columns
                foreach (DataColumn col in dt.Columns)
                {
                    DataGridViewTextBoxColumn gridCol = new DataGridViewTextBoxColumn
                    {
                        Name = col.ColumnName,
                        HeaderText = col.ColumnName == "ID" ? "ID" : (col.ColumnName == "Giá" ? "Giá" : "Dịch vụ"),
                        DataPropertyName = col.ColumnName
                    };
                    if (col.ColumnName == "ID") gridCol.Visible = false;
                    if (col.ColumnName == "Giá") gridCol.DefaultCellStyle.Format = "N0";
                    dgvServices.Columns.Add(gridCol);
                }

                dgvServices.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dịch vụ: {ex.Message}");
            }
        }

        #endregion

        #region Medicine Management

        private void BtnAddMedicine_Click(object sender, EventArgs e)
        {
            Form form = new Form
            {
                Text = "Thêm thuốc vào đơn",
                Size = new Size(500, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            ComboBox cboMedicine = new ComboBox
            {
                Location = new Point(120, 30),
                Size = new Size(330, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // TextBox với placeholder giả lập
            TextBox txtDosage = new TextBox
            {
                Location = new Point(120, 70),
                Size = new Size(330, 25),
                ForeColor = Color.Gray,
                Text = "VD: 2 viên/ngày"
            };

            txtDosage.GotFocus += (s, ev) =>
            {
                if (txtDosage.Text == "VD: 2 viên/ngày")
                {
                    txtDosage.Text = "";
                    txtDosage.ForeColor = Color.Black;
                }
            };

            txtDosage.LostFocus += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtDosage.Text))
                {
                    txtDosage.Text = "VD: 2 viên/ngày";
                    txtDosage.ForeColor = Color.Gray;
                }
            };

            NumericUpDown numQty = new NumericUpDown
            {
                Location = new Point(120, 110),
                Size = new Size(330, 25),
                Minimum = 1,
                Value = 1
            };

            TextBox txtNotes = new TextBox
            {
                Location = new Point(120, 150),
                Size = new Size(330, 60),
                Multiline = true
            };

            // Load medicines
            DataTable dtMed = DatabaseHelper.ExecuteQuery("SELECT medicine_id, name FROM Medicine ORDER BY name");
            cboMedicine.DisplayMember = "name";
            cboMedicine.ValueMember = "medicine_id";
            cboMedicine.DataSource = dtMed;

            // Labels
            form.Controls.Add(new Label { Text = "Thuốc:", Location = new Point(30, 33), AutoSize = true });
            form.Controls.Add(new Label { Text = "Liều lượng:", Location = new Point(30, 73), AutoSize = true });
            form.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(30, 113), AutoSize = true });
            form.Controls.Add(new Label { Text = "Ghi chú:", Location = new Point(30, 153), AutoSize = true });

            form.Controls.Add(cboMedicine);
            form.Controls.Add(txtDosage);
            form.Controls.Add(numQty);
            form.Controls.Add(txtNotes);

            Button btnAdd = new Button
            {
                Text = "Thêm",
                Location = new Point(200, 230),
                Size = new Size(100, 35),
                BackColor = ColorTranslator.FromHtml("#007ACC"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnAdd.Click += (s, ev) =>
            {
                string dosage = txtDosage.Text.Trim();
                if (dosage == "VD: 2 viên/ngày" || string.IsNullOrWhiteSpace(dosage))
                {
                    MessageBoxHelper.ShowValidationError("liều lượng");
                    return;
                }

                int medicineId = Convert.ToInt32(cboMedicine.SelectedValue);
                string medicineName = cboMedicine.Text;

                dgvMedicines.Rows.Add(medicineId, medicineName, dosage, (int)numQty.Value, txtNotes.Text.Trim());

                prescriptions.Add(new PrescriptionItem
                {
                    MedicineId = medicineId,
                    Dosage = dosage,
                    Quantity = (int)numQty.Value,
                    Notes = txtNotes.Text.Trim()
                });

                form.Close();
            };

            form.Controls.Add(btnAdd);
            form.ShowDialog(this);
        }

        #endregion

        #region Save Examination

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cboPatient.SelectedValue == null)
            {
                MessageBoxHelper.ShowValidationError("bệnh nhân");
                return;
            }

            if (!Validator.IsNotEmpty(txtDiagnosis.Text.Trim()))
            {
                MessageBoxHelper.ShowValidationError("chẩn đoán");
                return;
            }

            if (!Auth.CurrentStaffId.HasValue)
            {
                MessageBoxHelper.ShowError("Không xác định được bác sĩ!");
                return;
            }

            try
            {
                int patientId = Convert.ToInt32(cboPatient.SelectedValue);
                int staffId = Auth.CurrentStaffId.Value;

                // 1. Insert Medical Record
                string insertRecord = @"
                    INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment)
                    OUTPUT INSERTED.record_id
                    VALUES (@patientId, @staffId, @diagnosis, @treatment)";

                var recordParams = new[]
                {
                    new SqlParameter("@patientId", patientId),
                    new SqlParameter("@staffId", staffId),
                    new SqlParameter("@diagnosis", txtDiagnosis.Text.Trim()),
                    new SqlParameter("@treatment", txtTreatment.Text.Trim())
                };

                int recordId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertRecord, recordParams));

                // 2. Insert Prescriptions & Calculate Medicine Total
                decimal medicineTotal = 0;
                foreach (DataGridViewRow row in dgvMedicines.Rows)
                {
                    if (row.IsNewRow || row.Cells["MedicineId"].Value == null) continue;

                    int medicineId = Convert.ToInt32(row.Cells["MedicineId"].Value);
                    string dosage = row.Cells["Dosage"].Value?.ToString() ?? "";
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    string notes = row.Cells["Notes"].Value?.ToString() ?? "";

                    string insertPres = @"
                        INSERT INTO Prescription (record_id, medicine_id, dosage, quantity, notes)
                        VALUES (@recordId, @medId, @dosage, @qty, @notes)";

                    DatabaseHelper.ExecuteNonQuery(insertPres, new[]
                    {
                        new SqlParameter("@recordId", recordId),
                        new SqlParameter("@medId", medicineId),
                        new SqlParameter("@dosage", dosage),
                        new SqlParameter("@qty", quantity),
                        new SqlParameter("@notes", notes)
                    });

                    object priceObj = DatabaseHelper.ExecuteScalar(
                        "SELECT price FROM Medicine WHERE medicine_id = @id",
                        new SqlParameter[] { new SqlParameter("@id", medicineId) } // ← Dùng mảng
                    );

                    if (priceObj != null)
                        medicineTotal += Convert.ToDecimal(priceObj) * quantity;
                }

                // 3. Calculate Service Total
                decimal serviceTotal = 0;
                foreach (DataGridViewRow row in dgvServices.Rows)
                {
                    if (row.Cells["Selected"].Value is bool selected && selected)
                    {
                        serviceTotal += Convert.ToDecimal(row.Cells["Giá"].Value);
                    }
                }

                decimal totalAmount = serviceTotal + medicineTotal;

                // 4. Create Invoice
                string insertInvoice = @"
                    INSERT INTO Invoice (patient_id, staff_id, total_amount, status)
                    OUTPUT INSERTED.invoice_id
                    VALUES (@patientId, @staffId, @total, N'unpaid')";

                int invoiceId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertInvoice, new[]
                {
                    new SqlParameter("@patientId", patientId),
                    new SqlParameter("@staffId", staffId),
                    new SqlParameter("@total", totalAmount)
                }));

                // 5. Insert Service Usage
                foreach (DataGridViewRow row in dgvServices.Rows)
                {
                    if (row.Cells["Selected"].Value is bool selected && selected)
                    {
                        int serviceId = Convert.ToInt32(row.Cells["ID"].Value);
                        string insertUsage = "INSERT INTO ServiceUsage (invoice_id, service_id, quantity) VALUES (@invId, @srvId, 1)";
                        DatabaseHelper.ExecuteNonQuery(insertUsage, new[]
                        {
                            new SqlParameter("@invId", invoiceId),
                            new SqlParameter("@srvId", serviceId)
                        });
                    }
                }

                MessageBoxHelper.ShowSuccess(
                    $"Đã lưu hồ sơ và tạo hóa đơn #{invoiceId}\n" +
                    $"Tổng tiền: {Formatter.FormatCurrency(totalAmount)}");

                Logger.LogExamination(cboPatient.Text);
                Logger.LogInvoiceCreation(cboPatient.Text, totalAmount);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi lưu hồ sơ: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private void ClearForm()
        {
            txtDiagnosis.Clear();
            txtTreatment.Clear();
            dgvMedicines.Rows.Clear();
            prescriptions.Clear();
            LoadServices(); // Reset checkboxes
        }

        #endregion

        #region Nested Class

        private class PrescriptionItem
        {
            public int MedicineId { get; set; }
            public string Dosage { get; set; }
            public int Quantity { get; set; }
            public string Notes { get; set; }
        }

        #endregion
    }
}