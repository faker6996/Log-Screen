using System.Collections.Generic;
using System.IO;

namespace LogScreen.Utils
{
    public static class Setting
    {
        public const string CONFIG_URL = "http://works.com.vn/request/sfsfd/sdf/config.php?authkey=44029402340230808408083080fsdfsf";
        public const string TOKEN = "a6d47a15-17f9-4824-b66d-42aa28023882";
        public const string API_UPLOAD = "https://works.com.vn/upload.php";
        public const string API_CHECK = "https://works.com.vn/check.php";
        public static string SCREEN_LOG_ADDRESS = Path.Combine(Path.GetTempPath(), "Monitoring");
        public const int WORKING_TIMER_INTERVAL = 60000;  // Khoảng cách giữa các lần kiểm tra xem có đang trong thời gian hoạt động theo config không
        public static string LOG_FILE_PATH = Path.Combine(Setting.SCREEN_LOG_ADDRESS, "MonitoringErrors.txt");
        public static class TRY_GET_CONFIG
        {
            public static int MAX_REP = 6;
            public static int INTERVAL = 10000;
        };


        /// <summary>
        /// FileName trong start up
        /// </summary>
        public static string FILE_NAME_START_UP = "Monitoring";
        public static string APP_NAME_EXE = "LogScreen.exe";
        public static string CAPTURE_FILE_EXTENTION = "*.jpg";

        public static class MODE_CHECK_TIMER
        {
            public const string GET = "get_value";
            public const string SET = "set_value";
            public static Dictionary<string, string> dctDesc = new Dictionary<string, string>()
            {
                { GET,          "get_value" },
                { SET,          "set_value" }
            };

        }
    }
}
