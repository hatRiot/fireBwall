using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using FM;

namespace ARPPoisoningProtection
{
    public class ARPPP : FirewallModule
    {
        public ARPPP()
            : base()
        {
            Help();
        }

        public ARPPP(INetworkAdapter adapter)
            : base(adapter)
        {
            Help();
        }

        void Help()
        {
            MetaData.Name = "ARP Poisoning Protection";
            MetaData.Version = "1.1.0.0";
            MetaData.HelpString = "ARP Poisoning:  A local network attack that redirects communications through the attacker.  This allows them see, modify, or drop your traffic.  There is very little for protection against these attacks currently out on the market."
                + "\r\n\r\nHow we prevent it is by ignoring any ARP response packet that reaches the firewall that you computer didn't ask for.  On top of that, we also save solicited responses so if a conflicting response is returned, we can tell an attack is happening, and the firewall attempts to tell the other computer being attacked the truth."
                + "\r\n\r\nThe module configuration is quite simple.  It displays the known MAC address to IP correlations that it knows.  If one of these values is incorrect, you can remove it by clicking on it, and hitting the 'Remove Entry' button.  You can also clear the entire cache, if you switch networks or something like that.  The Save ARP Cache check box will allow you to save or not save the values for the next time fireBwall runs.  The Log Unsolicited check box lets you choose if you want to be notified of ARP Responses that your computer did not request.  The Log Attacks check box lets you choose if you want to be notified of ARP Poisoning Attacks that have been detected and defeated.  The Rectify Attack checkbox lets you choose if fireBwall tries to fix the attack on the other victim, this is extremely effective for simple attacks.";
            MetaData.Description = "Protects against ARP Poisoning attacks";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Author = "Brian W. (schizo)";
        }

        public override ModuleError ModuleStart()
        {
            LoadConfig();
            if (PersistentData == null)
            {
                data = new ArpData();
            }
            else
            {
                data = (ArpData)PersistentData;
            }
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        public override ModuleError ModuleStop()
        {
            if (!data.Save)
                data.arpCache = new SerializableDictionary<IPAddress, byte[]>();

            PersistentData = data;
            SaveConfig();
            ModuleError me = new ModuleError();
            me.errorType = ModuleErrorType.Success;
            return me;
        }

        List<int> requestedIPs = new List<int>();

        [Serializable]
        public class ArpData
        {
            public SerializableDictionary<IPAddress, byte[]> arpCache = new SerializableDictionary<IPAddress, byte[]>();
            public bool Save = true;
            public bool LogUnsolic = true;
            public bool LogAttacks = true;
            public bool RectifyAttacks = false;
        }

        public ArpData data;

        public event System.Threading.ThreadStart UpdatedArpCache;
        object padlock = new object();



        public SerializableDictionary<IPAddress, byte[]> GetCache()
        {
            lock (padlock)
            {
                return new SerializableDictionary<IPAddress, byte[]>(data.arpCache);
            }
        }

        public void UpdateCache(SerializableDictionary<IPAddress, byte[]> cache)
        {
            lock (padlock)
            {
                data.arpCache = new SerializableDictionary<IPAddress, byte[]>(cache);
            }
        }

        public override System.Windows.Forms.UserControl GetControl()
        {
            return new ArpPoisoningProtection(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }



        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            if (in_packet.GetHighestLayer() == Protocol.ARP)
            {
                ARPPacket arpp = (ARPPacket)in_packet;
                if (arpp.isRequest && arpp.Outbound)
                {
                    int ip = arpp.ATargetIP.GetHashCode();
                    if (!requestedIPs.Contains(ip))
                        requestedIPs.Add(ip);
                }
                else if (!arpp.Outbound)
                {
                    int ip = arpp.ASenderIP.GetHashCode();
                    if (!arpp.isRequest)
                    {
                        if (requestedIPs.Contains(ip))
                        {
                            lock (padlock)
                            {
                                if (data.arpCache.ContainsKey(arpp.ASenderIP))
                                {
                                    if (!Compare(data.arpCache[arpp.ASenderIP], arpp.ASenderMac))
                                    {
                                        PacketMainReturn pmr = new PacketMainReturn(this);
                                        if (data.RectifyAttacks)
                                            pmr.returnType = PacketMainReturnType.Edited;
                                        else
                                            pmr.returnType = PacketMainReturnType.Drop;
                                        if (data.LogAttacks)
                                            pmr.returnType |= PacketMainReturnType.Log | PacketMainReturnType.Popup;
                                        switch (LanguageConfig.GetCurrentLanguage())
                                        {
                                            case LanguageConfig.Language.NONE:
                                            case LanguageConfig.Language.ENGLISH:
                                                pmr.logMessage = "ARP Response from " + new PhysicalAddress(arpp.ASenderMac).ToString() + " for " + arpp.ASenderIP.ToString() + " does not match the ARP cache.";
                                                break;
                                            case LanguageConfig.Language.CHINESE:
                                                pmr.logMessage = new PhysicalAddress(arpp.ASenderMac).ToString() + "为" + arpp.ASenderIP.ToString() + "的ARP响应不匹配的ARP缓存。";
                                                break;
                                            case LanguageConfig.Language.GERMAN:
                                                pmr.logMessage = "ARP Response von " + new PhysicalAddress(arpp.ASenderMac).ToString() + " für " + arpp.ASenderIP.ToString() + " nicht mit dem ARP-Cache.";
                                                break;
                                            case LanguageConfig.Language.RUSSIAN:
                                                pmr.logMessage = "ARP-ответ от " + new PhysicalAddress(arpp.ASenderMac).ToString() + " для " + arpp.ASenderIP.ToString() + " не соответствует кэш ARP.";
                                                break;
                                            case LanguageConfig.Language.SPANISH:
                                                pmr.logMessage = "Respuesta de ARP de " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString() + " no coincide con la caché ARP.";
                                                break;
                                            case LanguageConfig.Language.PORTUGUESE:
                                                pmr.logMessage = "Resposta da ARP " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString() + " não coincide com o cache ARP.";
                                                break;
                                        }
                                        if (data.RectifyAttacks)
                                        {
                                            arpp.ATargetIP = arpp.ASenderIP;
                                            arpp.ATargetMac = data.arpCache[arpp.ATargetIP];
                                            arpp.ASenderMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
                                            arpp.FromMac = arpp.ASenderMac;
                                            arpp.ToMac = arpp.ATargetMac;
                                            foreach (UnicastIPAddressInformation ipv4 in adapter.InterfaceInformation.GetIPProperties().UnicastAddresses)
                                            {
                                                if (ipv4.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                                {
                                                    arpp.ASenderIP = ipv4.Address;
                                                    break;
                                                }
                                            }
                                            arpp.Outbound = true;
                                            in_packet = arpp;
                                        }
                                        return pmr;
                                    }
                                    else
                                    {
                                        requestedIPs.Remove(ip);
                                    }
                                }
                                else
                                {
                                    data.arpCache[arpp.ASenderIP] = arpp.ASenderMac;
                                    if (UpdatedArpCache != null)
                                        UpdatedArpCache();
                                    requestedIPs.Remove(ip);
                                }
                            }
                        }
                        else
                        {
                            lock (padlock)
                            {
                                if (data.arpCache.ContainsKey(arpp.ASenderIP))
                                {
                                    if (!Compare(data.arpCache[arpp.ASenderIP], arpp.ASenderMac))
                                    {
                                        PacketMainReturn pmra = new PacketMainReturn(this);
                                        if (data.RectifyAttacks)
                                            pmra.returnType = PacketMainReturnType.Edited;
                                        else
                                            pmra.returnType = PacketMainReturnType.Drop | PacketMainReturnType.Popup;
                                        switch (LanguageConfig.GetCurrentLanguage())
                                        {
                                            case LanguageConfig.Language.NONE:
                                            case LanguageConfig.Language.ENGLISH:
                                                pmra.logMessage = "ARP Response from " + new PhysicalAddress(arpp.ASenderMac).ToString() + " for " + arpp.ASenderIP.ToString() + " does not match the ARP cache.";
                                                break;
                                            case LanguageConfig.Language.CHINESE:
                                                pmra.logMessage = new PhysicalAddress(arpp.ASenderMac).ToString() + "为" + arpp.ASenderIP.ToString() + "的ARP响应不匹配的ARP缓存。";
                                                break;
                                            case LanguageConfig.Language.GERMAN:
                                                pmra.logMessage = "ARP Response von " + new PhysicalAddress(arpp.ASenderMac).ToString() + " für " + arpp.ASenderIP.ToString() + " nicht mit dem ARP-Cache.";
                                                break;
                                            case LanguageConfig.Language.RUSSIAN:
                                                pmra.logMessage = "ARP-ответ от " + new PhysicalAddress(arpp.ASenderMac).ToString() + " для " + arpp.ASenderIP.ToString() + " не соответствует кэш ARP.";
                                                break;
                                            case LanguageConfig.Language.SPANISH:
                                                pmra.logMessage = "Respuesta de ARP de " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString() + " no coincide con la caché ARP.";
                                                break;
                                            case LanguageConfig.Language.PORTUGUESE:
                                                pmra.logMessage = "Resposta da ARP " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString() + " não coincide com o cache ARP.";
                                                break;
                                        }
                                        if (data.RectifyAttacks)
                                        {
                                            arpp.ATargetIP = arpp.ASenderIP;
                                            arpp.ATargetMac = data.arpCache[arpp.ATargetIP];
                                            arpp.ASenderMac = adapter.InterfaceInformation.GetPhysicalAddress().GetAddressBytes();
                                            arpp.FromMac = arpp.ASenderMac;
                                            arpp.ToMac = arpp.ATargetMac;
                                            foreach (UnicastIPAddressInformation ipv4 in adapter.InterfaceInformation.GetIPProperties().UnicastAddresses)
                                            {
                                                if (ipv4.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                                {
                                                    arpp.ASenderIP = ipv4.Address;
                                                    break;
                                                }
                                            }
                                            arpp.Outbound = true;
                                            in_packet = arpp;
                                        }
                                        return pmra;
                                    }
                                }
                            }
                            PacketMainReturn pmr = new PacketMainReturn(this);
                            pmr.returnType = PacketMainReturnType.Drop;
                            if (data.LogUnsolic)
                                pmr.returnType |= PacketMainReturnType.Log;
                            switch (LanguageConfig.GetCurrentLanguage())
                            {
                                case LanguageConfig.Language.NONE:
                                case LanguageConfig.Language.ENGLISH:
                                    pmr.logMessage = "Unsolicited ARP Response from " + new PhysicalAddress(arpp.ASenderMac).ToString() + " for " + arpp.ASenderIP.ToString();
                                    break;
                                case LanguageConfig.Language.CHINESE:
                                    pmr.logMessage = "未经请求的ARP应答为" + arpp.ASenderIP.ToString() + "从" + new PhysicalAddress(arpp.ASenderMac).ToString();
                                    break;
                                case LanguageConfig.Language.GERMAN:
                                    pmr.logMessage = "Initiativbewerbung ARP Response von " + new PhysicalAddress(arpp.ASenderMac).ToString() + " für " + arpp.ASenderIP.ToString();
                                    break;
                                case LanguageConfig.Language.RUSSIAN:
                                    pmr.logMessage = "Незапрошенные ответ ARP от " + new PhysicalAddress(arpp.ASenderMac).ToString() + " для " + arpp.ASenderIP.ToString();
                                    break;
                                case LanguageConfig.Language.SPANISH:
                                    pmr.logMessage = "Respuesta ARP no solicitados de " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString();
                                    break;
                                case LanguageConfig.Language.PORTUGUESE:
                                    pmr.logMessage = "Resposta ARP não solicitadas a partir de " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString();
                                    break;
                            }
                            return pmr;
                        }
                    }
                    else
                    {
                        lock (padlock)
                        {
                            if (data.arpCache.ContainsKey(arpp.ASenderIP))
                            {
                                if (!Compare(data.arpCache[arpp.ASenderIP], arpp.ASenderMac))
                                {
                                    PacketMainReturn pmr = new PacketMainReturn(this);
                                    pmr.returnType = PacketMainReturnType.Drop;
                                    if (data.LogAttacks)
                                        pmr.returnType |= PacketMainReturnType.Log | PacketMainReturnType.Popup;
                                    switch (LanguageConfig.GetCurrentLanguage())
                                    {
                                        case LanguageConfig.Language.NONE:
                                        case LanguageConfig.Language.ENGLISH:
                                            pmr.logMessage = "ARP Response from " + new PhysicalAddress(arpp.ASenderMac).ToString() + " for " + arpp.ASenderIP.ToString() + " does not match the ARP cache.";
                                            break;
                                        case LanguageConfig.Language.CHINESE:
                                            pmr.logMessage = new PhysicalAddress(arpp.ASenderMac).ToString() + "为" + arpp.ASenderIP.ToString() + "的ARP响应不匹配的ARP缓存。";
                                            break;
                                        case LanguageConfig.Language.GERMAN:
                                            pmr.logMessage = "ARP Response von " + new PhysicalAddress(arpp.ASenderMac).ToString() + " für " + arpp.ASenderIP.ToString() + " nicht mit dem ARP-Cache.";
                                            break;
                                        case LanguageConfig.Language.RUSSIAN:
                                            pmr.logMessage = "ARP-ответ от " + new PhysicalAddress(arpp.ASenderMac).ToString() + " для " + arpp.ASenderIP.ToString() + " не соответствует кэш ARP.";
                                            break;
                                        case LanguageConfig.Language.SPANISH:
                                            pmr.logMessage = "Respuesta de ARP de " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString() + " no coincide con la caché ARP.";
                                            break;
                                        case LanguageConfig.Language.PORTUGUESE:
                                            pmr.logMessage = "Resposta da ARP " + new PhysicalAddress(arpp.ASenderMac).ToString() + " para " + arpp.ASenderIP.ToString() + " não coincide com o cache ARP.";
                                            break;
                                    }
                                    return pmr;
                                }
                            }
                        }
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        bool Compare(byte[] a, byte[] b)
        {
            // compare buffers
            return a.Length == b.Length && memcmp(a, b, a.Length) == 0;
        }
    }
}
