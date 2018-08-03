using System.Data.Entity;

using XAS.Model;
using DemoModel.Schema;

namespace DemoModel.Repository {

    public class DinosaurRepository: Repository<Dinosaurs> {

        public DinosaurRepository(DbContext context):  base(context) { }

    }

}