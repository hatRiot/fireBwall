using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using FM;
using System.Net.NetworkInformation;

// abstract class for poisoners
namespace PoisonIvy
{
    abstract class Poisoner
    {
        public bool isPoisoning;
        public bool isWaitingFROM;
        public bool isWaitingTO;

        public IPAddress from;
        public IPAddress to;
        public IPAddress local;

        public PhysicalAddress from_mac;
        public PhysicalAddress to_mac;
        public PhysicalAddress local_mac;

        public abstract void poison(IPAddress from, IPAddress to);
        public abstract PacketMainReturn handlePacket(Packet in_packet);
    }
}
