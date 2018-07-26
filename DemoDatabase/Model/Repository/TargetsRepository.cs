using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class TargetsRepository: Repository<Targets> {

        public TargetsRepository(DbContext context): base(context) { }

    }

}