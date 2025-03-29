using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogScreen.Utils;

namespace LogScreen
{
    public class MyApplicationContext : ApplicationContext
    {

        public MyApplicationContext()
        {
            // Tạo menu chuột phải cho NotifyIcon
            ContextMenuStrip menu = new ContextMenuStrip();
            //menu.Items.Add("Mở ứng dụng", null, ShowForm);

            //Henry todo: kiểm tra xem config có cho thoát hay không ?
            menu.Items.Add("Thoát", null, Exit);

            // Tạo NotifyIcon
            IconHelper.InitIcon(menu);

            // Ẩn form (không cần hiển thị cửa sổ chính)
            MainForm form = new MainForm();
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }


    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyApplicationContext());
        }

    }
}
