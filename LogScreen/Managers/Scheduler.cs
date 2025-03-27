using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public class Scheduler
    {
        private Timer _captureTimer;
        private Timer _workingTimer;
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private int _interval;
        private int _actionQuantity;
        private bool _soundDetect;

        private int _totalDuration; // m giây (thời gian mỗi chu kỳ)
        private int _captureCount;  // n lần chụp
        private Random _random;
        private List<int> _captureTimes; // Danh sách thời điểm chụp
        private int _elapsedTime; // Thời gian đã trôi qua trong chu kỳ hiện tại
        private bool _isCapturing; // Tránh chụp đồng thời nhiều lần

        public Scheduler()
        {
            _random = new Random();
            _isCapturing = false;
        }

        private void SetupRandomCapture(int durationMs, int numberOfCaptures)
        {
            _totalDuration = durationMs; // m giây
            _captureCount = numberOfCaptures; // n lần
            _elapsedTime = 0;

            if (_captureTimer == null)
            {
                _captureTimer = new Timer();
            }

            _captureTimer.Interval = 100; // Giữ 100ms để kiểm tra thời gian chính xác
            _captureTimer.Tick += CaptureTimer_Tick;

            ResetCaptureTimes();
            _captureTimer.Start();
        }

        private async void CaptureTimer_Tick(object sender, EventArgs e)
        {
            _elapsedTime += _captureTimer.Interval;

            // Chỉ chụp nếu không có tác vụ chụp nào đang chạy
            if (_captureTimes.Count > 0 && _elapsedTime >= _captureTimes[0] && !_isCapturing)
            {
                _isCapturing = true;
                try
                {
                    // Chuyển tác vụ chụp màn hình sang luồng riêng
                    await Task.Run(() =>
                    {
                        ScreenshotManager screenshotManager = new ScreenshotManager();
                        screenshotManager.CaptureAndSaveAllScreens(_soundDetect);
                    });

                    _captureTimes.RemoveAt(0); // Xóa thời điểm đã chụp
                }
                finally
                {
                    _isCapturing = false; // Đánh dấu hoàn thành
                }
            }

            // Reset chu kỳ khi hết thời gian
            if (_elapsedTime >= _totalDuration)
            {
                ResetCaptureTimes();
                _elapsedTime = 0;
            }
        }

        private void ResetCaptureTimes()
        {
            _captureTimes = new List<int>();

            int nextTime = 0;
            for (int i = 0; i < _captureCount; i++)
            {
                 nextTime = nextTime + _random.Next(_totalDuration / _actionQuantity);
                _captureTimes.Add(nextTime);
                Console.WriteLine($"i = {i}, nextTime = {nextTime}");
            }
        }

        public void SetupTimerWorkingTime(TimeSpan startTime, TimeSpan endTime, int interval, int actionQuantity, bool soundDetect)
        {
            _startTime = startTime;
            _endTime = endTime;
            _interval = interval;
            _actionQuantity = actionQuantity;
            _soundDetect = soundDetect;

            if (_workingTimer == null)
            {
                _workingTimer = new Timer();
            }
            _workingTimer.Interval = Setting.WORKING_TIMER_INTERVAL;
            _workingTimer.Tick += WorkingTimer_Tick;
            WorkingTimer_Tick(this, EventArgs.Empty);
            _workingTimer.Start();
        }

        private void WorkingTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.TimeOfDay >= _startTime && now.TimeOfDay <= _endTime)
            {
                if (_captureTimer == null || !_captureTimer.Enabled)
                {
                    if (_interval > 0)
                    {
                        SetupRandomCapture(_interval, _actionQuantity);
                    }
                }
            }
            else
            {
                if (_captureTimer != null && _captureTimer.Enabled)
                {
                    _captureTimer.Stop();
                }
            }
        }

        // Dừng tất cả timer khi cần
        public void Stop()
        {
            _captureTimer?.Stop();
            _workingTimer?.Stop();
        }
    }
}