using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;

namespace FM
{
    public interface INetworkAdapter
    {
        NetworkInterface InterfaceInformation
        {
            get;
        }

        string Name
        {
            get;
        }

        unsafe void SendPacket(Packet pkt);
        void ProcessPacket(Packet pkt);
    }
}
