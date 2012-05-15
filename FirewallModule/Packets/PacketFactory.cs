using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;

namespace FM
{
    public static class PacketFactory
    {
        public static TCPPacket MakeSynPacket(byte[] fromMac, byte[] toMac, byte[] fromIP, byte[] toIP, ushort fromPort, ushort toPort)
        {
            EthPacket e = new EthPacket(60);
            e.FromMac = fromMac;
            e.ToMac = toMac;
            e.Proto = new byte[2] { 0x08, 0x00 };
            IPPacket ip = new IPPacket(e);
            ip.DestIP = new IPAddress(fromIP);
            ip.SourceIP = new IPAddress(toIP);
            ip.NextProtocol = 0x06;
            ip.TotalLength = 40;
            ip.HeaderChecksum = ip.GenerateIPChecksum;
            TCPPacket tcp = new TCPPacket(ip);
            tcp.SourcePort = fromPort;
            tcp.DestPort = toPort;
            tcp.SequenceNumber = (uint)new Random().Next();
            tcp.AckNumber = 0;
            tcp.WindowSize = 8192;
            tcp.SYN = true;
            tcp.Checksum = tcp.GenerateChecksum;
            tcp.Outbound = true;
            return tcp;
        }

        public static TCPPacket MakeSynPacket(INetworkAdapter fromAdapter, byte[] toMac, byte[] toIP, ushort fromPort, ushort toPort)
        {
            byte[] fromIP = null;
            foreach (UnicastIPAddressInformation ip in fromAdapter.InterfaceInformation.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    fromIP = ip.Address.GetAddressBytes();
                }
            }
            return MakeSynPacket(fromAdapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes(), toMac, fromIP, toIP, fromPort, toPort);
        }

        public static TCPPacket MakePortClosedPacket(byte[] fromMac, byte[] toMac, byte[] fromIP, byte[] toIP, ushort fromPort, ushort toPort, uint ackNumber)
        {
            EthPacket e = new EthPacket(60);
            e.FromMac = fromMac;
            e.ToMac = toMac;
            e.Proto = new byte[2] { 0x08, 0x00 };
            IPPacket ip = new IPPacket(e);
            ip.DestIP = new IPAddress(fromIP);
            ip.SourceIP = new IPAddress(toIP);
            ip.NextProtocol = 0x06;
            ip.TotalLength = 40;
            ip.HeaderChecksum = ip.GenerateIPChecksum;
            TCPPacket tcp = new TCPPacket(ip);
            tcp.SourcePort = fromPort;
            tcp.DestPort = toPort;
            tcp.SequenceNumber = (uint)0;
            tcp.AckNumber = ackNumber;
            tcp.WindowSize = 8192;
            tcp.ACK = true;
            tcp.RST = true;
            tcp.Checksum = tcp.GenerateChecksum;
            tcp.Outbound = true;
            return tcp;
        }

        public static TCPPacket MakePortClosedPacket(TCPPacket in_packet)
        {
            return MakePortClosedPacket(in_packet.ToMac, in_packet.FromMac, in_packet.DestIP.GetAddressBytes(), in_packet.SourceIP.GetAddressBytes(), in_packet.DestPort, in_packet.SourcePort, in_packet.SequenceNumber);
        }
    }
}
