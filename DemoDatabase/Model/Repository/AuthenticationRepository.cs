using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class AuthenticationRepository: Repository<Authentication> {

        public AuthenticationRepository(DbContext context): base(context) {
        }

    }

}
