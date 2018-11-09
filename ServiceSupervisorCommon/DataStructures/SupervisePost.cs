using System;

namespace ServiceSupervisorCommon.DataStructures {

    public class SupervisePost {

        public String Verb { get; set; }
        public String Name { get; set; }
        public String Domain { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String AutoStart { get; set; }
        public String ExitCodes { get; set; }
        public String ExitRetries { get; set; }
        public String AutoRestart { get; set; }
        public String Environment { get; set; }
        public String WorkingDirectory { get; set; }

    }

    public class SuperviseUpdate: SupervisePost { }

}
