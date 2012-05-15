using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using FM;
using System.Text.RegularExpressions;

/// Class mimics the behavior of tools like PeerBlock.  Give it some lists
/// and it'll block all outgoing (or incoming) requests from the IP addresses.
namespace PassThru
{
    public class IPGuard : FirewallModule
    {
        // all the blocked ranges
        private List<IPAddressRange> block_ranges = new List<IPAddressRange>();
        
        private List<string> available_lists = new List<string>();
        public List<string> Available_Lists
        { get { return available_lists; } set { available_lists = value; } }

        private IPGuardUI guardUI;

        public IPGuard() : base()
        {
            Help();
        }

        public IPGuard(INetworkAdapter adapt)
            : base(adapt)
        {
            Help();
        }
        
        /// <summary>
        /// returns the user control gui
        /// </summary>
        /// <returns>A UI!</returns>
        public override System.Windows.Forms.UserControl GetControl()
        {
            // generate the UI and load the available lists
            guardUI = new IPGuardUI(this) { Dock = System.Windows.Forms.DockStyle.Fill };
            return guardUI;
        }
        
        /// <summary>
        /// Start the mod, deserialize data into GuardData
        /// </summary>
        /// <returns></returns>
        public override ModuleError ModuleStart()
        {
            ModuleError error = new ModuleError();
            error.errorType = ModuleErrorType.Success;

            try
            {
                LoadConfig();
                if (PersistentData == null)
                    data = new GuardData();
                else
                    data = (GuardData)PersistentData;

                // block ranges aren't serialized, so go rebuild them with the loaded lists
                // when the module is started
                rebuild();
            }
            catch (Exception e)
            {
                error.errorMessage = e.Message;
                error.errorType = ModuleErrorType.UnknownError;
                error.moduleName = "IPGuard";
                data = new GuardData();
            }
        
            return error;
        }

        /// <summary>
        /// Stop the module, serialize the data object out
        /// </summary>
        /// <returns></returns>
        public override ModuleError ModuleStop()
        {
            ModuleError error = new ModuleError();
            error.errorType = ModuleErrorType.Success;

            try
            {
                if (!data.Save)
                {
                    data.Loaded_Lists = new List<string>();
                    available_lists = new List<string>();
                    data.logBlocked = false;
                    data.blockIncoming = false;
                }

                PersistentData = data;
                SaveConfig();
            }
            catch (Exception e)
            {
                error.errorMessage = e.Message;
                error.errorType = ModuleErrorType.UnknownError;
                error.moduleName = "IPGuard";
            }
            return error;
        }

        // serialized object for IPGuard data
        [Serializable]
        public class GuardData
        {
            private List<string> loaded_lists = new List<string>();
            public List<string> Loaded_Lists
                    { get { return loaded_lists; } set { loaded_lists = value; } }

            public bool Save = true;
            public bool logBlocked = false;
            public bool blockIncoming = false;
        }

        public GuardData data;

        /// <summary>
        /// chuck out bad packets
        /// </summary>
        /// <param name="in_packet"></param>
        /// <returns></returns>
        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            try
            {
                PacketMainReturn pmr;
                if (in_packet.ContainsLayer(Protocol.TCP))
                {
                    // cast the packet and check for SYN/outbound
                    TCPPacket packet = (TCPPacket)in_packet;
                    if (packet.SYN && packet.Outbound)
                    {
                        // check if it's blocked
                        for (int i = 0; i < block_ranges.Count; ++i)
                        {
                            // if its heading towards a blacklisted IP
                            if (block_ranges[i].IsInRange(packet.DestIP))
                            {
                                pmr = new PacketMainReturn("IPGuard");
                                // check if we should log it
                                if (this.data.logBlocked)
                                {
                                    pmr.returnType = PacketMainReturnType.Drop | PacketMainReturnType.Log;
                                    pmr.logMessage = "IPGuard has blocked outgoing packets to " + packet.DestIP;
                                }
                                else
                                    pmr.returnType = PacketMainReturnType.Drop;
                                return pmr;
                            }
                        }
                    }
                    // check if they want to block incoming packets from these addresses
                    // as well.
                    if (this.data.blockIncoming && !(packet.Outbound))
                    {
                        for (int i = 0; i < block_ranges.Count; ++i)
                        {
                            if (block_ranges[i].IsInRange(packet.SourceIP))
                            {
                                pmr = new PacketMainReturn("IPGuard");
                                // check if we should log it
                                if (this.data.logBlocked)
                                {
                                    pmr.returnType = PacketMainReturnType.Drop | PacketMainReturnType.Log;
                                    pmr.logMessage = "IPGuard has blocked incoming packets from " + packet.SourceIP;
                                }
                                else
                                    pmr.returnType = PacketMainReturnType.Drop;
                                return pmr;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error processing pmr: " + e.Message );
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
            return null;
        }
        
        /// <summary>
        /// reads in the block list and builds the obj.  Hopefully the user is smart and either
        /// downloaded the list from the de facto location, or has it formatted properly.
        /// Documented that in the help string.
        /// </summary>
        public void buildRanges(String file)
        {
            // open the file
            try
            {
                // make sure the file wasn't removed sometime between loading
                // and adding
                if (!File.Exists(file))
                    return;

                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        // PARSINGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                        // if the line isn't a comment, has stuff on it, and is formatted with a -
                        if (!line.StartsWith("#") && line.Length > 2 && 
                             line.Contains("-"))
                        {
                            string addrs = line.Substring(
                                line.LastIndexOf(":")+1, (line.Length - line.LastIndexOf(":")-1));
                            block_ranges.Add(new IPAddressRange(IPAddress.Parse(addrs.Substring(0, addrs.IndexOf("-"))),
                                                                IPAddress.Parse(addrs.Substring(addrs.IndexOf("-")+1, (addrs.Length - addrs.IndexOf("-")-1)))));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error parsing file: " + e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// method used to rebuild the blocked IP ranges when lists are removed.  
        /// there's really no 'quick' way to discern which block matches to which list
        /// aside from either a) dumping the entire thing when a list is removed and rebuilding
        /// it or b) using a hash mapping of file -> list of ranges, but then that makes 
        /// iterating through the block ranges slower. 
        /// </summary>
        public void rebuild()
        {
            // clear out the current list
            block_ranges.Clear();
            // REBULID IT
            foreach (Object s in data.Loaded_Lists)
                buildRanges(s.ToString());
        }

        // block lists are in ranges; use this to quickly discover
        // if a given IP is within the blocked range
        private class IPAddressRange
        {
            private AddressFamily addressFamily;
            private byte[] lowerBytes;
            private byte[] upperBytes;

            public IPAddressRange(IPAddress lower, IPAddress upper)
            {
                this.addressFamily = lower.AddressFamily;
                this.lowerBytes = lower.GetAddressBytes();
                this.upperBytes = upper.GetAddressBytes();
            }

            /// <summary>
            /// Check if an IP address falls within the given range
            /// </summary>
            /// <param name="addr">address to check</param>
            /// <returns>true/false</returns>
            public bool IsInRange(IPAddress addr)
            {
                // if it's not in the same addr
                if (addr.AddressFamily != this.addressFamily)
                {
                    return false;
                }

                byte[] addrBytes = addr.GetAddressBytes();

                bool lBound = true;
                bool uBound = true;

                // iterate over ip bytes in the range
                for (int i = 0; i < this.lowerBytes.Length &&
                        (lBound || uBound); ++i)
                {
                    if ((lBound && addrBytes[i] < lowerBytes[i]) ||
                        (uBound && addrBytes[i] > upperBytes[i]))
                    {
                        return false;
                    }

                    lBound &= (addrBytes[i] == lowerBytes[i]);
                    uBound &= (addrBytes[i] == upperBytes[i]);
                }
                return true;
            }

            /// <summary>
            /// Return the lower IP addr
            /// </summary>
            /// <returns>IPAddress</returns>
            public IPAddress getLower()
            {
                return new IPAddress(lowerBytes);
            }

            /// <summary>
            /// Return the upper IP addr
            /// </summary>
            /// <returns>IPAddress</returns>
            public IPAddress getUpper()
            {
                return new IPAddress(upperBytes);
            }
        }

        /// <summary>
        /// metadata 
        /// </summary>
        private void Help()
        {
            MetaData.Name = "IPGuard";
            MetaData.Author = "Bryan A.";
            MetaData.Contact = "shodivine@gmail.com";
            MetaData.Description = "Blocks IPs from given lists.";
            MetaData.Version = "1.0.0.0";
            MetaData.HelpString = "IPGuard is a module that mimics the behavior of other blocklist applications such as PeerBlock, or its predecessor PeerGuardian.  Given a correctly formatted list,"
                                  + "  IPGuard can block TCP packets, both incoming and outgoing, to a wide range of IPs.  The most widely distributed lists are typically those found on"
                                  + "www.iblocklist.com.  \n\nThese lists need to be downloaded and added to your /firebwall/modules/IPGuard folder, and then enabled in the module's GUI."
                                  + "  These lists need to be formatted in the following way: <string>:ip-ip.  If you, for example, wanted to block a single IP address, it would be"
                                  + " required to be in the following form: firebwall:66.172.10.29-66.172.10.29";
        }
    }
}
