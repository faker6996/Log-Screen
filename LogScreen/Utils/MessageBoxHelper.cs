using System;
using System.Windows;

namespace LogScreen.Utils
{
    public static class MessageBoxHelper
    {
        private static bool isShowingMessageBoxToken = false;
        private static bool isShowingMessageBoxServerError = false;//

        public static void ShowMessageToken(object sender, EventArgs e)
        {
            try
            {
                if (isShowingMessageBoxToken) return;

                isShowingMessageBoxToken = true;

                System.Windows.MessageBox.Show(Setting.MESSAGE_TOKEN_INVALID, Setting.MESSAGE_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);

                isShowingMessageBoxToken = false;
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error when showing message: {ex.Message}");
                isShowingMessageBoxToken = false;
            }
        }
        public static void ShowMessageServerError(string message, string title)
        {
            try
            {
                if (isShowingMessageBoxServerError) return;

                isShowingMessageBoxServerError = true;


                System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

                isShowingMessageBoxServerError = false;
            }
            catch (Exception ex)
            {
                FileHelper.LogError($"Error when showing message: {ex.Message}");
                isShowingMessageBoxServerError = false;
            }
        }
    }
}