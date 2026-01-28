using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Base
{
    /// <summary>
    /// Base class for data pages with CRUD operations
    /// Eliminates code duplication across forms
    /// </summary>
    /// <typeparam name="T">Entity type (Patient, Invoice, etc.)</typeparam>
    public abstract class BaseDataPage<T> : UserControl where T : class, new()
    {
        #region Protected Fields

        protected DataGridView dataGridView;
        protected Panel panelTop;
        protected Panel panelBottom;
        protected TextBox txtSearch;
        protected Button btnAdd;
        protected Button btnRefresh;
        protected Button btnExport;
        
        protected QueryOptimizer queryOptimizer;
        protected CacheManager cacheManager;
        protected ValidationHelper validationHelper;
        protected LoadingHelper loadingHelper;
        protected PaginationHelper paginationHelper;

        protected List<T> currentData;
        protected T selectedItem;

        #endregion

        #region Constructor

        public BaseDataPage()
        {
            InitializeHelpers();
            InitializeBaseLayout();
            InitializeDataGridView();
            SetupEventHandlers();
        }

        #endregion

        #region Initialization

        private void InitializeHelpers()
        {
            queryOptimizer = new QueryOptimizer();
            cacheManager = CacheManager.Instance;
            validationHelper = new ValidationHelper(this);
            loadingHelper = new LoadingHelper(this);
        }

        private void InitializeBaseLayout()
        {
            this.Dock = DockStyle.Fill;

            // Top panel (search, buttons)
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            // Search box
            Label lblSearch = new Label
            {
                Text = "🔍 Tìm kiếm:",
                Location = new System.Drawing.Point(10, 18),
                Size = new System.Drawing.Size(80, 25),
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            panelTop.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new System.Drawing.Point(95, 15),
                Size = new System.Drawing.Size(250, 25),
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            txtSearch.TextChanged += async (s, e) => await SearchAsync();
            panelTop.Controls.Add(txtSearch);

            // Buttons
            btnAdd = CreateButton("➕ Thêm mới", 360, 12, System.Drawing.Color.FromArgb(40, 167, 69));
            btnAdd.Click += async (s, e) => await AddAsync();
            panelTop.Controls.Add(btnAdd);

            btnRefresh = CreateButton("🔄 Làm mới", 480, 12, System.Drawing.Color.FromArgb(23, 162, 184));
            btnRefresh.Click += async (s, e) => await RefreshAsync();
            panelTop.Controls.Add(btnRefresh);

            btnExport = CreateButton("📥 Export CSV", 600, 12, System.Drawing.Color.FromArgb(255, 193, 7));
            btnExport.Click += (s, e) => ExportToCsv();
            panelTop.Controls.Add(btnExport);

            this.Controls.Add(panelTop);

            // DataGridView (fills remaining space)
            dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(dataGridView);

            // Bottom panel (pagination controls will be added here)
            panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };
            this.Controls.Add(panelBottom);
        }

        private Button CreateButton(string text, int x, int y, System.Drawing.Color color)
        {
            return new Button
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(110, 35),
                BackColor = color,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
        }

        private void InitializeDataGridView()
        {
            // Apply modern style
            dataGridView.ApplyModernStyle();

            // Add columns
            var columns = GetColumns();
            foreach (var column in columns)
            {
                dataGridView.Columns.Add(column);
            }

            // Add context menu
            var contextMenuItems = GetContextMenuItems();
            if (contextMenuItems != null && contextMenuItems.Any())
            {
                dataGridView.AddContextMenu(contextMenuItems);
            }

            // Format columns
            FormatColumns();
        }

        private void SetupEventHandlers()
        {
            this.Load += async (s, e) => await OnPageLoadAsync();
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            dataGridView.CellDoubleClick += async (s, e) => await OnRowDoubleClickAsync(e.RowIndex);
        }

        #endregion

        #region Abstract Methods (Must Override)

        /// <summary>
        /// Get column definitions for DataGridView
        /// </summary>
        protected abstract List<DataGridViewColumn> GetColumns();

        /// <summary>
        /// Load data from database
        /// </summary>
        protected abstract Task<List<T>> LoadDataAsync();

        /// <summary>
        /// Convert entity to DataGridView row
        /// </summary>
        protected abstract object[] EntityToRow(T entity);

        /// <summary>
        /// Get entity from selected row
        /// </summary>
        protected abstract T RowToEntity(DataGridViewRow row);

        /// <summary>
        /// Get cache key for this page
        /// </summary>
        protected abstract string GetCacheKey();

        #endregion

        #region Virtual Methods (Can Override)

        /// <summary>
        /// Get context menu items (can override)
        /// </summary>
        protected virtual ContextMenuItem[] GetContextMenuItems()
        {
            return new[]
            {
                new ContextMenuItem
                {
                    Text = "👁 Xem chi tiết",
                    Action = async row => await ViewAsync()
                },
                new ContextMenuItem
                {
                    Text = "✏ Chỉnh sửa",
                    Action = async row => await EditAsync()
                },
                ContextMenuItem.Separator(),
                new ContextMenuItem
                {
                    Text = "❌ Xóa",
                    Action = async row => await DeleteAsync()
                }
            };
        }

        /// <summary>
        /// Format columns (currency, date, etc.)
        /// </summary>
        protected virtual void FormatColumns()
        {
            // Override to format specific columns
        }

        /// <summary>
        /// Filter data by search text
        /// </summary>
        protected virtual bool FilterEntity(T entity, string searchText)
        {
            // Default: convert to string and search
            return entity.ToString().ToLower().Contains(searchText.ToLower());
        }

        /// <summary>
        /// Enable pagination (override to disable)
        /// </summary>
        protected virtual bool EnablePagination => true;

        /// <summary>
        /// Page size for pagination
        /// </summary>
        protected virtual int PageSize => 100;

        /// <summary>
        /// Called when page loads
        /// </summary>
        protected virtual async Task OnPageLoadAsync()
        {
            await RefreshAsync();
        }

        /// <summary>
        /// Called when row double clicked
        /// </summary>
        protected virtual async Task OnRowDoubleClickAsync(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                await ViewAsync();
            }
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Refresh data
        /// </summary>
        protected virtual async Task RefreshAsync()
        {
            await loadingHelper.ExecuteWithLoadingAsync(
                async () =>
                {
                    using (PerformanceMonitor.Instance.TrackQuery($"{typeof(T).Name}_LoadData", "Load all data"))
                    {
                        // Load from database
                        currentData = await LoadDataAsync();

                        // Bind to grid
                        BindDataToGrid(currentData);

                        // Invalidate cache
                        cacheManager.Remove(GetCacheKey());
                    }
                },
                "Đang tải dữ liệu..."
            );
        }

        /// <summary>
        /// Search data
        /// </summary>
        protected virtual async Task SearchAsync()
        {
            string searchText = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                BindDataToGrid(currentData);
                return;
            }

            await Task.Run(() =>
            {
                var filtered = currentData.Where(item => FilterEntity(item, searchText)).ToList();
                
                this.Invoke(new Action(() =>
                {
                    BindDataToGrid(filtered);
                }));
            });
        }

        /// <summary>
        /// Add new item
        /// </summary>
        protected virtual async Task AddAsync()
        {
            // Override in derived class to show add form
            MessageBoxHelper.ShowInfo("Override AddAsync() in derived class");
        }

        /// <summary>
        /// View selected item
        /// </summary>
        protected virtual async Task ViewAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Vui lòng chọn một dòng");
                return;
            }

            // Override in derived class to show view form
            MessageBoxHelper.ShowInfo($"Override ViewAsync() to show details for {typeof(T).Name}");
        }

        /// <summary>
        /// Edit selected item
        /// </summary>
        protected virtual async Task EditAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Vui lòng chọn một dòng");
                return;
            }

            // Override in derived class to show edit form
            MessageBoxHelper.ShowInfo($"Override EditAsync() to edit {typeof(T).Name}");
        }

        /// <summary>
        /// Delete selected item
        /// </summary>
        protected virtual async Task DeleteAsync()
        {
            if (selectedItem == null)
            {
                MessageBoxHelper.ShowWarning("Vui lòng chọn một dòng để xóa");
                return;
            }

            if (!MessageBoxHelper.ShowConfirm($"Bạn có chắc muốn xóa {typeof(T).Name} này?"))
            {
                return;
            }

            // Override in derived class to implement delete logic
            MessageBoxHelper.ShowInfo($"Override DeleteAsync() to delete {typeof(T).Name}");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Bind data to DataGridView
        /// </summary>
        protected void BindDataToGrid(List<T> data)
        {
            dataGridView.Rows.Clear();

            foreach (var item in data)
            {
                var row = EntityToRow(item);
                dataGridView.Rows.Add(row);
            }
        }

        /// <summary>
        /// Export to CSV
        /// </summary>
        protected virtual void ExportToCsv()
        {
            try
            {
                string fileName = $"{typeof(T).Name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                dataGridView.ExportToCsv(fileName);
                MessageBoxHelper.ShowSuccess($"Đã export thành công: {fileName}");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Lỗi khi export CSV");
            }
        }

        /// <summary>
        /// Selection changed event
        /// </summary>
        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                selectedItem = RowToEntity(dataGridView.SelectedRows[0]);
            }
            else
            {
                selectedItem = null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get selected item
        /// </summary>
        public T GetSelectedItem()
        {
            return selectedItem;
        }

        /// <summary>
        /// Reload data (public method)
        /// </summary>
        public async Task ReloadAsync()
        {
            await RefreshAsync();
        }

        #endregion
    }
}
