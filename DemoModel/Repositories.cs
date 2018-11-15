using System.Data.Entity;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModel.Repository;

namespace DemoModel {

    /// <summary>
    /// Define the interface to the repositories.
    /// </summary>
    ///
    public class Repositories: XAS.Model.Database.Repositories {

        public DinosaurRepository Dinosaurs { get; private set; }

        public Repositories(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, DbContext context): 
            base(config, handler, logFactory, context) {

            this.Dinosaurs = new DinosaurRepository(config, handler, logFactory, context);

        }

    }

}
