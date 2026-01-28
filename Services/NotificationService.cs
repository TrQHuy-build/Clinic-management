using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Services
{
/// <summary>
    /// Service thông báo realtime cho bác s?
    /// - Ki?m tra l?ch h?n m?i ??nh k?
    /// - Hi?n th? notification popup
    /// - Qu?n lý notification badge
    /// </summary>
    public class NotificationService : IDisposable
    {
        private static NotificationService _instance;
        private static readonly object _lock = new object();
        
      private System.Timers.Timer _checkTimer;
        private int _lastAppointmentCount = 0;
        private DateTime _lastCheckTime = DateTime.MinValue;
        private bool _isDisposed = false;
  
        // Event ?? thông báo cho UI
        public event EventHandler<NewAppointmentEventArgs> OnNewAppointment;
        public event EventHandler<int> OnNotificationCountChanged;
        
  // Singleton instance
public static NotificationService Instance
{
            get
    {
          if (_instance == null)
      {
           lock (_lock)
         {
     if (_instance == null)
      {
     _instance = new NotificationService();
  }
     }
      }
       return _instance;
          }
        }
     
 private NotificationService()
      {
      // Private constructor for singleton
        }
        
        /// <summary>
        /// B?t ??u ki?m tra thông báo ??nh k?
  /// </summary>
        /// <param name="intervalSeconds">Kho?ng th?i gian ki?m tra (giây), m?c ??nh 30s</param>
        public void StartMonitoring(int intervalSeconds = 30)
   {
            if (!Auth.IsDoctor() || !Auth.CurrentStaffId.HasValue)
           return;
    
            StopMonitoring();
      
        // L?y s? l??ng ban ??u
            _lastAppointmentCount = GetPendingAppointmentCount();
            _lastCheckTime = DateTime.Now;
       
      _checkTimer = new System.Timers.Timer(intervalSeconds * 1000);
  _checkTimer.Elapsed += CheckTimer_Elapsed;
_checkTimer.AutoReset = true;
      _checkTimer.Start();
     
            Logger.LogAction("NOTIFICATION_SERVICE", "Started monitoring for new appointments");
        }
     
        /// <summary>
        /// D?ng ki?m tra thông báo
 /// </summary>
        public void StopMonitoring()
        {
  if (_checkTimer != null)
            {
                _checkTimer.Stop();
             _checkTimer.Dispose();
    _checkTimer = null;
       }
     }
        
        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
     try
    {
if (!Auth.IsDoctor() || !Auth.CurrentStaffId.HasValue)
   return;
                  
        int currentCount = GetPendingAppointmentCount();
  
       if (currentCount > _lastAppointmentCount)
  {
 // Có l?ch h?n m?i
            int newCount = currentCount - _lastAppointmentCount;
         var newAppointments = GetNewAppointments(_lastCheckTime);
        
          // Raise event
 OnNewAppointment?.Invoke(this, new NewAppointmentEventArgs
    {
             NewCount = newCount,
   Appointments = newAppointments
  });
         
   Logger.LogAction("NEW_APPOINTMENT_DETECTED", $"{newCount} new appointment(s) detected");
    }
        
          // C?p nh?t notification count
         OnNotificationCountChanged?.Invoke(this, currentCount);
          
                _lastAppointmentCount = currentCount;
       _lastCheckTime = DateTime.Now;
            }
      catch (Exception ex)
      {
  Logger.LogAction("NOTIFICATION_ERROR", ex.Message);
    }
}
    
        /// <summary>
        /// L?y s? l??ng l?ch h?n ?ang ch? c?a bác s?
   /// </summary>
  public int GetPendingAppointmentCount()
        {
            if (!Auth.CurrentStaffId.HasValue) return 0;
          
            try
            {
     string query = @"
  SELECT COUNT(*) 
FROM Appointment 
               WHERE assigned_doctor_id = @doctorId 
    AND status = 'Booked'
         AND appointment_date >= GETDATE()";
          
             object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter[] {
      new SqlParameter("@doctorId", Auth.CurrentStaffId.Value)
         });
    
          return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
{
         return 0;
            }
        }
   
        /// <summary>
        /// L?y danh sách l?ch h?n m?i sau th?i ?i?m ch? ??nh
        /// </summary>
        private DataTable GetNewAppointments(DateTime afterTime)
        {
            if (!Auth.CurrentStaffId.HasValue) return new DataTable();
      
            try
            {
  string query = @"
           SELECT TOP 5
     a.appointment_id,
     CASE 
    WHEN p.patient_id IS NOT NULL THEN u.fullname
          ELSE a.patient_name
  END AS patient_name,
         a.phone,
      s.service_name,
 a.appointment_date
      FROM Appointment a
  LEFT JOIN Service s ON a.service_id = s.service_id
            LEFT JOIN Patient p ON a.patient_id = p.patient_id
      LEFT JOIN UserAccount u ON p.user_id = u.user_id
           WHERE a.assigned_doctor_id = @doctorId 
         AND a.status = 'Booked'
        AND a.created_at > @afterTime
                ORDER BY a.created_at DESC";
   
   return DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
  new SqlParameter("@doctorId", Auth.CurrentStaffId.Value),
        new SqlParameter("@afterTime", afterTime)
                });
          }
     catch
   {
 return new DataTable();
            }
        }
 
        /// <summary>
        /// Hi?n th? notification popup (g?i t? UI thread)
        /// </summary>
     public static void ShowNotificationPopup(Form parentForm, string title, string message, int durationMs = 5000)
        {
 if (parentForm == null || parentForm.IsDisposed) return;
        
            if (parentForm.InvokeRequired)
            {
      parentForm.Invoke(new Action(() => ShowNotificationPopup(parentForm, title, message, durationMs)));
     return;
    }
    
            // T?o notification form
         Form notifyForm = new Form
    {
         FormBorderStyle = FormBorderStyle.None,
     StartPosition = FormStartPosition.Manual,
    ShowInTaskbar = false,
          TopMost = true,
          Size = new Size(350, 120),
           BackColor = Color.White
  };
            
   // V? trí góc ph?i d??i màn hình
            var screen = Screen.PrimaryScreen.WorkingArea;
  notifyForm.Location = new Point(screen.Right - notifyForm.Width - 20, screen.Bottom - notifyForm.Height - 20);
   
    // Panel ch?a n?i dung v?i vi?n
            Panel panel = new Panel
          {
   Dock = DockStyle.Fill,
            BackColor = Color.White,
              Padding = new Padding(2)
      };
          
    // Vi?n trái màu accent
    Panel accentBar = new Panel
            {
  Dock = DockStyle.Left,
        Width = 5,
 BackColor = ColorTranslator.FromHtml("#667eea")
    };
            
            // Icon
  Label lblIcon = new Label
        {
       Text = "??",
            Font = new Font("Segoe UI Emoji", 24),
      Location = new Point(15, 25),
                AutoSize = true
     };
    
  // Title
 Label lblTitle = new Label
            {
         Text = title,
  Font = new Font("Segoe UI", 11, FontStyle.Bold),
      ForeColor = Color.FromArgb(51, 51, 51),
       Location = new Point(60, 15),
         AutoSize = true
     };
            
            // Message
        Label lblMessage = new Label
      {
           Text = message,
Font = new Font("Segoe UI", 9),
 ForeColor = Color.FromArgb(102, 102, 102),
  Location = new Point(60, 40),
     Size = new Size(270, 50),
     AutoSize = false
      };
          
            // Close button
    Button btnClose = new Button
      {
       Text = "×",
        Font = new Font("Segoe UI", 12, FontStyle.Bold),
           ForeColor = Color.Gray,
     FlatStyle = FlatStyle.Flat,
    Size = new Size(30, 30),
            Location = new Point(notifyForm.Width - 35, 5),
      Cursor = Cursors.Hand
            };
 btnClose.FlatAppearance.BorderSize = 0;
    btnClose.Click += (s, e) => notifyForm.Close();
 
         panel.Controls.AddRange(new Control[] { accentBar, lblIcon, lblTitle, lblMessage, btnClose });
       notifyForm.Controls.Add(panel);
      
            // Border effect
notifyForm.Paint += (s, e) =>
            {
        ControlPaint.DrawBorder(e.Graphics, notifyForm.ClientRectangle,
          Color.FromArgb(200, 200, 200), ButtonBorderStyle.Solid);
     };
 
  // Auto close timer
     System.Windows.Forms.Timer closeTimer = new System.Windows.Forms.Timer { Interval = durationMs };
    closeTimer.Tick += (s, e) =>
            {
        closeTimer.Stop();
             notifyForm.Close();
            };

            notifyForm.Show(parentForm);
       closeTimer.Start();
        }
        
  public void Dispose()
        {
            if (!_isDisposed)
  {
       StopMonitoring();
 _isDisposed = true;
   }
        }
    }
    
    /// <summary>
    /// Event args cho s? ki?n l?ch h?n m?i
 /// </summary>
    public class NewAppointmentEventArgs : EventArgs
    {
        public int NewCount { get; set; }
      public DataTable Appointments { get; set; }
    }
}
