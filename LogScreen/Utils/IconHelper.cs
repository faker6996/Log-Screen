using System.Drawing;
using System.Windows.Forms;
using System;
using LogScreen.Entities;

namespace LogScreen.Utils
{
    public static class IconHelper
    {
        /// <summary>
        /// Handles the exit event when the user selects "Exit" from the menu.
        /// </summary>
        private static void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // NotifyIcon object to display an icon in the system tray
        private static NotifyIcon _notifyIcon = new NotifyIcon()
        {
            Text = "Performance monitor"
        };

        /// <summary>
        /// Initializes and displays the tray icon, with an optional context menu.
        /// </summary>
        /// <param name="config">Application configuration to determine if the exit option is allowed.</param>
        public static void InitIcon(Config config)
        {
            var allowForceEndTask = config.ALLOW_FORCE_ENDTASK == "1" ? true : false;

            ContextMenuStrip menu = new ContextMenuStrip();

            if (allowForceEndTask)
            {
                menu.Items.Add("Thoát", null, Exit);
            }

            _notifyIcon.Icon = Properties.Resources.darkIcon;
            _notifyIcon.ContextMenuStrip = menu;
            _notifyIcon.Visible = true;
        }


        /// <summary>
        /// Creates a blinking effect on the tray icon to grab the user's attention.
        /// </summary>
        public static void BlinkIcon()
        {
            _notifyIcon.Icon = Properties.Resources.lightIcon;

            Timer timer = new Timer();
            timer.Interval = 1000; // 0.5 giây
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
