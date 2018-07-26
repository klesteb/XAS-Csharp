using System.Data.Entity;

using XAS.Model;
using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Repository {

    public class AttributesRepository: Repository<Attributes> {

        public AttributesRepository(DbContext context):  base(context) { }

    }

}