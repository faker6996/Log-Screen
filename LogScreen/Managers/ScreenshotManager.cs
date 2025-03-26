
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public static class ScreenshotManager
    {
        public static void CaptureAndSaveAllScreens()
        {
            try
            {
                // Lấy danh sách tất cả màn hình
                Screen[] screens = Screen.AllScreens;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH'h'mm-ss");
                FileHelper.CreateCaptureAddress(Setting.SCREEN_LOG_ADDRESS);
                // Chụp từng màn hình
                for (int i = 0; i < screens.Length; i++)
                {
                    Rectangle screenBounds = screens[i].Bounds;
                    using (Bitmap screenshot = new Bitmap(screenBounds.Width, screenBounds.Height))
                    {
                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            graphics.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, screenBounds.Size);
                        }

                        // Tạo tên file với suffix (_1, _2, ...)
                        string filePath = Path.Combine(Setting.SCREEN_LOG_ADDRESS, $"Screenshot_{timestamp}_{i + 1}.jpg");

                        // Lưu ảnh
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                        screenshot.Save(filePath, jpgEncoder, encoderParams);

                        // Cập nhật trạng thái
                        Console.WriteLine($"Saved: {Path.GetFileName(filePath)}");
                    }
                }

                if (screens.Length > 1)
                {
                    Console.WriteLine($" (Total: {screens.Length} screens)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}




