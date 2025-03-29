using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public class ScreenshotManager
    {
        public void CaptureAndSaveAllScreens(bool soundDetect)
        {
            try
            {
                // Lấy danh sách tất cả màn hình
                Screen[] screens = Screen.AllScreens;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH'h'mm-ss");
                FileHelper.CreateMonitoringAddress();

                // Chụp từng màn hình
                for (int i = 0; i < screens.Length; i++)
                {
                    Rectangle screenBounds = screens[i].Bounds;
                    using (Bitmap screenshot = new Bitmap(screenBounds.Width, screenBounds.Height))
                    {
                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            // Chụp màn hình
                            graphics.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, screenBounds.Size);

                            // Kiểm tra âm thanh và thêm watermark nếu cần
                            if (soundDetect)
                            {
                                string audioInfo = SoundHelper.GetActiveAudioTab();
                                if (!string.IsNullOrEmpty(audioInfo))
                                {
                                    // Thêm watermark audioInfo ở góc phải
                                    AddWatermark(graphics, audioInfo, screenBounds.Width);
                                }
                            }
                        }

                        // Tạo tên file với suffix (_1, _2, ...)
                        string filePath = Path.Combine(FileHelper.GetCaptureAddress(), $"{timestamp}_{i + 1}.jpg");

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

        // Hàm thêm watermark vào góc phải
        private void AddWatermark(Graphics graphics, string watermarkText, int screenWidth)
        {
            Font font = new Font("Arial", 12, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            SizeF textSize = graphics.MeasureString(watermarkText, font);

            // Tính toán vị trí góc phải (cách lề phải và dưới 10px)
            float x = screenWidth - textSize.Width - 10;
            float y = 10; // Có thể điều chỉnh để đặt ở dưới nếu muốn

            // Thêm nền mờ để dễ đọc
            RectangleF background = new RectangleF(x - 5, y - 5, textSize.Width + 10, textSize.Height + 10);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), background);

            // Vẽ watermark
            graphics.DrawString(watermarkText, font, brush, x, y);
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