using System;
using System.Net;
using System.Runtime.InteropServices;

namespace XAS.Network.DNS {

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]

    public struct IP4_ARRAY {

        /// DWORD->unsigned int
        public uint AddrCount;

        /// IP4_ADDRESS[1]
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = System.Runtime.InteropServices.UnmanagedType.U4)]
        public uint[] AddrArray;
    }


    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct byte_addr {

        /// u_char->unsigned char
        public byte s_b1;

        /// u_char->unsigned char
        public byte s_b2;

        /// u_char->unsigned char
        public byte s_b3;

        /// u_char->unsigned char
        public byte s_b4;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct word_addr {

        /// u_short->unsigned short
        public ushort s_w1;

        /// u_short->unsigned short
        public ushort s_w2;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct addr_union {

        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public byte_addr S_un_b;

        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public word_addr S_un_w;

        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public uint S_addr;
    }

    /*****************
    * Solution from: http://stackoverflow.com/questions/1323845/using-in-addr-in-c-sharp
    *
    */

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct in_addr {

        public addr_union S_un;


        public in_addr(String name) {
            // Set this first, otherwise it wipes out the other fields
            S_un.S_un_w = new word_addr();

            IPAddress[] addresses = Dns.GetHostAddresses(name);
            byte[] address = addresses[0].GetAddressBytes();

            S_un.S_addr = (uint)BitConverter.ToInt32(address, 0);

            S_un.S_un_b.s_b1 = address[0];
            S_un.S_un_b.s_b2 = address[1];
            S_un.S_un_b.s_b3 = address[2];
            S_un.S_un_b.s_b4 = address[3];
        }

        public in_addr(IPAddress address) : this(address.GetAddressBytes()) { }

        public in_addr(byte[] address) {
            // Set this first, otherwise it wipes out the other fields
            S_un.S_un_w = new word_addr();

            S_un.S_addr = (uint)BitConverter.ToInt32(address, 0);

            S_un.S_un_b.s_b1 = address[0];
            S_un.S_un_b.s_b2 = address[1];
            S_un.S_un_b.s_b3 = address[2];
            S_un.S_un_b.s_b4 = address[3];
        }

        /// <summary>
        /// Unpacks an in_addr struct to an IPAddress object
        /// </summary>
        /// <returns></returns>
        public IPAddress ToIPAddress() {
            byte[] bytes = new[] {
            S_un.S_un_b.s_b1,
            S_un.S_un_b.s_b2,
            S_un.S_un_b.s_b3,
            S_un.S_un_b.s_b4
        };

            return new IPAddress(bytes);
        }
    }
}
