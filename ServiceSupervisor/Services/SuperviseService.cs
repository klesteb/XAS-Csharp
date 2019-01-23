using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using XAS.Model;
using XAS.Model.Paging;
using XAS.Core.Logging;
using XAS.Core.Extensions;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Schema;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Services {

    public class SupervisedPagedCriteria: PagedCriteria { }
    public class SupervisedCriteria: Criteria<SupervisedProcess> { }

    /// <summary>
    /// A repository service.
    /// </summary>
    /// 
    public class SuperviseService: ISuperviseService {

        private readonly ILogger log = null;
        private readonly IManager manager = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly Model.Services.Supervised service = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// <param name ="manager">An IManager object.</param>
        /// 
        public SuperviseService(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IManager manager) {

            this.config = config;
            this.handler = handler;
            this.manager = manager;

            var key = config.Key;
            var section = config.Section;

            this.service = new Model.Services.Supervised(config, handler, logFactory);
            this.log = logFactory.Create(typeof(SuperviseService));

            log.Trace("Initialized SuperviseService()");

        }

        public SuperviseDTO Get(String name) {

            var dto = new SuperviseDTO();

            using (var repo = manager.Repository as Model.Repositories) {

                var data = service.Get(repo, name);
                dto = NewDTO(repo, data);

            }

            return dto;

        }

        public SuperviseDTO Create(SupervisePost binding) {

            string name;
            SuperviseDTO dto = null;

            using (var repo = manager.Repository as Model.Repositories) {

                var dti = MoveBinding(binding);

                if ((name = service.Create(repo, dti)) != null) {

                    var data = service.Get(repo, name);
                    dto = NewDTO(repo, data);

                }

            }

            return dto;

        }

        public SuperviseDTO Update(String name, SuperviseUpdate binding) {

            SuperviseDTO dto = null;

            using (var repo = manager.Repository as Model.Repositories) {

                var dti = MoveBinding(binding);

                if (service.Update(repo, name, dti)) {

                    var data = service.Get(repo, name);
                    dto = NewDTO(repo, data);

                }

            }

            return dto;

        }

        public Boolean Delete(String name) {

            bool stat = false;

            using (var repo = manager.Repository as Model.Repositories) {

                stat = service.Delete(repo, name);

            }

            return stat;
        }

        public List<SuperviseDTO> List() {

            var dtos = new List<SuperviseDTO>();

            using (var repo = manager.Repository as Model.Repositories) {

                var data = service.List(repo);

                foreach (var item in data) {

                    dtos.Add(NewDTO(repo, item));

                }

            }

            return dtos;

        }

        public IPagedList<SuperviseDTO> Paged(SupervisedPagedCriteria criteria) {

            PagedList<SuperviseDTO> paged = null;
            Dictionary<String, ListSortDirection> sortDir = new Dictionary<String, ListSortDirection>();

            if (String.IsNullOrEmpty(criteria.SortDir)) {

                sortDir.Add("asc", ListSortDirection.Ascending);

            } else {

                if (criteria.SortDir.Equals("asc", StringComparison.OrdinalIgnoreCase)) {

                    sortDir.Add("asc", ListSortDirection.Ascending);

                } else {

                    sortDir.Add("desc", ListSortDirection.Descending);

                }

            }

            var dbCriteria = new SupervisedCriteria() {
                Page = (criteria.Page ?? 1),
                PageSize = (criteria.PageSize ?? 20),
                SortBy = (criteria.SortBy ?? new string[0]).ToList(),
                SortDir = sortDir
            };

            using (var repo = manager.Repository as Model.Repositories) {

                var page = repo.Supervised.Page(dbCriteria);

                paged = new PagedList<SuperviseDTO>(
                    page.PageNumber,
                    page.PageSize,
                    page.TotalResults,
                    criteria.SortBy,
                    dbCriteria.SortDir,
                    page.Data.Select(rec => Get(rec.Name)).ToList()
                );

            }

            return paged;

        }

        public Boolean Start(String name) {

            bool stat = false;

            using (var repo = manager.Repository as Model.Repositories) {

                var job = service.Get(repo, name);

                if (job != null) {

                    if (job.Status == RunStatus.Stopped) {

                        job.Spawn.Start();
                        stat = true;

                    }

                }

            }

            return stat;

        }

        public Boolean Stop(String name) {

            bool stat = false;

            using (var repo = manager.Repository as Model.Repositories) {

                var job = service.Get(repo, name);

                if (job != null) {

                    if (job.Status != RunStatus.Stopped) {

                        job.Spawn.Stop();
                        stat = true;

                    }

                }

            }

            return stat;

        }

        #region Private Methods

        private SuperviseDTO NewDTO(IRepositories repo, SupervisedProcess data) {

            return new SuperviseDTO {
                Name = data.Name,
                Verb = data.Config.Verb,
                Status = (Int32)data.Status,
                Domain = data.Config.Domain,
                Username = data.Config.Username,
                Password = data.Config.Password,
                RetryCount = data.RetryCount,
                ExitRetries = data.Config.ExitRetries,
                AutoRestart = data.Config.AutoRestart,
                ExitCodes = data.Config.ExitCodes,
                WorkingDirectory = data.Config.WorkingDirectory,
                Environment = data.Config.Environment
            };

        }

        private SupervisedProcess MoveDTI(IRepositories repo, SuperviseDTI data) {

            var sp = new SupervisedProcess();

            sp.Name = data.Name;
            sp.Config.Verb = data.Verb;
            sp.Config.Domain = data.Domain;
            sp.Config.Username = data.Username;
            sp.Config.Password = data.Password;
            sp.Config.ExitRetries = data.ExitRetries;
            sp.Config.AutoRestart = data.AutoRestart;
            sp.Config.ExitCodes = data.ExitCodes;
            sp.Config.WorkingDirectory = data.WorkingDirectory;
            sp.Config.Environment = data.Environment;

            return sp;

        }

        private SupervisedProcess MergeDTI(IRepositories repo, SupervisedProcess record, SuperviseDTI data) {

            record.Config.Verb = (data.Verb != "")
                ? data.Verb
                : record.Config.Verb;

            record.Name = (data.Name != "")
                ? data.Name
                : record.Name;

            record.Config.Domain = (data.Domain != "")
                ? data.Domain
                : record.Config.Domain;

            record.Config.Username = (data.Username != "")
                ? data.Username
                : record.Config.Username;

            record.Config.Password = (data.Password != "")
                ? data.Password
                : record.Config.Password;

            record.Config.ExitRetries = (data.ExitRetries != record.Config.ExitRetries)
                ? data.ExitRetries
                : record.Config.ExitRetries;

            record.Config.AutoRestart = (data.AutoRestart != record.Config.AutoRestart)
                ? data.AutoRestart
                : record.Config.AutoRestart;

            record.Config.ExitCodes = (data.ExitCodes.Count() > 0)
                ? data.ExitCodes
                : record.Config.ExitCodes;

            record.Config.WorkingDirectory = (data.WorkingDirectory != "")
                ? data.WorkingDirectory
                : record.Config.WorkingDirectory;

            record.Config.Environment = (data.Environment.Count() > 0)
                ? data.Environment
                : record.Config.Environment;

            return record;

        }

        private SupervisedProcess MoveBinding(SupervisePost binding) {

            var sp = new SupervisedProcess();

            sp.Name = binding.Name;
            sp.Config.Verb = binding.Verb;
            sp.Config.Domain = binding.Domain;
            sp.Config.Username = binding.Username;
            sp.Config.Password = binding.Password;
            sp.Config.AutoStart = binding.AutoStart.ToBoolean();
            sp.Config.ExitRetries = binding.ExitRetries.ToInt32();
            sp.Config.ExitCodes = binding.ExitCodes.ToInt32List();
            sp.Config.WorkingDirectory = binding.WorkingDirectory;
            sp.Config.Environment = binding.Environment.ToKeyValuePairs();

            return sp;

        }

        #endregion

    }

}
