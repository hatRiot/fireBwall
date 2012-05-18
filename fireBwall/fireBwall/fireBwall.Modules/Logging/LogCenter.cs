using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using fireBwall.Utils;
using fireBwall.Configuration;

namespace fireBwall.Logging
{
    public sealed class LogCenter
    {
        #region ConcurrentSingleton

        private static volatile LogCenter instance;
        private static object syncRoot = new Object();

        private LogCenter() { }

        public static LogCenter Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot)
                    {
                        instance = new LogCenter();
                    }
                }
                return instance;           
            }
        }

        #endregion

        #region Variables

        SwapBufferQueue<Exception> ExceptionQueue = new SwapBufferQueue<Exception>();
        SwapBufferQueue<DebugLogMessage> DebugQueue = new SwapBufferQueue<DebugLogMessage>();

        #endregion

        #region Functions

        public void LogException(Exception e)
        {
            if(GeneralConfiguration.Instance.DeveloperMode)
                ExceptionQueue.Enqueue(e);
        }

        public void LogDebugMessage(string message)
        {
            DebugQueue.Enqueue(new DebugLogMessage(message));
        }

        #endregion
    }
}
