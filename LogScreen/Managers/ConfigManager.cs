
using System.Net.Http;
using System.Threading.Tasks;

namespace LogScreen.Managers
{
    public class ConfigManager
    {
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public int Interval { get; set; }
        public int ActionQty { get; set; }
        public bool StartWithWindows { get; set; }
        public bool AllowForceEndTask { get; set; }
        public int LiveCaptureCheck { get; set; }
        public bool SoundDetect { get; set; }
        public string FtpServer { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }

        public async Task LoadConfig(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var lines = response.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("START=")) StartTime = line.Split('=')[1].Trim();
                    if (line.StartsWith("STOP=")) StopTime = line.Split('=')[1].Trim();
                    if (line.StartsWith("INTERVAL=")) Interval = int.Parse(line.Split('=')[1].Trim());
                    if (line.StartsWith("ACTION_QTY=")) ActionQty = int.Parse(line.Split('=')[1].Trim());
                    if (line.StartsWith("START_WITH_WINDOW=")) StartWithWindows = line.Split('=')[1].Trim() == "1";
                    if (line.StartsWith("ALLOW_FORCE_ENDTASK=")) AllowForceEndTask = line.Split('=')[1].Trim() == "1";
                    if (line.StartsWith("LIVE_CAPTURE_CHECK_FREQUENT=")) LiveCaptureCheck = int.Parse(line.Split('=')[1].Trim());
                    if (line.StartsWith("SOUND_DETECT=")) SoundDetect = line.Split('=')[1].Trim() == "1";
                    if (line.StartsWith("FTP_SERVER=")) FtpServer = line.Split('=')[1].Trim();
                    if (line.StartsWith("FTP_USERNAME=")) FtpUsername = line.Split('=')[1].Trim();
                    if (line.StartsWith("FTP_PASSWORD=")) FtpPassword = line.Split('=')[1].Trim();
                }
            }
        }
    }
}



