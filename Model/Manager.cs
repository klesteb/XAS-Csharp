using System.Data.Entity;

namespace XAS.Model {

    public class Manager: IManager {

        public DbContext Context { get; set; }
        public Repositories Repository { get; set; }

        public Manager(DbContext context, Repositories repository) {

            this.Context = context;
            this.Repository = repository;

        }

    }

}
