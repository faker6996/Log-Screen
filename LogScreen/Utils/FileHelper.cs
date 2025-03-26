using System;
using System.IO;
using System.Windows.Forms;

namespace LogScreen.Utils
{
    public static class FileHelper
    {
        public static void CreateCaptureAddress(string captureAddress)
        {
            try
            {
                if (!Directory.Exists(captureAddress))
                {
                    Directory.CreateDirectory(captureAddress);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error: Please run the application as Administrator to create the folder in Program Files.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating folder: {ex.Message}");
            }
        }
        public static void CreateErrorLogFile(string errorLogAddress)
        {
            try
            {
                if (!Directory.Exists(errorLogAddress))
                {
                    Directory.CreateDirectory(errorLogAddress);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error: Please run the application as Administrator to create the folder in Program Files.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating folder: {ex.Message}");
            }
        }
        // Hàm ghi log vào file txt
        public static void LogError(string message)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(Setting.LOG_FILE_PATH, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Nếu ghi log thất bại, có thể bỏ qua hoặc xử lý thêm
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
