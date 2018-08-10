using System;

namespace DemoModelCommon.DataStructures {

    /// <summary>
    /// For creating a record.
    /// </summary>
    /// 
    public class DinosaurPost {

        public String Name { get; set; }
        public String Status { get; set; }
        public String Height { get; set; }

    }

    /// <summary>
    /// For updating a record.
    /// </summary>
    /// 
    public class DinosaurUpdate: DinosaurPost { }

}
