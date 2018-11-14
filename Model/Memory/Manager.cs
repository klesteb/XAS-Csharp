

namespace XAS.Model.Memory {

    public class Manager: IManager {

        public IRepositories Repository { get; set; }

        public Manager(IRepositories repository) {

            this.Repository = repository;

        }

    }

}
