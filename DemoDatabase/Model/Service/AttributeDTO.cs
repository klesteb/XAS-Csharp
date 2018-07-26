using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DemoDatabase.Model.Service {

    public class AttributeDTO {

        public Int32 Id { get; set; }
        public String Type { get; set; }
        public String Name { get; set; }
        public String StrValue { get; set; }
        public Int32 NumValue { get; set; }

    }

}
