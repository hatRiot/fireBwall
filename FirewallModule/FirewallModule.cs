using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Serialization;

namespace FM
{
    /// <summary>
    /// Flags used to describe how the module handled the packet
    /// </summary>
    [Flags]
    public enum PacketMainReturnType
    {
        Error = 1,          //Reports an error in the packet processing
        Drop = 1 << 1,           //Drops the packet
        Allow = 1 << 2,          //Allows the packet to be passed on to the next module
        Edited = 1 << 3,
        Log = 1 << 4,	        //Logs the packet
        Popup = 1 << 5
    }

    /// <summary>
    /// Class to be returned from a main in a module
    /// </summary>
    public class PacketMainReturn
    {       
        /// <summary>
        /// Creates a PacketMainReturn with the basic unknown error message
        /// </summary>
        /// <param name="moduleName"></param>
        public PacketMainReturn(string moduleName)
        {
            Module = moduleName;
            returnType = PacketMainReturnType.Error | PacketMainReturnType.Log;
            logMessage = "An error has occurred in " + moduleName + " with no other details.";
        }

        /// <summary>
        /// The constructor to use for new modules, supports the more functional popup tray
        /// </summary>
        /// <param name="fm">The firewall module returning this PMR</param>
        public PacketMainReturn(FirewallModule fm)
        {
            actualModule = fm;
            Module = fm.MetaData.Name;
            logMessage = "An error has occurred in " + Module + " with no other details.";
        }
        public PacketMainReturn(string moduleName, Exception e)
        {
            Module = moduleName;
            returnType = PacketMainReturnType.Error | PacketMainReturnType.Log;
            logMessage = "An error has occurred in " + moduleName + ". " + e.Message + "\r\n" + e.StackTrace;
        }
        public string Module = null;
        public FirewallModule actualModule = null;
        public Packet SendPacket = null;
        public string logMessage = null;
        public PacketMainReturnType returnType;
    }

    public enum ModuleErrorType
    {
        Success,        //No error
        UnknownError    //I'm not sure what type of errors it'll run into yet
    }

    public class ModuleError
    {
        public byte[] errorBinary = null;
        public string errorMessage = null;
        public ModuleErrorType errorType;
        public string moduleName = null;
    }

    /// <summary>
    /// An abstract class for the firewall modules, making input and output uniform
    /// </summary>
    public abstract class FirewallModule
    {
        [DllImport("msvcrt.dll")]
        public static extern int memcmp(byte[] b1, byte[] b2, int count);

        public FirewallModule() { }

        public FirewallModule(INetworkAdapter adapter)
        {
            this.adapter = adapter;
        }

        public ModuleMeta MetaData = new ModuleMeta();
        public INetworkAdapter adapter = null;
        public System.Windows.Forms.UserControl uiControl = null;
        public object PersistentData = null;
        public bool Enabled = true;

        public void SaveConfig()
        {
            if (PersistentData == null)
                return;
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + Path.DirectorySeparatorChar + "firebwall";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                folder = folder + Path.DirectorySeparatorChar + "modules";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                folder = folder + Path.DirectorySeparatorChar + "configs";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string file = folder + Path.DirectorySeparatorChar + adapter.InterfaceInformation.Name + MetaData.Name + ".cfg";                
                FileStream stream = File.Open(file, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                bFormatter.Serialize(stream, PersistentData);
                stream.Close();
            }
            catch (Exception ex) 
            {
                System.Diagnostics.Debug.WriteLine("SaveConfig Exception: " + ex.Message);
            }
        }

        internal sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                try
                {
                    string ToAssemblyName = assemblyName.Split(',')[0];
                    Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly ass in Assemblies)
                    {
                        if (ass.FullName.Split(',')[0] == ToAssemblyName)
                        {
                            typeToDeserialize = ass.GetType(typeName);
                            break;
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
                return typeToDeserialize;
            }
        }

        public void LoadConfig()
        {
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder = folder + Path.DirectorySeparatorChar + "firebwall";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                folder = folder + Path.DirectorySeparatorChar + "modules";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                folder = folder + Path.DirectorySeparatorChar + "configs";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string file = folder + Path.DirectorySeparatorChar + adapter.InterfaceInformation.Name + MetaData.Name + ".cfg";
                if (File.Exists(file))
                {
                    FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    BinaryFormatter bFormatter = new BinaryFormatter();
                    bFormatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    bFormatter.Binder = new VersionConfigToNamespaceAssemblyObjectBinder();
                    PersistentData = (Object)bFormatter.Deserialize(stream);
                    stream.Close();
                }
            }
            catch (Exception)
            {
                PersistentData = null;
            }
        }

        public virtual System.Windows.Forms.UserControl GetControl()
        {
            return null;
        }

        /// <summary>
        /// Ran after the module is loaded, to prime it for processing if required
        /// </summary>
        /// <returns>Any error that occured during the starting of it</returns>
        public abstract ModuleError ModuleStart();

        /// <summary>
        /// Ran when the module is to be stopped, to clear up any uneeded resources
        /// </summary>
        /// <returns>Any error that occured during the stopping of it</returns>
        public abstract ModuleError ModuleStop();

        /// <summary>
        /// The wrapper function for processing packets
        /// </summary>
        /// <param name="in_packet">Packet to be processed</param>
        /// <returns>A PacketMainReturn object, either from the interiorMain or default error one</returns>
        public PacketMainReturn PacketMain(ref Packet in_packet)
        {
            if (Enabled)
            {
                try
                {
                    PacketMainReturn pmr = interiorMain(ref in_packet);
                    return pmr;
                }
                catch (Exception e)
                {
                    return new PacketMainReturn(MetaData.Name, e);
                }
            }
            else
                return new PacketMainReturn(MetaData.Name) { returnType = PacketMainReturnType.Allow };
        }

        /// <summary>
        /// The internal function for processing packets implemented by the module
        /// </summary>
        /// <param name="in_packet">Packet to be processed</param>
        /// <returns>PacketMainReturn object describing what to do with the packet and/or
        /// anything that is notable during the processing</returns>
        public abstract PacketMainReturn interiorMain(ref Packet in_packet);
    }

    public class Quad : IEquatable<Quad>
    {
        public IPAddress dstIP = null;
        public int dstPort = -1;
        public IPAddress srcIP = null;
        public int srcPort = -1;

        public override int GetHashCode()
        {
            return srcIP.GetHashCode() ^ dstIP.GetHashCode() ^ dstPort ^ srcPort;
        }

        public class EqualityComparer : IEqualityComparer<Quad>
        {

            public bool Equals(Quad x, Quad y)
            {
                return (x.srcIP == y.srcIP && x.srcPort == y.srcPort &&
                        x.dstIP == y.dstIP && x.dstPort == y.dstPort) ||
                        (x.srcIP == y.dstIP && x.srcPort == y.dstPort &&
                        x.dstIP == y.srcIP && x.dstPort == y.srcPort);
            }

            public int GetHashCode(Quad obj)
            {
                return obj.srcIP.GetHashCode() ^ obj.dstIP.GetHashCode() ^ obj.dstPort ^ obj.srcPort;
            }
        }

        public bool Equals(Quad other)
        {
            return (srcIP.Equals(other.srcIP) && srcPort == other.srcPort && dstIP.Equals(other.dstIP) && dstPort == other.dstPort);
        }
    }
}
