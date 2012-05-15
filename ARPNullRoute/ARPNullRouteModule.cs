using System;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using FM;

namespace ARPNullRoute
{
    class ARPNullRouteModule : FirewallModule
    {
        Thread t;

        public ARPNullRouteModule() : base()
        {
            MetaData.Name = "ARP Null Route";
            MetaData.Version = "0.0.0.1";
            MetaData.HelpString = "There is no help at this time";
            MetaData.Description = "Example for sending a packet generated inside the firewall";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Author = "Brian W.";
        }

        public override ModuleError ModuleStart()
        {            
            t = new Thread(threadMain);
            t.Start();
            return new ModuleError(){ errorType = ModuleErrorType.Success};
        }

        public void threadMain()
        {
            while (true)
            {
                if (Enabled)
                {
                    EthPacket ep = new EthPacket(42);
                    ep.FromMac = PhysicalAddress.Parse("F07BCB8F7AC5").GetAddressBytes();
                    ep.ToMac = PhysicalAddress.Parse("FFFFFFFFFFFF").GetAddressBytes();
                    ep.Proto = new byte[2] { 0x08, 0x06 };
                    ARPPacket arpp = new ARPPacket(ep);
                    arpp.ASenderMac = ep.FromMac;
                    arpp.ASenderIP = IPAddress.Parse("192.168.0.1");
                    arpp.ATargetMac = ep.ToMac;
                    arpp.ATargetIP = IPAddress.Parse("192.168.0.255");
                    adapter.SendPacket(arpp);
                }
                Thread.Sleep(1000);
            }
        }

        public override ModuleError ModuleStop()
        {
            if(Enabled)
                t.Abort();
            Enabled = false;            
            return new ModuleError() { errorType = ModuleErrorType.Success };
        }

        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {          
            return null;
        }
    }
}
