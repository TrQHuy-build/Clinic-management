using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Pages.Common
{
    public partial class SettingsPage : UserControl
    {
        private Label lblFullnameError;
        private Label lblPhoneError;
        private Label lblOldPasswordError;
        private Label lblNewPasswordError;
        private Label lblConfirmPasswordError;

        

        public SettingsPage()
        {
            InitializeComponent();
            this.Load += SettingsPage_Load;
        }

        private void SettingsPage_Load(object sender, EventArgs e)
        {
            InitializeErrorLabels();
            AttachValidationEvents();
            LoadUserInfo();
            btnToggleOldPassword.FlatStyle = FlatStyle.Flat;
            btnToggleNewPassword.FlatStyle = FlatStyle.Flat;
            btnToggleConfirmPassword.FlatStyle = FlatStyle.Flat;

            btnToggleOldPassword.FlatAppearance.BorderSize = 0;
            btnToggleNewPassword.FlatAppearance.BorderSize = 0;
            btnToggleConfirmPassword.FlatAppearance.BorderSize = 0;

            btnToggleOldPassword.Cursor = Cursors.Hand;
            btnToggleNewPassword.Cursor = Cursors.Hand;
            btnToggleConfirmPassword.Cursor = Cursors.Hand;

            SetupToggle(btnToggleOldPassword, txtOldPassword);
            SetupToggle(btnToggleNewPassword, txtNewPassword);
            SetupToggle(btnToggleConfirmPassword, txtConfirmPassword);
        }

        private void SetupToggle(Button btn, TextBox txt)
        {
            if (btn == null || txt == null) return;

            // Ban đầu ẩn mật khẩu
            txt.UseSystemPasswordChar = true;

            btn.Click += (s, e) => TogglePasswordVisibility(txt, btn);
        }

        private void TogglePasswordVisibility(TextBox txt, Button btn)
        {
            if (txt.UseSystemPasswordChar)
            {
                txt.UseSystemPasswordChar = false;
                btn.Text = "👁‍🗨";     // đang hiển thị
            }
            else
            {
                txt.UseSystemPasswordChar = true;
                btn.Text = "👁";      // đang ẩn
            }
        }

        private void InitializeErrorLabels()
        {
            // Kiểm tra xem error labels đã tồn tại chưa (tránh tạo lại)
            if (lblFullnameError != null) return;

            // Tạo error labels
            lblFullnameError = CreateErrorLabel();
            lblPhoneError = CreateErrorLabel();
            lblOldPasswordError = CreateErrorLabel();
            lblNewPasswordError = CreateErrorLabel();
            lblConfirmPasswordError = CreateErrorLabel();

            // Thử tìm parent container (Panel, GroupBox, TabPage)
            Control profileContainer = FindParentContainer(txtFullname);
            Control passwordContainer = FindParentContainer(txtOldPassword);

            // Thêm vào container hoặc UserControl
            if (profileContainer != null)
            {
                AddErrorLabelToContainer(profileContainer, txtFullname, lblFullnameError);
                AddErrorLabelToContainer(profileContainer, txtPhone, lblPhoneError);
            }
            else
            {
                // Fallback: thêm trực tiếp vào UserControl
                AddErrorLabelDirect(txtFullname, lblFullnameError);
                AddErrorLabelDirect(txtPhone, lblPhoneError);
            }

            if (passwordContainer != null)
            {
                AddErrorLabelToContainer(passwordContainer, txtOldPassword, lblOldPasswordError);
                AddErrorLabelToContainer(passwordContainer, txtNewPassword, lblNewPasswordError);
                AddErrorLabelToContainer(passwordContainer, txtConfirmPassword, lblConfirmPasswordError);
            }
            else
            {
                // Fallback: thêm trực tiếp vào UserControl
                AddErrorLabelDirect(txtOldPassword, lblOldPasswordError);
                AddErrorLabelDirect(txtNewPassword, lblNewPasswordError);
                AddErrorLabelDirect(txtConfirmPassword, lblConfirmPasswordError);
            }
        }

        private Control FindParentContainer(Control control)
        {
            if (control == null) return null;

            Control parent = control.Parent;
            while (parent != null)
            {
                if (parent is Panel || parent is GroupBox || parent is TabPage)
                    return parent;
                if (parent == this) // Đã đến UserControl gốc
                    return null;
                parent = parent.Parent;
            }
            return null;
        }

        private void AddErrorLabelToContainer(Control container, TextBox textBox, Label errorLabel)
        {
            if (textBox == null || errorLabel == null || container == null) return;

            // Tính toán vị trí tương đối so với container
            Point relativePosToContainer = GetRelativePosition(textBox, container);
            errorLabel.Location = new Point(relativePosToContainer.X, relativePosToContainer.Y + textBox.Height + 2);
            errorLabel.MaximumSize = new Size(textBox.Width, 0);

            container.Controls.Add(errorLabel);
            errorLabel.BringToFront();
        }

        private void AddErrorLabelDirect(TextBox textBox, Label errorLabel)
        {
            if (textBox == null || errorLabel == null) return;

            // Tính vị trí tương đối với form
            Point absolutePos = GetAbsolutePosition(textBox);
            errorLabel.Location = new Point(absolutePos.X, absolutePos.Y + textBox.Height + 2);
            errorLabel.MaximumSize = new Size(textBox.Width, 0);

            this.Controls.Add(errorLabel);
            errorLabel.BringToFront();
        }

        private Point GetRelativePosition(Control control, Control relativeTo)
        {
            Point pos = control.Location;
            Control parent = control.Parent;

            while (parent != null && parent != relativeTo)
            {
                pos.X += parent.Left;
                pos.Y += parent.Top;
                parent = parent.Parent;
            }

            return pos;
        }

        private Point GetAbsolutePosition(Control control)
        {
            Point pos = control.Location;
            Control parent = control.Parent;

            while (parent != null && parent != this)
            {
                pos.X += parent.Left;
                pos.Y += parent.Top;
                parent = parent.Parent;
            }

            return pos;
        }

        private void AttachValidationEvents()
        {
            // Real-time validation cho profile
            if (txtFullname != null)
                txtFullname.TextChanged += (s, e) => ValidateFullnameRealtime();

            if (txtPhone != null)
                txtPhone.TextChanged += (s, e) => ValidatePhoneRealtime();

            // Real-time validation cho password
            if (txtOldPassword != null)
                txtOldPassword.TextChanged += (s, e) => ValidateOldPasswordRealtime();

            if (txtNewPassword != null)
                txtNewPassword.TextChanged += (s, e) => ValidateNewPasswordRealtime();

            if (txtConfirmPassword != null)
                txtConfirmPassword.TextChanged += (s, e) => ValidateConfirmPasswordRealtime();
        }

        private Label CreateErrorLabel()
        {
            return new Label
            {
                AutoSize = true,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Visible = false,
                BackColor = Color.Transparent
            };
        }

        private void LoadUserInfo()
        {
            try
            {
                if (Auth.CurrentUserId == null)
                {
                    MessageBoxHelper.ShowError("Không thể xác định người dùng hiện tại!");
                    return;
                }

                string query = "SELECT fullname, phone, email FROM UserAccount WHERE user_id = @userId";
                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter[] { new SqlParameter("@userId", Auth.CurrentUserId) });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    if (txtFullname != null)
                        txtFullname.Text = row["fullname"]?.ToString() ?? "";

                    if (txtPhone != null)
                        txtPhone.Text = row["phone"]?.ToString() ?? "";

                    if (txtEmail != null)
                    {
                        txtEmail.Text = row["email"]?.ToString() ?? "";
                        txtEmail.ReadOnly = true;
                        txtEmail.BackColor = Color.LightGray;
                    }

                    // Set password fields to use system password char
                    if (txtOldPassword != null)
                        txtOldPassword.UseSystemPasswordChar = true;

                    if (txtNewPassword != null)
                        txtNewPassword.UseSystemPasswordChar = true;

                    if (txtConfirmPassword != null)
                        txtConfirmPassword.UseSystemPasswordChar = true;
                }
                else
                {
                    MessageBoxHelper.ShowError("Không tìm thấy thông tin người dùng!");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi tải thông tin: {ex.Message}");
            }
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            if (!ValidateProfileInput())
                return;

            string fullname = txtFullname.Text.Trim();
            string phone = txtPhone.Text.Trim();

            try
            {
                // Kiểm tra SĐT trùng
                string checkQuery = "SELECT COUNT(*) FROM UserAccount WHERE phone = @phone AND user_id != @userId";
                object count = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter[]
                {
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (Convert.ToInt32(count) > 0)
                {
                    MessageBoxHelper.ShowValidationError("Số điện thoại đã được sử dụng bởi người khác!");
                    txtPhone.Focus();
                    return;
                }

                string updateQuery = "UPDATE UserAccount SET fullname = @fullname, phone = @phone WHERE user_id = @userId";
                int result = DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[]
                {
                    new SqlParameter("@fullname", fullname),
                    new SqlParameter("@phone", phone),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (result > 0)
                {
                    Auth.CurrentUserName = fullname;
                    MessageBoxHelper.ShowSuccess("Cập nhật thông tin cá nhân thành công!");
                    Logger.LogUpdate("UserProfile", $"Cập nhật: {fullname} - {phone}");
                }
                else
                {
                    MessageBoxHelper.ShowError("Không thể cập nhật thông tin!");
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                {
                    MessageBoxHelper.ShowError("Số điện thoại đã tồn tại!");
                }
                else
                {
                    MessageBoxHelper.ShowError($"Lỗi SQL: {sqlEx.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi cập nhật: {ex.Message}");
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            if (!ValidatePasswordInput())
                return;

            string oldPassword = txtOldPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();

            try
            {
                // Kiểm tra mật khẩu cũ - lấy hash từ DB rồi so sánh
                string checkQuery = "SELECT password_hash FROM UserAccount WHERE user_id = @userId";
                object hashObj = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter[]
                {
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (hashObj == null || !(PasswordHelper.VerifyPassword(oldPassword, hashObj.ToString())))
                {
                    MessageBoxHelper.ShowError("Mật khẩu cũ không đúng!");
                    txtOldPassword.Focus();
                    ShowFieldError(txtOldPassword, lblOldPasswordError, "Mật khẩu cũ không đúng");
                    return;
                }

                // Kiểm tra mật khẩu mới không giống mật khẩu cũ
                if (oldPassword == newPassword)
                {
                    MessageBoxHelper.ShowValidationError("Mật khẩu mới phải khác mật khẩu cũ!");
                    txtNewPassword.Focus();
                    ShowFieldError(txtNewPassword, lblNewPasswordError, "Mật khẩu mới phải khác mật khẩu cũ");
                    return;
                }

                // ✅ Mã hóa mật khẩu mới
                string hashedNewPassword = PasswordHelper.HashPassword(newPassword);

                string updateQuery = "UPDATE UserAccount SET password_hash = @newPassword WHERE user_id = @userId";
                int result = DatabaseHelper.ExecuteNonQuery(updateQuery, new SqlParameter[]
                {
                    new SqlParameter("@newPassword", hashedNewPassword),
                    new SqlParameter("@userId", Auth.CurrentUserId)
                });

                if (result > 0)
                {
                    MessageBoxHelper.ShowSuccess("Đổi mật khẩu thành công!");
                    Logger.LogPasswordChange();

                    // Clear fields
                    if (txtOldPassword != null) txtOldPassword.Clear();
                    if (txtNewPassword != null) txtNewPassword.Clear();
                    if (txtConfirmPassword != null) txtConfirmPassword.Clear();

                    // Clear errors
                    ClearFieldError(txtOldPassword, lblOldPasswordError);
                    ClearFieldError(txtNewPassword, lblNewPasswordError);
                    ClearFieldError(txtConfirmPassword, lblConfirmPasswordError);
                }
                else
                {
                    MessageBoxHelper.ShowError("Không thể đổi mật khẩu!");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Lỗi đổi mật khẩu: {ex.Message}");
            }
        }


        // Validation methods
        private bool ValidateProfileInput()
        {
            string fullname = txtFullname?.Text?.Trim() ?? "";
            string phone = txtPhone?.Text?.Trim() ?? "";

            // Kiểm tra họ tên
            if (string.IsNullOrWhiteSpace(fullname))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập họ và tên!");
                txtFullname?.Focus();
                return false;
            }

            if (fullname.Length < 3)
            {
                MessageBoxHelper.ShowValidationError("Họ và tên phải có ít nhất 3 ký tự!");
                txtFullname?.Focus();
                return false;
            }

            if (!IsValidVietnameseName(fullname))
            {
                MessageBoxHelper.ShowValidationError("Họ tên chỉ được chứa chữ cái, phải có ít nhất 2 từ (họ và tên)!");
                txtFullname?.Focus();
                return false;
            }

            // Kiểm tra SĐT
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập số điện thoại!");
                txtPhone?.Focus();
                return false;
            }

            if (!Validator.IsValidPhone(phone))
            {
                MessageBoxHelper.ShowValidationError("Số điện thoại không hợp lệ (10-11 số)!");
                txtPhone?.Focus();
                return false;
            }

            return true;
        }

        private bool ValidatePasswordInput()
        {
            string oldPassword = txtOldPassword?.Text?.Trim() ?? "";
            string newPassword = txtNewPassword?.Text?.Trim() ?? "";
            string confirmPassword = txtConfirmPassword?.Text?.Trim() ?? "";

            // Kiểm tra mật khẩu cũ
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập mật khẩu cũ!");
                txtOldPassword?.Focus();
                return false;
            }

            // Kiểm tra mật khẩu mới
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng nhập mật khẩu mới!");
                txtNewPassword?.Focus();
                return false;
            }

            if (newPassword.Length < 6)
            {
                MessageBoxHelper.ShowValidationError("Mật khẩu mới phải có ít nhất 6 ký tự!");
                txtNewPassword?.Focus();
                return false;
            }

            if (newPassword.Length > 50)
            {
                MessageBoxHelper.ShowValidationError("Mật khẩu mới không được vượt quá 50 ký tự!");
                txtNewPassword?.Focus();
                return false;
            }

            // Kiểm tra độ mạnh mật khẩu
            if (!HasStrongPassword(newPassword))
            {
                var result = MessageBox.Show(
                    "Mật khẩu yếu (nên có chữ hoa, chữ thường, số). Bạn có muốn tiếp tục?",
                    "Cảnh báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    txtNewPassword?.Focus();
                    return false;
                }
            }

            // Kiểm tra xác nhận mật khẩu
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBoxHelper.ShowValidationError("Vui lòng xác nhận mật khẩu mới!");
                txtConfirmPassword?.Focus();
                return false;
            }

            if (newPassword != confirmPassword)
            {
                MessageBoxHelper.ShowValidationError("Xác nhận mật khẩu không khớp!");
                txtConfirmPassword?.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidVietnameseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Chỉ cho phép chữ cái và khoảng trắng
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                    return false;
            }

            // Không toàn khoảng trắng
            if (name.Trim().Length == 0)
                return false;

            // Không có nhiều khoảng trắng liên tiếp
            if (name.Contains("  "))
                return false;

            // Phải có ít nhất 2 từ
            string[] words = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return false;

            return true;
        }

        private bool HasStrongPassword(string password)
        {
            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper && hasLower && hasDigit;
        }

        // Real-time validation methods
        private void ValidateFullnameRealtime()
        {
            if (txtFullname == null || lblFullnameError == null) return;

            string fullname = txtFullname.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullname))
            {
                ShowFieldError(txtFullname, lblFullnameError, "Họ tên không được để trống");
            }
            else if (fullname.Length < 3)
            {
                ShowFieldError(txtFullname, lblFullnameError, "Họ tên phải có ít nhất 3 ký tự");
            }
            else if (!IsValidVietnameseName(fullname))
            {
                ShowFieldError(txtFullname, lblFullnameError, "Họ tên chỉ được chứa chữ cái và phải có ít nhất 2 từ");
            }
            else
            {
                ClearFieldError(txtFullname, lblFullnameError);
            }
        }

        private void ValidatePhoneRealtime()
        {
            if (txtPhone == null || lblPhoneError == null) return;

            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowFieldError(txtPhone, lblPhoneError, "SĐT không được để trống");
            }
            else if (!Validator.IsValidPhone(phone))
            {
                ShowFieldError(txtPhone, lblPhoneError, "SĐT không hợp lệ (10-11 số)");
            }
            else
            {
                ClearFieldError(txtPhone, lblPhoneError);
            }
        }

        private void ValidateOldPasswordRealtime()
        {
            if (txtOldPassword == null || lblOldPasswordError == null) return;

            if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                ShowFieldError(txtOldPassword, lblOldPasswordError, "Mật khẩu cũ không được để trống");
            }
            else
            {
                ClearFieldError(txtOldPassword, lblOldPasswordError);
            }
        }

        private void ValidateNewPasswordRealtime()
        {
            if (txtNewPassword == null || lblNewPasswordError == null) return;

            string newPassword = txtNewPassword.Text;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowFieldError(txtNewPassword, lblNewPasswordError, "Mật khẩu mới không được để trống");
            }
            else if (newPassword.Length < 6)
            {
                ShowFieldError(txtNewPassword, lblNewPasswordError, "Mật khẩu phải có ít nhất 6 ký tự");
            }
            else if (newPassword.Length > 50)
            {
                ShowFieldError(txtNewPassword, lblNewPasswordError, "Mật khẩu không được vượt quá 50 ký tự");
            }
            else if (!HasStrongPassword(newPassword))
            {
                ShowFieldError(txtNewPassword, lblNewPasswordError, "⚠ Mật khẩu yếu (nên có chữ hoa, chữ thường, số)");
                txtNewPassword.BackColor = Color.FromArgb(255, 243, 205); // Warning color
            }
            else
            {
                ClearFieldError(txtNewPassword, lblNewPasswordError);
            }

            // Validate confirm password khi new password thay đổi
            ValidateConfirmPasswordRealtime();
        }

        private void ValidateConfirmPasswordRealtime()
        {
            if (txtConfirmPassword == null || lblConfirmPasswordError == null) return;
            if (txtNewPassword == null) return;

            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowFieldError(txtConfirmPassword, lblConfirmPasswordError, "Vui lòng xác nhận mật khẩu");
            }
            else if (confirmPassword != txtNewPassword.Text)
            {
                ShowFieldError(txtConfirmPassword, lblConfirmPasswordError, "Mật khẩu xác nhận không khớp");
            }
            else
            {
                ClearFieldError(txtConfirmPassword, lblConfirmPasswordError);
            }
        }

        private void ShowFieldError(TextBox txt, Label lblError, string message)
        {
            if (txt == null || lblError == null) return;

            if (message.StartsWith("⚠"))
            {
                txt.BackColor = Color.FromArgb(255, 243, 205); // Warning - light orange
                lblError.ForeColor = Color.Orange;
            }
            else
            {
                txt.BackColor = Color.FromArgb(255, 235, 238); // Error - light red
                lblError.ForeColor = Color.Red;
            }

            lblError.Text = message;
            lblError.Visible = true;
        }

        private void ClearFieldError(TextBox txt, Label lblError)
        {
            if (txt == null || lblError == null) return;

            txt.BackColor = Color.White;
            lblError.Text = "";
            lblError.Visible = false;
        }
    }
}