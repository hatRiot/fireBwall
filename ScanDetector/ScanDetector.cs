using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using FM;
using System.Timers;

namespace ScanDetector
{
    public class ScanDetector : FirewallModule
    {
        // this is our 'scratch pad' for recording data
        public Dictionary<IPAddress, IPObj> ip_table = new Dictionary<IPAddress, IPObj>();

        // map holds ip -> object; this does not persist through restarts because it isn't a structure we should 
        // be keeping around; they're POTENTIAL ip's, not blocked IPs
        public Dictionary<IPAddress, IPObj> potentials = new Dictionary<IPAddress, IPObj>();

        // janitor timer for clearing out old IPs
        private System.Timers.Timer janitor = new System.Timers.Timer();

        // gui obj
        private ScanDetectorUI detect;

        public ScanDetector()
            : base()
        {
            Help();
        }

        public ScanDetector(INetworkAdapter adapter) :
            base(adapter)
        {
            Help();
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            detect = new ScanDetectorUI(this) { Dock = System.Windows.Forms.DockStyle.Fill };
            return detect;
        }

        [Serializable]
        public class ScanData
        {
            private SerializableDictionary<IPAddress, IPObj> blockcache = new SerializableDictionary<IPAddress, IPObj>();
            public SerializableDictionary<IPAddress, IPObj> BlockCache
            { get { return blockcache; } set { blockcache = new SerializableDictionary<IPAddress, IPObj>(value); } }

            public bool Save = true;
            public bool blockImmediately = false;
            public bool cloaked_mode = false;
        }

        public ScanData data;

        /// <summary>
        /// Module initialization
        /// </summary>
        /// <returns></returns>
        public override ModuleError ModuleStart()
        {
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;

            try
            {
                LoadConfig();
                if (PersistentData == null)
                    data = new ScanData();
                else
                    data = (ScanData)PersistentData;

                // set our timer to do stuff
                janitor.Elapsed += new ElapsedEventHandler(timer_Tick);
                janitor.Interval = 60000;
                janitor.Enabled = true;
                janitor.Start();
            }
            catch (Exception e)
            {
                me.errorMessage = e.Message;
                me.errorType = ModuleErrorType.UnknownError;
                me.moduleName = "Scan Detector";
                data = new ScanData();
            }

            return me;
        }

        public override ModuleError ModuleStop()
        {
            if (null != data)
            {
                if (!data.Save)
                    data = new ScanData();

                PersistentData = data;
                SaveConfig();
            }

            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override PacketMainReturn  interiorMain(ref Packet in_packet)
        {
            PacketMainReturn pmr;
            float av = 0;

            if (in_packet.ContainsLayer(Protocol.TCP))
            {
                // if we're in cloaked mode, respond with the SYN ACK
                // More information about this in the GUI code and help string
                if (data.cloaked_mode && ((TCPPacket)in_packet).SYN)
                {
                    TCPPacket from = (TCPPacket)in_packet;

                    EthPacket eth = new EthPacket(60);
                    eth.FromMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
                    eth.ToMac = from.FromMac;
                    eth.Proto = new byte[2] { 0x08, 0x00 };

                    IPPacket ip = new IPPacket(eth);
                    ip.DestIP = from.SourceIP;
                    ip.SourceIP = from.DestIP;
                    ip.NextProtocol = 0x06;
                    ip.TotalLength = 40;
                    ip.HeaderChecksum = ip.GenerateIPChecksum;

                    TCPPacket tcp = new TCPPacket(ip);
                    tcp.SourcePort = from.DestPort;
                    tcp.DestPort = from.SourcePort;
                    tcp.SequenceNumber = (uint)new Random().Next();
                    tcp.AckNumber = 0;
                    tcp.WindowSize = 8192;
                    tcp.SYN = true;
                    tcp.ACK = true;
                    tcp.Checksum = tcp.GenerateChecksum;
                    tcp.Outbound = true;
                    adapter.SendPacket(tcp);
                }

                try
                {
                    TCPPacket packet = (TCPPacket)in_packet;

                    // if the IP is in the blockcache, then return 
                    if (data.BlockCache.ContainsKey(packet.SourceIP))
                    {
                        pmr = new PacketMainReturn(this);
                        pmr.returnType = PacketMainReturnType.Drop;
                        return pmr;
                    }

                    // checking for TTL allows us to rule out the local network
                    // Don't check for TCP flags because we can make an educated guess that if 100+ of our ports are 
                    // fingered with a short window, we're being scanned. this will detect syn, ack, null, xmas, etc. scans.
                    if ((!packet.Outbound) && (packet.TTL < 250))
                    {
                        IPObj tmp;
                        if (ip_table.ContainsKey(packet.SourceIP))
                            tmp = (IPObj)ip_table[packet.SourceIP];
                        else
                            tmp = new IPObj(packet.SourceIP);

                        // add the port to the ipobj, set the access time, and update the table
                        tmp.addPort(packet.DestPort);
                        tmp.time(packet.PacketTime);
                        ip_table[packet.SourceIP] = tmp;
                        av = tmp.getAverage();

                        // if they've touched more than 100 ports in less than 30 seconds and the average
                        // packet time was less than 2s, something's wrong
                        if (tmp.getTouchedPorts().Count >= 100 && (!tmp.Reported) &&
                             tmp.getAverage() < 2000 )
                        {
                            pmr = new PacketMainReturn(this);
                            pmr.returnType = PacketMainReturnType.Log | PacketMainReturnType.Allow;
                            pmr.logMessage = string.Format("{0} touched {1} ports with an average of {2}\n", packet.SourceIP,
                                            tmp.getTouchedPorts().Count, tmp.getAverage());

                            // set the reported status of the IP address
                            ip_table[packet.SourceIP].Reported = true;

                            // add the address to the potential list of IPs and to the local SESSION-BASED list
                            if (!data.blockImmediately)
                            {
                                potentials.Add(packet.SourceIP, ip_table[packet.SourceIP]);
                                detect.addPotential(packet.SourceIP);
                            }
                            // else we want to block it immediately
                            else
                                data.BlockCache.Add(packet.SourceIP, ip_table[packet.SourceIP]);
                            
                            return pmr;
                        }
                    }
                }
                catch (Exception e)
                {
                    pmr = new PacketMainReturn(this);
                    pmr.returnType = PacketMainReturnType.Log;
                    pmr.logMessage = String.Format("{0}\n{1}\n", e.Message, e.StackTrace);

                    return pmr;
                }
            }
            // This will detect UDP knockers.  typically UDP scans are slower, but are combined with SYN scans
            // (-sSU in nmap) so we'll be sure to check for these guys too.
            else if (in_packet.ContainsLayer(Protocol.UDP))
            {
                try
                {
                    UDPPacket packet = (UDPPacket)in_packet;

                    // if the source addr is in the block cache, return 
                    if (data.BlockCache.ContainsKey(packet.SourceIP))
                    {
                        pmr = new PacketMainReturn(this);
                        pmr.returnType = PacketMainReturnType.Drop;
                        return pmr;
                    }

                    if ((!packet.Outbound) && (packet.TTL < 250) && 
                        (!packet.isDNS()))
                    {
                        IPObj tmp;
                        if (ip_table.ContainsKey(packet.SourceIP))
                            tmp = (IPObj)ip_table[packet.SourceIP];
                        else
                            tmp = new IPObj(packet.SourceIP);

                        tmp.addPort(packet.DestPort);
                        tmp.time(packet.PacketTime);
                        ip_table[packet.SourceIP] = tmp;
                        av = tmp.getAverage();

                        if ((tmp.getTouchedPorts().Count >= 100) && (!tmp.Reported) &&
                                (tmp.getAverage() < 2000))
                        {
                            pmr = new PacketMainReturn(this);
                            pmr.returnType = PacketMainReturnType.Log | PacketMainReturnType.Allow;
                            pmr.logMessage = string.Format("{0} touched {1} ports with an average of {2}\n", packet.SourceIP,
                                        tmp.getTouchedPorts().Count, tmp.getAverage());

                            ip_table[packet.SourceIP].Reported = true;

                            if (!data.blockImmediately)
                            {
                                potentials.Add(packet.SourceIP, ip_table[packet.SourceIP]);
                                detect.addPotential(packet.SourceIP);
                            }
                            else
                                data.BlockCache.Add(packet.SourceIP, ip_table[packet.SourceIP]);
                            return pmr;
                        }
                    }
                }
                catch (Exception e)
                {
                    pmr = new PacketMainReturn(this);
                    pmr.returnType = PacketMainReturnType.Log;
                    pmr.logMessage = String.Format("{0}\n{1}\n", e.Message, e.StackTrace);
                    return pmr;
                }
            }
            return null;
        }   
        
        /// <summary>
        /// metadata
        /// </summary>
        private void Help()
        {
            MetaData.Author = "Bryan A.";
            MetaData.Contact = "shodivine@gmail.com";
            MetaData.Description = "Detects port scans.";
            MetaData.HelpString = "OVERVIEW\nPort scans typically range from troubleshooting, harmless self-inspection to a preemptive strike for a malicious attack."
                                    + "They provide valuable information to attackers when searching for potential avenues for exploitation.  They are, also, not all"
                                    + " completely malicious.  Many system administrators port scan themselves when attempting to diagnose issues, perform self-audits, or other various maintenance work."
                                    + "  Scan Detector, on its default settings, only alerts the user of a potential scan.  The user can then decide "
                                    + "whether or not to continue receiving packets from the IP address.  If the user wishes to not be the arbiter of that, a \'block immediately'" 
                                    + " option is selectable.  \n\nTECHNICAL\nScan Detector logs how many ports an IP address has touched with a short window of time.  An IP has its ports washed "
                                    + " after 30 seconds, and the IP is completely removed after 1 minute of inactivity.  These numbers were chosen based on performance and nmap timings.  nmap at its"
                                    + " most paranoid spits out one packet per 15 seconds.  The number of distinct ports touched within this window is 100; this number was chosen based on nmap's "
                                    + "-F flag, which runs a scan in \'Fast\' mode, or scan only the top 100 ports.\n\nCLOAKED MODE\nCloaked mode is an attempt to exploit the security-through-obscurity"
                                    + " mechanisms behind port scanning/detecting.  It is an adaptation of Jon Erickson's Shroud application in \'The Art of Exploitation\'.  The objective is to "
                                    + "disguise real ports within a sea of false positives.  If, for example, an attacker scans 2000 ports on the host system, all 2000 ports will respond as if they"
                                    + " are actually open.  This works by merely responding to every SYN that passes by with a SYN ACK.";
            MetaData.Name = "Scan Detector";
            MetaData.Version = "0.0.2.0";
        }

        /// <summary>
        /// This is my janitor tick.  If an object hasn't been accessed in 30 seconds, it wipes
        /// all of its ports.  If it hasn't been accessed in a minute, it's removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            List<IPAddress> list = new List<IPAddress>(ip_table.Keys);
            foreach ( IPAddress ip in list )
            {
                IPObj tmp = (IPObj)ip_table[ip];
                if ((DateTime.Now - tmp.last_access).TotalSeconds > 30 && (DateTime.Now - tmp.last_access).TotalSeconds < 60)
                {
                    tmp.Touched_Ports = new List<int>();
                }
                else if ((DateTime.Now - tmp.last_access).TotalSeconds >= 60)
                {
                    ip_table.Remove(ip);
                }
            }
        }
    }

    // ip address object and it's relevant stuff
    [Serializable]
    public class IPObj
    {
        private IPAddress address;
        public IPAddress Address { get { return address; } set { this.address = value; }}

        [NonSerialized]
        private List<int> touched_ports = null;
        public List<int> Touched_Ports 
        { 
            get 
            {
                if (touched_ports == null)
                    touched_ports = new List<int>();
                return touched_ports; 
            } 
            set 
            { 
                this.touched_ports = value;
            } 
        }

        public DateTime last_access;
        private DateTime last_packet;
        private float average_time;

        private bool reported = false;
        public bool Reported { get { return reported; } set { this.reported = value; } }

        public IPObj(IPAddress addr)
        {
            this.address = addr;
            time(DateTime.Now);
        }

        /// <summary>
        /// adds a port and access time
        /// </summary>
        /// <param name="p"></param>
        public void addPort(int p)
        {
            if (!Touched_Ports.Contains(p))
                Touched_Ports.Add(p);
            time(DateTime.Now);
        }

        /// <summary>
        /// Give me the datetime from one of my packets so I can use it to calculate my average!
        /// </summary>
        /// <param name="t"></param>
        public void time(DateTime t)
        {
            if (last_packet != null)
            {
                TimeSpan span = last_packet.Subtract(t);
                average_time = (average_time + span.Milliseconds) / 2;
            }

            last_packet = t;
            last_access = DateTime.Now;
        }

        /// <summary>
        /// Returns a list of all the ports touched by this IP
        /// </summary>
        /// <returns></returns>
        public List<int> getTouchedPorts()
        {
            return Touched_Ports;
        }

        /// <summary>
        /// Returns the average packet time for this IPObj
        /// </summary>
        /// <returns></returns>
        public float getAverage()
        {
            return average_time;
        }
    }
}
