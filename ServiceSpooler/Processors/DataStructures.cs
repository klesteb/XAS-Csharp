using System;
using System.IO;

using XAS.Core.Spooling;

namespace ServiceSpooler.Processors {

    /// <summary>
    /// Keep track of packets.
    /// </summary>
    /// 
    public struct Packet {
        public string queue;
        public string json;
        public string receipt;
    }

    /// <summary>
    /// Keep track of watchers.
    /// </summary>
    /// 
    public struct Watcher {
        public string directory;
        public string type;
        public string queue;
        public string alias;
        public Spooler spool;
        public FileSystemWatcher watch;
    }

    /// <summary>
    /// The handler to enqueue packets.
    /// </summary>
    /// <param name="filename">The file to process.</param>
    /// 
    public delegate void EnqueueHandler(String filename);

    /// <summary>
    /// The handler to dequeue packets.
    /// </summary>
    /// 
    public delegate void DequeueHandler(Packet packet);

}
