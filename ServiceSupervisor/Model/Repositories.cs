using System;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Repository;

namespace ServiceSupervisor.Model {

    /// <summary>
    /// Define the interface to the repositories.
    /// </summary>
    ///
    public class Repositories: IDisposable {

        public Supervised Supervised { get; private set; }

        public Repositories(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.Supervised = new Supervised(config, handler, logFactory);

        }

        public void Save() {
        
        }

        public void DoTransaction(Action method) {
        
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Repositories() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
