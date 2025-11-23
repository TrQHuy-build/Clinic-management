using System.Data;
using System.IO;
using System.Text;

namespace DentalClinicManagement.Utils
{
    public static class CsvExporter
    {
        public static void ExportToCsv(DataTable table, string filePath)
        {
            var sb = new StringBuilder();

            // headers
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append(table.Columns[i].ColumnName);
                if (i < table.Columns.Count - 1) sb.Append(",");
            }
            sb.AppendLine();

            // rows
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    var value = row[i]?.ToString()?.Replace("\"", "\"\"");
                    if (value != null && (value.Contains(",") || value.Contains("\n")))
                        sb.Append('"').Append(value).Append('"');
                    else
                        sb.Append(value);

                    if (i < table.Columns.Count - 1) sb.Append(",");
                }
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
