
using XAS.Model;

namespace ServiceSupervisor.Model {

    public class Manager: IManager {

        public IRepositories Repository { get; set; }

        public Manager(IRepositories repository) {

            this.Repository = repository;

        }

    }

}
