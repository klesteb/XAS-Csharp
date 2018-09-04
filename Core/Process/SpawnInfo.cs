using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace XAS.Core.Process {

    /// <summary>
    /// A class for providing data to the Spawn method.
    /// </summary>
    /// 
    public class SpawnInfo {

        public String Name { get;set; }
        public String Domain { get; set; }
        public String Command { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public Boolean AutoStart { get; set; }
        public Int32 ExitRetries { get; set; }
        public Boolean AutoRestart { get; set; }
        public List<Int32> ExitCodes { get; set; }
        public String WorkingDirectory { get; set; }
        public EventHandler ExitHandler { get; set; }
        public Dictionary<String, String> Environment { get; set; }
        public DataReceivedEventHandler StdoutHandler { get; set; }
        public DataReceivedEventHandler StderrHandler { get; set; }

        public SpawnInfo() {

            ExitRetries = 5;
            AutoStart = false;
            AutoRestart = false;
            WorkingDirectory = "C:\\";
            ExitCodes = new List<Int32> { 0, 1 };
            Environment = new Dictionary<String, String>();

        }

    }

}
