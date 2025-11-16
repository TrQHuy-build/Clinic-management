using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminInvoices : UserControl
    {
        public AdminInvoices()
        {
            InitializeComponent();
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            try
            {
                string status = cboStatus.SelectedItem?.ToString();
                string query = @"
                    SELECT
                        i.invoice_id AS [Mã HĐ],
                        u.fullname AS [Bệnh nhân],
                        FORMAT(i.invoice_date, 'dd/MM/yyyy') AS [Ngày],
                        i.total_amount AS [Tổng tiền],
                        i.status AS [Trạng thái]
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    WHERE CAST(i.invoice_date AS DATE) BETWEEN @from AND @to";

                if (status != "Tất cả" && !string.IsNullOrEmpty(status))
                    query += " AND i.status = @status";

                query += " ORDER BY i.invoice_date DESC";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@from", dtpFrom.Value.Date),
                    new SqlParameter("@to", dtpTo.Value.Date),
                    new SqlParameter("@status", status == "Tất cả" ? (object)DBNull.Value : status)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvInvoices.DataSource = dt;

                // Định dạng cột tiền
                if (dgvInvoices.Columns.Contains("Tổng tiền"))
                    dgvInvoices.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";

                // Thêm cột Xem nếu chưa có
                if (!dgvInvoices.Columns.Contains("View"))
                {
                    var btnColumn = new DataGridViewButtonColumn
                    {
                        Name = "View",
                        HeaderText = "",
                        Text = "Xem",
                        UseColumnTextForButtonValue = true,
                        Width = 70
                    };
                    dgvInvoices.Columns.Add(btnColumn);
                }

                // Định dạng trạng thái + màu nền
                foreach (DataGridViewRow row in dgvInvoices.Rows)
                {
                    if (row.Cells["Trạng thái"].Value is string st)
                    {
                        row.Cells["Trạng thái"].Value = Formatter.FormatStatus(st);
                        if (st.Equals("unpaid", StringComparison.OrdinalIgnoreCase))
                            row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khi tải hóa đơn: {ex.Message}");
            }
        }

        private void DgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvInvoices.Columns["View"] is DataGridViewColumn viewCol && e.ColumnIndex == viewCol.Index)
            {
                int invoiceId = Convert.ToInt32(dgvInvoices.Rows[e.RowIndex].Cells["Mã HĐ"].Value);
                ShowInvoiceDetail(invoiceId);
            }
        }

        private void ShowInvoiceDetail(int invoiceId)
        {
            try
            {
                string query = @"
                    SELECT i.*, u.fullname AS patient_name, u2.fullname AS staff_name
                    FROM Invoice i
                    INNER JOIN Patient p ON i.patient_id = p.patient_id
                    INNER JOIN UserAccount u ON p.user_id = u.user_id
                    LEFT JOIN Staff s ON i.staff_id = s.staff_id
                    LEFT JOIN UserAccount u2 ON s.user_id = u2.user_id
                    WHERE i.invoice_id = @id";

                DataTable dtInvoice = DatabaseHelper.ExecuteQuery(
                    query,
                    new SqlParameter[] { new SqlParameter("@id", invoiceId) }
                );

                if (dtInvoice.Rows.Count == 0)
                {
                    MessageBoxHelper.ShowError("Không tìm thấy hóa đơn.");
                    return;
                }

                var row = dtInvoice.Rows[0];

                using (Form form = new Form
                {
                    Text = $"Chi tiết hóa đơn #{invoiceId}",
                    Size = new Size(700, 600),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    MaximizeBox = false
                })
                {
                    Label lblInfo = new Label
                    {
                        Text = $"Bệnh nhân: {row["patient_name"]}\n" +
                               $"Ngày: {Convert.ToDateTime(row["invoice_date"]):dd/MM/yyyy}\n" +
                               $"Nhân viên: {(row["staff_name"] == DBNull.Value ? "N/A" : row["staff_name"])}\n" +
                               $"Trạng thái: {Formatter.FormatStatus(row["status"].ToString())}",
                        Location = new Point(20, 20),
                        Size = new Size(650, 90),
                        Font = new Font("Segoe UI", 11)
                    };

                    DataGridView dgvDetails = new DataGridView
                    {
                        Location = new Point(20, 120),
                        Size = new Size(650, 350),
                        BackgroundColor = Color.White,
                        AllowUserToAddRows = false,
                        ReadOnly = true,
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                        RowHeadersVisible = false
                    };

                    string serviceQuery = @"
                        SELECT s.service_name AS [Dịch vụ], su.quantity AS [SL], s.price * su.quantity AS [Thành tiền]
                        FROM ServiceUsage su
                        INNER JOIN Service s ON su.service_id = s.service_id
                        WHERE su.invoice_id = @id";

                    DataTable dtServices = DatabaseHelper.ExecuteQuery(
                        serviceQuery,
                        new SqlParameter[] { new SqlParameter("@id", invoiceId) }
                    );

                    dgvDetails.DataSource = dtServices;
                    if (dgvDetails.Columns["Thành tiền"] != null)
                        dgvDetails.Columns["Thành tiền"].DefaultCellStyle.Format = "N0";

                    Label lblTotal = new Label
                    {
                        Text = $"TỔNG TIỀN: {Convert.ToDecimal(row["total_amount"]):N0}đ",
                        Location = new Point(20, 490),
                        Size = new Size(650, 30),
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        ForeColor = ColorTranslator.FromHtml("#DC3545"),
                        TextAlign = ContentAlignment.MiddleRight
                    };

                    form.Controls.AddRange(new Control[] { lblInfo, dgvDetails, lblTotal });
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi khi xem chi tiết: {ex.Message}");
            }
        }

        // Event handlers (được gán trong Designer)
        private void dtpFrom_ValueChanged(object sender, EventArgs e) => LoadInvoices();
        private void dtpTo_ValueChanged(object sender, EventArgs e) => LoadInvoices();
        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e) => LoadInvoices();
    }
}