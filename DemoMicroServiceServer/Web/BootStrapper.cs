using System;

using Nancy;
using Nancy.TinyIoc;
using Nancy.Hal.Configuration;
using Nancy.Authentication.Basic;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Rest.Server.Repository;

using DemoMicroServiceServer.Web.Services;

namespace DemoMicroServiceServer.Web {

    /// <summary>
    /// Configure the NancyHost.
    /// </summary>
    /// 
    public class BootStrapper: XAS.Rest.Server.BootStrapper {

        private readonly ILogger log = null;

        public BootStrapper(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IUserValidator userValidator, IRootPathProvider rootPathProvider, IManager manager): 
            base(config, handler, logFactory, userValidator, rootPathProvider, manager) {

            this.log = logFactory.Create(typeof(BootStrapper));

        }
        
        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {

            log.Trace("Entering ConfigureApplicatonContainer()");

            base.ConfigureApplicationContainer(container);

            container.Register<IDinoService, DinoService>();
            container.Register(typeof(IResourceConfiguration), Configure.ResourceConfiguration());
            container.Register(typeof(IProvideHalTypeConfiguration), Configure.HypermediaConfiguration());

            log.Trace("Leaving ConfigureApplicatonContainer()");

        }

    }

}
