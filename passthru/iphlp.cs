using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NdisapiSpace
{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct EtherAddr {
			public byte b1;
			public byte b2;
			public byte b3;
			public byte b4;
			public byte b5;
			public byte b6;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct ETHER_HEADER {
			public EtherAddr dest;
			public EtherAddr source;
			public ushort proto;
			public const int ETH_ALEN = 6;
			public const int ETH_P_IP = 0x0800;
			public const int ETH_P_RARP = 0x8035;
			public const int ETH_P_ARP = 0x0806;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct IPHeader {
			/// <summary>
			/// header length : 4, version :4
			/// </summary>
			public byte IPLenVer;

			/// <summary>
			/// type of service
			/// </summary>
			public byte TOS;

			/// <summary>
			/// Total length
			/// </summary>
			public ushort Len;

			/// <summary>
			/// Identification
			/// </summary>
			public ushort ID;

			/// <summary>
			/// Fragment offset field
			/// </summary>
			public ushort Off;

			/// <summary>
			/// Time to live
			/// </summary>
			public byte TTL;

			/// <summary>
			/// Protocol
			/// </summary>
			public byte P;

			/// <summary>
			/// Checksum
			/// </summary>
			public ushort Sum;
			public uint Src;
			public uint Dest;
			public const int IP_DF = 0x4000;
			public const int IP_MF = 0x2000;
			public const int ETHER_HEADER_LENGTH = 14;
			public const int IPPROTO_TCP = 6;
			public const int IPPROTO_UDP = 17;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct TcpHeader {
			public ushort th_sport;
			public ushort th_dport;
			public uint Seq;
			public uint Ack;
			public byte Off;
			public byte Flags;
			public ushort Window;
			public ushort CheckSum;
			public ushort Urp;
			public const int TH_FIN = 0x01;
			public const int TH_SYN = 0x02;
			public const int TH_RST = 0x04;
			public const int TH_PSH = 0x08;
			public const int TH_ACK = 0x10;
			public const int TH_URG = 0x20;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct UdpHeader {
			public ushort th_sport;
			public ushort th_dport;
			public ushort length;
			public ushort th_sum;
		}

}
