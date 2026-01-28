using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.DataAccess
{
    /// <summary>
    /// Lớp hỗ trợ kết nối và thao tác với SQL Server
    /// </summary>
    public class DatabaseHelper
    {
        // Connection string - ĐỌC TỪ APP.CONFIG (không hardcode nữa!)
        private static string connectionString;

        /// <summary>
        /// Static constructor - khởi tạo connection string từ config
        /// </summary>
        static DatabaseHelper()
        {
            try
            {
                connectionString = ConfigHelper.GetConnectionString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Không thể đọc connection string từ App.config!\n\n" +
                    $"Error: {ex.Message}\n\n" +
                    "Vui lòng kiểm tra file App.config có đúng cấu hình không.",
                    "Lỗi Configuration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                // Fallback to default (for backward compatibility)
                
                //connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DentalClinicDB;Integrated Security=True;TrustServerCertificate=True";
            }
        }

        /// <summary>
        /// Cập nhật connection string từ nơi khác (cho testing hoặc dynamic config)
        /// </summary>
        public static void SetConnectionString(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connStr));
            
            connectionString = connStr;
        }

        /// <summary>
        /// Lấy connection string hiện tại (để debug hoặc logging)
        /// </summary>
        public static string GetConnectionStringValue()
        {
            return connectionString;
        }

        /// <summary>
        /// Lấy connection hiện tại (mới). Caller nên quản lý lifecycle nếu dùng trực tiếp.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Beging a database transaction and return a disposable scope that ensures both
        /// the transaction and its connection are disposed/closed when the scope is disposed.
        /// The returned DbTransactionScope can implicitly convert to SqlTransaction for backward compatibility.
        /// </summary>
        public static DbTransactionScope BeginTransaction()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            var tran = conn.BeginTransaction();
            return new DbTransactionScope(conn, tran);
        }

        /// <summary>
        /// Disposable wrapper that holds a SqlConnection and its SqlTransaction.
        /// It exposes implicit conversion to SqlTransaction so existing methods accepting SqlTransaction still work.
        /// </summary>
        public sealed class DbTransactionScope : IDisposable
        {
            public SqlConnection Connection { get; }
            public SqlTransaction Transaction { get; }

            public DbTransactionScope(SqlConnection conn, SqlTransaction tran)
            {
                Connection = conn ?? throw new ArgumentNullException(nameof(conn));
                Transaction = tran ?? throw new ArgumentNullException(nameof(tran));
            }

            // Allow passing DbTransactionScope where SqlTransaction is expected.
            public static implicit operator SqlTransaction(DbTransactionScope scope) => scope?.Transaction;

            public void Commit() => Transaction?.Commit();
            public void Rollback() => Transaction?.Rollback();

            public void Dispose()
            {
                try
                {
                    Transaction?.Dispose();
                }
                catch { /* swallow to avoid throwing on dispose */ }
                finally
                {
                    try
                    {
                        if (Connection != null)
                        {
                            Connection.Close();
                            Connection.Dispose();
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Thực thi câu lệnh INSERT, UPDATE, DELETE
        /// Supports optional transaction.
        /// </summary>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null, SqlTransaction transaction = null)
        {
            try
            {
                if (transaction != null)
                {
                    using (var cmd = new SqlCommand(query, transaction.Connection, transaction))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi SQL: {ex.Message}", "Lỗi Database",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SELECT và trả về DataTable
        /// Supports optional transaction.
        /// </summary>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null, SqlTransaction transaction = null)
        {
            DataTable dt = new DataTable();
            try
            {
                if (transaction != null)
                {
                    using (var cmd = new SqlCommand(query, transaction.Connection, transaction))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi SQL: {ex.Message}", "Lỗi Database",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        /// <summary>
        /// Thực thi câu lệnh SELECT và trả về giá trị đơn (scalar)
        /// Hỗ trợ transaction nếu cần
        /// </summary>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null, SqlTransaction transaction = null)
        {
            try
            {
                // Nếu có transaction, sử dụng connection từ transaction
                if (transaction != null)
                {
                    using (var cmd = new SqlCommand(query, transaction.Connection, transaction))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);

                        return cmd.ExecuteScalar();
                    }
                }
                else
                {
                    // Không có transaction, tạo connection mới
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        conn.Open();
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Lỗi SQL: {ex.Message}", "Lỗi Database",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra kết nối database
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể kết nối database:\n{ex.Message}",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Execute query with automatic soft delete filtering
        /// Only returns non-deleted records (is_deleted = 0)
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <param name="transaction">Optional transaction</param>
        /// <returns>DataTable with active records only</returns>
        public static DataTable ExecuteQueryActiveOnly(string query, SqlParameter[] parameters = null, SqlTransaction transaction = null)
        {
            // Add WHERE is_deleted = 0 filter if not already present
            if (!query.ToUpper().Contains("IS_DELETED"))
            {
                // Simple approach: add to WHERE clause if exists, or create new WHERE
                if (query.ToUpper().Contains("WHERE"))
                {
                    query = System.Text.RegularExpressions.Regex.Replace(query, "WHERE", "WHERE is_deleted = 0 AND", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
                else if (query.ToUpper().Contains("ORDER BY"))
                {
                    query = System.Text.RegularExpressions.Regex.Replace(query, "ORDER BY", "WHERE is_deleted = 0 ORDER BY", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
                else
                {
                    // Append WHERE clause at the end
                    query += " WHERE is_deleted = 0";
                }
            }

            return ExecuteQuery(query, parameters, transaction);
        }

        /// <summary>
        /// Soft delete a record by setting is_deleted = 1
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <param name="deletedBy">User who deleted the record</param>
        /// <returns>Number of affected rows</returns>
        public static int SoftDelete(string tableName, int recordId, string deletedBy = null)
        {
            string query = $@"
                UPDATE {tableName}
                SET 
                    is_deleted = 1,
                    deleted_at = @DeletedAt,
                    deleted_by = @DeletedBy
                WHERE id = @Id AND is_deleted = 0";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", recordId),
                new SqlParameter("@DeletedAt", DateTime.Now),
                new SqlParameter("@DeletedBy", deletedBy ?? Environment.UserName)
            };

            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Restore a soft-deleted record
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <returns>Number of affected rows</returns>
        public static int RestoreSoftDeleted(string tableName, int recordId)
        {
            string query = $@"
                UPDATE {tableName}
                SET 
                    is_deleted = 0,
                    deleted_at = NULL,
                    deleted_by = NULL
                WHERE id = @Id AND is_deleted = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", recordId)
            };

            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Permanently delete a soft-deleted record
        /// WARNING: This action cannot be undone!
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <returns>Number of affected rows</returns>
        public static int HardDelete(string tableName, int recordId)
        {
            string query = $@"
                DELETE FROM {tableName}
                WHERE id = @Id AND is_deleted = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", recordId)
            };

            return ExecuteNonQuery(query, parameters);
        }
    }
}