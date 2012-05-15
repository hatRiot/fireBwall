using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace PassThru.Tabs
{
    public partial class DownloadCenter : Form
    {
        static DownloadCenter dc = null;
        fireBwallMetaData meta = null;
        static object padlock = new object();
        public static DownloadCenter Instance
        {
            get
            {
                lock (padlock)
                {
                    if (dc == null)
                        dc = new DownloadCenter();
                    return dc;
                }
            }
        }

        public void ShowFirebwallUpdate()
        {
            if (this.InvokeRequired)
            {
                System.Threading.ThreadStart ts = new System.Threading.ThreadStart(ShowFirebwallUpdate);
                this.Invoke(ts);
            }
            else
            {
                this.Visible = true;
                meta = UpdateChecker.availableFirebwall;
                this.Text = "New Version: fireBwall " + meta.version;
                textBox1.Text = "New Version: fireBwall " + meta.version + "\r\n" + meta.Description + "\r\n\r\nChange Log:\r\n";
                foreach (string s in meta.changelog)
                {
                    textBox1.Text += "\t- " + s + "\r\n";
                }
                ColorScheme_ThemeChanged();
            }
        }

        private DownloadCenter()
        {
            InitializeComponent();
            ColorScheme.ThemeChanged += new System.Threading.ThreadStart(ColorScheme_ThemeChanged);
        }

        void ColorScheme_ThemeChanged()
        {
            ColorScheme.SetColorScheme(this);
        }

        private void DownloadCenter_Load(object sender, EventArgs e)
        {
            System.Reflection.Assembly target = System.Reflection.Assembly.GetExecutingAssembly();
            this.Icon = new System.Drawing.Icon(target.GetManifestResourceStream("PassThru.Resources.newIcon.ico"));
        }

        private void DownloadCenter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Visible = false;
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + Path.DirectorySeparatorChar + "firebwall";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                folder = folder + Path.DirectorySeparatorChar + "installers";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string file = folder + Path.DirectorySeparatorChar + meta.filename;
                WebClient wc = new WebClient();
                wc.DownloadFile(meta.downloadUrl, file);
                System.Diagnostics.Process.Start(file);
                Program.Close(null, null);
            }
            catch
            {
                MessageBox.Show("An error occurred during downloading and installing the newest version of fireBwall.  Please visit https://firebwall.com");
            }
        }
    }
}
