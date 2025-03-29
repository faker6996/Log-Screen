using System;
using System.Drawing;
using System.Windows.Forms;
using LogScreen.Managers;
using LogScreen.Utils;

namespace LogScreen
{
    public partial class MainForm : Form
    {
        Scheduler _captureScheduler;
        public MainForm()
        {
            InitializeComponent();
            // Gán icon mặc định
            Initialize();

        }
        private async void Initialize()
        {
            try
            {
                var config = await ConfigManager.GetConfigFromUrlAsync(Setting.CONFIG_URL);

                if (config != null)
                {
                    TimeSpan startTime = TimeSpan.Parse(config.START);
                    TimeSpan endTime = TimeSpan.Parse(config.STOP);
                    int interval = Int32.Parse(config.INTERVAL) * 1000 * 60;
                    int actionQuantity = Int32.Parse(config.ACTION_QTY);
                    bool soundDetect = config.SOUND_DETECT == "1" ? true : false;

                    _captureScheduler = new Scheduler();
                    _captureScheduler.SetupTimerWorkingTime(startTime, endTime, interval, actionQuantity, soundDetect);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
