﻿
using Nancy;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Hal.Configuration;
using Nancy.Authentication.Basic;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Rest.Server.Extensions;
using XAS.Rest.Server.Repository;

using DemoMicroServiceServer.Web.Services;

namespace DemoMicroServiceServer.Web {

    /// <summary>
    /// Configure the NancyHost.
    /// </summary>
    /// 
    public class BootStrapper: XAS.Rest.Server.BootStrapper {

        private readonly ILogger log = null;

        public BootStrapper(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IUserValidator userValidator, IRootPathProvider rootPathProvider): 
            base(config, handler, logFactory, userValidator, rootPathProvider) {

            this.log = logFactory.Create(typeof(BootStrapper));

        }

        /// <summary>
        /// Configure application startup.
        /// </summary>
        /// <param name="container">A TinyIoc container.</param>
        /// <param name="pipelines">The Nancy pipelines.</param>
        /// 
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {

            log.Trace("Entering ApplicationStartup()");

            // call the base

            base.ApplicationStartup(container, pipelines);

            // enable CORS processing

            pipelines.EnableCORS();

            log.Trace("Leaving ApplicationStartup()");

        }

        /// <summary>
        /// Configure the application conatiner.
        /// </summary>
        /// <param name="container">A TinyIocContainer object.</param>
        /// 
        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {

            log.Trace("Entering ConfigureApplicatonContainer()");

            base.ConfigureApplicationContainer(container);

            container.Register<IDinoService, DinoService>().AsMultiInstance();
            container.Register(typeof(IResourceConfiguration), Configure.ResourceConfiguration());
            container.Register(typeof(IProvideHalTypeConfiguration), Configure.HypermediaConfiguration());

            log.Trace("Leaving ConfigureApplicatonContainer()");

        }

    }

}
