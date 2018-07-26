using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using XAS.Model.Paging;
using XAS.Rest.Server.Errors.Exceptions;

using DemoMicroServiceServer.Web.Services;
using DemoMicroServiceServer.Web.Requests;

using DemoMicroServiceServer.Model.Schema;
using DemoMicroServiceServer.Model.Repository;

namespace DemoMicroServiceServer.Model.Services {

    public class DinosaurService: IDinosaurService {

        public DinosaurRepository Repository { get; set; }

        public DinosaurService(DinosaurRepository repo) {

            this.Repository = repo;

        }

        public DinosaurDTO CreateDinosaur(DinosaurFMI binding) {

            int id = 0;

            if (!Repository.IsDinosaur(binding.Name)) {

                id = Repository.CreateDinosaur(binding.Name, binding.Height, binding.Status);

            }

            return GetDinosaur(id);

        }

        public IEnumerable<DinosaurDTO> GetDinosaurs() {

            return Repository.GetAllDinosaurs()
                .Select(item => new DinosaurDTO {
                    Id = item.Id,
                    Name = item.Name,
                    Status = item.Status,
                    HeightInFeet = item.HeightInFeet
                }
            );

        }

        public DinosaurDTO GetDinosaur(int id) {

            DinosaurDTO dino = null;

            if (Repository.IsDinosaur(id)) {

                Dinosaur temp = null;

                if ((temp = Repository.GetDinosaur(id)) != null) {

                    dino = new DinosaurDTO {
                        Id = temp.Id,
                        Name = temp.Name,
                        Status = temp.Status,
                        HeightInFeet = temp.HeightInFeet
                    };

                }

            }

            return dino;

        }

        public DinosaurDTO UpdateDinosaur(Int32 id, DinosaurFMI binding) {

            DinosaurDTO dino = null;

            if (Repository.UpdateDinosaur(id, binding.Name, binding.Height, binding.Status)) {

                Dinosaur temp = null;

                if ((temp = Repository.GetDinosaur(id)) != null) {

                    dino = new DinosaurDTO {
                        Id = temp.Id,
                        Name = temp.Name,
                        Status = temp.Status,
                        HeightInFeet = temp.HeightInFeet
                    };

                }

            } else {

                throw new NotFoundErrorException(String.Format("invalid dinosaur: {0}", binding.Name));
            }

            return dino;

        }

        public int DeleteDinosaur(int id) {

            var dino = Repository.GetDinosaur(id);

            if (dino == null) {

                throw new NotFoundErrorException(String.Format("invalid id: {0}", id));

            }

            Repository.DeleteDinosaur(id);

            return id;

        }

        public IPagedList<DinosaurDTO> GetPage(DinosaurPagedCriteria criteria) {

            Dictionary<String, ListSortDirection> sortDir = new Dictionary<String, ListSortDirection>();

            if (String.IsNullOrEmpty(criteria.SortDir)) {

                sortDir.Add("asc", ListSortDirection.Ascending);

            } else {

                if (criteria.SortDir.Equals("asc", StringComparison.OrdinalIgnoreCase)) {

                    sortDir.Add("asc", ListSortDirection.Ascending);

                } else {

                    sortDir.Add("desc", ListSortDirection.Descending );

                }

            }

            var dbCriteria = new DinosaurCriteria() {
                Page = (criteria.Page ?? 1),
                PageSize = (criteria.PageSize ?? 20),
                SortBy = (criteria.SortBy ?? new string[0]).ToList(),
                SortDir = sortDir
            };

            var page = Repository.Page(dbCriteria);

            return new PagedList<DinosaurDTO>(
                page.PageNumber,
                page.PageSize,
                page.TotalResults,
                criteria.SortBy,
                dbCriteria.SortDir,
                page.Data.Select(db => 
                    new DinosaurDTO { Id = db.Id, Name = db.Name, HeightInFeet = db.HeightInFeet, Status= db.Status }
                ).ToList()
            );

        }

    }

}
