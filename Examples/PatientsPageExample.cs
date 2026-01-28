using System;
using System.Collections.Generic;
using System.Data;
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
    /// Example implementation of BaseDataPage for Patients
    /// Shows how to eliminate code duplication using base class
    /// </summary>
    public partial class PatientsPageExample : BaseDataPage<Patient>
    {
        private DatabaseHelper dbHelper;

        public PatientsPageExample()
        {
            dbHelper = new DatabaseHelper();
            
            // Base class handles all initialization!
            // - DataGridView setup
            // - Search box
            // - Buttons (Add, Refresh, Export)
            // - Loading indicators
            // - Error handling
        }

        #region Override Abstract Methods

        protected override List<DataGridViewColumn> GetColumns()
        {
            // Use template factory methods instead of manual configuration
            return new List<DataGridViewColumn>
            {
                DataGridViewTemplate.CreateIdColumn(),
                DataGridViewTemplate.CreateTextColumn("full_name", "Họ tên", 200),
                DataGridViewTemplate.CreateTextColumn("phone", "Số điện thoại", 120),
                DataGridViewTemplate.CreateTextColumn("email", "Email", 200),
                DataGridViewTemplate.CreateDateColumn("date_of_birth", "Ngày sinh", 100),
                DataGridViewTemplate.CreateTextColumn("gender", "Giới tính", 80),
                DataGridViewTemplate.CreateTextColumn("address", "Địa chỉ")
            };

            // BEFORE: 50+ lines of manual column configuration
            // AFTER: 7 lines with template methods!
        }

        protected override async Task<List<Patient>> LoadDataAsync()
        {
            // Use QueryOptimizer to prevent N+1 problems
            return await CodeTemplates.ExecuteWithResult(
                async () =>
                {
                    var data = await dbHelper.ExecuteQueryAsync<Patient>(
                        "SELECT * FROM Patients ORDER BY full_name"
                    );
                    return data.ToList();
                },
                new List<Patient>(),
                "Không thể tải danh sách bệnh nhân"
            );

            // BEFORE: Manual try-catch, error handling, loading indicator
            // AFTER: CodeTemplates handles everything!
        }

        protected override object[] EntityToRow(Patient patient)
        {
            return new object[]
            {
                patient.id,
                patient.full_name,
                patient.phone,
                patient.email,
                patient.date_of_birth,
                patient.gender,
                patient.address
            };
        }

        protected override Patient RowToEntity(DataGridViewRow row)
        {
            return new Patient
            {
                id = Convert.ToInt32(row.Cells["id"].Value),
                full_name = row.Cells["full_name"].Value?.ToString(),
                phone = row.Cells["phone"].Value?.ToString(),
                email = row.Cells["email"].Value?.ToString(),
                date_of_birth = Convert.ToDateTime(row.Cells["date_of_birth"].Value),
                gender = row.Cells["gender"].Value?.ToString(),
                address = row.Cells["address"].Value?.ToString()
            };
        }

        protected override string GetCacheKey()
        {
            return "patients_all";
        }

        #endregion

        #region Override Virtual Methods (Optional)

        protected override bool FilterEntity(Patient patient, string searchText)
        {
            // Custom search logic
            searchText = searchText.ToLower();
            return patient.full_name?.ToLower().Contains(searchText) == true ||
                   patient.phone?.Contains(searchText) == true ||
                   patient.email?.ToLower().Contains(searchText) == true;
        }

        protected override async Task AddAsync()
        {
            // Open add form
            var addForm = new PatientAddEditForm();
            
            if (await CodeTemplates.OpenDialogAndRefresh(addForm, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Đã thêm bệnh nhân mới!");
            }

            // BEFORE: Manual form.ShowDialog(), check result, refresh
            // AFTER: 3 lines with CodeTemplates.OpenDialogAndRefresh!
        }

        protected override async Task EditAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Vui lòng chọn bệnh nhân cần sửa");
                return;
            }

            var editForm = new PatientAddEditForm(selectedItem);
            
            if (await CodeTemplates.OpenDialogAndRefresh(editForm, RefreshAsync))
            {
                MessageBoxHelper.ShowSuccess("Đã cập nhật thông tin bệnh nhân!");
            }
        }

        protected override async Task DeleteAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Vui lòng chọn bệnh nhân cần xóa");
                return;
            }

            if (!CodeTemplates.ConfirmDelete($"bệnh nhân '{selectedItem.full_name}'"))
            {
                return;
            }

            await CodeTemplates.ExecuteAsync(
                async () =>
                {
                    await dbHelper.ExecuteNonQueryAsync(
                        "DELETE FROM Patients WHERE id = @id",
                        new { id = selectedItem.id }
                    );

                    await RefreshAsync();
                    MessageBoxHelper.ShowSuccess("Đã xóa bệnh nhân!");

                    // Invalidate cache
                    cacheManager.InvalidateRelated("patient");
                },
                "Không thể xóa bệnh nhân"
            );

            // BEFORE: 15+ lines (try-catch, confirmation, delete, refresh, error)
            // AFTER: 5 lines with CodeTemplates!
        }

        #endregion

        #region Custom Methods (If Needed)

        // Add any custom methods specific to patients
        public async Task ViewMedicalHistoryAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Vui lòng chọn bệnh nhân");
                return;
            }

            // Custom logic here
        }

        #endregion
    }

    /// <summary>
    /// Add/Edit form for Patient
    /// Also uses templates to reduce duplication
    /// </summary>
    public partial class PatientAddEditForm : Form
    {
        private Patient patient;
        private bool isEditMode;
        private ValidationHelper validationHelper;
        private DatabaseHelper dbHelper;

        // Controls
        private TextBox txtFullName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private DateTimePicker dtpBirthDate;
        private ComboBox cmbGender;
        private TextBox txtAddress;
        private Button btnSave;
        private Button btnCancel;

        public PatientAddEditForm(Patient patient = null)
        {
            this.patient = patient;
            this.isEditMode = patient != null;

            InitializeComponent();
            InitializeHelpers();
            
            if (isEditMode)
            {
                LoadPatientData();
            }
        }

        private void InitializeComponent()
        {
            // Use CodeTemplates for initialization
            CodeTemplates.InitializeForm(
                this,
                isEditMode ? "Sửa thông tin bệnh nhân" : "Thêm bệnh nhân mới",
                600,
                450
            );

            // Create controls (simplified example)
            int y = 20;
            int spacing = 50;

            // Full name
            AddLabel("Họ tên:", 20, y);
            txtFullName = AddTextBox(150, y, 400);
            y += spacing;

            // Phone
            AddLabel("Số điện thoại:", 20, y);
            txtPhone = AddTextBox(150, y, 200);
            y += spacing;

            // Email
            AddLabel("Email:", 20, y);
            txtEmail = AddTextBox(150, y, 300);
            y += spacing;

            // Birth date
            AddLabel("Ngày sinh:", 20, y);
            dtpBirthDate = new DateTimePicker
            {
                Location = new System.Drawing.Point(150, y),
                Size = new System.Drawing.Size(200, 25),
                Format = DateTimePickerFormat.Short
            };
            this.Controls.Add(dtpBirthDate);
            y += spacing;

            // Gender
            AddLabel("Giới tính:", 20, y);
            cmbGender = new ComboBox
            {
                Location = new System.Drawing.Point(150, y),
                Size = new System.Drawing.Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new[] { "Nam", "Nữ", "Khác" });
            this.Controls.Add(cmbGender);
            y += spacing;

            // Address
            AddLabel("Địa chỉ:", 20, y);
            txtAddress = AddTextBox(150, y, 400);
            txtAddress.Multiline = true;
            txtAddress.Height = 60;
            y += 80;

            // Buttons
            btnSave = new Button
            {
                Text = "💾 Lưu",
                Location = new System.Drawing.Point(200, y),
                Size = new System.Drawing.Size(100, 35),
                DialogResult = DialogResult.OK,
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += async (s, e) => await SaveAsync();
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "❌ Hủy",
                Location = new System.Drawing.Point(310, y),
                Size = new System.Drawing.Size(100, 35),
                DialogResult = DialogResult.Cancel,
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private Label AddLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y + 3),
                Size = new System.Drawing.Size(120, 20)
            };
            this.Controls.Add(label);
            return label;
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            var textBox = new TextBox
            {
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(width, 25)
            };
            this.Controls.Add(textBox);
            return textBox;
        }

        private void InitializeHelpers()
        {
            validationHelper = new ValidationHelper(this);
            dbHelper = new DatabaseHelper();

            // Setup validation using templates
            CodeTemplates.SetupRealtimeValidation(
                validationHelper,
                (txtFullName, tb => validationHelper.ValidateRequired(tb, "Họ tên")),
                (txtPhone, tb => validationHelper.ValidatePhone(tb)),
                (txtEmail, tb => validationHelper.ValidateEmail(tb))
            );

            // BEFORE: Manual validation setup for each field (10+ lines)
            // AFTER: 1 line with CodeTemplates.SetupRealtimeValidation!
        }

        private void LoadPatientData()
        {
            if (patient == null) return;

            txtFullName.Text = patient.full_name;
            txtPhone.Text = patient.phone;
            txtEmail.Text = patient.email;
            dtpBirthDate.Value = patient.date_of_birth;
            cmbGender.SelectedItem = patient.gender;
            txtAddress.Text = patient.address;
        }

        private async Task SaveAsync()
        {
            // Use template for save with validation
            bool success = await CodeTemplates.SaveWithValidationAsync(
                validationHelper,
                ValidateForm,
                SaveToDatabase,
                successMessage: isEditMode ? "Cập nhật thành công!" : "Thêm mới thành công!",
                errorMessage: "Không thể lưu thông tin bệnh nhân"
            );

            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            // BEFORE: 20+ lines (validate, try-catch, save, message, close)
            // AFTER: 5 lines with CodeTemplates.SaveWithValidationAsync!
        }

        private bool ValidateForm()
        {
            return validationHelper.ValidateForm(
                () => validationHelper.ValidateRequired(txtFullName, "Họ tên"),
                () => validationHelper.ValidatePhone(txtPhone),
                () => validationHelper.ValidateEmail(txtEmail),
                () => validationHelper.ValidateRequired(cmbGender, "Giới tính"),
                () => validationHelper.ValidateAge(dtpBirthDate, "Ngày sinh", 0, 150)
            );
        }

        private async Task SaveToDatabase()
        {
            if (isEditMode)
            {
                // Update
                await dbHelper.ExecuteNonQueryAsync(@"
                    UPDATE Patients SET
                        full_name = @full_name,
                        phone = @phone,
                        email = @email,
                        date_of_birth = @date_of_birth,
                        gender = @gender,
                        address = @address
                    WHERE id = @id",
                    new
                    {
                        id = patient.id,
                        full_name = txtFullName.Text.Trim(),
                        phone = txtPhone.Text.Trim(),
                        email = txtEmail.Text.Trim(),
                        date_of_birth = dtpBirthDate.Value,
                        gender = cmbGender.SelectedItem.ToString(),
                        address = txtAddress.Text.Trim()
                    });
            }
            else
            {
                // Insert
                await dbHelper.ExecuteNonQueryAsync(@"
                    INSERT INTO Patients (full_name, phone, email, date_of_birth, gender, address)
                    VALUES (@full_name, @phone, @email, @date_of_birth, @gender, @address)",
                    new
                    {
                        full_name = txtFullName.Text.Trim(),
                        phone = txtPhone.Text.Trim(),
                        email = txtEmail.Text.Trim(),
                        date_of_birth = dtpBirthDate.Value,
                        gender = cmbGender.SelectedItem.ToString(),
                        address = txtAddress.Text.Trim()
                    });
            }

            // Invalidate cache
            CacheManager.Instance.InvalidateRelated("patient");
        }
    }
}
