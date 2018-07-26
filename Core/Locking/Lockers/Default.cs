using System;


namespace XAS.Core.Locking.Lockers {

    /// <summary>
    /// A class that implements a locker.
    /// </summary>
    /// 
    public class Default: Locker {

        private string lockName;

        /// <summary>
        /// Get/Set the name of the lock.
        /// </summary>
        /// 
        public override String Lockname {
            get { return lockName; }
            set { lockName = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Default(String lockName) {

            this.Lockname = lockName;

        }

        /// <summary>
        /// Check to see if the lock is available.
        /// </summary>
        /// <returns>True is available.</returns>
        /// 
        public override Boolean TryLock() {

            return true;

        }

        /// <summary>
        /// Aquire a lock.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// 
        public override Boolean Lock() {

            return true;

        }

        /// <summary>
        /// Release a lock.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// 
        public override Boolean Unlock() {

            return true;

        }

    }

}
