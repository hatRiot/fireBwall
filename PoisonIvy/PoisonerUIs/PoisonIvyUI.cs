using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using FM;

namespace PoisonIvy
{
    public partial class PoisonIvyUI : UserControl
    {
        PoisonIvy ivy;
        public PoisonIvyUI(PoisonIvy p)
        {
            InitializeComponent();
            this.ivy = p;

            // initialize the list box selection and load up the usercontrol
            poisonBox.SelectedIndex = 0;
            listBox1_SelectedIndexChanged(null, null);
        }

        private void poisonBox_Loaded(object sender, EventArgs e)
        {
            poisonBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Display the correct UserControl when the user flips through the possible poisoners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (poisonBox.Visible)
            {
                int idx = poisonBox.SelectedIndex;
                if (idx >= 0)
                {
                    switch (idx)
                    {
                        case 0:
                            ARPUI arp = new ARPUI(ivy);
                            poisonPanel.Controls.Add(arp);
                            arp.Dock = DockStyle.Fill;
                            arp.Visible = true;
                            arp.Show();
                            arp.Refresh();
                            arp.BringToFront();
                            break;
                        case 1:
                            DNSUI dns = new DNSUI(ivy);
                            poisonPanel.Controls.Add(dns);
                            dns.Dock = DockStyle.Fill;
                            dns.Visible = true;
                            dns.Show();
                            dns.Refresh();
                            dns.BringToFront();
                            break;
                        case 2:
                            DHCPUI dhcp = new DHCPUI(ivy);
                            poisonPanel.Controls.Add(dhcp);
                            dhcp.Dock = DockStyle.Fill;
                            dhcp.Visible = true;
                            dhcp.Show();
                            dhcp.Refresh();
                            dhcp.BringToFront();
                            break;
                        case 3:
                            break;
                    }
                }
            }
        }
    }
}
