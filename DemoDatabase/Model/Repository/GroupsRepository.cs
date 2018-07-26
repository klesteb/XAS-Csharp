using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class GroupsRepository: Repository<Groups> {

        public GroupsRepository(DbContext context): base(context) { }

    }

}