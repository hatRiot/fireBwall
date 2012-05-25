using System;
using System.Text;

namespace fireBwall.Logging
{
    public class LogEvent
    {
        public string Module;
        public string Message;
        public DateTime time;

        public LogEvent(string message, string module)
        {
            Module = module;
            Message = message;
            time = DateTime.Now;
        }
    }
}
