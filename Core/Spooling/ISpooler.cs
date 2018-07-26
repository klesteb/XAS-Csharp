using System;

namespace XAS.Core.Spooling {

    public interface ISpooler {

        String Extension { get; set; }
        String Seqfile { get; set; }
        String Directory { get; set; }
        Int32 Retries { get; set; }
        Int32 Timeout { get; set;  }
        Byte[] Read(string filename);
        void Write(Byte[] packet);
        String[] Scan();
        Boolean Delete(string filename);
        Int32 Count();
        String Get();

    }

}
