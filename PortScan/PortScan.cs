using System;
using System.Collections.Generic;
using System.Text;
using FM;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;

//Part of my(schizo's) night of assualting Damn Vulnerable Linux with firebwall
namespace PortScan
{
    public class PortScanner : FirewallModule
    {
        public PortScanner()
            : base()
        {
            MetaData.Author = "schizo";
            MetaData.Name = "PortScanner";
            MetaData.Version = "0.0.0.1";
            MetaData.HelpString = "";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Description = "A crude port scanner that was created as part of my attack on damn vulnerable linux using only firebwall.";
        }

        void ScanThread()
        {
            for (int x = 0; x < ushort.MaxValue; x++)
            {
                EthPacket e = new EthPacket(60);
                e.FromMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
                e.ToMac = PhysicalAddress.Parse("080027465EDE").GetAddressBytes();
                e.Proto = new byte[2] { 0x08, 0x00 };
                IPPacket ip = new IPPacket(e);
                ip.DestIP = IPAddress.Parse("192.168.1.4");
                ip.SourceIP = IPAddress.Parse("192.168.1.3");
                ip.NextProtocol = 0x06;
                ip.TotalLength = 40;
                ip.HeaderChecksum = ip.GenerateIPChecksum;
                TCPPacket tcp = new TCPPacket(ip);
                tcp.SourcePort = (ushort)new Random().Next(65534);
                tcp.DestPort = (ushort)x;
                tcp.SequenceNumber = (uint)new Random().Next();
                tcp.AckNumber = 0;
                tcp.WindowSize = 8192;
                tcp.SYN = true;
                tcp.Checksum = tcp.GenerateChecksum;
                tcp.Outbound = true;
                adapter.SendPacket(tcp);
                Thread.Sleep(1);
            }
        }

        public override ModuleError ModuleStart()
        {
            new Thread(ScanThread).Start();
            return new ModuleError() { errorType = ModuleErrorType.Success };
        }

        public override ModuleError ModuleStop()
        {
            return new ModuleError() { errorType = ModuleErrorType.Success };
        }

        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            if (!in_packet.Outbound && in_packet.ContainsLayer(Protocol.TCP))
            {
                TCPPacket tcp = (TCPPacket)in_packet;
                if (IPAddress.Parse("192.168.1.4").Equals(tcp.SourceIP))
                {
                    if (tcp.SYN && tcp.ACK)
                    {
                        PacketMainReturn pmr = new PacketMainReturn("PortScanner") { logMessage = "Port " + tcp.SourcePort.ToString() + " is open on " + tcp.SourceIP.ToString(), returnType = PacketMainReturnType.Drop | PacketMainReturnType.Log };
                        return pmr;
                    }
                }
            }
            return null;
        }
    }
}
