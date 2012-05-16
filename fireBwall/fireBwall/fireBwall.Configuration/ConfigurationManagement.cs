using System;
using System.IO;

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

        private ConfigurationManagement() { }

        public static ConfigurationManagement Instance
        {
            get 
            {
                lock (syncRoot)
                {
                    if (instance == null) 
                    {
                        if (instance == null)
                            instance = new ConfigurationManagement();
                    }
                    return instance;
                }                
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
                if (configPath == null)
                {
                    configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "fireBwall";
                    if (!Directory.Exists(configPath))
                    {
                        Directory.CreateDirectory(configPath);
                    }
                }
                return configPath;
            }
            set
            {
                configPath = value;
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
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
