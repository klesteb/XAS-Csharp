using System;

namespace XAS.Core.Locking {
    
    /// <summary>
    /// Interface for lockers.
    /// </summary>
    /// 
    public interface ILocker {

        /// <summary>
        /// Gets/Sets the lock name.
        /// </summary>
        /// 
        String Lockname { get; set; }

        /// <summary>
        /// Method to try a lock.
        /// </summary>
        /// <returns>True is successful.</returns>
        /// 
        Boolean TryLock();

        /// <summary>
        /// Method to aquire a lock.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// 
        Boolean Lock();

        /// <summary>
        /// Method to release a lock.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// 
        Boolean Unlock();

    }

}
