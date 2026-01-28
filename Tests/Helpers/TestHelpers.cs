using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DentalClinicManagement.Tests.Helpers
{
    /// <summary>
    /// Helper utilities for testing
    /// Provides common test functionality
    /// </summary>
    public static class TestHelpers
    {
        #region Performance Testing

        /// <summary>
        /// Measure execution time of an action
        /// </summary>
        public static TimeSpan MeasureExecutionTime(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Measure execution time of an async action
        /// </summary>
        public static async Task<TimeSpan> MeasureExecutionTimeAsync(Func<Task> action)
        {
            var sw = Stopwatch.StartNew();
            await action();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Assert that an action completes within specified time
        /// </summary>
        public static void AssertExecutionTime(Action action, TimeSpan maxDuration, string message = null)
        {
            var duration = MeasureExecutionTime(action);
            Assert.IsTrue(
                duration <= maxDuration,
                message ?? $"Expected execution time <= {maxDuration}, but was {duration}"
            );
        }

        /// <summary>
        /// Assert that an async action completes within specified time
        /// </summary>
        public static async Task AssertExecutionTimeAsync(Func<Task> action, TimeSpan maxDuration, string message = null)
        {
            var duration = await MeasureExecutionTimeAsync(action);
            Assert.IsTrue(
                duration <= maxDuration,
                message ?? $"Expected execution time <= {maxDuration}, but was {duration}"
            );
        }

        #endregion

        #region Exception Testing

        /// <summary>
        /// Assert that an action throws a specific exception
        /// </summary>
        public static void AssertThrows<TException>(Action action, string expectedMessage = null)
            where TException : Exception
        {
            try
            {
                action();
                Assert.Fail($"Expected {typeof(TException).Name} but no exception was thrown");
            }
            catch (TException ex)
            {
                if (expectedMessage != null)
                {
                    Assert.IsTrue(
                        ex.Message.Contains(expectedMessage),
                        $"Expected message to contain '{expectedMessage}', but was '{ex.Message}'"
                    );
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected {typeof(TException).Name} but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Assert that an async action throws a specific exception
        /// </summary>
        public static async Task AssertThrowsAsync<TException>(Func<Task> action, string expectedMessage = null)
            where TException : Exception
        {
            try
            {
                await action();
                Assert.Fail($"Expected {typeof(TException).Name} but no exception was thrown");
            }
            catch (TException ex)
            {
                if (expectedMessage != null)
                {
                    Assert.IsTrue(
                        ex.Message.Contains(expectedMessage),
                        $"Expected message to contain '{expectedMessage}', but was '{ex.Message}'"
                    );
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected {typeof(TException).Name} but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Assert that an action does not throw any exception
        /// </summary>
        public static void AssertDoesNotThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        #endregion

        #region Retry Logic

        /// <summary>
        /// Retry an action multiple times if it fails
        /// Useful for flaky tests
        /// </summary>
        public static void RetryOnFailure(Action action, int maxAttempts = 3, int delayMs = 100)
        {
            int attempt = 0;
            Exception lastException = null;

            while (attempt < maxAttempts)
            {
                try
                {
                    action();
                    return; // Success
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    attempt++;
                    if (attempt < maxAttempts)
                    {
                        System.Threading.Thread.Sleep(delayMs);
                    }
                }
            }

            throw new Exception($"Action failed after {maxAttempts} attempts", lastException);
        }

        /// <summary>
        /// Retry an async action multiple times if it fails
        /// </summary>
        public static async Task RetryOnFailureAsync(Func<Task> action, int maxAttempts = 3, int delayMs = 100)
        {
            int attempt = 0;
            Exception lastException = null;

            while (attempt < maxAttempts)
            {
                try
                {
                    await action();
                    return; // Success
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    attempt++;
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(delayMs);
                    }
                }
            }

            throw new Exception($"Action failed after {maxAttempts} attempts", lastException);
        }

        #endregion

        #region Wait Helpers

        /// <summary>
        /// Wait until a condition is true or timeout
        /// </summary>
        public static bool WaitUntil(Func<bool> condition, TimeSpan timeout, TimeSpan? pollingInterval = null)
        {
            var interval = pollingInterval ?? TimeSpan.FromMilliseconds(100);
            var sw = Stopwatch.StartNew();

            while (sw.Elapsed < timeout)
            {
                if (condition())
                    return true;

                System.Threading.Thread.Sleep(interval);
            }

            return false;
        }

        /// <summary>
        /// Wait until an async condition is true or timeout
        /// </summary>
        public static async Task<bool> WaitUntilAsync(Func<Task<bool>> condition, TimeSpan timeout, TimeSpan? pollingInterval = null)
        {
            var interval = pollingInterval ?? TimeSpan.FromMilliseconds(100);
            var sw = Stopwatch.StartNew();

            while (sw.Elapsed < timeout)
            {
                if (await condition())
                    return true;

                await Task.Delay(interval);
            }

            return false;
        }

        #endregion

        #region Comparison Helpers

        /// <summary>
        /// Assert that two objects have the same property values
        /// </summary>
        public static void AssertObjectsEqual<T>(T expected, T actual, params string[] propertyNames)
        {
            var type = typeof(T);

            foreach (var propertyName in propertyNames)
            {
                var property = type.GetProperty(propertyName);
                if (property == null)
                {
                    Assert.Fail($"Property '{propertyName}' not found on type {type.Name}");
                }

                var expectedValue = property.GetValue(expected);
                var actualValue = property.GetValue(actual);

                Assert.AreEqual(
                    expectedValue,
                    actualValue,
                    $"Property '{propertyName}': expected {expectedValue}, but was {actualValue}"
                );
            }
        }

        /// <summary>
        /// Assert that a decimal value is within a tolerance range
        /// </summary>
        public static void AssertDecimalEqual(decimal expected, decimal actual, decimal tolerance = 0.01m)
        {
            var diff = Math.Abs(expected - actual);
            Assert.IsTrue(
                diff <= tolerance,
                $"Expected {expected} ± {tolerance}, but was {actual} (diff: {diff})"
            );
        }

        /// <summary>
        /// Assert that a DateTime value is within a tolerance range
        /// </summary>
        public static void AssertDateTimeEqual(DateTime expected, DateTime actual, TimeSpan? tolerance = null)
        {
            var maxDiff = tolerance ?? TimeSpan.FromSeconds(1);
            var diff = (expected - actual).Duration();

            Assert.IsTrue(
                diff <= maxDiff,
                $"Expected {expected} ± {maxDiff}, but was {actual} (diff: {diff})"
            );
        }

        #endregion

        #region Collection Helpers

        /// <summary>
        /// Assert that a collection contains elements matching a predicate
        /// </summary>
        public static void AssertContains<T>(System.Collections.Generic.IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                    return;
            }

            Assert.Fail(message ?? "Collection does not contain expected element");
        }

        /// <summary>
        /// Assert that all elements in a collection match a predicate
        /// </summary>
        public static void AssertAll<T>(System.Collections.Generic.IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            int index = 0;
            foreach (var item in collection)
            {
                if (!predicate(item))
                {
                    Assert.Fail(message ?? $"Element at index {index} does not match predicate");
                }
                index++;
            }
        }

        #endregion

        #region Memory Helpers

        /// <summary>
        /// Get current memory usage in bytes
        /// </summary>
        public static long GetMemoryUsage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            return GC.GetTotalMemory(false);
        }

        /// <summary>
        /// Measure memory increase after an action
        /// </summary>
        public static long MeasureMemoryIncrease(Action action)
        {
            var before = GetMemoryUsage();
            action();
            var after = GetMemoryUsage();
            return after - before;
        }

        /// <summary>
        /// Assert that an action does not leak memory
        /// </summary>
        public static void AssertNoMemoryLeak(Action action, long maxIncreaseBytes = 1024 * 1024) // 1MB default
        {
            var increase = MeasureMemoryIncrease(action);
            Assert.IsTrue(
                increase <= maxIncreaseBytes,
                $"Memory increased by {increase} bytes, exceeds maximum {maxIncreaseBytes} bytes"
            );
        }

        #endregion

        #region String Helpers

        /// <summary>
        /// Generate a random string for testing
        /// </summary>
        public static string RandomString(int length, bool numbersOnly = false)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string numbers = "0123456789";
            
            var pool = numbersOnly ? numbers : chars;
            var result = new char[length];
            
            for (int i = 0; i < length; i++)
            {
                result[i] = pool[random.Next(pool.Length)];
            }
            
            return new string(result);
        }

        #endregion

        #region Console Output Capture

        /// <summary>
        /// Capture console output during an action
        /// </summary>
        public static string CaptureConsoleOutput(Action action)
        {
            var originalOut = Console.Out;
            try
            {
                using (var writer = new System.IO.StringWriter())
                {
                    Console.SetOut(writer);
                    action();
                    return writer.ToString();
                }
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        #endregion
    }
}
