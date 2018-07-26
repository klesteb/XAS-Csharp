using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class ServersAttributesRepository: Repository<ServersAttributes> {

        public ServersAttributesRepository(DbContext context): base(context) { }

    }

}