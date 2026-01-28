// (Only modified ShowDialog(...) calls updated to use FindForm() as owner)
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
     // Context menu for medicines grid
     private ContextMenuStrip medicineContextMenu;

        public DoctorExamine()
        {
     InitializeComponent();
   InitializeEvents();
        ConfigureDataGridViews();
       LoadPatients();
      LoadServices();
      UpdateSaveButtonState();

  // ✅ TÍNH NĂNG 6: Kiểm tra context từ DoctorAppointments
   CheckAndAutoSelectPatient();
   
   // ✅ FIX LAYOUT: Đưa Kê đơn thuốc và nút Lưu sang bên phải
   AdjustLayout();
        }

        /// <summary>
        /// ✅ TÍNH NĂNG 6: Tự động chọn bệnh nhân từ lịch hẹn
    /// </summary>
        private void CheckAndAutoSelectPatient()
        {
       if (DoctorExamineContext.ShouldAutoSelect && DoctorExamineContext.CurrentPatientId.HasValue)
       {
       // Delay để đảm bảo data đã load
      this.Load += (s, e) =>
 {
         try
         {
// Tìm và chọn bệnh nhân trong combobox
           cboPatient.SelectedValue = DoctorExamineContext.CurrentPatientId.Value;
        
       if (cboPatient.SelectedIndex >= 0)
 {
 // Hiển thị thông báo
      MessageBoxHelper.ShowInfo(
       $"✅ Đã tự động chọn bệnh nhân từ lịch hẹn #{DoctorExamineContext.CurrentAppointmentId}\n\n" +
         "Vui lòng tiến hành khám và điền thông tin.");
                 }
}
      catch { }
    finally
  {
         // Clear context sau khi sử dụng
DoctorExamineContext.Clear();
   }
      };
        }
        }

   // ...existing code...
        private void InitializeEvents()
        {
         btnAddMedicine.Click += BtnAddMedicine_Click;
     btnSave.Click += BtnSave_Click;
     BTNHSBA.Click += BTNHSBA_Click;
            cboPatient.SelectedIndexChanged += (s, e) => UpdateSaveButtonState();
       txtDiagnosis.TextChanged += (s, e) => UpdateSaveButtonState();
            txtTreatment.TextChanged += (s, e) => UpdateSaveButtonState();
       dgvServices.CellValueChanged += (s, e) => { if (e.RowIndex >= 0) UpdateSaveButtonState(); };
            dgvServices.CurrentCellDirtyStateChanged += (s, e) =>
     {
             if (dgvServices.IsCurrentCellDirty)
    dgvServices.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
        dgvMedicines.RowsRemoved += (s, e) => UpdateSaveButtonState();
            dgvMedicines.RowsAdded += (s, e) => UpdateSaveButtonState();
      dgvMedicines.CellContentClick += DgvMedicines_CellContentClick;
 dgvMedicines.KeyDown += DgvMedicines_KeyDown;
            dgvMedicines.MouseDown += DgvMedicines_MouseDown;
   }

        // ...rest of existing code remains the same...
        private void ConfigureDataGridViews()
        {
        dgvMedicines.Columns.Clear();
            dgvMedicines.AllowUserToAddRows = false;
            dgvMedicines.ReadOnly = false;
       dgvMedicines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
 dgvMedicines.Columns.Add("MedicineId", "ID");
            dgvMedicines.Columns.Add("Name", "Tên thuốc");
   dgvMedicines.Columns.Add("Dosage", "Liều lượng");
     dgvMedicines.Columns.Add("Quantity", "Số lượng");
            dgvMedicines.Columns.Add("Notes", "Ghi chú");
        dgvMedicines.Columns["MedicineId"].Visible = false;

if (!dgvMedicines.Columns.Contains("Edit"))
            {
              dgvMedicines.Columns.Add(new DataGridViewButtonColumn
                {
 Name = "Edit",
                HeaderText = "",
            Text = "Sửa",
  UseColumnTextForButtonValue = true,
        Width = 70
        });
            }

            if (!dgvMedicines.Columns.Contains("Delete"))
          {
           dgvMedicines.Columns.Add(new DataGridViewButtonColumn
      {
            Name = "Delete",
          HeaderText = "",
        Text = "Xóa",
     UseColumnTextForButtonValue = true,
      Width = 70
           });
            }

            dgvServices.Columns.Clear();
dgvServices.AllowUserToAddRows = false;
dgvServices.AutoGenerateColumns = false;
 dgvServices.ReadOnly = false;
   dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

medicineContextMenu = new ContextMenuStrip();
       var miDelete = new ToolStripMenuItem("Xóa", null, MedicineDeleteMenuItem_Click) { Name = "miDelete" };
            var miEdit = new ToolStripMenuItem("Sửa", null, MedicineEditMenuItem_Click) { Name = "miEdit" };
      medicineContextMenu.Items.Add(miEdit);
 medicineContextMenu.Items.Add(miDelete);
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
    cboPatient.SelectedIndex = -1;
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
      string query = "SELECT service_id AS ID, service_name AS ServiceName, price AS Price FROM Service WHERE status = N'available'";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);
             dgvServices.Columns.Clear();
  dgvServices.AutoGenerateColumns = false;

                DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
  {
         Name = "Selected",
     HeaderText = "Chọn",
   Width = 50,
         FalseValue = false,
                    TrueValue = true
     };
         dgvServices.Columns.Add(chkCol);

            dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", DataPropertyName = "ID", Visible = false });
       dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "ServiceName", HeaderText = "Dịch vụ", DataPropertyName = "ServiceName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
   dgvServices.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Giá", DataPropertyName = "Price", DefaultCellStyle = { Format = "N0" }, Width = 120 });

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
    ShowMedicineEditor(isEdit: false, editRow: null);
        }

        private void ShowMedicineEditor(bool isEdit, DataGridViewRow editRow)
        {
        using (Form form = new Form())
  {
                form.Text = isEdit ? "Sửa thuốc trong đơn" : "Thêm thuốc vào đơn";
           form.Size = new Size(500, 380);
  form.StartPosition = FormStartPosition.CenterParent;
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
  form.MaximizeBox = false;
          form.MinimizeBox = false;
                form.ShowIcon = false;
    form.ShowInTaskbar = false;

    ComboBox cboMedicine = new ComboBox { Location = new Point(120, 30), Size = new Size(330, 25), DropDownStyle = ComboBoxStyle.DropDownList };
      TextBox txtDosage = new TextBox { Location = new Point(120, 70), Size = new Size(330, 25), ForeColor = Color.Gray, Text = "VD: 2 viên/ngày" };
  NumericUpDown numQty = new NumericUpDown { Location = new Point(120, 110), Size = new Size(330, 25), Minimum = 1, Value = 1 };
  TextBox txtNotes = new TextBox { Location = new Point(120, 150), Size = new Size(330, 80), Multiline = true };

txtDosage.GotFocus += (s, ev) => { if (txtDosage.Text == "VD: 2 viên/ngày") { txtDosage.Text = ""; txtDosage.ForeColor = Color.Black; } };
  txtDosage.LostFocus += (s, ev) => { if (string.IsNullOrWhiteSpace(txtDosage.Text)) { txtDosage.Text = "VD: 2 viên/ngày"; txtDosage.ForeColor = Color.Gray; } };

      DataTable dtMed;
          try
{
           dtMed = DatabaseHelper.ExecuteQuery("SELECT medicine_id, name FROM Medicine ORDER BY name");
            cboMedicine.DisplayMember = "name";
 cboMedicine.ValueMember = "medicine_id";
             cboMedicine.DataSource = dtMed;
     cboMedicine.SelectedIndex = -1;
   }
catch (Exception ex)
        {
        MessageBoxHelper.ShowError($"Lỗi tải danh sách thuốc: {ex.Message}");
             return;
        }

           if (isEdit && editRow != null)
 {
           if (TryGetIntFromObject(editRow.Cells["MedicineId"].Value, out int medId))
       try { cboMedicine.SelectedValue = medId; } catch { }
      txtDosage.Text = editRow.Cells["Dosage"].Value?.ToString() ?? "";
  txtDosage.ForeColor = string.IsNullOrWhiteSpace(txtDosage.Text) ? Color.Gray : Color.Black;
   if (int.TryParse(editRow.Cells["Quantity"].Value?.ToString(), out int q)) numQty.Value = Math.Max(1, q);
                    txtNotes.Text = editRow.Cells["Notes"].Value?.ToString() ?? "";
       }

            form.Controls.Add(new Label { Text = "Thuốc:", Location = new Point(30, 33), AutoSize = true });
           form.Controls.Add(new Label { Text = "Liều lượng:", Location = new Point(30, 73), AutoSize = true });
         form.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(30, 113), AutoSize = true });
     form.Controls.Add(new Label { Text = "Ghi chú:", Location = new Point(30, 153), AutoSize = true });
      form.Controls.AddRange(new Control[] { cboMedicine, txtDosage, numQty, txtNotes });

         Button btnAdd = new Button { Text = isEdit ? "Lưu" : "Thêm", Location = new Point(200, 260), Size = new Size(100, 35), BackColor = ColorTranslator.FromHtml("#007ACC"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };

   void ValidateAddForm()
                {
         bool medicineSelected = cboMedicine.SelectedValue != null && cboMedicine.SelectedIndex >= 0;
        bool dosageValid = !string.IsNullOrWhiteSpace(txtDosage.Text) && txtDosage.Text.Trim() != "VD: 2 viên/ngày";
         btnAdd.Enabled = medicineSelected && dosageValid && numQty.Value >= 1;
       }

      cboMedicine.SelectedIndexChanged += (s, ev) => ValidateAddForm();
       txtDosage.TextChanged += (s, ev) => ValidateAddForm();
   numQty.ValueChanged += (s, ev) => ValidateAddForm();
      ValidateAddForm();

           btnAdd.Click += (s, ev) =>
      {
  if (cboMedicine.SelectedValue == null || cboMedicine.SelectedIndex < 0) { MessageBoxHelper.ShowValidationError("thuốc"); return; }
         string dosage = txtDosage.Text.Trim();
     if (dosage == "VD: 2 viên/ngày" || string.IsNullOrWhiteSpace(dosage)) { MessageBoxHelper.ShowValidationError("liều lượng"); return; }
        if (!TryGetIntFromObject(cboMedicine.SelectedValue, out int medicineId)) { MessageBoxHelper.ShowError("ID thuốc không hợp lệ"); return; }

         if (isEdit && editRow != null)
           {
            editRow.Cells["MedicineId"].Value = medicineId;
  editRow.Cells["Name"].Value = cboMedicine.Text;
            editRow.Cells["Dosage"].Value = dosage;
          editRow.Cells["Quantity"].Value = (int)numQty.Value;
            editRow.Cells["Notes"].Value = txtNotes.Text.Trim();
      }
     else
             {
         dgvMedicines.Rows.Add(medicineId, cboMedicine.Text, dosage, (int)numQty.Value, txtNotes.Text.Trim());
          }
        UpdateSaveButtonState();
  form.DialogResult = DialogResult.OK;
         form.Close();
   };

     form.Controls.Add(btnAdd);
                var owner = this.FindForm();
    if (owner != null) form.ShowDialog(owner); else form.ShowDialog();
            }
        }

      private void DgvMedicines_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
   {
      if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
       string colName = dgvMedicines.Columns[e.ColumnIndex].Name;
        var row = dgvMedicines.Rows[e.RowIndex];

if (colName == "Edit") ShowMedicineEditor(isEdit: true, editRow: row);
  else if (colName == "Delete")
          {
           if (!MessageBoxHelper.ShowConfirm("Xác nhận xóa thuốc khỏi đơn?")) return;
       if (!row.IsNewRow) { dgvMedicines.Rows.RemoveAt(e.RowIndex); UpdateSaveButtonState(); }
          }
   }
            catch (Exception ex) { MessageBoxHelper.ShowError($"Lỗi thao tác thuốc: {ex.Message}"); }
     }

        private void RemoveSelectedMedicineRows()
        {
    if (dgvMedicines.SelectedRows.Count == 0) { MessageBoxHelper.ShowWarning("Vui lòng chọn một thuốc để xóa."); return; }
        if (!MessageBoxHelper.ShowConfirm("Xác nhận xóa thuốc đã chọn khỏi đơn?")) return;

        List<int> indices = new List<int>();
         foreach (DataGridViewRow r in dgvMedicines.SelectedRows)
       if (!r.IsNewRow) indices.Add(r.Index);

   indices.Sort();
   for (int i = indices.Count - 1; i >= 0; i--)
 dgvMedicines.Rows.RemoveAt(indices[i]);
            UpdateSaveButtonState();
  }

    private void DgvMedicines_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Delete) { RemoveSelectedMedicineRows(); e.Handled = true; } }

        private void DgvMedicines_MouseDown(object sender, MouseEventArgs e)
 {
          if (e.Button != MouseButtons.Right) return;
        var hit = dgvMedicines.HitTest(e.X, e.Y);
if (hit.RowIndex >= 0)
    {
       if (!dgvMedicines.Rows[hit.RowIndex].Selected) { dgvMedicines.ClearSelection(); dgvMedicines.Rows[hit.RowIndex].Selected = true; }
      medicineContextMenu.Show(dgvMedicines, new Point(e.X, e.Y));
         }
        }

        private void MedicineDeleteMenuItem_Click(object sender, EventArgs e) => RemoveSelectedMedicineRows();
        private void MedicineEditMenuItem_Click(object sender, EventArgs e)
        {
          if (dgvMedicines.SelectedRows.Count == 0) return;
            ShowMedicineEditor(isEdit: true, editRow: dgvMedicines.SelectedRows[0]);
        }
     #endregion

        #region Save Examination
        private void BtnSave_Click(object sender, EventArgs e)
        {
    if (cboPatient.SelectedValue == null || !TryGetIntFromObject(cboPatient.SelectedValue, out int patientId))
          { MessageBoxHelper.ShowValidationError("bệnh nhân"); return; }

            string diagnosis = txtDiagnosis.Text.Trim();
            if (!Validator.IsNotEmpty(diagnosis)) { MessageBoxHelper.ShowValidationError("chẩn đoán"); return; }
  if (!Auth.CurrentStaffId.HasValue) { MessageBoxHelper.ShowError("Không xác định được bác sĩ!"); return; }
     if (!HasAnyServiceOrMedicine()) { MessageBoxHelper.ShowValidationError("ít nhất một dịch vụ hoặc thuốc"); return; }

            try
        {
     int staffId = Auth.CurrentStaffId.Value;
      object treatmentValue = string.IsNullOrWhiteSpace(txtTreatment.Text) ? (object)DBNull.Value : txtTreatment.Text.Trim();

    using (SqlConnection conn = DatabaseHelper.GetConnection())
    {
               conn.Open();
        using (SqlTransaction tran = conn.BeginTransaction())
        {
   try
        {
       // 1. Tạo MedicalRecord
             string insertRecord = @"INSERT INTO MedicalRecord (patient_id, staff_id, diagnosis, treatment) OUTPUT INSERTED.record_id VALUES (@patientId, @staffId, @diagnosis, @treatment)";
         SqlCommand cmdRecord = new SqlCommand(insertRecord, conn, tran);
       cmdRecord.Parameters.AddWithValue("@patientId", patientId);
cmdRecord.Parameters.AddWithValue("@staffId", staffId);
      cmdRecord.Parameters.AddWithValue("@diagnosis", diagnosis);
                cmdRecord.Parameters.AddWithValue("@treatment", treatmentValue);

         object recObj = cmdRecord.ExecuteScalar();
            if (recObj == null || !int.TryParse(recObj.ToString(), out int recordId)) { tran.Rollback(); MessageBoxHelper.ShowError("Không tạo được hồ sơ."); return; }

    // 2. Lưu Prescriptions
               decimal medicineTotal = 0M;
        List<int> prescriptionIds = new List<int>();

           foreach (DataGridViewRow row in dgvMedicines.Rows)
        {
 if (row.IsNewRow || row.Cells["MedicineId"].Value == null) continue;
  if (!TryGetIntFromObject(row.Cells["MedicineId"].Value, out int medicineId)) continue;

    string dosage = row.Cells["Dosage"].Value?.ToString()?.Trim() ?? "";
          if (!int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int quantity)) quantity = 1;
         string notes = row.Cells["Notes"].Value?.ToString()?.Trim() ?? "";

       if (string.IsNullOrWhiteSpace(dosage)) { tran.Rollback(); MessageBoxHelper.ShowValidationError("liều lượng thuốc"); return; }

              string insertPres = @"INSERT INTO Prescription (record_id, medicine_id, dosage, quantity, notes) OUTPUT INSERTED.prescription_id VALUES (@recordId, @medId, @dosage, @qty, @notes)";
          SqlCommand cmdPres = new SqlCommand(insertPres, conn, tran);
        cmdPres.Parameters.AddWithValue("@recordId", recordId);
        cmdPres.Parameters.AddWithValue("@medId", medicineId);
   cmdPres.Parameters.AddWithValue("@dosage", dosage);
        cmdPres.Parameters.AddWithValue("@qty", quantity);
     cmdPres.Parameters.AddWithValue("@notes", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes);

      object prescriptionIdObj = cmdPres.ExecuteScalar();
         if (prescriptionIdObj != null && int.TryParse(prescriptionIdObj.ToString(), out int prescriptionId))
            prescriptionIds.Add(prescriptionId);

    SqlCommand cmdPrice = new SqlCommand("SELECT price FROM Medicine WHERE medicine_id = @id", conn, tran);
       cmdPrice.Parameters.AddWithValue("@id", medicineId);
   object priceObj = cmdPrice.ExecuteScalar();
             if (!TryGetDecimalFromObject(priceObj, out decimal price)) { tran.Rollback(); MessageBoxHelper.ShowError($"Giá thuốc ID {medicineId} không hợp lệ."); return; }
         medicineTotal += price * quantity;
     }

       // 3. Thu thập thông tin dịch vụ (sẽ lưu sau khi có invoice_id)
       decimal serviceTotal = 0M;
       var selectedServices = new List<(int serviceId, decimal price)>();
       
       foreach (DataGridViewRow row in dgvServices.Rows)
       {
           if (row.IsNewRow || !IsCellChecked(row.Cells["Selected"])) continue;
           if (!TryGetIntFromObject(row.Cells["ID"].Value, out int serviceId)) continue;
           if (!TryGetDecimalFromObject(row.Cells["Price"].Value, out decimal servicePrice)) 
           { 
               tran.Rollback(); 
               MessageBoxHelper.ShowError("Giá dịch vụ không hợp lệ."); 
               return; 
           }
           
           selectedServices.Add((serviceId, servicePrice));
           serviceTotal += servicePrice;
       }

             decimal totalAmount = serviceTotal + medicineTotal;

   // 4. Tạo Invoice
       string insertInvoice = @"INSERT INTO Invoice (patient_id, staff_id, total_amount, status) OUTPUT INSERTED.invoice_id VALUES (@patientId, @staffId, @total, N'unpaid')";
    SqlCommand cmdInvoice = new SqlCommand(insertInvoice, conn, tran);
        cmdInvoice.Parameters.AddWithValue("@patientId", patientId);
                cmdInvoice.Parameters.AddWithValue("@staffId", staffId);
     cmdInvoice.Parameters.AddWithValue("@total", totalAmount);

       object invObj = cmdInvoice.ExecuteScalar();
        if (invObj == null || !int.TryParse(invObj.ToString(), out int invoiceId)) { tran.Rollback(); MessageBoxHelper.ShowError("Không tạo được hóa đơn."); return; }

          // 5. Lưu ServiceUsage từ danh sách đã thu thập
          foreach (var service in selectedServices)
          {
              SqlCommand cmdUsage = new SqlCommand("INSERT INTO ServiceUsage (invoice_id, service_id, quantity) VALUES (@invId, @srvId, 1)", conn, tran);
              cmdUsage.Parameters.AddWithValue("@invId", invoiceId);
              cmdUsage.Parameters.AddWithValue("@srvId", service.serviceId);
              cmdUsage.ExecuteNonQuery();
          }

     // 5.5. Lưu InvoicePrescription
     foreach (int prescriptionId in prescriptionIds)
    {
       SqlCommand cmdInvPres = new SqlCommand("INSERT INTO InvoicePrescription (invoice_id, prescription_id) VALUES (@invId, @presId)", conn, tran);
    cmdInvPres.Parameters.AddWithValue("@invId", invoiceId);
   cmdInvPres.Parameters.AddWithValue("@presId", prescriptionId);
                 cmdInvPres.ExecuteNonQuery();
       }

                // 6. Cập nhật Appointment status
                string updateAppointment = @"UPDATE Appointment SET status = N'Completed' WHERE patient_id = @patientId AND assigned_doctor_id = @doctorId AND status = N'Booked' AND CAST(appointment_date AS DATE) = CAST(GETDATE() AS DATE)";
        SqlCommand cmdUpdateApp = new SqlCommand(updateAppointment, conn, tran);
         cmdUpdateApp.Parameters.AddWithValue("@patientId", patientId);
        cmdUpdateApp.Parameters.AddWithValue("@doctorId", staffId);
 cmdUpdateApp.ExecuteNonQuery();

                 tran.Commit();

           MessageBoxHelper.ShowSuccess($"Đã lưu hồ sơ và tạo hóa đơn #{invoiceId}\nTổng tiền: {Formatter.FormatCurrency(totalAmount)}");
     Logger.LogExamination(cboPatient.Text);
           Logger.LogInvoiceCreation(cboPatient.Text, totalAmount);
    ClearForm();
    }
       catch { tran.Rollback(); throw; }
            }
          }
       }
  catch (Exception ex) { MessageBoxHelper.ShowError($"Lỗi lưu hồ sơ: {ex.Message}"); }
        }
    #endregion

        #region Patient Records (HSBA) Button
private void BTNHSBA_Click(object sender, EventArgs e)
        {
       if (cboPatient.SelectedValue == null || !TryGetIntFromObject(cboPatient.SelectedValue, out int patientId))
       { MessageBoxHelper.ShowValidationError("bệnh nhân"); return; }

     string patientDisplay = cboPatient.Text;
        using (var form = new Form())
            {
     form.Text = $"Hồ sơ bệnh án của {patientDisplay}";
    form.Size = new Size(780, 520);
                form.StartPosition = FormStartPosition.CenterParent;
       form.FormBorderStyle = FormBorderStyle.FixedDialog;
    form.MaximizeBox = false;
           form.MinimizeBox = false;

             var dgv = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, BorderStyle = BorderStyle.None };
         form.Controls.Add(dgv);

                try
     {
    string query = @"SELECT m.record_id AS ID, FORMAT(m.record_date, 'dd/MM/yyyy') AS [Ngày khám], m.diagnosis AS [Chẩn đoán], m.treatment AS [Điều trị] FROM MedicalRecord m WHERE m.patient_id = @patientId ORDER BY m.record_date DESC";
    DataTable dt = DatabaseHelper.ExecuteQuery(query, new[] { new SqlParameter("@patientId", SqlDbType.Int) { Value = patientId } });
     dgv.DataSource = dt;
     if (dgv.Columns.Contains("ID")) dgv.Columns["ID"].Visible = false;

              if (!dgv.Columns.Contains("View"))
      dgv.Columns.Add(new DataGridViewButtonColumn { Name = "View", HeaderText = "Hành động", Text = "Xem chi tiết", UseColumnTextForButtonValue = true, Width = 100 });

             dgv.CellClick += (s, ev) =>
    {
       if (ev.RowIndex < 0 || ev.ColumnIndex < 0 || dgv.Columns[ev.ColumnIndex].Name != "View") return;
          var idCell = dgv.Rows[ev.RowIndex].Cells["ID"];
    if (idCell?.Value == null) return;
  if (!TryGetIntFromObject(idCell.Value, out int recordId)) return;
         ShowRecordDetailInExamine(recordId, form);
    };

var owner = this.FindForm();
  if (owner != null) form.ShowDialog(owner); else form.ShowDialog();
            }
             catch (Exception ex) { MessageBoxHelper.ShowError($"Lỗi tải hồ sơ bệnh án: {ex.Message}"); }
            }
        }

        private void ShowRecordDetailInExamine(int recordId, Form ownerForm)
        {
    using (var form = new Form())
            {
     form.Text = $"Chi tiết hồ sơ #{recordId}";
      form.Size = new Size(700, 600);
      form.StartPosition = FormStartPosition.CenterParent;
     form.FormBorderStyle = FormBorderStyle.FixedDialog;
         form.MaximizeBox = false;
 form.MinimizeBox = false;

          string query = @"SELECT m.*, u.fullname AS patient_name FROM MedicalRecord m INNER JOIN Patient p ON m.patient_id = p.patient_id INNER JOIN UserAccount u ON p.user_id = u.user_id WHERE m.record_id = @id";
       DataTable dt = DatabaseHelper.ExecuteQuery(query, new[] { new SqlParameter("@id", SqlDbType.Int) { Value = recordId } });
  if (dt.Rows.Count == 0) return;
            var dr = dt.Rows[0];

      var lblInfo = new Label { Text = $"Bệnh nhân: {dr["patient_name"]}\nNgày khám: {Convert.ToDateTime(dr["record_date"]):dd/MM/yyyy}\n\nChẩn đoán:\n{dr["diagnosis"]}\n\nĐiều trị:\n{dr["treatment"]}", Location = new Point(20, 20), Size = new Size(650, 160), Font = new Font("Segoe UI", 11), AutoSize = false };
     var lblPres = new Label { Text = "Đơn thuốc:", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, 190), AutoSize = true };
                var dgvPres = new DataGridView { Location = new Point(20, 220), Size = new Size(650, 300), BackgroundColor = Color.White, AllowUserToAddRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BorderStyle = BorderStyle.None };

    string presQuery = @"SELECT m.name AS [Tên thuốc], p.dosage AS [Liều lượng], p.quantity AS [Số lượng], p.notes AS [Ghi chú] FROM Prescription p INNER JOIN Medicine m ON p.medicine_id = m.medicine_id WHERE p.record_id = @id";
              DataTable dtPres = DatabaseHelper.ExecuteQuery(presQuery, new[] { new SqlParameter("@id", SqlDbType.Int) { Value = recordId } });
 dgvPres.DataSource = dtPres;

 form.Controls.AddRange(new Control[] { lblInfo, lblPres, dgvPres });
            if (ownerForm != null) form.ShowDialog(ownerForm); else form.ShowDialog();
    }
 }
        #endregion

        #region Helper Methods
        private void ClearForm()
     {
            cboPatient.SelectedIndex = -1;
            txtDiagnosis.Clear();
            txtTreatment.Clear();
    dgvMedicines.Rows.Clear();
            LoadServices();
    UpdateSaveButtonState();
        }

        private void UpdateSaveButtonState()
   {
     bool hasPatient = cboPatient.SelectedValue != null && TryGetIntFromObject(cboPatient.SelectedValue, out _);
  bool hasDiagnosis = Validator.IsNotEmpty(txtDiagnosis.Text?.Trim());
            bool hasContent = HasAnyServiceOrMedicine();
  btnSave.Enabled = hasPatient && hasDiagnosis && hasContent;
          btnSave.BackColor = btnSave.Enabled ? ColorTranslator.FromHtml("#28A745") : Color.Gray;
            btnSave.ForeColor = btnSave.Enabled ? Color.White : Color.LightGray;
        }

      private bool HasAnyServiceOrMedicine()
        {
bool hasMedicine = dgvMedicines.Rows.Count > 0;
   foreach (DataGridViewRow row in dgvServices.Rows)
      if (!row.IsNewRow && IsCellChecked(row.Cells["Selected"])) return true;
            return hasMedicine;
  }

      private static bool TryGetIntFromObject(object obj, out int value)
    {
     value = 0;
            if (obj == null || obj == DBNull.Value) return false;
     if (obj is int i) { value = i; return true; }
     return int.TryParse(obj.ToString(), out value);
        }

        private static bool TryGetDecimalFromObject(object obj, out decimal value)
        {
            value = 0M;
 if (obj == null || obj == DBNull.Value) return false;
     if (obj is decimal d) { value = d; return true; }
    if (obj is double db) { value = Convert.ToDecimal(db); return true; }
        if (obj is float f) { value = Convert.ToDecimal(f); return true; }
     return decimal.TryParse(obj.ToString(), out value);
   }

        private static bool IsCellChecked(DataGridViewCell cell)
        {
if (cell == null) return false;
            var v = cell.Value;
    if (v == null || v == DBNull.Value) return false;
     if (v is bool b) return b;
            if (bool.TryParse(v.ToString(), out bool bb)) return bb;
            if (int.TryParse(v.ToString(), out int i)) return i != 0;
      return false;
        }
        #endregion
        
        #region Layout Adjustment
        private void AdjustLayout()
        {
            // Tính toán kích thước dựa trên form width
            int padding = 20;
            int formWidth = this.Width > 0 ? this.Width : 1200; // Default nếu chưa có size
            int availableWidth = formWidth - (padding * 3);
            int leftWidth = (int)(availableWidth * 0.45); // 45% cho bên trái
            int rightWidth = (int)(availableWidth * 0.55); // 55% cho bên phải
            int rightX = padding + leftWidth + padding;
            
            // === BÊN TRÁI (45%): Bệnh nhân, Chẩn đoán, Dịch vụ ===
            // Panel Chọn bệnh nhân
            panelPatient.Location = new Point(padding, 90);
            panelPatient.Width = leftWidth;
            panelPatient.Height = 100;
            
            // Panel Chẩn đoán & Điều trị
            panelDiagnosis.Location = new Point(padding, panelPatient.Bottom + 15);
            panelDiagnosis.Width = leftWidth;
            panelDiagnosis.Height = 150;
            
            // Resize textboxes trong panelDiagnosis
            int diagTextWidth = leftWidth - 180;
            if (diagTextWidth > 100)
            {
                txtDiagnosis.Width = diagTextWidth;
                txtTreatment.Width = diagTextWidth;
            }
            
            // Panel Dịch vụ sử dụng
            panelServices.Location = new Point(padding, panelDiagnosis.Bottom + 15);
            panelServices.Width = leftWidth;
            panelServices.Height = 220;
            dgvServices.Width = leftWidth - 40;
            dgvServices.Height = 160;
            
            // === BÊN PHẢI (55%): Kê đơn thuốc + Nút Lưu ===
            // Panel Kê đơn thuốc - tăng chiều cao
            panelMedicines.Location = new Point(rightX, 90);
            panelMedicines.Width = rightWidth;
            panelMedicines.Height = 450; // Tăng từ 400 lên 450
            
            // Resize DataGridView trong panelMedicines
            dgvMedicines.Location = new Point(15, 55);
            dgvMedicines.Width = rightWidth - 30;
            dgvMedicines.Height = 330; // Tăng từ 280 lên 330
            
            // Button Thêm thuốc
            btnAddMedicine.Location = new Point(15, dgvMedicines.Bottom + 15);
            btnAddMedicine.Width = 180;
            
            // Nút Lưu hồ sơ - full width bên phải, dưới cùng
            btnSave.Location = new Point(rightX, panelMedicines.Bottom + 20);
            btnSave.Width = rightWidth;
            btnSave.Height = 50;
            
            // Set minimum size cho UserControl để đảm bảo nút Lưu hiển thị
            int minHeight = btnSave.Bottom + padding + 20;
            if (this.Height < minHeight)
            {
                this.MinimumSize = new Size(1200, minHeight);
            }
            
            // Di chuyển nút Hồ sơ bệnh án vào vị trí hợp lý trong panelPatient
            if (BTNHSBA != null && leftWidth > 200)
            {
                BTNHSBA.Location = new Point(leftWidth - BTNHSBA.Width - 20, 50);
            }
            
            // Resize combobox bệnh nhân
            if (cboPatient != null && leftWidth > 250)
            {
                cboPatient.Width = leftWidth - BTNHSBA.Width - 60;
            }
        }
        
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (this.Width > 0)
            {
                AdjustLayout();
            }
        }
        #endregion

        private void btnSave_Click_1(object sender, EventArgs e) { }
    }
}