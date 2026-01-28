using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Base
{
    /// <summary>
    /// Template configurations for DataGridView
    /// Eliminates repetitive DataGridView setup code
    /// </summary>
    public static class DataGridViewTemplate
    {
        #region Predefined Templates

        /// <summary>
        /// Basic template with common settings
        /// </summary>
        public static void ApplyBasicTemplate(this DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersHeight = 35;
            dgv.RowTemplate.Height = 30;

            // Double buffering for smooth scrolling
            if (!System.Linq.Enumerable.Contains(dgv.GetType().GetProperties(),
                dgv.GetType().GetProperty("DoubleBuffered")))
            {
                typeof(DataGridView).InvokeMember("DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                    null, dgv, new object[] { true });
            }
        }

        /// <summary>
        /// Modern template with styling
        /// </summary>
        public static void ApplyModernTemplate(this DataGridView dgv)
        {
            ApplyBasicTemplate(dgv);

            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 122, 204);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);

            // Alternate row colors
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);

            // Row style
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.DefaultCellStyle.Padding = new Padding(5, 3, 5, 3);

            // Hover effect
            dgv.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255);
                }
            };

            dgv.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(240, 248, 255);
                }
            };
        }

        /// <summary>
        /// Editable template (allows editing)
        /// </summary>
        public static void ApplyEditableTemplate(this DataGridView dgv)
        {
            ApplyBasicTemplate(dgv);
            dgv.ReadOnly = false;
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
        }

        /// <summary>
        /// Compact template (smaller rows/columns)
        /// </summary>
        public static void ApplyCompactTemplate(this DataGridView dgv)
        {
            ApplyModernTemplate(dgv);
            dgv.ColumnHeadersHeight = 28;
            dgv.RowTemplate.Height = 24;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 8);
        }

        /// <summary>
        /// Large template (bigger for touch screens)
        /// </summary>
        public static void ApplyLargeTemplate(this DataGridView dgv)
        {
            ApplyModernTemplate(dgv);
            dgv.ColumnHeadersHeight = 45;
            dgv.RowTemplate.Height = 40;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11);
        }

        #endregion

        #region Column Factory Methods

        /// <summary>
        /// Create text column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateTextColumn(
            string name,
            string headerText,
            int? width = null,
            bool visible = true)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                Visible = visible,
                SortMode = DataGridViewColumnSortMode.Automatic
            };

            if (width.HasValue)
                column.Width = width.Value;
            else
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            return column;
        }

        /// <summary>
        /// Create ID column (hidden)
        /// </summary>
        public static DataGridViewTextBoxColumn CreateIdColumn(string name = "id")
        {
            return CreateTextColumn(name, "ID", 50, visible: false);
        }

        /// <summary>
        /// Create date column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateDateColumn(
            string name,
            string headerText,
            int? width = null)
        {
            var column = CreateTextColumn(name, headerText, width);
            column.DefaultCellStyle.Format = "dd/MM/yyyy";
            return column;
        }

        /// <summary>
        /// Create datetime column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateDateTimeColumn(
            string name,
            string headerText,
            int? width = null)
        {
            var column = CreateTextColumn(name, headerText, width);
            column.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            return column;
        }

        /// <summary>
        /// Create currency column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateCurrencyColumn(
            string name,
            string headerText,
            int? width = null)
        {
            var column = CreateTextColumn(name, headerText, width);
            column.DefaultCellStyle.Format = "#,##0 ₫";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            return column;
        }

        /// <summary>
        /// Create number column
        /// </summary>
        public static DataGridViewTextBoxColumn CreateNumberColumn(
            string name,
            string headerText,
            int? width = null,
            string format = "#,##0")
        {
            var column = CreateTextColumn(name, headerText, width);
            column.DefaultCellStyle.Format = format;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            return column;
        }

        /// <summary>
        /// Create checkbox column
        /// </summary>
        public static DataGridViewCheckBoxColumn CreateCheckBoxColumn(
            string name,
            string headerText,
            int? width = null)
        {
            return new DataGridViewCheckBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                Width = width ?? 60,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
        }

        /// <summary>
        /// Create image column
        /// </summary>
        public static DataGridViewImageColumn CreateImageColumn(
            string name,
            string headerText,
            int? width = null)
        {
            return new DataGridViewImageColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                Width = width ?? 100,
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
        }

        /// <summary>
        /// Create combobox column
        /// </summary>
        public static DataGridViewComboBoxColumn CreateComboBoxColumn(
            string name,
            string headerText,
            object[] items,
            int? width = null)
        {
            var column = new DataGridViewComboBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                Width = width ?? 150
            };

            column.Items.AddRange(items);
            return column;
        }

        #endregion

        #region Quick Setup Methods

        /// <summary>
        /// Quick setup for invoices DataGridView
        /// </summary>
        public static void SetupForInvoices(this DataGridView dgv)
        {
            dgv.ApplyModernTemplate();

            dgv.Columns.Add(CreateIdColumn());
            dgv.Columns.Add(CreateTextColumn("patient_name", "Tên bệnh nhân", 200));
            dgv.Columns.Add(CreateTextColumn("service", "Dịch vụ", 250));
            dgv.Columns.Add(CreateCurrencyColumn("total_amount", "Tổng tiền", 120));
            dgv.Columns.Add(CreateCurrencyColumn("amount_paid", "Đã thanh toán", 120));
            dgv.Columns.Add(CreateDateColumn("invoice_date", "Ngày", 100));
            dgv.Columns.Add(CreateTextColumn("status", "Trạng thái", 120));

            // Style status column
            dgv.StyleStatusColumn("status", new[]
            {
                StatusColorMap.Success("Đã thanh toán"),
                StatusColorMap.Warning("Chờ thanh toán"),
                StatusColorMap.Danger("Quá hạn")
            });
        }

        /// <summary>
        /// Quick setup for patients DataGridView
        /// </summary>
        public static void SetupForPatients(this DataGridView dgv)
        {
            dgv.ApplyModernTemplate();

            dgv.Columns.Add(CreateIdColumn());
            dgv.Columns.Add(CreateTextColumn("full_name", "Họ tên", 200));
            dgv.Columns.Add(CreateTextColumn("phone", "Số điện thoại", 120));
            dgv.Columns.Add(CreateTextColumn("email", "Email", 200));
            dgv.Columns.Add(CreateDateColumn("date_of_birth", "Ngày sinh", 100));
            dgv.Columns.Add(CreateTextColumn("gender", "Giới tính", 80));
            dgv.Columns.Add(CreateTextColumn("address", "Địa chỉ", 300));
        }

        /// <summary>
        /// Quick setup for appointments DataGridView
        /// </summary>
        public static void SetupForAppointments(this DataGridView dgv)
        {
            dgv.ApplyModernTemplate();

            dgv.Columns.Add(CreateIdColumn());
            dgv.Columns.Add(CreateTextColumn("patient_name", "Bệnh nhân", 180));
            dgv.Columns.Add(CreateTextColumn("doctor_name", "Bác sĩ", 150));
            dgv.Columns.Add(CreateDateColumn("appointment_date", "Ngày", 100));
            dgv.Columns.Add(CreateTextColumn("appointment_time", "Giờ", 80));
            dgv.Columns.Add(CreateTextColumn("service", "Dịch vụ", 200));
            dgv.Columns.Add(CreateTextColumn("status", "Trạng thái", 120));

            // Style status column
            dgv.StyleStatusColumn("status", new[]
            {
                StatusColorMap.Info("Đã đặt"),
                StatusColorMap.Warning("Đã xác nhận"),
                StatusColorMap.Success("Hoàn thành"),
                StatusColorMap.Danger("Đã hủy")
            });
        }

        /// <summary>
        /// Quick setup for services DataGridView
        /// </summary>
        public static void SetupForServices(this DataGridView dgv)
        {
            dgv.ApplyModernTemplate();

            dgv.Columns.Add(CreateIdColumn());
            dgv.Columns.Add(CreateTextColumn("service_name", "Tên dịch vụ", 250));
            dgv.Columns.Add(CreateTextColumn("description", "Mô tả", 350));
            dgv.Columns.Add(CreateCurrencyColumn("price", "Giá", 120));
            dgv.Columns.Add(CreateNumberColumn("duration", "Thời gian (phút)", 120));
        }

        /// <summary>
        /// Quick setup for medicines DataGridView
        /// </summary>
        public static void SetupForMedicines(this DataGridView dgv)
        {
            dgv.ApplyModernTemplate();

            dgv.Columns.Add(CreateIdColumn());
            dgv.Columns.Add(CreateTextColumn("medicine_name", "Tên thuốc", 200));
            dgv.Columns.Add(CreateTextColumn("description", "Mô tả", 300));
            dgv.Columns.Add(CreateCurrencyColumn("price", "Giá", 100));
            dgv.Columns.Add(CreateNumberColumn("stock_quantity", "Tồn kho", 100));
            dgv.Columns.Add(CreateTextColumn("unit", "Đơn vị", 80));
            dgv.Columns.Add(CreateDateColumn("expiry_date", "Hạn sử dụng", 100));
        }

        /// <summary>
        /// Quick setup for staff DataGridView
        /// </summary>
        public static void SetupForStaff(this DataGridView dgv)
        {
            dgv.ApplyModernTemplate();

            dgv.Columns.Add(CreateIdColumn());
            dgv.Columns.Add(CreateTextColumn("full_name", "Họ tên", 180));
            dgv.Columns.Add(CreateTextColumn("position", "Chức vụ", 120));
            dgv.Columns.Add(CreateTextColumn("phone", "Số điện thoại", 120));
            dgv.Columns.Add(CreateTextColumn("email", "Email", 200));
            dgv.Columns.Add(CreateTextColumn("specialty", "Chuyên môn", 150));
            dgv.Columns.Add(CreateTextColumn("status", "Trạng thái", 100));

            // Style status column
            dgv.StyleStatusColumn("status", new[]
            {
                StatusColorMap.Success("Đang làm việc"),
                StatusColorMap.Warning("Nghỉ phép"),
                StatusColorMap.Danger("Đã nghỉ")
            });
        }

        #endregion

        #region Styling Helpers

        /// <summary>
        /// Add alternating row colors
        /// </summary>
        public static void AddAlternatingRowColors(this DataGridView dgv, Color color1, Color color2)
        {
            dgv.AlternatingRowsDefaultCellStyle.BackColor = color2;
            dgv.DefaultCellStyle.BackColor = color1;
        }

        /// <summary>
        /// Add row hover effect
        /// </summary>
        public static void AddRowHoverEffect(this DataGridView dgv, Color hoverColor)
        {
            dgv.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = hoverColor;
                }
            };

            dgv.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        e.RowIndex % 2 == 0 ? dgv.DefaultCellStyle.BackColor : dgv.AlternatingRowsDefaultCellStyle.BackColor;
                }
            };
        }

        /// <summary>
        /// Set header style
        /// </summary>
        public static void SetHeaderStyle(this DataGridView dgv, Color backColor, Color foreColor, Font font = null)
        {
            dgv.ColumnHeadersDefaultCellStyle.BackColor = backColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = foreColor;
            
            if (font != null)
                dgv.ColumnHeadersDefaultCellStyle.Font = font;
        }

        /// <summary>
        /// Auto-size all columns
        /// </summary>
        public static void AutoSizeAllColumns(this DataGridView dgv, DataGridViewAutoSizeColumnsMode mode = DataGridViewAutoSizeColumnsMode.Fill)
        {
            dgv.AutoSizeColumnsMode = mode;
        }

        #endregion
    }
}
