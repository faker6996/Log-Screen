
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogScreen.Utils;

namespace LogScreen.Managers
{
    public class ScreenshotManager
    {
        public List<string> CaptureAndSaveAllScreens(bool soundDetect)
        {
            try
            {

                // Khởi tạo danh sách lưu đường dẫn ảnh
                List<string> savedScreenshots = new List<string>();

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
                        // henry todo sound detect
                        if (soundDetect)
                        {
                            Console.WriteLine(SoundHelper.DetectSound()); //henry todo sound check
                        }


                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            graphics.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, screenBounds.Size);
                        }

                        // Tạo tên file với suffix (_1, _2, ...)
                        string filePath = Path.Combine(FileHelper.GetCaptureAddress(), $"{timestamp}_{i + 1}.jpg");

                        // Lưu ảnh
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                        screenshot.Save(filePath, jpgEncoder, encoderParams);

                        // Thêm đường dẫn ảnh vào danh sách
                        savedScreenshots.Add(filePath);

                        // Cập nhật trạng thái
                        Console.WriteLine($"Saved: {Path.GetFileName(filePath)}");
                    }
                }


                if (screens.Length > 1)
                {
                    Console.WriteLine($" (Total: {screens.Length} screens)");
                }
                return savedScreenshots;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
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

        /// <summary>
        /// Lấy danh sách tất cả các đường dẫn ảnh đã lưu
        /// </summary>
        /// <returns>Danh sách các đường dẫn ảnh</returns>
        public List<string> GetAllSavedScreenshots()
        {
            try
            {
                // Lấy đường dẫn thư mục lưu ảnh từ FileHelper
                string captureDirectory = FileHelper.GetCaptureAddress();

                // Kiểm tra nếu thư mục tồn tại
                if (Directory.Exists(captureDirectory))
                {
                    // Lấy danh sách tất cả các file .jpg
                    string[] files = Directory.GetFiles(captureDirectory, "*.jpg");

                    Console.WriteLine($"Tìm thấy {files.Length} ảnh đã lưu.");
                    return new List<string>(files); // Trả về danh sách file
                }
                else
                {
                    Console.WriteLine("Thư mục lưu ảnh không tồn tại.");
                    return new List<string>(); // Trả về danh sách trống
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Lỗi khi đọc danh sách ảnh: {ex.Message}");
                return new List<string>(); // Trả về danh sách trống khi xảy ra lỗi
            }

        }
    }
}





