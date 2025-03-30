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
                // Initialize a list to store image file paths
                List<string> savedScreenshots = new List<string>();

                // Get a list of all screens
                Screen[] screens = Screen.AllScreens;
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH'h'mm-ss");
                FileHelper.CreateMonitoringAddress();

                // Capture each screen
                for (int i = 0; i < screens.Length; i++)
                {
                    Rectangle screenBounds = screens[i].Bounds;
                    using (Bitmap screenshot = new Bitmap(screenBounds.Width, screenBounds.Height))
                    {
                        using (Graphics graphics = Graphics.FromImage(screenshot))
                        {
                            // Capture the screen
                            graphics.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, screenBounds.Size);

                            // Check for sound activity and add a watermark if needed
                            if (soundDetect)
                            {
                                string audioInfo = SoundHelper.GetActiveAudioTab();
                                if (!string.IsNullOrEmpty(audioInfo))
                                {
                                    // Add watermark with audioInfo in the top-right corner
                                    AddWatermark(graphics, audioInfo, screenBounds.Width);
                                }
                            }
                        }

                        // Create a filename with suffix (_1, _2, ...)
                        string filePath = Path.Combine(FileHelper.GetCaptureAddress(), $"{timestamp}_{i + 1}.jpg");

                        // Save the image
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                        screenshot.Save(filePath, jpgEncoder, encoderParams);

                        // Add the image path to the list
                        savedScreenshots.Add(filePath);
                    }
                }
                return savedScreenshots;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
            Font font = new Font("Arial", 12, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            SizeF textSize = graphics.MeasureString(watermarkText, font);

            // Calculate the position for the top-right corner (10px margin from the right and top)
            float x = screenWidth - textSize.Width - 10;
            float y = 10; // Adjust this to place it at the bottom if needed

            // Add a semi-transparent background for readability
            RectangleF background = new RectangleF(x - 5, y - 5, textSize.Width + 10, textSize.Height + 10);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), background);

            // Draw the watermark
            graphics.DrawString(watermarkText, font, brush, x, y);
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
        /// Retrieve a list of all saved image file paths
        /// </summary>
        /// <returns>List of image file paths</returns>
        public List<string> GetAllSavedScreenshots()
        {
            try
            {
                // Get the image storage directory path from FileHelper
                string captureDirectory = FileHelper.GetCaptureAddress();

                // Check if the directory exists
                if (Directory.Exists(captureDirectory))
                {
                    // Get all .jpg files in the directory
                    string[] files = Directory.GetFiles(captureDirectory, "*.jpg");

                    return new List<string>(files); // Return the list of image files
                }
                else
                {
                    FileHelper.LogError("The image storage directory does not exist.");
                    return new List<string>(); // Return an empty list
                }
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error retrieving image list: {ex.Message}");
                return new List<string>(); // Return an empty list in case of an error
            }
        }
    }
}
