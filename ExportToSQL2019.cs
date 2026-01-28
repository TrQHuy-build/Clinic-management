using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Linq;

class ExportToSQL2019
{
    static void Main(string[] args)
    {
        string sourceConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True";
        string outputFile = @"D:\DatabaseBackup\DentalClinicDB_SQL2019_Compatible.sql";
        
        Console.WriteLine("=========================================");
        Console.WriteLine("EXPORT DATABASE TO SQL SERVER 2019");
        Console.WriteLine("=========================================\n");
        
        try
        {
            using (var conn = new SqlConnection(sourceConn))
            {
                conn.Open();
                Console.WriteLine("✓ Đã kết nối LocalDB\n");
                
                var sb = new StringBuilder();
                
                // Header
                sb.AppendLine("-- DentalClinicDB Export for SQL Server 2019");
                sb.AppendLine("-- Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine("-- Source: LocalDB SQL Server 2022");
                sb.AppendLine("-- Target: SQL Server 2019");
                sb.AppendLine();
                sb.AppendLine("USE master;");
                sb.AppendLine("GO");
                sb.AppendLine();
                sb.AppendLine("IF EXISTS (SELECT name FROM sys.databases WHERE name = 'DentalClinicDB')");
                sb.AppendLine("BEGIN");
                sb.AppendLine("    ALTER DATABASE DentalClinicDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
                sb.AppendLine("    DROP DATABASE DentalClinicDB;");
                sb.AppendLine("END");
                sb.AppendLine("GO");
                sb.AppendLine();
                sb.AppendLine("CREATE DATABASE DentalClinicDB");
                sb.AppendLine("COLLATE SQL_Latin1_General_CP1_CI_AS;");
                sb.AppendLine("GO");
                sb.AppendLine();
                sb.AppendLine("USE DentalClinicDB;");
                sb.AppendLine("GO");
                sb.AppendLine();
                
                Console.WriteLine("Đang export schema...");
                
                // Get all tables
                var cmd = new SqlCommand(@"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE = 'BASE TABLE'
                    AND TABLE_NAME NOT IN ('sysdiagrams')
                    ORDER BY TABLE_NAME", conn);
                
                var tables = new System.Collections.Generic.List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
                
                Console.WriteLine("Tìm thấy {0} tables\n", tables.Count);
                
                // Export schema for each table
                foreach (var table in tables)
                {
                    Console.WriteLine("  → Exporting {0}...", table);
                    sb.AppendLine("-- Table: " + table);
                    sb.AppendLine(GetCreateTableScript(conn, table));
                    sb.AppendLine("GO");
                    sb.AppendLine();
                }
                
                Console.WriteLine("\nĐang export data...");
                
                // Export data for each table
                foreach (var table in tables)
                {
                    Console.Write("  → Exporting data from {0}...", table);
                    int rowCount = ExportTableData(conn, table, sb);
                    Console.WriteLine(" ({0} rows)", rowCount);
                }
                
                // Export foreign keys
                sb.AppendLine("\n-- Foreign Keys");
                sb.AppendLine(GetForeignKeys(conn));
                sb.AppendLine("GO");
                
                // Write to file
                File.WriteAllText(outputFile, sb.ToString(), Encoding.UTF8);
                
                var fileInfo = new FileInfo(outputFile);
                Console.WriteLine("\n=========================================");
                Console.WriteLine("✓ EXPORT THÀNH CÔNG!");
                Console.WriteLine("=========================================");
                Console.WriteLine("File: {0}", outputFile);
                Console.WriteLine("Size: {0} MB", fileInfo.Length / (1024 * 1024));
                Console.WriteLine("\nBước tiếp theo:");
                Console.WriteLine("1. Chạy lệnh: sqlcmd -S \"localhost\" -E -i \"{0}\"", outputFile);
                Console.WriteLine("2. Tạo SQL user");
                Console.WriteLine("3. Test connection");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n✗ LỖI: " + ex.Message);
            Environment.Exit(1);
        }
    }
    
    static string GetCreateTableScript(SqlConnection conn, string tableName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CREATE TABLE [" + tableName + "] (");
        
                // Get columns
                var cmd = new SqlCommand(string.Format(@"
                    SELECT 
                        c.COLUMN_NAME,
                        c.DATA_TYPE,
                        c.CHARACTER_MAXIMUM_LENGTH,
                        c.NUMERIC_PRECISION,
                        c.NUMERIC_SCALE,
                        c.IS_NULLABLE,
                        c.COLUMN_DEFAULT,
                        CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY,
                        CASE WHEN c.DATA_TYPE IN ('int', 'bigint') 
                             AND COLUMNPROPERTY(OBJECT_ID(c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') = 1 
                             THEN 1 ELSE 0 END AS IS_IDENTITY
                    FROM INFORMATION_SCHEMA.COLUMNS c
                    LEFT JOIN (
                        SELECT ku.TABLE_NAME, ku.COLUMN_NAME
                        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                            ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                        WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                    ) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
                    WHERE c.TABLE_NAME = '{0}'
                    ORDER BY c.ORDINAL_POSITION", tableName), conn);        var columns = new System.Collections.Generic.List<string>();
        var pkColumns = new System.Collections.Generic.List<string>();
        
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string colName = reader.GetString(0);
                string dataType = reader.GetString(1).ToUpper();
                bool isNullable = reader.GetString(5) == "YES";
                bool isPK = reader.GetInt32(7) == 1;
                bool isIdentity = reader.GetInt32(8) == 1;
                
                var colDef = new StringBuilder();
                colDef.Append("    [" + colName + "] ");
                
                // Data type
                if (dataType == "NVARCHAR" || dataType == "VARCHAR" || dataType == "CHAR" || dataType == "NCHAR")
                {
                    int maxLen = reader.IsDBNull(2) ? -1 : reader.GetInt32(2);
                    colDef.Append(dataType + "(" + (maxLen == -1 ? "MAX" : maxLen.ToString()) + ")");
                }
                else if (dataType == "DECIMAL" || dataType == "NUMERIC")
                {
                    int precision = reader.IsDBNull(3) ? 18 : Convert.ToInt32(reader.GetValue(3));
                    int scale = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4));
                    colDef.Append(dataType + "(" + precision + "," + scale + ")");
                }
                else
                {
                    colDef.Append(dataType);
                }
                
                // Identity
                if (isIdentity)
                {
                    colDef.Append(" IDENTITY(1,1)");
                }
                
                // Nullable
                colDef.Append(isNullable ? " NULL" : " NOT NULL");
                
                // Default
                if (!reader.IsDBNull(6))
                {
                    colDef.Append(" DEFAULT " + reader.GetString(6));
                }
                
                columns.Add(colDef.ToString());
                
                if (isPK)
                {
                    pkColumns.Add(colName);
                }
            }
        }
        
        sb.AppendLine(string.Join(",\n", columns));
        
        // Primary key
        if (pkColumns.Count > 0)
        {
            sb.AppendLine("    CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED ([" + string.Join("], [", pkColumns) + "])");
        }
        
        sb.AppendLine(");");
        
        return sb.ToString();
    }
    
    static int ExportTableData(SqlConnection conn, string tableName, StringBuilder sb)
    {
        var cmd = new SqlCommand("SELECT * FROM [" + tableName + "]", conn);
        int rowCount = 0;
        
        using (var reader = cmd.ExecuteReader())
        {
            if (!reader.HasRows) return 0;
            
            sb.AppendLine("\n-- Data for " + tableName);
            sb.AppendLine("SET IDENTITY_INSERT [" + tableName + "] ON;");
            
            while (reader.Read())
            {
                var values = new System.Collections.Generic.List<string>();
                
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        values.Add("NULL");
                    }
                    else
                    {
                        var value = reader.GetValue(i);
                        var type = reader.GetFieldType(i);
                        
                        if (type == typeof(string))
                        {
                            values.Add("N'" + value.ToString().Replace("'", "''") + "'");
                        }
                        else if (type == typeof(DateTime))
                        {
                            values.Add("'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        }
                        else if (type == typeof(bool))
                        {
                            values.Add((bool)value ? "1" : "0");
                        }
                        else if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
                        {
                            values.Add(value.ToString().Replace(",", "."));
                        }
                        else
                        {
                            values.Add(value.ToString());
                        }
                    }
                }
                
                var columns = Enumerable.Range(0, reader.FieldCount)
                    .Select(i => "[" + reader.GetName(i) + "]");
                
                sb.AppendLine("INSERT INTO [" + tableName + "] (" + string.Join(", ", columns) + ") VALUES (" + string.Join(", ", values) + ");");
                rowCount++;
            }
            
            sb.AppendLine("SET IDENTITY_INSERT [" + tableName + "] OFF;");
            sb.AppendLine("GO");
        }
        
        return rowCount;
    }
    
    static string GetForeignKeys(SqlConnection conn)
    {
        var sb = new StringBuilder();
        var cmd = new SqlCommand(@"
            SELECT 
                fk.name AS FK_Name,
                tp.name AS Parent_Table,
                cp.name AS Parent_Column,
                tr.name AS Referenced_Table,
                cr.name AS Referenced_Column
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
            INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
            INNER JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
            INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
            ORDER BY tp.name, fk.name", conn);
        
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string fkName = reader.GetString(0);
                string parentTable = reader.GetString(1);
                string parentCol = reader.GetString(2);
                string refTable = reader.GetString(3);
                string refCol = reader.GetString(4);
                
                sb.AppendLine("ALTER TABLE [" + parentTable + "] ADD CONSTRAINT [" + fkName + "]");
                sb.AppendLine("    FOREIGN KEY ([" + parentCol + "]) REFERENCES [" + refTable + "] ([" + refCol + "]);");
            }
        }
        
        return sb.ToString();
    }
}
