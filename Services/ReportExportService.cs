using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Services
{
    /// <summary>
    /// Service xu?t báo cáo cá nhân cho bác s?
    /// - Export ra Excel (CSV format - không c?n th? vi?n bên ngoài)
    /// - Export ra HTML (có th? in PDF t? browser)
  /// - Preview và in tr?c ti?p
    /// </summary>
    public static class ReportExportService
    {
        #region Export to CSV (Excel compatible)

        /// <summary>
    /// Export DataTable ra file CSV (m? ???c b?ng Excel)
        /// </summary>
        public static bool ExportToCsv(DataTable dt, string filePath, string title = "")
     {
    try
  {
        StringBuilder sb = new StringBuilder();

 // BOM for UTF-8 (?? Excel hi?u ti?ng Vi?t)
   sb.Append('\uFEFF');

 // Title n?u có
          if (!string.IsNullOrEmpty(title))
      {
          sb.AppendLine(title);
       sb.AppendLine($"Ngày xu?t: {DateTime.Now:dd/MM/yyyy HH:mm}");
   sb.AppendLine();
    }

         // Headers
string[] headers = new string[dt.Columns.Count];
     for (int i = 0; i < dt.Columns.Count; i++)
     {
     headers[i] = EscapeCsvValue(dt.Columns[i].ColumnName);
     }
       sb.AppendLine(string.Join(",", headers));

     // Data rows
         foreach (DataRow row in dt.Rows)
     {
   string[] values = new string[dt.Columns.Count];
        for (int i = 0; i < dt.Columns.Count; i++)
      {
     values[i] = EscapeCsvValue(row[i]?.ToString() ?? "");
      }
      sb.AppendLine(string.Join(",", values));
     }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
     Logger.LogAction("EXPORT_CSV", $"Exported {dt.Rows.Count} rows to {filePath}");
        return true;
          }
  catch (Exception ex)
     {
    Logger.LogAction("EXPORT_ERROR", $"CSV export failed: {ex.Message}");
   return false;
            }
  }

        private static string EscapeCsvValue(string value)
        {
   if (string.IsNullOrEmpty(value)) return "";
  
    // N?u ch?a d?u ph?y, xu?ng dòng ho?c d?u nháy kép thì c?n escape
   if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
  {
    return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
   return value;
      }

      #endregion

 #region Export to HTML (printable PDF)

        /// <summary>
        /// HTML encode ?? tránh XSS và hi?n th? ?úng ký t? ??c bi?t
        /// </summary>
private static string HtmlEncode(string value)
        {
 if (string.IsNullOrEmpty(value)) return "";
      return value
      .Replace("&", "&amp;")
      .Replace("<", "&lt;")
                .Replace(">", "&gt;")
          .Replace("\"", "&quot;")
   .Replace("'", "&#39;");
        }

        /// <summary>
  /// Export báo cáo ra HTML (có th? in ra PDF t? browser)
        /// </summary>
        public static bool ExportToHtml(DataTable dt, string filePath, string title, string subtitle = "")
   {
    try
       {
    StringBuilder html = new StringBuilder();

    html.AppendLine("<!DOCTYPE html>");
      html.AppendLine("<html lang='vi'>");
       html.AppendLine("<head>");
     html.AppendLine("    <meta charset='UTF-8'>");
                html.AppendLine("  <title>" + HtmlEncode(title) + "</title>");
    html.AppendLine("  <style>");
   html.AppendLine(" body { font-family: 'Segoe UI', Arial, sans-serif; margin: 40px; color: #333; }");
        html.AppendLine("      .header { text-align: center; margin-bottom: 30px; }");
  html.AppendLine("  .header h1 { color: #667eea; margin: 0; }");
        html.AppendLine("      .header p { color: #666; margin: 5px 0; }");
   html.AppendLine("        .meta { display: flex; justify-content: space-between; margin-bottom: 20px; font-size: 12px; color: #888; }");
  html.AppendLine("  table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
        html.AppendLine(" th { background: linear-gradient(135deg, #667eea, #764ba2); color: white; padding: 12px 8px; text-align: left; font-weight: 600; }");
    html.AppendLine("        td { padding: 10px 8px; border-bottom: 1px solid #e0e0e0; }");
     html.AppendLine("      tr:hover { background-color: #f5f5f5; }");
         html.AppendLine("    tr:nth-child(even) { background-color: #fafafa; }");
html.AppendLine("        .footer { text-align: center; margin-top: 30px; font-size: 12px; color: #888; }");
       html.AppendLine("        .summary { background: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0; }");
  html.AppendLine(".summary h3 { margin: 0 0 10px 0; color: #667eea; }");
      html.AppendLine(" @media print { body { margin: 20px; } .no-print { display: none; } }");
    html.AppendLine("     .btn-print { padding: 10px 20px; background: #667eea; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 14px; }");
         html.AppendLine("    .btn-print:hover { background: #5a6fd6; }");
     html.AppendLine("  </style>");
         html.AppendLine("</head>");
    html.AppendLine("<body>");

      // Header
        html.AppendLine("    <div class='header'>");
      html.AppendLine("        <h1>?? Phòng khám Nha khoa UTC2</h1>");
       html.AppendLine("        <h2>" + HtmlEncode(title) + "</h2>");
  if (!string.IsNullOrEmpty(subtitle))
   html.AppendLine("        <p>" + HtmlEncode(subtitle) + "</p>");
        html.AppendLine("    </div>");

            // Meta info
          html.AppendLine("    <div class='meta'>");
html.AppendLine("     <span>Ngày xu?t: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</span>");
       html.AppendLine("    <span>T?ng s?: " + dt.Rows.Count + " dòng</span>");
      html.AppendLine("</div>");

   // Print button
       html.AppendLine("    <div class='no-print' style='text-align: right; margin-bottom: 20px;'>");
  html.AppendLine(" <button class='btn-print' onclick='window.print()'>??? In báo cáo / L?u PDF</button>");
    html.AppendLine("    </div>");

  // Table
        html.AppendLine("<table>");
       
       // Headers
     html.AppendLine("      <thead><tr>");
  foreach (DataColumn col in dt.Columns)
      {
     html.AppendLine("  <th>" + HtmlEncode(col.ColumnName) + "</th>");
      }
   html.AppendLine("   </tr></thead>");

     // Rows
  html.AppendLine("        <tbody>");
          foreach (DataRow row in dt.Rows)
     {
html.AppendLine("        <tr>");
   foreach (DataColumn col in dt.Columns)
      {
   string value = row[col]?.ToString() ?? "";
  html.AppendLine("      <td>" + HtmlEncode(value) + "</td>");
     }
      html.AppendLine("  </tr>");
     }
 html.AppendLine("        </tbody>");
     html.AppendLine("    </table>");

      // Footer
      html.AppendLine("    <div class='footer'>");
      html.AppendLine("        <p>Báo cáo ???c xu?t t? h? th?ng Qu?n lý Phòng khám Nha khoa UTC2</p>");
    html.AppendLine("     <p>© " + DateTime.Now.Year + " - Dental Clinic Management System</p>");
   html.AppendLine(" </div>");

        html.AppendLine("</body>");
    html.AppendLine("</html>");

   File.WriteAllText(filePath, html.ToString(), Encoding.UTF8);
         Logger.LogAction("EXPORT_HTML", $"Exported {dt.Rows.Count} rows to {filePath}");
  return true;
            }
  catch (Exception ex)
            {
           Logger.LogAction("EXPORT_ERROR", $"HTML export failed: {ex.Message}");
          return false;
         }
        }

        #endregion

        #region Doctor Report Generation

        /// <summary>
/// Xu?t báo cáo khám b?nh c?a bác s? trong kho?ng th?i gian
        /// </summary>
     public static DataTable GetDoctorExaminationReport(int staffId, DateTime fromDate, DateTime toDate)
     {
         string query = @"
  SELECT 
      ROW_NUMBER() OVER (ORDER BY m.record_date) AS [STT],
        FORMAT(m.record_date, 'dd/MM/yyyy') AS [Ngày khám],
  u.fullname AS [B?nh nhân],
 u.phone AS [S?T],
                 m.diagnosis AS [Ch?n ?oán],
        m.treatment AS [?i?u tr?],
      ISNULL(i.total_amount, 0) AS [T?ng ti?n],
 CASE i.status 
 WHEN 'paid' THEN N'?ã thanh toán'
  WHEN 'unpaid' THEN N'Ch?a thanh toán'
ELSE N'N/A'
       END AS [Tr?ng thái]
                FROM MedicalRecord m
  INNER JOIN Patient p ON m.patient_id = p.patient_id
    INNER JOIN UserAccount u ON p.user_id = u.user_id
           LEFT JOIN Invoice i ON i.patient_id = p.patient_id 
       AND CAST(i.invoice_date AS DATE) = CAST(m.record_date AS DATE)
          WHERE m.staff_id = @staffId
      AND CAST(m.record_date AS DATE) BETWEEN @fromDate AND @toDate
     ORDER BY m.record_date DESC";

  return DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
          new SqlParameter("@staffId", staffId),
 new SqlParameter("@fromDate", fromDate.Date),
      new SqlParameter("@toDate", toDate.Date)
       });
        }

/// <summary>
    /// Xu?t th?ng kê t?ng h?p c?a bác s?
     /// </summary>
      public static DataTable GetDoctorStatisticsReport(int staffId, DateTime fromDate, DateTime toDate)
    {
      string query = @"
    SELECT 
      FORMAT(@fromDate, 'dd/MM/yyyy') + ' - ' + FORMAT(@toDate, 'dd/MM/yyyy') AS [K? báo cáo],
        COUNT(DISTINCT m.record_id) AS [S? l?n khám],
     COUNT(DISTINCT m.patient_id) AS [S? b?nh nhân],
       ISNULL(SUM(i.total_amount), 0) AS [T?ng doanh thu],
      COUNT(DISTINCT CASE WHEN i.status = 'paid' THEN i.invoice_id END) AS [?ã thanh toán],
           COUNT(DISTINCT CASE WHEN i.status = 'unpaid' THEN i.invoice_id END) AS [Ch?a thanh toán]
     FROM MedicalRecord m
       INNER JOIN Patient p ON m.patient_id = p.patient_id
    LEFT JOIN Invoice i ON i.patient_id = p.patient_id 
      AND i.staff_id = m.staff_id
     AND CAST(i.invoice_date AS DATE) = CAST(m.record_date AS DATE)
    WHERE m.staff_id = @staffId
         AND CAST(m.record_date AS DATE) BETWEEN @fromDate AND @toDate";

            return DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
   new SqlParameter("@staffId", staffId),
   new SqlParameter("@fromDate", fromDate.Date),
     new SqlParameter("@toDate", toDate.Date)
            });
        }

 /// <summary>
     /// Xu?t danh sách b?nh nhân ?ã khám
        /// </summary>
        public static DataTable GetDoctorPatientListReport(int staffId)
        {
         string query = @"
  SELECT 
        ROW_NUMBER() OVER (ORDER BY MAX(m.record_date) DESC) AS [STT],
        u.fullname AS [H? tên],
        u.phone AS [S?T],
     u.email AS [Email],
    CASE p.gender 
       WHEN 'Male' THEN N'Nam'
            WHEN 'Female' THEN N'N?'
  ELSE N'Khác'
         END AS [Gi?i tính],
    DATEDIFF(YEAR, p.date_of_birth, GETDATE()) AS [Tu?i],
  COUNT(m.record_id) AS [S? l?n khám],
     FORMAT(MAX(m.record_date), 'dd/MM/yyyy') AS [L?n khám cu?i]
        FROM MedicalRecord m
                INNER JOIN Patient p ON m.patient_id = p.patient_id
      INNER JOIN UserAccount u ON p.user_id = u.user_id
    WHERE m.staff_id = @staffId
       GROUP BY u.fullname, u.phone, u.email, p.gender, p.date_of_birth
    ORDER BY MAX(m.record_date) DESC";

     return DatabaseHelper.ExecuteQuery(query, new SqlParameter[] {
    new SqlParameter("@staffId", staffId)
          });
   }

        #endregion

        #region Dialog Helpers

      /// <summary>
      /// Hi?n th? dialog ch?n n?i l?u file và export
        /// </summary>
        public static void ShowExportDialog(DataTable dt, string defaultFileName, string reportTitle)
 {
            using (SaveFileDialog sfd = new SaveFileDialog())
     {
         sfd.Title = "Xu?t báo cáo";
           sfd.Filter = "Excel CSV (*.csv)|*.csv|HTML (*.html)|*.html";
   sfd.FileName = defaultFileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
       sfd.FilterIndex = 1;

   if (sfd.ShowDialog() == DialogResult.OK)
            {
   bool success = false;
string ext = Path.GetExtension(sfd.FileName).ToLower();

 if (ext == ".csv")
     {
    success = ExportToCsv(dt, sfd.FileName, reportTitle);
   }
        else if (ext == ".html")
{
    success = ExportToHtml(dt, sfd.FileName, reportTitle);
      }

         if (success)
       {
         var result = MessageBox.Show(
  $"Xu?t báo cáo thành công!\n\nFile: {sfd.FileName}\n\nB?n có mu?n m? file ngay?",
       "Thành công",
           MessageBoxButtons.YesNo,
    MessageBoxIcon.Information);

     if (result == DialogResult.Yes)
  {
 try
 {
       System.Diagnostics.Process.Start(sfd.FileName);
      }
 catch (Exception ex)
      {
   MessageBox.Show($"Không th? m? file: {ex.Message}", "L?i", 
  MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
         }
      }
  else
       {
     MessageBox.Show("Xu?t báo cáo th?t b?i!", "L?i", 
       MessageBoxButtons.OK, MessageBoxIcon.Error);
   }
  }
     }
        }

        #endregion
    }
}
