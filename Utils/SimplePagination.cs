using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Simple pagination helper cho DataGridView với data đồng bộ
    /// </summary>
    public class SimplePagination
    {
        private DataGridView dataGridView;
        private DataTable fullDataTable;
        private readonly int pageSize;
        private int currentPage;
        private int totalPages;
        
        private Panel paginationPanel;
        private Button btnFirst, btnPrevious, btnNext, btnLast;
        private Label lblPageInfo;
        
        public event EventHandler PageChanged;
        
        public int CurrentPage => currentPage;
        public int TotalPages => totalPages;
        public int TotalRecords => fullDataTable?.Rows.Count ?? 0;
        
        public SimplePagination(DataGridView dgv, int pageSize = 12)
        {
            this.dataGridView = dgv;
            this.pageSize = pageSize;
            this.currentPage = 1;
            
            CreatePaginationControls();
        }
        
        private void CreatePaginationControls()
        {
            paginationPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            
            int buttonWidth = 80;
            int buttonHeight = 35;
            var buttonColor = Color.FromArgb(0, 122, 204);
            
            btnFirst = CreateButton("⏮ Đầu", buttonWidth, buttonHeight, buttonColor);
            btnFirst.Click += (s, e) => GoToPage(1);
            
            btnPrevious = CreateButton("◀ Trước", buttonWidth, buttonHeight, buttonColor);
            btnPrevious.Click += (s, e) => GoToPage(currentPage - 1);
            
            lblPageInfo = new Label
            {
                Text = "Trang 0 / 0",
                Width = 200,
                Height = buttonHeight,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White
            };
            
            btnNext = CreateButton("Sau ▶", buttonWidth, buttonHeight, buttonColor);
            btnNext.Click += (s, e) => GoToPage(currentPage + 1);
            
            btnLast = CreateButton("Cuối ⏭", buttonWidth, buttonHeight, buttonColor);
            btnLast.Click += (s, e) => GoToPage(totalPages);
            
            paginationPanel.Controls.AddRange(new Control[] { btnFirst, btnPrevious, lblPageInfo, btnNext, btnLast });
            paginationPanel.Resize += (s, e) => PositionControls();
            
            PositionControls();
        }
        
        private Button CreateButton(string text, int width, int height, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
        
        private void PositionControls()
        {
            int centerX = paginationPanel.Width / 2;
            int y = (paginationPanel.Height - btnFirst.Height) / 2;
            
            btnFirst.Location = new Point(centerX - 250, y);
            btnPrevious.Location = new Point(centerX - 160, y);
            lblPageInfo.Location = new Point(centerX - 100, y);
            btnNext.Location = new Point(centerX + 100, y);
            btnLast.Location = new Point(centerX + 190, y);
        }
        
        public Panel GetPaginationPanel()
        {
            return paginationPanel;
        }
        
        public void SetDataSource(DataTable dataTable)
        {
            fullDataTable = dataTable;
            
            if (fullDataTable == null || fullDataTable.Rows.Count == 0)
            {
                dataGridView.DataSource = null;
                totalPages = 0;
                currentPage = 0;
                UpdatePaginationUI();
                return;
            }
            
            totalPages = (int)Math.Ceiling((double)fullDataTable.Rows.Count / pageSize);
            currentPage = 1;
            
            ShowPage();
            UpdatePaginationUI();
        }
        
        private void ShowPage()
        {
            if (fullDataTable == null || fullDataTable.Rows.Count == 0) return;
            
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, fullDataTable.Rows.Count);
            
            DataTable pageTable = fullDataTable.Clone();
            
            for (int i = startIndex; i < endIndex; i++)
            {
                pageTable.ImportRow(fullDataTable.Rows[i]);
            }
            
            dataGridView.DataSource = pageTable;
            
            PageChanged?.Invoke(this, EventArgs.Empty);
        }
        
        private void GoToPage(int page)
        {
            if (page < 1 || page > totalPages) return;
            
            currentPage = page;
            ShowPage();
            UpdatePaginationUI();
        }
        
        private void UpdatePaginationUI()
        {
            btnFirst.Enabled = currentPage > 1;
            btnPrevious.Enabled = currentPage > 1;
            btnNext.Enabled = currentPage < totalPages;
            btnLast.Enabled = currentPage < totalPages;
            
            if (totalPages == 0)
            {
                lblPageInfo.Text = "Không có dữ liệu";
            }
            else
            {
                int startRecord = (currentPage - 1) * pageSize + 1;
                int endRecord = Math.Min(currentPage * pageSize, fullDataTable.Rows.Count);
                lblPageInfo.Text = $"Trang {currentPage}/{totalPages} ({startRecord}-{endRecord}/{fullDataTable.Rows.Count})";
            }
        }
        
        public void Refresh()
        {
            ShowPage();
            UpdatePaginationUI();
        }
    }
}
