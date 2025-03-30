using Microsoft.Win32;
using System.IO;
using System;

namespace LogScreen.Utils
{
    public class WinAPIHelper
    {
        /// <summary>
        /// Adds the application to the startup list with Windows.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        public static void SetStartup(string appName = "Monitoring")
        {
            // Find the .exe file path of the Monitoring application
            var exePath = WinAPIHelper.FindClickOnceExe(Setting.APP_NAME_EXE);

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
                    // Add the application startup entry
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

        /// <summary>
        /// Searches for the .exe file in the ClickOnce installation directory.
        /// </summary>
        /// <param name="appName">The name of the application executable.</param>
        /// <returns>The full path of the .exe file if found; otherwise, null.</returns>
        public static string FindClickOnceExe(string appName)
        {
            string clickOnceRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps", "2.0");

            if (!Directory.Exists(clickOnceRoot))
            {
                FileHelper.LogError("ClickOnce directory not found.");
                return null;
            }

            try
            {
                // Iterate through all subdirectories
                string[] directories = Directory.GetDirectories(clickOnceRoot, "*", SearchOption.AllDirectories);

                foreach (string dir in directories)
                {
                    try
                    {
                        // Retrieve the list of .exe files in the current directory
                        string[] exeFiles = Directory.GetFiles(dir, "*.exe", SearchOption.TopDirectoryOnly);
                        foreach (string exe in exeFiles)
                        {
                            // Check if the file name matches
                            if (Path.GetFileName(exe).Equals(appName, StringComparison.OrdinalIgnoreCase))
                            {
                                // Return the .exe file path
                                return exe;
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip directories without access permissions
                        FileHelper.LogError($"Unable to access directory: {dir}. Skipping.");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"An error occurred while searching for the .exe file: {ex.Message}");
            }

            FileHelper.LogError($"Could not find the file {appName} in the ClickOnce directory.");
            return null;
        }
    }
}