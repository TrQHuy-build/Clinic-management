using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Pages.Staff
{
    public partial class StaffInvoicing : UserControl
    {
        public StaffInvoicing()
        {
            InitializeComponent();
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            try
            {
                string statusFilter = cboStatus.SelectedItem?.ToString() ?? "Tất cả";

                string query = @"
                    SELECT
                        i.invoice_id AS [ID],
                        u.fullname AS [Bệnh nhân],
                        FORMAT(i.invoice_date, 'dd/MM/yyyy HH:mm') AS [Ngày tạo],
                        i.total_amount AS [Tổng tiền],
                        CASE i.status
                            WHEN 'unpaid' THEN N'Chưa thanh toán'
                            WHEN 'paid' THEN N'Đã thanh toán'
                            WHEN 'cancelled' THEN N'Đã hủy'
                            ELSE i.status
                        END AS [Trạng thái],
                        st.fullname AS [Người tạo]
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    LEFT JOIN Staff s ON i.staff_id = s.staff_id
                    LEFT JOIN UserAccount st ON s.user_id = st.user_id
                    WHERE 1=1";

                System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();

                if (statusFilter != "Tất cả")
                {
                    string dbStatus = statusFilter == "Chưa thanh toán" ? "unpaid" : statusFilter == "Đã thanh toán" ? "paid" : "cancelled";
                    query += " AND i.status = @status";
                    parameters.Add(new SqlParameter("@status", dbStatus));
                }

                query += " ORDER BY i.invoice_date DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
                dgvInvoices.DataSource = dt;

                if (dgvInvoices.Columns.Contains("ID"))
                    dgvInvoices.Columns["ID"].Visible = false;

                if (dgvInvoices.Columns.Contains("Tổng tiền"))
                    dgvInvoices.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";

                AddActionColumns();
                ApplyRowColors();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void AddActionColumns()
        {
            if (!dgvInvoices.Columns.Contains("View"))
            {
                dgvInvoices.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "View",
                    HeaderText = "",
                    Text = "Xem",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                });
            }

            if (!dgvInvoices.Columns.Contains("Pay"))
            {
                dgvInvoices.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Pay",
                    HeaderText = "",
                    Text = "Thanh toán",
                    UseColumnTextForButtonValue = true,
                    Width = 100
                });
            }

            if (!dgvInvoices.Columns.Contains("Print"))
            {
                dgvInvoices.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = "Print",
                    HeaderText = "",
                    Text = "In",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                });
            }
        }

        private void ApplyRowColors()
        {
            foreach (DataGridViewRow row in dgvInvoices.Rows)
            {
                if (row.IsNewRow) continue;

                string status = row.Cells["Trạng thái"].Value?.ToString();

                if (status == "Chưa thanh toán")
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                else if (status == "Đã thanh toán")
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                else if (status == "Đã hủy")
                    row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F5F5F5");
            }
        }

        private void DgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int invoiceId = Convert.ToInt32(dgvInvoices.Rows[e.RowIndex].Cells["ID"].Value);
            string status = dgvInvoices.Rows[e.RowIndex].Cells["Trạng thái"].Value?.ToString();
            string columnName = dgvInvoices.Columns[e.ColumnIndex].Name;

            if (columnName == "View")
            {
                ViewInvoice(invoiceId);
            }
            else if (columnName == "Pay")
            {
                if (status != "Chưa thanh toán")
                {
                    MessageBoxHelper.ShowWarning("Hóa đơn này đã được thanh toán hoặc đã hủy!");
                    return;
                }
                PayInvoice(invoiceId);
            }
            else if (columnName == "Print")
            {
                PrintInvoice(invoiceId);
            }
        }

        private void ViewInvoice(int invoiceId)
        {
            using (Form form = new Form())
            {
                form.Text = $"Chi tiết hóa đơn #{invoiceId}";
                form.Size = new Size(800, 600);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                try
                {
                    // Load invoice info
                    string queryInvoice = @"
                        SELECT i.*, u.fullname AS patient_name, u.phone
                        FROM Invoice i
                        INNER JOIN Patient p ON i.patient_id = p.patient_id
                        INNER JOIN UserAccount u ON p.user_id = u.user_id
                        WHERE i.invoice_id = @id";

                    DataTable dtInvoice = DatabaseHelper.ExecuteQuery(queryInvoice, new SqlParameter[] {
                        new SqlParameter("@id", invoiceId)
                    });

                    if (dtInvoice.Rows.Count == 0) return;

                    DataRow invoice = dtInvoice.Rows[0];

                    Label lblInfo = new Label
                    {
                        Text = $"Bệnh nhân: {invoice["patient_name"]}\n" +
                               $"SĐT: {invoice["phone"]}\n" +
                               $"Ngày tạo: {Convert.ToDateTime(invoice["invoice_date"]):dd/MM/yyyy HH:mm}\n" +
                               $"Trạng thái: {invoice["status"]}\n",
                        Location = new Point(20, 20),
                        Size = new Size(750, 80),
                        Font = new Font("Segoe UI", 11)
                    };

                    // Load services
                    Label lblServices = new Label
                    {
                        Text = "Dịch vụ đã sử dụng:",
                        Location = new Point(20, 110),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold)
                    };

                    DataGridView dgvServices = new DataGridView
                    {
                        Location = new Point(20, 140),
                        Size = new Size(750, 150),
                        BackgroundColor = Color.White,
                        AllowUserToAddRows = false,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    };

                    string queryServices = @"
                        SELECT s.service_name AS [Dịch vụ], s.price AS [Giá], su.quantity AS [Số lượng]
                        FROM ServiceUsage su
                        INNER JOIN Service s ON su.service_id = s.service_id
                        WHERE su.invoice_id = @id";

                    DataTable dtServices = DatabaseHelper.ExecuteQuery(queryServices, new SqlParameter[] {
                        new SqlParameter("@id", invoiceId)
                    });
                    dgvServices.DataSource = dtServices;

                    if (dgvServices.Columns.Contains("Giá"))
                        dgvServices.Columns["Giá"].DefaultCellStyle.Format = "N0";

                    // Load medicines
                    Label lblMedicines = new Label
                    {
                        Text = "Thuốc:",
                        Location = new Point(20, 310),
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12, FontStyle.Bold)
                    };

                    DataGridView dgvMedicines = new DataGridView
                    {
                        Location = new Point(20, 340),
                        Size = new Size(750, 150),
                        BackgroundColor = Color.White,
                        AllowUserToAddRows = false,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                    };

                    string queryMedicines = @"
                        SELECT m.name AS [Thuốc], pr.dosage AS [Liều lượng], pr.quantity AS [Số lượng], m.price AS [Đơn giá]
                        FROM InvoicePrescription ip
                        INNER JOIN Prescription pr ON ip.prescription_id = pr.prescription_id
                        INNER JOIN Medicine m ON pr.medicine_id = m.medicine_id
                        WHERE ip.invoice_id = @id";

                    DataTable dtMedicines = DatabaseHelper.ExecuteQuery(queryMedicines, new SqlParameter[] {
                        new SqlParameter("@id", invoiceId)
                    });
                    dgvMedicines.DataSource = dtMedicines;

                    if (dgvMedicines.Columns.Contains("Đơn giá"))
                        dgvMedicines.Columns["Đơn giá"].DefaultCellStyle.Format = "N0";

                    Label lblTotal = new Label
                    {
                        Text = $"TỔNG TIỀN: {Formatter.FormatCurrency(Convert.ToDecimal(invoice["total_amount"]))}",
                        Location = new Point(20, 510),
                        Size = new Size(750, 30),
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        ForeColor = ColorTranslator.FromHtml("#DC3545")
                    };

                    form.Controls.AddRange(new Control[] { lblInfo, lblServices, dgvServices, lblMedicines, dgvMedicines, lblTotal });
                    form.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
                }
            }
        }

        private void PayInvoice(int invoiceId)
        {
            if (!MessageBoxHelper.ShowConfirm("Xác nhận đã nhận thanh toán?"))
                return;

            try
            {
                string query = "UPDATE Invoice SET status = 'paid' WHERE invoice_id = @id";
                int result = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter[] {
                    new SqlParameter("@id", invoiceId)
                });

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đã xác nhận thanh toán!");
                    Logger.LogAction("PAY_INVOICE", $"Thanh toán hóa đơn #{invoiceId}");
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void PrintInvoice(int invoiceId)
        {
            MessageBoxHelper.ShowInfo($"Chức năng in hóa đơn #{invoiceId} đang được phát triển!");
            // TODO: Implement print functionality
        }

        private void CboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            cboStatus.SelectedIndex = 0;
            LoadInvoices();
        }
    }
}