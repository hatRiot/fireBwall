using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using FM;
using System.Diagnostics;

namespace SaveFlashVideo
{
    public partial class SaveFlashVideoDisplay : UserControl
    {
        private SaveFlashVideo sfv;
        private BindingSource source;
        private Thread updater;

        public SaveFlashVideoDisplay(SaveFlashVideo sfv)
        {
            InitializeComponent();
            this.sfv = sfv;
        }

        void Loop()
        {
            while (true)
            {
                Thread.Sleep(1000);
                UpdateDisplay();
                
            }
        }

        void UpdateDisplay()
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new ThreadStart(UpdateDisplay));
            }
            else
            {
                try
                {
                    source.Clear();
                    List<SaveFlashVideo.VideoInformation> temp = new List<SaveFlashVideo.VideoInformation>();
                    lock (sfv.videos)
                    {
                        temp = new List<SaveFlashVideo.VideoInformation>(sfv.videos.Values);
                    }
                    foreach (SaveFlashVideo.VideoInformation vi in temp)
                    {
                        source.Add(vi);
                    }
                }
                catch { }
            }
        }

        private void SaveFlashVideoDisplay_Load(object sender, EventArgs e)
        {
            source = new BindingSource();
            UpdateDisplay();
            dataGridView1.DataSource = source;
            updater = new Thread(new ThreadStart(Loop));
            updater.Start();
            this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
        }

        void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            updater.Abort();
        }

        private void SaveFlashVideoDisplay_EnabledChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + "firebwall" + System.IO.Path.DirectorySeparatorChar + "modules" + System.IO.Path.DirectorySeparatorChar + "SaveFlashVideo");
            }
            catch { };
        }
    }
}
