using System;
using System.Windows.Forms;
using System.Threading;
using fireBwall.UI.Tabs;
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
            ConfigurationManagement.Instance.LoadAllConfigurations();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
