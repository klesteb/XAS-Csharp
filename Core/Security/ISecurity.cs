using System;
using System.Security;
using System.Collections.Generic;

namespace XAS.Core.Security {
    
    /// <summary>
    /// Public interface for ISecurity.
    /// </summary>
    /// 
    public interface ISecurity {

        Boolean IsElevated { get; }

        void RunElevated();
        SecureString MakeSecureString(string inSecureString);
        Int32 RunAs(String command, String args, out List<string> stdout, out List<string> stdin);

    }

}
