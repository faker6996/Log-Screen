using System;
using System.IO;
using System.Management;
using System.Windows.Forms;

namespace LogScreen.Utils
{
    public static class FileHelper
    {
        /// <summary>
        /// Retrieves the capture address, ensuring the directory exists.
        /// </summary>
        /// <returns>Capture directory path or null if an error occurs.</returns>
        public static string GetCaptureAddress()
        {
            try
            {
                if (!Directory.Exists(Setting.SCREEN_LOG_ADDRESS))
                {
                    Directory.CreateDirectory(Setting.SCREEN_LOG_ADDRESS);
                }
                return Path.Combine(Setting.SCREEN_LOG_ADDRESS);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error: Please run the application as Administrator to create the folder in Program Files.");
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating folder: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates the monitoring address if it does not already exist.
        /// </summary>
        public static void CreateMonitoringAddress()
        {
            try
            {
                if (!Directory.Exists(Setting.SCREEN_LOG_ADDRESS))
                {
                    Directory.CreateDirectory(Setting.SCREEN_LOG_ADDRESS);
                }
            }
            catch (UnauthorizedAccessException)
            {
                FileHelper.LogError("Please run the application as Administrator to create the folder in Program Files");
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error creating folder: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs an error message to a log file.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void LogError(string message)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(Setting.LOG_FILE_PATH, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the Windows ID (Operating System UUID).
        /// </summary>
        /// <returns>Windows ID as a string.</returns>
        public static string GetWindowsId()
        {
            try
            {
                string uuid = string.Empty;

                // Connect to WMI and retrieve information from Win32_ComputerSystemProduct
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        uuid = obj["UUID"]?.ToString();
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(uuid))
                {
                    return uuid;
                }
                else
                {
                    throw new Exception("Not Found Windows ID");
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error When Get Window ID: {ex}");
                return string.Empty;
            }
        }

    }
}
