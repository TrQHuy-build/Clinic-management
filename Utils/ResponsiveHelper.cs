using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LiveCharts.WinForms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Helper class for responsive form design
    /// ✅ Auto-resize controls
    /// ✅ Maintain aspect ratios
    /// ✅ Responsive charts
    /// ✅ Anchor-based layout
    /// </summary>
    public class ResponsiveHelper
    {
        private Form parentForm;
        private Dictionary<Control, Rectangle> originalBounds;
        private Size originalFormSize;
        private bool isInitialized = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResponsiveHelper(Form form)
        {
            parentForm = form;
            originalBounds = new Dictionary<Control, Rectangle>();
            originalFormSize = form.Size;

            // Capture original bounds after form load
            form.Load += (s, e) => Initialize();
            form.Resize += (s, e) => OnFormResize();
        }

        #region Initialization

        /// <summary>
        /// Initialize - capture original control bounds
        /// </summary>
        private void Initialize()
        {
            if (isInitialized)
                return;

            originalFormSize = parentForm.Size;
            CaptureOriginalBounds(parentForm);
            isInitialized = true;
        }

        /// <summary>
        /// Capture original bounds of all controls
        /// </summary>
        private void CaptureOriginalBounds(Control container)
        {
            foreach (Control control in container.Controls)
            {
                // Skip if already captured or is menu/toolstrip
                if (originalBounds.ContainsKey(control) || control is MenuStrip || control is ToolStrip)
                    continue;

                originalBounds[control] = new Rectangle(control.Location, control.Size);

                // Recursively capture child controls
                if (control.HasChildren && !(control is DataGridView))
                {
                    CaptureOriginalBounds(control);
                }
            }
        }

        #endregion

        #region Form Resize Handling

        /// <summary>
        /// Handle form resize event
        /// </summary>
        private void OnFormResize()
        {
            if (!isInitialized || parentForm.WindowState == FormWindowState.Minimized)
                return;

            // Calculate scaling factors
            float scaleX = (float)parentForm.Width / originalFormSize.Width;
            float scaleY = (float)parentForm.Height / originalFormSize.Height;

            // Resize controls
            ResizeControls(parentForm, scaleX, scaleY);

            // Refresh form
            parentForm.Refresh();
        }

        /// <summary>
        /// Resize controls proportionally
        /// </summary>
        private void ResizeControls(Control container, float scaleX, float scaleY)
        {
            foreach (Control control in container.Controls)
            {
                if (!originalBounds.ContainsKey(control))
                    continue;

                Rectangle original = originalBounds[control];

                // Calculate new bounds
                int newX = (int)(original.X * scaleX);
                int newY = (int)(original.Y * scaleY);
                int newWidth = (int)(original.Width * scaleX);
                int newHeight = (int)(original.Height * scaleY);

                // Apply new bounds
                control.SetBounds(newX, newY, newWidth, newHeight);

                // Special handling for specific control types
                HandleSpecialControls(control, scaleX, scaleY);

                // Recursively resize child controls
                if (control.HasChildren && !(control is DataGridView))
                {
                    ResizeControls(control, scaleX, scaleY);
                }
            }
        }

        /// <summary>
        /// Handle special control types
        /// </summary>
        private void HandleSpecialControls(Control control, float scaleX, float scaleY)
        {
            // DataGridView - adjust row height
            if (control is DataGridView dgv)
            {
                int originalRowHeight = 35; // Default row height
                dgv.RowTemplate.Height = (int)(originalRowHeight * Math.Min(scaleX, scaleY));
            }

            // Charts - force redraw
            if (control is CartesianChart || control is PieChart)
            {
                control.Invalidate();
            }

            // Adjust font size for labels
            if (control is Label label && originalBounds.ContainsKey(label))
            {
                float originalFontSize = 9f; // Assuming default font size
                float scaleFactor = Math.Min(scaleX, scaleY);
                float newFontSize = originalFontSize * scaleFactor;

                if (newFontSize >= 6 && newFontSize <= 20) // Reasonable font size limits
                {
                    label.Font = new Font(label.Font.FontFamily, newFontSize, label.Font.Style);
                }
            }
        }

        #endregion

        #region Anchor-based Layout

        /// <summary>
        /// Setup responsive layout using anchors
        /// </summary>
        public void SetupResponsiveLayout()
        {
            SetupControlAnchors(parentForm);
        }

        /// <summary>
        /// Setup anchors for controls
        /// </summary>
        private void SetupControlAnchors(Control container)
        {
            foreach (Control control in container.Controls)
            {
                // Skip menus and toolstrips
                if (control is MenuStrip || control is ToolStrip)
                    continue;

                // Determine anchor based on position
                AnchorStyles anchor = DetermineAnchor(control, container);
                control.Anchor = anchor;

                // Recursively setup child controls
                if (control.HasChildren && !(control is DataGridView))
                {
                    SetupControlAnchors(control);
                }
            }
        }

        /// <summary>
        /// Determine appropriate anchor for control
        /// </summary>
        private AnchorStyles DetermineAnchor(Control control, Control container)
        {
            // Default anchor
            AnchorStyles anchor = AnchorStyles.Top | AnchorStyles.Left;

            // Calculate relative position
            float relativeX = (float)control.Left / container.Width;
            float relativeY = (float)control.Top / container.Height;
            float relativeRight = (float)control.Right / container.Width;
            float relativeBottom = (float)control.Bottom / container.Height;

            // DataGridView - typically full width
            if (control is DataGridView)
            {
                return AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            }

            // Charts - typically full width
            if (control is CartesianChart || control is PieChart)
            {
                return AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            }

            // Panel - typically full width
            if (control is Panel)
            {
                // Top panel
                if (relativeY < 0.2f)
                {
                    return AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                }
                // Bottom panel
                else if (relativeBottom > 0.8f)
                {
                    return AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                }
                // Side panel
                else if (relativeX < 0.2f)
                {
                    return AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
                }
                else if (relativeRight > 0.8f)
                {
                    return AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
                }
                // Center panel
                else
                {
                    return AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                }
            }

            // Buttons in bottom-right corner
            if (control is Button)
            {
                if (relativeX > 0.7f && relativeY > 0.8f)
                {
                    return AnchorStyles.Bottom | AnchorStyles.Right;
                }
                else if (relativeX < 0.3f && relativeY > 0.8f)
                {
                    return AnchorStyles.Bottom | AnchorStyles.Left;
                }
            }

            // TextBox/ComboBox - typically stretch horizontally
            if (control is TextBox || control is ComboBox || control is DateTimePicker)
            {
                if (relativeRight > 0.8f)
                {
                    return AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                }
            }

            return anchor;
        }

        #endregion

        #region Chart Responsive

        /// <summary>
        /// Make chart responsive
        /// </summary>
        public void MakeChartResponsive(CartesianChart chart)
        {
            chart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Adjust axis label size on resize
            parentForm.Resize += (s, e) =>
            {
                if (chart.Width > 400)
                {
                    chart.AxisX[0].LabelFormatter = value => value.ToString();
                }
                else
                {
                    // Shorter labels for small width
                    chart.AxisX[0].LabelFormatter = value =>
                    {
                        string label = value.ToString();
                        return label.Length > 5 ? label.Substring(0, 5) + "..." : label;
                    };
                }
            };
        }

        /// <summary>
        /// Make pie chart responsive
        /// </summary>
        public void MakePieChartResponsive(PieChart chart)
        {
            chart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Adjust size on resize
            parentForm.Resize += (s, e) =>
            {
                chart.Invalidate();
            };
        }

        #endregion

        #region Min/Max Size

        /// <summary>
        /// Set form minimum size
        /// </summary>
        public void SetMinimumSize(int width, int height)
        {
            parentForm.MinimumSize = new Size(width, height);
        }

        /// <summary>
        /// Set form maximum size
        /// </summary>
        public void SetMaximumSize(int width, int height)
        {
            parentForm.MaximumSize = new Size(width, height);
        }

        /// <summary>
        /// Set form to maintain aspect ratio
        /// </summary>
        public void MaintainAspectRatio(double aspectRatio)
        {
            parentForm.Resize += (s, e) =>
            {
                if (parentForm.WindowState == FormWindowState.Minimized)
                    return;

                int width = parentForm.Width;
                int height = (int)(width / aspectRatio);

                if (height != parentForm.Height)
                {
                    parentForm.Height = height;
                }
            };
        }

        #endregion

        #region Responsive Breakpoints

        /// <summary>
        /// Define responsive breakpoints
        /// </summary>
        public void AddBreakpoint(int width, Action<Form> layoutAction)
        {
            parentForm.Resize += (s, e) =>
            {
                if (parentForm.Width <= width)
                {
                    layoutAction(parentForm);
                }
            };
        }

        /// <summary>
        /// Setup common responsive breakpoints
        /// </summary>
        public void SetupBreakpoints()
        {
            // Small screen (< 800px)
            AddBreakpoint(800, form =>
            {
                // Hide some labels or panels for small screens
                foreach (Control control in form.Controls)
                {
                    if (control is Label label && label.Text.Length > 50)
                    {
                        label.AutoEllipsis = true;
                    }
                }
            });

            // Medium screen (< 1024px)
            AddBreakpoint(1024, form =>
            {
                // Adjust layout for medium screens
            });

            // Large screen (>= 1024px)
            parentForm.Resize += (s, e) =>
            {
                if (parentForm.Width >= 1024)
                {
                    // Restore full layout for large screens
                }
            };
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Quick setup responsive form (static helper)
        /// </summary>
        public static ResponsiveHelper SetupResponsive(Form form, int minWidth = 800, int minHeight = 600)
        {
            ResponsiveHelper helper = new ResponsiveHelper(form);
            helper.SetMinimumSize(minWidth, minHeight);
            helper.SetupResponsiveLayout();
            return helper;
        }

        /// <summary>
        /// Make control fill parent (static helper)
        /// </summary>
        public static void FillParent(Control control)
        {
            control.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Make control dock to side (static helper)
        /// </summary>
        public static void DockToSide(Control control, DockStyle dock)
        {
            control.Dock = dock;
        }

        #endregion
    }
}
