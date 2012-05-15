using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using FM;

namespace PoisonIvy
{
    public class PoisonIvy : FirewallModule
    {
        private ARPPoisoner arpPoisoner;
        private DNSPoisoner dnsPoisoner;

        private bool isARP;
        private bool isDNS;

        public PoisonIvy() : base()
        {
            MetaData.Name = "PoisonIvy";
            MetaData.Version = "0.0.0.1";
            MetaData.HelpString = "None at this time.";
            MetaData.Description = "Poison IPs of various protocols";
            MetaData.Contact = "shodivine@gmail.com";
            MetaData.Author = "Bryan A (drone)";
        }

        public override ModuleError ModuleStart()
        {
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            
            return me;
        }

        public override ModuleError ModuleStop()
        {
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            ivyui = new PoisonIvyUI(this) { Dock = System.Windows.Forms.DockStyle.Fill };
            return ivyui;
        }

        private PoisonIvyUI ivyui;

        /// <summary>
        /// Initialize a poisoning session
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void initializePoisoner(Protocol p, IPAddress from, IPAddress to)
        {
            switch (p)
            {
                case Protocol.ARP:
                    arpPoisoner = new ARPPoisoner(this.adapter);
                    isARP = true;
                    arpPoisoner.poison(from, to);
                    break;
                case Protocol.DNS:
                    dnsPoisoner = new DNSPoisoner(this.adapter);
                    isDNS = true;
                    dnsPoisoner.poison(from, to);
                    break;
                case Protocol.DHCP:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// cease the poisoning and start rectifying an attacks if we need to
        /// </summary>
        /// <param name="p"></param>
        public void stopPoisoner(Protocol p)
        {
            switch (p)
            {
                case Protocol.ARP:
                    if (isARP)
                    {
                        arpPoisoner.isPoisoning = false;
                        arpPoisoner.rectifyAttack();
                        isARP = false;
                    }
                    break;
            }
        }
        /// <summary>
        /// This is the entry point of a packet and routes it based on what poisoners are currently running
        /// </summary>
        /// <param name="in_packet"></param>
        /// <returns></returns>
        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            if ( isARP )
            {
                return arpPoisoner.handlePacket ( in_packet );
            }
            return null;
        }
    }
}
