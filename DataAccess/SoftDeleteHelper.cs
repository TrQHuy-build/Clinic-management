using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DentalClinicManagement.DataAccess
{
    /// <summary>
    /// Helper class for soft delete operations
    /// Provides methods to soft delete, restore, and permanently delete records
    /// Issue #8: Database Design - Soft Delete Implementation
    /// </summary>
    public class SoftDeleteHelper
    {
        private readonly string _connectionString;

        public SoftDeleteHelper(string connectionString = null)
        {
            _connectionString = connectionString ?? ConfigManager.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Soft delete a record by setting is_deleted = 1
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <param name="deletedBy">User who deleted the record</param>
        /// <returns>True if successful</returns>
        public bool SoftDelete(string tableName, int recordId, string deletedBy = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        UPDATE {tableName}
                        SET 
                            is_deleted = 1,
                            deleted_at = @DeletedAt,
                            deleted_by = @DeletedBy
                        WHERE id = @Id AND is_deleted = 0";

                    int rowsAffected = connection.Execute(sql, new
                    {
                        Id = recordId,
                        DeletedAt = DateTime.Now,
                        DeletedBy = deletedBy ?? Environment.UserName
                    });

                    bool success = rowsAffected > 0;

                    if (success)
                    {
                        Logger.Info($"Soft deleted record: {tableName} [ID: {recordId}] by {deletedBy}");
                        
                        // Invalidate cache
                        CacheManager.Instance.InvalidateRelated(tableName.ToLower());
                    }
                    else
                    {
                        Logger.Warning($"Record not found or already deleted: {tableName} [ID: {recordId}]");
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error soft deleting record from {tableName} [ID: {recordId}]", ex);
                throw;
            }
        }

        /// <summary>
        /// Soft delete multiple records
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordIds">Array of record IDs</param>
        /// <param name="deletedBy">User who deleted the records</param>
        /// <returns>Number of records deleted</returns>
        public int SoftDeleteBatch(string tableName, int[] recordIds, string deletedBy = null)
        {
            if (recordIds == null || recordIds.Length == 0)
                return 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        UPDATE {tableName}
                        SET 
                            is_deleted = 1,
                            deleted_at = @DeletedAt,
                            deleted_by = @DeletedBy
                        WHERE id IN @Ids AND is_deleted = 0";

                    int rowsAffected = connection.Execute(sql, new
                    {
                        Ids = recordIds,
                        DeletedAt = DateTime.Now,
                        DeletedBy = deletedBy ?? Environment.UserName
                    });

                    if (rowsAffected > 0)
                    {
                        Logger.Info($"Soft deleted {rowsAffected} records from {tableName} by {deletedBy}");
                        CacheManager.Instance.InvalidateRelated(tableName.ToLower());
                    }

                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error soft deleting batch from {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Restore a soft-deleted record
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <returns>True if successful</returns>
        public bool Restore(string tableName, int recordId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        UPDATE {tableName}
                        SET 
                            is_deleted = 0,
                            deleted_at = NULL,
                            deleted_by = NULL
                        WHERE id = @Id AND is_deleted = 1";

                    int rowsAffected = connection.Execute(sql, new { Id = recordId });

                    bool success = rowsAffected > 0;

                    if (success)
                    {
                        Logger.Info($"Restored record: {tableName} [ID: {recordId}]");
                        CacheManager.Instance.InvalidateRelated(tableName.ToLower());
                    }
                    else
                    {
                        Logger.Warning($"Record not found or not deleted: {tableName} [ID: {recordId}]");
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error restoring record from {tableName} [ID: {recordId}]", ex);
                throw;
            }
        }

        /// <summary>
        /// Permanently delete a soft-deleted record
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <returns>True if successful</returns>
        public bool HardDelete(string tableName, int recordId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        DELETE FROM {tableName}
                        WHERE id = @Id AND is_deleted = 1";

                    int rowsAffected = connection.Execute(sql, new { Id = recordId });

                    bool success = rowsAffected > 0;

                    if (success)
                    {
                        Logger.Warning($"HARD DELETED record: {tableName} [ID: {recordId}] - PERMANENT!");
                        CacheManager.Instance.InvalidateRelated(tableName.ToLower());
                    }
                    else
                    {
                        Logger.Warning($"Record not found or not soft-deleted: {tableName} [ID: {recordId}]");
                    }

                    return success;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error hard deleting record from {tableName} [ID: {recordId}]", ex);
                throw;
            }
        }

        /// <summary>
        /// Purge old soft-deleted records (permanently delete)
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="daysOld">Delete records older than this many days</param>
        /// <returns>Number of records permanently deleted</returns>
        public int PurgeOldRecords(string tableName, int daysOld = 90)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    DateTime cutoffDate = DateTime.Now.AddDays(-daysOld);

                    string sql = $@"
                        DELETE FROM {tableName}
                        WHERE is_deleted = 1 
                        AND deleted_at < @CutoffDate";

                    int rowsAffected = connection.Execute(sql, new { CutoffDate = cutoffDate });

                    if (rowsAffected > 0)
                    {
                        Logger.Warning($"Purged {rowsAffected} old records from {tableName} (older than {daysOld} days) - PERMANENT!");
                    }

                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error purging old records from {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Check if a record is soft-deleted
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <returns>True if deleted, false if active, null if not found</returns>
        public bool? IsDeleted(string tableName, int recordId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        SELECT is_deleted 
                        FROM {tableName}
                        WHERE id = @Id";

                    return connection.QueryFirstOrDefault<bool?>(sql, new { Id = recordId });
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking delete status for {tableName} [ID: {recordId}]", ex);
                throw;
            }
        }

        /// <summary>
        /// Get all soft-deleted records from a table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>DataTable with deleted records</returns>
        public DataTable GetDeletedRecords(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = $@"
                        SELECT * 
                        FROM {tableName}
                        WHERE is_deleted = 1
                        ORDER BY deleted_at DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting deleted records from {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get statistics for a table (total, active, deleted counts)
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Tuple with (Total, Active, Deleted) counts</returns>
        public (int Total, int Active, int Deleted, double ActivePercentage) GetTableStatistics(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        SELECT 
                            COUNT(*) AS Total,
                            SUM(CASE WHEN is_deleted = 0 THEN 1 ELSE 0 END) AS Active,
                            SUM(CASE WHEN is_deleted = 1 THEN 1 ELSE 0 END) AS Deleted
                        FROM {tableName}";

                    var result = connection.QueryFirst<dynamic>(sql);

                    int total = result.Total;
                    int active = result.Active;
                    int deleted = result.Deleted;
                    double activePercentage = total > 0 ? (active * 100.0 / total) : 0;

                    return (total, active, deleted, activePercentage);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting statistics for {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get count of active records (non-deleted)
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Count of active records</returns>
        public int GetActiveCount(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        SELECT COUNT(*) 
                        FROM {tableName}
                        WHERE is_deleted = 0";

                    return connection.QueryFirst<int>(sql);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting active count from {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get count of deleted records
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Count of deleted records</returns>
        public int GetDeletedCount(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        SELECT COUNT(*) 
                        FROM {tableName}
                        WHERE is_deleted = 1";

                    return connection.QueryFirst<int>(sql);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting deleted count from {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Bulk restore multiple records
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="recordIds">Array of record IDs to restore</param>
        /// <returns>Number of records restored</returns>
        public int RestoreBatch(string tableName, int[] recordIds)
        {
            if (recordIds == null || recordIds.Length == 0)
                return 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $@"
                        UPDATE {tableName}
                        SET 
                            is_deleted = 0,
                            deleted_at = NULL,
                            deleted_by = NULL
                        WHERE id IN @Ids AND is_deleted = 1";

                    int rowsAffected = connection.Execute(sql, new { Ids = recordIds });

                    if (rowsAffected > 0)
                    {
                        Logger.Info($"Restored {rowsAffected} records from {tableName}");
                        CacheManager.Instance.InvalidateRelated(tableName.ToLower());
                    }

                    return rowsAffected;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error restoring batch from {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Check if table supports soft delete
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>True if table has is_deleted column</returns>
        public bool SupportsSoftDelete(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = @TableName 
                        AND COLUMN_NAME = 'is_deleted'";

                    int columnExists = connection.QueryFirst<int>(sql, new { TableName = tableName });
                    return columnExists > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking soft delete support for {tableName}", ex);
                throw;
            }
        }
    }
}
