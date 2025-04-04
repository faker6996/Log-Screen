using System;
using System.Drawing;
using System.Windows.Forms;
using Monitoring.Entities;
using Monitoring.Managers;
using Monitoring.Utils;

namespace Monitoring
{
    public partial class MainForm : Form
    {
        SchedulerManager _captureScheduler;
        UploadApiManager _managerUploadApi;
        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }
        private async void Initialize()
        {
            try
            {
                // By default, monitoring app will start with Windows
                WinAPIHelper.SetStartup(Setting.FILE_NAME_START_UP);
                var config = await ConfigManager.GetConfigFromUrlAsync(Setting.CONFIG_URL) ?? ConfigManager.GetDefaultConfig();

                // Turn of run monitoring app when start window
                if (config.START_WITH_WINDOW == "0")
                {
                    WinAPIHelper.RemoveStartup(Setting.FILE_NAME_START_UP);
                }

                // Create a context menu for the NotifyIcon
                IconHelper.InitIcon(config);

                _captureScheduler = new SchedulerManager(config);
                _captureScheduler.SetupTimerWorkingTime();

                _managerUploadApi = new UploadApiManager(config);
                _managerUploadApi.SetupUploadTimer(Int32.Parse(config.INTERVAL));
                _managerUploadApi.SetupCheckValueTimer(Int32.Parse(config.LIVE_CAPTURE_CHECK_FREQUENT));

            }
            catch (Exception ex)
            {
                FileHelper.LogError($"{ex}");
            }
        }
    }
}
