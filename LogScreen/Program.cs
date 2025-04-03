using System;
using System.Threading;
using System.Windows.Forms;

namespace LogScreen
{
    public class LogScreenApplicationContext : ApplicationContext
    {
        private MainForm _mainForm;

        public LogScreenApplicationContext()
        {
            _mainForm = new MainForm();
            _mainForm.Visible = false;
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng hoàn toàn
        }
    }

    internal static class Program
    {
        static Mutex mutex;

        [STAThread]
        static void Main()
        {
            bool isNewInstance;
            mutex = new Mutex(true, "Global\\LogScreenMutex", out isNewInstance);

            if (!isNewInstance)
            {
                return; // Thoát ngay nếu ứng dụng đã chạy trước đó
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LogScreenApplicationContext());

            mutex.ReleaseMutex();
        }
    }
}