using Microsoft.Win32;
using System.IO;
using System;

namespace LogScreen.Utils
{
    internal class WinAPIHelper
    {
        /// <summary>
        /// Thêm ứng dụng vào danh sách khởi động cùng Windows
        /// </summary>
        /// <param name="appName">Tên ứng dụng</param>
        public static void SetStartup(string appName = "Monitoring")
        {
            // Tìm đường dẫn file .exe của ứng dụng ClickOnce
            var exePath = WinAPIHelper.FindClickOnceExe("LogScreen.exe");

            // Kiểm tra nếu exePath là null trước khi thực hiện
            if (exePath == null)
            {
                FileHelper.LogError("Không tìm thấy file .exe của ứng dụng. Không thể thêm vào khởi động.");
                return;
            }

            // Mở khóa Registry để sửa đổi
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key != null)
                {
                    // Thêm giá trị ứng dụng khởi động
                    key.SetValue(appName, exePath);
                    Console.WriteLine($"Đã thêm {appName} vào danh sách khởi động cùng Windows.");
                }
                else
                {
                    FileHelper.LogError("Không thể truy cập Registry. Hãy kiểm tra quyền Administrator.");
                }
            }
        }

        // Xóa ứng dụng khỏi danh sách khởi động cùng Windows
        public static void RemoveStartup(string appName = "Monitoring")
        {
            // Mở khóa Registry để sửa đổi
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key != null)
                {
                    // Kiểm tra nếu giá trị tồn tại trước khi xóa
                    if (key.GetValue(appName) != null)
                    {
                        key.DeleteValue(appName);
                        Console.WriteLine($"Đã xóa {appName} khỏi danh sách khởi động cùng Windows.");
                    }
                    else
                    {
                        FileHelper.LogError($"Không tìm thấy {appName} trong danh sách khởi động.");
                    }
                }
                else
                {
                    FileHelper.LogError("Không thể truy cập Registry. Hãy kiểm tra quyền Administrator.");
                }
            }
        }

        /// <summary>
        /// Tìm file .exe trong thư mục ClickOnce
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string FindClickOnceExe(string appName)
        {
            string clickOnceRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps", "2.0");

            // Kiểm tra nếu thư mục gốc không tồn tại
            if (!Directory.Exists(clickOnceRoot))
            {
                FileHelper.LogError("Không tìm thấy thư mục ClickOnce.");
                return null;
            }

            try
            {
                // Duyệt qua tất cả các thư mục con
                string[] directories = Directory.GetDirectories(clickOnceRoot, "*", SearchOption.AllDirectories);

                foreach (string dir in directories)
                {
                    try
                    {
                        // Lấy danh sách file .exe trong thư mục hiện tại
                        string[] exeFiles = Directory.GetFiles(dir, "*.exe", SearchOption.TopDirectoryOnly);
                        foreach (string exe in exeFiles)
                        {
                            // Kiểm tra nếu tên file trùng khớp
                            if (Path.GetFileName(exe).Equals(appName, StringComparison.OrdinalIgnoreCase))
                            {
                                return exe; // Trả về đường dẫn file .exe
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Bỏ qua các thư mục không có quyền truy cập
                        FileHelper.LogError($"Không thể truy cập thư mục: {dir}. Bỏ qua.");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác
                FileHelper.LogError($"Đã xảy ra lỗi khi tìm file .exe: {ex.Message}");
            }

            // Trả về null nếu không tìm thấy file .exe
            FileHelper.LogError($"Không tìm thấy file {appName} trong thư mục ClickOnce.");
            return null;
        }
    }
}