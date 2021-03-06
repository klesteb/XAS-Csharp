﻿using System;
using System.Collections.Generic;

namespace ServiceSupervisorCommon.DataStructures {

    public class SuperviseDTI {

        public String Verb { get; set; }
        public String Name { get; set; }
        public String Domain { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public Int32 ExitRetries { get; set; }
        public Boolean AutoRestart { get; set; }
        public List<Int32> ExitCodes { get; set; }
        public String WorkingDirectory { get; set; }
        public Dictionary<String, String> Environment { get; set; }

    }

}
