using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Pagination helper for DataGridView to handle large datasets
    /// Uses virtual scrolling and lazy loading to prevent memory issues
    /// </summary>
    public class PaginationHelper
    {
        private DataGridView dataGridView;
        private Func<int, int, Task<DataTable>> loadDataFunc;
        private Func<Task<int>> getTotalCountFunc;
        
        private int currentPage = 1;
        private int pageSize = 100; // Default: 100 rows per page
        private int totalRecords = 0;
        private int totalPages = 0;
        
        private Panel paginationPanel;
        private Label lblPageInfo;
        private Button btnFirst;
        private Button btnPrevious;
        private Button btnNext;
        private Button btnLast;
        private ComboBox cmbPageSize;
        private TextBox txtGoToPage;
        private Button btnGoToPage;

        public int CurrentPage => currentPage;
        public int PageSize => pageSize;
        public int TotalRecords => totalRecords;
        public int TotalPages => totalPages;

        /// <summary>
        /// Initialize pagination for DataGridView
        /// </summary>
        /// <param name="dgv">DataGridView to paginate</param>
        /// <param name="loadDataFunc">Function to load data (pageNumber, pageSize) => DataTable</param>
        /// <param name="getTotalCountFunc">Function to get total record count</param>
        /// <param name="defaultPageSize">Default page size (default: 100)</param>
        public PaginationHelper(
            DataGridView dgv,
            Func<int, int, Task<DataTable>> loadDataFunc,
            Func<Task<int>> getTotalCountFunc,
            int defaultPageSize = 100)
        {
            this.dataGridView = dgv;
            this.loadDataFunc = loadDataFunc;
            this.getTotalCountFunc = getTotalCountFunc;
            this.pageSize = defaultPageSize;

            CreatePaginationControls();
        }

        #region Initialization

        private void CreatePaginationControls()
        {
            // Create panel for pagination controls
            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240)
            };

            // Page info label
            lblPageInfo = new Label
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };

            // First page button
            btnFirst = new Button
            {
                Text = "<<",
                Location = new System.Drawing.Point(220, 8),
                Size = new System.Drawing.Size(40, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            btnFirst.Click += async (s, e) => await GoToPageAsync(1);

            // Previous page button
            btnPrevious = new Button
            {
                Text = "<",
                Location = new System.Drawing.Point(265, 8),
                Size = new System.Drawing.Size(40, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            btnPrevious.Click += async (s, e) => await GoToPageAsync(currentPage - 1);

            // Next page button
            btnNext = new Button
            {
                Text = ">",
                Location = new System.Drawing.Point(310, 8),
                Size = new System.Drawing.Size(40, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            btnNext.Click += async (s, e) => await GoToPageAsync(currentPage + 1);

            // Last page button
            btnLast = new Button
            {
                Text = ">>",
                Location = new System.Drawing.Point(355, 8),
                Size = new System.Drawing.Size(40, 25),
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            btnLast.Click += async (s, e) => await GoToPageAsync(totalPages);

            // Page size selector
            Label lblPageSize = new Label
            {
                Text = "Rows:",
                Location = new System.Drawing.Point(410, 12),
                Size = new System.Drawing.Size(45, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            cmbPageSize = new ComboBox
            {
                Location = new System.Drawing.Point(460, 9),
                Size = new System.Drawing.Size(70, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPageSize.Items.AddRange(new object[] { 50, 100, 200, 500, 1000 });
            cmbPageSize.SelectedItem = pageSize;
            cmbPageSize.SelectedIndexChanged += async (s, e) =>
            {
                pageSize = Convert.ToInt32(cmbPageSize.SelectedItem);
                await RefreshAsync();
            };

            // Go to page
            Label lblGoTo = new Label
            {
                Text = "Go to:",
                Location = new System.Drawing.Point(545, 12),
                Size = new System.Drawing.Size(50, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            txtGoToPage = new TextBox
            {
                Location = new System.Drawing.Point(600, 9),
                Size = new System.Drawing.Size(60, 25)
            };
            txtGoToPage.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true;
            };

            btnGoToPage = new Button
            {
                Text = "Go",
                Location = new System.Drawing.Point(665, 8),
                Size = new System.Drawing.Size(50, 25),
                FlatStyle = FlatStyle.Flat
            };
            btnGoToPage.Click += async (s, e) =>
            {
                if (int.TryParse(txtGoToPage.Text, out int page))
                {
                    await GoToPageAsync(page);
                }
            };

            // Add controls to panel
            paginationPanel.Controls.Add(lblPageInfo);
            paginationPanel.Controls.Add(btnFirst);
            paginationPanel.Controls.Add(btnPrevious);
            paginationPanel.Controls.Add(btnNext);
            paginationPanel.Controls.Add(btnLast);
            paginationPanel.Controls.Add(lblPageSize);
            paginationPanel.Controls.Add(cmbPageSize);
            paginationPanel.Controls.Add(lblGoTo);
            paginationPanel.Controls.Add(txtGoToPage);
            paginationPanel.Controls.Add(btnGoToPage);

            // Add panel to DataGridView parent
            if (dataGridView.Parent != null)
            {
                dataGridView.Parent.Controls.Add(paginationPanel);
                paginationPanel.BringToFront();
            }
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Load first page
        /// </summary>
        public async Task InitializeAsync()
        {
            await RefreshAsync();
        }

        /// <summary>
        /// Go to specific page
        /// </summary>
        public async Task GoToPageAsync(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > totalPages)
                return;

            currentPage = pageNumber;
            await LoadPageAsync();
        }

        /// <summary>
        /// Refresh current page
        /// </summary>
        public async Task RefreshAsync()
        {
            // Get total count
            totalRecords = await getTotalCountFunc();
            totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Ensure current page is valid
            if (currentPage > totalPages)
                currentPage = Math.Max(1, totalPages);

            await LoadPageAsync();
        }

        /// <summary>
        /// Load current page data
        /// </summary>
        private async Task LoadPageAsync()
        {
            try
            {
                // Show loading indicator
                dataGridView.Enabled = false;
                dataGridView.Cursor = Cursors.WaitCursor;

                // Load data for current page
                var data = await loadDataFunc(currentPage, pageSize);
                
                // Bind to DataGridView
                dataGridView.DataSource = data;

                // Update UI
                UpdatePaginationUI();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Lỗi khi tải dữ liệu phân trang");
            }
            finally
            {
                dataGridView.Enabled = true;
                dataGridView.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region UI Updates

        private void UpdatePaginationUI()
        {
            // Update page info
            int startRecord = (currentPage - 1) * pageSize + 1;
            int endRecord = Math.Min(currentPage * pageSize, totalRecords);
            
            lblPageInfo.Text = $"Page {currentPage} of {totalPages} ({startRecord}-{endRecord} of {totalRecords} records)";

            // Update button states
            btnFirst.Enabled = currentPage > 1;
            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
            btnLast.Enabled = currentPage < totalPages;

            // Clear go to page textbox
            txtGoToPage.Text = "";
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get pagination panel to add to form
        /// </summary>
        public Panel GetPaginationPanel()
        {
            return paginationPanel;
        }

        /// <summary>
        /// Show/hide pagination controls
        /// </summary>
        public void SetVisible(bool visible)
        {
            paginationPanel.Visible = visible;
        }

        /// <summary>
        /// Dispose pagination controls
        /// </summary>
        public void Dispose()
        {
            paginationPanel?.Dispose();
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Quick setup pagination for DataGridView
        /// </summary>
        public static async Task<PaginationHelper> SetupAsync(
            DataGridView dgv,
            Func<int, int, Task<DataTable>> loadDataFunc,
            Func<Task<int>> getTotalCountFunc,
            int pageSize = 100)
        {
            var helper = new PaginationHelper(dgv, loadDataFunc, getTotalCountFunc, pageSize);
            await helper.InitializeAsync();
            return helper;
        }

        /// <summary>
        /// Create pagination query for SQL Server
        /// </summary>
        public static string CreatePaginationQuery(string baseQuery, string orderBy, int pageNumber, int pageSize)
        {
            int offset = (pageNumber - 1) * pageSize;
            
            return $@"
                {baseQuery}
                ORDER BY {orderBy}
                OFFSET {offset} ROWS
                FETCH NEXT {pageSize} ROWS ONLY";
        }

        /// <summary>
        /// Create count query from base query
        /// </summary>
        public static string CreateCountQuery(string baseQuery)
        {
            // Simple approach: wrap in SELECT COUNT(*)
            return $"SELECT COUNT(*) FROM ({baseQuery}) AS CountQuery";
        }

        #endregion
    }

    /// <summary>
    /// Virtual scrolling helper for very large datasets (10,000+ records)
    /// Only loads visible rows + buffer
    /// </summary>
    public class VirtualScrollHelper
    {
        private DataGridView dataGridView;
        private Func<int, int, Task<DataTable>> loadDataFunc;
        private int totalRecords;
        private int bufferSize = 50; // Load 50 rows before/after visible range
        private DataTable cachedData;
        private int cachedStartIndex = -1;
        private int cachedEndIndex = -1;

        public VirtualScrollHelper(DataGridView dgv, Func<int, int, Task<DataTable>> loadDataFunc, int totalRecords)
        {
            this.dataGridView = dgv;
            this.loadDataFunc = loadDataFunc;
            this.totalRecords = totalRecords;

            SetupVirtualMode();
        }

        private void SetupVirtualMode()
        {
            dataGridView.VirtualMode = true;
            dataGridView.RowCount = totalRecords;

            dataGridView.CellValueNeeded += DataGridView_CellValueNeeded;
            dataGridView.Scroll += async (s, e) => await OnScrollAsync();
        }

        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // Check if row is in cached range
            if (cachedData != null && e.RowIndex >= cachedStartIndex && e.RowIndex < cachedEndIndex)
            {
                int localIndex = e.RowIndex - cachedStartIndex;
                if (localIndex < cachedData.Rows.Count)
                {
                    e.Value = cachedData.Rows[localIndex][e.ColumnIndex];
                }
            }
        }

        private async Task OnScrollAsync()
        {
            int firstVisible = dataGridView.FirstDisplayedScrollingRowIndex;
            if (firstVisible == -1) return;

            int visibleCount = dataGridView.DisplayedRowCount(false);
            
            // Calculate range to load
            int startIndex = Math.Max(0, firstVisible - bufferSize);
            int endIndex = Math.Min(totalRecords, firstVisible + visibleCount + bufferSize);

            // Check if we need to load new data
            if (startIndex < cachedStartIndex || endIndex > cachedEndIndex)
            {
                await LoadDataRangeAsync(startIndex, endIndex - startIndex);
            }
        }

        private async Task LoadDataRangeAsync(int startIndex, int count)
        {
            try
            {
                cachedData = await loadDataFunc(startIndex, count);
                cachedStartIndex = startIndex;
                cachedEndIndex = startIndex + cachedData.Rows.Count;

                dataGridView.Invalidate();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Lỗi khi tải dữ liệu virtual scroll");
            }
        }

        public async Task InitializeAsync()
        {
            // Load first visible rows
            await LoadDataRangeAsync(0, bufferSize * 2);
        }
    }
}
