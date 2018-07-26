using System;
using System.Collections.Generic;

namespace DemoDatabase.Model.Service {

    public class GroupDTO {

        public String Name { get; set; }
        public Int32 Id { get; set; }
        public List<TargetDTO> Targets { get; set; }

    }

}
