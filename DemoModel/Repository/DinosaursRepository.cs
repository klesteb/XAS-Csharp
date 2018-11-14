using System.Data.Entity;

using XAS.Core.Logging;
using XAS.Model.Database;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModel.Schema;

namespace DemoModel.Repository {

    public class DinosaurRepository: Repository<Dinosaurs> {

        public DinosaurRepository(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, DbContext context): 
            base(config, handler, logFactory, context) { }

    }

}