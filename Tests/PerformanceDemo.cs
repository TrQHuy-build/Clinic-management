using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests
{
    /// <summary>
    /// Demo form showing performance optimizations
    /// Demonstrates: N+1 fix, caching, pagination
    /// </summary>
    public partial class PerformanceDemo : Form
    {
        private QueryOptimizer queryOptimizer;
        private CacheManager cacheManager;
        private PaginationHelper paginationHelper;
        private DatabaseHelper dbHelper;

        private TabControl tabControl;
        private TabPage tabN1Problem;
        private TabPage tabCaching;
        private TabPage tabPagination;
        private TabPage tabStats;

        public PerformanceDemo()
        {
            InitializeForm();
            InitializeHelpers();
            SetupTabs();
        }

        #region Form Initialization

        private void InitializeForm()
        {
            this.Text = "Performance Optimization Demo";
            this.Size = new System.Drawing.Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 9);

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(tabControl);
        }

        private void InitializeHelpers()
        {
            queryOptimizer = new QueryOptimizer();
            cacheManager = CacheManager.Instance;
            dbHelper = new DatabaseHelper();
        }

        private void SetupTabs()
        {
            // Tab 1: N+1 Problem
            tabN1Problem = new TabPage("1. N+1 Problem Solution");
            SetupN1ProblemTab(tabN1Problem);
            tabControl.TabPages.Add(tabN1Problem);

            // Tab 2: Caching
            tabCaching = new TabPage("2. Caching");
            SetupCachingTab(tabCaching);
            tabControl.TabPages.Add(tabCaching);

            // Tab 3: Pagination
            tabPagination = new TabPage("3. Pagination");
            SetupPaginationTab(tabPagination);
            tabControl.TabPages.Add(tabPagination);

            // Tab 4: Statistics
            tabStats = new TabPage("4. Performance Stats");
            SetupStatsTab(tabStats);
            tabControl.TabPages.Add(tabStats);
        }

        #endregion

        #region Tab 1: N+1 Problem

        private DataGridView dgvN1Before;
        private DataGridView dgvN1After;
        private Button btnLoadN1Before;
        private Button btnLoadN1After;
        private Label lblN1BeforeTime;
        private Label lblN1AfterTime;

        private void SetupN1ProblemTab(TabPage tab)
        {
            // Before (N+1)
            Label lblBefore = new Label
            {
                Text = "❌ BEFORE: N+1 Problem (1 + N queries)",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(500, 25),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Red
            };
            tab.Controls.Add(lblBefore);

            dgvN1Before = new DataGridView
            {
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(550, 300),
                ReadOnly = true
            };
            dgvN1Before.ApplyModernStyle();
            tab.Controls.Add(dgvN1Before);

            btnLoadN1Before = new Button
            {
                Text = "Load with N+1 (Slow)",
                Location = new System.Drawing.Point(10, 350),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadN1Before.Click += async (s, e) => await LoadN1BeforeAsync();
            tab.Controls.Add(btnLoadN1Before);

            lblN1BeforeTime = new Label
            {
                Location = new System.Drawing.Point(220, 355),
                Size = new System.Drawing.Size(340, 25),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblN1BeforeTime);

            // After (JOIN)
            Label lblAfter = new Label
            {
                Text = "✅ AFTER: Single JOIN Query",
                Location = new System.Drawing.Point(620, 10),
                Size = new System.Drawing.Size(500, 25),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.Green
            };
            tab.Controls.Add(lblAfter);

            dgvN1After = new DataGridView
            {
                Location = new System.Drawing.Point(620, 40),
                Size = new System.Drawing.Size(550, 300),
                ReadOnly = true
            };
            dgvN1After.ApplyModernStyle();
            tab.Controls.Add(dgvN1After);

            btnLoadN1After = new Button
            {
                Text = "Load with JOIN (Fast)",
                Location = new System.Drawing.Point(620, 350),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadN1After.Click += async (s, e) => await LoadN1AfterAsync();
            tab.Controls.Add(btnLoadN1After);

            lblN1AfterTime = new Label
            {
                Location = new System.Drawing.Point(830, 355),
                Size = new System.Drawing.Size(340, 25),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblN1AfterTime);

            // Explanation
            TextBox txtExplanation = new TextBox
            {
                Location = new System.Drawing.Point(10, 400),
                Size = new System.Drawing.Size(1160, 300),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = @"N+1 PROBLEM EXPLANATION:

❌ BEFORE (N+1):
1. SELECT * FROM Invoices → 1 query (returns N invoices)
2. For each invoice:
   - SELECT * FROM Patients WHERE id = @patient_id → N queries
Total: 1 + N queries (if 100 invoices → 101 queries!)

✅ AFTER (JOIN):
SELECT i.*, p.full_name, p.phone FROM Invoices i 
INNER JOIN Patients p ON i.patient_id = p.id
Total: 1 query only!

PERFORMANCE IMPROVEMENT:
- 100 invoices: 101 queries → 1 query (100x faster)
- 1000 invoices: 1001 queries → 1 query (1000x faster)

Click the buttons above to see the difference in execution time!"
            };
            tab.Controls.Add(txtExplanation);
        }

        private async Task LoadN1BeforeAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                btnLoadN1Before.Enabled = false;
                lblN1BeforeTime.Text = "Loading...";

                // Simulate N+1 problem
                var invoices = await dbHelper.ExecuteQueryAsync<dynamic>("SELECT TOP 50 * FROM Invoices");
                var dt = new DataTable();
                dt.Columns.Add("InvoiceId");
                dt.Columns.Add("PatientName");
                dt.Columns.Add("TotalAmount");
                dt.Columns.Add("Status");

                foreach (var invoice in invoices)
                {
                    // N additional queries!
                    var patient = await dbHelper.ExecuteScalarAsync<string>(
                        "SELECT full_name FROM Patients WHERE id = @id", 
                        new { id = invoice.patient_id });

                    dt.Rows.Add(invoice.id, patient, invoice.total_amount, invoice.status);
                }

                dgvN1Before.DataSource = dt;
                stopwatch.Stop();
                
                lblN1BeforeTime.Text = $"⏱ Loaded in {stopwatch.ElapsedMilliseconds}ms ({invoices.Count} invoices = {invoices.Count + 1} queries)";
                lblN1BeforeTime.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Error: {ex.Message}");
            }
            finally
            {
                btnLoadN1Before.Enabled = true;
            }
        }

        private async Task LoadN1AfterAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                btnLoadN1After.Enabled = false;
                lblN1AfterTime.Text = "Loading...";

                // Single JOIN query
                var invoices = await queryOptimizer.LoadInvoicesWithDetailsAsync("1=1", null);
                
                var dt = new DataTable();
                dt.Columns.Add("InvoiceId");
                dt.Columns.Add("PatientName");
                dt.Columns.Add("TotalAmount");
                dt.Columns.Add("Status");

                foreach (var invoice in invoices)
                {
                    dt.Rows.Add(invoice.InvoiceId, invoice.PatientName, invoice.TotalAmount, invoice.Status);
                }

                dgvN1After.DataSource = dt;
                stopwatch.Stop();
                
                lblN1AfterTime.Text = $"⏱ Loaded in {stopwatch.ElapsedMilliseconds}ms ({invoices.Count} invoices = 1 query only!)";
                lblN1AfterTime.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Error: {ex.Message}");
            }
            finally
            {
                btnLoadN1After.Enabled = true;
            }
        }

        #endregion

        #region Tab 2: Caching

        private Button btnLoadNoCache;
        private Button btnLoadWithCache;
        private Button btnClearCache;
        private Button btnCacheStats;
        private Label lblNoCacheTime;
        private Label lblWithCacheTime;
        private TextBox txtCacheInfo;
        private DataGridView dgvCached;

        private void SetupCachingTab(TabPage tab)
        {
            Label lblTitle = new Label
            {
                Text = "CACHING DEMO - Services List",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(500, 25),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblTitle);

            dgvCached = new DataGridView
            {
                Location = new System.Drawing.Point(10, 45),
                Size = new System.Drawing.Size(800, 350),
                ReadOnly = true
            };
            dgvCached.ApplyModernStyle();
            tab.Controls.Add(dgvCached);

            btnLoadNoCache = new Button
            {
                Text = "Load WITHOUT Cache",
                Location = new System.Drawing.Point(10, 410),
                Size = new System.Drawing.Size(180, 35),
                BackColor = System.Drawing.Color.FromArgb(220, 53, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadNoCache.Click += async (s, e) => await LoadWithoutCacheAsync();
            tab.Controls.Add(btnLoadNoCache);

            lblNoCacheTime = new Label
            {
                Location = new System.Drawing.Point(200, 415),
                Size = new System.Drawing.Size(300, 25),
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblNoCacheTime);

            btnLoadWithCache = new Button
            {
                Text = "Load WITH Cache",
                Location = new System.Drawing.Point(10, 455),
                Size = new System.Drawing.Size(180, 35),
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoadWithCache.Click += async (s, e) => await LoadWithCacheAsync();
            tab.Controls.Add(btnLoadWithCache);

            lblWithCacheTime = new Label
            {
                Location = new System.Drawing.Point(200, 460),
                Size = new System.Drawing.Size(300, 25),
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblWithCacheTime);

            btnClearCache = new Button
            {
                Text = "Clear Cache",
                Location = new System.Drawing.Point(10, 500),
                Size = new System.Drawing.Size(180, 35),
                BackColor = System.Drawing.Color.FromArgb(255, 193, 7),
                ForeColor = System.Drawing.Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnClearCache.Click += (s, e) =>
            {
                cacheManager.Clear();
                MessageBoxHelper.ShowSuccess("Cache cleared!");
            };
            tab.Controls.Add(btnClearCache);

            btnCacheStats = new Button
            {
                Text = "Show Cache Stats",
                Location = new System.Drawing.Point(10, 545),
                Size = new System.Drawing.Size(180, 35),
                BackColor = System.Drawing.Color.FromArgb(23, 162, 184),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCacheStats.Click += (s, e) => ShowCacheStats();
            tab.Controls.Add(btnCacheStats);

            txtCacheInfo = new TextBox
            {
                Location = new System.Drawing.Point(10, 590),
                Size = new System.Drawing.Size(800, 120),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = @"CACHING EXPLANATION:

❌ WITHOUT Cache: Database query every time (slow)
✅ WITH Cache: Query once, reuse result from memory (fast!)

Try this:
1. Click 'Load WITHOUT Cache' → See database query time
2. Click 'Load WITH Cache' 3 times → First time: DB query, 2nd-3rd: From cache (instant!)
3. Click 'Clear Cache' → Cache emptied
4. Click 'Show Cache Stats' → See cache hit/miss statistics"
            };
            tab.Controls.Add(txtCacheInfo);
        }

        private async Task LoadWithoutCacheAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                btnLoadNoCache.Enabled = false;
                lblNoCacheTime.Text = "Loading from database...";

                // Direct database query (no cache)
                var services = await dbHelper.ExecuteQueryAsync<dynamic>("SELECT * FROM Services");
                
                var dt = new DataTable();
                dt.Columns.Add("ServiceName");
                dt.Columns.Add("Price");
                dt.Columns.Add("Duration");

                foreach (var service in services)
                {
                    dt.Rows.Add(service.service_name, service.price, service.duration);
                }

                dgvCached.DataSource = dt;
                stopwatch.Stop();
                
                lblNoCacheTime.Text = $"⏱ Loaded in {stopwatch.ElapsedMilliseconds}ms (from database)";
                lblNoCacheTime.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Error: {ex.Message}");
            }
            finally
            {
                btnLoadNoCache.Enabled = true;
            }
        }

        private async Task LoadWithCacheAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            bool fromCache = false;
            
            try
            {
                btnLoadWithCache.Enabled = false;
                lblWithCacheTime.Text = "Loading...";

                // Check cache first
                const string cacheKey = "services_demo";
                var cached = cacheManager.Get<DataTable>(cacheKey);

                DataTable dt;
                if (cached != null)
                {
                    dt = cached;
                    fromCache = true;
                }
                else
                {
                    // Load from database
                    var services = await dbHelper.ExecuteQueryAsync<dynamic>("SELECT * FROM Services");
                    
                    dt = new DataTable();
                    dt.Columns.Add("ServiceName");
                    dt.Columns.Add("Price");
                    dt.Columns.Add("Duration");

                    foreach (var service in services)
                    {
                        dt.Rows.Add(service.service_name, service.price, service.duration);
                    }

                    // Cache for 5 minutes
                    cacheManager.Set(cacheKey, dt, TimeSpan.FromMinutes(5));
                }

                dgvCached.DataSource = dt;
                stopwatch.Stop();
                
                if (fromCache)
                {
                    lblWithCacheTime.Text = $"⚡ Loaded in {stopwatch.ElapsedMilliseconds}ms (from CACHE - instant!)";
                    lblWithCacheTime.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblWithCacheTime.Text = $"⏱ Loaded in {stopwatch.ElapsedMilliseconds}ms (from database, now cached)";
                    lblWithCacheTime.ForeColor = System.Drawing.Color.Orange;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Error: {ex.Message}");
            }
            finally
            {
                btnLoadWithCache.Enabled = true;
            }
        }

        private void ShowCacheStats()
        {
            var stats = cacheManager.GetStatistics();
            string message = $"Cache Statistics:\n\n" +
                           $"Total Items: {stats.TotalItems}\n" +
                           $"Memory Limit: {stats.TotalMemorySize / 1024 / 1024} MB\n\n" +
                           $"Cached Items:\n";

            foreach (var item in stats.Items)
            {
                message += $"  - {item.Key}: {item.Age.TotalMinutes:F1} min old\n";
            }

            MessageBoxHelper.ShowInfo(message, "Cache Statistics");
        }

        #endregion

        #region Tab 3: Pagination

        private DataGridView dgvPaginated;
        private Label lblPaginationInfo;

        private void SetupPaginationTab(TabPage tab)
        {
            Label lblTitle = new Label
            {
                Text = "PAGINATION DEMO - Handle Large Datasets",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(500, 25),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblTitle);

            dgvPaginated = new DataGridView
            {
                Location = new System.Drawing.Point(10, 45),
                Size = new System.Drawing.Size(1160, 500),
                ReadOnly = true
            };
            dgvPaginated.ApplyModernStyle();
            tab.Controls.Add(dgvPaginated);

            // Pagination controls will be added by PaginationHelper

            lblPaginationInfo = new Label
            {
                Location = new System.Drawing.Point(10, 595),
                Size = new System.Drawing.Size(1160, 100),
                Text = @"PAGINATION EXPLANATION:

❌ WITHOUT Pagination: Load ALL 10,000 records → Memory overflow, slow performance
✅ WITH Pagination: Load only 100 records per page → Fast, responsive

Use the pagination controls at the bottom to navigate pages.
Try changing 'Rows per page' to see different page sizes.",
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            tab.Controls.Add(lblPaginationInfo);

            // Initialize pagination
            InitializePaginationAsync();
        }

        private async void InitializePaginationAsync()
        {
            try
            {
                paginationHelper = await PaginationHelper.SetupAsync(
                    dgvPaginated,
                    LoadPageDataAsync,
                    GetTotalRecordCountAsync,
                    pageSize: 100
                );
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Error initializing pagination: {ex.Message}");
            }
        }

        private async Task<DataTable> LoadPageDataAsync(int pageNumber, int pageSize)
        {
            // Simulate loading page data with OFFSET/FETCH
            string sql = PaginationHelper.CreatePaginationQuery(
                "SELECT * FROM Invoices",
                "id DESC",
                pageNumber,
                pageSize
            );

            var invoices = await dbHelper.ExecuteQueryAsync<dynamic>(sql);
            
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Patient ID");
            dt.Columns.Add("Total Amount");
            dt.Columns.Add("Status");
            dt.Columns.Add("Invoice Date");

            foreach (var invoice in invoices)
            {
                dt.Rows.Add(
                    invoice.id,
                    invoice.patient_id,
                    invoice.total_amount,
                    invoice.status,
                    invoice.invoice_date
                );
            }

            return dt;
        }

        private async Task<int> GetTotalRecordCountAsync()
        {
            return await dbHelper.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Invoices");
        }

        #endregion

        #region Tab 4: Statistics

        private TextBox txtStats;
        private Button btnRefreshStats;

        private void SetupStatsTab(TabPage tab)
        {
            Label lblTitle = new Label
            {
                Text = "PERFORMANCE STATISTICS",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(500, 25),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };
            tab.Controls.Add(lblTitle);

            txtStats = new TextBox
            {
                Location = new System.Drawing.Point(10, 45),
                Size = new System.Drawing.Size(1160, 600),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new System.Drawing.Font("Consolas", 9)
            };
            tab.Controls.Add(txtStats);

            btnRefreshStats = new Button
            {
                Text = "Refresh Statistics",
                Location = new System.Drawing.Point(10, 660),
                Size = new System.Drawing.Size(200, 35),
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefreshStats.Click += (s, e) => RefreshStatistics();
            tab.Controls.Add(btnRefreshStats);
        }

        private void RefreshStatistics()
        {
            var perfMonitor = PerformanceMonitor.Instance;
            var stats = perfMonitor.GetStatistics();
            var slowQueries = perfMonitor.GetSlowQueries();
            var cacheStats = perfMonitor.GetCacheEffectiveness();

            string output = "=== PERFORMANCE STATISTICS ===\n\n";
            output += $"Total Queries Tracked: {stats.Count}\n";
            output += $"Total Executions: {stats.Sum(s => s.ExecutionCount)}\n";
            output += $"Cache Hit Rate: {cacheStats.HitRate:F1}%\n";
            output += $"Slow Queries (>500ms): {slowQueries.Count}\n\n";

            if (slowQueries.Count > 0)
            {
                output += "=== SLOW QUERIES ===\n";
                foreach (var query in slowQueries)
                {
                    output += $"  {query.QueryName}\n";
                    output += $"    Avg: {query.AverageExecutionTime}ms\n";
                    output += $"    Max: {query.MaxExecutionTime}ms\n";
                    output += $"    Count: {query.ExecutionCount}\n";
                    output += $"    Cache Hit Rate: {query.CacheHitRate:F1}%\n\n";
                }
            }

            output += "=== MOST EXECUTED QUERIES ===\n";
            var topQueries = perfMonitor.GetMostExecutedQueries(10);
            foreach (var query in topQueries)
            {
                output += $"  {query.QueryName}: {query.ExecutionCount} times (Avg: {query.AverageExecutionTime}ms)\n";
            }

            txtStats.Text = output;
        }

        #endregion

        #region Main Entry Point

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PerformanceDemo());
        }

        #endregion
    }
}
