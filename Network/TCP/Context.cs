using System;
using System.IO;
using System.Net.Sockets;

namespace XAS.Network.TCP {

    /// <summary>
    /// A class to maintain context for a network connection
    /// </summary>
    /// 
    public class Context {

        /// <summary>
        /// Internal buffer.
        /// </summary>
        /// 
        public byte[] Buffer;

        /// <summary>
        /// Gets/Sets the availble bytes in the buffer.
        /// </summary>
        /// 
        public Int32 Count { get; set; }

        /// <summary>
        /// Gets/Sets the IO stream.
        /// </summary>
        /// 
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets/Sets the socket.
        /// </summary>
        /// 
        public Socket Socket { get; set; }

        /// <summary>
        /// Toggles the connection status.
        /// </summary>
        /// 
        public Boolean Connected { get; set; }

        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        /// 
        public Int32 Size { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">Optionally set the internal buffer size.</param>
        /// 
        public Context(Int32 size = 1024) {

            this.Count = 0;
            this.Size = size;
            this.Socket = null;
            this.Stream = null;
            this.Connected = false;
            this.Buffer = new byte[size];

        }

    }

}
