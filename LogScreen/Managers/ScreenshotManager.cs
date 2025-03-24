
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LogScreen.Managers
{
    public class ScreenshotManager
    {
        public void CaptureScreens(string folderPath, int screenId)
        {
            foreach (var screen in Screen.AllScreens)
            {
                var bounds = screen.Bounds;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }
                    string fileName = $"{folderPath}\\{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{screenId}.jpg";
                    bitmap.Save(fileName);
                }
            }
        }
    }
}




