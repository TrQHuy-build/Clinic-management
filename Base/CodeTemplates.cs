using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Base
{
    /// <summary>
    /// Reusable code templates to eliminate duplication
    /// </summary>
    public static class CodeTemplates
    {
        #region Try-Catch Templates

        /// <summary>
        /// Execute action with standard error handling
        /// </summary>
        public static void Execute(Action action, string errorMessage = "Đã xảy ra lỗi")
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, errorMessage);
            }
        }

        /// <summary>
        /// Execute async action with standard error handling
        /// </summary>
        public static async System.Threading.Tasks.Task ExecuteAsync(
            Func<System.Threading.Tasks.Task> action,
            string errorMessage = "Đã xảy ra lỗi")
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, errorMessage);
            }
        }

        /// <summary>
        /// Execute action with loading indicator
        /// </summary>
        public static async System.Threading.Tasks.Task ExecuteWithLoadingAsync(
            Form parentForm,
            Func<System.Threading.Tasks.Task> action,
            string loadingMessage = "Đang xử lý...",
            string errorMessage = "Đã xảy ra lỗi")
        {
            var loadingHelper = new LoadingHelper(parentForm);
            
            try
            {
                await loadingHelper.ExecuteWithLoadingAsync(action, loadingMessage);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, errorMessage);
            }
        }

        /// <summary>
        /// Execute function with result and error handling
        /// </summary>
        public static T ExecuteWithResult<T>(Func<T> func, T defaultValue = default(T), string errorMessage = "Đã xảy ra lỗi")
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, errorMessage);
                return defaultValue;
            }
        }

        #endregion

        #region Validation Templates

        /// <summary>
        /// Validate required fields
        /// </summary>
        public static bool ValidateRequiredFields(ValidationHelper validator, params (Control control, string fieldName)[] fields)
        {
            bool isValid = true;

            foreach (var (control, fieldName) in fields)
            {
                if (control is TextBox textBox)
                {
                    if (!validator.ValidateRequired(textBox, fieldName))
                        isValid = false;
                }
                else if (control is ComboBox comboBox)
                {
                    if (!validator.ValidateRequired(comboBox, fieldName))
                        isValid = false;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    if (!validator.ValidateRequired(dateTimePicker, fieldName))
                        isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Setup real-time validation for multiple controls
        /// </summary>
        public static void SetupRealtimeValidation(
            ValidationHelper validator,
            params (TextBox textBox, Func<TextBox, bool> validationFunc)[] validations)
        {
            foreach (var (textBox, validationFunc) in validations)
            {
                validator.SetupRealtimeValidation(textBox, validationFunc);
            }
        }

        #endregion

        #region Confirmation Templates

        /// <summary>
        /// Confirm action with Yes/No dialog
        /// </summary>
        public static bool ConfirmAction(string message, string title = "Xác nhận")
        {
            return MessageBoxHelper.ShowConfirm(message, title);
        }

        /// <summary>
        /// Confirm delete action
        /// </summary>
        public static bool ConfirmDelete(string itemName = "mục này")
        {
            return MessageBoxHelper.ShowConfirm($"Bạn có chắc muốn xóa {itemName}?", "Xác nhận xóa");
        }

        /// <summary>
        /// Confirm save action if data changed
        /// </summary>
        public static DialogResult ConfirmSaveChanges()
        {
            return MessageBoxHelper.ShowYesNoCancel(
                "Dữ liệu đã thay đổi. Bạn có muốn lưu?",
                "Lưu thay đổi"
            );
        }

        #endregion

        #region Form Opening Templates

        /// <summary>
        /// Open form as dialog
        /// </summary>
        public static DialogResult OpenDialog(Form form)
        {
            return Execute(() => form.ShowDialog(), DialogResult.None, "Không thể mở form");
        }

        /// <summary>
        /// Open form and refresh parent on OK
        /// </summary>
        public static async System.Threading.Tasks.Task<bool> OpenDialogAndRefresh(
            Form form,
            Func<System.Threading.Tasks.Task> refreshAction)
        {
            var result = form.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                await refreshAction();
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Show form in panel (MDI)
        /// </summary>
        public static void ShowInPanel(UserControl control, Panel container)
        {
            Execute(() =>
            {
                container.Controls.Clear();
                control.Dock = DockStyle.Fill;
                container.Controls.Add(control);
            }, "Không thể hiển thị form");
        }

        #endregion

        #region DataGridView Templates

        /// <summary>
        /// Get selected row value
        /// </summary>
        public static T GetSelectedRowValue<T>(DataGridView dgv, string columnName, T defaultValue = default(T))
        {
            try
            {
                if (dgv.SelectedRows.Count == 0)
                    return defaultValue;

                var value = dgv.SelectedRows[0].Cells[columnName].Value;
                
                if (value == null || value == DBNull.Value)
                    return defaultValue;

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get selected row ID
        /// </summary>
        public static int GetSelectedId(DataGridView dgv, string idColumnName = "id")
        {
            return GetSelectedRowValue(dgv, idColumnName, 0);
        }

        /// <summary>
        /// Clear and fill DataGridView
        /// </summary>
        public static void RefreshDataGridView(DataGridView dgv, System.Data.DataTable dt)
        {
            Execute(() =>
            {
                dgv.DataSource = null;
                dgv.DataSource = dt;
            }, "Không thể cập nhật dữ liệu");
        }

        /// <summary>
        /// Find row by column value
        /// </summary>
        public static DataGridViewRow FindRow(DataGridView dgv, string columnName, object value)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[columnName].Value?.Equals(value) == true)
                {
                    return row;
                }
            }
            return null;
        }

        /// <summary>
        /// Select row by ID
        /// </summary>
        public static void SelectRowById(DataGridView dgv, int id, string idColumnName = "id")
        {
            var row = FindRow(dgv, idColumnName, id);
            if (row != null)
            {
                dgv.ClearSelection();
                row.Selected = true;
                dgv.FirstDisplayedScrollingRowIndex = row.Index;
            }
        }

        #endregion

        #region Common Patterns

        /// <summary>
        /// Initialize form pattern
        /// </summary>
        public static void InitializeForm(
            Form form,
            string title,
            int width,
            int height,
            FormStartPosition startPosition = FormStartPosition.CenterScreen)
        {
            form.Text = title;
            form.Size = new System.Drawing.Size(width, height);
            form.StartPosition = startPosition;
            form.Font = new System.Drawing.Font("Segoe UI", 9);
        }

        /// <summary>
        /// Setup search textbox
        /// </summary>
        public static void SetupSearchBox(
            TextBox txtSearch,
            DataGridView dgv,
            Func<string, bool> filterFunc)
        {
            txtSearch.TextChanged += (s, e) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                
                if (string.IsNullOrEmpty(searchText))
                {
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        row.Visible = true;
                    }
                }
                else
                {
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        bool matches = false;
                        
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null &&
                                cell.Value.ToString().ToLower().Contains(searchText))
                            {
                                matches = true;
                                break;
                            }
                        }
                        
                        row.Visible = matches;
                    }
                }
            };
        }

        /// <summary>
        /// Bind ComboBox from enum
        /// </summary>
        public static void BindComboBoxFromEnum<TEnum>(ComboBox comboBox) where TEnum : Enum
        {
            comboBox.Items.Clear();
            foreach (var value in Enum.GetValues(typeof(TEnum)))
            {
                comboBox.Items.Add(value);
            }
            
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Bind ComboBox from list
        /// </summary>
        public static void BindComboBox<T>(
            ComboBox comboBox,
            List<T> items,
            string displayMember,
            string valueMember)
        {
            comboBox.DataSource = items;
            comboBox.DisplayMember = displayMember;
            comboBox.ValueMember = valueMember;
        }

        /// <summary>
        /// Clear form controls
        /// </summary>
        public static void ClearFormControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectedIndex = -1;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.Value = DateTime.Now;
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.Value = numericUpDown.Minimum;
                }
                else if (control.HasChildren)
                {
                    ClearFormControls(control); // Recursive
                }
            }
        }

        /// <summary>
        /// Enable/disable form controls
        /// </summary>
        public static void SetFormControlsEnabled(Control parent, bool enabled, params Control[] excludedControls)
        {
            var excludedSet = new HashSet<Control>(excludedControls);

            foreach (Control control in parent.Controls)
            {
                if (!excludedSet.Contains(control))
                {
                    control.Enabled = enabled;
                    
                    if (control.HasChildren)
                    {
                        SetFormControlsEnabled(control, enabled, excludedControls);
                    }
                }
            }
        }

        #endregion

        #region Save/Load Patterns

        /// <summary>
        /// Standard save pattern with validation
        /// </summary>
        public static async System.Threading.Tasks.Task<bool> SaveWithValidationAsync(
            ValidationHelper validator,
            Func<bool> validateFunc,
            Func<System.Threading.Tasks.Task> saveFunc,
            string successMessage = "Lưu thành công!",
            string errorMessage = "Không thể lưu dữ liệu")
        {
            try
            {
                // Validate
                if (!validateFunc())
                {
                    return false;
                }

                // Save
                await saveFunc();

                // Success
                MessageBoxHelper.ShowSuccess(successMessage);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, errorMessage);
                return false;
            }
        }

        /// <summary>
        /// Standard load pattern with loading indicator
        /// </summary>
        public static async System.Threading.Tasks.Task<T> LoadWithIndicatorAsync<T>(
            Form parentForm,
            Func<System.Threading.Tasks.Task<T>> loadFunc,
            string loadingMessage = "Đang tải dữ liệu...",
            string errorMessage = "Không thể tải dữ liệu")
        {
            var loadingHelper = new LoadingHelper(parentForm);

            try
            {
                return await loadingHelper.ExecuteWithLoadingAsync(loadFunc, loadingMessage);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, errorMessage);
                return default(T);
            }
        }

        #endregion

        #region Async Helpers

        /// <summary>
        /// Run async method synchronously (use with caution!)
        /// </summary>
        public static T RunSync<T>(Func<System.Threading.Tasks.Task<T>> func)
        {
            return System.Threading.Tasks.Task.Run(func).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Run async method synchronously (use with caution!)
        /// </summary>
        public static void RunSync(Func<System.Threading.Tasks.Task> func)
        {
            System.Threading.Tasks.Task.Run(func).GetAwaiter().GetResult();
        }

        #endregion
    }
}
