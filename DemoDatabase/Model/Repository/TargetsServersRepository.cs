using System.Data.Entity;

using XAS.Model;

using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class TargetsServersRepository: Repository<TargetsServers> {

        public TargetsServersRepository(DbContext context): base(context) {  }

    }

}