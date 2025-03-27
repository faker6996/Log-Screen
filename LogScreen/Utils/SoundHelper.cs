using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using NAudio.CoreAudioApi;

namespace LogScreen.Utils
{
    public static class SoundHelper
    {
        // Hàm phát hiện âm thanh và lấy tiêu đề cửa sổ
        public static string DetectSound()
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                var sessions = device.AudioSessionManager.Sessions;

                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    // Bỏ kiểm tra GetState, chỉ kiểm tra mức âm thanh
                    if (session.AudioMeterInformation.MasterPeakValue > 0) // Nếu có âm thanh
                    {
                        // Lấy thông tin tiến trình
                        var processId = (int)session.GetProcessID;
                        try
                        {
                            var process = Process.GetProcessById(processId);
                            string windowTitle = GetWindowTitle(process.MainWindowHandle);
                            return $"{process.ProcessName} | Sound: \"{windowTitle}\"";
                        }
                        catch (ArgumentException)
                        {
                            // Bỏ qua nếu process không còn tồn tại
                            continue;
                        }
                    }
                    else
                    {
                        return $"No sound!";
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
            System.Text.StringBuilder title = new System.Text.StringBuilder(256);
            GetWindowText(hWnd, title, title.Capacity);
            return title.ToString();
        }

    }
}
