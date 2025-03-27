using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using NAudio.CoreAudioApi;

namespace LogScreen.Utils
{
    public static class SoundHelper
    {
        public static string DetectSound()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                var sessions = device.AudioSessionManager.Sessions;

                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    float peakValue = session.AudioMeterInformation.MasterPeakValue;

                    if (peakValue > 0.001f) // Phát hiện âm thanh nhỏ
                    {
                        var processId = (int)session.GetProcessID;
                        try
                        {
                            var process = Process.GetProcessById(processId);
                            string windowTitle = GetWindowTitle(process.MainWindowHandle);
                            string processName = process.ProcessName.ToLower();

                            // Xử lý trường hợp Chrome
                            if (processName.Contains("chrome"))
                            {
                                string tabTitle = GetChromeTabTitle(process);
                                return $"chrome | Sound: \"{tabTitle}\"";
                            }
                            // Các ứng dụng khác
                            else if (string.IsNullOrEmpty(windowTitle))
                            {
                                return $"{processName} | Sound: \"{processName}\"";
                            }
                            else
                            {
                                return $"{processName} | Sound: \"{windowTitle}\"";
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

        // WinAPI để lấy tiêu đề cửa sổ
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        private static string GetWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return "Unknown";
            System.Text.StringBuilder title = new System.Text.StringBuilder(256);
            GetWindowText(hWnd, title, title.Capacity);
            string result = title.ToString();
            return string.IsNullOrEmpty(result) ? "Unknown" : result;
        }

        // Hàm cố gắng lấy tiêu đề tab của Chrome
        private static string GetChromeTabTitle(Process process)
        {
            string windowTitle = GetWindowTitle(process.MainWindowHandle);
            if (!string.IsNullOrEmpty(windowTitle) && windowTitle != "Unknown")
            {
                // Chrome thường hiển thị tiêu đề dạng "Tên tab - Google Chrome"
                if (windowTitle.Contains(" - Google Chrome"))
                {
                    return windowTitle.Replace(" - Google Chrome", "");
                }
                return windowTitle;
            }
            return "Chrome (Unknown Tab)";
        }
    }
}
