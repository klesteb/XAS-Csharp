using System;

namespace XAS.Network.DNS {

    /// <summary>
    /// Class to hold the DNS response for SRV records.
    /// </summary>
    /// 
    public class SrvDTO {

        public String Host { get; set; }
        public Int32 Priority { get; set; }
        public Int32 Weight { get; set; }
        public Int32 Port { get; set; }

    }

}
