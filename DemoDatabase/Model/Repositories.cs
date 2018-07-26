using System.Data.Entity;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoDatabase.Model.Repository;

namespace DemoDatabase.Model {

    /// <summary>
    /// Define the interface to the repositories.
    /// </summary>
    ///
    public class Repositories: XAS.Model.Repositories {

        public GroupsRepository Groups { get; private set; }
        public ServersRepository Servers { get; private set; }
        public TargetsRepository Targets { get; private set; }
        public AttributesRepository Attributes { get; private set; }
        public GroupsTargetsRepository GroupsTargets { get; private set; }
        public TargetsServersRepository TargetsServers { get; private set; }
        public AuthenticationRepository Authentication { get; private set; }
        public ServersAttributesRepository ServersAttributes { get; private set; }

        public Repositories(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, DbContext context): 
            base(config, handler, logFactory, context) {

            this.Groups = new GroupsRepository(context);
            this.Servers = new ServersRepository(context);
            this.Targets = new TargetsRepository(context);
            this.Attributes = new AttributesRepository(context);
            this.GroupsTargets = new GroupsTargetsRepository(context);
            this.Authentication = new AuthenticationRepository(context);
            this.TargetsServers = new TargetsServersRepository(context);
            this.ServersAttributes = new ServersAttributesRepository(context);

        }

    }

}
