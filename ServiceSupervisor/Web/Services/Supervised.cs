using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using XAS.Model;
using XAS.Model.Paging;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;


namespace ServiceSupervisor.Web.Services {

    /// <summary>
    /// A repository service.
    /// </summary>
    /// 
    public class Supervised: ISupervised {

        private readonly ILogger log = null;
        private readonly Model.Manager manager = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly Web.Services.Supervised service = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Supervised(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;

            var key = config.Key;
            var section = config.Section;

            var repository = new Model.Repositories(config, handler, logFactory);

            this.manager = new Model.Manager(repository);
            this.service = new Web.Services.Supervised(config, handler, logFactory);
            this.log = logFactory.Create(typeof(Supervised));

            log.Trace("Initialized DinoService()");

        }

    }

}
