using System.Configuration;


namespace Monitoring.Utils
{
    public static class AppConfigHelper
    {
        /// <summary>
        /// Reads a value from the App.config file.
        /// </summary>
        /// <param name="key">The key of the configuration setting.</param>
        /// <returns>The corresponding value as a string.</returns>
        public static string ReadAppConfig(string key)
        {
            // Check if the key exists in App.config
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key];
            }
            else
            {
                FileHelper.LogError($"Key '{key}' not found in App.config.");
                return null;
            }
        }

        /// <summary>
        /// Function to write or update a value in App.config
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The value to be set</param>
        public static void WriteAppConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value; 
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}