using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using FM;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace SimpleMacEncryption
{
    public class SimpleMacEncryptionSend : FirewallModule
    {
        public SimpleMacEncryptionSend():base()
        {
            MetaData.Name = "Simple Mac Encryption";
            MetaData.Version = "0.1.0.0";
            MetaData.HelpString = "none";
            MetaData.Author = "Brian W. (schizo)";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Description = "";
            key = new SHA256Managed().ComputeHash(ASCIIEncoding.ASCII.GetBytes("Dear truck, CODE SOMETHING"));
            RC4 r = new RC4();
            r.Init(key);
            key = r.GetKeyState();
        }

        public override ModuleError ModuleStart()
        {
            myMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
            return new ModuleError() { errorType = ModuleErrorType.Success, moduleName = "SimpleMacEncryption" };
        }

        public override ModuleError ModuleStop()
        {
            return new ModuleError() { errorType = ModuleErrorType.Success, moduleName = "SimpleMacEncryption" };
        }

        object padlock = new object();
        byte[] theirMac = null;
        byte[] myMac = null;

        bool CompareMac(byte[] a, byte[] b)
        {
            for (int x = 0; x < 6; x++)
            {
                if (a[x] != b[x])
                    return false;
            }
            return true;
        }

        public void SetOtherMac(PhysicalAddress mac)
        {
            lock (padlock)
            {
                theirMac = mac.GetAddressBytes();
            }
        }

        public class RC4
        {
            byte[] S = new byte[256];
            int i = 0;
            int j = 0;

            void Swap()
            {
                byte temp = S[i];
                S[i] = S[j];
                S[j] = temp;
            }

            public RC4()
            {                
            }

            public void Init()
            {
                for (i = 0; i < 256; i++)
                    S[i] = (byte)i;
                i = 0;
                j = 0;
            }

            public byte[] GetKeyState()
            {
                return S;
            }

            public void SetKeyState(byte[] state)
            {
                Buffer.BlockCopy(state, 0, S, 0, 256);
            }

            public void Init(byte[] key)
            {
                Init();
                for (i = 0; i < 256; i++)
                {
                    j = (j + S[i] + key[i % key.Length]) & 0xff;
                    Swap();
                }
                i = 0;
                j = 0;
            }

            public byte Next()
            {
                i = (i + 1) & 0xff;
                j = (j + S[i]) & 0xff;
                Swap();

                return S[(S[i] + S[j]) & 0xff];
            }
        }

        RC4 rc4 = new RC4();
        byte[] key;

        public unsafe override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            lock (padlock)
            {
                if (theirMac == null)
                    return null;
                EthPacket e = (EthPacket)in_packet;                
                if ((e.Proto[0] & 0x01) == 0x01 && (CompareMac(theirMac, e.FromMac) && CompareMac(myMac, e.ToMac) || (CompareMac(theirMac, e.ToMac) && CompareMac(myMac, e.FromMac))))
                {
                    INTERMEDIATE_BUFFER* IB = e.IB;
                    rc4.SetKeyState(key);
                    INTERMEDIATE_BUFFER* iba = (INTERMEDIATE_BUFFER*)Marshal.AllocHGlobal(Marshal.SizeOf(new INTERMEDIATE_BUFFER()));
                    iba->m_dwDeviceFlags = IB->m_dwDeviceFlags;
                    iba->m_Flags = IB->m_Flags;
                    iba->m_Length = IB->m_Length;
                    iba->m_qLink = IB->m_qLink;
                    for (int x = 12; x < IB->m_Length; x++)
                    {
                        iba->m_IBuffer[x] = (byte)(IB->m_IBuffer[x] ^ rc4.Next());
                    }                   
                    EthPacket nPacket = new EthPacket(iba);
                    nPacket.FromMac = e.FromMac;
                    nPacket.ToMac = e.ToMac;
                    nPacket.Proto = new byte[2];
                    nPacket.Proto[0] = (byte)(0xFE & e.Proto[0]);
                    nPacket.Proto[1] = e.Proto[1];
                    adapter.ProcessPacket(nPacket);
                    return new PacketMainReturn("SimpleMacEncryption") { returnType = PacketMainReturnType.Drop };
                }
            }
            return null;
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            return new MacEntryControl(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }
    }
}
