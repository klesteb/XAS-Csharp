using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class ServersRepository: Repository<Servers> {

        public ServersRepository(DbContext context): base(context) { }

    }

}
