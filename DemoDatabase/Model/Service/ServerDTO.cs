using System;
using System.Collections.Generic;

using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Service {

    public class ServerDTO {

        public String Name { get; set; }
        public Int32 Id { get; set; }
        public List<AttributeDTO> Attributes { get; set; }

    }

}
