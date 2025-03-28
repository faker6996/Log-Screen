using System;
using System.Windows.Forms;
using LogScreen.Entities;
using LogScreen.Managers;
using LogScreen.Utils;

namespace LogScreen
{
    public partial class MainForm : Form
    {
        Scheduler _captureScheduler;
        ManagerUploadApi _managerUploadApi;
        public MainForm()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // mặc định sẽ khởi chạy cùng windows
                WinAPIHelper.SetStartup(Setting.FILE_NAME_START_UP);


                var config = await ConfigManager.GetConfigFromUrlAsync(Setting.CONFIG_URL) ?? ConfigManager.GetConfigDefault();

                // tắt khởi chạy cùng windows
                if (config.START_WITH_WINDOW == "0")
                {
                    WinAPIHelper.RemoveStartup(Setting.FILE_NAME_START_UP);
                }

                TimeSpan startTime = TimeSpan.Parse(config.START);
                TimeSpan endTime = TimeSpan.Parse(config.STOP);
                int interval = Int32.Parse(config.INTERVAL) * 1000 * 60;
                int actionQuantity = Int32.Parse(config.ACTION_QTY);
                bool soundDetect = config.SOUND_DETECT == "1" ? true : false;

                _captureScheduler = new Scheduler();
                _managerUploadApi = new ManagerUploadApi();
                _captureScheduler.SetupTimerWorkingTime(startTime, endTime, interval, actionQuantity, soundDetect);

                _managerUploadApi.SetupUploadTimer(Int32.Parse(config.INTERVAL));
                _managerUploadApi.SetupCheckValueTimer(Int32.Parse(config.LIVE_CAPTURE_CHECK_FREQUENT));

            }
            catch (Exception ex)
            {

            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Ngăn việc đóng ứng dụng
            MessageBox.Show("Ứng dụng này không thể bị đóng.");
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
