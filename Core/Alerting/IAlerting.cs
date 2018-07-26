using System;

namespace XAS.Core.Alerting {
    
    /// <summary>
    /// Public interface for alerting.
    /// </summary>
    /// 
    public interface IAlerting {

        void Send(String message);

    }

}
