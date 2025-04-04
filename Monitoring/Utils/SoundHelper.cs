using System;
using System.Diagnostics;
using System.Windows.Automation;
using NAudio.CoreAudioApi;
using System.Runtime.InteropServices;

namespace Monitoring.Utils
{
    public class SoundHelper
    {
        // WinAPI để liệt kê cửa sổ
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// Detects the active browser tab that is currently playing audio.
        /// </summary>
        /// <returns>A string containing the browser name and the tab title, or null if no active tab is found.</returns>
        public static string GetActiveAudioTab()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                var sessions = device.AudioSessionManager.Sessions;

                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    float peakValue = session.AudioMeterInformation.MasterPeakValue;

                    if (peakValue > 0.001f) // Phát hiện âm thanh
                    {
                        var processId = (int)session.GetProcessID;
                        try
                        {
                            var process = Process.GetProcessById(processId);
                            string processName = process.ProcessName.ToLower();

                            if (processName.Contains("chrome") || processName.Contains("msedge") || processName.Contains("firefox"))
                            {
                                string tabTitle = GetBrowserTabWithSound(process);
                                if (!string.IsNullOrEmpty(tabTitle))
                                {
                                    return $"{processName} | Sound: \"{tabTitle}\"";
                                }
                                return $"{processName} | Sound: \"Unknown Tab\"";
                            }
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the title of the browser tab currently playing audio.
        /// </summary>
        /// <param name="process">The process of the browser.</param>
        /// <returns>The tab title if found, otherwise null.</returns>
        private static string GetBrowserTabWithSound(Process process)
        {
            try
            {
                // Kiểm tra MainWindowHandle
                IntPtr windowHandle = process.MainWindowHandle;
                if (windowHandle == IntPtr.Zero)
                {
                    // Nếu MainWindowHandle không hợp lệ, tìm cửa sổ chính của trình duyệt
                    windowHandle = FindBrowserMainWindow(process.Id);
                    if (windowHandle == IntPtr.Zero)
                    {
                        Console.WriteLine("No valid browser window found.");
                        return null;
                    }
                }

                AutomationElement browserWindow = AutomationElement.FromHandle(windowHandle);
                if (browserWindow == null) return null;

                Condition tabCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
                AutomationElementCollection tabs = browserWindow.FindAll(TreeScope.Descendants, tabCondition);

                foreach (AutomationElement tab in tabs)
                {
                    string tabName = tab.Current.Name;
                    if (!string.IsNullOrEmpty(tabName))
                    {
                        AutomationElement soundIndicator = tab.FindFirst(TreeScope.Children,
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Image));

                        if (soundIndicator != null || tab.Current.IsKeyboardFocusable)
                        {
                            return tabName;
                        }
                    }
                }

                return browserWindow.Current.Name;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting tab: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Finds the main window handle of a given browser process.
        /// </summary>
        /// <param name="processId">The process ID of the browser.</param>
        /// <returns>The main window handle if found, otherwise IntPtr.Zero.</returns>
        private static IntPtr FindBrowserMainWindow(int processId)
        {
            IntPtr foundHandle = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint windowProcessId);
                if (windowProcessId == processId)
                {
                    foundHandle = hWnd;
                    return false; // Dừng liệt kê khi tìm thấy cửa sổ đầu tiên
                }
                return true;
            }, IntPtr.Zero);

            return foundHandle;
        }
    }
}