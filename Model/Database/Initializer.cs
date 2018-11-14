using System.Data.Entity;

namespace XAS.Model.Database {

    public class Initializer: CreateDatabaseIfNotExists<Context> {

        private readonly IDBM dbm = null;

        public DbModelBuilder ModelBuilder { get;set; }

        public Initializer(IDBM dbm) {

            this.dbm = dbm;

        }

        protected override void Seed(Context db) {

            if (dbm != null) {

                dbm.Populate(db);

            }

            base.Seed(db);

        }

    }

}
