using System;
using System.ComponentModel;
using System.Collections.Generic;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModel.Schema;
using XAS.Model.Paging;
using System.Linq;

namespace DemoModel.Service {

    public class DinosaursPagedCriteria: PagedCriteria { }
    public class DinosaursCriteria: Criteria<Dinosaurs> { }

    public class DinosaurDTO {

        public Int32 Id { get;set; }
        public String Name { get; set; }
        public String Status { get; set; }
        public Int32 HeightInFeet { get; set; }

    }

    public class DinosaurDTI {
    
        public String Name { get; set; }
        public String Status { get; set; }
        public String HeightInFeet { get; set; }

    }

    public class Dinosaur {

        private readonly ILogger log = null;
        private readonly IManager manager = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;


        public Dinosaur(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IManager manager) {

            this.config = config;
            this.handler = handler;
            this.manager = manager;
            this.log = logFactory.Create(typeof(Dinosaur));

        }

        public DinosaurDTO Get(String name) {

            Dinosaurs dino = null;
            DinosaurDTO dto = null;

            using (var repo = manager.Repository as Repositories) {

                if ((dino = repo.Dinosaurs.Find(r => (r.Name == name))) != null) {

                    dto = NewDTO(repo, dino);

                }

            }

            return dto;

        }

        public DinosaurDTO Get(Int32 id) {

            Dinosaurs dino = null;
            DinosaurDTO dto = null;

            using (var repo = manager.Repository as Repositories) {

                if ((dino = repo.Dinosaurs.Find(r => (r.Id == id))) != null) {

                    dto = NewDTO(repo, dino);

                }

            }

            return dto;

        }

        public List<DinosaurDTO> List() {

            var dtos = new List<DinosaurDTO>();

            using (var repo = manager.Repository as Repositories) {

                foreach (var record = repo.Dinosaurs.Search()) {

                    dtos.Add(NewDTO(repo, record));

                }

            }

            return dtos;

        }

        public Int32 Create(DinosaurDTI dti) {

            Int32 id = 0;

            using (var repo = manager.Repository as Repositories) {

                var record = MoveDTI(repo, dti);
                repo.Dinosaurs.Create(record);
                repo.Save();

                id = record.Id;

            }

            return id;

        }

        public Boolean Delete(Int32 id) {

            bool stat = false;

            using (var repo = manager.Repository as Repositories) {

                repo.Dinosaurs.Delete(id);
                repo.Save();

                stat = true;

            }

            return stat;

        }

        public Boolean Update(Int32 id, DinosaurDTI dti) {

            bool stat = false;
            Dinosaurs dino = null;

            using (var repo = manager.Repository as Repositories) {

                if ((dino = repo.Dinosaurs.Find(r => (r.Id == id))) != null) {

                    var record = MergeDTI(repo, dino, dti);

                    repo.Dinosaurs.Update(record);
                    repo.Save();

                    stat = true;

                }

            }

            return stat;

        }

        public IPagedList<DinosaurDTO> Paged(DinosaursPagedCriteria criteria) {

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

            using (var repo = manager.Repository as Repositories) {

                var page = repo.Dinosaurs.Page(dbCriteria);

                paged = new PagedList<DinosaurDTO>(
                    page.PageNumber,
                    page.PageSize,
                    page.TotalResults,
                    criteria.SortBy,
                    dbCriteria.SortDir,
                    page.Data.Select(rec => Get(rec.Id)).ToList()
                );

            }

            return paged;

        }

        #region Private Methods

        private DinosaurDTO NewDTO(Repositories repo, Schema.Dinosaurs dino) {

            return new DinosaurDTO {
                Id = dino.Id,
                Name = dino.Name,
                Status = dino.Status,
                HeightInFeet = dino.HeightInFeet
            };

        }

        private Dinosaurs MoveDTI(Repositories repo, DinosaurDTI dino) {

            return new Dinosaurs {
                Name = dino.Name,
                Status = dino.Status,
                HeightInFeet = Convert.ToInt32(dino.HeightInFeet)
            };

        }

        private Dinosaurs MergeDTI(Repositories repo, Dinosaurs record, DinosaurDTI dti) {

            Int32 heightInFeet = Convert.ToInt32(dti.HeightInFeet);

            record.Name = (record.Name != dti.Name)
                ? dti.Name
                : record.Name;

            record.Status = (record.Status != dit.Status)
                ? dti.Status
                : record.Status;

            record.HeightInFeet = (record.HeightInFeet != heightInFeet)
                ? heightInFeet
                : record.HeightInFeet;

            return record;

        }

        #endregion

    }

}
