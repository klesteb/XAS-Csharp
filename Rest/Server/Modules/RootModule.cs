using Nancy;
using Nancy.Hal;
using Nancy.Responses.Negotiation;
using XAS.Core.Logging;
using XAS.Rest.Server.Repository;

namespace XAS.Rest.Server.Modules {

    /// <summary>
    /// A class for creating the default HAL root.
    /// </summary>
    /// 
    public class RootModule: Base {

        private readonly ILogger log = null;

        /// <summary>
        /// The root link.
        /// </summary>
        /// 
        public static Link Self = new Link("self", "/", "Root");

        /// <summary>
        /// The root link for navagating up the linl chain.
        /// </summary>
        /// 
        public static Link Root = new Link("root", "/", "Root");

        /// <summary>
        /// A module to handle the root of an application
        /// </summary>
        /// <param name="logFactory">A logging factory to initialize logging.</param>
        /// <param name="ResourceConfiguration">The resources exposed by this application.</param>
        /// 
        public RootModule(ILoggerFactory logFactory, IResourceConfiguration ResourceConfiguration) {

            log = logFactory.Create(typeof(RootModule));
            log.Trace("Entering RootModule()");

            Get["/"] = _ => {

                log.Debug("Processing GET(/)");

                return Negotiate
                    .WithView("index")
                    .WithModel(ResourceConfiguration)
                    .WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Headers", "*");

            };

        }

    }

}
