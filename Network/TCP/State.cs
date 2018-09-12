using System;
using System.IO;
using System.Net.Sockets;

namespace XAS.Network.TCP {

    /// <summary>
    /// A class to maintain client state for the server.
    /// </summary>
    /// 
    public class State {

        /// <summary>
        /// Get/Set the socket id.
        /// </summary>
        /// 
        public Int32 Id { get; set; }

        /// <summary>
        /// Get/Set wither to close the socect after write.
        /// </summary>
        /// 
        public Boolean Close { get; set; }

        /// <summary>
        /// Gets/Sets the IO stream.
        /// </summary>
        /// 
        public Stream Stream { get; set; }

        /// <summary>
        /// Get/Size the clients socket.
        /// </summary>
        /// 
        public Socket Socket { get; set; }

        /// <summary>
        /// Gets/Sets the availble bytes in the buffer.
        /// </summary>
        /// 
        public Int32 Count { get; set; }

        /// <summary>
        /// Get/Set the buffer.
        /// </summary>
        /// 
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Gets the buffer size.
        /// </summary>
        /// 
        public Int32 Size { get; private set; }

        /// <summary>
        /// Get/Set wither the client is connected.
        /// </summary>
        /// 
        public Boolean Connected { get; set; }

        /// <summary>
        /// Get/Set the remote host.
        /// </summary>
        /// 
        public String RemoteHost { get; set; }

        /// <summary>
        /// Get/Set the remote port.
        /// </summary>
        /// 
        public Int32 RemotePort { get;set; }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="size">Optional buffer size paramter, defaults to 1024.</param>
        /// 
        public State(Int32 size = 1024) {

            this.Id = 0;
            this.Size = size;
            this.Stream = null;
            this.Close = false;
            this.Socket = null;
            this.Buffer = new byte[size];

        }

    }

}
