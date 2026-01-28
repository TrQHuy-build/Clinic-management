using System;
using System.Configuration;

namespace DentalClinicManagement.Utils
{
    /// <summary>
    /// Helper class để đọc configuration từ App.config
    /// Centralized configuration management
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Lấy connection string từ App.config
        /// </summary>
        /// <param name="name">Tên của connection string (mặc định: "DentalClinicDB")</param>
        /// <returns>Connection string</returns>
        public static string GetConnectionString(string name = null)
        {
            try
            {
                // Nếu không chỉ định name, lấy từ appSettings
                if (string.IsNullOrEmpty(name))
                {
                    name = GetAppSetting("ActiveConnectionString", "DentalClinicDB");
                }

                var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ConfigurationErrorsException(
                        $"Connection string '{name}' not found in App.config. " +
                        "Please check your configuration file.");
                }

                return connectionString;
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    "Error reading connection string from configuration.", ex);
            }
        }

        /// <summary>
        /// Lấy app setting từ App.config
        /// </summary>
        /// <param name="key">Key của setting</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị của setting</returns>
        public static string GetAppSetting(string key, string defaultValue = null)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogWarning(ex, $"Error reading app setting '{key}'", showToUser: false);
                return defaultValue;
            }
        }

        /// <summary>
        /// Lấy app setting dạng boolean
        /// </summary>
        public static bool GetBoolSetting(string key, bool defaultValue = false)
        {
            var value = GetAppSetting(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// Lấy app setting dạng integer
        /// </summary>
        public static int GetIntSetting(string key, int defaultValue = 0)
        {
            var value = GetAppSetting(key);
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        /// <summary>
        /// Kiểm tra environment hiện tại
        /// </summary>
        public static bool IsDevelopment()
        {
            return GetAppSetting("Environment", "Development")
                .Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra environment hiện tại có phải Production không
        /// </summary>
        public static bool IsProduction()
        {
            return GetAppSetting("Environment", "Development")
                .Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Lấy environment name
        /// </summary>
        public static string GetEnvironment()
        {
            return GetAppSetting("Environment", "Development");
        }

        /// <summary>
        /// Kiểm tra logging có được enable không
        /// </summary>
        public static bool IsLoggingEnabled()
        {
            return GetBoolSetting("EnableLogging", true);
        }

        /// <summary>
        /// Lấy đường dẫn log file
        /// </summary>
        public static string GetLogFilePath()
        {
            return GetAppSetting("LogFilePath", "Logs\\application.log");
        }

        /// <summary>
        /// Lấy session timeout (phút)
        /// </summary>
        public static int GetSessionTimeout()
        {
            return GetIntSetting("SessionTimeout", 30);
        }

        /// <summary>
        /// Validate configuration khi khởi động app
        /// </summary>
        public static bool ValidateConfiguration()
        {
            try
            {
                // Kiểm tra connection string
                var connString = GetConnectionString();
                if (string.IsNullOrEmpty(connString))
                {
                    ErrorHandler.LogMessage("❌ Connection string is empty or not found.", ErrorLevel.ERROR);
                    return false;
                }

                // Kiểm tra các settings quan trọng
                var environment = GetEnvironment();
                ErrorHandler.LogMessage($"✅ Environment: {environment}", ErrorLevel.INFO);
                ErrorHandler.LogMessage($"✅ Logging Enabled: {IsLoggingEnabled()}", ErrorLevel.INFO);
                ErrorHandler.LogMessage($"✅ Connection String: {MaskConnectionString(connString)}", ErrorLevel.INFO);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "Configuration validation failed", showToUser: false);
                return false;
            }
        }

        /// <summary>
        /// Mask sensitive information trong connection string
        /// </summary>
        private static string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return string.Empty;

            // Mask password nếu có
            if (connectionString.Contains("Password="))
            {
                var parts = connectionString.Split(';');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Trim().StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "Password=*****";
                    }
                }
                return string.Join(";", parts);
            }

            // Nếu là Integrated Security, hiển thị data source
            if (connectionString.Contains("Data Source="))
            {
                var dataSourceStart = connectionString.IndexOf("Data Source=");
                var dataSourceEnd = connectionString.IndexOf(";", dataSourceStart);
                if (dataSourceEnd > dataSourceStart)
                {
                    var dataSource = connectionString.Substring(dataSourceStart, dataSourceEnd - dataSourceStart);
                    return dataSource + "; [Integrated Security]";
                }
            }

            return "Connection String Configured";
        }
    }
}
