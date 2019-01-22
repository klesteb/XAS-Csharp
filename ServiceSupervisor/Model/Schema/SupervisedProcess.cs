using System;

using XAS.Core.Processes;

namespace ServiceSupervisor.Model.Schema {

    /// <summary>
    /// Configuration and status of a supervised process.
    /// </summary>
    /// 
    public class SupervisedProcess {

        public Int32 Pid { get; set; }
        public String Name { get; set; }
        public Spawn Spawn { get; set; }
        public Int32 RetryCount { get; set; }
        public RunStatus Status { get; set; }
        public SpawnInfo Config { get; set; }

        public SupervisedProcess() {

            RetryCount = 0;
            Config = new SpawnInfo();
            Status = RunStatus.Stopped;

        }

    }

}
