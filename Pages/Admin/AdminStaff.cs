using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DentalClinicManagement.Pages.Admin
{
    public partial class AdminStaff : UserControl
    {
        public AdminStaff()
        {
            InitializeComponent();
            InitializeComboBoxes();
            LoadStaff();
        }

        private void InitializeComboBoxes()
        {
            // Khởi tạo ComboBox Position filter
            if (cboPosition != null)
            {
                cboPosition.Items.Clear();
                cboPosition.Items.AddRange(new object[] { "Tất cả", "Doctor", "Staff" });
                cboPosition.SelectedIndex = 0;
            }
        }

        private void LoadStaff()
        {
            try
            {
                string search = txtSearch?.Text?.Trim() ?? "";
                string position = cboPosition?.SelectedItem?.ToString();

                string query = @"
                    SELECT
                        s.staff_id AS [ID],
                        u.fullname AS [Họ tên],
                        u.phone AS [SĐT],
                        u.email AS [Email],
                        s.position AS [Chức vụ],
                        s.specialization AS [Chuyên môn],
                        s.salary AS [Lương],
                        s.hire_date AS [Ngày vào],
                        u.status AS [Trạng thái]
                    FROM Staff s
                    INNER JOIN UserAccount u ON s.user_id = u.user_id
                    WHERE (u.fullname LIKE @search OR u.email LIKE @search OR u.phone LIKE @search)";

                List<SqlParameter> parameterList = new List<SqlParameter>
                {
                    new SqlParameter("@search", $"%{search}%")
                };

                if (position != "Tất cả" && !string.IsNullOrEmpty(position))
                {
                    query += " AND s.position = @position";
                    parameterList.Add(new SqlParameter("@position", position));
                }

                query += " ORDER BY u.fullname";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parameterList.ToArray());
                dgvStaff.DataSource = dt;

                // Ẩn ID
                if (dgvStaff.Columns["ID"] != null)
                    dgvStaff.Columns["ID"].Visible = false;

                // Format lương
                if (dgvStaff.Columns["Lương"] != null)
                {
                    dgvStaff.Columns["Lương"].DefaultCellStyle.Format = "N0";
                    dgvStaff.Columns["Lương"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                // Format ngày vào
                if (dgvStaff.Columns["Ngày vào"] != null)
                    dgvStaff.Columns["Ngày vào"].DefaultCellStyle.Format = "dd/MM/yyyy";

                // Thêm nút Edit/Delete
                AddButtonColumn("Edit", "Sửa", 70);
                AddButtonColumn("Delete", "Xóa", 70);

                // Format trạng thái và chuyên môn
                foreach (DataGridViewRow row in dgvStaff.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Format trạng thái
                    if (row.Cells["Trạng thái"].Value is string status)
                    {
                        row.Cells["Trạng thái"].Value = Formatter.FormatStatus(status);
                    }

                    // Format chuyên môn (nếu null hoặc rỗng)
                    if (row.Cells["Chuyên môn"].Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(row.Cells["Chuyên môn"].Value?.ToString()))
                    {
                        row.Cells["Chuyên môn"].Value = "N/A";
                    }

                    // ✅ FIX: Highlight lương = 0 (màu xám)
                    if (row.Cells["Lương"].Value != DBNull.Value)
                    {
                        decimal salary = Convert.ToDecimal(row.Cells["Lương"].Value);
                        if (salary == 0)
                        {
                            row.Cells["Lương"].Style.ForeColor = Color.Gray;
                            row.Cells["Lương"].Style.Font = new Font(dgvStaff.Font, FontStyle.Italic);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void AddButtonColumn(string name, string text, int width)
        {
            if (!dgvStaff.Columns.Contains(name))
            {
                dgvStaff.Columns.Add(new DataGridViewButtonColumn
                {
                    Name = name,
                    HeaderText = "",
                    Text = text,
                    UseColumnTextForButtonValue = true,
                    Width = width
                });
            }
        }

        private void DgvStaff_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int staffId = Convert.ToInt32(dgvStaff.Rows[e.RowIndex].Cells["ID"].Value);
                string name = dgvStaff.Rows[e.RowIndex].Cells["Họ tên"].Value?.ToString() ?? "";

                if (e.ColumnIndex == dgvStaff.Columns["Edit"]?.Index)
                    ShowAddEditForm(staffId);
                else if (e.ColumnIndex == dgvStaff.Columns["Delete"]?.Index)
                    DeleteStaff(staffId, name);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    btn.Enabled = false;
                    btn.Text = "Đang tải...";
                }
                LoadStaff();
                if (sender is Button btn2)
                {
                    btn2.Enabled = true;
                    btn2.Text = "🔄 Làm mới";
                }
                MessageBoxHelper.ShowInfo("Dữ liệu đã được làm mới!");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi làm mới: {ex.Message}");
                if (sender is Button btn)
                {
                    btn.Enabled = true;
                    btn.Text = "🔄 Làm mới";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ShowAddEditForm(null);
        }

        private void ShowAddEditForm(int? staffId)
        {
            using (Form form = new Form
            {
                Text = staffId.HasValue ? "Sửa nhân viên" : "Thêm nhân viên mới",
                Size = new Size(550, 730), // ✅ FIX: Tăng chiều cao cho error labels
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                // ✅ FIX: Tạo controls với spacing tốt hơn
                TextBox txtFullname = CreateTextBox(150, 30);
                Label lblFullnameError = CreateErrorLabel(150, 57);

                TextBox txtPhone = CreateTextBox(150, 90);  // ✅ FIX: Điều chỉnh Y
                Label lblPhoneError = CreateErrorLabel(150, 117);

                TextBox txtEmail = CreateTextBox(150, 150);  // ✅ FIX: Điều chỉnh Y
                Label lblEmailError = CreateErrorLabel(150, 177);

                TextBox txtPassword = CreateTextBox(150, 210);  // ✅ FIX: Điều chỉnh Y
                txtPassword.UseSystemPasswordChar = true;
                Label lblPasswordError = CreateErrorLabel(150, 237);

                ComboBox cboPos = CreateComboBox(150, 270, new[] { "Doctor", "Staff" });  // ✅ FIX: Điều chỉnh Y

                TextBox txtSpec = CreateTextBox(150, 320);  // ✅ FIX: Điều chỉnh Y
                Label lblSpecError = CreateErrorLabel(150, 347);  // ✅ FIX: Thêm error label cho chuyên môn

                TextBox txtSalary = CreateTextBox(150, 380);  // ✅ FIX: Điều chỉnh Y
                txtSalary.Text = "0";
                Label lblSalaryError = CreateErrorLabel(150, 407);

                DateTimePicker dtpHireDate = new DateTimePicker
                {
                    Location = new Point(150, 440),  // ✅ FIX: Điều chỉnh Y
                    Size = new Size(350, 25),
                    Format = DateTimePickerFormat.Short,
                    MaxDate = DateTime.Today,  // ✅ GOOD: Đã có MaxDate
                    Value = DateTime.Today
                };
                Label lblDateError = CreateErrorLabel(150, 467);

                // ✅ FIX: Biến lưu password cũ để so sánh
                string originalPassword = "";

                // ✅ FIX: Real-time validation events với null checks
                txtFullname.TextChanged += (s, e) =>
                {
                    if (txtFullname != null && lblFullnameError != null)
                        ValidateFullnameRealtime(txtFullname, lblFullnameError);
                };

                txtPhone.TextChanged += (s, e) =>
                {
                    if (txtPhone != null && lblPhoneError != null)
                        ValidatePhoneRealtime(txtPhone, lblPhoneError, staffId);
                };

                txtEmail.TextChanged += (s, e) =>
                {
                    if (txtEmail != null && lblEmailError != null && !txtEmail.ReadOnly)
                        ValidateEmailRealtime(txtEmail, lblEmailError, staffId);
                };

                txtPassword.TextChanged += (s, e) =>
                {
                    if (txtPassword != null && lblPasswordError != null)
                        ValidatePasswordRealtime(txtPassword, lblPasswordError, staffId);
                };

                // ✅ FIX: Real-time validation cho chuyên môn dựa trên chức vụ
                EventHandler validateSpec = (s, e) =>
                {
                    if (txtSpec != null && lblSpecError != null && cboPos != null)
                        ValidateSpecializationRealtime(txtSpec, lblSpecError, cboPos);
                };
                txtSpec.TextChanged += validateSpec;
                cboPos.SelectedIndexChanged += validateSpec;

                txtSalary.TextChanged += (s, e) =>
                {
                    if (txtSalary != null && lblSalaryError != null)
                        ValidateSalaryRealtime(txtSalary, lblSalaryError);
                };

                dtpHireDate.ValueChanged += (s, e) =>
                {
                    if (dtpHireDate != null && lblDateError != null)
                        ValidateDateRealtime(dtpHireDate, lblDateError);
                };

                form.Controls.AddRange(new Control[]
                {
                    CreateLabel("Họ tên:", 30, 33), txtFullname, lblFullnameError,
                    CreateLabel("SĐT:", 30, 93), txtPhone, lblPhoneError,
                    CreateLabel("Email:", 30, 153), txtEmail, lblEmailError,
                    CreateLabel("Mật khẩu:", 30, 213), txtPassword, lblPasswordError,
                    CreateLabel("Chức vụ:", 30, 273), cboPos,
                    CreateLabel("Chuyên môn:", 30, 323), txtSpec, lblSpecError,  // ✅ FIX: Thêm error label
                    CreateLabel("Lương:", 30, 383), txtSalary, lblSalaryError,
                    CreateLabel("Ngày vào:", 30, 443), dtpHireDate, lblDateError,
                });

                int userId = 0;

                // Load data nếu sửa
                if (staffId.HasValue)
                {
                    try
                    {
                        string query = @"
                            SELECT u.user_id, u.fullname, u.phone, u.email, u.password_hash,
                                   s.position, s.specialization, s.salary, s.hire_date
                            FROM Staff s 
                            INNER JOIN UserAccount u ON s.user_id = u.user_id 
                            WHERE s.staff_id = @id";

                        DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@id", staffId.Value) });

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            userId = Convert.ToInt32(row["user_id"]);
                            txtFullname.Text = row["fullname"]?.ToString() ?? "";
                            txtPhone.Text = row["phone"]?.ToString() ?? "";
                            txtEmail.Text = row["email"]?.ToString() ?? "";

                            // ✅ FIX: Email readonly với tooltip
                            txtEmail.ReadOnly = true;
                            txtEmail.BackColor = Color.LightGray;
                            ToolTip tooltip = new ToolTip();
                            tooltip.SetToolTip(txtEmail, "Email không thể thay đổi sau khi tạo tài khoản");

                            originalPassword = row["password_hash"]?.ToString() ?? "";
                            txtPassword.Text = originalPassword;

                            string position = row["position"]?.ToString();
                            if (!string.IsNullOrEmpty(position))
                                cboPos.SelectedItem = position;

                            txtSpec.Text = row["specialization"]?.ToString() ?? "";
                            txtSalary.Text = row["salary"]?.ToString() ?? "0";

                            if (row["hire_date"] != DBNull.Value)
                            {
                                DateTime hireDate = Convert.ToDateTime(row["hire_date"]);
                                if (hireDate <= DateTime.Today)
                                {
                                    dtpHireDate.Value = hireDate;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
                        form.Close();
                        return;
                    }
                }
                else
                {
                    // Khi thêm mới, yêu cầu mật khẩu
                    txtPassword.Text = "";
                }

                Button btnSave = CreateButton("Lưu", 225, 520, "#007ACC");  // ✅ FIX: Điều chỉnh Y
                btnSave.Cursor = Cursors.Hand;

                btnSave.Click += (s, ev) =>
                {
                    // ✅ FIX: Disable button để tránh double-click
                    btnSave.Enabled = false;
                    btnSave.Text = "Đang lưu...";

                    try
                    {
                        // ✅ FIX: Validation đầy đủ với chức vụ
                        if (!ValidateInput(txtFullname, txtPhone, txtEmail, txtPassword, txtSalary,
                            txtSpec, cboPos, dtpHireDate, staffId, userId))
                        {
                            btnSave.Enabled = true;
                            btnSave.Text = "Lưu";
                            return;
                        }

                        // ✅ FIX: Normalize tất cả inputs
                        string fullname = NormalizeWhitespace(txtFullname.Text.Trim());
                        string phone = txtPhone.Text.Trim();
                        string email = txtEmail.Text.Trim().ToLower();
                        string password = txtPassword.Text.Trim();
                        string specialization = string.IsNullOrWhiteSpace(txtSpec.Text) ? null : NormalizeWhitespace(txtSpec.Text.Trim());
                        decimal salary = decimal.Parse(txtSalary.Text);
                        string selectedPosition = cboPos.SelectedItem.ToString();

                        if (staffId.HasValue)
                        {
                            // ✅ FIX: Kiểm tra trùng SĐT khi UPDATE (ngoại trừ chính user đang edit)
                            object existingPhone = DatabaseHelper.ExecuteScalar(
                                "SELECT COUNT(*) FROM UserAccount WHERE phone = @phone AND user_id != @uid",
                                new SqlParameter[] {
                                    new SqlParameter("@phone", phone),
                                    new SqlParameter("@uid", userId)
                                });

                            if (Convert.ToInt32(existingPhone) > 0)
                            {
                                MessageBoxHelper.ShowValidationError("Số điện thoại đã tồn tại trong hệ thống!");
                                txtPhone.Focus();
                                btnSave.Enabled = true;
                                btnSave.Text = "Lưu";
                                return;
                            }

                            // ✅ FIX: Confirm khi thay đổi mật khẩu
                            if (password != originalPassword)
                            {
                                var result = MessageBox.Show(
                                    "Bạn đã thay đổi mật khẩu. Bạn có chắc chắn muốn lưu mật khẩu mới?",
                                    "Xác nhận thay đổi mật khẩu",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);

                                if (result != DialogResult.Yes)
                                {
                                    txtPassword.Focus();
                                    btnSave.Enabled = true;
                                    btnSave.Text = "Lưu";
                                    return;
                                }
                            }

                            // Update UserAccount
                            DatabaseHelper.ExecuteNonQuery(
                                "UPDATE UserAccount SET fullname=@name, phone=@phone, password_hash=@pass WHERE user_id=@uid",
                                new SqlParameter[] {
                                    new SqlParameter("@name", fullname),
                                    new SqlParameter("@phone", phone),
                                    new SqlParameter("@pass", password),
                                    new SqlParameter("@uid", userId)
                                });

                            // Update Staff
                            DatabaseHelper.ExecuteNonQuery(
                                @"UPDATE Staff SET position=@pos, specialization=@spec, salary=@sal, hire_date=@date 
                                  WHERE staff_id=@id",
                                new SqlParameter[] {
                                    new SqlParameter("@pos", selectedPosition),
                                    new SqlParameter("@spec", specialization == null ? (object)DBNull.Value : specialization),
                                    new SqlParameter("@sal", salary),
                                    new SqlParameter("@date", dtpHireDate.Value.Date),
                                    new SqlParameter("@id", staffId.Value)
                                });
                        }
                        else
                        {
                            // Kiểm tra email đã tồn tại chưa
                            object existingEmail = DatabaseHelper.ExecuteScalar(
                                "SELECT COUNT(*) FROM UserAccount WHERE email = @email",
                                new SqlParameter[] { new SqlParameter("@email", email) });

                            if (Convert.ToInt32(existingEmail) > 0)
                            {
                                MessageBoxHelper.ShowValidationError("Email đã tồn tại trong hệ thống!");
                                txtEmail.Focus();
                                btnSave.Enabled = true;
                                btnSave.Text = "Lưu";
                                return;
                            }

                            // Kiểm tra SĐT đã tồn tại chưa
                            object existingPhone = DatabaseHelper.ExecuteScalar(
                                "SELECT COUNT(*) FROM UserAccount WHERE phone = @phone",
                                new SqlParameter[] { new SqlParameter("@phone", phone) });

                            if (Convert.ToInt32(existingPhone) > 0)
                            {
                                MessageBoxHelper.ShowValidationError("Số điện thoại đã tồn tại trong hệ thống!");
                                txtPhone.Focus();
                                btnSave.Enabled = true;
                                btnSave.Text = "Lưu";
                                return;
                            }

                            // Insert UserAccount
                            object newUserId = DatabaseHelper.ExecuteScalar(
                                @"INSERT INTO UserAccount (fullname, phone, email, password_hash, role, status)
                                  OUTPUT INSERTED.user_id
                                  VALUES (@name, @phone, @email, @pass, @role, 'active')",
                                new SqlParameter[] {
                                    new SqlParameter("@name", fullname),
                                    new SqlParameter("@phone", phone),
                                    new SqlParameter("@email", email),
                                    new SqlParameter("@pass", password),
                                    new SqlParameter("@role", selectedPosition.ToLower())
                                });

                            if (newUserId == null)
                            {
                                MessageBoxHelper.ShowError("Không thể tạo tài khoản!");
                                btnSave.Enabled = true;
                                btnSave.Text = "Lưu";
                                return;
                            }

                            // Insert Staff
                            DatabaseHelper.ExecuteNonQuery(
                                @"INSERT INTO Staff (user_id, position, specialization, salary, hire_date) 
                                  VALUES (@uid, @pos, @spec, @sal, @date)",
                                new SqlParameter[] {
                                    new SqlParameter("@uid", newUserId),
                                    new SqlParameter("@pos", selectedPosition),
                                    new SqlParameter("@spec", specialization == null ? (object)DBNull.Value : specialization),
                                    new SqlParameter("@sal", salary),
                                    new SqlParameter("@date", dtpHireDate.Value.Date)
                                });
                        }

                        MessageBoxHelper.ShowSaveSuccess();
                        Logger.LogAction(staffId.HasValue ? "UPDATE_STAFF" : "CREATE_STAFF", fullname);
                        form.Close();
                        LoadStaff();
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                        {
                            MessageBoxHelper.ShowError("Email hoặc SĐT đã tồn tại!");
                        }
                        else
                        {
                            MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                        }
                        btnSave.Enabled = true;
                        btnSave.Text = "Lưu";
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError($"Lỗi: {ex.Message}");
                        btnSave.Enabled = true;
                        btnSave.Text = "Lưu";
                    }
                };

                form.Controls.Add(btnSave);
                form.ShowDialog();
            }
        }

        // ✅ FIX: Validation đầy đủ với chức vụ và chuyên môn
        private bool ValidateInput(TextBox txtFullname, TextBox txtPhone, TextBox txtEmail,
            TextBox txtPassword, TextBox txtSalary, TextBox txtSpec, ComboBox cboPos,
            DateTimePicker dtpHireDate, int? staffId, int userId)
        {
            // ✅ FIX: Normalize trước khi validate
            string fullname = NormalizeWhitespace(txtFullname.Text.Trim());

            // Kiểm tra họ tên
            if (string.IsNullOrWhiteSpace(fullname))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập họ tên!");
                txtFullname.Focus();
                return false;
            }

            if (fullname.Length < 3)
            {
                MessageBoxHelper.ShowValidationError("Họ tên phải có ít nhất 3 ký tự!");
                txtFullname.Focus();
                return false;
            }

            // ✅ FIX: Kiểm tra max length
            if (fullname.Length > 100)
            {
                MessageBoxHelper.ShowValidationError("Họ tên không được vượt quá 100 ký tự!");
                txtFullname.Focus();
                return false;
            }

            if (!IsValidVietnameseName(fullname))
            {
                MessageBoxHelper.ShowValidationError("Họ tên chỉ được chứa chữ cái, phải có ít nhất 2 từ (họ và tên)!");
                txtFullname.Focus();
                return false;
            }

            // Kiểm tra SĐT
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập số điện thoại!");
                txtPhone.Focus();
                return false;
            }

            if (!Validator.IsValidPhone(txtPhone.Text))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không hợp lệ (10-11 số)!");
                txtPhone.Focus();
                return false;
            }

            // Kiểm tra email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập email!");
                txtEmail.Focus();
                return false;
            }

            if (!Validator.IsValidEmail(txtEmail.Text))
            {
                MessageBoxHelper.ShowValidationError("Email không hợp lệ!");
                txtEmail.Focus();
                return false;
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập mật khẩu!");
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBoxHelper.ShowValidationError("Mật khẩu phải có ít nhất 6 ký tự!");
                txtPassword.Focus();
                return false;
            }

            // ✅ FIX: Kiểm tra max length mật khẩu
            if (txtPassword.Text.Length > 50)
            {
                MessageBoxHelper.ShowValidationError("Mật khẩu không được vượt quá 50 ký tự!");
                txtPassword.Focus();
                return false;
            }

            // ✅ FIX: Cảnh báo mật khẩu yếu
            if (!HasStrongPassword(txtPassword.Text))
            {
                var result = MessageBox.Show(
                    "Mật khẩu yếu! Nên có chữ hoa, chữ thường và số để bảo mật tốt hơn.\nBạn có muốn tiếp tục?",
                    "Cảnh báo mật khẩu yếu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtPassword.Focus();
                    return false;
                }
            }

            // ✅ FIX: QUAN TRỌNG - Validate chuyên môn theo chức vụ
            string selectedPosition = cboPos.SelectedItem?.ToString();
            if (selectedPosition == "Doctor")
            {
                // Doctor BẮT BUỘC phải có chuyên môn
                if (string.IsNullOrWhiteSpace(txtSpec.Text))
                {
                    MessageBoxHelper.ShowValidationError("Bác sĩ bắt buộc phải nhập chuyên môn!");
                    txtSpec.Focus();
                    return false;
                }

                string spec = NormalizeWhitespace(txtSpec.Text.Trim());
                if (spec.Length < 3)
                {
                    MessageBoxHelper.ShowValidationError("Chuyên môn phải có ít nhất 3 ký tự!");
                    txtSpec.Focus();
                    return false;
                }

                if (spec.Length > 100)
                {
                    MessageBoxHelper.ShowValidationError("Chuyên môn không được vượt quá 100 ký tự!");
                    txtSpec.Focus();
                    return false;
                }

                // ✅ FIX: Validate chỉ cho phép chữ cái
                if (!IsValidSpecialization(spec))
                {
                    MessageBoxHelper.ShowValidationError("Chuyên môn chỉ được chứa chữ cái và khoảng trắng!");
                    txtSpec.Focus();
                    return false;
                }
            }
            else if (selectedPosition == "Staff")
            {
                // Staff thì chuyên môn optional, nhưng nếu có thì validate
                if (!string.IsNullOrWhiteSpace(txtSpec.Text))
                {
                    string spec = NormalizeWhitespace(txtSpec.Text.Trim());
                    if (spec.Length > 100)
                    {
                        MessageBoxHelper.ShowValidationError("Chuyên môn không được vượt quá 100 ký tự!");
                        txtSpec.Focus();
                        return false;
                    }

                    // ✅ FIX: Validate chỉ cho phép chữ cái
                    if (!IsValidSpecialization(spec))
                    {
                        MessageBoxHelper.ShowValidationError("Chuyên môn chỉ được chứa chữ cái và khoảng trắng!");
                        txtSpec.Focus();
                        return false;
                    }
                }
            }

            // Kiểm tra lương
            if (!decimal.TryParse(txtSalary.Text, out decimal salary))
            {
                MessageBoxHelper.ShowValidationError("Lương phải là số!");
                txtSalary.Focus();
                return false;
            }

            if (salary < 0)
            {
                MessageBoxHelper.ShowValidationError("Lương không được âm!");
                txtSalary.Focus();
                return false;
            }

            if (salary > 999999999)
            {
                MessageBoxHelper.ShowValidationError("Lương không được vượt quá 999,999,999!");
                txtSalary.Focus();
                return false;
            }

            // ✅ FIX: Cảnh báo lương = 0
            if (salary == 0)
            {
                var result = MessageBox.Show(
                    "Lương = 0 VNĐ. Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtSalary.Focus();
                    return false;
                }
            }
            // ✅ FIX: Cảnh báo lương quá thấp
            else if (salary > 0 && salary < 1000000)
            {
                var result = MessageBox.Show(
                    "Lương rất thấp (< 1,000,000 VNĐ). Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtSalary.Focus();
                    return false;
                }
            }
            // ✅ FIX: Cảnh báo lương quá cao
            else if (salary > 100000000)
            {
                var result = MessageBox.Show(
                    "Lương rất cao (> 100,000,000 VNĐ). Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtSalary.Focus();
                    return false;
                }
            }

            // Kiểm tra ngày vào
            if (dtpHireDate.Value.Date > DateTime.Today)
            {
                MessageBoxHelper.ShowValidationError("Ngày vào không được là ngày tương lai!");
                dtpHireDate.Focus();
                return false;
            }

            // Kiểm tra ngày vào hợp lý (không quá 50 năm trước)
            if (dtpHireDate.Value.Date < DateTime.Today.AddYears(-50))
            {
                MessageBoxHelper.ShowValidationError("Ngày vào không hợp lý (quá 50 năm trước)!");
                dtpHireDate.Focus();
                return false;
            }

            return true;
        }

        // ✅ FIX: Real-time validation methods với đầy đủ checks
        private void ValidateFullnameRealtime(TextBox txt, Label lblError)
        {
            if (txt == null || lblError == null) return;

            string value = NormalizeWhitespace(txt.Text.Trim());

            if (string.IsNullOrWhiteSpace(value))
            {
                ShowFieldError(txt, lblError, "Họ tên không được để trống");
            }
            else if (value.Length < 3)
            {
                ShowFieldError(txt, lblError, "Họ tên phải có ít nhất 3 ký tự");
            }
            else if (value.Length > 100)
            {
                ShowFieldError(txt, lblError, "Họ tên không được vượt quá 100 ký tự");
            }
            else if (!IsValidVietnameseName(value))
            {
                ShowFieldError(txt, lblError, "Họ tên chỉ được chứa chữ cái, phải có ít nhất 2 từ");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private bool IsValidVietnameseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Normalize trước khi check
            name = NormalizeWhitespace(name);

            // Chỉ cho phép chữ cái (bao gồm tiếng Việt có dấu), khoảng trắng
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                    return false;
            }

            // Kiểm tra không phải toàn khoảng trắng
            if (name.Trim().Length == 0)
                return false;

            // Kiểm tra phải có ít nhất 2 từ (họ và tên)
            string[] words = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return false;

            return true;
        }

        // ✅ FIX: Validate phone real-time với duplicate check khi UPDATE
        private void ValidatePhoneRealtime(TextBox txt, Label lblError, int? staffId)
        {
            if (txt == null || lblError == null) return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "SĐT không được để trống");
            }
            else if (!Validator.IsValidPhone(txt.Text))
            {
                ShowFieldError(txt, lblError, "SĐT không hợp lệ (10-11 số)");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        // ✅ FIX: Validate email real-time (chỉ khi không readonly)
        private void ValidateEmailRealtime(TextBox txt, Label lblError, int? staffId)
        {
            if (txt == null || lblError == null || txt.ReadOnly) return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Email không được để trống");
            }
            else if (!Validator.IsValidEmail(txt.Text))
            {
                ShowFieldError(txt, lblError, "Email không hợp lệ");
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        // ✅ FIX: Validate password với độ mạnh
        private void ValidatePasswordRealtime(TextBox txt, Label lblError, int? staffId)
        {
            if (txt == null || lblError == null) return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Mật khẩu không được để trống");
            }
            else if (txt.Text.Length < 6)
            {
                ShowFieldError(txt, lblError, "Mật khẩu phải có ít nhất 6 ký tự");
            }
            else if (txt.Text.Length > 50)
            {
                ShowFieldError(txt, lblError, "Mật khẩu không được vượt quá 50 ký tự");
            }
            else if (!HasStrongPassword(txt.Text))
            {
                ShowFieldError(txt, lblError, "⚠ Mật khẩu yếu (nên có chữ hoa, chữ thường, số)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Orange warning
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        // ✅ FIX: Validate chuyên môn dựa trên chức vụ
        private void ValidateSpecializationRealtime(TextBox txt, Label lblError, ComboBox cboPos)
        {
            if (txt == null || lblError == null || cboPos == null) return;

            string position = cboPos.SelectedItem?.ToString();
            string value = txt.Text.Trim();

            if (position == "Doctor")
            {
                // Doctor bắt buộc phải có chuyên môn
                if (string.IsNullOrWhiteSpace(value))
                {
                    ShowFieldError(txt, lblError, "Bác sĩ bắt buộc phải nhập chuyên môn");
                }
                else if (value.Length < 3)
                {
                    ShowFieldError(txt, lblError, "Chuyên môn phải có ít nhất 3 ký tự");
                }
                else if (value.Length > 100)
                {
                    ShowFieldError(txt, lblError, "Chuyên môn không được vượt quá 100 ký tự");
                }
                // ✅ FIX: Validate chỉ cho phép chữ cái
                else if (!IsValidSpecialization(value))
                {
                    ShowFieldError(txt, lblError, "Chuyên môn chỉ được chứa chữ cái và khoảng trắng");
                }
                else
                {
                    ClearFieldError(txt, lblError);
                }
            }
            else if (position == "Staff")
            {
                // Staff thì optional, nhưng nếu có thì validate
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (value.Length > 100)
                    {
                        ShowFieldError(txt, lblError, "Chuyên môn không được vượt quá 100 ký tự");
                    }
                    // ✅ FIX: Validate chỉ cho phép chữ cái
                    else if (!IsValidSpecialization(value))
                    {
                        ShowFieldError(txt, lblError, "Chuyên môn chỉ được chứa chữ cái và khoảng trắng");
                    }
                    else
                    {
                        ClearFieldError(txt, lblError);
                    }
                }
                else
                {
                    ClearFieldError(txt, lblError);
                }
            }
        }

        // ✅ FIX: Validate salary với warnings
        private void ValidateSalaryRealtime(TextBox txt, Label lblError)
        {
            if (txt == null || lblError == null) return;

            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                ShowFieldError(txt, lblError, "Lương không được để trống");
            }
            else if (!decimal.TryParse(txt.Text, out decimal salary))
            {
                ShowFieldError(txt, lblError, "Lương phải là số");
            }
            else if (salary < 0)
            {
                ShowFieldError(txt, lblError, "Lương không được âm");
            }
            else if (salary > 999999999)
            {
                ShowFieldError(txt, lblError, "Lương không được vượt quá 999,999,999");
            }
            else if (salary == 0)
            {
                ShowFieldError(txt, lblError, "⚠ Lương = 0 VNĐ");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Orange warning
            }
            else if (salary > 0 && salary < 1000000)
            {
                ShowFieldError(txt, lblError, "⚠ Lương rất thấp (< 1,000,000 VNĐ)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Orange warning
            }
            else if (salary > 100000000)
            {
                ShowFieldError(txt, lblError, "⚠ Lương rất cao (> 100,000,000 VNĐ)");
                txt.BackColor = Color.FromArgb(255, 243, 205); // Orange warning
            }
            else
            {
                ClearFieldError(txt, lblError);
            }
        }

        private void ValidateDateRealtime(DateTimePicker dtp, Label lblError)
        {
            if (dtp == null || lblError == null) return;

            if (dtp.Value.Date > DateTime.Today)
            {
                ShowFieldError(null, lblError, "Ngày vào không được là ngày tương lai");
            }
            else if (dtp.Value.Date < DateTime.Today.AddYears(-50))
            {
                ShowFieldError(null, lblError, "Ngày vào không hợp lý (quá 50 năm trước)");
            }
            else
            {
                ClearFieldError(null, lblError);
            }
        }

        private void ShowFieldError(TextBox txt, Label lblError, string message)
        {
            if (txt != null)
            {
                if (message.StartsWith("⚠"))
                    txt.BackColor = Color.FromArgb(255, 243, 205); // Warning - light orange
                else
                    txt.BackColor = Color.FromArgb(255, 235, 238); // Error - light red
            }
            lblError.Text = message;
            lblError.ForeColor = message.StartsWith("⚠") ? Color.Orange : Color.Red;
            lblError.Visible = true;
        }

        private void ClearFieldError(TextBox txt, Label lblError)
        {
            if (txt != null)
                txt.BackColor = Color.White;
            lblError.Text = "";
            lblError.Visible = false;
        }

        private Label CreateErrorLabel(int x, int y)
        {
            return new Label
            {
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Visible = false,
                MaximumSize = new Size(350, 0)
            };
        }

        // ✅ FIX: Helper methods mới

        /// <summary>
        /// Normalize whitespace - loại bỏ khoảng trắng thừa và khoảng trắng liên tiếp
        /// </summary>
        private string NormalizeWhitespace(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Loại bỏ khoảng trắng đầu/cuối và replace nhiều spaces thành 1 space
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// Validate chuyên môn - chỉ cho phép chữ cái và khoảng trắng
        /// Ví dụ: "Nội khoa", "Răng hàm mặt", "Chỉnh nha"
        /// </summary>
        private bool IsValidSpecialization(string specialization)
        {
            if (string.IsNullOrWhiteSpace(specialization))
                return false;

            // Chỉ cho phép chữ cái (bao gồm tiếng Việt có dấu) và khoảng trắng
            // KHÔNG cho phép số hoặc ký tự đặc biệt
            foreach (char c in specialization)
            {
                if (!char.IsLetter(c) && c != ' ')
                    return false;
            }

            // Kiểm tra không phải toàn khoảng trắng
            if (specialization.Trim().Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Kiểm tra độ mạnh mật khẩu - có chữ hoa, chữ thường và số
        /// </summary>
        private bool HasStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            // Mật khẩu mạnh nếu có ít nhất 2 trong 3 loại
            int strongCount = (hasUpper ? 1 : 0) + (hasLower ? 1 : 0) + (hasDigit ? 1 : 0);
            return strongCount >= 2;
        }

        private void DeleteStaff(int staffId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBoxHelper.ShowError("Không thể xác định nhân viên cần xóa!");
                return;
            }

            if (!MessageBoxHelper.ShowDeleteConfirm(name)) return;

            try
            {
                // Kiểm tra nhân viên có đang được sử dụng không
                object recordCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM MedicalRecord WHERE staff_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", staffId) });

                if (Convert.ToInt32(recordCount) > 0)
                {
                    MessageBoxHelper.ShowError("Không thể xóa nhân viên này vì đã có hồ sơ bệnh án liên quan!");
                    return;
                }

                object invoiceCount = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Invoice WHERE staff_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", staffId) });

                if (Convert.ToInt32(invoiceCount) > 0)
                {
                    MessageBoxHelper.ShowError("Không thể xóa nhân viên này vì đã có hóa đơn liên quan!");
                    return;
                }

                // Lấy user_id
                object userIdObj = DatabaseHelper.ExecuteScalar(
                    "SELECT user_id FROM Staff WHERE staff_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", staffId) });

                if (userIdObj == null || userIdObj == DBNull.Value)
                {
                    MessageBoxHelper.ShowError("Không tìm thấy nhân viên!");
                    return;
                }

                int userId = Convert.ToInt32(userIdObj);

                // Xóa Staff trước
                int staffDeleted = DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM Staff WHERE staff_id = @id",
                    new SqlParameter[] { new SqlParameter("@id", staffId) });

                if (staffDeleted > 0)
                {
                    // Xóa UserAccount
                    DatabaseHelper.ExecuteNonQuery(
                        "DELETE FROM UserAccount WHERE user_id = @uid",
                        new SqlParameter[] { new SqlParameter("@uid", userId) });

                    MessageBoxHelper.ShowDeleteSuccess(name);
                    Logger.LogDelete("Staff", name);
                    LoadStaff();
                }
                else
                {
                    MessageBoxHelper.ShowError("Không thể xóa nhân viên!");
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547) // Foreign key constraint
                {
                    MessageBoxHelper.ShowError("Không thể xóa nhân viên vì có dữ liệu liên quan!");
                }
                else
                {
                    MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi xóa: {ex.Message}");
            }
        }

        // Helper methods
        private Label CreateLabel(string text, int x, int y) => new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9)
        };

        private TextBox CreateTextBox(int x, int y) => new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(350, 25),
            Font = new Font("Segoe UI", 9)
        };

        private ComboBox CreateComboBox(int x, int y, object[] items)
        {
            var cbo = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(350, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;
            return cbo;
        }

        private Button CreateButton(string text, int x, int y, string backColor) => new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(100, 35),
            BackColor = ColorTranslator.FromHtml(backColor),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        // Event handlers
        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadStaff();
        private void cboPosition_SelectedIndexChanged(object sender, EventArgs e) => LoadStaff();
    }
}