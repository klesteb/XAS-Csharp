using System;
using System.Security;

namespace XAS.Core.Security {
    
    /// <summary>
    /// Public interface for ISecurity.
    /// </summary>
    /// 
    public interface ISecurity {

        void RunElevated();
        Boolean IsElevated { get; }
        SecureString MakeSecureString(string inSecureString);

    }

}
