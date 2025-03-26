
using System;
using System.Collections.Generic;
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

        private int _totalDuration; // m giây (thời gian mỗi chu kỳ)
        private int _captureCount;  // n lần in "đã chụp"
        private Random _random;
        private List<int> _captureTimes; // Danh sách thời điểm chụp
        private int _elapsedTime; // Thời gian đã trôi qua trong chu kỳ hiện tại

        public Scheduler()
        {
            _random = new Random();
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

            _captureTimer.Interval = 100; // Kiểm tra mỗi 100ms để tăng độ chính xác
            _captureTimer.Tick += CaptureTimer_Tick;

            // Khởi tạo chu kỳ đầu tiên
            ResetCaptureTimes();
            _captureTimer.Start();
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            _elapsedTime += _captureTimer.Interval;

            // Kiểm tra và chụp tại các thời điểm ngẫu nhiên
            while (_captureTimes.Count > 0 && _elapsedTime >= _captureTimes[0])
            {
                ScreenshotManager.CaptureAndSaveAllScreens();
                _captureTimes.RemoveAt(0); // Xóa thời điểm đã chụp
            }

            // Khi hết chu kỳ, reset và bắt đầu lại
            if (_elapsedTime >= _totalDuration)
            {
                ResetCaptureTimes();
                _elapsedTime = 0; // Reset thời gian cho chu kỳ mới
            }
        }

        private void ResetCaptureTimes()
        {
            _captureTimes = new List<int>();

            // Sinh thời điểm đầu tiên
            int firstTime = _random.Next(0, _totalDuration - (_captureCount - 1) * Setting.MIN_GAP_SCREEN_CAPTURE);
            _captureTimes.Add(firstTime);

            // Sinh các thời điểm tiếp theo với khoảng cách tối thiểu 1 giây
            for (int i = 1; i < _captureCount; i++)
            {
                int previousTime = _captureTimes[i - 1];
                int nextTime;

                // Đảm bảo thời điểm tiếp theo cách ít nhất 1 giây và không vượt quá _totalDuration
                do
                {
                    nextTime = previousTime + _random.Next(Setting.MIN_GAP_SCREEN_CAPTURE, _totalDuration - previousTime);
                } while (nextTime >= _totalDuration || _captureTimes.Contains(nextTime));

                _captureTimes.Add(nextTime);
            }

            _captureTimes.Sort(); // Sắp xếp để chụp theo thứ tự thời gian
        }

        public void SetupTimerWorkingTime(TimeSpan startTime, TimeSpan endTime, int interval, int actionQuantity)
        {
            _startTime = startTime;
            _endTime = endTime;
            _interval = interval;
            _actionQuantity = actionQuantity;

            _workingTimer = new Timer();
            _workingTimer.Interval = Setting.WORKING_TIMER_INTERVAL;
            _workingTimer.Tick += WorkingTimer_Tick;
            _workingTimer.Start();
        }

        private void WorkingTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now; ;
            // Kiểm tra nếu hiện tại trong khoảng startTime - endTime
            if (now.TimeOfDay >= _startTime && now.TimeOfDay <= _endTime)
            {
                if (_captureTimer == null)
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
    }
}








