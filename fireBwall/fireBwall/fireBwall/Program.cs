using System;
using System.Windows.Forms;

namespace fireBwall
{
    static class Program
    {
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
