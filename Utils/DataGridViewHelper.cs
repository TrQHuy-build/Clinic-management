using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Helper class for DataGridView enhancements
    /// ✅ Replace button columns with context menu
    /// ✅ Better styling and formatting
    /// ✅ Row highlighting on hover
    /// ✅ Common DataGridView configurations
    /// </summary>
    public static class DataGridViewHelper
    {
        // Color scheme
        private static readonly Color HeaderBackColor = Color.FromArgb(0, 122, 204);
        private static readonly Color HeaderForeColor = Color.White;
        private static readonly Color AlternateRowColor = Color.FromArgb(245, 248, 252);
        private static readonly Color HoverRowColor = Color.FromArgb(230, 240, 255);
        private static readonly Color SelectionBackColor = Color.FromArgb(0, 122, 204);
        private static readonly Color SelectionForeColor = Color.White;

        #region Basic Styling

        /// <summary>
        /// Apply modern styling to DataGridView
        /// </summary>
        public static void ApplyModernStyle(this DataGridView dgv)
        {
            // Basic settings
            dgv.BorderStyle = BorderStyle.None;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternateRowColor;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = SelectionBackColor;
            dgv.DefaultCellStyle.SelectionForeColor = SelectionForeColor;
            dgv.BackgroundColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowTemplate.Height = 35;

            // Header styling
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = HeaderBackColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = HeaderForeColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgv.ColumnHeadersHeight = 40;

            // Cell styling
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgv.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);

            // Enable double buffering to reduce flicker
            if (!SystemInformation.TerminalServerSession)
            {
                Type dgvType = dgv.GetType();
                System.Reflection.PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                pi?.SetValue(dgv, true, null);
            }

            // Add row hover effect
            dgv.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = HoverRowColor;
                }
            };

            dgv.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    // Reset to default or alternate color
                    if (e.RowIndex % 2 == 0)
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = AlternateRowColor;
                    }
                }
            };
        }

        #endregion

        #region Context Menu (Replace Button Columns)

        /// <summary>
        /// Add context menu to DataGridView (replaces button columns)
        /// </summary>
        public static ContextMenuStrip AddContextMenu(this DataGridView dgv, params ContextMenuItem[] menuItems)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Font = new Font("Segoe UI", 9);

            foreach (var menuItem in menuItems)
            {
                if (menuItem.IsSeparator)
                {
                    contextMenu.Items.Add(new ToolStripSeparator());
                }
                else
                {
                    ToolStripMenuItem item = new ToolStripMenuItem
                    {
                        Text = menuItem.Text,
                        Image = menuItem.Icon,
                        Enabled = menuItem.Enabled
                    };

                    item.Click += (s, e) =>
                    {
                        if (dgv.CurrentRow != null)
                        {
                            menuItem.Action?.Invoke(dgv.CurrentRow);
                        }
                    };

                    contextMenu.Items.Add(item);
                }
            }

            dgv.ContextMenuStrip = contextMenu;

            // Show context menu on right-click or left-click on row
            dgv.CellMouseDown += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.Button == MouseButtons.Right)
                {
                    dgv.ClearSelection();
                    dgv.Rows[e.RowIndex].Selected = true;
                    dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[0];
                }
            };

            return contextMenu;
        }

        /// <summary>
        /// Add action column with icon button (cleaner than button columns)
        /// </summary>
        public static void AddActionColumn(this DataGridView dgv, string columnName, string headerText, Image icon, Action<DataGridViewRow> action)
        {
            DataGridViewImageColumn actionColumn = new DataGridViewImageColumn
            {
                Name = columnName,
                HeaderText = headerText,
                Width = 50,
                Image = icon ?? SystemIcons.Information.ToBitmap(),
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            dgv.Columns.Add(actionColumn);

            // Handle click on image cell
            dgv.CellContentClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgv.Columns[columnName].Index)
                {
                    action?.Invoke(dgv.Rows[e.RowIndex]);
                }
            };

            // Change cursor to hand on hover
            dgv.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgv.Columns[columnName].Index)
                {
                    dgv.Cursor = Cursors.Hand;
                }
            };

            dgv.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgv.Columns[columnName].Index)
                {
                    dgv.Cursor = Cursors.Default;
                }
            };
        }

        #endregion

        #region Column Configuration

        /// <summary>
        /// Hide column
        /// </summary>
        public static void HideColumn(this DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].Visible = false;
            }
        }

        /// <summary>
        /// Set column width
        /// </summary>
        public static void SetColumnWidth(this DataGridView dgv, string columnName, int width)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].Width = width;
                dgv.Columns[columnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        /// <summary>
        /// Set column header text
        /// </summary>
        public static void SetColumnHeader(this DataGridView dgv, string columnName, string headerText)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].HeaderText = headerText;
            }
        }

        /// <summary>
        /// Format currency column
        /// </summary>
        public static void FormatCurrencyColumn(this DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].DefaultCellStyle.Format = "N0";
                dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        /// <summary>
        /// Format date column
        /// </summary>
        public static void FormatDateColumn(this DataGridView dgv, string columnName, string format = "dd/MM/yyyy")
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].DefaultCellStyle.Format = format;
                dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// Format datetime column
        /// </summary>
        public static void FormatDateTimeColumn(this DataGridView dgv, string columnName, string format = "dd/MM/yyyy HH:mm")
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].DefaultCellStyle.Format = format;
                dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        /// <summary>
        /// Center align column
        /// </summary>
        public static void CenterAlignColumn(this DataGridView dgv, string columnName)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        #endregion

        #region Status Column Styling

        /// <summary>
        /// Style status column with colors
        /// </summary>
        public static void StyleStatusColumn(this DataGridView dgv, string columnName, StatusColorMap[] colorMaps)
        {
            dgv.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns[columnName].Index && e.RowIndex >= 0)
                {
                    string statusValue = e.Value?.ToString()?.ToLower();

                    foreach (var map in colorMaps)
                    {
                        if (statusValue == map.Value.ToLower())
                        {
                            e.CellStyle.BackColor = map.BackColor;
                            e.CellStyle.ForeColor = map.ForeColor;
                            e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Add status badge to cell
        /// </summary>
        public static void AddStatusBadge(this DataGridView dgv, string columnName)
        {
            dgv.CellPainting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns[columnName].Index && e.RowIndex >= 0)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    // Draw rounded rectangle as badge
                    using (var brush = new SolidBrush(e.CellStyle.BackColor))
                    {
                        Rectangle rect = new Rectangle(
                            e.CellBounds.X + 10,
                            e.CellBounds.Y + 5,
                            e.CellBounds.Width - 20,
                            e.CellBounds.Height - 10
                        );

                        int radius = 5;
                        var path = GetRoundedRectPath(rect, radius);
                        e.Graphics.FillPath(brush, path);

                        // Draw text
                        using (var textBrush = new SolidBrush(e.CellStyle.ForeColor))
                        {
                            StringFormat sf = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            };
                            e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, textBrush, rect, sf);
                        }
                    }

                    e.Handled = true;
                }
            };
        }

        #endregion

        #region Search and Filter

        /// <summary>
        /// Add search functionality
        /// </summary>
        public static void EnableSearch(this DataGridView dgv, TextBox searchTextBox)
        {
            searchTextBox.TextChanged += (s, e) =>
            {
                string searchText = searchTextBox.Text.ToLower();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    // Show all rows
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        row.Visible = true;
                    }
                }
                else
                {
                    // Filter rows
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        bool found = false;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText))
                            {
                                found = true;
                                break;
                            }
                        }
                        row.Visible = found;
                    }
                }
            };
        }

        #endregion

        #region Export

        /// <summary>
        /// Export to CSV
        /// </summary>
        public static void ExportToCsv(this DataGridView dgv, string filePath)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Write headers
                    var headers = new System.Collections.Generic.List<string>();
                    foreach (DataGridViewColumn column in dgv.Columns)
                    {
                        if (column.Visible)
                        {
                            headers.Add($"\"{column.HeaderText}\"");
                        }
                    }
                    writer.WriteLine(string.Join(",", headers));

                    // Write rows
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.Visible)
                        {
                            var values = new System.Collections.Generic.List<string>();
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.OwningColumn.Visible)
                                {
                                    string value = cell.Value?.ToString() ?? "";
                                    value = value.Replace("\"", "\"\""); // Escape quotes
                                    values.Add($"\"{value}\"");
                                }
                            }
                            writer.WriteLine(string.Join(",", values));
                        }
                    }
                }

                MessageBoxHelper.ShowSuccess($"Đã xuất dữ liệu thành công!\nFile: {filePath}");
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "DataGridViewHelper.ExportToCsv", showToUser: true);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get rounded rectangle path for drawing
        /// </summary>
        private static System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Context menu item configuration
    /// </summary>
    public class ContextMenuItem
    {
        public string Text { get; set; }
        public Image Icon { get; set; }
        public Action<DataGridViewRow> Action { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsSeparator { get; set; } = false;

        public static ContextMenuItem Separator()
        {
            return new ContextMenuItem { IsSeparator = true };
        }
    }

    /// <summary>
    /// Status color mapping
    /// </summary>
    public class StatusColorMap
    {
        public string Value { get; set; }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }

        public StatusColorMap(string value, Color backColor, Color foreColor)
        {
            Value = value;
            BackColor = backColor;
            ForeColor = foreColor;
        }

        // Common status colors
        public static StatusColorMap Success(string value) =>
            new StatusColorMap(value, Color.FromArgb(220, 255, 220), Color.FromArgb(0, 128, 0));

        public static StatusColorMap Warning(string value) =>
            new StatusColorMap(value, Color.FromArgb(255, 243, 205), Color.FromArgb(133, 100, 4));

        public static StatusColorMap Danger(string value) =>
            new StatusColorMap(value, Color.FromArgb(255, 220, 220), Color.FromArgb(200, 0, 0));

        public static StatusColorMap Info(string value) =>
            new StatusColorMap(value, Color.FromArgb(220, 240, 255), Color.FromArgb(0, 100, 200));

        public static StatusColorMap Default(string value) =>
            new StatusColorMap(value, Color.FromArgb(240, 240, 240), Color.FromArgb(80, 80, 80));
    }

    #endregion
}
