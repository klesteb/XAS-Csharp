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

}
