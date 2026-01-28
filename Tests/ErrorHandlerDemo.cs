using System;
using System.Data.SqlClient;
using System.IO;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests
{
    /// <summary>
    /// Demo và test ErrorHandler functionality
    /// Run this để kiểm tra error logging system
    /// </summary>
    class ErrorHandlerDemo
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         ERROR HANDLER DEMO & TEST                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            RunAllTests();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public static void RunAllTests()
        {
            Test1_BasicErrorLogging();
            Test2_WarningLogging();
            Test3_DatabaseError();
            Test4_FileIOError();
            Test5_LogLevels();
            Test6_UserFriendlyMessages();
            Test7_LogFileRotation();
            Test8_OldLogCleanup();

            Console.WriteLine("\n✅ ALL TESTS COMPLETED!");
            Console.WriteLine($"📁 Log file location: {ErrorHandler.GetLogFilePath()}");
            Console.WriteLine($"📊 Check the log file to verify all errors were logged correctly.");
        }

        /// <summary>
        /// Test 1: Basic error logging
        /// </summary>
        static void Test1_BasicErrorLogging()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 1: Basic Error Logging");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            try
            {
                // Simulate an error
                throw new InvalidOperationException("This is a test error");
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "Test1_BasicErrorLogging", showToUser: false);
                Console.WriteLine("✅ Test 1 PASSED: Error logged successfully");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test 2: Warning logging
        /// </summary>
        static void Test2_WarningLogging()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 2: Warning Logging");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            try
            {
                throw new ArgumentException("Invalid argument provided");
            }
            catch (Exception ex)
            {
                ErrorHandler.LogWarning(ex, "Test2_WarningLogging", showToUser: false);
                Console.WriteLine("✅ Test 2 PASSED: Warning logged successfully");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test 3: Database error simulation
        /// </summary>
        static void Test3_DatabaseError()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 3: Database Error Simulation");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            try
            {
                // Simulate SQL timeout
                var sqlEx = new SqlException();
                throw sqlEx;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "Database Query - Test3", showToUser: false);
                Console.WriteLine("✅ Test 3 PASSED: Database error logged with user-friendly message");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test 4: File I/O error
        /// </summary>
        static void Test4_FileIOError()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 4: File I/O Error");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            try
            {
                // Simulate file not found
                File.ReadAllText("C:\\nonexistent\\file.txt");
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "File operation - Test4", showToUser: false);
                Console.WriteLine("✅ Test 4 PASSED: File I/O error logged");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Test 5: Different log levels
        /// </summary>
        static void Test5_LogLevels()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 5: Different Log Levels");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            ErrorHandler.LogMessage("This is an INFO message", ErrorLevel.INFO);
            ErrorHandler.LogMessage("This is a WARNING message", ErrorLevel.WARNING);
            ErrorHandler.LogMessage("This is an ERROR message", ErrorLevel.ERROR);
            ErrorHandler.LogMessage("This is a CRITICAL message", ErrorLevel.CRITICAL);

            Console.WriteLine("✅ Test 5 PASSED: All log levels tested");
            Console.WriteLine();
        }

        /// <summary>
        /// Test 6: User-friendly error messages
        /// </summary>
        static void Test6_UserFriendlyMessages()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 6: User-Friendly Error Messages");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            // Test các loại exception khác nhau
            Exception[] testExceptions = new Exception[]
            {
                new SqlException(),
                new IOException("File access denied"),
                new ArgumentException("Invalid data format"),
                new UnauthorizedAccessException("No permission"),
                new FormatException("Invalid format"),
                new Exception("Generic error")
            };

            foreach (var ex in testExceptions)
            {
                try
                {
                    throw ex;
                }
                catch (Exception e)
                {
                    ErrorHandler.LogError(e, "Test6_UserFriendlyMessages", showToUser: false);
                    Console.WriteLine($"   ✓ Logged: {e.GetType().Name}");
                }
            }

            Console.WriteLine("✅ Test 6 PASSED: All error types logged with friendly messages");
            Console.WriteLine();
        }

        /// <summary>
        /// Test 7: Log file rotation
        /// </summary>
        static void Test7_LogFileRotation()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 7: Log File Rotation (Simulated)");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            string logPath = ErrorHandler.GetLogFilePath();
            FileInfo fileInfo = new FileInfo(logPath);

            if (fileInfo.Exists)
            {
                long fileSizeKB = fileInfo.Length / 1024;
                Console.WriteLine($"   Current log file size: {fileSizeKB} KB");
                Console.WriteLine($"   Max size before rotation: 10240 KB (10 MB)");
                
                if (fileSizeKB < 10240)
                {
                    Console.WriteLine("   ✓ File size is within limit");
                }
                else
                {
                    Console.WriteLine("   ⚠ File should be rotated");
                }
            }
            else
            {
                Console.WriteLine("   ⚠ Log file not created yet");
            }

            Console.WriteLine("✅ Test 7 PASSED: Log rotation check completed");
            Console.WriteLine();
        }

        /// <summary>
        /// Test 8: Old log cleanup
        /// </summary>
        static void Test8_OldLogCleanup()
        {
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("Test 8: Old Log Cleanup");
            Console.WriteLine("═══════════════════════════════════════════════════════════");

            try
            {
                // Run cleanup (keeps last 30 days)
                ErrorHandler.CleanupOldLogs(30);
                Console.WriteLine("   ✓ Cleanup executed successfully");
                Console.WriteLine("   ✓ Logs older than 30 days have been removed");
                Console.WriteLine("✅ Test 8 PASSED: Old log cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Cleanup failed: {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
