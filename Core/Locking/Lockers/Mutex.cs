using System;
using System.IO;
using System.Threading;

using XAS.Core.Exceptions;

namespace XAS.Core.Locking.Lockers {

    /// <summary>
    /// A class that implements a mutex based locker.
    /// </summary>
    /// 
    public class Mutex: Locker, IDisposable {

        private String lockName;
        private System.Threading.Mutex mutex = null;

        /// <summary>
        /// Get/Set the lock name.
        /// </summary>
        /// 
        public override String Lockname { 
            get { return lockName; }
            set {
                this.lockName = String.Format("Global\\{0}", value);
                this.lockName = this.lockName.Replace(Path.DirectorySeparatorChar, '_');
                this.mutex = new System.Threading.Mutex(false, lockName);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Mutex(String lockName) {

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

            bool stat = false;

            try {

                stat = this.mutex.WaitOne();

            } catch (AbandonedMutexException) {

                stat = true;

            }

            return stat;

        }

        /// <summary>
        /// Release a lock.
        /// </summary>
        /// <returns>True if successful.</returns>
        /// 
        public override Boolean Unlock() {

            this.mutex.ReleaseMutex();
            return true;

        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic dispose.
        /// </summary>
        /// <param name="disposing"></param>

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // TODO: dispose managed state (managed objects).

                    this.mutex = null;

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Shares() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Generic dispose.
        /// </summary>

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
