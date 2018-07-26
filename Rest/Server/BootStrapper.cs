
using System;
using System.IO;
using System.Reflection;

using Nancy;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Authentication.Basic;
using Nancy.Responses.Negotiation;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Rest.Server.Errors;

namespace XAS.Rest.Server {

    /// <summary>
    /// The default bootstrapper for REST applications.
    /// </summary>
    /// <remarks>
    /// The bootstrapper is invoked by the ioc. So any parameters 
    /// must be initalized in the contructor.
    /// </remarks>
    /// <example>
    /// 
    /// public class BootStrapper: XAS.Rest.Server.BootStrapper {
    ///     
    ///     public BootStrapper(): base() {
    ///     
    ///         this.Domain = "WSIPC";
    ///         this.RootPath = @"U:\My Documents\Visual Studio 2015\Projects\Rest\trunk\Demo";
    ///
    ///     }
    ///
    ///     protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
    ///
    ///         base.ConfigureApplicationContainer(container);
    ///
    ///         container.Register<IDinosaurService, DinosaurService>();
    ///         container.Register(typeof(IResourceConfiguration), Configure.ResourceConfiguration());
    ///         container.Register(typeof(IProvideHalTypeConfiguration), Configure.HypermediaConfiguration());
    ///
    ///     }
    ///
    /// }
    ///
    /// </example>
    /// 
    public class BootStrapper: DefaultNancyBootstrapper {

        private readonly ILogger log = null;
        private readonly IManager manager = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;
        private readonly ILoggerFactory logFactory = null;
        private readonly IUserValidator userValidator = null;
        private readonly IRootPathProvider rootPathProvider = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public BootStrapper(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IUserValidator userValidator, IRootPathProvider rootPathProvider, IManager manager): base() {

            this.config = config;
            this.manager = manager;
            this.handler = handler;
            this.logFactory = logFactory;
            this.userValidator = userValidator;
            this.rootPathProvider = rootPathProvider;
            this.log = logFactory.Create(typeof(BootStrapper));

        }

        /// <summary>
        /// For the loading of Nancy.Validation.FluentValidation
        /// </summary>
        /// <param name="container">A container object.</param>
        /// 
        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {

            base.ConfigureApplicationContainer(container);

            var key = config.Key;
            var section = config.Section;

            // force the loading of Nancy.Validation.FluentValidation. otherwise it
            // dosen't resolve correctly.

            string libsDir = config.GetValue(section.Environment(), key.LibDir());
            var assemblyFile = "Nancy.Validation.FluentValidation.dll";
            string targetPath = Path.Combine(libsDir, assemblyFile);

            Assembly.LoadFile(targetPath);

            // configuration

            container.Register(typeof(IManager), manager);
            container.Register(typeof(IConfiguration), config);
            container.Register(typeof(IErrorHandler), handler);
            container.Register(typeof(ILoggerFactory), logFactory);
            container.Register(typeof(IUserValidator), userValidator);


        }

        /// <summary>
        /// Provide the RootPath to Nancy.
        /// </summary>
        /// 
        protected override IRootPathProvider RootPathProvider {
            get {
                return rootPathProvider;
            }
        }

        /// <summary>
        /// Defaults for application startup.
        /// </summary>
        /// <param name="container">A TinyIoc container.</param>
        /// <param name="pipelines">The Nancy pipelines.</param>
        /// 
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {

            base.ApplicationStartup(container, pipelines);

            var userValidator = container.Resolve<IUserValidator>();

            pipelines.EnableBasicAuthentication(
                new BasicAuthenticationConfiguration(
                    userValidator,
                    "XAS",
                    UserPromptBehaviour.Always
                )
            );

        }

        /// <summary>
        /// Setup default error reporting.
        /// </summary>
        /// 
        protected override NancyInternalConfiguration InternalConfiguration {

            get {
                return NancyInternalConfiguration.WithOverrides(config => {
                    config.StatusCodeHandlers = new[] {
                        typeof(StatusCodeHandler400),
                        typeof(StatusCodeHandler401),
                        typeof(StatusCodeHandler403),
                        typeof(StatusCodeHandler404),
                        typeof(StatusCodeHandler406),
                        typeof(StatusCodeHandler411),
                        typeof(StatusCodeHandler422),
                        typeof(StatusCodeHandler500),
                        typeof(StatusCodeHandler503)
                    };
                });
            }

        }

        /// <summary>
        /// Enable default error reporting.
        /// </summary>
        /// <param name="container">A TinyIoc container.</param>
        /// <param name="pipelines">Nancy pipelines.</param>
        /// <param name="context">Nancy Context.</param>
        /// 
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context) {

            CustomErrorHandler.Enable(pipelines, container.Resolve<IResponseNegotiator>());

        }

        private static string ExtractRootPath() {

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var location = assembly.Location;

            return Path.GetDirectoryName(location);

        }

    }

}
