using System;

using XAS.Model.Memory;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Schema;

namespace ServiceSupervisor.Model.Repository {

    public class Supervised: Repository<Supervise> {

        public Supervised(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, Object context) :
            base(config, handler, logFactory, context) {
        }

    }

}
