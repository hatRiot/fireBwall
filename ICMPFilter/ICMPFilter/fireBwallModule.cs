using System;
using System.Collections.Generic;
using System.Text;
using FM;

namespace ICMPFilter
{
    /// <summary>
    /// ICMP filter module
    /// 
    /// Allows filtering by type/code
    /// </summary>
    public class ICMPFilterModule : FirewallModule
    {
        public ICMPFilterModule()
            : base()
        {
            Help();
        }

        // return local user control
        public override System.Windows.Forms.UserControl GetControl()
        {
            return new ICMPFilterDisplay(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }

        // Action for ModuleStart
        public override ModuleError ModuleStart()
        {
            LoadConfig();
            if (PersistentData == null)
                data = new ICMPData();
            else
                data = (ICMPData)PersistentData;

            ModuleError moduleError = new ModuleError();
            moduleError.errorType = ModuleErrorType.Success;
            return moduleError;
        }

        // Action for ModuleStop
        public override ModuleError ModuleStop()
        {
            if (data != null)
            {
                if (!data.Save)
                {
                    data.RuleTable = new SerializableDictionary<string, List<string>>();
                    data.RuleTablev6 = new SerializableDictionary<string, List<string>>();
                }

                PersistentData = data;
                SaveConfig();
            }
            ModuleError moduleError = new ModuleError();
            moduleError.errorType = ModuleErrorType.Success;
            return moduleError;
        }

        /// <summary>
        /// Object used to serialize the data we need to persist.
        /// 
        /// The v4 and v6 rule tables need to be split up because of clashing types/codes
        /// </summary>
        [Serializable]
        public class ICMPData
        {
            private SerializableDictionary<string, List<string>> ruleTable = new SerializableDictionary<string, List<string>>();
            public SerializableDictionary<string, List<string>> RuleTable
            { get { return ruleTable; } set { ruleTable = new SerializableDictionary<string, List<string>>(value); } }

            private SerializableDictionary<string, List<string>> ruleTablev6 = new SerializableDictionary<string, List<string>>();
            public SerializableDictionary<string, List<string>> RuleTablev6
            { get { return ruleTablev6; } set { ruleTablev6 = new SerializableDictionary<string, List<string>>(value); } }

            private bool denyIPv4 = false;
            public bool DenyIPv4
            { get { return denyIPv4; } set { denyIPv4 = value; } }

            private bool denyIPv6 = false;
            public bool DenyIPv6
            { get { return denyIPv6; } set { denyIPv6 = value; } }

            private bool denyIPv6NDP = false;
            public bool DenyIPv6NDP
            { get { return denyIPv6NDP; } set { denyIPv6NDP = value; } }

            private bool log = true;
            public bool Log
            { get { return log; } set { log = value; } }

            public bool Save = true;
        }

        public ICMPData data;

        // main routine
        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            // if the packet is ICMPv4
            if (in_packet.GetHighestLayer() == Protocol.ICMP)
            {
                ICMPPacket packet = (ICMPPacket)in_packet;
                // check if the packet is allowed and deny all is false
                if (isAllowed(packet.Type.ToString(), packet.Code.ToString(), 4) &&
                    !data.DenyIPv4)
                {
                    return null;
                }
                // else, log and drop it
                else
                {
                    PacketMainReturn pmr;
                    pmr = new PacketMainReturn(this);
                    pmr.returnType = PacketMainReturnType.Drop;
                    if (data.Log)
                    {
                        pmr.returnType |= PacketMainReturnType.Log;
                        pmr.logMessage = "ICMP from " + packet.SourceIP.ToString() + " for " +
                            packet.DestIP.ToString() + " was dropped.";
                    }
                    return pmr;
                }
            }

            // if the packet is ICMPv6
            if (in_packet.GetHighestLayer() == Protocol.ICMPv6)
            {
                ICMPv6Packet packet = (ICMPv6Packet)in_packet;
                if ((isAllowed(packet.Type.ToString(), packet.Code.ToString(), 6) &&
                    !data.DenyIPv6) && isDeniedNDP(packet))
                {
                    return null;
                }
                else
                {
                    PacketMainReturn pmr;
                    pmr = new PacketMainReturn(this);
                    pmr.returnType = PacketMainReturnType.Drop;
                    if (data.Log)
                    {
                        pmr.returnType |= PacketMainReturnType.Log;
                        pmr.logMessage = "ICMPv6 from " + packet.SourceIP.ToString() + " for " +
                            packet.DestIP.ToString() + " was dropped.";
                    }
                    return pmr;
                }
            }
            return null;
        }

        /*
         * Method used to check whether an ICMP packet should be
         * allowed through.
         * 
         * @param type is the ICMP type
         * @param code is the ICMP code
         * @param version is the IP version to look for the type/code in
         *  Accepts 4 for ipv4 and 6 for ipv6
         */
        private bool isAllowed(string type, string code, int version)
        {
            bool isAllowed = true;

            if (version == 4)
            {
                // if the table contains the type, check if it
                // also contains the code
                if (data.RuleTable.ContainsKey(type))
                {
                    List<string> temp;
                    data.RuleTable.TryGetValue(type, out temp);
                    // invert logic; if found, disallow, if not, allow
                    isAllowed = !(temp.Contains(code));
                }
            }
            else if (version == 6)
            {
                if (data.RuleTablev6.ContainsKey(type))
                {
                    List<string> tmp;
                    data.RuleTablev6.TryGetValue(type, out tmp);
                    isAllowed = !(tmp.Contains(code));
                }

            }

            return isAllowed;
        }

        /// <summary>
        /// Checks if ICMPv6 is blocked, and then whether or not the packet is an NDP 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>Returns true if the packet is allowed, false if it is not allowed</returns>
        private bool isDeniedNDP(ICMPv6Packet packet)
        {
            // if they're denying all IPv6 except NDP
            if (data.DenyIPv6NDP)
            {
                // check if it's NDP
                if ((packet.Type <= 133) && (packet.Type >= 137))
                {
                    // it is, return allowed
                    return true;
                }
                // nope it's not, return BLOCKED
                else
                    return false;
            }

            // default true because they're not blocking ipv6 
            return true;
        }

        // module metadata
        private void Help()
        {
            MetaData.Name = "ICMP Filter";
            MetaData.Version = "1.0.3.0";
            MetaData.Description = "Blocks ICMP packets of a given type/code";
            MetaData.Contact = "shodivine@gmail.com";
            MetaData.Author = "Bryan A. (drone)";
            MetaData.HelpString = "This module can be used to block all (or particular) ICMP packets from flowing in or out of your network."
                + "ICMP packets are used to exchange error messages between networked computers, but it's also used with tools such as nmap "
                + "and nessus to gain valuable information about systems behind a gateway.\n\n"
                + "ICMP Segment Structure\n "
                + "\t|——————————————————————————————|\n"
                + "\t|  BITS  |  0-7  |  8-15  |  16-23  |  24-31 |\n"
                + "\t|——————————————————————————————|\n"
                + "\t|    0   |  Type |  Code  |  Checksum       |\n"
                + "\t|   32   |      Rest of Header                   |\n"
                + "\t|——————————————————————————————|\n"
                + "A full list of supported control messages can be found on the module page (View ICMP)."
                + "\n\nAs of .3.11, ICMPFilter differentiates between ICMPv4 and ICMPv6.  It can block all IPv4 or all IPv6 packets, as well"
                + " as in inidividual v4/v6.\n\nThe module can also now block all ICMPv6 packets EXCEPT for NDP packets.  This is because"
                + " NDP packets are required for intranet connectivity and other vital tasks.";
        }
    }
}
