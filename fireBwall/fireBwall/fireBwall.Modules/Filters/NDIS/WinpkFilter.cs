using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using fireBwall.Modules;
using fireBwall.Packets;
using fireBwall.Utils;
using System.Net.NetworkInformation;

namespace fireBwall.Filters.NDIS
{
    public class WinpkFilter : INDISFilter
    {
        #region Constructor

        public WinpkFilter(IntPtr hNdisapi, IntPtr adapterHandle, string name) 
        {
            this.hNdisapi = hNdisapi;
			this.adapterHandle = adapterHandle;
            name = name.Substring(0, name.IndexOf((char)0x00));
			foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
			{
                if (name.StartsWith("\\DEVICE\\" + ni.Id))
                {
                    inter = new AdapterInformation();
                    inter.InterfaceInformation = ni;
                    inter.DataIn = new BandwidthCounter();
                    inter.DataOut = new BandwidthCounter();
                }
			}
		}

        #endregion

        #region Variables

        ManualResetEvent hEvent = null;
        AdapterInformation inter = new AdapterInformation();
        ADAPTER_MODE mode = new ADAPTER_MODE();
        public ModuleList modules;
        bool processing = false;
        Thread processingThread = null;
        PcapFileWriter pcaplog;
        IntPtr hNdisapi = IntPtr.Zero;
        Queue<Packet> processQueue = new Queue<Packet>();
        public IntPtr adapterHandle = IntPtr.Zero;

        #endregion

        #region Functions

        public void UpdateNetworkInterface(string name)
        {
            name = name.Substring(0, name.IndexOf((char)0x00));
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (name.EndsWith(ni.Id))
                {
                    inter.InterfaceInformation = ni;
                }
            }
        }

        void SetAdapterMode()
        {
            mode.dwFlags = Ndisapi.MSTCP_FLAG_SENT_TUNNEL | Ndisapi.MSTCP_FLAG_RECV_TUNNEL;
            mode.hAdapterHandle = adapterHandle;
            Ndisapi.SetAdapterMode(hNdisapi, ref mode);
        }

        void SetPacketEvent()
        {
            hEvent = new ManualResetEvent(false);
            Ndisapi.SetPacketEvent(hNdisapi, adapterHandle, hEvent.SafeWaitHandle);
        }

        public void StartProcessing()
        {
            SetAdapterMode();
            SetPacketEvent();
            processingThread = new Thread(ProcessLoop);
            processingThread.Name = "ProcessLoop for " + this.inter.Name;
            processing = true;
            processingThread.Start();
        }

        public void StopProcessing()
        {
            if (processingThread != null)
            {
                processingThread.Abort();
                processing = false;
                hEvent.Close();
                modules.ShutdownAllModules();
                pcaplog.Close();
            }
        }

        public AdapterInformation GetAdapterInformation()
        {
            inter.Adapter = this;
            return inter;
        }

        public void ProcessPacket(Packets.Packet pkt)
        {
            processQueue.Enqueue(pkt);
        }

        public unsafe void SendPacket(Packets.Packet pkt)
        {
            bool worked = false;
            if (pkt.Outbound)
            {
                ETH_REQUEST Request = new ETH_REQUEST();
                INTERMEDIATE_BUFFER* ib = pkt.IB;
                Request.EthPacket.Buffer = (IntPtr)ib;
                Request.hAdapterHandle = adapterHandle;
                if (!Ndisapi.SendPacketToAdapter(hNdisapi, ref Request))
                {
                    worked = false;
                }
                else
                {
                    worked = true;
                }
            }
            else
            {
                ETH_REQUEST Request = new ETH_REQUEST();
                INTERMEDIATE_BUFFER* ib = pkt.IB;
                Request.EthPacket.Buffer = (IntPtr)ib;
                Request.hAdapterHandle = adapterHandle;
                worked = Ndisapi.SendPacketToMstcp(hNdisapi, ref Request);
            }
            if (pkt.CodeGenerated)
                pkt.ClearGeneratedPacket();
        }

        public Modules.ModuleList Modules { get; set; }

        public bool Filtering { get; set; }

        public bool DropAll { get; set; }

        public void ProcessLoop()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
