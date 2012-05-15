using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using FM;
using System.Runtime.InteropServices;

namespace MacFilter
{
    public class MacFilterModule : FirewallModule
    {
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

        [Serializable]
        public class MacRule
        {
            public PacketStatus ps;
            public byte[] mac;
            public Direction direction;
            public bool log = true;
            public bool notify = true;

            public string String
            {
                get { return ToString(); }
            }

            public override string ToString()
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
                if (mac != null)
                    ret += " MAC " + new PhysicalAddress(mac).ToString();
                else
                    ret += " all MACs ";
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

            public MacRule(PacketStatus ps, Direction direction, bool log, bool notify)
            {
                this.ps = ps;
                this.mac = null;
                this.direction = direction;
                this.log = log;
                this.notify = notify;
            }

            public MacRule(PacketStatus ps, PhysicalAddress mac, Direction direction, bool log, bool notify)
            {
                this.ps = ps;
                this.mac = mac.GetAddressBytes();
                this.direction = direction;
                this.log = log;
                this.notify = notify;
            }

            [DllImport("msvcrt.dll")]
            static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);
            bool Compare(byte[] a, byte[] b)
            {
                // compare buffers
                return a.Length == b.Length && memcmp(a, b, (UIntPtr)a.Length) == 0;
            }

            public PacketStatus GetStatus(Packet pkt)
            {
                EthPacket epkt = (EthPacket)pkt;
                if (pkt.Outbound && (direction & Direction.OUT) == Direction.OUT)
                {
                    if (mac == null || Compare(mac, epkt.ToMac))
                    {
                        if (log)
                            message = "packet from " + new PhysicalAddress(epkt.FromMac).ToString() +
                                " to " + new PhysicalAddress(epkt.ToMac).ToString();
                        return ps;
                    }
                }
                else if (!pkt.Outbound && (direction & Direction.IN) == Direction.IN)
                {
                    if (mac == null || Compare(mac, epkt.FromMac))
                    {
                        if (log)
                            message = "packet from " + new PhysicalAddress(epkt.FromMac).ToString() +
                                " to " + new PhysicalAddress(epkt.ToMac).ToString();
                        return ps;
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
        }

        public MacFilterModule()
            : base()
        {
            MetaData.Name = "MAC Address Filter";
            MetaData.Version = "1.1.0.0";
            MetaData.HelpString = "Each network adapter has a MAC address.  It can only be changed or faked in rare circumstances."
                + "Each packet sent over the network says the MAC its from and the MAC its to."
                + "This module allows you to control which MAC you will send or recieve data from.  Similarly to the Basic Firewall, the rules are processed in order from top to bottom.  You can also reorder the rules by clicking move up and move down.  To add a rule, click on Add Rule, and to remove one, click Remove Rule.";
            MetaData.Description = "Blocks or allows packets based on MAC address";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Author = "Brian W. (schizo)";
        }

        readonly object padlock = new object();
        public List<MacRule> rules = new List<MacRule>();

        public override ModuleError ModuleStart()
        {
            LoadConfig();
            rules = new List<MacRule>();
            lock (padlock)
            {
                if (PersistentData != null)
                {
                    rules.AddRange((MacRule[])PersistentData);
                }
                else
                    rules = new List<MacRule>();
            }

            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override ModuleError ModuleStop()
        {
            PersistentData = rules.ToArray();
            SaveConfig();
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            return new MacFilterControl(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }

        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            lock (padlock)
            {
                PacketStatus status = PacketStatus.UNDETERMINED;
                foreach (MacRule r in rules)
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
                        if (r.notify)
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

        public void InstanceGetRuleUpdates(List<MacRule> r)
        {
            lock (padlock)
            {
                rules = new List<MacRule>(r);
            }
        }
    }
}
