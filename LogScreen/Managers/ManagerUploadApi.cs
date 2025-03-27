using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public class ManagerUploadApi
    {

        /// <summary>
        /// Timer này phục vụ công việc quét folder chứa ảnh sau 1 chu kỳ sẽ đẩy ảnh lên API
        /// </summary>
        private Timer _uploadTimer;

        /// <summary>
        /// Timer này phục vụ công việc check value trong file txt để chụp ảnh đẩy lên API
        /// </summary>
        private Timer _checkValueTimer;

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
            _checkValueTimer.Interval = liveCapture * 1000; // 30s (ms)
            _checkValueTimer.Tick += CheckValueTimer_Tick; // Gắn sự kiện Tick
            _checkValueTimer.Start(); // Bắt đầu timer
        }

        /// <summary>
        ///  Hàm này check file text trên ftp
        ///     - value: 0 -> khong làm gì
        ///     - value: 1 -> chụp ảnh đẩy lên api và update value về 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckValueTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var apiUploader = new APIUploader();
                var value = await apiUploader.GetCheckTimer(FileHelper.GetWindowsId(), Setting.API_CHECK, Setting.TOKEN, Constant.ModeCheckTimer.Get);

                if (value == 1)
                {
                    //chụp ảnh
                    ScreenshotManager screenshotManager = new ScreenshotManager();
                    var listImg = screenshotManager.CaptureAndSaveAllScreens(true);

                    if (listImg != null && listImg.Count != 0)
                    {
                        // Upload các ảnh đã lưu lên API
                        await apiUploader.UploadFileAsync(listImg, FileHelper.GetWindowsId(), Setting.API_UPLOAD, Setting.TOKEN);
                    }

                    // thay đổi value về 0
                    await apiUploader.SetCheckTimer(FileHelper.GetWindowsId(), Setting.API_CHECK, Setting.TOKEN, Constant.ModeCheckTimer.Set, 0);

                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Lỗi khi quét và xử lý ảnh: {ex.Message}");
            }
        }

        #endregion

        #region Upload Timers

        /// <summary>
        /// set_up timer upload 
        /// </summary>
        public void SetupUploadTimer()
        {
            if (_uploadTimer == null)
            {
                _uploadTimer = new Timer();
            }
            _uploadTimer.Interval = 1 * 60 * 1000; // 30 phút (ms)
            _uploadTimer.Tick += UploadTimer_Tick; // Gắn sự kiện Tick
            _uploadTimer.Start(); // Bắt đầu timer
        }

        /// <summary>
        ///  xử lý sự kiện upload lên api
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UploadTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Bắt đầu quét ảnh...");
                ScreenshotManager screenshotManager = new ScreenshotManager();
                List<string> savedScreenshots = screenshotManager.GetAllSavedScreenshots();

                if (savedScreenshots.Count > 0)
                {
                    Console.WriteLine($"Tìm thấy {savedScreenshots.Count} ảnh cần xử lý:");
                    var apiUploader = new APIUploader();
                    await apiUploader.UploadFileAsync(savedScreenshots, FileHelper.GetWindowsId(), Setting.API_UPLOAD, Setting.TOKEN);

                }
                else
                {
                    Console.WriteLine("Không tìm thấy ảnh nào cần xử lý.");
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Lỗi khi quét và xử lý ảnh: {ex.Message}");
            }
        }

        #endregion
    }
}