using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using FM;
using System.Runtime.Serialization;

namespace DDoS
{
    /*
     * DDoS Protection Module object
     * Implements specific techniques to help mitigate DDoS.  This includes:
     * 
     * - SYN flood protection and flow control
     * - UDP echos (fraggle)
     * - ICMP echo replies (smurf)
     * 
     * @note: this module WILL NOT stop a DDoS; it will only assist in mitigating it.
     * Proper DDoS protection should be implemented at a higher level (ie. hardware or better yet ISP)
     */
    public class DDoSModule : FirewallModule
    {
        private Dictionary<IPAddress, int> ipcache = new Dictionary<IPAddress, int>();
        private TCPPacket TCPprevious_packet;
        private ICMPPacket ICMPprevious_packet;

        // constructor 
        public DDoSModule()
            : base()
        {
            Help();
        }

        // return local user control
        public override System.Windows.Forms.UserControl GetControl()
        {
            return new DDoSDisplay(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }

        // Action for ModuleStart
        public override ModuleError ModuleStart()
        {
            LoadConfig();
            if (PersistentData == null)
                data = new DDoSData();
            else
                data = (DDoSData)PersistentData;

            ModuleError moduleError = new ModuleError();
            moduleError.errorType = ModuleErrorType.Success;
            return moduleError;
        }

        // Action for ModuleError
        public override ModuleError ModuleStop()
        {
            if (data != null)
            {
                if (!data.Save)
                    data = new DDoSData();

                PersistentData = data;
                SaveConfig();
            }
            ModuleError moduleError = new ModuleError();
            moduleError.errorType = ModuleErrorType.Success;
            return moduleError;
        }

        [Serializable]
        public class DDoSData
        {
            [NonSerialized]
            private List<BlockedIP> blockcache = null;

            public List<BlockedIP> BlockCache 
            { 
                get 
                {
                    if (cache == null)
                        cache = new BlockedIP[0]; 
                    if(blockcache == null)
                        blockcache = new List<BlockedIP>(cache);
                    return blockcache;
                } 
                set 
                { 
                    blockcache = value;
                    cache = blockcache.ToArray();
                } 
            }
            private BlockedIP[] cache = new BlockedIP[0];

            public int dos_threshold = 10;
            public bool Save = true;
        }

        public DDoSData data;

        // main routine
        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            PacketMainReturn pmr;

            // check it the packet is, or contains, IP
            if (in_packet.ContainsLayer(Protocol.IP))
            {
                // create a temp IPPacket obj and
                // check the IP address
                IPPacket temp = (IPPacket)in_packet;
                if (!(isIPAllowed(temp.SourceIP)))
                {
                    pmr = new PacketMainReturn(this);
                    pmr.returnType = PacketMainReturnType.Drop;
                    return pmr;
                }
            }

            // simple sanity check to dump the ipcache if it gets too large.
            // this does not effect the blockcache of banned IPs
            if ((ipcache.Count) > 200)
                ipcache.Clear();

            // TCP incoming packets
            if (in_packet.GetHighestLayer() == Protocol.TCP)
            {
                TCPPacket packet = ((TCPPacket)in_packet);
                packet.PacketTime = DateTime.UtcNow;

                // if it's inbound and the SYN flag is set
                if (!packet.Outbound && packet.SYN && !packet.ACK)
                {
                    // first packet init
                    if (TCPprevious_packet == null)
                        TCPprevious_packet = packet;

                    // if the IP hasn't been logged yet 
                    if (!(ipcache.ContainsKey(packet.SourceIP)))
                        ipcache.Add(packet.SourceIP, 1);
                    // if the ipcache contains the ip
                    else if (ipcache.ContainsKey(packet.SourceIP))
                    {
                        // increment the packet count if they're coming in fast
                        if ((packet.PacketTime - TCPprevious_packet.PacketTime).TotalMilliseconds <= data.dos_threshold)
                            ipcache[packet.SourceIP] = (ipcache[packet.SourceIP]) + 1;
                        else ipcache[packet.SourceIP] = 1;

                        // check if this packet = previous, if the packet count is > 50, 
                        // and if the time between sent packets is less than the threshhold
                        if (packet.SourceIP.Equals(TCPprevious_packet.SourceIP) &&
                            ((ipcache[packet.SourceIP]) > 50) &&
                            (packet.PacketTime - TCPprevious_packet.PacketTime).TotalMilliseconds <= data.dos_threshold)
                        {
                            pmr = new PacketMainReturn(this);
                            pmr.returnType = PacketMainReturnType.Drop | PacketMainReturnType.Log | PacketMainReturnType.Popup;
                            pmr.logMessage = "DoS attempt detected from IP " + packet.SourceIP + " (likely spoofed). "
                                        + " Packets from this IP will be dropped.  You can unblock this IP from the module interface.";
                            data.BlockCache.Add(new BlockedIP(packet.SourceIP, DateTime.UtcNow, "DoS Attempt"));
                            return pmr;
                        }
                    }
                    TCPprevious_packet = packet;
                }
            }

            // fraggle attack mitigation
            if (in_packet.GetHighestLayer() == Protocol.UDP)
            {
                UDPPacket packet = ((UDPPacket)in_packet);
                packet.PacketTime = DateTime.UtcNow;

                // if it's inbound
                if (!(packet.Outbound))
                {
                    // add IP to cache or increment packet count
                    if (!(ipcache.ContainsKey(packet.SourceIP)))
                        ipcache.Add(packet.SourceIP, 1);
                    else
                        ipcache[packet.SourceIP] = (ipcache[packet.SourceIP]) + 1;

                    // if the packet header is empty, headed towards port (7,13,19,17), and count > 50,
                    // then it's probably a fraggle attack
                    if (packet.isEmpty() && packet.DestPort.Equals(7) || packet.DestPort.Equals(13) ||
                         packet.DestPort.Equals(19) || packet.DestPort.Equals(17) &&
                         (ipcache[packet.SourceIP]) > 50)
                    {
                        pmr = new PacketMainReturn(this);
                        pmr.returnType = PacketMainReturnType.Drop | PacketMainReturnType.Log | PacketMainReturnType.Popup;
                        pmr.logMessage = "Potential fraggle attack from " + packet.SourceIP + " (likely spoofed). "
                            + " Packets from this IP will be dropped.  You can unblock this IP from the module interface.";
                        data.BlockCache.Add(new BlockedIP(packet.SourceIP, DateTime.UtcNow, "Fraggle Attempt"));
                        return pmr;
                    }
                }
            }

            // smurf attack mitigation
            if (in_packet.GetHighestLayer() == Protocol.ICMP)
            {
                ICMPPacket packet = ((ICMPPacket)in_packet);
                packet.PacketTime = DateTime.UtcNow;

                if (!(packet.Outbound))
                {
                    // init the previous packet
                    if (ICMPprevious_packet == null)
                        ICMPprevious_packet = packet;

                    // add IP to cache or increment packet count
                    if (!(ipcache.ContainsKey(packet.SourceIP)))
                        ipcache.Add(packet.SourceIP, 1);
                    // if the packet is >= threshold after the previous and it's the same packet, clear up the cache
                    else if ((packet.PacketTime.Millisecond - ICMPprevious_packet.PacketTime.Millisecond) >= data.dos_threshold &&
                                packet.Equals(ICMPprevious_packet))
                        ipcache[packet.SourceIP] = 1;
                    // if the packet is coming in quickly, add it to the packet count
                    else if ((packet.PacketTime.Millisecond - ICMPprevious_packet.PacketTime.Millisecond) <= data.dos_threshold)
                        ipcache[packet.SourceIP] = (ipcache[packet.SourceIP]) + 1;

                    // if the packet is an echo reply and the IP source
                    // is the same as localhost and the time between packets is <= threshhold and
                    // there are over 50 accumulated packets, it's probably a smurf attack
                    if (packet.Type.ToString().Equals("0") &&
                         packet.Code.ToString().Equals("0") &&
                         packet.SourceIP.Equals(getLocalIP()) &&
                         (packet.PacketTime.Millisecond - ICMPprevious_packet.PacketTime.Millisecond) <= data.dos_threshold &&
                         ipcache[packet.SourceIP] > 50)
                    {
                        pmr = new PacketMainReturn(this);
                        pmr.returnType = PacketMainReturnType.Drop | PacketMainReturnType.Log | PacketMainReturnType.Popup;
                        pmr.logMessage = "Potential Smurf attack from " + packet.SourceIP + " (likely spoofed). "
                            + " Packets from this IP will be dropped.  You can unblock this IP from the module interface.";
                        data.BlockCache.Add(new BlockedIP(packet.SourceIP, DateTime.UtcNow, "Smurf Attempt"));
                        return pmr;
                    }
                    ICMPprevious_packet = packet;
                }
            }

            return null;
        }

        /// <summary>
        /// Determine whether a given IP address is blocked
        /// 
        /// Note to Drone:  HashSet would make this faster, even more so after IPv6 is connected everywheres
        /// </summary>
        /// <param name="i">IP address to resolve</param>
        /// <returns>bool</returns>
        private bool isIPAllowed(IPAddress i)
        {
            bool isAllowed = true;
            foreach (BlockedIP l in data.BlockCache)
            {
                if (l.Blockedip.Equals(i))
                {
                    isAllowed = false;
                    break;
                }
            }
            return isAllowed;
        }

        /// <summary>
        /// Returns local IP address
        /// </summary>
        /// <returns>IPAddress</returns>
        private IPAddress getLocalIP()
        {
            IPAddress local = null;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    local = ip;
                }
            }
            return local;
        }

        // module metadata
        private void Help()
        {
            MetaData.Name = "DDoS Mitigation";
            MetaData.Version = "1.0.1.0";
            MetaData.Description = "Mitigates DoS Attacks";
            MetaData.Contact = "shodivine@gmail.com";
            MetaData.Author = "Bryan A. (drone)";
            MetaData.HelpString = "A DDoS, or a distributed denial of service, is an attack on a network in an attempt to exhaust all possible resources.  The idea "
                + " is not to 'hack' passwords or steal data (although it can be used as a staging attack), but rather to cause a service to become unavailable.  The theft "
                + " of availability.  This module, at its current inception, assists in the mitigation of three different types of attacks.\n\n"
                + "1. DDoS/TCP Flood\n\tThis general attack is characterized by an overwhelming amount of incoming TCP packets destined for arbitrary ports, or, if it"
                + " is a web server, port 80.  The idea behind mitigation is to keep a count of IP's, packet counts, and packet timestamps to trace a possible source of a "
                + "DDoS.  As the name would suggest, a DDoS will not be limited to a single IP, but rather be distributed over hundreds of thousands of systems.  This module "
                + "attempts to take these concepts into account and premptively cease a DDoS attack.  When a DDoS is detected, packets from violating IPs are added to a block list."
                + "  Eventually, this module will be intelligent enough to take more decisive action when floods are detected.  To test the module, disable the basic firewall module "
                + "(or temporarily allow packets through TCP port 3211), and use the following command: hping3 --flood -p 3211 -S <ip of PC with firebwall> This will spam the given "
                + "IP with TCP packets, destined for port 3211\n\n"
                + "2. Fraggle Flood\nA fraggle attack is a UDP flood of echo traffic to IP broadcast addresses.  When it hits the broadcast address, its spammed throughout the entire"
                + " block of IPs, then those IPs all reply to the spoof source address.  These packets usually have empty packet headers and are destined for a select few ports, "
                + " typically 7, 13, 17, and 19.\n\n"
                + "3. Smurf Flood\nMuch like the fraggle attack, this flood takes advantage broadcast addresses, though instead of UDP traffic it's ICMP.  An attacker A sends "
                + " a slew of echo requests towards a router broadcat address, spoofed with the source address of the victim.  The router then sends all the ICMP echo requests to "
                + " every IP it has.  These systems then send back an echo reply towards the spoofed address, the victim.  Modern systems have these capabilites disabled, but there"
                + " are still unpatched systems that hive this vulnerability (use nmap if you're bored)."
                + "\n\nThe DoS Threshold setting allows you to control what too many packets per second actually means.  The default value is 10ms (meaning, "
                + " if more than X packets fly in with less than 10ms between them, block the IP).  If you're unsure, or you're getting too many false positives, "
                + " try adjusting this number.  Please supply logs and pcaps if you have any inquries regarding this.";
        }
    }

    // object used for storing information regarding a blocked IP
    [Serializable]
    public class BlockedIP
    {
        private IPAddress blockedip;
        public IPAddress Blockedip
        {
            get { return blockedip; }
            set { blockedip = value; }
        }

        private DateTime dateblocked;
        public DateTime DateBlocked
        {
            get { return dateblocked; }
            set { dateblocked = value; }
        }

        private string reason;
        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }

        // constructor with vars
        public BlockedIP(IPAddress b, DateTime d, string s)
        {
            this.blockedip = b;
            this.dateblocked = d;
            this.reason = s;
        }

        // def init
        public BlockedIP()
        {
            blockedip = null;
            dateblocked = DateTime.Now;
            reason = "";
        }
    }
}
