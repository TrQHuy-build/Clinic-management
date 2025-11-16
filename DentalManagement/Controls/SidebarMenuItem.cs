using System;
using System.Drawing;
using System.Windows.Forms;

namespace DentalClinicManagement.Controls
{
    /// <summary>
    /// Control custom cho menu item trong sidebar
    /// </summary>
    public class SidebarMenuItem : Panel
    {
        private Label lblIcon;
        private Label lblText;
        private Panel indicatorPanel;
        private bool isSelected = false;

        public string MenuText
        {
            get => lblText.Text;
            set => lblText.Text = value;
        }

        public string IconText
        {
            get => lblIcon.Text;
            set => lblIcon.Text = value;
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                UpdateAppearance();
            }
        }

        public SidebarMenuItem()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Height = 50;
            this.Dock = DockStyle.Top;
            this.Cursor = Cursors.Hand;
            this.Padding = new Padding(10, 0, 0, 0);

            // Indicator (thanh dọc bên trái)
            indicatorPanel = new Panel
            {
                Width = 4,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Visible = false
            };

            // Icon label (sử dụng font icon hoặc emoji)
            lblIcon = new Label
            {
                Font = new Font("Segoe UI", 16),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(40, 50),
                Location = new Point(15, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "●"
            };

            // Text label
            lblText = new Label
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(150, 50),
                Location = new Point(60, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Menu Item"
            };

            this.Controls.Add(lblText);
            this.Controls.Add(lblIcon);
            this.Controls.Add(indicatorPanel);

            // Events
            this.MouseEnter += SidebarMenuItem_MouseEnter;
            this.MouseLeave += SidebarMenuItem_MouseLeave;
            this.Click += SidebarMenuItem_Click;
            lblIcon.Click += (s, e) => this.OnClick(e);
            lblText.Click += (s, e) => this.OnClick(e);
        }

        private void SidebarMenuItem_Click(object sender, EventArgs e)
        {
            // Event sẽ được handle ở frmMain
        }

        private void SidebarMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = ColorTranslator.FromHtml("#005A9E");
            }
        }

        private void SidebarMenuItem_MouseLeave(object sender, EventArgs e)
        {
            if (!isSelected)
            {
                this.BackColor = Color.Transparent;
            }
        }

        private void UpdateAppearance()
        {
            if (isSelected)
            {
                this.BackColor = ColorTranslator.FromHtml("#005A9E");
                indicatorPanel.Visible = true;
                lblIcon.ForeColor = Color.White;
                lblText.ForeColor = Color.White;
                lblText.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            }
            else
            {
                this.BackColor = Color.Transparent;
                indicatorPanel.Visible = false;
                lblIcon.ForeColor = Color.White;
                lblText.ForeColor = Color.White;
                lblText.Font = new Font("Segoe UI", 11);
            }
        }
    }
}