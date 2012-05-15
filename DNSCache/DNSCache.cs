using System;
using System.Collections.Generic;
using System.Text;
using FM;
using System.Net;

namespace DNSCache
{
    public class DNSCache : FirewallModule
    {
        public DNSCache() : base()
        {
            MetaData.Name = "DNSCache";
            MetaData.Author = "Brian W. (schizo)";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Description = "Caches your DNS query responses, so in the rare chance that DNS servers go down or get lagged, you will be mostly unaffected.";
            MetaData.HelpString = "";
            MetaData.Version = "0.1.0.0";
        }

        public SerializableDictionary<string, DNSPacket.DNSAnswer[]> cache = new SerializableDictionary<string, DNSPacket.DNSAnswer[]>();

        public SerializableDictionary<string, DNSPacket.DNSAnswer[]> GetCache()
        {
            lock (cache)
            {
                return new SerializableDictionary<string, DNSPacket.DNSAnswer[]>(cache);
            }
        }

        public void ClearCache()
        {
            lock (cache)
            {
                cache.Clear();
            }
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            return new DNSCacheUI(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }

        public override ModuleError ModuleStart()
        {
            lock (cache)
            {
                this.LoadConfig();
                if (this.PersistentData != null)
                {
                    cache = (SerializableDictionary<string, DNSPacket.DNSAnswer[]>)PersistentData;
                }
                else
                    cache = new SerializableDictionary<string, DNSPacket.DNSAnswer[]>();
                return new ModuleError() { errorType = ModuleErrorType.Success };
            }
        }

        public override ModuleError ModuleStop()
        {
            lock (cache)
            {
                PersistentData = cache;
                this.SaveConfig();
                return new ModuleError() { errorType = ModuleErrorType.Success };
            }
        }

        public event System.Threading.ThreadStart CacheUpdate;

        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            if (in_packet.Outbound && in_packet.ContainsLayer(Protocol.DNS))
            {
                DNSPacket dns = (DNSPacket)in_packet;
                lock (cache)
                {
                    DNSPacket.DNSAnswer[] answer;
                    if (dns.Queries.Length > 0 && cache.TryGetValue(dns.Queries[0].ToString(), out answer))
                    {
                        DNSPacket.DNSAnswer[] answers = answer;
                        dns.Answers = answers;
                        dns.DNSFlags = 0x8180;
                        dns.UDPLength = (ushort)(8 + dns.LayerLength());
                        dns.TotalLength = (ushort)(20 + dns.UDPLength);
                        dns.UDPChecksum = dns.GenerateUDPChecksum;
                        dns.Outbound = false;
                        IPAddress temp = dns.SourceIP;
                        dns.SourceIP = dns.DestIP;
                        dns.DestIP = temp;
                        ushort t = dns.SourcePort;
                        dns.SourcePort = dns.DestPort;
                        dns.DestPort = t;
                        dns.HeaderChecksum = dns.GenerateIPChecksum;
                        dns.UDPChecksum = dns.GenerateUDPChecksum;
                        return new PacketMainReturn("DNSCache") { returnType = PacketMainReturnType.Edited };
                    }
                    else
                        return null;
                }
            }
            else if (!in_packet.Outbound && in_packet.ContainsLayer(Protocol.DNS))
            {
                lock (cache)
                {
                    DNSPacket dns = (DNSPacket)in_packet;
                    cache[dns.Queries[0].ToString()] = dns.Answers;
                }
                if (CacheUpdate != null)
                    new System.Threading.Thread(CacheUpdate).Start();
                return null;
            }
            return null;
        }
    }
}
