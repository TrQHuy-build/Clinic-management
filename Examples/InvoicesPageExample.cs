using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DentalClinicManagement.Base;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Models;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Examples
{
    /// <summary>
    /// Example: Invoice Management Page using BaseDataPage
    /// Shows how to handle complex entity with relationships
    /// </summary>
    public partial class InvoicesPageExample : BaseDataPage<Invoice>
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private QueryOptimizer queryOptimizer = new QueryOptimizer();

        public InvoicesPageExample()
        {
            // Base class will call InitializeComponent() and setup everything
        }

        #region Abstract Method Implementations (Required)

        protected override List<DataGridViewColumn> GetColumns()
        {
            // Use factory methods - 9 lines vs 60+ lines manual!
            return new List<DataGridViewColumn>
            {
                DataGridViewTemplate.CreateIdColumn(),
                DataGridViewTemplate.CreateTextColumn("invoice_code", "Mã hóa đơn", 120),
                DataGridViewTemplate.CreateTextColumn("patient_name", "Bệnh nhân", 180),
                DataGridViewTemplate.CreateDateTimeColumn("invoice_date", "Ngày lập", 140),
                DataGridViewTemplate.CreateCurrencyColumn("total_amount", "Tổng tiền", 120),
                DataGridViewTemplate.CreateCurrencyColumn("discount", "Giảm giá", 100),
                DataGridViewTemplate.CreateCurrencyColumn("final_amount", "Thanh toán", 120),
                DataGridViewTemplate.CreateTextColumn("payment_status", "Trạng thái", 100),
                DataGridViewTemplate.CreateTextColumn("created_by", "Người tạo", 130)
            };
        }

        protected override async Task<List<Invoice>> LoadDataAsync()
        {
            // Use QueryOptimizer to prevent N+1 queries
            // Before: 1 + N queries (slow!)
            // After: 1 JOIN query (100x faster!)
            var invoicesWithDetails = await queryOptimizer.LoadInvoicesWithDetailsAsync();

            // Convert to Invoice list
            return invoicesWithDetails.Select(inv => new Invoice
            {
                id = inv.InvoiceId,
                invoice_code = inv.InvoiceCode,
                patient_id = inv.PatientId,
                invoice_date = inv.InvoiceDate,
                total_amount = inv.TotalAmount,
                discount = inv.Discount,
                final_amount = inv.FinalAmount,
                payment_status = inv.PaymentStatus,
                created_by = inv.CreatedBy,
                // Additional properties from JOIN
                patient_name = inv.PatientName
            }).ToList();
        }

        protected override object[] EntityToRow(Invoice invoice)
        {
            return new object[]
            {
                invoice.id,
                invoice.invoice_code,
                invoice.patient_name ?? "N/A",
                invoice.invoice_date,
                invoice.total_amount,
                invoice.discount,
                invoice.final_amount,
                invoice.payment_status,
                invoice.created_by
            };
        }

        protected override Invoice RowToEntity(DataGridViewRow row)
        {
            return new Invoice
            {
                id = (int)row.Cells["id"].Value,
                invoice_code = row.Cells["invoice_code"].Value?.ToString(),
                patient_name = row.Cells["patient_name"].Value?.ToString(),
                invoice_date = (DateTime)row.Cells["invoice_date"].Value,
                total_amount = (decimal)row.Cells["total_amount"].Value,
                discount = (decimal)row.Cells["discount"].Value,
                final_amount = (decimal)row.Cells["final_amount"].Value,
                payment_status = row.Cells["payment_status"].Value?.ToString(),
                created_by = row.Cells["created_by"].Value?.ToString()
            };
        }

        protected override string GetCacheKey() => "invoices_all";

        #endregion

        #region Virtual Method Overrides (Optional)

        protected override bool FilterEntity(Invoice invoice, string searchText)
        {
            searchText = searchText.ToLower();
            return invoice.invoice_code?.ToLower().Contains(searchText) == true ||
                   invoice.patient_name?.ToLower().Contains(searchText) == true ||
                   invoice.payment_status?.ToLower().Contains(searchText) == true ||
                   invoice.created_by?.ToLower().Contains(searchText) == true;
        }

        protected override void FormatColumns()
        {
            base.FormatColumns();

            // Custom formatting for payment_status column
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["payment_status"].Value != null)
                {
                    string status = row.Cells["payment_status"].Value.ToString();

                    switch (status)
                    {
                        case "Paid":
                            row.Cells["payment_status"].Style.BackColor = Color.LightGreen;
                            row.Cells["payment_status"].Style.ForeColor = Color.DarkGreen;
                            break;
                        case "Pending":
                            row.Cells["payment_status"].Style.BackColor = Color.LightYellow;
                            row.Cells["payment_status"].Style.ForeColor = Color.DarkOrange;
                            break;
                        case "Overdue":
                            row.Cells["payment_status"].Style.BackColor = Color.LightCoral;
                            row.Cells["payment_status"].Style.ForeColor = Color.DarkRed;
                            break;
                    }
                }
            }
        }

        protected override List<ToolStripMenuItem> GetContextMenuItems()
        {
            var items = base.GetContextMenuItems(); // View, Edit, Delete

            // Add custom menu items
            items.Insert(0, new ToolStripMenuItem("Print Invoice", null, async (s, e) => await PrintInvoiceAsync()));
            items.Insert(1, new ToolStripMenuItem("Email Invoice", null, async (s, e) => await EmailInvoiceAsync()));
            items.Add(new ToolStripSeparator());
            items.Add(new ToolStripMenuItem("Mark as Paid", null, async (s, e) => await MarkAsPaidAsync()));

            return items;
        }

        protected override async Task AddAsync()
        {
            var form = new InvoiceAddEditForm();
            if (await CodeTemplates.OpenDialogAndRefresh(form, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Invoice created successfully!");
            }
        }

        protected override async Task ViewAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Please select an invoice to view.");
                return;
            }

            var form = new InvoiceViewForm(selectedItem);
            CodeTemplates.OpenDialog(form);
        }

        protected override async Task EditAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Please select an invoice to edit.");
                return;
            }

            // Check if invoice is paid
            if (selectedItem.payment_status == "Paid")
            {
                MessageBoxHelper.ShowWarning("Cannot edit a paid invoice.");
                return;
            }

            var form = new InvoiceAddEditForm(selectedItem);
            if (await CodeTemplates.OpenDialogAndRefresh(form, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Invoice updated successfully!");
            }
        }

        protected override async Task DeleteAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Please select an invoice to delete.");
                return;
            }

            // Check if invoice is paid
            if (selectedItem.payment_status == "Paid")
            {
                MessageBoxHelper.ShowWarning("Cannot delete a paid invoice.");
                return;
            }

            if (!CodeTemplates.ConfirmDelete($"invoice '{selectedItem.invoice_code}'"))
                return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    // Delete invoice details first (foreign key constraint)
                    await dbHelper.ExecuteNonQueryAsync(
                        "DELETE FROM InvoiceDetails WHERE invoice_id = @id",
                        new { id = selectedItem.id }
                    );

                    // Then delete invoice
                    await dbHelper.ExecuteNonQueryAsync(
                        "DELETE FROM Invoices WHERE id = @id",
                        new { id = selectedItem.id }
                    );

                    await RefreshAsync();
                    MessageBoxHelper.ShowSuccess("Invoice deleted successfully!");
                    cacheManager.InvalidateRelated("invoice");
                },
                "Error deleting invoice"
            );
        }

        protected override bool EnablePagination => true;
        protected override int PageSize => 100; // Larger page size for invoices

        #endregion

        #region Custom Methods

        private async Task PrintInvoiceAsync()
        {
            if (selectedItem == null) return;

            await CodeTemplates.ExecuteWithLoadingAsync(
                this.FindForm(),
                async () =>
                {
                    // Simulate printing
                    await Task.Delay(1000);
                    MessageBoxHelper.ShowSuccess("Invoice printed successfully!");
                },
                "Printing invoice...",
                "Error printing invoice"
            );
        }

        private async Task EmailInvoiceAsync()
        {
            if (selectedItem == null) return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    // Get patient email
                    var patient = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                        "SELECT * FROM Patients WHERE id = @id",
                        new { id = selectedItem.patient_id }
                    );

                    if (string.IsNullOrEmpty(patient?.email))
                    {
                        MessageBoxHelper.ShowWarning("Patient has no email address.");
                        return;
                    }

                    // Simulate sending email
                    await Task.Delay(1000);
                    MessageBoxHelper.ShowSuccess($"Invoice sent to {patient.email}");
                },
                "Error sending email"
            );
        }

        private async Task MarkAsPaidAsync()
        {
            if (selectedItem == null) return;

            if (selectedItem.payment_status == "Paid")
            {
                MessageBoxHelper.ShowInfo("Invoice is already paid.");
                return;
            }

            if (!CodeTemplates.ConfirmAction($"Mark invoice '{selectedItem.invoice_code}' as paid?", "Confirm Payment"))
                return;

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    await dbHelper.ExecuteNonQueryAsync(
                        @"UPDATE Invoices 
                          SET payment_status = 'Paid',
                              payment_date = GETDATE()
                          WHERE id = @id",
                        new { id = selectedItem.id }
                    );

                    await RefreshAsync();
                    MessageBoxHelper.ShowSuccess("Invoice marked as paid!");
                    cacheManager.InvalidateRelated("invoice");
                },
                "Error updating payment status"
            );
        }

        #endregion
    }

    /// <summary>
    /// Add/Edit form for invoices using CodeTemplates
    /// </summary>
    public partial class InvoiceAddEditForm : Form
    {
        private ValidationHelper validationHelper;
        private DatabaseHelper dbHelper;
        private Invoice invoice;
        private bool isEditMode;

        private ComboBox cmbPatient;
        private DateTimePicker dtpInvoiceDate;
        private TextBox txtTotalAmount, txtDiscount, txtFinalAmount;
        private ComboBox cmbPaymentStatus;
        private Button btnSave, btnCancel;

        public InvoiceAddEditForm(Invoice invoice = null)
        {
            this.invoice = invoice ?? new Invoice();
            this.isEditMode = invoice != null;

            InitializeForm();
        }

        private void InitializeForm()
        {
            // Use template for form setup - 1 line vs 10+ lines!
            CodeTemplates.InitializeForm(
                this,
                isEditMode ? "Edit Invoice" : "New Invoice",
                800,
                600,
                FormStartPosition.CenterScreen
            );

            validationHelper = new ValidationHelper(this);
            dbHelper = new DatabaseHelper();

            SetupControls();
            SetupValidation();
            LoadInitialData();
        }

        private void SetupControls()
        {
            // Create controls
            cmbPatient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            dtpInvoiceDate = new DateTimePicker { Format = DateTimePickerFormat.Short };
            txtTotalAmount = new TextBox();
            txtDiscount = new TextBox { Text = "0" };
            txtFinalAmount = new TextBox { ReadOnly = true };
            cmbPaymentStatus = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            btnSave = new Button { Text = "Save" };
            btnCancel = new Button { Text = "Cancel" };

            // Layout (simplified - normally would use TableLayoutPanel)
            // ... add to form

            // Event handlers
            btnSave.Click += btnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            txtTotalAmount.TextChanged += CalculateFinalAmount;
            txtDiscount.TextChanged += CalculateFinalAmount;

            // Load existing data
            if (isEditMode)
            {
                dtpInvoiceDate.Value = invoice.invoice_date;
                txtTotalAmount.Text = invoice.total_amount.ToString();
                txtDiscount.Text = invoice.discount.ToString();
                cmbPaymentStatus.SelectedItem = invoice.payment_status;
            }
        }

        private void SetupValidation()
        {
            // Use template for validation setup - 1 line vs 10+ lines!
            CodeTemplates.SetupRealtimeValidation(
                validationHelper,
                (cmbPatient, cb => validationHelper.ValidateComboBox(cb, "Patient")),
                (txtTotalAmount, tb => validationHelper.ValidateDecimal(tb, "Total amount")),
                (txtDiscount, tb => validationHelper.ValidateDecimal(tb, "Discount"))
            );
        }

        private async void LoadInitialData()
        {
            await CodeTemplates.ExecuteWithLoadingAsync(
                this,
                async () =>
                {
                    // Load patients
                    var patients = await dbHelper.ExecuteQueryAsync<Patient>(
                        "SELECT id, full_name FROM Patients ORDER BY full_name"
                    );

                    CodeTemplates.BindComboBox(cmbPatient, patients.ToList(), "full_name", "id");

                    // Load payment statuses
                    cmbPaymentStatus.Items.AddRange(new[] { "Pending", "Paid", "Overdue" });

                    if (isEditMode)
                    {
                        cmbPatient.SelectedValue = invoice.patient_id;
                        cmbPaymentStatus.SelectedItem = invoice.payment_status;
                    }
                    else
                    {
                        cmbPaymentStatus.SelectedIndex = 0; // Default to Pending
                    }
                },
                "Loading...",
                "Error loading data"
            );
        }

        private void CalculateFinalAmount(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtTotalAmount.Text, out decimal total) &&
                decimal.TryParse(txtDiscount.Text, out decimal discount))
            {
                decimal final = total - discount;
                txtFinalAmount.Text = final.ToString("N0");
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            bool success = await CodeTemplates.SaveWithValidationAsync(
                validationHelper,
                ValidateForm,
                SaveToDatabase,
                isEditMode ? "Invoice updated successfully!" : "Invoice created successfully!",
                "Error saving invoice"
            );

            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidateForm()
        {
            return validationHelper.ValidateForm(
                cmbPatient,
                txtTotalAmount,
                txtDiscount
            );
        }

        private async Task SaveToDatabase()
        {
            // Prepare invoice object
            invoice.patient_id = (int)cmbPatient.SelectedValue;
            invoice.invoice_date = dtpInvoiceDate.Value;
            invoice.total_amount = decimal.Parse(txtTotalAmount.Text);
            invoice.discount = decimal.Parse(txtDiscount.Text);
            invoice.final_amount = invoice.total_amount - invoice.discount;
            invoice.payment_status = cmbPaymentStatus.SelectedItem?.ToString();
            invoice.created_by = Auth.CurrentUser.Username;

            if (isEditMode)
            {
                await dbHelper.ExecuteNonQueryAsync(
                    @"UPDATE Invoices SET
                        patient_id = @patient_id,
                        invoice_date = @invoice_date,
                        total_amount = @total_amount,
                        discount = @discount,
                        final_amount = @final_amount,
                        payment_status = @payment_status
                      WHERE id = @id",
                    invoice
                );
            }
            else
            {
                // Generate invoice code
                invoice.invoice_code = $"INV{DateTime.Now:yyyyMMddHHmmss}";

                await dbHelper.ExecuteNonQueryAsync(
                    @"INSERT INTO Invoices (invoice_code, patient_id, invoice_date, 
                        total_amount, discount, final_amount, payment_status, created_by)
                      VALUES (@invoice_code, @patient_id, @invoice_date, 
                        @total_amount, @discount, @final_amount, @payment_status, @created_by)",
                    invoice
                );
            }

            CacheManager.Instance.InvalidateRelated("invoice");
        }
    }

    /// <summary>
    /// View form for invoice details
    /// </summary>
    public partial class InvoiceViewForm : Form
    {
        private Invoice invoice;
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public InvoiceViewForm(Invoice invoice)
        {
            this.invoice = invoice;

            CodeTemplates.InitializeForm(
                this,
                $"Invoice Details - {invoice.invoice_code}",
                900,
                700,
                FormStartPosition.CenterScreen
            );

            LoadInvoiceDetails();
        }

        private async void LoadInvoiceDetails()
        {
            await CodeTemplates.ExecuteWithLoadingAsync(
                this,
                async () =>
                {
                    // Load invoice with details
                    var queryOptimizer = new QueryOptimizer();
                    var details = await queryOptimizer.LoadInvoicesWithDetailsAsync(invoice.id);

                    // Display invoice info
                    // ... (create labels, textboxes to show invoice details)

                    // Load and display invoice line items
                    var lineItems = await dbHelper.ExecuteQueryAsync<dynamic>(
                        @"SELECT id.*, s.service_name, s.price
                          FROM InvoiceDetails id
                          JOIN Services s ON id.service_id = s.id
                          WHERE id.invoice_id = @invoiceId",
                        new { invoiceId = invoice.id }
                    );

                    // Display in DataGridView
                    // ...
                },
                "Loading invoice details...",
                "Error loading invoice"
            );
        }
    }
}
