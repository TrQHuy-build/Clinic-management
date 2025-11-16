using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Patient
{
    public partial class PatientInvoices : UserControl
    {
        public PatientInvoices()
        {
            InitializeComponent();
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            if (!Auth.CurrentPatientId.HasValue) return;

            try
            {
                string query = @"
                    SELECT 
                        i.invoice_id AS [Mã HĐ],
                        FORMAT(i.invoice_date, 'dd/MM/yyyy') AS [Ngày],
                        i.total_amount AS [Tổng tiền],
                        i.status AS [Trạng thái]
                    FROM Invoice i
                    WHERE i.patient_id = @patientId
                    ORDER BY i.invoice_date DESC";

                SqlParameter[] parameters = { new SqlParameter("@patientId", Auth.CurrentPatientId.Value) };
                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvInvoices.DataSource = dt;

                dgvInvoices.Columns["Tổng tiền"].DefaultCellStyle.Format = "N0";

                if (!dgvInvoices.Columns.Contains("View"))
                {
                    dgvInvoices.Columns.Add(new DataGridViewButtonColumn { Name = "View", Text = "Xem", UseColumnTextForButtonValue = true, Width = 70 });
                    dgvInvoices.Columns.Add(new DataGridViewButtonColumn { Name = "Pay", Text = "Thanh toán", UseColumnTextForButtonValue = true, Width = 100 });
                }

                foreach (DataGridViewRow row in dgvInvoices.Rows)
                {
                    string status = row.Cells["Trạng thái"].Value?.ToString();
                    row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);

                    if (status?.ToLower() == "unpaid")
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFEBEE");
                    else
                        row.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8F5E9");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void DgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int invoiceId = Convert.ToInt32(dgvInvoices.Rows[e.RowIndex].Cells["Mã HĐ"].Value);
            string status = dgvInvoices.Rows[e.RowIndex].Cells["Trạng thái"].Value?.ToString();

            if (e.ColumnIndex == dgvInvoices.Columns["View"].Index)
                ShowInvoiceDetail(invoiceId);
            else if (e.ColumnIndex == dgvInvoices.Columns["Pay"].Index)
            {
                if (status == "Đã thanh toán")
                {
                    MessageBoxHelper.ShowWarning("Hóa đơn này đã được thanh toán!");
                    return;
                }
                PayInvoice(invoiceId);
            }
        }

        private void ShowInvoiceDetail(int invoiceId)
        {
            MessageBoxHelper.ShowInfo($"Chi tiết hóa đơn #{invoiceId}\n\nChức năng đang phát triển...");
        }

        private void PayInvoice(int invoiceId)
        {
            decimal amount = Convert.ToDecimal(dgvInvoices.CurrentRow.Cells["Tổng tiền"].Value);

            if (!MessageBoxHelper.ShowConfirm($"Xác nhận thanh toán {Formatter.FormatCurrency(amount)}?"))
                return;

            try
            {
                string query = "UPDATE Invoice SET status = N'paid' WHERE invoice_id = @id";
                SqlParameter[] parameters = { new SqlParameter("@id", invoiceId) };

                int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Thanh toán thành công!");
                    Logger.LogPayment(invoiceId, amount);
                    LoadInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }
    }
}