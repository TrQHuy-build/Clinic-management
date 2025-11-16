using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Doctor
{
    public partial class DoctorMedicalRecords : UserControl
    {
        public DoctorMedicalRecords()
        {
            InitializeComponent();
            InitializeEvents(); // Gọi sau InitializeComponent
            LoadRecords();
        }

        private void InitializeEvents()
        {
            // Placeholder logic cho txtSearch
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Tìm theo tên bệnh nhân...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Tìm theo tên bệnh nhân...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            txtSearch.TextChanged += (s, e) => LoadRecords();
            dgvRecords.CellClick += DgvRecords_CellClick;
        }

        private void LoadRecords()
        {
            if (!Auth.CurrentStaffId.HasValue) return;

            try
            {
                string search = txtSearch.Text.Trim();
                string query = @"
                    SELECT
                        m.record_id AS [ID],
                        u.fullname AS [Bệnh nhân],
                        m.diagnosis AS [Chẩn đoán],
                        m.treatment AS [Điều trị],
                        FORMAT(m.record_date, 'dd/MM/yyyy') AS [Ngày khám]
                    FROM MedicalRecord m
                    INNER JOIN Patient p ON m.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE m.staff_id = @staffId AND u.fullname LIKE @search
                    ORDER BY m.record_date DESC";

                var parameters = new[]
                {
                    new SqlParameter("@staffId", Auth.CurrentStaffId.Value),
                    new SqlParameter("@search", $"%{search}%")
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvRecords.DataSource = dt;

                if (dgvRecords.Columns["ID"] != null)
                    dgvRecords.Columns["ID"].Visible = false;

                if (!dgvRecords.Columns.Contains("View"))
                {
                    var btnCol = new DataGridViewButtonColumn
                    {
                        Name = "View",
                        HeaderText = "Hành động",
                        Text = "Xem chi tiết",
                        UseColumnTextForButtonValue = true,
                        Width = 100
                    };
                    dgvRecords.Columns.Add(btnCol);
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải hồ sơ: {ex.Message}");
            }
        }

        private void DgvRecords_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var idCell = dgvRecords.Rows[e.RowIndex].Cells["ID"];
            if (idCell?.Value == null) return;

            int recordId = Convert.ToInt32(idCell.Value);

            if (dgvRecords.Columns[e.ColumnIndex].Name == "View")
            {
                ShowRecordDetail(recordId);
            }
        }

        private void ShowRecordDetail(int recordId)
        {
            var form = new Form
            {
                Text = $"Chi tiết hồ sơ #{recordId}",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            string query = @"
                SELECT m.*, u.fullname AS patient_name
                FROM MedicalRecord m
                INNER JOIN Patient p ON m.patient_id = p.patient_id
                INNER JOIN UserAccount u ON p.user_id = u.user_id
                WHERE m.record_id = @id";

            DataTable dt = DatabaseHelper.ExecuteQuery(query, new[] { new SqlParameter("@id", recordId) });
            if (dt.Rows.Count == 0) return;

            var dr = dt.Rows[0];

            var lblInfo = new Label
            {
                Text = $"Bệnh nhân: {dr["patient_name"]}\n" +
                       $"Ngày khám: {Convert.ToDateTime(dr["record_date"]):dd/MM/yyyy}\n\n" +
                       $"Chẩn đoán:\n{dr["diagnosis"]}\n\n" +
                       $"Điều trị:\n{dr["treatment"]}",
                Location = new Point(20, 20),
                Size = new Size(650, 160),
                Font = new Font("Segoe UI", 11),
                AutoSize = false
            };

            var lblPres = new Label
            {
                Text = "Đơn thuốc:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 190),
                AutoSize = true
            };

            var dgvPres = new DataGridView
            {
                Location = new Point(20, 220),
                Size = new Size(650, 300),
                BackgroundColor = Color.White,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None
            };

            string presQuery = @"
                SELECT
                    m.name AS [Tên thuốc],
                    p.dosage AS [Liều lượng],
                    p.quantity AS [Số lượng],
                    p.notes AS [Ghi chú]
                FROM Prescription p
                INNER JOIN Medicine m ON p.medicine_id = m.medicine_id
                WHERE p.record_id = @id";

            DataTable dtPres = DatabaseHelper.ExecuteQuery(presQuery, new[] { new SqlParameter("@id", recordId) });
            dgvPres.DataSource = dtPres;

            form.Controls.AddRange(new Control[] { lblInfo, lblPres, dgvPres });
            form.ShowDialog(this);
        }
    }
}