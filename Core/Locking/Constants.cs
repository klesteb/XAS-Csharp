using System;

namespace XAS.Core.Locking {

    /// <summary>
    /// Indicates which lock driver to use.
    /// </summary>
    /// 
    public enum LockDriver {

        /// <summary>
        /// Default locking.
        /// </summary>
        /// 
        Default,

        /// <summary>
        /// Use a global system mutex.
        /// </summary>
        /// 
        Mutex

    };

}
