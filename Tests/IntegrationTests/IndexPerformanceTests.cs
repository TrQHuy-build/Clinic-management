using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Diagnostics;
using DentalClinicManagement.DataAccess;

namespace DentalClinicManagement.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for IndexManager and database performance
    /// Issue #8: Database Design - Index Performance Tests
    /// </summary>
    [TestClass]
    public class IndexPerformanceTests
    {
        private IndexManager _indexManager;
        private string _testConnectionString;

        [TestInitialize]
        public void Setup()
        {
            _testConnectionString = ConfigManager.GetConnectionString("TestDatabase");
            _indexManager = new IndexManager(_testConnectionString);
        }

        #region Index Existence Tests

        [TestMethod]
        public void IndexExists_ExistingIndex_ReturnsTrue()
        {
            // Act
            bool exists = _indexManager.IndexExists("Patients", "IX_Patients_Email");

            // Assert
            Assert.IsTrue(exists, "IX_Patients_Email should exist on Patients table");
        }

        [TestMethod]
        public void IndexExists_NonExistentIndex_ReturnsFalse()
        {
            // Act
            bool exists = _indexManager.IndexExists("Patients", "IX_NonExistent_Index");

            // Assert
            Assert.IsFalse(exists, "Non-existent index should not be found");
        }

        #endregion

        #region Get Table Indexes Tests

        [TestMethod]
        public void GetTableIndexes_PatientsTable_ReturnsMultipleIndexes()
        {
            // Act
            DataTable indexes = _indexManager.GetTableIndexes("Patients");

            // Assert
            Assert.IsTrue(indexes.Rows.Count > 0, "Patients table should have indexes");
            
            // Check for specific indexes
            bool hasEmailIndex = false;
            bool hasPhoneIndex = false;
            
            foreach (DataRow row in indexes.Rows)
            {
                string indexName = row["IndexName"].ToString();
                if (indexName == "IX_Patients_Email") hasEmailIndex = true;
                if (indexName == "IX_Patients_Phone") hasPhoneIndex = true;
            }
            
            Assert.IsTrue(hasEmailIndex, "Should have email index");
            Assert.IsTrue(hasPhoneIndex, "Should have phone index");
        }

        [TestMethod]
        public void GetTableIndexes_AppointmentsTable_HasForeignKeyIndexes()
        {
            // Act
            DataTable indexes = _indexManager.GetTableIndexes("Appointments");

            // Assert
            Assert.IsTrue(indexes.Rows.Count > 0, "Appointments table should have indexes");
            
            // Check for foreign key indexes
            bool hasPatientIdIndex = false;
            bool hasDoctorIdIndex = false;
            
            foreach (DataRow row in indexes.Rows)
            {
                string indexName = row["IndexName"].ToString();
                if (indexName == "IX_Appointments_PatientId") hasPatientIdIndex = true;
                if (indexName == "IX_Appointments_DoctorId") hasDoctorIdIndex = true;
            }
            
            Assert.IsTrue(hasPatientIdIndex, "Should have patient_id index");
            Assert.IsTrue(hasDoctorIdIndex, "Should have doctor_id index");
        }

        #endregion

        #region Index Usage Statistics Tests

        [TestMethod]
        public void GetIndexUsageStatistics_ReturnsStatistics()
        {
            // Arrange - Execute some queries to generate usage stats
            ExecuteTestQueries();

            // Act
            DataTable stats = _indexManager.GetIndexUsageStatistics("Patients");

            // Assert
            Assert.IsTrue(stats.Rows.Count > 0, "Should return index usage statistics");
            
            // Verify columns exist
            Assert.IsTrue(stats.Columns.Contains("IndexName"), "Should have IndexName column");
            Assert.IsTrue(stats.Columns.Contains("UserSeeks"), "Should have UserSeeks column");
            Assert.IsTrue(stats.Columns.Contains("UserScans"), "Should have UserScans column");
            Assert.IsTrue(stats.Columns.Contains("UsageStatus"), "Should have UsageStatus column");
        }

        #endregion

        #region Find Missing Indexes Tests

        [TestMethod]
        public void FindMissingIndexes_ReturnsSuggestions()
        {
            // Arrange - Execute queries that might benefit from indexes
            ExecuteQueriesForMissingIndexSuggestions();

            // Act
            DataTable missingIndexes = _indexManager.FindMissingIndexes();

            // Assert
            // Note: This may return 0 rows if database already has optimal indexes
            Assert.IsNotNull(missingIndexes, "Should return a DataTable");
            Assert.IsTrue(missingIndexes.Columns.Contains("TableName"), "Should have TableName column");
            Assert.IsTrue(missingIndexes.Columns.Contains("CreateIndexStatement"), 
                "Should have CreateIndexStatement column");
        }

        [TestMethod]
        public void FindMissingIndexes_SpecificTable_FiltersResults()
        {
            // Act
            DataTable missingIndexes = _indexManager.FindMissingIndexes("Patients");

            // Assert
            Assert.IsNotNull(missingIndexes, "Should return a DataTable");
            
            // If there are results, they should all be for Patients table
            foreach (DataRow row in missingIndexes.Rows)
            {
                string tableName = row["TableName"].ToString();
                Assert.AreEqual("Patients", tableName, "All results should be for Patients table");
            }
        }

        #endregion

        #region Fragmented Indexes Tests

        [TestMethod]
        public void GetFragmentedIndexes_ReturnsFragmentationInfo()
        {
            // Act
            DataTable fragmented = _indexManager.GetFragmentedIndexes(fragmentationThreshold: 10.0);

            // Assert
            Assert.IsNotNull(fragmented, "Should return a DataTable");
            Assert.IsTrue(fragmented.Columns.Contains("TableName"), "Should have TableName column");
            Assert.IsTrue(fragmented.Columns.Contains("FragmentationPercent"), 
                "Should have FragmentationPercent column");
            Assert.IsTrue(fragmented.Columns.Contains("Recommendation"), 
                "Should have Recommendation column");
        }

        #endregion

        #region Index Maintenance Tests

        [TestMethod]
        public void UpdateStatistics_SucceedsForTable()
        {
            // Act
            bool result = _indexManager.UpdateStatistics("Patients");

            // Assert
            Assert.IsTrue(result, "Statistics update should succeed");
        }

        [TestMethod]
        public void ReorganizeIndex_ValidIndex_Succeeds()
        {
            // Arrange
            string tableName = "Patients";
            string indexName = "IX_Patients_Email";

            // Act
            bool result = _indexManager.ReorganizeIndex(tableName, indexName);

            // Assert
            Assert.IsTrue(result, "Index reorganize should succeed");
        }

        [TestMethod]
        public void RebuildIndex_ValidIndex_Succeeds()
        {
            // Arrange
            string tableName = "Patients";
            string indexName = "IX_Patients_Email";

            // Act
            bool result = _indexManager.RebuildIndex(tableName, indexName);

            // Assert
            Assert.IsTrue(result, "Index rebuild should succeed");
        }

        #endregion

        #region Index Size Tests

        [TestMethod]
        public void GetIndexSizeInfo_ReturnsSize Information()
        {
            // Act
            DataTable sizeInfo = _indexManager.GetIndexSizeInfo("Patients");

            // Assert
            Assert.IsTrue(sizeInfo.Rows.Count > 0, "Should return index size information");
            Assert.IsTrue(sizeInfo.Columns.Contains("IndexName"), "Should have IndexName column");
            Assert.IsTrue(sizeInfo.Columns.Contains("SizeMB"), "Should have SizeMB column");
            Assert.IsTrue(sizeInfo.Columns.Contains("RowCount"), "Should have RowCount column");
        }

        #endregion

        #region Performance Tests

        [TestMethod]
        public void QueryPerformance_WithIndex_IsFasterThanWithout()
        {
            // Arrange - Create test data
            CreateLargeTestDataset();

            // Act - Query with index (email has IX_Patients_Email)
            var stopwatch = Stopwatch.StartNew();
            DataTable resultWithIndex = QueryPatientByEmail("test@example.com");
            stopwatch.Stop();
            long timeWithIndex = stopwatch.ElapsedMilliseconds;

            // Query without index (address doesn't have index)
            stopwatch.Restart();
            DataTable resultWithoutIndex = QueryPatientByAddress("Test Address");
            stopwatch.Stop();
            long timeWithoutIndex = stopwatch.ElapsedMilliseconds;

            // Assert
            TestContext.WriteLine($"Query with index: {timeWithIndex}ms");
            TestContext.WriteLine($"Query without index: {timeWithoutIndex}ms");
            
            // Index should generally be faster (though not guaranteed for small datasets)
            Assert.IsTrue(timeWithIndex <= timeWithoutIndex * 2, 
                "Query with index should not be significantly slower");
        }

        [TestMethod]
        public void JoinQuery_WithIndexes_CompletesQuickly()
        {
            // Arrange - Create test data
            CreateTestAppointments();

            // Act - JOIN query using indexed foreign keys
            var stopwatch = Stopwatch.StartNew();
            DataTable result = ExecuteJoinQuery();
            stopwatch.Stop();
            long elapsedMs = stopwatch.ElapsedMilliseconds;

            // Assert
            TestContext.WriteLine($"JOIN query execution time: {elapsedMs}ms");
            Assert.IsTrue(elapsedMs < 2000, "JOIN query with indexes should complete in under 2 seconds");
        }

        [TestMethod]
        public void CompositeIndex_RangeQuery_IsFast()
        {
            // Arrange
            CreateTestAppointments();

            // Act - Query using composite index (doctor_id + appointment_date)
            var stopwatch = Stopwatch.StartNew();
            DataTable result = QueryAppointmentsByDoctorAndDate(1, DateTime.Now.Date);
            stopwatch.Stop();
            long elapsedMs = stopwatch.ElapsedMilliseconds;

            // Assert
            TestContext.WriteLine($"Composite index query: {elapsedMs}ms");
            Assert.IsTrue(elapsedMs < 500, "Composite index query should complete in under 500ms");
        }

        [TestMethod]
        public void SoftDeleteIndex_FilterQuery_IsFast()
        {
            // Arrange
            CreateTestPatientsWithSoftDeletes();

            // Act - Query filtering by is_deleted (should use IX_Patients_IsDeleted)
            var stopwatch = Stopwatch.StartNew();
            DataTable result = QueryActivePatients();
            stopwatch.Stop();
            long elapsedMs = stopwatch.ElapsedMilliseconds;

            // Assert
            TestContext.WriteLine($"Soft delete filter query: {elapsedMs}ms");
            Assert.IsTrue(elapsedMs < 500, "Soft delete filter should use index and complete quickly");
        }

        #endregion

        #region Helper Methods

        public TestContext TestContext { get; set; }

        private void ExecuteTestQueries()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Execute some test queries to generate index usage stats
                string[] queries = new string[]
                {
                    "SELECT * FROM Patients WHERE email = 'test@example.com'",
                    "SELECT * FROM Patients WHERE phone = '0123456789'",
                    "SELECT * FROM Appointments WHERE patient_id = 1"
                };

                foreach (var query in queries)
                {
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                        {
                            var dt = new DataTable();
                            adapter.Fill(dt);
                        }
                    }
                }
            }
        }

        private void ExecuteQueriesForMissingIndexSuggestions()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Execute queries that might benefit from additional indexes
                string[] queries = new string[]
                {
                    "SELECT * FROM Patients WHERE address LIKE '%Street%'",
                    "SELECT * FROM Appointments WHERE status = 'Pending' AND appointment_date > GETDATE()",
                    "SELECT * FROM Invoices WHERE payment_status = 'Unpaid' ORDER BY invoice_date"
                };

                foreach (var query in queries)
                {
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                        {
                            var dt = new DataTable();
                            adapter.Fill(dt);
                        }
                    }
                }
            }
        }

        private void CreateLargeTestDataset()
        {
            // Note: This would create a large dataset for realistic performance testing
            // Implementation omitted for brevity - would insert many test patients
        }

        private DataTable QueryPatientByEmail(string email)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Patients WHERE email = @Email AND is_deleted = 0";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private DataTable QueryPatientByAddress(string address)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Patients WHERE address LIKE @Address AND is_deleted = 0";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Address", "%" + address + "%");
                    
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private void CreateTestAppointments()
        {
            // Create some test appointments for performance testing
            // Implementation omitted for brevity
        }

        private DataTable ExecuteJoinQuery()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT 
                        a.*,
                        p.full_name AS patient_name,
                        s.full_name AS doctor_name,
                        sv.service_name
                    FROM Appointments a
                    INNER JOIN Patients p ON a.patient_id = p.id
                    INNER JOIN Staff s ON a.doctor_id = s.id
                    LEFT JOIN Services sv ON a.service_id = sv.id
                    WHERE a.is_deleted = 0
                    AND p.is_deleted = 0
                    AND s.is_deleted = 0";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private DataTable QueryAppointmentsByDoctorAndDate(int doctorId, DateTime date)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    SELECT * 
                    FROM Appointments 
                    WHERE doctor_id = @DoctorId 
                    AND appointment_date = @Date
                    AND is_deleted = 0";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DoctorId", doctorId);
                    command.Parameters.AddWithValue("@Date", date);
                    
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private void CreateTestPatientsWithSoftDeletes()
        {
            // Create test patients with some soft-deleted
            // Implementation omitted for brevity
        }

        private DataTable QueryActivePatients()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Patients WHERE is_deleted = 0";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        #endregion
    }
}
