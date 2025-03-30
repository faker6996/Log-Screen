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
        // Static constructor to ensure the configuration directory exists
        static ConfigManager()
        {
            if (!Directory.Exists(Setting.SCREEN_LOG_ADDRESS))
            {
                Directory.CreateDirectory(Setting.SCREEN_LOG_ADDRESS);
            }
        }
        /// <summary>
        /// Fetches the configuration from a given URL.
        /// Tries multiple times in case of failure.
        /// </summary>
        /// <param name="url">The URL to fetch the configuration from.</param>
        /// <returns>The configuration object if successful, otherwise null.</returns>
        public static async Task<Config> GetConfigFromUrlAsync(string url)
        {
            int numOfTries = 0;

            while (numOfTries < Setting.TRY_GET_CONFIG.MAX_REP)
            {
                try
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        FileHelper.LogError("ConfigUrl not found.");
                        return null;
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string configContent = await response.Content.ReadAsStringAsync();
                        Config config = ParseConfig(configContent);

                        if (config != null)
                        {
                            OverrideDefaultConfig(config);
                        }
                        return config;
                    }
                }
                catch (Exception ex)
                {
                    numOfTries++;
                    FileHelper.LogError($"Attempt #{numOfTries}: Error reading configuration - {ex.Message}");
                }

                await Task.Delay(Setting.TRY_GET_CONFIG.INTERVAL);
            }

            FileHelper.LogError("Maximum number of attempts reached. Unable to load configuration.");
            return null;
        }
        /// <summary>
        /// Retrieves the default configuration values from the application settings.
        /// </summary>
        /// <returns>A Config object populated with default values.</returns>
        public static Config GetDefaultConfig()
        {
            return new Config()
            {
                START = AppConfigHelper.ReadAppConfig(nameof(Config.START)),
                STOP = AppConfigHelper.ReadAppConfig(nameof(Config.STOP)),
                INTERVAL = AppConfigHelper.ReadAppConfig(nameof(Config.INTERVAL)),
                ACTION_QTY = AppConfigHelper.ReadAppConfig(nameof(Config.ACTION_QTY)),
                START_WITH_WINDOW = AppConfigHelper.ReadAppConfig(nameof(Config.START_WITH_WINDOW)),
                ALLOW_FORCE_ENDTASK = AppConfigHelper.ReadAppConfig(nameof(Config.ALLOW_FORCE_ENDTASK)),
                LIVE_CAPTURE_CHECK_FREQUENT = AppConfigHelper.ReadAppConfig(nameof(Config.LIVE_CAPTURE_CHECK_FREQUENT)),
                SOUND_DETECT = AppConfigHelper.ReadAppConfig(nameof(Config.SOUND_DETECT))
            };
        }
        /// <summary>
        /// Overrides the default configuration with the values from the provided config object.
        /// </summary>
        /// <param name="config">The new configuration object.</param>
        public static void OverrideDefaultConfig(Config config)
        {
            try
            {
                if (config == null)
                {
                    FileHelper.LogError("Invalid Config object.");
                    return;
                }

                Dictionary<string, string> configValues = new Dictionary<string, string>
                {
                    { nameof(Config.START), config.START },
                    { nameof(Config.STOP), config.STOP },
                    { nameof(Config.INTERVAL), config.INTERVAL },
                    { nameof(Config.ACTION_QTY), config.ACTION_QTY },
                    { nameof(Config.START_WITH_WINDOW), config.START_WITH_WINDOW },
                    { nameof(Config.ALLOW_FORCE_ENDTASK), config.ALLOW_FORCE_ENDTASK },
                    { nameof(Config.LIVE_CAPTURE_CHECK_FREQUENT), config.LIVE_CAPTURE_CHECK_FREQUENT },
                    { nameof(Config.SOUND_DETECT), config.SOUND_DETECT }
                };

                foreach (var pair in configValues)
                {
                    AppConfigHelper.WriteAppConfig(pair.Key, pair.Value);
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"An error occurred while writing default values to App.config: {ex.Message}");
            }
        }
        /// <summary>
        /// Parses the configuration from a JSON string.
        /// </summary>
        /// <param name="configContent">The JSON string containing the configuration.</param>
        /// <returns>A Config object if parsing is successful, otherwise null.</returns>
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