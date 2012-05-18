using System;
using System.Collections.Generic;
using System.Text;
using FM;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace SaveFlashVideo
{
    public class SaveFlashVideo : FirewallModule
    {
        public SaveFlashVideo()
            : base()
        {
            MetaData.Name = "Save Flash Video";
            MetaData.Version = "1.1.0.0";
            MetaData.HelpString = "Video streaming services like Youtube stream flv files over the network.  They do not allow you to just download them like a normal video, but you can sniff them from the network.  This module does just that.  If it sees a flash video being transferred over the network, it saves it to a file.  There isn't a graphical interface for this yet, or ways to configure it yet.  All files are put into MyDocs/firebwall/modules/SaveFlashVideo";
            MetaData.Description = "Dumps Flash Video streams to file";
            MetaData.Contact = "nightstrike9809@gmail.com";
            MetaData.Author = "Brian W. (schizo)";
        }

        public override ModuleError ModuleStart()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = folder + Path.DirectorySeparatorChar + "firebwall";
            folder = folder + Path.DirectorySeparatorChar + "modules" + Path.DirectorySeparatorChar;
            folder = folder + Path.DirectorySeparatorChar + "SaveFlashVideo" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return new ModuleError() { errorType = ModuleErrorType.Success };
        }

        public override ModuleError ModuleStop()
        {
            //WriteThread.Abort();
            foreach (VideoInformation vi in videos.Values)
            {
                vi.dumpThread.Abort();
            }
            videos.Clear();
            return new ModuleError() { errorType = ModuleErrorType.Success };
        }

        //Dictionary<Quad, SortedDictionary<UInt32, byte[]>> writeQueue = new Dictionary<Quad, SortedDictionary<uint, byte[]>>();
        public Dictionary<Quad, VideoInformation> videos = new Dictionary<Quad, VideoInformation>();

        public override System.Windows.Forms.UserControl GetControl()
        {
            return new SaveFlashVideoDisplay(this) { Dock = System.Windows.Forms.DockStyle.Fill };
        }

        public class VideoInformation
        {
            ulong Length = 0;
            string Type = "";
            Quad quad;
            uint NextSequence = 0;
            SwapBufferQueue<byte[]> swapQueue = new SwapBufferQueue<byte[]>();
            string outputFile = null;
            string tempFile;
            public Thread dumpThread;
            SortedDictionary<uint, byte[]> outOfOrder = new SortedDictionary<uint, byte[]>();
            bool done = false;
            ulong cached = 0;
            ulong written = 0;

            public string SourceIP
            {
                get { return quad.srcIP.ToString(); }
            }

            public string Total
            {
                get { return Length.ToString(); }
            }

            public string Written
            {
                get { return written.ToString(); }
            }

            public string Progress
            {
                get 
                {
                    if (Length == 0)
                        return "No known data length";
                    int percent = ((int)(100.0 * ((double)written)/((double)Length)));
                    if (percent > 100)
                    {
                        return "Invalid length given.";
                    }
                    return percent.ToString() + "%"; 
                }
            }

            public VideoInformation(Quad q, ulong length, uint NextSeq, string t)
            {
                quad = q;
                Length = length;
                NextSequence = NextSeq;
                Type = t;
                dumpThread = new Thread(DumpFileAsync);
                dumpThread.Start();
            }

            public void AddData(byte[] d)
            {
                swapQueue.Enqueue(d);
                NextSequence += (uint)d.Length;
                byte[] t;
                while (outOfOrder.TryGetValue(NextSequence, out t))
                {                        
                    swapQueue.Enqueue(t);
                    outOfOrder.Remove(NextSequence);
                    NextSequence += (uint)t.Length;
                }
            }

            public void AddData(byte[] d, uint s)
            {
                cached += (uint)d.Length;
                if (s != NextSequence)
                {
                    outOfOrder[s] = d;
                }
                else
                {
                    AddData(d);
                }
            }

            public bool CheckSequence(uint s)
            {
                if (s == NextSequence)
                {
                    return true;
                }
                return false;
            }

            public void SetSequence(uint s)
            {
                NextSequence = s;
            }

            public void Done()
            {
                done = true;
            }

            protected string GetMD5HashFromFile(string fileName)
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                return BitConverter.ToString(retVal).Replace("-", "");
            }

            void DumpFileAsync()
            {
                try
                {
                    string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    folder = folder + Path.DirectorySeparatorChar + "firebwall";
                    folder = folder + Path.DirectorySeparatorChar + "modules" + Path.DirectorySeparatorChar;
                    folder = folder + Path.DirectorySeparatorChar + "SaveFlashVideo" + Path.DirectorySeparatorChar;
                    if (!Directory.Exists(folder + "temp"))
                        Directory.CreateDirectory(folder + "temp");
                    string extension = ".flv";
                    switch (Type)
                    {
                        case "video/vnd.avi":
                        case "video/avi":
                        case "video/masvideo":
                        case "video/x-msvideo":
                            extension = ".avi";
                            break;
                        case "video/x-mp4":
                            extension = ".mp4";
                            break;
                        case "audio/mp4":
                            extension = ".m4a";
                            break;
                        case "audio/mpeg":
                        case "audio/MPA":
                        case "audio/mpa-robust":
                            extension = ".mp3";
                            break;
                    }
                    tempFile = folder + "temp" + Path.DirectorySeparatorChar + DateTime.Now.Ticks.ToString() + "-" + ((uint)quad.GetHashCode()).ToString() + extension;
                    FileStream bin = new FileStream(tempFile, FileMode.Append);
                    DateTime last = DateTime.Now;
                    while (!done)
                    {
                        Thread.Sleep(10);
                        if ((DateTime.Now - last).TotalSeconds > 30)
                            break;
                        Queue<byte[]> dataQueue = swapQueue.DumpBuffer();
                        foreach (byte[] t in dataQueue)
                        {
                            last = DateTime.Now;
                            bin.Write(t, 0, t.Length);
                            written += (uint)t.Length;
                        }
                    }
                    Queue<byte[]> q = swapQueue.DumpBuffer();
                    foreach (byte[] t in q)
                    {
                        last = DateTime.Now;
                        bin.Write(t, 0, t.Length);
                        written += (uint)t.Length;
                    }
                    q = swapQueue.DumpBuffer();
                    foreach (byte[] t in q)
                    {
                        last = DateTime.Now;
                        bin.Write(t, 0, t.Length);
                        written += (uint)t.Length;
                    }
                    bin.Close();
                    if (outputFile == null)
                    {
                        outputFile = folder + GetMD5HashFromFile(tempFile) + extension;
                    }
                    File.Move(tempFile, outputFile);
                }
                catch { }
            }
        }

        public override PacketMainReturn interiorMain(ref Packet in_packet)
        {
            if (in_packet.ContainsLayer(Protocol.TCP) && !in_packet.Outbound)
            {
                TCPPacket tcp = (TCPPacket)in_packet;
                Quad q = new Quad() { dstIP = tcp.DestIP, dstPort = tcp.DestPort, srcIP = tcp.SourceIP, srcPort = tcp.SourcePort };
                VideoInformation vid;
                if(videos.TryGetValue(q, out vid))
                {
                    if (tcp.SourcePort == 80 && tcp.ACK)
                    {
                        if (tcp.FIN)
                        {
                            //finish the file
                            vid.Done();
                            lock (videos)
                            {
                                videos.Remove(q);
                            }
                        }
                        else
                        {
                            //continue writing then update the openFiles dictionary
                            byte[] data = tcp.GetApplicationLayer_safe();
                            if (data != null && data.Length != 0)
                            {
                                vid.AddData(data, tcp.SequenceNumber);
                            }
                        }
                    }
                }
                else if (tcp.SourcePort == 80 && tcp.ACK)
                {
                    byte[] data = tcp.GetApplicationLayer_safe();
                    string str = ASCIIEncoding.ASCII.GetString(data);
                    if (!str.StartsWith("HTTP/1.1 200 OK"))
                        return null;
                    if (str.Contains("Content-Type: "))
                    {
                        string type = null;
                        string strLen = null;
                        if (!str.Contains("Content-Length: "))
                        {
                            strLen = "0";
                        }
                        string[] lines = str.Replace("\r","").Split("\n".ToCharArray());
                        foreach (string line in lines)
                        {
                            if (type == null && line.StartsWith("Content-Type: "))
                            {
                                type = line.Replace("Content-Type: ", "").Replace("\n", "");
                            }
                            else if (strLen == null && line.StartsWith("Content-Length: "))
                            {
                                strLen = line.Replace("Content-Length: ", "").Replace("\n", "");
                            }
                            if (type != null && strLen != null)
                                break;
                        }
                        if (type.StartsWith("video/") || type == "flv-application/octet-stream" || type.StartsWith("audio/"))
                        {
                            ulong length = ulong.Parse(strLen);
                            if (!videos.ContainsKey(q))
                            {
                                VideoInformation vi = new VideoInformation(q, length, tcp.GetNextSequenceNumber(), type);

                                if (str.Contains("\r\n\r\n"))
                                {
                                    int dataStart = str.IndexOf("\r\n\r\n") + 4;
                                    if (data.Length > dataStart)
                                    {
                                        byte[] first = new byte[data.Length - dataStart];
                                        Buffer.BlockCopy(data, dataStart, first, 0, first.Length);
                                        vi.AddData(first);
                                    }
                                }
                                vi.SetSequence(tcp.GetNextSequenceNumber());
                                lock (videos)
                                {
                                    videos[q] = vi;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
