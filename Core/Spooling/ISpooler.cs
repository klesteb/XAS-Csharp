using System;
using System.Collections.Generic;

namespace XAS.Core.Spooling {

    public interface ISpooler {

        Int32 Retries { get; set; }
        Int32 Timeout { get; set; }
        String Seqfile { get; set; }
        String Extension { get; set; }
        String Directory { get; set; }

        String Get();
        Int32 Count();
        List<String> Scan();
        void Write(Byte[] packet);
        Byte[] Read(String filename);
        Boolean Delete(String filename);

    }

}
