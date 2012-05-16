using System;
using System.Windows.Forms;
using System.Threading;
using fireBwall.Configuration;

namespace fireBwall
{
    public static class Program
    {
        public static event ThreadStart OnShutdown;

        public static void Shutdown()
        {
            ConfigurationManagement.Instance.SaveAllConfigurations();
            if (OnShutdown != null)
                OnShutdown();
            Application.Exit();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            fireBwall.Configuration.GeneralConfiguration.Instance.PreferredLanguage = "English";
            fireBwall.Configuration.GeneralConfiguration.Instance.Save();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
