using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net;
using FM;
using System.Net.NetworkInformation;

namespace PoisonIvy
{
    public partial class ARPUI : UserControl
    {
        private PoisonIvy ivy;
        public ARPUI(PoisonIvy ivy)
        {
            InitializeComponent();
            this.ivy = ivy;
            init_local();
        }

        /// <summary>
        /// Kick off the spoofing with a delegate thread so we can do waits and stuff.
        /// </summary>
        /// <param name="p">The protocol to initialize with</param>
        /// <param name="a">The from address</param>
        /// <param name="b">The to address</param>
        public delegate void DPoison(Protocol p, IPAddress a, IPAddress b);
        private void poisonButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                IPAddress from = IPAddress.Parse(poisonIP.Text);
                IPAddress to = IPAddress.Parse(toIP.Text);

                status.Text = String.Format("STATUS: Spoofing {0} -> {1}", from.ToString(), to.ToString());
                DPoison dp = new DPoison(ivy.initializePoisoner);
                dp.BeginInvoke(Protocol.ARP, from, to, null, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void stopSpoofButton_Click(object sender, EventArgs e)
        {
            // stop the poisoner
            ivy.stopPoisoner(Protocol.ARP);
        }

        /// <summary>
        /// Initialize some of the static GUI elements
        /// </summary>
        private void init_local()
        {
            status.Text = "STATUS: Not Spoofing";

            // set local information
            localIP.Text = GetLocalIP().ToString();
            localGateway.Text = ivy.adapter.InterfaceInformation.GetIPProperties().GatewayAddresses[0].Address.ToString();
            localMAC.Text = ivy.adapter.InterfaceInformation.GetPhysicalAddress().ToString();

            ICollection<UnicastIPAddressInformation> tmp = ivy.adapter.InterfaceInformation.GetIPProperties().UnicastAddresses;
            foreach ( UnicastIPAddressInformation addr in tmp )
            {
                // depends on the interface
                if (null != addr)
                    localSubnet.Text = addr.IPv4Mask.ToString();
            }
        }

        // return local IPv4 addr
        private IPAddress GetLocalIP()
        {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress addr in he.AddressList)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return addr;
                }
            }
            return null;
        }
    }
}
