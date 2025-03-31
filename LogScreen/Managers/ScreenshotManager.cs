using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
                var savedScreenshots = new List<string>();
                var screens = Screen.AllScreens;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH'h'mm-ss");
                FileHelper.CreateMonitoringAddress();
                string captureDir = FileHelper.GetCaptureAddress();

                var jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                using (var encoderParams = new EncoderParameters(1))
                {
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);

                    for (int i = 0; i < screens.Length; i++)
                    {
                        Rectangle bounds = screens[i].Bounds;
                        using (var screenshot = new Bitmap(bounds.Width, bounds.Height))
                        using (var graphics = Graphics.FromImage(screenshot))
                        {
                            graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);

                            if (soundDetect)
                            {
                                string audioInfo = SoundHelper.GetActiveAudioTab();
                                if (!string.IsNullOrEmpty(audioInfo))
                                {
                                    AddWatermark(graphics, audioInfo, bounds.Width);
                                }
                            }

                            string filePath = Path.Combine(captureDir, $"{timestamp}_{i + 1}.jpg");
                            screenshot.Save(filePath, jpgEncoder, encoderParams);
                            savedScreenshots.Add(filePath);
                        }
                    }
                }
                return savedScreenshots;
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Adds a watermark text to the top-right corner of an image.
        /// </summary>
        /// <param name="graphics">Graphics object used to draw the watermark.</param>
        /// <param name="watermarkText">The text to be used as the watermark.</param>
        /// <param name="screenWidth">The width of the screen to determine the positioning.</param>

        private void AddWatermark(Graphics graphics, string watermarkText, int screenWidth)
        {
            using (Font font = new Font("Arial", 12, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.White))
            {
                SizeF textSize = graphics.MeasureString(watermarkText, font);
                float x = screenWidth - textSize.Width - 10;
                float y = 10;

                using (Brush bgBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                {
                    graphics.FillRectangle(bgBrush, x - 5, y - 5, textSize.Width + 10, textSize.Height + 10);
                }
                graphics.DrawString(watermarkText, font, brush, x, y);
            }
        }

        /// <summary>
        /// Retrieves the image encoder for the specified image format.
        /// </summary>
        /// <param name="format">The image format to find the corresponding encoder for.</param>
        /// <returns>
        /// An <see cref="ImageCodecInfo"/> object representing the encoder for the specified format,
        /// or null if no matching encoder is found.
        /// </returns>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (var codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve a list of all saved image file paths
        /// </summary>
        /// <returns>List of image file paths</returns>
        public List<string> GetAllSavedScreenshots()
        {
            try
            {
                string captureDirectory = FileHelper.GetCaptureAddress();
                return Directory.Exists(captureDirectory)
                    ? new List<string>(Directory.GetFiles(captureDirectory, "*.jpg"))
                    : new List<string>();
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error retrieving image list: {ex.Message}\n{ex.StackTrace}");
                return new List<string>();
            }
        }
    }
}
