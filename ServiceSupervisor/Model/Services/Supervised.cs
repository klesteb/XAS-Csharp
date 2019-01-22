using System;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Schema;

namespace ServiceSupervisor.Model.Services {

    /// <summary>
    /// Service class for SupervisedProcess table.
    /// </summary>
    /// 
    public class Supervised {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        public Supervised(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Supervised));

        }

        public SupervisedProcess Get(Repositories repo, String name) {

            return repo.Supervised.Find(r => (r.Name == name));

        }

        public SupervisedProcess Get(Repositories repo, Int32 pid) {

            return repo.Supervised.Find(r => (r.Pid == pid));

        }

        public List<SupervisedProcess> List(Repositories repo) {

            var dtos = new List<SupervisedProcess>();

            foreach (var record in repo.Supervised.Search()) {

                dtos.Add(record);

            }

            return dtos;

        }

        public String Create(Repositories repo, SupervisedProcess dti) {

            repo.Supervised.Create(dti);
            repo.Save();

            return dti.Name;

        }

        public Boolean Delete(Repositories repo, String name) {

            bool stat = false;
            var datum = repo.Supervised.Search(r => (r.Name == name));

            if (datum != null) {

                foreach (var data in datum) {

                    repo.Supervised.Delete(data);
                    stat = true;

                }

                repo.Save();

            }

            return stat;

        }

        public Boolean Update(Repositories repo, String name, SupervisedProcess dti) {

            bool stat = false;
            SupervisedProcess data = null;

            if ((data = repo.Supervised.Find(r => (r.Name == name))) != null) {

                repo.Supervised.Update(dti);
                repo.Save();

                stat = true;

            }

            return stat;

        }

    }

}
