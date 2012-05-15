using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using FM;

namespace BasicFirewall
{
    /// <summary>
    /// Basic firewall module
    /// </summary>
    public class BasicFirewallModule : FirewallModule
    {
        public BasicFirewallModule()
            : base()
        {
            MetaData.Name = "Basic Firewall";
            MetaData.Version = "1.2.0.0";
            MetaData.HelpString = "Computers on networks often communicate using protocols that have ports.  TCP and UDP are the most common protocols.  You will see TCP being used very often, as it allows for stable and reliable connections.  UDP is less reliable and is used a bit less.  Both of these use ports to communicate.  Certain ports are used for certain things.  Some you may want open so you can share files or remotely control you computer, but in most cases, you want them closed."
                + "\r\n\r\nThis module uses rules to Allow or Drop packets depending on what port or ip they are for.  It is the one part that is in about every firewall."
                + "\r\n\r\nThis module works based on ordered rules.  The rules are displayed from top to bottom, and the order can be changed by clicking and dragging the rule.  Rules can be added with the Add Rule button, and removed with the Remove Rule button."
                + "\n\nArguments are designated on a rule-by-rule basis.  Some have required arguments, others do not.  If the arguments box is greyed out, "
                + "that particular rule has no arguments.  Otherwise, the required arguments will be denoted by the line of text above the arguments box.\n\n"
                + "BasicFirewall port rules also allow multiple ports or port ranges.  Port ranges should be spaced with a '-'.  For example, to block ports 22 and 80 and range 100 to 200,"
                + " you would use the following: 22 80 100-200";
            MetaData.Description = "Blocks or allows packets based on IP/Port";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Author = "Brian W. (schizo)";
        }

        public class RuleFactory
        {
            public static Rule MakeRule(RuleType ruleType, PacketStatus ps, Direction dir, string args, bool log, bool notify)
            {
                switch (ruleType)
                {
                    case RuleType.IP:
                        return GenIPRule(ps, args, dir, log, notify);
                    case RuleType.TCPALL:
                        return new TCPAllRule(ps, dir, log, notify);
                    case RuleType.TCPIPPORT:
                        return new TCPIPPortRule(ps, IPAddress.Parse(args.Split(' ')[0]), int.Parse(args.Split(' ')[1]), dir, log, notify);
                    case RuleType.TCPPORT:
                        return GenTCPPORT(ps, args, dir, log, notify);
                    case RuleType.UDPALL:
                        return new UDPAllRule(ps, dir, log, notify);
                    case RuleType.UDPPORT:
                        return GenUDPPORT(ps, args, dir, log, notify);
                    case RuleType.ALL:
                        return new AllRule(ps, dir, log, notify);
                }
                return null;
            }
        }

        /// <summary>
        /// Parses out the arguments of a UDPPort rule
        /// 
        /// We accept single or multiple ports and port ranges
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static UDPPortRule GenUDPPORT(PacketStatus ps, string args, Direction dir, bool log, bool notify)
        {
            // first tokenize the args
            List<string> tmp = new List<string>(args.Split(' '));
            UDPPortRule rule = new UDPPortRule();

            try
            {
                // iterate through the given arguments
                foreach (string s in tmp)
                {
                    string loc = s;
                    // parse the port range
                    if (loc.Contains("-"))
                    {
                        // parse the start/end ports out of the arguments
                        PortRange p = new PortRange();
                        string[] split = loc.Split('-');
                        p.start = Convert.ToInt32(split[0]);
                        p.end = Convert.ToInt32(split[1]);

                        // instead of an error message, we can just swap them
                        if (p.start > p.end)
                        {
                            int temp = p.start;
                            p.start = p.end;
                            p.end = temp;
                        }

                        // add it to the rule port ranges list
                        rule.port_ranges.Add(p);
                    }
                    // else it's just a port, add it as usual
                    else
                    {
                        rule.port.Add(Convert.ToInt32(s));
                    }
                }

                // set the other rule stuff
                rule.ps = ps;
                rule.direction = dir;
                rule.log = log;
                rule.notify = notify;
            }
            catch (Exception e)
            {
                // probably a parsing error; log it and throw an 
                // exception so the rule isn't touched
                //LogCenter.WriteErrorLog(e);
                throw new Exception();
            }
            return rule;
        }

        /// <summary>
        /// Parses out the arguments of a TCPPort rule
        /// 
        /// We accept single or multiple ports and port ranges
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static TCPPortRule GenTCPPORT(PacketStatus ps, string args, Direction dir, bool log, bool notify)
        {
            // first tokenize the args
            List<string> tmp = new List<string>(args.Split(' '));
            TCPPortRule rule = new TCPPortRule();

            try
            {
                // iterate through the given arguments
                foreach (string s in tmp)
                {
                    string loc = s;
                    // parse the port range
                    if (loc.Contains("-"))
                    {
                        // parse the start/end ports out of the arguments
                        PortRange p = new PortRange();
                        string[] split = loc.Split('-');
                        p.start = Convert.ToInt32(split[0]);
                        p.end = Convert.ToInt32(split[1]);

                        // someday we'll give more meaningful error messages, but 
                        // for now just throw an exception if they're doing something
                        // dumb like range 200-50
                        if (p.start > p.end)
                            throw new Exception();

                        // add it to the rule port ranges list
                        rule.port_ranges.Add(p);
                    }
                    // else it's just a port, add it as usual
                    else
                    {
                        rule.port.Add(Convert.ToInt32(s));
                    }
                }

                // set the other rule stuff
                rule.ps = ps;
                rule.direction = dir;
                rule.log = log;
                rule.notify = notify;
            }
            catch (Exception)
            {
                // probably a parsing error; log it
                // and throw an exception so the rule isn't changed
                //LogCenter.WriteErrorLog(e);
                throw new Exception();
            }
            return rule;
        }

        /// <summary>
        /// Used to generate a new IPRule
        /// </summary>
        /// <returns></returns>
        private static IPRule GenIPRule(PacketStatus ps, string args, Direction dir, bool log, bool notify)
        {
            List<string> tokens = new List<string>(args.Split(' '));
            IPRule rule = new IPRule();
            
            try
            {
                // iterate through the tokens and try and parse them into 
                // an IP address
                foreach ( string s in tokens )
                {
                    IPAddress tmp = IPAddress.Parse(s);
                    rule.ips.Add(tmp);
                }

                // set the rest of the fields
                rule.ps = ps;
                rule.direction = dir;
                rule.log = log;
                rule.notify = notify;
            }
            catch (Exception)
            {
                // probably a parse error
                throw new Exception();
            }
            return rule;
        }

        readonly object padlock = new object();
        public List<Rule> rules = new List<Rule>();

        public override ModuleError ModuleStart()
        {
            rules = new List<Rule>();
            LoadConfig();
            lock (padlock)
            {
                if (PersistentData == null)
                {
                    rules.Add(new TCPAllRule(PacketStatus.BLOCKED, Direction.IN, true, true));
                }
                else
                {
                    rules.AddRange((Rule[])PersistentData);
                }
            }

            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            return new BasicFirewallControl(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }

        public override ModuleError ModuleStop()
        {
            PersistentData = rules.ToArray();
            SaveConfig();
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            lock (padlock)
            {
                PacketStatus status = PacketStatus.UNDETERMINED;
                foreach (Rule r in rules)
                {
                    status = r.GetStatus(in_packet);
                    if (status == PacketStatus.BLOCKED)
                    {
                        PacketMainReturn pmr = new PacketMainReturn(this);
                        pmr.returnType = PacketMainReturnType.Drop;
                        if (r.GetLogMessage() != null)
                        {
                            pmr.returnType |= PacketMainReturnType.Log;
                            pmr.logMessage = r.GetLogMessage();
                        }
                        if (r.Notify())
                        {
                            pmr.returnType |= PacketMainReturnType.Popup;
                        }
                        return pmr;
                    }
                    else if (status == PacketStatus.ALLOWED)
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public void InstanceGetRuleUpdates(List<Rule> r)
        {
            lock (padlock)
            {
                rules = new List<Rule>(r);
            }
        }

        Quad MakeQuad(Packet in_packet)
        {
            if (in_packet.ContainsLayer(Protocol.TCP))
            {
                TCPPacket tcp = (TCPPacket)in_packet;
                Quad q = new Quad
                {
                    dstIP = tcp.DestIP,
                    dstPort = tcp.DestPort,
                    srcPort = tcp.SourcePort,
                    srcIP = tcp.SourceIP
                };
                return q;
            }
            return null;
        }
    }

    public enum PacketStatus
    {
        UNDETERMINED,
        BLOCKED,
        ALLOWED
    }

    public enum Direction
    {
        IN = 1,
        OUT = 1 << 1
    }

    public interface Rule
    {
        PacketStatus GetStatus(Packet pkt);
        string ToFileString();
        string GetLogMessage();
        bool Notify();
        string String
        {
            get;
        }
    }

    public enum RuleType
    {
        UDPALL,
        UDPPORT,
        TCPIPPORT,
        TCPPORT,
        TCPALL,
        IP,
        ALL
    }

    // object for storing port ranges
    [Serializable]
    public class PortRange
    {
        public int start;
        public int end;

        // check if the queried port is within range
        public Boolean isInRange(int port)
        {
            if ((port >= start) && (port < end))
                return true;

            return false;
        }

        public override string ToString()
        {
            return start.ToString() + "-" + end.ToString();
        }
    }

    #region Rules

    [Serializable]
    public class TCPAllRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public bool log = true;
        public bool notify = true;

        public string String
        {
            get { return ToString(); }
        }

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " all TCP ports";
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += " and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public TCPAllRule(PacketStatus ps, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.log = log;
            this.notify = notify;
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.ContainsLayer(Protocol.TCP))
            {
                TCPPacket tcppkt = (TCPPacket)pkt;
                if (tcppkt.SYN && !(tcppkt.ACK))
                {
                    if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                    {
                        if (log)
                            message = " TCP packet from " + tcppkt.SourceIP.ToString() + ":" +
                                      tcppkt.SourcePort.ToString() + " to " + tcppkt.DestIP.ToString() +
                                      ":" + tcppkt.DestPort.ToString();
                        return ps;
                    }
                    else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                    {
                        if (log)
                            message = " TCP packet from " + tcppkt.SourceIP.ToString() + ":" +
                                tcppkt.SourcePort.ToString() + " to " + tcppkt.DestIP.ToString() + ":" +
                                tcppkt.DestPort.ToString();
                        return ps;
                    }
                }
            }
            return PacketStatus.UNDETERMINED;
        }

        string message = null;
        public string GetLogMessage()
        {
            if (!log)
                return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string ToFileString()
        {
            return null;
        }

        public bool Notify()
        {
            return notify;
        }
    }

    [Serializable]
    public class TCPPortRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public List<int> port;
        public bool log = true;
        public bool notify = true;

        [NonSerialized]
        List<PortRange> in_port_ranges = null;
        PortRange[] prs = new PortRange[0];

        public List<PortRange> port_ranges
        {
            get
            {
                if (in_port_ranges == null)
                {
                    in_port_ranges = new List<PortRange>(prs);
                }
                return in_port_ranges;
            }
            set
            {
                in_port_ranges = value;
                prs = in_port_ranges.ToArray();
            }
        }

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " TCP port(s) " + GetPortString();
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += " and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public TCPPortRule(PacketStatus ps, List<int> port, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.port = port;
            this.log = log;
        }

        public TCPPortRule()
        {
            port = new List<int>();
            port_ranges = new List<PortRange>();
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.ContainsLayer(Protocol.TCP))
            {
                TCPPacket tcppkt = (TCPPacket)pkt;
                if (tcppkt.SYN && !(tcppkt.ACK))
                {
                    if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                    {
                        if (port.Contains(tcppkt.DestPort) ||
                             inPortRange(tcppkt.DestPort))
                        {
                            if (log)
                                message = " TCP packet from " + tcppkt.SourceIP.ToString() +
                                    ":" + tcppkt.SourcePort.ToString() + " to " +
                                    tcppkt.DestIP.ToString() + ":" + tcppkt.DestPort.ToString();
                            return ps;
                        }
                    }
                    else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                    {
                        if (port.Contains(tcppkt.DestPort) ||
                            inPortRange(tcppkt.DestPort))
                        {
                            if (log)
                                message = " TCP packet from " + tcppkt.SourceIP.ToString() +
                                    ":" + tcppkt.SourcePort.ToString() + " to " + tcppkt.DestIP.ToString() +
                                    ":" + tcppkt.DestPort.ToString();
                            return ps;
                        }
                    }
                }
            }
            return PacketStatus.UNDETERMINED;
        }

        /// <summary>
        /// iterate through the port_ranges object to find
        /// if the incoming port is allowed
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private bool inPortRange(int port)
        {
            foreach (PortRange p in port_ranges)
            {
                if (p.isInRange(port))
                    return true;
            }
            return false;
        }

        public string String
        {
            get { return ToString(); }
        }

        string message = null;
        public string GetLogMessage()
        {
            if (!log)
                return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string ToFileString()
        {
            return null;
        }

        /// <summary>
        /// Returns the array of ports as a single string.
        /// 
        /// This takes the list of ints and port ranges, converts it to a list of strings, returns it as an array
        /// and finally joins all elements together with a space.
        /// </summary>
        /// <returns></returns>
        public string GetPortString()
        {
            string tmp = String.Join(" ", port.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            // for formatting sake, add a space between ports and ranges
            if (tmp.Length > 0 && port_ranges.Count > 0)
                tmp += " ";
            tmp += String.Join(" ", port_ranges.ConvertAll<string>(delegate(PortRange i) { return i.ToString(); }).ToArray());
            return tmp;
        }

        public bool Notify()
        {
            return notify;
        }
    }

    [Serializable]
    public class TCPIPPortRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public int port;
        public IPAddress ip;
        public bool log = true;
        public bool notify = true;

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " TCP " + ip.ToString() + ":" + port.ToString();
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += " and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public TCPIPPortRule(PacketStatus ps, IPAddress ip, int port, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.port = port;
            this.ip = ip;
            this.log = log;
            this.notify = notify;
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.ContainsLayer(Protocol.TCP))
            {
                TCPPacket tcppkt = (TCPPacket)pkt;
                if (tcppkt.SYN && !(tcppkt.ACK))
                {
                    if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                    {
                        if ((tcppkt.DestPort == port) && (tcppkt.DestIP.Equals(ip)))
                        {
                            if (log)
                                message = " TCP packet from " + tcppkt.SourceIP.ToString() + ":" +
                                    tcppkt.SourcePort.ToString() + " to " + tcppkt.DestIP.ToString() +
                                    ":" + tcppkt.DestPort.ToString();
                            return ps;
                        }
                    }
                    else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                    {
                        if ((tcppkt.DestPort == port) && (tcppkt.DestIP.Equals(ip)))
                        {
                            if (log)
                                message = " TCP packet from " + tcppkt.SourceIP.ToString() +
                                    ":" + tcppkt.SourcePort.ToString() + " to " + tcppkt.DestIP.ToString() +
                                    ":" + tcppkt.DestPort.ToString();
                            return ps;
                        }
                    }
                }
            }
            return PacketStatus.UNDETERMINED;
        }

        public string String
        {
            get { return ToString(); }
        }

        string message = null;
        public string GetLogMessage()
        {
            if (!log)
                return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string ToFileString()
        {
            return null;
        }

        public bool Notify()
        {
            return notify;
        }
    }

    [Serializable]
    public class UDPPortRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public List<int> port;
        public bool log = true;
        public bool notify = true;

        [NonSerialized]
        List<PortRange> in_port_ranges = null;
        PortRange[] prs = new PortRange[0];

        public List<PortRange> port_ranges
        {
            get
            {
                if (in_port_ranges == null)
                {
                    in_port_ranges = new List<PortRange>(prs);
                }
                return in_port_ranges;
            }
            set
            {
                in_port_ranges = value;
                prs = in_port_ranges.ToArray();
            }
        }

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " UDP port(s) " + GetPortString();
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += " and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public UDPPortRule(PacketStatus ps, List<int> port, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.port = port;
            this.log = log;
            this.notify = notify;
        }

        public UDPPortRule()
        {
            port = new List<int>();
            port_ranges = new List<PortRange>();
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.ContainsLayer(Protocol.UDP))
            {
                UDPPacket udppkt = (UDPPacket)pkt;
                if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                {
                    if (port.Contains(udppkt.DestPort) ||
                        inPortRange(udppkt.DestPort))
                    {
                        if (log)
                            message = " UDP packet from " + udppkt.SourceIP.ToString() +
                                ":" + udppkt.SourcePort.ToString() + " to " + udppkt.DestIP.ToString() +
                                ":" + udppkt.DestPort.ToString();
                        return ps;
                    }
                }
                else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                {
                    if (port.Contains(udppkt.DestPort) ||
                        inPortRange(udppkt.DestPort))
                    {
                        if (log)
                            message = " UDP packet from " + udppkt.SourceIP.ToString() +
                                ":" + udppkt.SourcePort.ToString() + " to " + udppkt.DestIP.ToString() +
                                ":" + udppkt.DestPort.ToString();
                        return ps;
                    }
                }
            }
            return PacketStatus.UNDETERMINED;
        }

        /// <summary>
        /// iterates through the rule's port ranges and checks if the given
        /// port is within that range
        /// </summary>
        /// <param name="port">the port to check</param>
        /// <returns></returns>
        private bool inPortRange(int port)
        {
            foreach (PortRange p in port_ranges)
            {
                if (p.isInRange(port))
                    return true;
            }
            return false;
        }

        public string String
        {
            get { return ToString(); }
        }

        string message = null;
        public string GetLogMessage()
        {
            if (!log)
                return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string ToFileString()
        {
            return null;
        }

        /// <summary>
        /// Returns the array of ports as a single string.
        /// 
        /// This takes the list of ints and port ranges, converts it to a list of strings, returns it as an array
        /// and finally joins all elements together with a space.
        /// </summary>
        /// <returns></returns>
        public string GetPortString()
        {
            string tmp = String.Join(" ", port.ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray());
            // for formatting sake, add a space between ports and ranges
            if (tmp.Length > 0 && port_ranges.Count > 0)
                tmp += " ";
            tmp += String.Join(" ", port_ranges.ConvertAll<string>(delegate(PortRange i) { return i.ToString(); }).ToArray());
            return tmp;
        }

        public bool Notify()
        {
            return notify;
        }
    }

    [Serializable]
    public class UDPAllRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public bool log = true;
        public bool notify = true;

        public UDPAllRule(PacketStatus ps, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.log = log;
            this.notify = notify;
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.ContainsLayer(Protocol.UDP))
            {
                UDPPacket udppkt = (UDPPacket)pkt;
                if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                {
                    if (log)
                        message = " UDP packet from " + udppkt.SourceIP.ToString() + ":" +
                            udppkt.SourcePort.ToString() + " to " + udppkt.DestIP.ToString() +
                            ":" + udppkt.DestPort.ToString();
                    return ps;
                }
                else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                {
                    if (log)
                        message = " UDP packet from " + udppkt.SourceIP.ToString() + ":" +
                            udppkt.SourcePort.ToString() + " to " + udppkt.DestIP.ToString() + ":" + udppkt.DestPort.ToString();
                    return ps;
                }
            }
            return PacketStatus.UNDETERMINED;
        }

        string message = null;
        public string GetLogMessage()
        {
            if (!log) return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string String
        {
            get { return ToString(); }
        }

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " all UDP ports";
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += " and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public string ToFileString()
        {
            return null;
        }

        public bool Notify()
        {
            return notify;
        }
    }

    [Serializable]
    public class AllRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public bool log = true;
        public bool notify = true;

        public AllRule(PacketStatus ps, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.log = log;
            this.notify = notify;
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
            {
                if (log)
                    message = " packet";
                return ps;
            }
            else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
            {
                if (log)
                    message = " packet";
                return ps;
            }
            return PacketStatus.UNDETERMINED;
        }

        string message = null;
        public string GetLogMessage()
        {
            if (!log) return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string String
        {
            get { return ToString(); }
        }

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " all";
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += " and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public string ToFileString()
        {
            return null;
        }

        public bool Notify()
        {
            return notify;
        }
    }

    [Serializable]
    public class IPRule : Rule
    {
        public PacketStatus ps;
        public Direction direction;
        public bool log = true;
        public bool notify = true;
        public List<IPAddress> ips = new List<IPAddress>();
        string message = "";

        public IPRule() { }

        public IPRule(PacketStatus ps, List<IPAddress> ip, Direction direction, bool log, bool notify)
        {
            this.ps = ps;
            this.direction = direction;
            this.ips = ip;
            this.log = log;
            this.notify = notify;
        }

        public PacketStatus GetStatus(Packet pkt)
        {
            if (pkt.ContainsLayer(Protocol.IP))
            {
                IPPacket tcppkt = (IPPacket)pkt;
                if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                {
                    if ( ips.Contains(tcppkt.DestIP))
                    {
                        if (log)
                            message = " IP packet from " + tcppkt.SourceIP.ToString() + " to " + tcppkt.DestIP.ToString();
                        return ps;
                    }
                }
                else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                {
                    if ( ips.Contains(tcppkt.DestIP))
                    {
                        if (log)
                            message = " IP packet from " + tcppkt.SourceIP.ToString() + " to " + tcppkt.DestIP.ToString();
                        return ps;
                    }
                }
            }
            return PacketStatus.UNDETERMINED;
        }

        public string ToFileString()
        {
            return null;
        }

        public string GetLogMessage()
        {
            if (!log) return null;
            if (ps == PacketStatus.ALLOWED)
            {
                return "Allowed " + message;
            }
            return "Blocked " + message;
        }

        public string String
        {
            get { return ToString(); }
        }

        public string ToString()
        {
            string ret = "";
            if (ps == PacketStatus.ALLOWED)
            {
                ret = "Allows";
            }
            else
            {
                ret = "Blocks";
            }
            ret += " IP " + GetIPString();
            if (direction == (Direction.IN | Direction.OUT))
            {
                ret += " in and out";
            }
            else if (direction == Direction.OUT)
            {
                ret += " out";
            }
            else if (direction == Direction.IN)
            {
                ret += " in";
            }
            if (notify)
                ret += "and notifies";
            if (log)
                ret += " and logs";
            return ret;
        }

        public bool Notify()
        {
            return notify;
        }


        /// <summary>
        /// Returns the array of IPs as a single string.
        /// 
        /// This takes the list of IPAddress's, converts it to a list of strings, returns it as an array
        /// and finally joins all elements together with a space.
        /// </summary>
        /// <returns></returns>
        public string GetIPString()
        {
            return String.Join(" ", ips.ConvertAll<string>(delegate(IPAddress i) { return i.ToString(); }).ToArray());
        }
    }

    #endregion
}
