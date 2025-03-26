using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogScreen.Entities;
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
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                var config = await ConfigManager.GetConfigFromUrlAsync(Setting.CONFIG_URL);

                if (config != null)
                {
                    TimeSpan startTime = TimeSpan.Parse(config.START);
                    TimeSpan endTime = TimeSpan.Parse(config.STOP);
                    int interval = Int32.Parse(config.INTERVAL) * 1000;
                    int actionQuantity = Int32.Parse(config.ACTION_QTY);

                    _captureScheduler = new Scheduler();
                    _captureScheduler.SetupTimerWorkingTime(startTime, endTime, interval, actionQuantity);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_captureScheduler != null)
            {
                //_captureScheduler.DisposeTimerWorkingTime();
            }
            base.OnFormClosing(e);
        }
    }
}
