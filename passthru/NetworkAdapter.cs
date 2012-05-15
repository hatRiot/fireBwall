using System;
using System.Collections.Generic;
using System.Text;
using NdisapiSpace;
using System.Threading;
using Win32APISPace;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using FM;

namespace PassThru
{
	public class NetworkAdapter : INetworkAdapter
    {
        static List<NetworkAdapter> currentAdapters = new List<NetworkAdapter>();

        public BandwidthCounter InBandwidth = new BandwidthCounter();
        public BandwidthCounter OutBandwidth = new BandwidthCounter();

		public NetworkAdapter(IntPtr adapterHandle, string name) 
        {
			this.adapterHandle = adapterHandle;
			ndisDeviveName = name;
			ndisDeviveName = this.ndisDeviveName.Substring(0, this.ndisDeviveName.IndexOf((char)0x00));
			foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
			{
					if (name.StartsWith("\\DEVICE\\" + ni.Id))
					{
							inter = ni;
							ndisDeviveName = ni.Id;
					}
			}
		}

        void UpdateNetworkInterface(string name)
        {
            ndisDeviveName = name;
            ndisDeviveName = this.ndisDeviveName.Substring(0, this.ndisDeviveName.IndexOf((char)0x00));
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ndisDeviveName.EndsWith(ni.Id))
                {
                    inter = ni;
                    ndisDeviveName = ni.Id;
                }
            }
        }

		NetworkAdapter() 
        {
			lock (staticPadlock)
			{
					UpdateAdapterList();
					//return new List<NetworkAdapter>(currentAdapters);
			}
		}

		public enum AdapterStartReturn
		{
				NoError,
				Error
		}
		IntPtr adapterHandle;
		ManualResetEvent hEvent = null;
		NetworkInterface inter = null;
		ADAPTER_MODE mode = new ADAPTER_MODE();
        public ModuleList modules;
		string ndisDeviveName = "";
		object padlock = new object();
		bool processing = false;
		Thread processingThread = null;
        PcapFileWriter pcaplog;
		static IntPtr hNdisapi = IntPtr.Zero;
		static bool isNdisFilterDriverOpen = false;
		static object staticPadlock = new object();
        bool enabled = true;
        Queue<Packet> processQueue = new Queue<Packet>();

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

		public static void ShutdownAll() {
			lock (staticPadlock)
			{
					CloseAllInterfaces();
					CloseNDISDriver();
			}
		}

		public AdapterStartReturn StartProcessing() {
			SetAdapterMode();
			SetPacketEvent();
			processingThread = new Thread(ProcessLoop);
            processingThread.Name = "ProcessLoop for " + this.InterfaceInformation.Name;
			processing = true;
			processingThread.Start();
			return AdapterStartReturn.NoError;
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

		static void CloseAllInterfaces() 
        {
			foreach (NetworkAdapter na in currentAdapters)
			{
				na.StopProcessing();
			}
		}

		static void CloseNDISDriver() {
			Ndisapi.CloseFilterDriver(hNdisapi);
		}

		static void OpenNDISDriver() 
        {
			if (hNdisapi != IntPtr.Zero)
			{
				LogCenter.Instance.Push("NetworkAdapter-static", "Bad state was found, attempting to open the NDIS Filter Driver while the IntPtr != IntPtr.Zero, continuing");
			}

			hNdisapi = Ndisapi.OpenFilterDriver(Ndisapi.NDISRD_DRIVER_NAME);                        
            TCP_AdapterList adList = new TCP_AdapterList();
            Ndisapi.GetTcpipBoundAdaptersInfo(hNdisapi, ref adList);
            if (adList.m_nAdapterCount == 0)
            {
                LogCenter.WriteErrorLog(new Exception("No adapters found on this driver interface"));
                return;
            }
			isNdisFilterDriverOpen = true;
		}

		unsafe void ProcessLoop() 
        {
			// Allocate and initialize packet structures
            ETH_REQUEST Request = new ETH_REQUEST();
            INTERMEDIATE_BUFFER PacketBuffer = new INTERMEDIATE_BUFFER();

            IntPtr PacketBufferIntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(PacketBuffer));
			try
			{
                win32api.ZeroMemory(PacketBufferIntPtr, Marshal.SizeOf(PacketBuffer));

                Request.hAdapterHandle = adapterHandle;
                Request.EthPacket.Buffer = PacketBufferIntPtr;

                modules = new ModuleList(this);                

                modules.LoadExternalModules();

                modules.UpdateModuleOrder();

                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + System.IO.Path.DirectorySeparatorChar + "firebwall";
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
                folder = folder + System.IO.Path.DirectorySeparatorChar + "pcapLogs";
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);
                string f = folder + System.IO.Path.DirectorySeparatorChar + "blocked-" + this.InterfaceInformation.Name + "-" + PcapCreator.Instance.GetNewDate() + ".pcap";
                pcaplog = new PcapFileWriter(f);

                INTERMEDIATE_BUFFER* PacketPointer;
                
				while (true)
				{
					hEvent.WaitOne();
                    while (Ndisapi.ReadPacket(hNdisapi, ref Request))
                    {
                        PacketPointer = (INTERMEDIATE_BUFFER*)PacketBufferIntPtr;
                        //PacketBuffer = (INTERMEDIATE_BUFFER)Marshal.PtrToStructure(PacketBufferIntPtr, typeof(INTERMEDIATE_BUFFER));

                        Packet pkt = new EthPacket(PacketPointer).MakeNextLayerPacket();

                        if (pkt.Outbound)
                        {
                            OutBandwidth.AddBits(pkt.Length());
                        }
                        else
                        {
                            InBandwidth.AddBits(pkt.Length());
                        }

                        bool drop = false;
                        bool edit = false;

                        if (enabled)
                        {
                            for (int x = 0; x < modules.Count; x++)
                            {
                                FirewallModule fm = modules.GetModule(x);
                                PacketMainReturn pmr = fm.PacketMain(ref pkt);
                                if (pmr == null)
                                    continue;
                                if ((pmr.returnType & PacketMainReturnType.Log) == PacketMainReturnType.Log && pmr.logMessage != null)
                                {
                                    LogCenter.Instance.Push(pmr);
                                }
                                if ((pmr.returnType & PacketMainReturnType.Drop) == PacketMainReturnType.Drop)
                                {
                                    drop = true;
                                    break;
                                }
                                if ((pmr.returnType & PacketMainReturnType.Edited) == PacketMainReturnType.Edited)
                                {
                                    edit = true;
                                }
                            }
                        }

                        if (!drop)
                        {
                            if (pkt.Outbound)
                                Ndisapi.SendPacketToAdapter(hNdisapi, ref Request);
                            else
                                Ndisapi.SendPacketToMstcp(hNdisapi, ref Request);
                        }
                        else
                            pcaplog.AddPacket(pkt.Data(), (int)pkt.Length());
                    }

                    //OM NOM NOM PASTA!
                    while (processQueue.Count != 0)
                    {
                        Packet pkt = processQueue.Dequeue().MakeNextLayerPacket();

                        if (pkt.Outbound)
                        {
                            OutBandwidth.AddBits(pkt.Length());
                        }
                        else
                        {
                            InBandwidth.AddBits(pkt.Length());
                        }

                        bool drop = false;
                        bool edit = false;

                        if (enabled)
                        {
                            for (int x = 0; x < modules.Count; x++)
                            {
                                FirewallModule fm = modules.GetModule(x);
                                PacketMainReturn pmr = fm.PacketMain(ref pkt);
                                if (pmr == null)
                                    continue;
                                if ((pmr.returnType & PacketMainReturnType.Log) == PacketMainReturnType.Log && pmr.logMessage != null)
                                {
                                    LogCenter.Instance.Push(pmr.Module, pmr.logMessage);
                                }
                                if ((pmr.returnType & PacketMainReturnType.Drop) == PacketMainReturnType.Drop)
                                {
                                    drop = true;
                                    break;
                                }
                                if ((pmr.returnType & PacketMainReturnType.Edited) == PacketMainReturnType.Edited)
                                {
                                    edit = true;
                                }
                            }
                        }

                        if (!drop)
                        {
                            if (pkt.Outbound)
                                Ndisapi.SendPacketToAdapter(hNdisapi, ref Request);
                            else
                                Ndisapi.SendPacketToMstcp(hNdisapi, ref Request);
                        }
                        else
                            pcaplog.AddPacket(pkt.Data(), (int)pkt.Length());
                    }
					hEvent.Reset();
				}
			}
			catch (Exception tae)
			{
                Marshal.FreeHGlobal(PacketBufferIntPtr);
			}
		}

        public void ProcessPacket(Packet pkt)
        {
            processQueue.Enqueue(pkt);
        }

        public unsafe void SendPacket(Packet pkt)
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

		void SetAdapterMode() 
        {
			mode.dwFlags = Ndisapi.MSTCP_FLAG_SENT_TUNNEL | Ndisapi.MSTCP_FLAG_RECV_TUNNEL;
			mode.hAdapterHandle = adapterHandle;
			Ndisapi.SetAdapterMode(hNdisapi, ref mode);
		}

		void SetNoLongerAvailable() 
        {
			//see if any unloading needs to be done
			//stop any processing
			//save any module information
			//save any adapter information
			//reset any variables to their defaults(non static)
		}

		void SetPacketEvent() 
        {
			hEvent = new ManualResetEvent(false);
            // note that SafeWaitHandle exposes user code indirectly; 
            // a fix is, basically, to modify ndisapi, or write our own driver.
            // todo for now
			Ndisapi.SetPacketEvent(hNdisapi, adapterHandle, hEvent.SafeWaitHandle);
		}

		static void UpdateAdapterList() 
        {
            bool succeeded = false;
            while (!succeeded)
            {

                if (!isNdisFilterDriverOpen)
                {
                    OpenNDISDriver();
                }
                TCP_AdapterList adList = new TCP_AdapterList();
                Ndisapi.GetTcpipBoundAdaptersInfo(hNdisapi, ref adList);
                List<NetworkAdapter> tempList = new List<NetworkAdapter>();

                //Populate with current adapters
                List<NetworkAdapter> notFound = new List<NetworkAdapter>();
                for (int x = 0; x < currentAdapters.Count; x++)
                {
                    bool found = false;
                    for (int y = 0; y < adList.m_nAdapterCount; y++)
                    {
                        if (adList.m_nAdapterHandle[y] == currentAdapters[x].adapterHandle)
                        {
                            currentAdapters[x].UpdateNetworkInterface(Encoding.ASCII.GetString(adList.m_szAdapterNameList, y * 256, 256));
                            tempList.Add(currentAdapters[x]);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        notFound.Add(currentAdapters[x]);
                    }
                }

                //Deal with no longer existant adapters
                for (int x = 0; x < notFound.Count; x++)
                {
                    notFound[x].SetNoLongerAvailable();
                }

                //Adding any new adapters
                for (int x = 0; x < adList.m_nAdapterCount; x++)
                {
                    bool found = false;
                    for (int y = 0; y < currentAdapters.Count; y++)
                    {
                        if (adList.m_nAdapterHandle[x] == currentAdapters[y].adapterHandle)
                            found = true;
                    }
                    if (!found)
                    {
                        NetworkAdapter newAdapter = new NetworkAdapter(adList.m_nAdapterHandle[x], Encoding.ASCII.GetString(adList.m_szAdapterNameList, x * 256, 256));
                        if (newAdapter.InterfaceInformation != null)
                        {
                            tempList.Add(newAdapter);
                        }
                    }
                }

                currentAdapters = new List<NetworkAdapter>(tempList);
                succeeded = true;
            }
		}

		public NetworkInterface InterfaceInformation 
        {
			get 
            {
				return inter;
			}
		}

		public string Name 
        {
			get 
            {
				return ndisDeviveName;
			}
		}

		public string Pointer 
        {
			get 
            {
				return adapterHandle.ToString();
			}
		}

        public static List<NetworkAdapter> GetAllAdapters()
        {
            UpdateAdapterList();
            return new List<NetworkAdapter>(currentAdapters);
        }

        public static List<NetworkAdapter> GetNewAdapters()
        {
            if (!isNdisFilterDriverOpen)
            {
                OpenNDISDriver();
            }
            TCP_AdapterList adList = new TCP_AdapterList();
            Ndisapi.GetTcpipBoundAdaptersInfo(hNdisapi, ref adList);
            List<NetworkAdapter> tempList = new List<NetworkAdapter>();
            for (int x = 0; x < currentAdapters.Count; x++)
            {
                for (int y = 0; y < adList.m_nAdapterCount; y++)
                {
                    if (adList.m_nAdapterHandle[y] == currentAdapters[x].adapterHandle)
                    {
                        currentAdapters[x].UpdateNetworkInterface(Encoding.ASCII.GetString(adList.m_szAdapterNameList, y * 256, 256));
                    }
                }
            }
            for (int x = 0; x < adList.m_nAdapterCount; x++)
            {
                bool found = false;
                for (int y = 0; y < currentAdapters.Count; y++)
                {
                    if (adList.m_nAdapterHandle[x] == currentAdapters[y].adapterHandle)
                        found = true;
                }
                if (!found)
                {
                    NetworkAdapter newAdapter = new NetworkAdapter(adList.m_nAdapterHandle[x], Encoding.ASCII.GetString(adList.m_szAdapterNameList, x * 256, 256));
                    if (newAdapter.InterfaceInformation != null)
                    {
                        tempList.Add(newAdapter);
                        currentAdapters.Add(newAdapter);
                    }
                }
            }

            return tempList;
        }        
    }
}
