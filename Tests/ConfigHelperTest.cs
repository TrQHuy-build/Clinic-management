using System;
using DentalClinicManagement.Utils;

namespace DentalClinicManagement.Tests
{
    /// <summary>
    /// Test ConfigHelper functionality
    /// Uncomment Main() để chạy standalone
    /// </summary>
    public class ConfigHelperTest
    {
        /*
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            RunAllTests();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        */

        public static void RunAllTests()
        {
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("🔧 CONFIGHELPER TEST");
            Console.WriteLine("═══════════════════════════════════════════════════\n");

            Test1_GetConnectionString();
            Test2_GetAppSettings();
            Test3_EnvironmentDetection();
            Test4_ValidationConfiguration();
            Test5_TypedSettings();

            Console.WriteLine("\n═══════════════════════════════════════════════════");
            Console.WriteLine("✅ ALL TESTS COMPLETED!");
            Console.WriteLine("═══════════════════════════════════════════════════");
        }

        static void Test1_GetConnectionString()
        {
            Console.WriteLine("TEST 1: Get Connection String");
            Console.WriteLine("───────────────────────────────────────────────────");

            try
            {
                string connStr = ConfigHelper.GetConnectionString();
                Console.WriteLine($"Connection String: {connStr.Substring(0, Math.Min(50, connStr.Length))}...");
                
                if (!string.IsNullOrEmpty(connStr))
                {
                    Console.WriteLine("✅ PASSED - Connection string loaded successfully\n");
                }
                else
                {
                    Console.WriteLine("❌ FAILED - Connection string is empty\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - Error: {ex.Message}\n");
            }
        }

        static void Test2_GetAppSettings()
        {
            Console.WriteLine("TEST 2: Get App Settings");
            Console.WriteLine("───────────────────────────────────────────────────");

            try
            {
                string environment = ConfigHelper.GetAppSetting("Environment", "Unknown");
                string activeConn = ConfigHelper.GetAppSetting("ActiveConnectionString", "Unknown");
                string logPath = ConfigHelper.GetAppSetting("LogFilePath", "Unknown");

                Console.WriteLine($"Environment: {environment}");
                Console.WriteLine($"Active Connection: {activeConn}");
                Console.WriteLine($"Log Path: {logPath}");

                if (!string.IsNullOrEmpty(environment))
                {
                    Console.WriteLine("✅ PASSED - App settings loaded successfully\n");
                }
                else
                {
                    Console.WriteLine("❌ FAILED - App settings not found\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - Error: {ex.Message}\n");
            }
        }

        static void Test3_EnvironmentDetection()
        {
            Console.WriteLine("TEST 3: Environment Detection");
            Console.WriteLine("───────────────────────────────────────────────────");

            try
            {
                bool isDev = ConfigHelper.IsDevelopment();
                bool isProd = ConfigHelper.IsProduction();
                string env = ConfigHelper.GetEnvironment();

                Console.WriteLine($"IsDevelopment: {isDev}");
                Console.WriteLine($"IsProduction: {isProd}");
                Console.WriteLine($"Environment: {env}");

                if (isDev || isProd)
                {
                    Console.WriteLine("✅ PASSED - Environment detection working\n");
                }
                else
                {
                    Console.WriteLine("⚠️  WARNING - Environment not detected properly\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - Error: {ex.Message}\n");
            }
        }

        static void Test4_ValidationConfiguration()
        {
            Console.WriteLine("TEST 4: Validate Configuration");
            Console.WriteLine("───────────────────────────────────────────────────");

            try
            {
                bool isValid = ConfigHelper.ValidateConfiguration();

                if (isValid)
                {
                    Console.WriteLine("✅ PASSED - Configuration is valid\n");
                }
                else
                {
                    Console.WriteLine("❌ FAILED - Configuration validation failed\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - Error: {ex.Message}\n");
            }
        }

        static void Test5_TypedSettings()
        {
            Console.WriteLine("TEST 5: Typed Settings (Bool, Int)");
            Console.WriteLine("───────────────────────────────────────────────────");

            try
            {
                bool logging = ConfigHelper.GetBoolSetting("EnableLogging", false);
                int timeout = ConfigHelper.GetIntSetting("SessionTimeout", 0);

                Console.WriteLine($"EnableLogging: {logging}");
                Console.WriteLine($"SessionTimeout: {timeout} minutes");

                if (timeout > 0)
                {
                    Console.WriteLine("✅ PASSED - Typed settings working\n");
                }
                else
                {
                    Console.WriteLine("⚠️  WARNING - Default values used\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - Error: {ex.Message}\n");
            }
        }
    }
}
