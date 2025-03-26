using System.Net.Http;
using System.Threading.Tasks;
using LogScreen.Entities;
using System;
using Newtonsoft.Json;
using System.IO;
using LogScreen.Utils;

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

        public static async Task<Config> GetConfigFromUrlAsync(string url, int retryDelayMs = 5000, int maxRetries = -1)
        {
            int attempt = 0;

            while (true) // Lặp vô hạn hoặc đến khi đạt maxRetries
            {
                try
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        FileHelper.LogError("Not found ConfigUrl");
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
                            return config;
                        }
                    }
                }
                catch (Exception ex)
                {
                    attempt++;
                    string errorMessage = $"Attempt {attempt}: Error when read config - {ex.Message}";

                    // Ghi lỗi vào file log
                    FileHelper.LogError(errorMessage);

                    // Nếu đạt tối đa số lần thử
                    if (maxRetries > 0 && attempt >= maxRetries)
                    {
                        FileHelper.LogError("Max retries reached. Unable to fetch config.");
                        return null;
                    }
                }

                // Chờ trước khi thử lại
                await Task.Delay(retryDelayMs);
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