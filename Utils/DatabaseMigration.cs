using System;
using System.Data.SqlClient;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Database migration helper - runs migrations automatically
    /// </summary>
    public static class DatabaseMigration
    {
        /// <summary>
        /// Run all pending migrations
        /// </summary>
        public static void RunMigrations()
        {
            try
            {
                Logger.LogAction("MIGRATION_START", "Starting database migrations...");

                // Migration 1: Add reject_reason column
                AddRejectReasonColumn();

                Logger.LogAction("MIGRATION_COMPLETE", "All migrations completed successfully");
            }
            catch (Exception ex)
            {
                Logger.LogAction("MIGRATION_ERROR", $"Migration failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Migration: Add reject_reason column to Appointment table
        /// </summary>
        private static void AddRejectReasonColumn()
        {
            try
            {
                // Check if column already exists
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Appointment' 
                    AND COLUMN_NAME = 'reject_reason'";

                object result = DatabaseHelper.ExecuteScalar(checkQuery, null);
                int columnCount = Convert.ToInt32(result ?? 0);

                if (columnCount > 0)
                {
                    Logger.LogAction("MIGRATION_SKIP", "Column reject_reason already exists - skipping");
                    return;
                }

                // Add column
                string migrationQuery = @"
                    ALTER TABLE Appointment
                    ADD reject_reason NVARCHAR(500) NULL";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(migrationQuery, null);

                Logger.LogAction("MIGRATION_SUCCESS", "Added column reject_reason to Appointment table");
            }
            catch (SqlException sqlEx)
            {
                // Column might already exist if migration was run manually
                if (sqlEx.Message.Contains("already") || sqlEx.Message.Contains("exists"))
                {
                    Logger.LogAction("MIGRATION_SKIP", "Column reject_reason already exists");
                }
                else
                {
                    Logger.LogAction("MIGRATION_ERROR", $"Failed to add reject_reason column: {sqlEx.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.LogAction("MIGRATION_ERROR", $"Unexpected error in AddRejectReasonColumn: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Check if a specific migration has been applied
        /// </summary>
        public static bool IsMigrationApplied(string migrationName)
        {
            try
            {
                // Create MigrationHistory table if it doesn't exist
                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MigrationHistory]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE MigrationHistory (
                            migration_id INT IDENTITY(1,1) PRIMARY KEY,
                            migration_name NVARCHAR(255) NOT NULL UNIQUE,
                            applied_at DATETIME DEFAULT GETDATE()
                        )
                    END";

                DatabaseHelper.ExecuteNonQuery(createTableQuery, null);

                // Check if migration exists
                string checkQuery = "SELECT COUNT(*) FROM MigrationHistory WHERE migration_name = @name";
                object result = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter[] {
                    new SqlParameter("@name", migrationName)
                });

                return Convert.ToInt32(result ?? 0) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Mark a migration as applied
        /// </summary>
        public static void MarkMigrationApplied(string migrationName)
        {
            try
            {
                string insertQuery = "INSERT INTO MigrationHistory (migration_name) VALUES (@name)";
                DatabaseHelper.ExecuteNonQuery(insertQuery, new SqlParameter[] {
                    new SqlParameter("@name", migrationName)
                });
            }
            catch (Exception ex)
            {
                Logger.LogAction("MIGRATION_WARNING", $"Failed to mark migration {migrationName}: {ex.Message}");
            }
        }
    }
}
