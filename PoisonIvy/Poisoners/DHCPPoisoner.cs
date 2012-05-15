using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using FM;

namespace PoisonIvy
{
    class DHCPPoisoner : Poisoner
    {
        public override void poison(IPAddress from, IPAddress to)
        {
            throw new NotImplementedException();
        }
        public override PacketMainReturn handlePacket(Packet in_packet )
        {
            throw new NotImplementedException();
        }
    }
}
