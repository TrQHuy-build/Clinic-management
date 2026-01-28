using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Helper class for loading indicators and progress bars
    /// ✅ Loading spinner overlay
    /// ✅ Progress bar for long operations
    /// ✅ Async operation support
    /// ✅ Cancellation support
    /// </summary>
    public class LoadingHelper
    {
        private Form parentForm;
        private Panel loadingPanel;
        private PictureBox loadingSpinner;
        private Label loadingLabel;
        private ProgressBar progressBar;
        private Button cancelButton;
        private CancellationTokenSource cancellationTokenSource;
        private bool isCancelable;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoadingHelper(Form form)
        {
            parentForm = form;
            CreateLoadingControls();
        }

        #region Create Loading UI

        /// <summary>
        /// Create loading controls
        /// </summary>
        private void CreateLoadingControls()
        {
            // Semi-transparent overlay panel
            loadingPanel = new Panel
            {
                BackColor = Color.FromArgb(180, 255, 255, 255), // Semi-transparent white
                Dock = DockStyle.Fill,
                Visible = false
            };

            // Loading spinner (we'll use a rotating label as simple spinner)
            loadingSpinner = new PictureBox
            {
                Size = new Size(64, 64),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };

            // Loading label
            loadingLabel = new Label
            {
                Text = "Đang tải...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(0, 122, 204)
            };

            // Progress bar
            progressBar = new ProgressBar
            {
                Width = 300,
                Height = 25,
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };

            // Cancel button
            cancelButton = new Button
            {
                Text = "Hủy",
                Width = 80,
                Height = 30,
                Visible = false
            };
            cancelButton.Click += CancelButton_Click;

            // Add controls to panel
            loadingPanel.Controls.Add(loadingSpinner);
            loadingPanel.Controls.Add(loadingLabel);
            loadingPanel.Controls.Add(progressBar);
            loadingPanel.Controls.Add(cancelButton);

            // Center controls
            parentForm.Resize += (s, e) => CenterLoadingControls();
            loadingPanel.VisibleChanged += (s, e) =>
            {
                if (loadingPanel.Visible)
                    CenterLoadingControls();
            };

            // Add panel to form (but keep it hidden initially)
            parentForm.Controls.Add(loadingPanel);
            loadingPanel.BringToFront();
        }

        /// <summary>
        /// Center loading controls in panel
        /// </summary>
        private void CenterLoadingControls()
        {
            if (loadingPanel == null || !loadingPanel.Visible)
                return;

            int centerX = loadingPanel.Width / 2;
            int centerY = loadingPanel.Height / 2;

            // Position spinner
            loadingSpinner.Location = new Point(
                centerX - loadingSpinner.Width / 2,
                centerY - 100
            );

            // Position label
            loadingLabel.Location = new Point(
                centerX - loadingLabel.Width / 2,
                centerY - 30
            );

            // Position progress bar (if visible)
            if (progressBar.Visible)
            {
                progressBar.Location = new Point(
                    centerX - progressBar.Width / 2,
                    centerY + 10
                );
            }

            // Position cancel button (if visible)
            if (cancelButton.Visible)
            {
                cancelButton.Location = new Point(
                    centerX - cancelButton.Width / 2,
                    centerY + 50
                );
            }
        }

        #endregion

        #region Show/Hide Loading

        /// <summary>
        /// Show simple loading spinner
        /// </summary>
        public void Show(string message = "Đang tải...")
        {
            if (parentForm.InvokeRequired)
            {
                parentForm.Invoke(new Action(() => Show(message)));
                return;
            }

            loadingLabel.Text = message;
            progressBar.Visible = false;
            cancelButton.Visible = false;
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            CenterLoadingControls();

            // Disable form controls
            DisableFormControls();

            // Start spinner animation
            StartSpinnerAnimation();

            parentForm.Refresh();
        }

        /// <summary>
        /// Show loading with progress bar
        /// </summary>
        public void ShowWithProgress(string message = "Đang xử lý...", bool cancelable = false)
        {
            if (parentForm.InvokeRequired)
            {
                parentForm.Invoke(new Action(() => ShowWithProgress(message, cancelable)));
                return;
            }

            loadingLabel.Text = message;
            progressBar.Value = 0;
            progressBar.Visible = true;
            cancelButton.Visible = cancelable;
            isCancelable = cancelable;

            if (cancelable)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            CenterLoadingControls();

            DisableFormControls();
            StartSpinnerAnimation();

            parentForm.Refresh();
        }

        /// <summary>
        /// Hide loading
        /// </summary>
        public void Hide()
        {
            if (parentForm.InvokeRequired)
            {
                parentForm.Invoke(new Action(Hide));
                return;
            }

            loadingPanel.Visible = false;
            progressBar.Visible = false;
            cancelButton.Visible = false;

            EnableFormControls();
            StopSpinnerAnimation();

            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        /// <summary>
        /// Update progress (0-100)
        /// </summary>
        public void UpdateProgress(int percentage, string message = null)
        {
            if (parentForm.InvokeRequired)
            {
                parentForm.Invoke(new Action(() => UpdateProgress(percentage, message)));
                return;
            }

            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;

            progressBar.Value = percentage;

            if (!string.IsNullOrEmpty(message))
            {
                loadingLabel.Text = $"{message} ({percentage}%)";
            }

            parentForm.Refresh();
        }

        #endregion

        #region Async Operations

        /// <summary>
        /// Execute async operation with loading indicator
        /// </summary>
        public async Task ExecuteWithLoadingAsync(Func<Task> operation, string loadingMessage = "Đang xử lý...")
        {
            try
            {
                Show(loadingMessage);
                await operation();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "LoadingHelper.ExecuteWithLoadingAsync", showToUser: true);
            }
            finally
            {
                Hide();
            }
        }

        /// <summary>
        /// Execute async operation with result and loading indicator
        /// </summary>
        public async Task<T> ExecuteWithLoadingAsync<T>(Func<Task<T>> operation, string loadingMessage = "Đang xử lý...")
        {
            try
            {
                Show(loadingMessage);
                return await operation();
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "LoadingHelper.ExecuteWithLoadingAsync", showToUser: true);
                return default(T);
            }
            finally
            {
                Hide();
            }
        }

        /// <summary>
        /// Execute async operation with progress reporting
        /// </summary>
        public async Task ExecuteWithProgressAsync(
            Func<IProgress<int>, CancellationToken, Task> operation,
            string loadingMessage = "Đang xử lý...",
            bool cancelable = false)
        {
            try
            {
                ShowWithProgress(loadingMessage, cancelable);

                var progress = new Progress<int>(percentage =>
                {
                    UpdateProgress(percentage);
                });

                CancellationToken cancellationToken = cancelable && cancellationTokenSource != null
                    ? cancellationTokenSource.Token
                    : CancellationToken.None;

                await operation(progress, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                MessageBoxHelper.ShowInfo("Thao tác đã bị hủy");
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "LoadingHelper.ExecuteWithProgressAsync", showToUser: true);
            }
            finally
            {
                Hide();
            }
        }

        /// <summary>
        /// Execute long-running query with loading
        /// </summary>
        public async Task<T> ExecuteQueryAsync<T>(Func<T> queryFunc, string loadingMessage = "Đang truy vấn...")
        {
            return await ExecuteWithLoadingAsync(
                () => Task.Run(queryFunc),
                loadingMessage
            );
        }

        #endregion

        #region Spinner Animation

        private Timer spinnerTimer;
        private int spinnerAngle = 0;

        /// <summary>
        /// Start spinner animation
        /// </summary>
        private void StartSpinnerAnimation()
        {
            if (spinnerTimer == null)
            {
                spinnerTimer = new Timer { Interval = 50 }; // 50ms = 20 FPS
                spinnerTimer.Tick += SpinnerTimer_Tick;
            }

            spinnerAngle = 0;
            DrawSpinner();
            spinnerTimer.Start();
        }

        /// <summary>
        /// Stop spinner animation
        /// </summary>
        private void StopSpinnerAnimation()
        {
            spinnerTimer?.Stop();
        }

        /// <summary>
        /// Spinner timer tick
        /// </summary>
        private void SpinnerTimer_Tick(object sender, EventArgs e)
        {
            spinnerAngle = (spinnerAngle + 15) % 360;
            DrawSpinner();
        }

        /// <summary>
        /// Draw rotating spinner
        /// </summary>
        private void DrawSpinner()
        {
            if (loadingSpinner.Image != null)
            {
                loadingSpinner.Image.Dispose();
            }

            Bitmap bitmap = new Bitmap(64, 64);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw rotating arcs
                using (Pen pen = new Pen(Color.FromArgb(0, 122, 204), 4))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                    // Draw 8 segments with decreasing opacity
                    for (int i = 0; i < 8; i++)
                    {
                        int opacity = 255 - (i * 30);
                        if (opacity < 50) opacity = 50;

                        pen.Color = Color.FromArgb(opacity, 0, 122, 204);

                        int angle = (spinnerAngle + (i * 45)) % 360;
                        int x = 32 + (int)(20 * Math.Cos(angle * Math.PI / 180));
                        int y = 32 + (int)(20 * Math.Sin(angle * Math.PI / 180));

                        g.FillEllipse(new SolidBrush(pen.Color), x - 3, y - 3, 6, 6);
                    }
                }
            }

            loadingSpinner.Image = bitmap;
        }

        #endregion

        #region Form Controls Management

        /// <summary>
        /// Disable all form controls except loading panel
        /// </summary>
        private void DisableFormControls()
        {
            foreach (Control control in parentForm.Controls)
            {
                if (control != loadingPanel && control is not MenuStrip)
                {
                    control.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Enable all form controls
        /// </summary>
        private void EnableFormControls()
        {
            foreach (Control control in parentForm.Controls)
            {
                if (control != loadingPanel)
                {
                    control.Enabled = true;
                }
            }
        }

        #endregion

        #region Cancellation

        /// <summary>
        /// Cancel button click handler
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (isCancelable && cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Get cancellation token
        /// </summary>
        public CancellationToken GetCancellationToken()
        {
            return cancellationTokenSource?.Token ?? CancellationToken.None;
        }

        #endregion

        #region Static Helper Methods

        /// <summary>
        /// Execute with loading (static helper)
        /// </summary>
        public static async Task ExecuteAsync(Form form, Func<Task> operation, string message = "Đang xử lý...")
        {
            var loader = new LoadingHelper(form);
            await loader.ExecuteWithLoadingAsync(operation, message);
        }

        /// <summary>
        /// Execute query with loading (static helper)
        /// </summary>
        public static async Task<T> QueryAsync<T>(Form form, Func<T> queryFunc, string message = "Đang truy vấn...")
        {
            var loader = new LoadingHelper(form);
            return await loader.ExecuteQueryAsync(queryFunc, message);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            spinnerTimer?.Stop();
            spinnerTimer?.Dispose();
            cancellationTokenSource?.Dispose();

            loadingSpinner?.Image?.Dispose();
            loadingPanel?.Dispose();
        }

        #endregion
    }
}
