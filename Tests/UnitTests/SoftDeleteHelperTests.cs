using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Models;

namespace DentalClinicManagement.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for SoftDeleteHelper
    /// Issue #8: Database Design - Soft Delete Tests
    /// </summary>
    [TestClass]
    public class SoftDeleteHelperTests
    {
        private SoftDeleteHelper _softDeleteHelper;
        private string _testConnectionString;

        [TestInitialize]
        public void Setup()
        {
            _testConnectionString = ConfigManager.GetConnectionString("TestDatabase");
            _softDeleteHelper = new SoftDeleteHelper(_testConnectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test data
            CleanupTestData();
        }

        #region Soft Delete Tests

        [TestMethod]
        public void SoftDelete_ValidRecord_SetsIsDeletedToTrue()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);

            // Act
            bool result = _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Assert
            Assert.IsTrue(result, "Soft delete should succeed");
            
            bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", patientId);
            Assert.IsTrue(isDeleted.HasValue, "Patient should exist");
            Assert.IsTrue(isDeleted.Value, "Patient should be marked as deleted");
        }

        [TestMethod]
        public void SoftDelete_AlreadyDeletedRecord_ReturnsFalse()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Act - try to delete again
            bool result = _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Assert
            Assert.IsFalse(result, "Should not be able to soft delete an already deleted record");
        }

        [TestMethod]
        public void SoftDelete_NonExistentRecord_ReturnsFalse()
        {
            // Arrange
            int nonExistentId = 999999;

            // Act
            bool result = _softDeleteHelper.SoftDelete("Patients", nonExistentId, "TestUser");

            // Assert
            Assert.IsFalse(result, "Should not be able to soft delete a non-existent record");
        }

        [TestMethod]
        public void SoftDelete_SetsDeletedAtTimestamp()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            DateTime beforeDelete = DateTime.Now.AddSeconds(-1);

            // Act
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");
            DateTime afterDelete = DateTime.Now.AddSeconds(1);

            // Assert
            var deletedRecord = GetPatientById(patientId);
            Assert.IsNotNull(deletedRecord["deleted_at"], "deleted_at should be set");
            DateTime deletedAt = Convert.ToDateTime(deletedRecord["deleted_at"]);
            Assert.IsTrue(deletedAt >= beforeDelete && deletedAt <= afterDelete, 
                "deleted_at should be within expected time range");
        }

        [TestMethod]
        public void SoftDelete_SetsDeletedByUser()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            string testUser = "TestUser123";

            // Act
            _softDeleteHelper.SoftDelete("Patients", patientId, testUser);

            // Assert
            var deletedRecord = GetPatientById(patientId);
            Assert.AreEqual(testUser, deletedRecord["deleted_by"].ToString(), 
                "deleted_by should match the user who deleted the record");
        }

        #endregion

        #region Batch Delete Tests

        [TestMethod]
        public void SoftDeleteBatch_MultipleRecords_DeletesAll()
        {
            // Arrange
            int[] patientIds = new int[3];
            for (int i = 0; i < 3; i++)
            {
                var patient = CreateTestPatient($"BatchTest{i}");
                patientIds[i] = InsertTestPatient(patient);
            }

            // Act
            int deletedCount = _softDeleteHelper.SoftDeleteBatch("Patients", patientIds, "TestUser");

            // Assert
            Assert.AreEqual(3, deletedCount, "Should delete all 3 records");
            
            foreach (int id in patientIds)
            {
                bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", id);
                Assert.IsTrue(isDeleted.Value, $"Patient {id} should be marked as deleted");
            }
        }

        [TestMethod]
        public void SoftDeleteBatch_EmptyArray_ReturnsZero()
        {
            // Arrange
            int[] emptyArray = new int[0];

            // Act
            int deletedCount = _softDeleteHelper.SoftDeleteBatch("Patients", emptyArray, "TestUser");

            // Assert
            Assert.AreEqual(0, deletedCount, "Should return 0 for empty array");
        }

        [TestMethod]
        public void SoftDeleteBatch_NullArray_ReturnsZero()
        {
            // Act
            int deletedCount = _softDeleteHelper.SoftDeleteBatch("Patients", null, "TestUser");

            // Assert
            Assert.AreEqual(0, deletedCount, "Should return 0 for null array");
        }

        #endregion

        #region Restore Tests

        [TestMethod]
        public void Restore_DeletedRecord_RestoresSuccessfully()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Act
            bool result = _softDeleteHelper.Restore("Patients", patientId);

            // Assert
            Assert.IsTrue(result, "Restore should succeed");
            
            bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", patientId);
            Assert.IsTrue(isDeleted.HasValue, "Patient should exist");
            Assert.IsFalse(isDeleted.Value, "Patient should be active (not deleted)");
        }

        [TestMethod]
        public void Restore_ClearsDeletedAtAndDeletedBy()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Act
            _softDeleteHelper.Restore("Patients", patientId);

            // Assert
            var restoredRecord = GetPatientById(patientId);
            Assert.IsTrue(restoredRecord["deleted_at"] == DBNull.Value, "deleted_at should be NULL");
            Assert.IsTrue(restoredRecord["deleted_by"] == DBNull.Value, "deleted_by should be NULL");
        }

        [TestMethod]
        public void Restore_ActiveRecord_ReturnsFalse()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);

            // Act - try to restore an active record
            bool result = _softDeleteHelper.Restore("Patients", patientId);

            // Assert
            Assert.IsFalse(result, "Should not be able to restore an active record");
        }

        [TestMethod]
        public void RestoreBatch_MultipleRecords_RestoresAll()
        {
            // Arrange
            int[] patientIds = new int[3];
            for (int i = 0; i < 3; i++)
            {
                var patient = CreateTestPatient($"RestoreTest{i}");
                patientIds[i] = InsertTestPatient(patient);
                _softDeleteHelper.SoftDelete("Patients", patientIds[i], "TestUser");
            }

            // Act
            int restoredCount = _softDeleteHelper.RestoreBatch("Patients", patientIds);

            // Assert
            Assert.AreEqual(3, restoredCount, "Should restore all 3 records");
            
            foreach (int id in patientIds)
            {
                bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", id);
                Assert.IsFalse(isDeleted.Value, $"Patient {id} should be active");
            }
        }

        #endregion

        #region Hard Delete Tests

        [TestMethod]
        public void HardDelete_SoftDeletedRecord_DeletesPermanently()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Act
            bool result = _softDeleteHelper.HardDelete("Patients", patientId);

            // Assert
            Assert.IsTrue(result, "Hard delete should succeed");
            
            bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", patientId);
            Assert.IsFalse(isDeleted.HasValue, "Patient should not exist anymore");
        }

        [TestMethod]
        public void HardDelete_ActiveRecord_ReturnsFalse()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);

            // Act - try to hard delete an active record (should fail)
            bool result = _softDeleteHelper.HardDelete("Patients", patientId);

            // Assert
            Assert.IsFalse(result, "Should not be able to hard delete an active record");
        }

        #endregion

        #region Purge Tests

        [TestMethod]
        public void PurgeOldRecords_OldDeletedRecords_PurgesSuccessfully()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");
            
            // Manually update deleted_at to 100 days ago
            UpdateDeletedAt(patientId, DateTime.Now.AddDays(-100));

            // Act
            int purgedCount = _softDeleteHelper.PurgeOldRecords("Patients", daysOld: 90);

            // Assert
            Assert.AreEqual(1, purgedCount, "Should purge 1 old record");
            
            bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", patientId);
            Assert.IsFalse(isDeleted.HasValue, "Patient should be permanently deleted");
        }

        [TestMethod]
        public void PurgeOldRecords_RecentDeletedRecords_DoesNotPurge()
        {
            // Arrange
            var patient = CreateTestPatient();
            int patientId = InsertTestPatient(patient);
            _softDeleteHelper.SoftDelete("Patients", patientId, "TestUser");

            // Act - purge records older than 90 days (this record is recent)
            int purgedCount = _softDeleteHelper.PurgeOldRecords("Patients", daysOld: 90);

            // Assert
            Assert.AreEqual(0, purgedCount, "Should not purge recent records");
            
            bool? isDeleted = _softDeleteHelper.IsDeleted("Patients", patientId);
            Assert.IsTrue(isDeleted.HasValue, "Patient should still exist");
            Assert.IsTrue(isDeleted.Value, "Patient should still be marked as deleted");
        }

        #endregion

        #region Statistics Tests

        [TestMethod]
        public void GetTableStatistics_ReturnsCorrectCounts()
        {
            // Arrange
            int activeCount = 3;
            int deletedCount = 2;
            
            // Create active records
            for (int i = 0; i < activeCount; i++)
            {
                var patient = CreateTestPatient($"Active{i}");
                InsertTestPatient(patient);
            }
            
            // Create deleted records
            for (int i = 0; i < deletedCount; i++)
            {
                var patient = CreateTestPatient($"Deleted{i}");
                int id = InsertTestPatient(patient);
                _softDeleteHelper.SoftDelete("Patients", id, "TestUser");
            }

            // Act
            var stats = _softDeleteHelper.GetTableStatistics("Patients");

            // Assert
            Assert.AreEqual(activeCount + deletedCount, stats.Total, "Total count should be correct");
            Assert.AreEqual(activeCount, stats.Active, "Active count should be correct");
            Assert.AreEqual(deletedCount, stats.Deleted, "Deleted count should be correct");
            Assert.AreEqual(60.0, stats.ActivePercentage, 0.1, "Active percentage should be 60%");
        }

        [TestMethod]
        public void GetActiveCount_ReturnsOnlyActiveRecords()
        {
            // Arrange
            int activeCount = 5;
            for (int i = 0; i < activeCount; i++)
            {
                var patient = CreateTestPatient($"Active{i}");
                InsertTestPatient(patient);
            }
            
            // Create some deleted records (should not be counted)
            for (int i = 0; i < 3; i++)
            {
                var patient = CreateTestPatient($"Deleted{i}");
                int id = InsertTestPatient(patient);
                _softDeleteHelper.SoftDelete("Patients", id, "TestUser");
            }

            // Act
            int count = _softDeleteHelper.GetActiveCount("Patients");

            // Assert
            Assert.AreEqual(activeCount, count, "Should only count active records");
        }

        [TestMethod]
        public void GetDeletedCount_ReturnsOnlyDeletedRecords()
        {
            // Arrange
            int deletedCount = 4;
            
            // Create active records (should not be counted)
            for (int i = 0; i < 3; i++)
            {
                var patient = CreateTestPatient($"Active{i}");
                InsertTestPatient(patient);
            }
            
            // Create deleted records
            for (int i = 0; i < deletedCount; i++)
            {
                var patient = CreateTestPatient($"Deleted{i}");
                int id = InsertTestPatient(patient);
                _softDeleteHelper.SoftDelete("Patients", id, "TestUser");
            }

            // Act
            int count = _softDeleteHelper.GetDeletedCount("Patients");

            // Assert
            Assert.AreEqual(deletedCount, count, "Should only count deleted records");
        }

        [TestMethod]
        public void GetDeletedRecords_ReturnsAllDeletedRecords()
        {
            // Arrange
            int deletedCount = 3;
            for (int i = 0; i < deletedCount; i++)
            {
                var patient = CreateTestPatient($"Deleted{i}");
                int id = InsertTestPatient(patient);
                _softDeleteHelper.SoftDelete("Patients", id, $"User{i}");
            }

            // Act
            DataTable deletedRecords = _softDeleteHelper.GetDeletedRecords("Patients");

            // Assert
            Assert.AreEqual(deletedCount, deletedRecords.Rows.Count, 
                "Should return all deleted records");
            
            // Verify all returned records have is_deleted = 1
            foreach (DataRow row in deletedRecords.Rows)
            {
                Assert.IsTrue(Convert.ToBoolean(row["is_deleted"]), 
                    "All returned records should be deleted");
            }
        }

        #endregion

        #region Support Detection Tests

        [TestMethod]
        public void SupportsSoftDelete_TableWithIsDeletedColumn_ReturnsTrue()
        {
            // Act
            bool supports = _softDeleteHelper.SupportsSoftDelete("Patients");

            // Assert
            Assert.IsTrue(supports, "Patients table should support soft delete");
        }

        [TestMethod]
        public void SupportsSoftDelete_TableWithoutIsDeletedColumn_ReturnsFalse()
        {
            // Note: This test assumes there's a table without is_deleted column
            // If all tables have been migrated, this test may need adjustment
            
            // Act
            bool supports = _softDeleteHelper.SupportsSoftDelete("NonExistentTable");

            // Assert
            Assert.IsFalse(supports, "Non-existent table should not support soft delete");
        }

        #endregion

        #region Helper Methods

        private Patient CreateTestPatient(string namePrefix = "Test")
        {
            return new Patient
            {
                full_name = $"{namePrefix} Patient {Guid.NewGuid().ToString().Substring(0, 8)}",
                email = $"test{Guid.NewGuid().ToString().Substring(0, 8)}@test.com",
                phone = "0123456789",
                date_of_birth = new DateTime(1990, 1, 1),
                gender = "Male",
                address = "Test Address"
            };
        }

        private int InsertTestPatient(Patient patient)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Patients (full_name, email, phone, date_of_birth, gender, address, is_deleted)
                    VALUES (@FullName, @Email, @Phone, @DateOfBirth, @Gender, @Address, 0);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FullName", patient.full_name);
                    command.Parameters.AddWithValue("@Email", patient.email);
                    command.Parameters.AddWithValue("@Phone", patient.phone);
                    command.Parameters.AddWithValue("@DateOfBirth", patient.date_of_birth);
                    command.Parameters.AddWithValue("@Gender", patient.gender);
                    command.Parameters.AddWithValue("@Address", patient.address);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        private DataRow GetPatientById(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Patients WHERE id = @Id";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    
                    using (var adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        
                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        
                        return null;
                    }
                }
            }
        }

        private void UpdateDeletedAt(int patientId, DateTime deletedAt)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "UPDATE Patients SET deleted_at = @DeletedAt WHERE id = @Id";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", patientId);
                    command.Parameters.AddWithValue("@DeletedAt", deletedAt);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void CleanupTestData()
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                string sql = "DELETE FROM Patients WHERE email LIKE '%@test.com'";
                using (var command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
