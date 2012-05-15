using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using FM;

namespace PassThru
{
        /*
         * Object used to describe an incoming event
         * houses local vars and constructor
         */
		public class LogEvent
        {
            /*
             * @param mo is the module of the log
             * @param me is the message to log
             */
			public LogEvent(string mo, string me) 
            {
				Module = mo;
				Message = me;
				time = DateTime.Now;
			}
            public LogEvent(PacketMainReturn pmr)
            {
                Module = pmr.Module;
                Message = pmr.logMessage;
                time = DateTime.Now;
                PMR = pmr;
            }

            public override string ToString()
            {
                return time.ToShortTimeString() + ": (" + Module + ") - " + Message;
            }

            public PacketMainReturn PMR = null;
			public string Message = null;
			public string Module = null;
			public DateTime time;
		}

		public class LogCenter
        {
            static Thread pusher = new Thread(new ThreadStart(PushLoop));
            public static TrayIcon ti;

			LogCenter() 
            {
                pusher.Name = "Log Pusher Loop";
                pusher.Start();
			}

            public static void Kill()
            {
                pusher.Abort();
            }

            static void PushLoop()
            {
                while (true)
                {
                    Thread.Sleep(50);
                    LogEvent[] temp = logQueue.DumpBuffer().ToArray();                    
                    if (temp != null)
                    {
                        foreach (LogEvent le in temp)
                            SendLogEvent(le);
                    }
                }
            }

            static SwapBufferQueue<LogEvent> logQueue = new SwapBufferQueue<LogEvent>();

			public delegate void NewLogEvent(LogEvent e);
			static readonly object padlock = new object();

            // generates the LogEvent object and pushes it out to be logged
			public void Push(string Module, string Message) 
            {
				LogEvent le = new LogEvent(Module, Message);
                logQueue.Enqueue(le);
			}

            public void Push(PacketMainReturn pmr)
            {
                logQueue.Enqueue(new LogEvent(pmr));
            }

            /*
             * pushes a log message to the tray icon, as well as the
             * log window
             * 
             * @param le is the log event to be logged
             */
			static void SendLogEvent(LogEvent le) 
            {
				if (PushLogEvent != null)
				{
                    ti.AddLine(le);
					PushLogEvent(le);
                    WriteLogFile(le);
				}
			}

            // Write the exception out to the error log
            public static void WriteErrorLog(Exception e)
            {
                string currentdate = DateTime.Now.ToString("M-d-yyyy");
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + Path.DirectorySeparatorChar + "firebwall";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string filepath = folder;
                string filename = Path.DirectorySeparatorChar + "ErrorLog.log";

                FileStream stream;
                if (e != null)
                {
                    if (Directory.Exists(filepath))
                    {
                        if (File.Exists(filepath + filename))
                            stream = new FileStream(filepath + filename, FileMode.Append, FileAccess.Write, FileShare.Write);
                        else
                            stream = new FileStream(filepath + filename, FileMode.CreateNew, FileAccess.Write, FileShare.Write);

                            StreamWriter m_streamWriter = new StreamWriter(stream);
                            // write out the current date and message
                            // followed by the source and a stack trace
                            m_streamWriter.WriteLine(currentdate + " " + e.Message);
                            m_streamWriter.WriteLine(e.Source);
                            m_streamWriter.WriteLine(e.StackTrace);

                            m_streamWriter.Close();
                            stream.Close();
                    }
                }
            }
            
            /*
             * pushes log events to the event log file
             * Log\Event_<date>.log
             * 
             * These logs are purged by the cleanLogs() method.
             */
            private static void WriteLogFile(LogEvent le)
            {
                string currentdate = DateTime.Now.ToString("M-d-yyyy");
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + Path.DirectorySeparatorChar + "firebwall";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                folder = folder + Path.DirectorySeparatorChar + "Log";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string filepath = folder;
                string filename = Path.DirectorySeparatorChar + "Event_" + currentdate + ".log";

                FileStream stream;
                // if the log event is not null
                if (le != null)
                {
                    // if the Log folder exists already
                    if (Directory.Exists(filepath))
                    {
                        // if the file exists, open in append and write to it
                        if (File.Exists(filepath + filename))
                            stream = new FileStream(filepath + filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        else
                            stream = new FileStream(filepath + filename, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);

                        StreamWriter m_streamWriter = new StreamWriter(stream);
                        m_streamWriter.WriteLine(le.time.ToString() + " " + le.Module + ": " + le.Message + "\r");
                        m_streamWriter.Close();
                        stream.Close();
                    }

                    // if the log path does not exist, create it and write out the log
                    if (!(Directory.Exists(filepath)))
                    {
                        Directory.CreateDirectory(filepath);
                        stream = new FileStream(filepath + filename, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                        StreamWriter m_streamWriter = new StreamWriter(stream);
                        m_streamWriter.WriteLine(le.time.ToString() + " " + le.Module + ": " + le.Message + "\r");
                        m_streamWriter.Close();
                        stream.Close();
                    }
                }
            }

            /*
             * Method called once per run, used to check log paths to 
             * clean up any old logs.  Called from MainWindow.cs - Load().
             * 
             * Logs are retained for however long the user requests (default is 5 days)
             */
            public static void cleanLogs()
            {
                if (OptionsDisplay.gSettings == null)
                    OptionsDisplay.LoadGeneralConfig();

                try
                {
                    //
                    // first clean up the /Log/ folder
                    //
                    string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    folder = folder + Path.DirectorySeparatorChar + "firebwall";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    folder = folder + Path.DirectorySeparatorChar + "Log";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    string filepath = folder;

                    if (Directory.Exists(filepath))
                    {
                        // grab all the logs in the directory
                        string[] files = Directory.GetFiles(filepath);

                        // iterate through them all looking for any that are old (>5)
                        foreach (string s in files)
                        {
                            // grab the log date from file path name and 
                            // convert to DateTime for day check                            
                            string logdate = s.Substring(s.LastIndexOf("_") + 1,
                                (s.LastIndexOf(".") - s.LastIndexOf("_")) - 1);

                            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                            dtfi.ShortDatePattern = "MM-dd-yyyy";
                            dtfi.DateSeparator = "-";
                            DateTime logDate = Convert.ToDateTime(logdate, dtfi);

                            // if it's old, get rid of it
                            if ((DateTime.Now - logDate).Days > OptionsDisplay.gSettings.max_logs)
                            {
                                if (isFileLocked(new FileInfo(s)))
                                    continue;

                                File.Delete(s);
                            }
                        }
                    }

                    //
                    // Now clean up the pcap logs folder
                    //
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    folder = folder + Path.DirectorySeparatorChar + "firebwall";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    folder = folder + Path.DirectorySeparatorChar + "pcapLogs";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    filepath = folder;

                    if (Directory.Exists(filepath))
                    {
                        string[] files = Directory.GetFiles(filepath);

                        if (files.Length > OptionsDisplay.gSettings.max_pcap_logs)
                        {
                            // hack off the number of files found in Settings (default should be 25), starting
                            // from the oldest
                            for (int i = 0; i < (files.Length - OptionsDisplay.gSettings.max_pcap_logs); ++i)
                            {
                                // if it's not accessible, skip it for next time
                                if (isFileLocked(new FileInfo(files[i])))
                                    continue;

                                File.Delete(files[i]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogCenter.WriteErrorLog(e);
                }
            }
            
            public static LogCenter Instance 
            {
				get 
                {
					lock (padlock)
					{
						if (instance==null)
						{
								instance = new LogCenter();
						}
						return instance;
					}
				}
			}
			static LogCenter instance = null;

			public static event NewLogEvent PushLogEvent;

            /// <summary>
            /// Utility method for checking if a file is locked/in-use or not
            /// </summary>
            /// <param name="file"></param>
            /// <returns>True if the file is locked, false if it isn't</returns>
            private static bool isFileLocked(FileInfo file)
            {
                FileStream stream = null;
                try
                {
                    stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    // the file cannot be opened for whatever reason; return that the file is indeed
                    // locked.
                    return true;
                }
                finally
                {
                    if (null != stream)
                        stream.Close();
                }
                return false;
            }
		}
}
