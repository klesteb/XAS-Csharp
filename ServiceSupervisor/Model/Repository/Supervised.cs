
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Model.Repository;

using ServiceSupervisor.Model.Schema;

namespace ServiceSupervisor.Model.Repository {

    public class Supervised: Memory<Supervise> {

        public Supervised(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory): base(config, handler, logFactory) { }

    }

}
