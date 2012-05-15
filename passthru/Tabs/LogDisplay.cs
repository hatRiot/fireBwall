using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PassThru.Tabs
{
    public partial class LogDisplay : UserControl
    {
        public LogDisplay()
        {
            InitializeComponent();
        }

        List<string> lines = new List<string>();

        /*
         * Object handles logging of a log event to the window
         * @param le is the log event object to be logged
         */
        public void AddLogEvent(object le)
        {
            // if the logger is busy, invoke it
            if (listBox1.InvokeRequired)
            {
                System.Threading.ParameterizedThreadStart d = new System.Threading.ParameterizedThreadStart(AddLogEvent);
                listBox1.Invoke(d, new object[] { le });
            }
            // else log the message
            else
            {
                LogEvent e = (LogEvent)le;
                listBox1.Items.Insert(0, e.time.ToString() + " " + e.Module + ": " + e.Message);
                while (listBox1.Items.Count > 1000)
                {
                    listBox1.Items.RemoveAt(1000);
                }
            }
        }

        // receives a log event and pushes it to AddLogEvent
        public void Instance_PushLogEvent(LogEvent e)
        {
            AddLogEvent(e);
        }

        // copy the selected log to their clipboard for easy access
        private void listBox1_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(listBox1.SelectedItem.ToString());
            }
            catch { }
        }
    }
}
