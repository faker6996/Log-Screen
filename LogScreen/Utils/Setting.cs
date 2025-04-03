using System.Collections.Generic;
using System.IO;

namespace Monitoring.Utils
{
    public static class Setting
    {
        #region End point

        /// <summary>
        /// End point lấy config
        /// </summary>
        public const string CONFIG_URL = "http://works.com.vn/request/sfsfd/sdf/config.php?authkey=44029402340230808408083080fsdfsf";

        /// <summary>
        /// End point api upload
        /// </summary>
        public const string API_UPLOAD = "https://works.com.vn/upload.php";

        /// <summary>
        /// End point api check
        /// </summary>
        public const string API_CHECK = "https://works.com.vn/check.php";

        #endregion

        #region  Token

        /// <summary>
        /// Token để call các api upload
        /// </summary>
        public const string TOKEN = "a6d47a15-17f9-4824-b66d-42aa28023882";

        #endregion

        #region Cấu hình chu kỳ
        /// <summary>
        /// Khoảng cách giữa các lần kiểm tra xem có đang trong thời gian hoạt động theo config không
        /// </summary>
        public const int WORKING_TIMER_INTERVAL = 60000;

        /// <summary>
        /// Thời gian hiển thị thông báo token hết hạn 1 lần (30p)
        /// </summary>
        public const int NOTIFY_SHOW_MESSAGE = 1800000;

        public static class TRY_GET_CONFIG
        {
            public static int MAX_REP = 6;
            public static int INTERVAL = 10000;
        };

        #endregion

        #region Địa chỉ ảnh và log

        public static string SCREEN_LOG_ADDRESS = Path.Combine(Path.GetTempPath(), "Monitoring");

        public static string LOG_FILE_PATH = Path.Combine(Setting.SCREEN_LOG_ADDRESS, "MonitoringErrors.txt");

        #endregion

        /// <summary>
        /// FileName trong start up
        /// </summary>
        public static string FILE_NAME_START_UP = "Monitoring";
        public static string APP_NAME_EXE = "Monitoring.exe";
        public static string CAPTURE_FILE_EXTENTION = "*.jpg";

        /// <summary>
        /// Message:'Invalid token', dùng để check token sai khi call api get txt
        /// </summary>
        public static string TOKEN_INVALID = "Invalid token";

        #region Message 
        /// <summary>
        /// Phần mềm Monitoring của bạn cần được cập nhật, vui lòng liên hệ với quản trị viên!
        /// </summary>
        public static string MESSAGE_TOKEN_INVALID = "Phần mềm Monitoring của bạn cần được cập nhật, vui lòng liên hệ với quản trị viên!";

        /// <summary>
        /// Không thể kết nối đến server
        /// </summary>
        public static string MESSAGE_SERVER_ERROR = "Không thể kết nối đến server";

        /// <summary>
        /// Thông báo
        /// </summary>
        public static string MESSAGE_TITLE = "Thông báo";
        #endregion

        #region Mode check api

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

        #endregion
    }
}
