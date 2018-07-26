using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class GroupsTargetsRepository: Repository<GroupsTargets> {

        public GroupsTargetsRepository(DbContext context): base(context) { }

    }

}