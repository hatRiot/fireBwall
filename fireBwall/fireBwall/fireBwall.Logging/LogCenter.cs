using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using fireBwall.Utils;

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

        #region Locks

        ReaderWriterLock ExceptionLock = new ReaderWriterLock();
        ReaderWriterLock BasicLock = new ReaderWriterLock();
        ReaderWriterLock PcapLock = new ReaderWriterLock();

        #endregion

        #region Variables

        SwapBufferQueue<Exception> ExceptionQueue = new SwapBufferQueue<Exception>();

        #endregion

        #region Functions

        public void LogException(Exception e)
        {
            try
            {
                ExceptionLock.AcquireWriterLock(new TimeSpan(0, 1, 0));
                try
                {
                    ExceptionQueue.Enqueue(e);
                }
                finally
                {
                    ExceptionLock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException app)
            {

            }
        }

        #endregion
    }
}
