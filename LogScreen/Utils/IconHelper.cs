using System.Drawing;
using System.Windows.Forms;
using System;

namespace LogScreen.Utils
{
    public static class IconHelper
    {
        private static NotifyIcon _notifyIcon = new NotifyIcon()
        {
            Text = "Performance monitor"
        };
        public static void InitIcon(ContextMenuStrip menu)
        {
            _notifyIcon.Icon = Properties.Resources.darkIcon;
            _notifyIcon.ContextMenuStrip = menu;
            _notifyIcon.Visible = true;
        }
        public static void BlinkIcon()
        {
            _notifyIcon.Icon = Properties.Resources.lightIcon;

            Timer timer = new Timer();
            timer.Interval = 500; // 0.5 giây
            timer.Tick += (s, e) =>
            {
                // Khôi phục icon ban đầu
                _notifyIcon.Icon = Properties.Resources.darkIcon;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
    }
}
