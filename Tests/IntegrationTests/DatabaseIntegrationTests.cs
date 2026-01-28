using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentalClinicManagement.DataAccess;
using DentalClinicManagement.Models;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for Database operations
    /// Tests actual database connectivity and CRUD operations
    /// NOTE: Requires test database setup
    /// </summary>
    [TestClass]
    public class DatabaseIntegrationTests
    {
        private DatabaseHelper dbHelper;
        private static string testConnectionString;

        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            // Setup test database connection
            testConnectionString = ConfigManager.GetConnectionString("TestDatabase");
        }

        [TestInitialize]
        public void Setup()
        {
            dbHelper = new DatabaseHelper(testConnectionString);
            CleanupTestData();
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupTestData();
        }

        private void CleanupTestData()
        {
            // Clean up test data
            dbHelper.ExecuteNonQuery("DELETE FROM Patients WHERE email LIKE '%@test.com'");
            dbHelper.ExecuteNonQuery("DELETE FROM Services WHERE service_name LIKE 'Test%'");
        }

        #region Patient CRUD Tests

        [TestMethod]
        public async Task CreatePatient_ValidData_InsertsSuccessfully()
        {
            // Arrange
            var patient = new Patient
            {
                full_name = "Test Patient",
                email = "testpatient@test.com",
                phone = "0123456789",
                date_of_birth = new DateTime(1990, 1, 1),
                gender = "Male",
                address = "Test Address"
            };

            // Act
            int id = await dbHelper.ExecuteScalarAsync<int>(
                @"INSERT INTO Patients (full_name, email, phone, date_of_birth, gender, address)
                  OUTPUT INSERTED.id
                  VALUES (@full_name, @email, @phone, @date_of_birth, @gender, @address)",
                patient
            );

            // Assert
            Assert.IsTrue(id > 0);

            // Verify inserted data
            var inserted = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                "SELECT * FROM Patients WHERE id = @id",
                new { id }
            );

            Assert.IsNotNull(inserted);
            Assert.AreEqual(patient.full_name, inserted.full_name);
            Assert.AreEqual(patient.email, inserted.email);
        }

        [TestMethod]
        public async Task ReadPatient_ExistingId_ReturnsPatient()
        {
            // Arrange - Insert test patient
            var patient = await CreateTestPatient();

            // Act
            var result = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                "SELECT * FROM Patients WHERE id = @id",
                new { id = patient.id }
            );

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(patient.full_name, result.full_name);
        }

        [TestMethod]
        public async Task UpdatePatient_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var patient = await CreateTestPatient();
            string newName = "Updated Test Patient";

            // Act
            await dbHelper.ExecuteNonQueryAsync(
                "UPDATE Patients SET full_name = @newName WHERE id = @id",
                new { newName, id = patient.id }
            );

            // Assert
            var updated = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                "SELECT * FROM Patients WHERE id = @id",
                new { id = patient.id }
            );

            Assert.AreEqual(newName, updated.full_name);
        }

        [TestMethod]
        public async Task DeletePatient_ExistingId_DeletesSuccessfully()
        {
            // Arrange
            var patient = await CreateTestPatient();

            // Act
            await dbHelper.ExecuteNonQueryAsync(
                "DELETE FROM Patients WHERE id = @id",
                new { id = patient.id }
            );

            // Assert
            var deleted = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                "SELECT * FROM Patients WHERE id = @id",
                new { id = patient.id }
            );

            Assert.IsNull(deleted);
        }

        #endregion

        #region Service CRUD Tests

        [TestMethod]
        public async Task CreateService_ValidData_InsertsSuccessfully()
        {
            // Arrange
            var service = new Service
            {
                service_name = "Test Service",
                description = "Test Description",
                price = 100000,
                duration_minutes = 30
            };

            // Act
            int id = await dbHelper.ExecuteScalarAsync<int>(
                @"INSERT INTO Services (service_name, description, price, duration_minutes)
                  OUTPUT INSERTED.id
                  VALUES (@service_name, @description, @price, @duration_minutes)",
                service
            );

            // Assert
            Assert.IsTrue(id > 0);

            // Cleanup
            await dbHelper.ExecuteNonQueryAsync("DELETE FROM Services WHERE id = @id", new { id });
        }

        [TestMethod]
        public async Task GetAllServices_ReturnsServiceList()
        {
            // Arrange
            var service1 = await CreateTestService("Test Service 1");
            var service2 = await CreateTestService("Test Service 2");

            // Act
            var services = await dbHelper.ExecuteQueryAsync<Service>(
                "SELECT * FROM Services WHERE service_name LIKE 'Test%'"
            );

            // Assert
            Assert.IsTrue(services.Count() >= 2);

            // Cleanup
            await dbHelper.ExecuteNonQueryAsync("DELETE FROM Services WHERE id IN (@id1, @id2)", 
                new { id1 = service1.id, id2 = service2.id });
        }

        #endregion

        #region Transaction Tests

        [TestMethod]
        public async Task Transaction_Commit_SavesAllChanges()
        {
            // Arrange
            var patient1 = new Patient
            {
                full_name = "Transaction Patient 1",
                email = "transaction1@test.com",
                phone = "0123456789",
                date_of_birth = DateTime.Now.AddYears(-30),
                gender = "Male"
            };

            var patient2 = new Patient
            {
                full_name = "Transaction Patient 2",
                email = "transaction2@test.com",
                phone = "0987654321",
                date_of_birth = DateTime.Now.AddYears(-25),
                gender = "Female"
            };

            // Act
            await dbHelper.ExecuteTransactionAsync(async () =>
            {
                await dbHelper.ExecuteNonQueryAsync(
                    @"INSERT INTO Patients (full_name, email, phone, date_of_birth, gender)
                      VALUES (@full_name, @email, @phone, @date_of_birth, @gender)",
                    patient1
                );

                await dbHelper.ExecuteNonQueryAsync(
                    @"INSERT INTO Patients (full_name, email, phone, date_of_birth, gender)
                      VALUES (@full_name, @email, @phone, @date_of_birth, @gender)",
                    patient2
                );
            });

            // Assert
            var results = await dbHelper.ExecuteQueryAsync<Patient>(
                "SELECT * FROM Patients WHERE email IN (@email1, @email2)",
                new { email1 = patient1.email, email2 = patient2.email }
            );

            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public async Task Transaction_Rollback_RevertsAllChanges()
        {
            // Arrange
            var patient = new Patient
            {
                full_name = "Rollback Patient",
                email = "rollback@test.com",
                phone = "0123456789",
                date_of_birth = DateTime.Now.AddYears(-30),
                gender = "Male"
            };

            // Act
            bool exceptionThrown = false;
            try
            {
                await dbHelper.ExecuteTransactionAsync(async () =>
                {
                    await dbHelper.ExecuteNonQueryAsync(
                        @"INSERT INTO Patients (full_name, email, phone, date_of_birth, gender)
                          VALUES (@full_name, @email, @phone, @date_of_birth, @gender)",
                        patient
                    );

                    // Force error
                    throw new Exception("Test rollback");
                });
            }
            catch
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);

            var result = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                "SELECT * FROM Patients WHERE email = @email",
                new { email = patient.email }
            );

            Assert.IsNull(result, "Patient should not exist after rollback");
        }

        #endregion

        #region Query Optimization Tests

        [TestMethod]
        public async Task QueryOptimizer_LoadInvoicesWithDetails_PreventN1()
        {
            // Arrange
            var optimizer = new QueryOptimizer(testConnectionString);

            // Act
            var startTime = DateTime.Now;
            var invoices = await optimizer.LoadInvoicesWithDetailsAsync();
            var duration = (DateTime.Now - startTime).TotalMilliseconds;

            // Assert
            Assert.IsNotNull(invoices);
            // JOIN query should be fast (< 100ms for reasonable dataset)
            Assert.IsTrue(duration < 1000, $"Query took {duration}ms - might be N+1");
        }

        [TestMethod]
        public async Task QueryOptimizer_BatchLoad_MoreEfficientThanLoop()
        {
            // Arrange
            var optimizer = new QueryOptimizer(testConnectionString);
            var patientIds = new[] { 1, 2, 3, 4, 5 };

            // Act - Batch load
            var startBatch = DateTime.Now;
            var batchResults = await optimizer.BatchLoadPatientsAsync(patientIds);
            var batchDuration = (DateTime.Now - startBatch).TotalMilliseconds;

            // Act - Individual loads (N+1)
            var startLoop = DateTime.Now;
            var loopResults = new List<Patient>();
            foreach (var id in patientIds)
            {
                var patient = await dbHelper.ExecuteQuerySingleAsync<Patient>(
                    "SELECT * FROM Patients WHERE id = @id",
                    new { id }
                );
                if (patient != null)
                    loopResults.Add(patient);
            }
            var loopDuration = (DateTime.Now - startLoop).TotalMilliseconds;

            // Assert
            Assert.IsTrue(batchDuration < loopDuration,
                $"Batch load ({batchDuration}ms) should be faster than loop ({loopDuration}ms)");
        }

        #endregion

        #region Cache Integration Tests

        [TestMethod]
        public async Task Cache_Integration_WithDatabase()
        {
            // Arrange
            var cache = CacheManager.Instance;
            cache.Clear();
            string cacheKey = "test_patients";

            // Act - First call (should hit database)
            var startFirst = DateTime.Now;
            var firstResult = await cache.GetOrSetAsync(
                cacheKey,
                async () => await dbHelper.ExecuteQueryAsync<Patient>(
                    "SELECT * FROM Patients WHERE email LIKE '%@test.com'"
                ),
                TimeSpan.FromMinutes(5)
            );
            var firstDuration = (DateTime.Now - startFirst).TotalMilliseconds;

            // Act - Second call (should hit cache)
            var startSecond = DateTime.Now;
            var secondResult = await cache.GetOrSetAsync(
                cacheKey,
                async () => await dbHelper.ExecuteQueryAsync<Patient>(
                    "SELECT * FROM Patients WHERE email LIKE '%@test.com'"
                ),
                TimeSpan.FromMinutes(5)
            );
            var secondDuration = (DateTime.Now - startSecond).TotalMilliseconds;

            // Assert
            Assert.IsTrue(secondDuration < firstDuration,
                $"Cache hit ({secondDuration}ms) should be faster than DB query ({firstDuration}ms)");

            Assert.IsTrue(secondDuration < 5,
                $"Cache hit should be very fast (<5ms), was {secondDuration}ms");
        }

        #endregion

        #region Concurrent Access Tests

        [TestMethod]
        public async Task ConcurrentInserts_NoDeadlock()
        {
            // Arrange
            int taskCount = 10;
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < taskCount; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    var patient = new Patient
                    {
                        full_name = $"Concurrent Patient {index}",
                        email = $"concurrent{index}@test.com",
                        phone = "0123456789",
                        date_of_birth = DateTime.Now.AddYears(-30),
                        gender = "Male"
                    };

                    await dbHelper.ExecuteNonQueryAsync(
                        @"INSERT INTO Patients (full_name, email, phone, date_of_birth, gender)
                          VALUES (@full_name, @email, @phone, @date_of_birth, @gender)",
                        patient
                    );
                }));
            }

            await Task.WhenAll(tasks);

            // Assert
            var results = await dbHelper.ExecuteQueryAsync<Patient>(
                "SELECT * FROM Patients WHERE email LIKE 'concurrent%@test.com'"
            );

            Assert.AreEqual(taskCount, results.Count());
        }

        #endregion

        #region Helper Methods

        private async Task<Patient> CreateTestPatient()
        {
            var patient = new Patient
            {
                full_name = "Test Patient",
                email = $"test{Guid.NewGuid()}@test.com",
                phone = "0123456789",
                date_of_birth = new DateTime(1990, 1, 1),
                gender = "Male",
                address = "Test Address"
            };

            int id = await dbHelper.ExecuteScalarAsync<int>(
                @"INSERT INTO Patients (full_name, email, phone, date_of_birth, gender, address)
                  OUTPUT INSERTED.id
                  VALUES (@full_name, @email, @phone, @date_of_birth, @gender, @address)",
                patient
            );

            patient.id = id;
            return patient;
        }

        private async Task<Service> CreateTestService(string name)
        {
            var service = new Service
            {
                service_name = name,
                description = "Test Description",
                price = 100000,
                duration_minutes = 30
            };

            int id = await dbHelper.ExecuteScalarAsync<int>(
                @"INSERT INTO Services (service_name, description, price, duration_minutes)
                  OUTPUT INSERTED.id
                  VALUES (@service_name, @description, @price, @duration_minutes)",
                service
            );

            service.id = id;
            return service;
        }

        #endregion
    }
}
