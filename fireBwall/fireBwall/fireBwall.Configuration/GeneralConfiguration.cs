using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace fireBwall.Configuration
{
    public sealed class GeneralConfiguration
    {
        #region ConcurrentSingleton

        private static volatile GeneralConfiguration instance;
        private static object syncRoot = new Object();
        private ReaderWriterLock locker = new ReaderWriterLock();

        private GeneralConfiguration() { }

        /// <summary>
        /// Makes sure that the creation of a new GeneralConfiguration is threadsafe
        /// </summary>
        public static GeneralConfiguration Instance
        {
            get 
            {
                lock (syncRoot) 
                {
                    if (instance == null)
                        instance = new GeneralConfiguration();
                }
                return instance;               
            }
        }

        #endregion

        #region Serializable State

        [Serializable()]
        public class GeneralConfig
        {
            public string CurrentTheme = null;
            public string PreferredLanguage = null;
            public bool StartMinimized = false;
            public bool ShowPopups = true;
            public uint MaxLogs = 5;
            public uint MaxPcapLogs = 25;
            public UpdateConfiguration UpdateConfig = new UpdateConfiguration();
        }

        [Serializable()]
        public class UpdateConfiguration
        {
            public bool CheckOnStartUp = true;
            public bool IntervaledChecks = true;
            public uint IntervalMinutes = 10;
        }

        #endregion

        #region Variables

        private GeneralConfig configuration = null;

        #endregion

        #region Members

        public string PreferredLanguage
        {
            get
            {
                if (configuration == null)
                    Load();
                return configuration.PreferredLanguage;
            }
            set
            {
                if (configuration == null)
                    Load();
                configuration.PreferredLanguage = value;
            }
        }

        public string CurrentTheme
        {
            get
            {
                if (configuration == null)
                    Load();
                return configuration.CurrentTheme;
            }
            set
            {
                if (configuration == null)
                    Load();
                configuration.CurrentTheme = value;
            }
        }

        public bool StartMinimized
        {
            get
            {
                if (configuration == null)
                    Load();
                return configuration.StartMinimized;
            }
            set
            {
                if (configuration == null)
                    Load();
                configuration.StartMinimized = value;
            }
        }

        public bool ShowPopups
        {
            get
            {
                if (configuration == null)
                    Load();
                return configuration.ShowPopups;
            }
            set
            {
                if (configuration == null)
                    Load();
                configuration.ShowPopups = value;
            }
        }

        public uint MaxLogs
        {
            get
            {
                if (configuration == null)
                    Load();
                return configuration.MaxLogs;
            }
            set
            {
                if (configuration == null)
                    Load();
                configuration.MaxLogs = value;
            }
        }

        public uint MaxPcapLogs
        {
            get
            {
                if (configuration == null)
                    Load();
                return configuration.MaxPcapLogs;
            }
            set
            {
                if (configuration == null)
                    Load();
                configuration.MaxPcapLogs = value;
            }
        }

        public bool CheckUpdateOnStartup
        {
            get
            {
                if (configuration == null)
                    Load();
                if (configuration.UpdateConfig == null)
                    configuration.UpdateConfig = new UpdateConfiguration();
                return configuration.UpdateConfig.CheckOnStartUp;
            }
            set
            {
                if (configuration == null)
                    Load();
                if (configuration.UpdateConfig == null)
                    configuration.UpdateConfig = new UpdateConfiguration();
                configuration.UpdateConfig.CheckOnStartUp = value;
            }
        }

        public bool IntervaledUpdateChecks
        {
            get
            {
                if (configuration == null)
                    Load();
                if (configuration.UpdateConfig == null)
                    configuration.UpdateConfig = new UpdateConfiguration();
                return configuration.UpdateConfig.IntervaledChecks;
            }
            set
            {
                if (configuration == null)
                    Load();
                if (configuration.UpdateConfig == null)
                    configuration.UpdateConfig = new UpdateConfiguration();
                configuration.UpdateConfig.IntervaledChecks = value;
            }
        }

        public uint IntervaledUpdateMinutes
        {
            get
            {
                if (configuration == null)
                    Load();
                if (configuration.UpdateConfig == null)
                    configuration.UpdateConfig = new UpdateConfiguration();
                return configuration.UpdateConfig.IntervalMinutes;
            }
            set
            {
                if (configuration == null)
                    Load();
                if (configuration.UpdateConfig == null)
                    configuration.UpdateConfig = new UpdateConfiguration();
                configuration.UpdateConfig.IntervalMinutes = value;
            }
        }

        #endregion

        #region Functions

        public bool Save()
        {
            try
            {
                locker.AcquireReaderLock(new TimeSpan(0, 1, 0));
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GeneralConfig));
                    TextWriter writer = new StreamWriter(ConfigurationManagement.Instance.ConfigurationPath + Path.DirectorySeparatorChar + "general.cfg");
                    serializer.Serialize(writer, configuration);
                    writer.Close();
                }
                catch
                {
                    return false;
                }
                finally
                {
                    locker.ReleaseReaderLock();
                }
                return true;
            }
            catch (ApplicationException ex)
            {
                return false;
            }
        }

        public bool Load()
        {
            try
            {
                locker.AcquireWriterLock(new TimeSpan(0, 1, 0));
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GeneralConfig));
                    TextReader reader = new StreamReader(ConfigurationManagement.Instance.ConfigurationPath + Path.DirectorySeparatorChar + "general.cfg");
                    configuration = (GeneralConfig)serializer.Deserialize(reader);
                    reader.Close();
                }
                catch
                {
                    configuration = new GeneralConfig();
                }
                finally
                {
                    locker.ReleaseWriterLock();
                }
                return true;
            }
            catch (ApplicationException ex)
            {
                return false;
            }
        }

        #endregion
    }
}
