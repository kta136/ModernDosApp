using System;
using System.IO;
using System.Configuration;

namespace FocusModern.Utilities
{
    /// <summary>
    /// Simple logging utility for FOCUS Modern
    /// </summary>
    public static class Logger
    {
        private static string logFilePath;
        private static readonly object lockObject = new object();

        /// <summary>
        /// Initialize logger
        /// </summary>
        public static void Initialize()
        {
            try
            {
                string logDir = Path.Combine(
                    Environment.ExpandEnvironmentVariables(
                        ConfigurationManager.AppSettings["DatabasePath"]), 
                    "Logs");
                
                Directory.CreateDirectory(logDir);
                
                string fileName = $"focus_modern_{DateTime.Now:yyyyMMdd}.log";
                logFilePath = Path.Combine(logDir, fileName);
                
                Info("Logger initialized");
            }
            catch (Exception ex)
            {
                // Fallback to temp directory
                string tempDir = Path.GetTempPath();
                logFilePath = Path.Combine(tempDir, $"focus_modern_{DateTime.Now:yyyyMMdd}.log");
                
                Error($"Logger initialization failed, using temp directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Log information message
        /// </summary>
        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// Log error message
        /// </summary>
        public static void Error(string message, Exception ex = null)
        {
            string errorMessage = message;
            if (ex != null)
            {
                errorMessage += $" Exception: {ex.Message}";
                if (ex.StackTrace != null)
                    errorMessage += $" StackTrace: {ex.StackTrace}";
            }
            
            WriteLog("ERROR", errorMessage);
        }

        /// <summary>
        /// Log warning message
        /// </summary>
        public static void Warning(string message)
        {
            WriteLog("WARN", message);
        }

        /// <summary>
        /// Log debug message
        /// </summary>
        public static void Debug(string message)
        {
            string logLevel = ConfigurationManager.AppSettings["LogLevel"] ?? "Info";
            if (logLevel.Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                WriteLog("DEBUG", message);
            }
        }

        /// <summary>
        /// Write log entry
        /// </summary>
        private static void WriteLog(string level, string message)
        {
            try
            {
                lock (lockObject)
                {
                    string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                    
                    if (!string.IsNullOrEmpty(logFilePath))
                    {
                        File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
                    }
                    
                    // Also write to console in debug mode
                    #if DEBUG
                    Console.WriteLine(logEntry);
                    #endif
                }
            }
            catch
            {
                // Silently ignore logging errors to prevent cascading failures
            }
        }
    }
}