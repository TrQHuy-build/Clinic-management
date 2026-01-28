using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DentalClinicManagement.Utils;
using DentalClinicManagement.Models;

namespace DentalClinicManagement.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for CacheManager
    /// Tests caching functionality, expiration, and invalidation
    /// </summary>
    [TestClass]
    public class CacheManagerTests
    {
        private CacheManager cache;

        [TestInitialize]
        public void Setup()
        {
            cache = CacheManager.Instance;
            cache.Clear(); // Clear cache before each test
        }

        [TestCleanup]
        public void Cleanup()
        {
            cache.Clear(); // Clean up after each test
        }

        #region Basic Cache Operations

        [TestMethod]
        public void Set_And_Get_ReturnsCorrectValue()
        {
            // Arrange
            string key = "test_key";
            string value = "test_value";

            // Act
            cache.Set(key, value, TimeSpan.FromMinutes(5));
            string result = cache.Get<string>(key);

            // Assert
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Get_NonExistentKey_ReturnsDefault()
        {
            // Act
            string result = cache.Get<string>("non_existent_key");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Set_ComplexObject_ReturnsCorrectObject()
        {
            // Arrange
            var patient = new Patient
            {
                id = 1,
                full_name = "John Doe",
                email = "john@example.com"
            };

            // Act
            cache.Set("patient_1", patient, TimeSpan.FromMinutes(5));
            var result = cache.Get<Patient>("patient_1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(patient.id, result.id);
            Assert.AreEqual(patient.full_name, result.full_name);
            Assert.AreEqual(patient.email, result.email);
        }

        #endregion

        #region Expiration Tests

        [TestMethod]
        public void Get_ExpiredKey_ReturnsDefault()
        {
            // Arrange
            string key = "expired_key";
            string value = "test_value";
            cache.Set(key, value, TimeSpan.FromMilliseconds(100));

            // Act
            Thread.Sleep(150); // Wait for expiration
            string result = cache.Get<string>(key);

            // Assert
            Assert.IsNull(result, "Expired cache entry should return null");
        }

        [TestMethod]
        public void Set_ZeroExpiration_DoesNotCache()
        {
            // Arrange
            string key = "zero_expiration";
            string value = "test_value";

            // Act
            cache.Set(key, value, TimeSpan.Zero);
            string result = cache.Get<string>(key);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Remove Tests

        [TestMethod]
        public void Remove_ExistingKey_RemovesSuccessfully()
        {
            // Arrange
            string key = "test_key";
            cache.Set(key, "value", TimeSpan.FromMinutes(5));

            // Act
            cache.Remove(key);
            string result = cache.Get<string>(key);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_NonExistentKey_DoesNotThrow()
        {
            // Act & Assert (should not throw exception)
            cache.Remove("non_existent_key");
        }

        #endregion

        #region Pattern Removal Tests

        [TestMethod]
        public void RemoveByPattern_MatchingPattern_RemovesAllMatching()
        {
            // Arrange
            cache.Set("patient_1", "value1", TimeSpan.FromMinutes(5));
            cache.Set("patient_2", "value2", TimeSpan.FromMinutes(5));
            cache.Set("service_1", "value3", TimeSpan.FromMinutes(5));

            // Act
            cache.RemoveByPattern("patient_");

            // Assert
            Assert.IsNull(cache.Get<string>("patient_1"));
            Assert.IsNull(cache.Get<string>("patient_2"));
            Assert.IsNotNull(cache.Get<string>("service_1"));
        }

        [TestMethod]
        public void RemoveByPattern_NoMatches_DoesNothing()
        {
            // Arrange
            cache.Set("patient_1", "value1", TimeSpan.FromMinutes(5));

            // Act
            cache.RemoveByPattern("service_");

            // Assert
            Assert.IsNotNull(cache.Get<string>("patient_1"));
        }

        #endregion

        #region Clear Tests

        [TestMethod]
        public void Clear_RemovesAllEntries()
        {
            // Arrange
            cache.Set("key1", "value1", TimeSpan.FromMinutes(5));
            cache.Set("key2", "value2", TimeSpan.FromMinutes(5));
            cache.Set("key3", "value3", TimeSpan.FromMinutes(5));

            // Act
            cache.Clear();

            // Assert
            Assert.IsNull(cache.Get<string>("key1"));
            Assert.IsNull(cache.Get<string>("key2"));
            Assert.IsNull(cache.Get<string>("key3"));
        }

        #endregion

        #region GetOrSet Tests

        [TestMethod]
        public void GetOrSet_NotInCache_ExecutesFactory()
        {
            // Arrange
            string key = "test_key";
            bool factoryExecuted = false;
            Func<string> factory = () =>
            {
                factoryExecuted = true;
                return "factory_value";
            };

            // Act
            string result = cache.GetOrSet(key, factory, TimeSpan.FromMinutes(5));

            // Assert
            Assert.AreEqual("factory_value", result);
            Assert.IsTrue(factoryExecuted);
        }

        [TestMethod]
        public void GetOrSet_AlreadyInCache_DoesNotExecuteFactory()
        {
            // Arrange
            string key = "test_key";
            cache.Set(key, "cached_value", TimeSpan.FromMinutes(5));
            
            bool factoryExecuted = false;
            Func<string> factory = () =>
            {
                factoryExecuted = true;
                return "factory_value";
            };

            // Act
            string result = cache.GetOrSet(key, factory, TimeSpan.FromMinutes(5));

            // Assert
            Assert.AreEqual("cached_value", result);
            Assert.IsFalse(factoryExecuted);
        }

        [TestMethod]
        public async Task GetOrSetAsync_NotInCache_ExecutesFactory()
        {
            // Arrange
            string key = "test_key";
            bool factoryExecuted = false;
            Func<Task<string>> factory = async () =>
            {
                await Task.Delay(10);
                factoryExecuted = true;
                return "factory_value";
            };

            // Act
            string result = await cache.GetOrSetAsync(key, factory, TimeSpan.FromMinutes(5));

            // Assert
            Assert.AreEqual("factory_value", result);
            Assert.IsTrue(factoryExecuted);
        }

        #endregion

        #region Invalidate Related Tests

        [TestMethod]
        public void InvalidateRelated_Patient_RemovesRelatedKeys()
        {
            // Arrange
            cache.Set("patients_all", "data", TimeSpan.FromMinutes(5));
            cache.Set("patient_1", "data", TimeSpan.FromMinutes(5));
            cache.Set("patient_search", "data", TimeSpan.FromMinutes(5));
            cache.Set("services_all", "data", TimeSpan.FromMinutes(5));

            // Act
            cache.InvalidateRelated("patient");

            // Assert
            Assert.IsNull(cache.Get<string>("patients_all"));
            Assert.IsNull(cache.Get<string>("patient_1"));
            Assert.IsNull(cache.Get<string>("patient_search"));
            Assert.IsNotNull(cache.Get<string>("services_all"));
        }

        [TestMethod]
        public void InvalidateRelated_Invoice_RemovesInvoiceAndDashboard()
        {
            // Arrange
            cache.Set("invoices_all", "data", TimeSpan.FromMinutes(5));
            cache.Set("dashboard_stats", "data", TimeSpan.FromMinutes(5));
            cache.Set("patients_all", "data", TimeSpan.FromMinutes(5));

            // Act
            cache.InvalidateRelated("invoice");

            // Assert
            Assert.IsNull(cache.Get<string>("invoices_all"));
            Assert.IsNull(cache.Get<string>("dashboard_stats"));
            Assert.IsNotNull(cache.Get<string>("patients_all"));
        }

        #endregion

        #region Statistics Tests

        [TestMethod]
        public void GetStatistics_ReturnsCorrectStats()
        {
            // Arrange
            cache.Set("key1", "value1", TimeSpan.FromMinutes(5));
            cache.Set("key2", "value2", TimeSpan.FromMinutes(5));
            cache.Get<string>("key1"); // Hit
            cache.Get<string>("non_existent"); // Miss

            // Act
            var stats = cache.GetStatistics();

            // Assert
            Assert.IsNotNull(stats);
            Assert.AreEqual(2, stats.TotalEntries);
            Assert.AreEqual(1, stats.Hits);
            Assert.AreEqual(1, stats.Misses);
        }

        [TestMethod]
        public void HitRate_CalculatesCorrectly()
        {
            // Arrange
            cache.Set("key1", "value1", TimeSpan.FromMinutes(5));
            cache.Get<string>("key1"); // Hit
            cache.Get<string>("key1"); // Hit
            cache.Get<string>("non_existent"); // Miss

            // Act
            var stats = cache.GetStatistics();

            // Assert
            double expectedHitRate = (2.0 / 3.0) * 100;
            Assert.AreEqual(expectedHitRate, stats.HitRate, 0.1);
        }

        #endregion

        #region Thread Safety Tests

        [TestMethod]
        public void ConcurrentAccess_ThreadSafe()
        {
            // Arrange
            int threadCount = 10;
            int operationsPerThread = 100;
            var threads = new Thread[threadCount];

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < operationsPerThread; j++)
                    {
                        string key = $"key_{threadId}_{j}";
                        cache.Set(key, $"value_{j}", TimeSpan.FromMinutes(5));
                        var value = cache.Get<string>(key);
                        cache.Remove(key);
                    }
                });
                threads[i].Start();
            }

            // Wait for all threads
            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Assert - No exception should occur
            Assert.IsTrue(true, "Cache operations should be thread-safe");
        }

        #endregion

        #region Memory Tests

        [TestMethod]
        public void LargeDataSet_HandlesCorrectly()
        {
            // Arrange & Act
            for (int i = 0; i < 1000; i++)
            {
                cache.Set($"key_{i}", $"value_{i}", TimeSpan.FromMinutes(5));
            }

            // Assert
            var stats = cache.GetStatistics();
            Assert.AreEqual(1000, stats.TotalEntries);

            // Verify random entries
            Assert.AreEqual("value_500", cache.Get<string>("key_500"));
            Assert.AreEqual("value_999", cache.Get<string>("key_999"));
        }

        #endregion

        #region Singleton Tests

        [TestMethod]
        public void Instance_AlwaysReturnsSameInstance()
        {
            // Act
            var instance1 = CacheManager.Instance;
            var instance2 = CacheManager.Instance;

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void Instance_AcrossThreads_ReturnsSameInstance()
        {
            // Arrange
            CacheManager instanceFromThread1 = null;
            CacheManager instanceFromThread2 = null;

            var thread1 = new Thread(() => instanceFromThread1 = CacheManager.Instance);
            var thread2 = new Thread(() => instanceFromThread2 = CacheManager.Instance);

            // Act
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            // Assert
            Assert.AreSame(instanceFromThread1, instanceFromThread2);
        }

        #endregion
    }
}
