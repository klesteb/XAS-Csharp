
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Repository;

namespace ServiceSupervisor.Model {

    /// <summary>
    /// Define the interface to the repositories.
    /// </summary>
    ///
    public class Repositories {

        public Supervised Supervised { get; private set; }

        public Repositories(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.Supervised = new Supervised(config, handler, logFactory);

        }

    }

}
