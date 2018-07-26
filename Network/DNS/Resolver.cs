using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace XAS.Network.DNS {

    // taken from: http://randronov.blogspot.com/2013/03/lookup-srv-record-in-c.html
    // with  modifications

    /// <summary>
    /// A DNS resolver.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// 
    ///    class Program {
    ///     
    ///        static void Main(string[] args) {
    ///    
    ///            string[] s = null;
    ///            string host = args[0];

    ///            // Let's use OpenDNS Server or comment out string below to use system settings

    ///            Resolver.SetDnsServer("208.67.222.222");
    ///
    ///            Console.WriteLine("Get MX for " + host);
    ///            
    ///            s = Resolver.GetMXRecords(host);
    ///            
    ///            foreach (string st in s) {
    ///            
    ///                Console.WriteLine("Server: {0}", st);
    ///                
    ///            }
    ///            
    ///            Console.WriteLine("\nGet SPF for " + host);
    ///            
    ///            s = Resolver.GetSPFRecords(host);
    ///            
    ///            foreach (string st in s) {
    ///            
    ///                Console.WriteLine("SPF: {0}", st);
    ///                
    ///            }
    ///
    ///            Console.WriteLine("\nGet SRV for " + host);
    ///            
    ///            s = Resolver.GetSRVRecords(host);
    ///            
    ///            foreach (string st in s) {
    ///            
    ///                Console.WriteLine("Server: {0}", st);
    ///                
    ///            }
    ///
    ///            Console.ReadLine();
    ///
    ///        }
    ///        
    ///    }
    ///    
    /// </remarks>
    /// 
    public class Resolver {

        [DllImport("dnsapi", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern int DnsQuery([MarshalAs(UnmanagedType.VBByRefStr)]ref string pszName, QueryTypes wType, QueryOptions options, ref IntPtr aipServers, ref IntPtr ppQueryResults, int pReserved);

        [DllImport("dnsapi", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void DnsRecordListFree(IntPtr pRecordList, int FreeType);

        private static IntPtr pDnsServer = IntPtr.Zero;

        public Resolver() { }

        public static void SetDnsServer(String host) {

            if (String.IsNullOrEmpty(host)) {

                IPAddress address = GetLocalDnsAdress();
                host = address.ToString();

            }

            in_addr addr = new in_addr(host);
            IP4_ARRAY arr = new IP4_ARRAY();

            arr.AddrCount = 1;
            arr.AddrArray = new uint[1];
            arr.AddrArray[0] = addr.S_un.S_addr;

            IntPtr pDnsServer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IP4_ARRAY)));
            Marshal.StructureToPtr(arr, pDnsServer, false);

        }

        public static string GetDnsServer() {

            return Marshal.PtrToStringAuto(pDnsServer);

        }

        public static string[] GetMXRecords(string domain) {

            MXRecord recMx;
            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            ArrayList list1 = new ArrayList();

            if (Environment.OSVersion.Platform != PlatformID.Win32NT) {

                throw new NotSupportedException();

            }

            try {

                int num1 = Resolver.DnsQuery(ref domain, QueryTypes.DNS_TYPE_MX, QueryOptions.DNS_QUERY_BYPASS_CACHE, ref pDnsServer, ref ptr1, 0);

                if (num1 != 0) {

                    if (num1 == 9003 || num1 == 9501) {

                        list1.Add("DNS record does not exist");

                    } else {

                        throw new Win32Exception(num1);

                    }

                }

                for (ptr2 = ptr1; !ptr2.Equals(IntPtr.Zero); ptr2 = recMx.pNext) {

                    recMx = (MXRecord)Marshal.PtrToStructure(ptr2, typeof(MXRecord));

                    if (recMx.wType == (ushort)QueryTypes.DNS_TYPE_MX) {

                        string text1 = Marshal.PtrToStringAuto(recMx.pNameExchange);
                        list1.Add(text1);

                    }

                }

            } finally {

                Resolver.DnsRecordListFree(ptr1, 0);

            }

            return (string[])list1.ToArray(typeof(string));

        }

        /// <summary>
        /// Get SRV records based on the resource query.
        /// </summary>
        /// <param name="needle">The resource query.</param>
        /// <returns>An odered list of SrvDTO object based on priority and weight.</returns>
        /// 
        public static List<SrvDTO> GetSRVRecords(string needle) {

            SRVRecord recSRV;
            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            var list = new List<SrvDTO>();

            if (Environment.OSVersion.Platform != PlatformID.Win32NT) {

                throw new NotSupportedException();

            }

            try {

                int num1 = Resolver.DnsQuery(ref needle, QueryTypes.DNS_TYPE_SRV, QueryOptions.DNS_QUERY_BYPASS_CACHE, ref pDnsServer, ref ptr1, 0);

                if (num1 != 0) {

                    if (num1 == 9003 || num1 == 9501) {

                        throw new Exception("DNS record does not exist");

                    } else {

                        throw new Win32Exception(num1);

                    }

                }

                for (ptr2 = ptr1; !ptr2.Equals(IntPtr.Zero); ptr2 = recSRV.pNext) {

                    recSRV = (SRVRecord)Marshal.PtrToStructure(ptr2, typeof(SRVRecord));

                    if (recSRV.wType == (ushort)QueryTypes.DNS_TYPE_SRV) {

                        var dto = new SrvDTO {
                            Host = Marshal.PtrToStringAuto(recSRV.pNameTarget),
                            Priority = recSRV.wPriority,
                            Weight = recSRV.wWeight,
                            Port = recSRV.wPort
                        };

                        list.Add(dto);

                    }

                }

            } finally {

                Resolver.DnsRecordListFree(ptr1, 0);

            }

            return list.OrderBy(s => s.Priority).ThenBy(s => s.Weight).ToList();

        }

        public static string[] GetSPFRecords(string domain) {

            SPFRecord recSPF;
            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            ArrayList list1 = new ArrayList();

            if (Environment.OSVersion.Platform != PlatformID.Win32NT) {

                throw new NotSupportedException();

            }

            try {

                int num1 = Resolver.DnsQuery(ref domain, QueryTypes.DNS_TYPE_TEXT, QueryOptions.DNS_QUERY_BYPASS_CACHE, ref pDnsServer, ref ptr1, 0);

                if (num1 != 0) {

                    if (num1 == 9003 || num1 == 9501) {

                        list1.Add("DNS record does not exist");

                    } else {

                        throw new Win32Exception(num1);

                    }

                }

                for (ptr2 = ptr1; !ptr2.Equals(IntPtr.Zero); ptr2 = recSPF.pNext) {

                    recSPF = (SPFRecord)Marshal.PtrToStructure(ptr2, typeof(SPFRecord));

                    if (recSPF.wType == (ushort)QueryTypes.DNS_TYPE_TEXT) {

                        for (int i = 0; i < recSPF.dwStringCount; i++) {

                            IntPtr pString = recSPF.pStringArray + i;
                            string text1 = Marshal.PtrToStringAuto(pString);

                            if (text1.Contains("v=spf1") || text1.Contains("spf2.0")) {

                                list1.Add(text1);

                            }

                        }

                    }

                }

            } finally {

                Resolver.DnsRecordListFree(ptr1, 0);

            }

            return (string[])list1.ToArray(typeof(string));

        }

        public static Boolean IsAvailable(String server, Int32 port, Int32 timeout) {

            bool stat;

            try {

                using (TcpClient tcp = new TcpClient()) {

                    IAsyncResult ar = tcp.BeginConnect(server, port, null, null);
                    WaitHandle wh = ar.AsyncWaitHandle;

                    try {

                        if (!wh.WaitOne(TimeSpan.FromMilliseconds(timeout), false)) {

                            tcp.EndConnect(ar);
                            tcp.Close();

                            throw new SocketException();

                        }

                        stat = true;
                        tcp.EndConnect(ar);

                    } finally {

                        wh.Close();

                    }

                }

            } catch (SocketException e) {

                stat = false;

            }

            return stat;

        }

        private static IPAddress GetLocalDnsAdress() {

            // taken from: http://www.robertsindall.co.uk/blog/blog/2011/05/28/how-to-get-local-dns-server-address/

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces) {

                if (networkInterface.OperationalStatus == OperationalStatus.Up) {

                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    foreach (IPAddress dnsAdress in dnsAddresses) {

                        return dnsAdress;

                    }

                }

            }

            throw new InvalidOperationException("Unable to find DNS Address");

        }

        private enum QueryOptions {
            DNS_QUERY_ACCEPT_TRUNCATED_RESPONSE = 1,
            DNS_QUERY_BYPASS_CACHE = 8,
            DNS_QUERY_DONT_RESET_TTL_VALUES = 0x100000,
            DNS_QUERY_NO_HOSTS_FILE = 0x40,
            DNS_QUERY_NO_LOCAL_NAME = 0x20,
            DNS_QUERY_NO_NETBT = 0x80,
            DNS_QUERY_NO_RECURSION = 4,
            DNS_QUERY_NO_WIRE_QUERY = 0x10,
            DNS_QUERY_RESERVED = -16777216,
            DNS_QUERY_RETURN_MESSAGE = 0x200,
            DNS_QUERY_STANDARD = 0,
            DNS_QUERY_TREAT_AS_FQDN = 0x1000,
            DNS_QUERY_USE_TCP_ONLY = 2,
            DNS_QUERY_WIRE_ONLY = 0x100
        }

        private enum QueryTypes {
            DNS_TYPE_A = 0x0001,
            DNS_TYPE_MX = 0x000f,
            DNS_TYPE_TEXT = 0x0010,
            DNS_TYPE_SRV = 0x0021
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MXRecord {
            public IntPtr pNext;
            public string pName;
            public ushort wType;
            public ushort wDataLength;
            public int flags;
            public int dwTtl;
            public int dwReserved;
            public IntPtr pNameExchange;
            public ushort wPreference;
            public ushort Pad;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SRVRecord {
            public IntPtr pNext;
            public string pName;
            public ushort wType;
            public ushort wDataLength;
            public int flags;
            public int dwTtl;
            public int dwReserved;
            public IntPtr pNameTarget;
            public ushort wPriority;
            public ushort wWeight;
            public ushort wPort;
            public ushort Pad;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SPFRecord {
            public IntPtr pNext;
            public string pName;
            public ushort wType;
            public ushort wDataLength;
            public int flags;
            public int dwTtl;
            public int dwReserved;
            public int dwStringCount;
            public IntPtr pStringArray;
        }

    }

}
