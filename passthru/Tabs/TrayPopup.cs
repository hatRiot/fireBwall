using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace PassThru.Tabs
{
    public partial class TrayPopup : Form
    {
        Thread t;

        public TrayPopup()
        {
            InitializeComponent();
            t = new Thread(Loop);
            t.Name = "Tray Hide Timer Loop";
            t.Start();
        }

        void Loop()
        {
            while (true)
            {
                if (DateTime.Now > hideTime)
                    HideMe();
                Thread.Sleep(1000);
            }
        }

        void ShowMe()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadStart(HideMe));
            }
            else
            {
                this.Visible = true;
            }
        }

        void HideMe()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadStart(HideMe));
            }
            else
            {
                this.Visible = false;
            }
        }

        DateTime hideTime = DateTime.Now;

        delegate void ALE(LogEvent le);

        public void AddLogEvent(LogEvent le)
        {
            if (listBox1.InvokeRequired)
            {
                ALE ale = new ALE(AddLogEvent);
                listBox1.Invoke(ale, le);
            }
            else
            {
                listBox1.Items.Insert(0, le);
                while (listBox1.Items.Count > 5)
                {
                    listBox1.Items.RemoveAt(5);
                }
                hideTime = DateTime.Now.AddSeconds(10);
                ShowMe();
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                LogEvent le = (LogEvent)listBox1.SelectedItem;
                if (le.PMR != null)
                {
                    if (le.PMR.actualModule != null)
                    {
                        try
                        {
                            UserControl uc = le.PMR.actualModule.GetControl();
                            if (uc != null)
                            {
                                ThemedForm f = new ThemedForm();
                                f.Size = new System.Drawing.Size(640, 480);
                                System.Reflection.Assembly target = System.Reflection.Assembly.GetExecutingAssembly();
                                f.Icon = new System.Drawing.Icon(target.GetManifestResourceStream("PassThru.Resources.newIcon.ico"));
                                f.Text = le.PMR.actualModule.adapter.InterfaceInformation.Name + ": " + le.PMR.actualModule.MetaData.Name + " - " + le.PMR.actualModule.MetaData.Version;
                                f.Controls.Add(uc);
                                ColorScheme.SetColorScheme(f);
                                f.Show();
                            }
                        }
                        catch (Exception ne)
                        {
                            LogCenter.WriteErrorLog(ne);
                        }
                    }
                }
            }
        }

        private void TrayPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Abort();
        }
    }
}
