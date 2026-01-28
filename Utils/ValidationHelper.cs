using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Helper class for input validation with visual feedback
    /// ✅ ErrorProvider indicators
    /// ✅ TextBox color changes
    /// ✅ Real-time validation
    /// ✅ Multiple validation rules
    /// </summary>
    public class ValidationHelper
    {
        private readonly ErrorProvider errorProvider;
        private readonly Form parentForm;

        // Colors for validation feedback
        private static readonly Color ValidColor = Color.White;
        private static readonly Color InvalidColor = Color.FromArgb(255, 230, 230); // Light red
        private static readonly Color FocusColor = Color.FromArgb(230, 240, 255); // Light blue

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationHelper(Form form)
        {
            parentForm = form;
            errorProvider = new ErrorProvider(form)
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink,
                Icon = SystemIcons.Error
            };
        }

        #region Required Field Validation

        /// <summary>
        /// Validate required text field
        /// </summary>
        public bool ValidateRequired(TextBox textBox, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetError(textBox, $"{fieldName} không được để trống");
                return false;
            }

            ClearError(textBox);
            return true;
        }

        /// <summary>
        /// Validate required ComboBox
        /// </summary>
        public bool ValidateRequired(ComboBox comboBox, string fieldName)
        {
            if (comboBox.SelectedIndex < 0)
            {
                SetError(comboBox, $"Vui lòng chọn {fieldName}");
                return false;
            }

            ClearError(comboBox);
            return true;
        }

        /// <summary>
        /// Validate required DateTimePicker
        /// </summary>
        public bool ValidateRequired(DateTimePicker dateTimePicker, string fieldName)
        {
            if (dateTimePicker.Value == null)
            {
                SetError(dateTimePicker, $"{fieldName} không được để trống");
                return false;
            }

            ClearError(dateTimePicker);
            return true;
        }

        #endregion

        #region Email Validation

        /// <summary>
        /// Validate email format
        /// </summary>
        public bool ValidateEmail(TextBox textBox, bool allowEmpty = false)
        {
            string email = textBox.Text.Trim();

            if (allowEmpty && string.IsNullOrEmpty(email))
            {
                ClearError(textBox);
                return true;
            }

            if (string.IsNullOrEmpty(email))
            {
                SetError(textBox, "Email không được để trống");
                return false;
            }

            // Email regex pattern
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, pattern))
            {
                SetError(textBox, "Email không hợp lệ");
                return false;
            }

            ClearError(textBox);
            return true;
        }

        #endregion

        #region Phone Validation

        /// <summary>
        /// Validate phone number (Vietnamese format)
        /// </summary>
        public bool ValidatePhone(TextBox textBox, bool allowEmpty = false)
        {
            string phone = textBox.Text.Trim();

            if (allowEmpty && string.IsNullOrEmpty(phone))
            {
                ClearError(textBox);
                return true;
            }

            if (string.IsNullOrEmpty(phone))
            {
                SetError(textBox, "Số điện thoại không được để trống");
                return false;
            }

            // Remove spaces and dashes
            phone = phone.Replace(" ", "").Replace("-", "");

            // Vietnamese phone pattern: 10 digits starting with 0
            string pattern = @"^0[0-9]{9}$";
            if (!Regex.IsMatch(phone, pattern))
            {
                SetError(textBox, "Số điện thoại không hợp lệ (10 chữ số, bắt đầu bằng 0)");
                return false;
            }

            ClearError(textBox);
            return true;
        }

        #endregion

        #region Numeric Validation

        /// <summary>
        /// Validate numeric input (integer)
        /// </summary>
        public bool ValidateNumeric(TextBox textBox, string fieldName, int min = int.MinValue, int max = int.MaxValue)
        {
            string text = textBox.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                SetError(textBox, $"{fieldName} không được để trống");
                return false;
            }

            if (!int.TryParse(text, out int value))
            {
                SetError(textBox, $"{fieldName} phải là số nguyên");
                return false;
            }

            if (value < min || value > max)
            {
                SetError(textBox, $"{fieldName} phải từ {min} đến {max}");
                return false;
            }

            ClearError(textBox);
            return true;
        }

        /// <summary>
        /// Validate decimal input
        /// </summary>
        public bool ValidateDecimal(TextBox textBox, string fieldName, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
        {
            string text = textBox.Text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                SetError(textBox, $"{fieldName} không được để trống");
                return false;
            }

            if (!decimal.TryParse(text, out decimal value))
            {
                SetError(textBox, $"{fieldName} phải là số");
                return false;
            }

            if (value < min || value > max)
            {
                SetError(textBox, $"{fieldName} phải từ {Formatter.FormatCurrency(min)} đến {Formatter.FormatCurrency(max)}");
                return false;
            }

            ClearError(textBox);
            return true;
        }

        #endregion

        #region Date Validation

        /// <summary>
        /// Validate date range
        /// </summary>
        public bool ValidateDateRange(DateTimePicker dateTimePicker, string fieldName, DateTime? minDate = null, DateTime? maxDate = null)
        {
            DateTime selectedDate = dateTimePicker.Value.Date;

            if (minDate.HasValue && selectedDate < minDate.Value.Date)
            {
                SetError(dateTimePicker, $"{fieldName} không được trước {minDate.Value:dd/MM/yyyy}");
                return false;
            }

            if (maxDate.HasValue && selectedDate > maxDate.Value.Date)
            {
                SetError(dateTimePicker, $"{fieldName} không được sau {maxDate.Value:dd/MM/yyyy}");
                return false;
            }

            ClearError(dateTimePicker);
            return true;
        }

        /// <summary>
        /// Validate date not in past
        /// </summary>
        public bool ValidateNotPastDate(DateTimePicker dateTimePicker, string fieldName)
        {
            DateTime selectedDate = dateTimePicker.Value.Date;
            DateTime today = DateTime.Today;

            if (selectedDate < today)
            {
                SetError(dateTimePicker, $"{fieldName} không được ở quá khứ");
                return false;
            }

            ClearError(dateTimePicker);
            return true;
        }

        /// <summary>
        /// Validate age (from date of birth)
        /// </summary>
        public bool ValidateAge(DateTimePicker dateTimePicker, string fieldName, int minAge = 0, int maxAge = 150)
        {
            DateTime birthDate = dateTimePicker.Value.Date;
            DateTime today = DateTime.Today;

            if (birthDate > today)
            {
                SetError(dateTimePicker, $"{fieldName} không được ở tương lai");
                return false;
            }

            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;

            if (age < minAge || age > maxAge)
            {
                SetError(dateTimePicker, $"Tuổi phải từ {minAge} đến {maxAge}");
                return false;
            }

            ClearError(dateTimePicker);
            return true;
        }

        #endregion

        #region Custom Validation

        /// <summary>
        /// Validate with custom rule
        /// </summary>
        public bool ValidateCustom(Control control, Func<bool> validationFunc, string errorMessage)
        {
            if (!validationFunc())
            {
                SetError(control, errorMessage);
                return false;
            }

            ClearError(control);
            return true;
        }

        /// <summary>
        /// Validate text length
        /// </summary>
        public bool ValidateLength(TextBox textBox, string fieldName, int minLength = 0, int maxLength = int.MaxValue)
        {
            string text = textBox.Text.Trim();
            int length = text.Length;

            if (length < minLength)
            {
                SetError(textBox, $"{fieldName} phải có ít nhất {minLength} ký tự");
                return false;
            }

            if (length > maxLength)
            {
                SetError(textBox, $"{fieldName} không được quá {maxLength} ký tự");
                return false;
            }

            ClearError(textBox);
            return true;
        }

        /// <summary>
        /// Validate password strength
        /// </summary>
        public bool ValidatePassword(TextBox textBox, out string errorMessage)
        {
            return PasswordHasher.ValidatePasswordStrength(textBox.Text, out errorMessage);
        }

        /// <summary>
        /// Validate password confirmation
        /// </summary>
        public bool ValidatePasswordMatch(TextBox passwordTextBox, TextBox confirmTextBox)
        {
            if (passwordTextBox.Text != confirmTextBox.Text)
            {
                SetError(confirmTextBox, "Mật khẩu xác nhận không khớp");
                return false;
            }

            ClearError(confirmTextBox);
            return true;
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Set error with visual feedback
        /// </summary>
        private void SetError(Control control, string errorMessage)
        {
            errorProvider.SetError(control, errorMessage);

            // Change background color for TextBox
            if (control is TextBox textBox)
            {
                textBox.BackColor = InvalidColor;
            }
        }

        /// <summary>
        /// Clear error
        /// </summary>
        public void ClearError(Control control)
        {
            errorProvider.SetError(control, string.Empty);

            // Reset background color
            if (control is TextBox textBox)
            {
                textBox.BackColor = ValidColor;
            }
        }

        /// <summary>
        /// Clear all errors in form
        /// </summary>
        public void ClearAllErrors()
        {
            errorProvider.Clear();

            // Reset all textbox colors
            foreach (Control control in GetAllControls(parentForm))
            {
                if (control is TextBox textBox)
                {
                    textBox.BackColor = ValidColor;
                }
            }
        }

        /// <summary>
        /// Setup real-time validation for TextBox
        /// </summary>
        public void SetupRealtimeValidation(TextBox textBox, Func<TextBox, bool> validationFunc)
        {
            // Validate on Leave
            textBox.Leave += (s, e) => validationFunc(textBox);

            // Visual feedback on focus
            textBox.Enter += (s, e) =>
            {
                if (textBox.BackColor == ValidColor)
                {
                    textBox.BackColor = FocusColor;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (textBox.BackColor == FocusColor)
                {
                    textBox.BackColor = ValidColor;
                }
            };
        }

        /// <summary>
        /// Setup numeric-only input
        /// </summary>
        public void SetupNumericInput(TextBox textBox, bool allowDecimal = false)
        {
            textBox.KeyPress += (s, e) =>
            {
                // Allow control keys (backspace, delete, etc.)
                if (char.IsControl(e.KeyChar))
                    return;

                // Allow digits
                if (char.IsDigit(e.KeyChar))
                    return;

                // Allow decimal point (only one)
                if (allowDecimal && e.KeyChar == '.' && !textBox.Text.Contains("."))
                    return;

                // Block all other characters
                e.Handled = true;
            };
        }

        #endregion

        #region Validation Groups

        /// <summary>
        /// Validate all controls in a group
        /// </summary>
        public bool ValidateAll(params (Control control, Func<bool> validationFunc)[] validations)
        {
            bool isValid = true;

            foreach (var (control, validationFunc) in validations)
            {
                if (!validationFunc())
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validate form before submit
        /// </summary>
        public bool ValidateForm(params Func<bool>[] validationFunctions)
        {
            ClearAllErrors();
            bool isValid = true;

            foreach (var validationFunc in validationFunctions)
            {
                if (!validationFunc())
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                MessageBoxHelper.ShowWarning("Vui lòng kiểm tra lại các trường đã đánh dấu đỏ!");
            }

            return isValid;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get all controls recursively
        /// </summary>
        private System.Collections.Generic.IEnumerable<Control> GetAllControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                yield return control;

                if (control.HasChildren)
                {
                    foreach (Control child in GetAllControls(control))
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            errorProvider?.Dispose();
        }

        #endregion
    }
}
