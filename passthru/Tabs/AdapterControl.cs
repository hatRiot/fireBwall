using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading;
using FM;

namespace PassThru
{
		public partial class AdapterControl: UserControl 
        {
            Thread t;

            bool timing = true;

            void Timing()
            {
                try
                {
                    while (timing)
                    {
                        Thread.Sleep(1000);
                        UpdateAdapterList();
                    }
                }
                catch (ArgumentOutOfRangeException e) 
                {
                    LogCenter.WriteErrorLog(e);
                }
            }

            public void Kill()
            {
                timing = false;
                t.Abort();
            }

			public AdapterControl() 
            {
				InitializeComponent();
				UpdateAdapterList();
                t = new Thread(new ThreadStart(Timing));
                t.Name = "AdapterControl Adapter Update Thread";
                t.Start();
                flowLayoutPanel1.SizeChanged += new EventHandler(flowLayoutPanel1_SizeChanged);
			}

            void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
            {
                foreach (Control c in flowLayoutPanel1.Controls)
                    c.Width = flowLayoutPanel1.Width - 5;
            }

			public void UpdateAdapterList() 
            {
                if (flowLayoutPanel1.InvokeRequired)
                {
                    ThreadStart ts = new ThreadStart(UpdateAdapterList);
                    flowLayoutPanel1.Invoke(ts);
                }
                else
                {
                    if (flowLayoutPanel1.Controls.Count == 0)
                    {
                        foreach (NetworkAdapter na in NetworkAdapter.GetAllAdapters())
                        {
                            AdapterDisplay ad = new AdapterDisplay(new AdapterInfo(na.Pointer, na.Name, na.InterfaceInformation, na.InBandwidth, na.OutBandwidth, na));
                            ad.Width = flowLayoutPanel1.Width - 5;
                            flowLayoutPanel1.Controls.Add(ad);
                        }
                    }
                    else
                    {
                        foreach (AdapterDisplay ad in flowLayoutPanel1.Controls)
                        {
                            ad.Update();
                        }
                        foreach (NetworkAdapter na in NetworkAdapter.GetNewAdapters())
                        {
                            AdapterDisplay ad = new AdapterDisplay(new AdapterInfo(na.Pointer, na.Name, na.InterfaceInformation, na.InBandwidth, na.OutBandwidth, na));
                            ad.Width = flowLayoutPanel1.Width - 5;
                            flowLayoutPanel1.Controls.Add(ad);
                        }
                    }
                }
			}
		}

        public class AdapterInfo
        {
            public AdapterInfo(string P, string N, NetworkInterface NI, BandwidthCounter In, BandwidthCounter Out, NetworkAdapter na)
            {
                pointer = P;
                this.deviceName = N;
                ni = na.InterfaceInformation;
                this.In = In;
                this.Out = Out;
                this.na = na;
            }

            private NetworkAdapter na;
            public NetworkAdapter NetAdapter 
            { 
                get { return na; } 
                set { this.na = value; } 
            }

            BandwidthCounter In;
            BandwidthCounter Out;
            string pointer = null;
            string deviceName = "";
            NetworkInterface ni = null;

            public string IPv4
            {
                get
                {
                    if (ni == null)
                        return "";
                    else
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                        return "";
                    }
                }
            }

            public string IPv6
            {
                get
                {
                    if (ni == null)
                        return "";
                    else
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                            {
                                return ip.Address.ToString();
                            }
                        }
                        return "";
                    }
                }
            }

            public string NIDescription
            {
                get
                {
                    if (ni == null)
                        return "";
                    else
                        return ni.Description;
                }
            }

            public string NIName
            {
                get
                {
                    if (ni == null)
                        return "";
                    else
                        return ni.Name;
                }
            }

            public string DataOut
            {
                get
                {
                    return Out.ToString();
                }
            }

            public string DataOutPerSecond
            {
                get
                {
                    return Out.GetPerSecond();
                }
            }

            public string DataIn
            {
                get
                {
                    return In.ToString();
                }
            }

            public string DataInPerSecond
            {
                get
                {
                    return In.GetPerSecond();
                }
            }

            public string GatewayIP
            {
                get
                {
                    foreach (GatewayIPAddressInformation ip in ni.GetIPProperties().GatewayAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            return ip.Address.ToString();
                    }
                    return null;
                }
            }

            public string GatewayIPv6
            {
                get
                {
                    foreach (GatewayIPAddressInformation ip in ni.GetIPProperties().GatewayAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                            return ip.Address.ToString();
                    }
                    return null;
                }
            }

            public string Summary
            {
                get
                {
                    ni = na.InterfaceInformation;
                    
                    string ret = NIName + "\t\t" + "In(" + DataIn + " | " + DataInPerSecond + ")\tOut(" + DataOut + " | " + DataOutPerSecond + ")\r\n";
                    ret += "MAC Address:\t" + ni.GetPhysicalAddress().ToString() + "\r\n";
                    ret += "IP Addresses:\t" + IPv4 + " \t" + IPv6 + "\r\n";
                    if (GatewayIP != null || GatewayIPv6 != null)
                    {
                        ret += "Gateway:\t\t";
                        if (GatewayIP != null)
                            ret += GatewayIP + "\t";
                        if (GatewayIPv6 != null)
                            ret += GatewayIPv6;
                    }
                    ret += "\r\nDNS Addresses:\t";

                    foreach (System.Net.IPAddress ip in ni.GetIPProperties().DnsAddresses)
                    {
                        ret += ip.ToString() + " \t";
                    }
                    return ret;
                }
            }
        }
}
