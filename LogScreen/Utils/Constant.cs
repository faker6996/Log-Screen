using System.Collections.Generic;
using System.IO;

namespace LogScreen.Utils
{
    public static class Constant
    {

        public static class ModeCheckTimer
        {
            public const string Get = "get_value";
            public const string Set = "set_value";
            public static Dictionary<string, string> dctDesc = new Dictionary<string, string>()
            {
                { Get,          "get_value" },
                { Set,          "set_value" }
            };

        }
    }
}
