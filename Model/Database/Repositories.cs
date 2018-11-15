using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.Model.Configuration.Extension;

namespace XAS.Model.Database {

    /// <summary>
    /// A class to manage repositories.
    /// </summary>
    /// 
    public class Repositories: IRepositories {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        private DbContext context { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">A DbContext object.</param>
        /// 
        public Repositories(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, Object context) {

            this.config = config;
            this.handler = handler;
            this.context = context as DbContext;

            log = logFactory.Create(typeof(Repositories));

        }

        /// <summary>
        /// Save the current database context.
        /// </summary>
        /// 
        public void Save() {

            var key = config.Key;
            var section = config.Section;

            try {

                context.SaveChanges();

            } catch (DbEntityValidationException e) {

                foreach (var eve in e.EntityValidationErrors) {

                    log.ErrorMsg(config.GetValue(section.Messages(), key.RepositoriesValidationErrors()),
                        eve.Entry.Entity.GetType().Name, eve.Entry.State
                    );

                    foreach (var ve in eve.ValidationErrors) {

                        log.ErrorMsg(config.GetValue(section.Messages(), key.RepositoriesValidationProperties()),
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage
                        );

                    }

                }

                throw;

            } catch (DbUpdateException e) {

                var hresult = (e.InnerException.InnerException != null) 
                    ? e.InnerException.InnerException.HResult 
                    : e.InnerException.HResult;

                var message = (e.InnerException.InnerException != null)
                    ? e.InnerException.InnerException.Message
                    : e.InnerException.Message;

                log.ErrorMsg(config.GetValue(section.Messages(), key.RepositoriesUpdateErrors()), 
                    hresult, 
                    message.Replace(System.Environment.NewLine, ", ")
                );

                throw;

            } catch (Exception ex) {

                handler.Exceptions(ex);

            }

        }

        /// <summary>
        /// Encapsulate a method within a transaction.
        /// </summary>
        /// <param name="method">The method to perform.</param>
        /// 
        public void DoTransaction(Action method) {

            using (var transaction = context.Database.BeginTransaction()) {

                try {

                    method();

                    this.Save();
                    transaction.Commit();

                } catch {

                    transaction.Rollback();
                    throw;

                }

            }
        
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // TODO: dispose managed state (managed objects).

                    context.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Repository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Generic Dispose.
        /// </summary>
        /// 
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}
