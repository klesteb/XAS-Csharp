
using System;

namespace XAS.Model.Memory {

    public class Manager: IManager {

        public Object Context { get; set; }
        public IRepositories Repository { get; set; }

        public Manager(Object context, IRepositories repository) {

            this.Context = context;
            this.Repository = repository;

        }

    }

}
