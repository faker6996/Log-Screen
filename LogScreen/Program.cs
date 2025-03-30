using System;
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
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LogScreenApplicationContext());
        }
    }
}