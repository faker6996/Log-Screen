using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LogScreen.Entities;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public class UploadApiManager
    {
        private Timer _uploadTimer; //This timer is responsible for scanning the folder containing images.After each cycle, it uploads the images to the API
        private Timer _checkValueTimer;// This timer is responsible for checking values in a text file to trigger screenshot capture and upload them to the API.
        private bool _soundDetect;

        public UploadApiManager(Config config)
        {
            _soundDetect = config.SOUND_DETECT == "1" ? true : false;
        }

        #region CheckValue Timers

        /// <summary>
        /// set_up timer upload 
        /// </summary>
        public void SetupCheckValueTimer(int liveCapture)
        {
            if (_checkValueTimer == null)
            {
                _checkValueTimer = new Timer();
            }
            _checkValueTimer.Interval = liveCapture * 1000; 
            _checkValueTimer.Tick += CheckValueTimer_Tick;
            CheckValueTimer_Tick(this,EventArgs.Empty);
            _checkValueTimer.Start(); 
        }

        /// <summary>
        /// This function checks a text file on the FTP server.
        ///     - value: 0 -> Do nothing.
        ///     - value: 1 -> Capture a screenshot, upload it to the API, and update the value back to 0.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private async void CheckValueTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var apiUploader = new APIUploader();
                var value = await apiUploader.GetCheckTimer(FileHelper.GetWindowsId(), Setting.API_CHECK, Setting.TOKEN, Setting.MODE_CHECK_TIMER.GET);

                if (value == 1)
                {
                    ScreenshotManager screenshotManager = new ScreenshotManager();
                    var listImg = screenshotManager.CaptureAndSaveAllScreens(_soundDetect);

                    if (listImg != null && listImg.Count != 0)
                    {
                        await apiUploader.UploadFileAsync(listImg, FileHelper.GetWindowsId(), Setting.API_UPLOAD, Setting.TOKEN);
                    }
                    await apiUploader.SetCheckTimer(FileHelper.GetWindowsId(), Setting.API_CHECK, Setting.TOKEN, Setting.MODE_CHECK_TIMER.SET, 0);
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error while scanning and processing images: {ex.Message}");
            }
        }
        #endregion

        #region Upload Timers

        /// <summary>
        /// set_up timer upload 
        /// </summary>
        public void SetupUploadTimer(int interval)
        {
            if (_uploadTimer == null)
            {
                _uploadTimer = new Timer();
            }
            _uploadTimer.Interval = interval * 60 * 1000; 
            _uploadTimer.Tick += UploadTimer_Tick;
            UploadTimer_Tick(this,EventArgs.Empty);
            _uploadTimer.Start(); 
        }

        /// <summary>
        /// Handles the event of uploading to the API.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ScreenshotManager screenshotManager = new ScreenshotManager();
                List<string> savedScreenshots = screenshotManager.GetAllSavedScreenshots();

                if (savedScreenshots.Count > 0)
                {
                    var apiUploader = new APIUploader();
                    await apiUploader.UploadFileAsync(savedScreenshots, FileHelper.GetWindowsId(), Setting.API_UPLOAD, Setting.TOKEN);

                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error when scan and process image: {ex.Message}");
            }
        }

        #endregion
    }
}