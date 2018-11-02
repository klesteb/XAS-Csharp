
namespace ServiceSupervisor.Model {

    public class Manager {

        public Repositories Repository { get; set; }

        public Manager(Repositories repository) {

            this.Repository = repository;

        }

    }

}
