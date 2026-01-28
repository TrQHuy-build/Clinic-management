using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DentalClinicManagement.DataAccess
{
    /// <summary>
    /// Helper class for database index management and optimization
    /// Provides methods to analyze, create, and maintain indexes
    /// Issue #8: Database Design - Index Management
    /// </summary>
    public class IndexManager
    {
        private readonly string _connectionString;

        public IndexManager(string connectionString = null)
        {
            _connectionString = connectionString ?? ConfigManager.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Check if an index exists
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="indexName">Index name</param>
        /// <returns>True if index exists</returns>
        public bool IndexExists(string tableName, string indexName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = @"
                        SELECT COUNT(*) 
                        FROM sys.indexes 
                        WHERE name = @IndexName 
                        AND object_id = OBJECT_ID(@TableName)";

                    int count = connection.QueryFirst<int>(sql, new 
                    { 
                        IndexName = indexName, 
                        TableName = tableName 
                    });

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error checking index existence: {indexName} on {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get all indexes for a table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>DataTable with index information</returns>
        public DataTable GetTableIndexes(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            i.name AS IndexName,
                            i.type_desc AS IndexType,
                            i.is_unique AS IsUnique,
                            i.is_primary_key AS IsPrimaryKey,
                            STUFF((
                                SELECT ', ' + c.name
                                FROM sys.index_columns ic
                                INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                                WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
                                AND ic.is_included_column = 0
                                ORDER BY ic.key_ordinal
                                FOR XML PATH('')
                            ), 1, 2, '') AS IndexColumns,
                            STUFF((
                                SELECT ', ' + c.name
                                FROM sys.index_columns ic
                                INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                                WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
                                AND ic.is_included_column = 1
                                ORDER BY ic.key_ordinal
                                FOR XML PATH('')
                            ), 1, 2, '') AS IncludedColumns
                        FROM sys.indexes i
                        WHERE i.object_id = OBJECT_ID(@TableName)
                        AND i.name IS NOT NULL
                        ORDER BY i.name";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting indexes for table: {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Analyze index usage statistics
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>DataTable with index usage statistics</returns>
        public DataTable GetIndexUsageStatistics(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            i.name AS IndexName,
                            i.type_desc AS IndexType,
                            ius.user_seeks AS UserSeeks,
                            ius.user_scans AS UserScans,
                            ius.user_lookups AS UserLookups,
                            ius.user_updates AS UserUpdates,
                            ius.last_user_seek AS LastUserSeek,
                            ius.last_user_scan AS LastUserScan,
                            CASE 
                                WHEN ius.user_seeks + ius.user_scans + ius.user_lookups = 0 THEN 'UNUSED'
                                WHEN ius.user_seeks + ius.user_scans + ius.user_lookups < ius.user_updates THEN 'LOW_USAGE'
                                ELSE 'ACTIVE'
                            END AS UsageStatus
                        FROM sys.indexes i
                        LEFT JOIN sys.dm_db_index_usage_stats ius 
                            ON i.object_id = ius.object_id 
                            AND i.index_id = ius.index_id
                            AND ius.database_id = DB_ID()
                        WHERE i.object_id = OBJECT_ID(@TableName)
                        AND i.name IS NOT NULL
                        ORDER BY 
                            (ius.user_seeks + ius.user_scans + ius.user_lookups) DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting index usage statistics for: {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Find missing indexes based on query patterns
        /// </summary>
        /// <param name="tableName">Table name (optional)</param>
        /// <returns>DataTable with missing index recommendations</returns>
        public DataTable FindMissingIndexes(string tableName = null)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            OBJECT_NAME(mid.object_id) AS TableName,
                            mid.equality_columns AS EqualityColumns,
                            mid.inequality_columns AS InequalityColumns,
                            mid.included_columns AS IncludedColumns,
                            migs.unique_compiles AS UniqueCompiles,
                            migs.user_seeks AS UserSeeks,
                            migs.user_scans AS UserScans,
                            migs.avg_total_user_cost AS AvgTotalUserCost,
                            migs.avg_user_impact AS AvgUserImpact,
                            (migs.user_seeks + migs.user_scans) * migs.avg_total_user_cost * migs.avg_user_impact AS ImpactScore,
                            'CREATE NONCLUSTERED INDEX IX_' + 
                            OBJECT_NAME(mid.object_id) + '_' +
                            REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns, ''), ', ', '_'), '[', ''), ']', '') +
                            CASE WHEN mid.inequality_columns IS NOT NULL THEN '_' + REPLACE(REPLACE(REPLACE(mid.inequality_columns, ', ', '_'), '[', ''), ']', '') ELSE '' END +
                            ' ON ' + OBJECT_SCHEMA_NAME(mid.object_id) + '.' + OBJECT_NAME(mid.object_id) +
                            ' (' + ISNULL(mid.equality_columns, '') + 
                            CASE WHEN mid.inequality_columns IS NOT NULL THEN ', ' + mid.inequality_columns ELSE '' END + ')' +
                            CASE WHEN mid.included_columns IS NOT NULL THEN ' INCLUDE (' + mid.included_columns + ')' ELSE '' END AS CreateIndexStatement
                        FROM sys.dm_db_missing_index_details mid
                        INNER JOIN sys.dm_db_missing_index_groups mig 
                            ON mid.index_handle = mig.index_handle
                        INNER JOIN sys.dm_db_missing_index_group_stats migs 
                            ON mig.index_group_handle = migs.group_handle
                        WHERE mid.database_id = DB_ID()
                        " + (string.IsNullOrEmpty(tableName) ? "" : "AND OBJECT_NAME(mid.object_id) = @TableName") + @"
                        ORDER BY ImpactScore DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        if (!string.IsNullOrEmpty(tableName))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                        }
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error finding missing indexes", ex);
                throw;
            }
        }

        /// <summary>
        /// Get fragmented indexes that need rebuilding
        /// </summary>
        /// <param name="fragmentationThreshold">Fragmentation percentage threshold (default 30%)</param>
        /// <returns>DataTable with fragmented indexes</returns>
        public DataTable GetFragmentedIndexes(double fragmentationThreshold = 30.0)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            OBJECT_NAME(ips.object_id) AS TableName,
                            i.name AS IndexName,
                            ips.index_type_desc AS IndexType,
                            ips.avg_fragmentation_in_percent AS FragmentationPercent,
                            ips.page_count AS PageCount,
                            CASE 
                                WHEN ips.avg_fragmentation_in_percent > 30 THEN 'REBUILD'
                                WHEN ips.avg_fragmentation_in_percent > 10 THEN 'REORGANIZE'
                                ELSE 'OK'
                            END AS Recommendation,
                            'ALTER INDEX ' + i.name + ' ON ' + OBJECT_SCHEMA_NAME(ips.object_id) + '.' + OBJECT_NAME(ips.object_id) + 
                            CASE 
                                WHEN ips.avg_fragmentation_in_percent > 30 THEN ' REBUILD'
                                ELSE ' REORGANIZE'
                            END AS MaintenanceCommand
                        FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
                        INNER JOIN sys.indexes i 
                            ON ips.object_id = i.object_id 
                            AND ips.index_id = i.index_id
                        WHERE ips.avg_fragmentation_in_percent > @Threshold
                        AND ips.page_count > 100
                        AND i.name IS NOT NULL
                        ORDER BY ips.avg_fragmentation_in_percent DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@Threshold", fragmentationThreshold);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error getting fragmented indexes", ex);
                throw;
            }
        }

        /// <summary>
        /// Rebuild a specific index
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="indexName">Index name</param>
        /// <returns>True if successful</returns>
        public bool RebuildIndex(string tableName, string indexName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $"ALTER INDEX {indexName} ON {tableName} REBUILD";
                    
                    connection.Execute(sql);
                    
                    Logger.Info($"Rebuilt index: {indexName} on {tableName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error rebuilding index: {indexName} on {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Reorganize a specific index
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="indexName">Index name</param>
        /// <returns>True if successful</returns>
        public bool ReorganizeIndex(string tableName, string indexName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $"ALTER INDEX {indexName} ON {tableName} REORGANIZE";
                    
                    connection.Execute(sql);
                    
                    Logger.Info($"Reorganized index: {indexName} on {tableName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error reorganizing index: {indexName} on {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Rebuild all fragmented indexes in the database
        /// </summary>
        /// <param name="fragmentationThreshold">Fragmentation percentage threshold</param>
        /// <returns>Number of indexes rebuilt</returns>
        public int RebuildAllFragmentedIndexes(double fragmentationThreshold = 30.0)
        {
            try
            {
                var fragmentedIndexes = GetFragmentedIndexes(fragmentationThreshold);
                int rebuiltCount = 0;

                foreach (DataRow row in fragmentedIndexes.Rows)
                {
                    string tableName = row["TableName"].ToString();
                    string indexName = row["IndexName"].ToString();
                    double fragmentation = Convert.ToDouble(row["FragmentationPercent"]);
                    string recommendation = row["Recommendation"].ToString();

                    try
                    {
                        if (recommendation == "REBUILD")
                        {
                            RebuildIndex(tableName, indexName);
                            rebuiltCount++;
                        }
                        else if (recommendation == "REORGANIZE")
                        {
                            ReorganizeIndex(tableName, indexName);
                            rebuiltCount++;
                        }

                        Logger.Info($"{recommendation} completed for {indexName} on {tableName} (Fragmentation: {fragmentation:F2}%)");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error processing index {indexName} on {tableName}", ex);
                        // Continue with next index
                    }
                }

                Logger.Info($"Index maintenance completed. Processed {rebuiltCount} indexes.");
                return rebuiltCount;
            }
            catch (Exception ex)
            {
                Logger.Error("Error rebuilding fragmented indexes", ex);
                throw;
            }
        }

        /// <summary>
        /// Update statistics for a table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>True if successful</returns>
        public bool UpdateStatistics(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $"UPDATE STATISTICS {tableName} WITH FULLSCAN";
                    
                    connection.Execute(sql);
                    
                    Logger.Info($"Updated statistics for table: {tableName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error updating statistics for: {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Get index size and space usage
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>DataTable with index size information</returns>
        public DataTable GetIndexSizeInfo(string tableName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                        SELECT 
                            i.name AS IndexName,
                            i.type_desc AS IndexType,
                            SUM(ps.used_page_count) * 8 / 1024.0 AS SizeMB,
                            SUM(ps.row_count) AS RowCount
                        FROM sys.indexes i
                        INNER JOIN sys.dm_db_partition_stats ps 
                            ON i.object_id = ps.object_id 
                            AND i.index_id = ps.index_id
                        WHERE i.object_id = OBJECT_ID(@TableName)
                        AND i.name IS NOT NULL
                        GROUP BY i.name, i.type_desc
                        ORDER BY SizeMB DESC";

                    using (var adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@TableName", tableName);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error getting index size info for: {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Disable an index
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="indexName">Index name</param>
        /// <returns>True if successful</returns>
        public bool DisableIndex(string tableName, string indexName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    string sql = $"ALTER INDEX {indexName} ON {tableName} DISABLE";
                    
                    connection.Execute(sql);
                    
                    Logger.Warning($"Disabled index: {indexName} on {tableName}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error disabling index: {indexName} on {tableName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Enable a disabled index by rebuilding it
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="indexName">Index name</param>
        /// <returns>True if successful</returns>
        public bool EnableIndex(string tableName, string indexName)
        {
            try
            {
                // Rebuilding a disabled index enables it
                return RebuildIndex(tableName, indexName);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error enabling index: {indexName} on {tableName}", ex);
                throw;
            }
        }
    }
}
