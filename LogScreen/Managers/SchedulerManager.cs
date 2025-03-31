using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogScreen.Entities;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public class SchedulerManager
    {
        private Timer _captureTimer;
        private Timer _workingTimer;
        private TimeSpan _startTime;
        private TimeSpan _endTime;
        private int _interval;
        private int _actionQuantity;
        private bool _soundDetect;

        private int _totalDuration; // milliseconds (time for each cycle)
        private int _captureCount;  // number of captures per cycle
        private Random _random;
        private List<int> _captureTimes; // List of capture timestamps
        private int _elapsedTime; // Elapsed time in the current cycle
        private bool _isCapturing; // Prevent multiple captures at the same time

        /// <summary>
        /// Initializes a new instance of the SchedulerManager class with a given configuration.
        /// </summary>
        /// <param name="config">The configuration object containing scheduling parameters.</param>
        public SchedulerManager(Config config)
        {
            _random = new Random();
            _isCapturing = false;

            _startTime = TimeSpan.Parse(config.START);
            _endTime = TimeSpan.Parse(config.STOP);
            _interval = Int32.Parse(config.INTERVAL) * 1000 * 60;
            _actionQuantity = Int32.Parse(config.ACTION_QTY);
            _soundDetect = config.SOUND_DETECT == "1" ? true : false;
        }

        /// <summary>
        /// Sets up a timer to check the working time periodically and start/stop capturing accordingly.
        /// </summary>
        public void SetupTimerWorkingTime()
        {
            if (_workingTimer == null)
            {
                _workingTimer = new Timer();
            }
            _workingTimer.Interval = Setting.WORKING_TIMER_INTERVAL;
            _workingTimer.Tick += WorkingTimer_Tick;
            WorkingTimer_Tick(this, EventArgs.Empty);
            _workingTimer.Start();
        }

        /// <summary>
        /// Sets up a random capture schedule within a given duration and number of captures.
        /// </summary>
        /// <param name="durationMs">Total duration for captures in milliseconds.</param>
        /// <param name="numberOfCaptures">Number of captures to take within the duration.</param>
        private void SetupRandomCapture(int durationMs, int numberOfCaptures)
        {
            _totalDuration = durationMs;
            _captureCount = numberOfCaptures;
            _elapsedTime = 0;

            if (_captureTimer == null)
            {
                _captureTimer = new Timer();
            }

            _captureTimer.Interval = 100;
            _captureTimer.Tick += CaptureTimer_Tick;

            ResetCaptureTimes();
            CaptureTimer_Tick(this, EventArgs.Empty);
            _captureTimer.Start();
        }

        /// <summary>
        /// Handles the capture timer tick event, triggering captures at the scheduled times.
        /// </summary>
        private async void CaptureTimer_Tick(object sender, EventArgs e)
        {
            _elapsedTime += _captureTimer.Interval;

            // Capture only if no other capture task is running
            if (_captureTimes.Count > 0 && _elapsedTime >= _captureTimes[0] && !_isCapturing)
            {
                _isCapturing = true;
                try
                {
                    // Run the screenshot task in a separate thread
                    await Task.Run(() =>
                    {
                        ScreenshotManager screenshotManager = new ScreenshotManager();
                        screenshotManager.CaptureAndSaveAllScreens(_soundDetect);
                    });

                    _captureTimes.RemoveAt(0); // Remove the captured timestamp
                }
                finally
                {
                    _isCapturing = false; // Mark as completed
                    IconHelper.BlinkIcon();
                }
            }

            // Reset cycle when time is up
            if (_elapsedTime >= _totalDuration)
            {
                ResetCaptureTimes();
                _elapsedTime = 0;
            }
        }

        /// <summary>
        /// Resets the capture timestamps for the next cycle.
        /// </summary>
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

        /// <summary>
        /// Handles the working timer tick event to check whether capturing should start or stop.
        /// </summary>
        private void WorkingTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            TimeSpan currentTime = now.TimeOfDay;

            bool isOnActivePeriod = (_startTime < _endTime && currentTime >= _startTime && currentTime <= _endTime) ||
                                    (_startTime > _endTime && (currentTime >= _startTime || currentTime <= _endTime));

            if (isOnActivePeriod)
            {
                StartCaptureIfNecessary();
            }
            else
            {
                StopCaptureIfNecessary();
            }
        }

        /// <summary>
        /// Starts capturing if it is necessary based on the defined schedule.
        /// </summary>
        private void StartCaptureIfNecessary()
        {
            if ((_captureTimer == null || !_captureTimer.Enabled) && _interval > 0)
            {
                SetupRandomCapture(_interval, _actionQuantity);
            }
        }

        /// <summary>
        /// Stops capturing if it is currently active.
        /// </summary>
        private void StopCaptureIfNecessary()
        {
            if (_captureTimer != null && _captureTimer.Enabled)
            {
                _captureTimer.Stop();
            }
        }

    }
}
