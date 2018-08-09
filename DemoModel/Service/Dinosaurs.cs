using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using XAS.Model;
using XAS.Model.Paging;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModel.Schema;
using DemoModelCommon.DataStructures;

namespace DemoModel.Service {

    public class DinosaursPagedCriteria: PagedCriteria { }
    public class DinosaursCriteria: Criteria<Dinosaurs> { }

    public class Dinosaur {

        private readonly ILogger log = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;

        public Dinosaur(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Dinosaur));

        }

        public DinosaurDTO Get(Repositories repo, String name) {

            Dinosaurs dino = null;
            DinosaurDTO dto = null;

            if ((dino = repo.Dinosaurs.Find(r => (r.Name == name))) != null) {

                dto = NewDTO(repo, dino);

            }

            return dto;

        }

        public DinosaurDTO Get(Repositories repo, Int32 id) {

            Dinosaurs dino = null;
            DinosaurDTO dto = null;

            if ((dino = repo.Dinosaurs.Find(r => (r.Id == id))) != null) {

                dto = NewDTO(repo, dino);

            }

            return dto;

        }

        public List<DinosaurDTO> List(Repositories repo) {

            var dtos = new List<DinosaurDTO>();

            foreach (var record in repo.Dinosaurs.Search()) {

                dtos.Add(NewDTO(repo, record));

            }

            return dtos;

        }

        public Int32 Create(Repositories repo, DinosaurDTI dti) {

            var record = MoveDTI(repo, dti);
            repo.Dinosaurs.Create(record);
            repo.Save();

            return record.Id;

        }

        public Boolean Delete(Repositories repo, Int32 id) {

            repo.Dinosaurs.Delete(id);
            repo.Save();

            return true;

        }

        public Boolean Update(Repositories repo, Int32 id, DinosaurDTI dti) {

            bool stat = false;
            Dinosaurs dino = null;

            if ((dino = repo.Dinosaurs.Find(r => (r.Id == id))) != null) {

                var record = MergeDTI(repo, dino, dti);

                repo.Dinosaurs.Update(record);
                repo.Save();

                stat = true;

            }

            return stat;

        }

        public IPagedList<DinosaurDTO> Paged(Repositories repo, DinosaursPagedCriteria criteria) {

            PagedList<DinosaurDTO> paged = null;
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

            var dbCriteria = new DinosaursCriteria() {
                Page = (criteria.Page ?? 1),
                PageSize = (criteria.PageSize ?? 20),
                SortBy = (criteria.SortBy ?? new string[0]).ToList(),
                SortDir = sortDir
            };

            var page = repo.Dinosaurs.Page(dbCriteria);

            paged = new PagedList<DinosaurDTO>(
                page.PageNumber,
                page.PageSize,
                page.TotalResults,
                criteria.SortBy,
                dbCriteria.SortDir,
                page.Data.Select(rec => Get(repo, rec.Id)).ToList()
            );

            return paged;

        }

        #region Private Methods

        private DinosaurDTO NewDTO(Repositories repo, Schema.Dinosaurs dino) {

            return new DinosaurDTO {
                Id = dino.Id,
                Name = dino.Name,
                Status = dino.Status,
                Height = dino.Height
            };

        }

        private Dinosaurs MoveDTI(Repositories repo, DinosaurDTI dino) {

            return new Dinosaurs {
                Name = dino.Name,
                Status = dino.Status,
                Height = Convert.ToInt32(dino.Height)
            };

        }

        private Dinosaurs MergeDTI(Repositories repo, Dinosaurs record, DinosaurDTI dti) {

            Int32 height = Convert.ToInt32(dti.Height);

            record.Name = (record.Name != dti.Name)
                ? dti.Name
                : record.Name;

            record.Status = (record.Status != dti.Status)
                ? dti.Status
                : record.Status;

            record.Height = (record.Height != height)
                ? height
                : record.Height;

            return record;

        }

        #endregion

    }

}
