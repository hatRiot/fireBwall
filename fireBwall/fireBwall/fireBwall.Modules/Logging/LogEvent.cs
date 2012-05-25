using System;
using System.Text;
using fireBwall.UI;

namespace fireBwall.Logging
{
    public class LogEvent
    {
        public string Module;
        public string Message;
        public DynamicUserControl userControl;
        public DateTime time;

        public LogEvent(string message, string module)
        {
            Module = module;
            Message = message;
            time = DateTime.Now;
        }
    }
}
