using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using XAS.Model.Paging;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using ServiceSupervisor.Model.Schema;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Model.Services {


    public class Supervised {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        public class SupervisedPagedCriteria: PagedCriteria { }
        public class SupervisedCriteria: Criteria<Supervise> { }

        public Supervised(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Supervised));

        }

        public SuperviseDTO Get(Repositories repo, String name) {

            Supervise data = null;
            SuperviseDTO dto = null;

            if ((data = repo.Supervised.Find(r => (r.Name == name))) != null) {

                dto = NewDTO(repo, data);

            }

            return dto;

        }

        public List<SuperviseDTO> List(Repositories repo) {

            var dtos = new List<SuperviseDTO>();

            foreach (var record in repo.Supervised.Search()) {

                dtos.Add(NewDTO(repo, record));

            }

            return dtos;

        }

        public String Create(Repositories repo, SuperviseDTI dti) {

            var record = MoveDTI(repo, dti);

            repo.Supervised.Create(record);
            repo.Save();

            return record.Name;

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

        public Boolean Update(Repositories repo, String name, SuperviseDTI dti) {

            bool stat = false;
            Supervise data = null;

            if ((data = repo.Supervised.Find(r => (r.Name == name))) != null) {

                var record = MergeDTI(repo, data, dti);

                repo.Supervised.Update(record);
                repo.Save();

                stat = true;

            }

            return stat;

        }

        public IPagedList<SuperviseDTO> Paged(Repositories repo, SupervisedPagedCriteria criteria) {

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

            var page = repo.Supervised.Page(dbCriteria);

            paged = new PagedList<SuperviseDTO>(
                page.PageNumber,
                page.PageSize,
                page.TotalResults,
                criteria.SortBy,
                dbCriteria.SortDir,
                page.Data.Select(rec => Get(repo, rec.Name)).ToList()
            );

            return paged;

        }

        #region Private Methods

        private SuperviseDTO NewDTO(Repositories repo, Schema.Supervise data) {

            return new SuperviseDTO {
                Verb = data.Verb,
                Name = data.Name,
                Status = (Int32)data.Status,
                Domain = data.Domain,
                Username = data.Username,
                Password = data.Password,
                RetryCount = data.RetryCount,
                AutoStart = data.AutoStart,
                ExitRetries = data.ExitRetries,
                AutoRestart = data.AutoRestart,
                ExitCodes = data.ExitCodes,
                WorkingDirectory = data.WorkingDirectory,
                Environment = data.Environment
            };

        }

        private Supervise MoveDTI(Repositories repo, SuperviseDTI data) {

            return new Supervise {
                Verb = data.Verb,
                Name = data.Name,
                Status = RunStatus.Stopped,
                Domain = data.Domain,
                Username = data.Username,
                Password = data.Password,
                RetryCount = 0,
                AutoStart = data.AutoStart,
                ExitRetries = data.ExitRetries,
                AutoRestart = data.AutoRestart,
                ExitCodes = data.ExitCodes,
                WorkingDirectory = data.WorkingDirectory,
                Environment = data.Environment
            };

        }

        private Supervise MergeDTI(Repositories repo, Supervise record, SuperviseDTI data) {

            record.Verb = (data.Verb != "")
                ? data.Verb
                : record.Verb;

            record.Name = (data.Name != "")
                ? data.Name
                : record.Name;

            record.Domain = (data.Domain != "")
                ? data.Domain
                : record.Domain;

            record.Username = (data.Username != "")
                ? data.Username
                : record.Username;

            record.Password = (data.Password != "")
                ? data.Password
                : record.Password;

            record.AutoStart = (data.AutoStart != record.AutoStart)
                ? data.AutoStart
                : record.AutoStart;

            record.ExitRetries = (data.ExitRetries != record.ExitRetries)
                ? data.ExitRetries
                : record.ExitRetries;

            record.AutoRestart = (data.AutoRestart != record.AutoRestart)
                ? data.AutoRestart
                : record.AutoRestart;

            record.ExitCodes = (data.ExitCodes.Count() > 0)
                ? data.ExitCodes
                : record.ExitCodes;

            record.WorkingDirectory = (data.WorkingDirectory != "")
                ? data.WorkingDirectory
                : record.WorkingDirectory;

            record.Environment = (data.Environment.Count() > 0)
                ? data.Environment
                : record.Environment;

            return record;

        }

        #endregion

    }

}
