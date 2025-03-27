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
        public const int WORKING_TIMER_INTERVAL = 60000;  //HenryTodo: force 60000 - Khoảng cách giữa các lần kiểm tra xem có đang trong thời gian hoạt động theo config không
        public const int MIN_GAP_SCREEN_CAPTURE = 1000;  //Khoảng cách tối thiểu giữa 2 lần chụp
        public static string LOG_FILE_PATH = Path.Combine(Setting.SCREEN_LOG_ADDRESS, "MonitoringErrors.txt");
    }
}
