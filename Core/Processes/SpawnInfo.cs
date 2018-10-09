using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace XAS.Core.Processes {

    /// <summary>
    /// A configuration class for Spawn.
    /// </summary>
    /// 
    public class SpawnInfo {

        /// <summary>
        /// Get/Set a name to associate with this process.
        /// </summary>
        /// 
        public String Name { get;set; }

        /// <summary>
        /// Get/Set the Windows Domain (optional).
        /// </summary>
        /// 
        public String Domain { get; set; }

        /// <summary>
        /// Get/Set the command line to execute.
        /// </summary>
        /// 
        public String Command { get; set; }

        /// <summary>
        /// Get/Set a username for authenticatiion (optional).
        /// </summary>
        /// 
        public String Username { get; set; }

        /// <summary>
        /// Get/Set a password for authentication (optional).
        /// </summary>
        /// 
        public String Password { get; set; }

        /// <summary>
        /// Get/Set the verb to run under.
        /// </summary>
        /// 
        public String Verb { get;set; }

        /// <summary>
        /// Get/Set wither to auto start the process, default is false.
        /// </summary>
        /// 
        public Boolean AutoStart { get; set; }

        /// <summary>
        /// Get/Set the number of auto restart retries, default is 5.
        /// </summary>
        /// 
        public Int32 ExitRetries { get; set; }

        /// <summary>
        /// Get/Set wither to auto restart the process, default is false.
        /// </summary>
        public Boolean AutoRestart { get; set; }

        /// <summary>
        /// Get/Set the delay between restarts, default is 0.
        /// </summary>
        public Int32 RestartDelay { get; set; }

        /// <summary>
        /// Get/Set the known exit codes, defaults are 0 and 1.
        /// </summary>
        /// 
        public List<Int32> ExitCodes { get; set; }

        /// <summary>
        /// Get/Set the working directory for the command, default is C:\.
        /// </summary>
        /// 
        public String WorkingDirectory { get; set; }

        /// <summary>
        /// Get/Set optional environment variables for the process.
        /// </summary>
        /// 
        public Dictionary<String, String> Environment { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public SpawnInfo() {

            Verb = "";
            ExitRetries = 5;
            RestartDelay = 0;
            AutoStart = false;
            AutoRestart = false;
            WorkingDirectory = "C:\\";
            ExitCodes = new List<Int32> { 0, 1 };
            Environment = new Dictionary<String, String>();

        }

    }

}
