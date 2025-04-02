using Microsoft.Win32;
using System.IO;
using System;
using System.Windows.Forms;

namespace LogScreen.Utils
{
    public class WinAPIHelper
    {
        /// <summary>
        /// Adds the application to the startup list with Windows.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        public static void SetStartup(string appName)
        {
            // Find the .exe file path of the Monitoring application
            var exePath = Application.ExecutablePath;

            if (exePath == null)
            {
                FileHelper.LogError("The application's .exe file was not found. Unable to add to startup.");
                return;
            }

            // Unlock the Registry for modifications
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key != null)
                {
                    // Remove any existing entry to avoid conflicts
                    key.DeleteValue(appName, false);

                    // Add the application startup entry again
                    key.SetValue(appName, exePath);
                }
                else
                {
                    FileHelper.LogError("Unable to access the Registry. Please check Administrator permissions.");
                }
            }
        }

        /// <summary>
        /// Remove the application from the Windows startup list
        /// </summary>
        /// <param name="appName">The name of the application.</param>

        public static void RemoveStartup(string appName)
        {
            // Unlock the Registry for modifications
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (key != null)
                {
                    if (key.GetValue(appName) != null)
                    {
                        key.DeleteValue(appName);
                    }
                    else
                    {
                        FileHelper.LogError($"Could not find {appName} in the startup list.");
                    }
                }
                else
                {
                    FileHelper.LogError("Unable to access the Registry. Please check Administrator permissions.");
                }
            }
        }
    }
}