using System;
using System.IO;
using System.Net.Sockets;

namespace XAS.Network.TCP {

    /// <summary>
    /// Usefull Socket extensions.
    /// </summary>
    /// 
    public static class SocketExtensions {

        private static readonly byte[] POLLING_BYTE_ARRAY = new byte[0];

        /// <summary>
        /// A structure to manipulate the Socket keepalive feature.
        /// </summary>
        /// 
        [System.Runtime.InteropServices.StructLayout(
            System.Runtime.InteropServices.LayoutKind.Explicit
        )]
        unsafe struct TcpKeepalive {

            [System.Runtime.InteropServices.FieldOffset(0)]
            [
              System.Runtime.InteropServices.MarshalAs(
                   System.Runtime.InteropServices.UnmanagedType.ByValArray,
                   SizeConst = 12
               )
            ]

            public fixed byte Bytes[12];

            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint OnOff;

            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint KeepaLiveTime;

            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint KeepaLiveInterval;

        }

        /// <summary>
        /// Set the sockets keepalive functionality
        /// </summary>
        /// <param name="socket">The socket to use.</param>
        /// <param name="timeout">The timeout in seconds.</param>
        /// <param name="interval">The interval to resend packets.</param>
        /// 
        public static void SetKeepaliveValues(this Socket socket, UInt32 timeout, UInt32 interval) {

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            unsafe {

                TcpKeepalive values = new TcpKeepalive();

                values.OnOff = 1;
                values.KeepaLiveTime = timeout;
                values.KeepaLiveInterval = interval;

                byte[] InValue = new byte[12];

                for (int I = 0; I < 12; I++) {

                    InValue[I] = values.Bytes[I];

                }

                socket.IOControl(IOControlCode.KeepAliveValues, InValue, null);

            }

        }

        /// <summary>
        /// Determine if a network connection is alive. 
        /// </summary>
        /// <remarks>
        /// This is done by writting a null byte to the socket. If the write errors out, then the socket is 
        /// dead.
        /// </remarks>
        /// <param name="socket">The socket to use.</param>
        /// <returns>
        /// This method returns a boolean value on wither the socket is dead or alive.
        /// </returns>
        /// 
        public static Boolean IsConnected(this Socket socket) {

            // taken from https://msdn.microsoft.com/en-us/library/system.net.sockets.socket.connected(v=vs.110).aspx
            // with modifications

            bool stat = false;
            bool blockingState = socket.Blocking;

            try {
            
                socket.Blocking = false;
                socket.Send(POLLING_BYTE_ARRAY, 0, 0);
                socket.Send(POLLING_BYTE_ARRAY, 0, 0);
                stat = true;
                                                
            } catch {

                stat = false;
                throw;

            } finally {

                socket.Blocking = blockingState;

            }

            return stat;
            
        }

    }

    /// <summary>
    /// Usefull Stream extensions.
    /// </summary>
    /// 
    public static class StreamExtensions {

        // taken from http://stackoverflow.com/questions/9707314/is-it-possible-to-detect-if-a-stream-has-been-closed-by-the-client

        private static readonly byte[] POLLING_BYTE_ARRAY = new byte[0];

        /// <summary>
        /// An extension to the Stream class to determine if a network connection is alive. This is
        /// done by writting a null byte to the stream. If the write errors out, then the stream is 
        /// dead. 
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        /// <returns>
        /// This method returns a boolean value on wither the stream is dead or alive.
        /// </returns>
        /// 
        public static Boolean IsConnected(this Stream stream) {

            try {

                // Twice because the first time will return without issue but
                // cause the Stream to become closed (if the Stream is actually
                // closed.)

                stream.Write(POLLING_BYTE_ARRAY, 0, POLLING_BYTE_ARRAY.Length);
                stream.Write(POLLING_BYTE_ARRAY, 0, POLLING_BYTE_ARRAY.Length);

                return true;

            } catch (ObjectDisposedException) {

                // The stream has been internally closed.

                return false;

            } catch (IOException) {

                // This will be thrown on the second stream.Write when the Stream
                // is closed on the client side.

                return false;

            }

        }

    }

}
