using System.Net.Http;
using System.Threading.Tasks;
using LogScreen.Entities;
using System;
using Newtonsoft.Json;
using System.IO;
using LogScreen.Utils;
using System.Collections.Generic;

namespace LogScreen.Managers
{
    public static class ConfigManager
    {

        // Đảm bảo thư mục log tồn tại
        static ConfigManager()
        {
            if (!Directory.Exists(Setting.SCREEN_LOG_ADDRESS))
            {
                Directory.CreateDirectory(Setting.SCREEN_LOG_ADDRESS);
            }
        }

        public static async Task<Config> GetConfigFromUrlAsync(string url, int retryDelayMs = 5000)
        {
            int maxRetries = 3; // Chỉ thử tối đa 3 lần
            int attempt = 0;

            while (attempt < maxRetries) // Giới hạn số lần thử
            {
                try
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        FileHelper.LogError("ConfigUrl không được tìm thấy.");
                        return null;
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string configContent = await response.Content.ReadAsStringAsync();
                        Config config = ParseConfig(configContent);

                        if (config != null) // Thành công thì trả về config
                        {
                            SetConfigDefault(config);
                            return config;
                        }
                    }
                }
                catch (Exception ex)
                {
                    attempt++;
                    string errorMessage = $"Thử lần thứ {attempt}: Lỗi khi đọc cấu hình - {ex.Message}";

                    // Ghi lỗi vào file log
                    FileHelper.LogError(errorMessage);
                }

                // Chờ trước khi thử lại
                await Task.Delay(retryDelayMs);
            }

            // Nếu đã thử đủ 3 lần mà không thành công
            FileHelper.LogError("Đã thử tối đa số lần cho phép. Không thể tải cấu hình.");
            return null;
        }
        public static Config GetConfigDefault()
        {
            return new Config()
            {
                START = AppConfigHelper.ReadAppConfig(Config._START),
                STOP = AppConfigHelper.ReadAppConfig(Config._STOP),
                INTERVAL = AppConfigHelper.ReadAppConfig(Config._INTERVAL),
                ACTION_QTY = AppConfigHelper.ReadAppConfig(Config._ACTION_QTY),
                START_WITH_WINDOW = AppConfigHelper.ReadAppConfig(Config._START_WITH_WINDOW),
                ALLOW_FORCE_ENDTASK = AppConfigHelper.ReadAppConfig(Config._ALLOW_FORCE_ENDTASK),
                LIVE_CAPTURE_CHECK_FREQUENT = AppConfigHelper.ReadAppConfig(Config._LIVE_CAPTURE_CHECK_FREQUENT),
                SOUND_DETECT = AppConfigHelper.ReadAppConfig(Config._SOUND_DETECT)
            };
        }

        public static void SetConfigDefault(Config config)
        {
            try
            {
                if (config == null)
                {
                    Console.WriteLine("Đối tượng Config không hợp lệ.");
                    return;
                }

                // Sử dụng Dictionary để duyệt qua các key-value của đối tượng Config
                Dictionary<string, string> configValues = new Dictionary<string, string>
                {
                    { Config._START, config.START },
                    { Config._STOP, config.STOP },
                    { Config._INTERVAL, config.INTERVAL },
                    { Config._ACTION_QTY, config.ACTION_QTY },
                    { Config._START_WITH_WINDOW, config.START_WITH_WINDOW },
                    { Config._ALLOW_FORCE_ENDTASK, config.ALLOW_FORCE_ENDTASK },
                    { Config._LIVE_CAPTURE_CHECK_FREQUENT, config.LIVE_CAPTURE_CHECK_FREQUENT },
                    { Config._SOUND_DETECT, config.SOUND_DETECT }
                };

                foreach (var pair in configValues)
                {
                    AppConfigHelper.WriteAppConfig(pair.Key, pair.Value);
                }

                Console.WriteLine("Đã ghi toàn bộ giá trị mặc định vào App.config.");
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Đã xảy ra lỗi khi ghi giá trị mặc định vào App.config: {ex.Message}");
            }
        }



        private static Config ParseConfig(string configContent)
        {
            try
            {
                Config config = JsonConvert.DeserializeObject<Config>(configContent);
                return config;
            }
            catch (JsonException ex)
            {
                FileHelper.LogError($"Error when parse JSON: {ex.Message}");
                return null;
            }
        }


    }
}