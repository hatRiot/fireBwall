using System;
using System.IO;
using System.Threading;
using fireBwall.Logging;

namespace fireBwall.Configuration
{
    /// <summary>
    /// Manages configurations for this running instance
    /// </summary>
    public sealed class ConfigurationManagement
    {
        #region ConcurrentSingleton

        private static volatile ConfigurationManagement instance;
        private static object syncRoot = new Object();
        private ReaderWriterLock rwlock = new ReaderWriterLock();

        private ConfigurationManagement() { }

        public static ConfigurationManagement Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new ConfigurationManagement();
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Variables

        private string configPath = null;

        #endregion

        #region Members

        public string ConfigurationPath
        {
            get
            {
                string ret = null;
                try
                {
                    rwlock.AcquireReaderLock(new TimeSpan(0, 1, 0));
                    try
                    {
                        if (configPath == null)
                        {
                            try
                            {
                                rwlock.ReleaseReaderLock();
                                rwlock.AcquireWriterLock(new TimeSpan(0, 1, 0));
                                try
                                {
                                    configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "fireBwall";
                                    if (!Directory.Exists(configPath))
                                    {
                                        Directory.CreateDirectory(configPath);
                                    }
                                }
                                finally
                                {
                                    rwlock.ReleaseWriterLock();
                                    rwlock.AcquireReaderLock(new TimeSpan(0, 1, 0));
                                }
                            }
                            catch (ApplicationException ex)
                            {
                                LogCenter.Instance.LogException(ex);
                            }
                        }
                        ret = "" + configPath;
                    }
                    catch (Exception e)
                    {
                        LogCenter.Instance.LogException(e);
                    }
                    finally
                    {
                        rwlock.ReleaseReaderLock();
                    }
                }
                catch (ApplicationException ex)
                {
                    LogCenter.Instance.LogException(ex);
                }
                return ret;
            }
            set
            {
                try
                {
                    rwlock.AcquireWriterLock(new TimeSpan(0, 1, 0));
                    try
                    {
                        configPath = value;
                        if (!Directory.Exists(configPath))
                        {
                            Directory.CreateDirectory(configPath);
                        }
                    }
                    finally
                    {
                        rwlock.ReleaseWriterLock();
                    }
                }
                catch (ApplicationException ex)
                {
                    LogCenter.Instance.LogException(ex);
                }
            }
        }

        #endregion

        #region Functions

        public bool SaveAllConfigurations()
        {
            if (!GeneralConfiguration.Instance.Save())
                return false;
            ThemeConfiguration.Instance.Save();
            return true;
        }

        public bool LoadAllConfigurations()
        {
            if (!GeneralConfiguration.Instance.Load())
                return false;
            ThemeConfiguration.Instance.Load();
            return true;
        }

        #endregion
    }
}
