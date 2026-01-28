using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests
{
    /// <summary>
    /// Demo form to test UI/UX enhancements
    /// Run this to see all improvements in action
    /// </summary>
    public partial class UIEnhancementDemo : Form
    {
        private ValidationHelper validationHelper;
        private LoadingHelper loadingHelper;
        private ResponsiveHelper responsiveHelper;

        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAge;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private ComboBox cmbGender;
        private DateTimePicker dtpBirthDate;
        private Button btnValidate;
        private Button btnLoadingSimple;
        private Button btnLoadingProgress;
        private Button btnLoadingCancel;
        private DataGridView dgvSample;
        private Panel panelValidation;
        private Panel panelLoading;
        private Panel panelDataGrid;

        public UIEnhancementDemo()
        {
            InitializeForm();
            InitializeHelpers();
            SetupValidationDemo();
            SetupLoadingDemo();
            SetupDataGridDemo();
        }

        #region Form Initialization

        private void InitializeForm()
        {
            // Form settings
            this.Text = "UI/UX Enhancement Demo";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9);

            // Create panels
            panelValidation = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(480, 300),
                BorderStyle = BorderStyle.FixedSingle
            };

            panelLoading = new Panel
            {
                Location = new Point(500, 10),
                Size = new Size(480, 300),
                BorderStyle = BorderStyle.FixedSingle
            };

            panelDataGrid = new Panel
            {
                Location = new Point(10, 320),
                Size = new Size(970, 340),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(panelValidation);
            this.Controls.Add(panelLoading);
            this.Controls.Add(panelDataGrid);

            // Add headers
            Label lblValidation = new Label
            {
                Text = "1. VALIDATION WITH ERROR PROVIDER",
                Location = new Point(10, 10),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            panelValidation.Controls.Add(lblValidation);

            Label lblLoading = new Label
            {
                Text = "2. LOADING INDICATORS",
                Location = new Point(10, 10),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            panelLoading.Controls.Add(lblLoading);

            Label lblDataGrid = new Label
            {
                Text = "3. DATAGRIDVIEW ENHANCEMENTS",
                Location = new Point(10, 10),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            panelDataGrid.Controls.Add(lblDataGrid);
        }

        private void InitializeHelpers()
        {
            validationHelper = new ValidationHelper(this);
            loadingHelper = new LoadingHelper(this);
            responsiveHelper = new ResponsiveHelper(this);
            responsiveHelper.SetMinimumSize(800, 600);
        }

        #endregion

        #region Validation Demo

        private void SetupValidationDemo()
        {
            int y = 45;
            int spacing = 35;

            // Name
            AddLabel(panelValidation, "Họ tên:", 10, y);
            txtName = AddTextBox(panelValidation, 120, y, 250);
            validationHelper.SetupRealtimeValidation(txtName, tb => validationHelper.ValidateRequired(tb, "Họ tên"));

            y += spacing;

            // Email
            AddLabel(panelValidation, "Email:", 10, y);
            txtEmail = AddTextBox(panelValidation, 120, y, 250);
            validationHelper.SetupRealtimeValidation(txtEmail, tb => validationHelper.ValidateEmail(tb));

            y += spacing;

            // Phone
            AddLabel(panelValidation, "Số điện thoại:", 10, y);
            txtPhone = AddTextBox(panelValidation, 120, y, 250);
            validationHelper.SetupRealtimeValidation(txtPhone, tb => validationHelper.ValidatePhone(tb));

            y += spacing;

            // Age
            AddLabel(panelValidation, "Tuổi:", 10, y);
            txtAge = AddTextBox(panelValidation, 120, y, 100);
            validationHelper.SetupNumericInput(txtAge, allowDecimal: false);
            validationHelper.SetupRealtimeValidation(txtAge, tb => validationHelper.ValidateNumeric(tb, "Tuổi", 0, 150));

            y += spacing;

            // Gender
            AddLabel(panelValidation, "Giới tính:", 10, y);
            cmbGender = AddComboBox(panelValidation, 120, y, 150);
            cmbGender.Items.AddRange(new[] { "Nam", "Nữ", "Khác" });

            y += spacing;

            // Birth Date
            AddLabel(panelValidation, "Ngày sinh:", 10, y);
            dtpBirthDate = AddDateTimePicker(panelValidation, 120, y, 200);

            y += spacing;

            // Password
            AddLabel(panelValidation, "Mật khẩu:", 10, y);
            txtPassword = AddTextBox(panelValidation, 120, y, 250);
            txtPassword.UseSystemPasswordChar = true;

            y += spacing;

            // Confirm Password
            AddLabel(panelValidation, "Xác nhận:", 10, y);
            txtConfirmPassword = AddTextBox(panelValidation, 120, y, 250);
            txtConfirmPassword.UseSystemPasswordChar = true;

            y += spacing + 10;

            // Validate button
            btnValidate = new Button
            {
                Text = "Validate Form",
                Location = new Point(120, y),
                Size = new Size(150, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnValidate.Click += BtnValidate_Click;
            panelValidation.Controls.Add(btnValidate);
        }

        private void BtnValidate_Click(object sender, EventArgs e)
        {
            bool isValid = validationHelper.ValidateForm(
                () => validationHelper.ValidateRequired(txtName, "Họ tên"),
                () => validationHelper.ValidateEmail(txtEmail),
                () => validationHelper.ValidatePhone(txtPhone),
                () => validationHelper.ValidateNumeric(txtAge, "Tuổi", 0, 150),
                () => validationHelper.ValidateRequired(cmbGender, "Giới tính"),
                () => validationHelper.ValidateAge(dtpBirthDate, "Ngày sinh", 0, 150),
                () => validationHelper.ValidatePassword(txtPassword, out string _),
                () => validationHelper.ValidatePasswordMatch(txtPassword, txtConfirmPassword)
            );

            if (isValid)
            {
                MessageBoxHelper.ShowSuccess("✅ Tất cả các trường hợp lệ!");
            }
        }

        #endregion

        #region Loading Demo

        private void SetupLoadingDemo()
        {
            int y = 45;
            int spacing = 45;

            // Simple loading
            btnLoadingSimple = new Button
            {
                Text = "Simple Loading (3s)",
                Location = new Point(20, y),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadingSimple.Click += BtnLoadingSimple_Click;
            panelLoading.Controls.Add(btnLoadingSimple);

            y += spacing;

            // Progress loading
            btnLoadingProgress = new Button
            {
                Text = "Loading with Progress (5s)",
                Location = new Point(20, y),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadingProgress.Click += BtnLoadingProgress_Click;
            panelLoading.Controls.Add(btnLoadingProgress);

            y += spacing;

            // Cancelable loading
            btnLoadingCancel = new Button
            {
                Text = "Cancelable Loading (10s)",
                Location = new Point(20, y),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadingCancel.Click += BtnLoadingCancel_Click;
            panelLoading.Controls.Add(btnLoadingCancel);

            y += spacing + 10;

            // Info label
            Label lblInfo = new Label
            {
                Text = "Click các button để xem loading indicators.\n" +
                       "Form sẽ bị disable khi loading.\n" +
                       "Spinner sẽ hiển thị với animation.\n" +
                       "Progress bar cho long operations.",
                Location = new Point(20, y),
                Size = new Size(430, 80),
                Font = new Font("Segoe UI", 8, FontStyle.Italic)
            };
            panelLoading.Controls.Add(lblInfo);
        }

        private async void BtnLoadingSimple_Click(object sender, EventArgs e)
        {
            await loadingHelper.ExecuteWithLoadingAsync(
                async () =>
                {
                    await Task.Delay(3000); // Simulate 3 second operation
                },
                "Đang tải dữ liệu..."
            );

            MessageBoxHelper.ShowSuccess("Loading hoàn tất!");
        }

        private async void BtnLoadingProgress_Click(object sender, EventArgs e)
        {
            await loadingHelper.ExecuteWithProgressAsync(
                async (progress, cancellationToken) =>
                {
                    for (int i = 0; i <= 100; i += 10)
                    {
                        await Task.Delay(500); // Simulate work
                        progress.Report(i);
                    }
                },
                "Đang xử lý...",
                cancelable: false
            );

            MessageBoxHelper.ShowSuccess("Xử lý hoàn tất!");
        }

        private async void BtnLoadingCancel_Click(object sender, EventArgs e)
        {
            await loadingHelper.ExecuteWithProgressAsync(
                async (progress, cancellationToken) =>
                {
                    for (int i = 0; i <= 100; i += 5)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(500); // Simulate work
                        progress.Report(i);
                    }
                },
                "Đang tải... (Có thể hủy)",
                cancelable: true
            );

            MessageBoxHelper.ShowSuccess("Hoàn tất hoặc đã hủy!");
        }

        #endregion

        #region DataGrid Demo

        private void SetupDataGridDemo()
        {
            dgvSample = new DataGridView
            {
                Location = new Point(10, 40),
                Size = new Size(950, 290)
            };

            // Apply modern style
            dgvSample.ApplyModernStyle();

            // Add columns
            dgvSample.Columns.Add("id", "ID");
            dgvSample.Columns.Add("patient_name", "Tên bệnh nhân");
            dgvSample.Columns.Add("service", "Dịch vụ");
            dgvSample.Columns.Add("amount", "Số tiền");
            dgvSample.Columns.Add("date", "Ngày");
            dgvSample.Columns.Add("status", "Trạng thái");

            // Hide ID column
            dgvSample.HideColumn("id");

            // Format columns
            dgvSample.FormatCurrencyColumn("amount");
            dgvSample.FormatDateColumn("date");

            // Style status column
            dgvSample.StyleStatusColumn("status", new[]
            {
                StatusColorMap.Success("Đã thanh toán"),
                StatusColorMap.Warning("Chờ thanh toán"),
                StatusColorMap.Danger("Quá hạn"),
                StatusColorMap.Info("Đang xử lý")
            });

            // Add sample data
            AddSampleData(dgvSample);

            // Add context menu (instead of button columns)
            dgvSample.AddContextMenu(
                new ContextMenuItem
                {
                    Text = "👁 Xem chi tiết",
                    Action = row => MessageBoxHelper.ShowInfo($"Xem hóa đơn #{row.Cells["id"].Value}")
                },
                new ContextMenuItem
                {
                    Text = "💳 Thanh toán",
                    Action = row => MessageBoxHelper.ShowInfo($"Thanh toán hóa đơn #{row.Cells["id"].Value}")
                },
                ContextMenuItem.Separator(),
                new ContextMenuItem
                {
                    Text = "📄 In hóa đơn",
                    Action = row => MessageBoxHelper.ShowInfo($"In hóa đơn #{row.Cells["id"].Value}")
                },
                new ContextMenuItem
                {
                    Text = "❌ Hủy hóa đơn",
                    Action = row =>
                    {
                        if (MessageBoxHelper.ShowConfirm("Bạn có chắc muốn hủy hóa đơn này?"))
                        {
                            MessageBoxHelper.ShowSuccess("Đã hủy hóa đơn!");
                        }
                    }
                }
            );

            panelDataGrid.Controls.Add(dgvSample);
        }

        private void AddSampleData(DataGridView dgv)
        {
            dgv.Rows.Add(1, "Nguyễn Văn A", "Khám tổng quát", 500000, DateTime.Now.AddDays(-5), "Đã thanh toán");
            dgv.Rows.Add(2, "Trần Thị B", "Nhổ răng khôn", 1500000, DateTime.Now.AddDays(-3), "Chờ thanh toán");
            dgv.Rows.Add(3, "Lê Văn C", "Trám răng", 300000, DateTime.Now.AddDays(-10), "Quá hạn");
            dgv.Rows.Add(4, "Phạm Thị D", "Tẩy trắng răng", 2000000, DateTime.Now.AddDays(-1), "Đang xử lý");
            dgv.Rows.Add(5, "Hoàng Văn E", "Niềng răng", 35000000, DateTime.Now.AddDays(-2), "Đã thanh toán");
            dgv.Rows.Add(6, "Võ Thị F", "Cấy implant", 15000000, DateTime.Now, "Chờ thanh toán");
        }

        #endregion

        #region Helper Methods

        private Label AddLabel(Panel panel, string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panel.Controls.Add(label);
            return label;
        }

        private TextBox AddTextBox(Panel panel, int x, int y, int width)
        {
            TextBox textBox = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Font = new Font("Segoe UI", 9)
            };
            panel.Controls.Add(textBox);
            return textBox;
        }

        private ComboBox AddComboBox(Panel panel, int x, int y, int width)
        {
            ComboBox comboBox = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            panel.Controls.Add(comboBox);
            return comboBox;
        }

        private DateTimePicker AddDateTimePicker(Panel panel, int x, int y, int width)
        {
            DateTimePicker dtp = new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(width, 25),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9)
            };
            panel.Controls.Add(dtp);
            return dtp;
        }

        #endregion

        #region Main Entry Point

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UIEnhancementDemo());
        }

        #endregion
    }
}
