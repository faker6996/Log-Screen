using System;
using System.Configuration;


namespace LogScreen.Utils
{
    public static class AppConfigHelper
    {
        /// <summary>
        /// Hàm đọc giá trị từ App.config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadAppConfig(string key)
        {
            // Kiểm tra nếu key tồn tại trong App.config
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key]; // Trả về giá trị của key
            }
            else
            {
                Console.WriteLine($"Không tìm thấy key '{key}' trong App.config.");
                return null;
            }
        }

        /// <summary>
        /// Hàm ghi hoặc cập nhật giá trị vào App.config
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteAppConfig(string key, string value)
        {
            // Mở App.config
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Kiểm tra nếu key đã tồn tại
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value; // Cập nhật giá trị
                Console.WriteLine($"Đã cập nhật key '{key}' với giá trị '{value}' vào App.config.");
            }
            else
            {
                config.AppSettings.Settings.Add(key, value); // Thêm key mới
                Console.WriteLine($"Đã thêm key '{key}' với giá trị '{value}' vào App.config.");
            }

            // Lưu thay đổi
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings"); // Làm mới section để áp dụng thay đổi
        }
    }
}