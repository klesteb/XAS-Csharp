using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using XAS.Model.Paging;
using DemoMicroServiceServer.Model.Schema;

namespace DemoMicroServiceServer.Model.Repository {

    public class DinosaurCriteria: Criteria<Dinosaur> { }

    public class DinosaurRepository {

        private int id = 4;
        private List<Dinosaur> dinosaurs = new List<Dinosaur>() {
            new Dinosaur { Id = 1, Name = "Kirekegarrd", HeightInFeet = 0, Status = "Defaulted" },
            new Dinosaur { Id = 2, Name = "Voltarie", HeightInFeet = 10, Status = "Inflated" },
            new Dinosaur { Id = 3, Name = "Plato", HeightInFeet = 6, Status = "Unknown" },
            new Dinosaur { Id = 4, Name = "Barney", HeightInFeet = 20, Status = "Famous" }
        };

        // access methods

        public Dinosaur GetDinosaur(int id) {

            return dinosaurs.FirstOrDefault(r => r.Id == id);

        }

        public Dinosaur GetDinosaur(string name) {

            return dinosaurs.FirstOrDefault(r => r.Name == name);

        }

        public IEnumerable<Dinosaur> GetAllDinosaurs() {

            return dinosaurs;

        }

        public bool IsDinosaur(int id) {

            return dinosaurs.Any(r => r.Id == id);

        }

        public bool IsDinosaur(string name) {

            return dinosaurs.Any(r => r.Name == name);

        }

        public int CreateDinosaur(string name, int height, string status) {

            id++;
            Dinosaur dino = new Dinosaur {
                Id = id,
                Name = name,
                HeightInFeet = height,
                Status = status
            };

            dinosaurs.Add(dino);

            return id;

        }

        public bool UpdateDinosaur(int id, string name, int height, string status) {
            
            bool stat = false;
            Dinosaur dino = null;
            
            if ((dino = dinosaurs.Single(r => r.Id == id)) != null) {

                dino.Name = name;
                dino.Status = status;
                dino.HeightInFeet = height;

                stat = true;

            }

            return stat;

        }

        public bool UpdateDinosaur(string name, int height, string status) {

            bool stat = false;
            Dinosaur dino = null;

            if ((dino = dinosaurs.Single(r => r.Name == name)) != null) {

                dino.Status = status;
                dino.HeightInFeet = height;
                stat = true;

            }

            return stat;

        }

        public bool DeleteDinosaur(int id) {

            bool stat = false;
            Dinosaur dino = null;

            if ((dino = dinosaurs.Find(r => r.Id == id)) != null) {

                dinosaurs.Remove(dino);
                stat = true;

            }

            return stat;

        }

        public IPaged<Dinosaur> Page(ICriteria<Dinosaur> criteria) {

            var query = GetAllDinosaurs();

            var totalRecords = query.Count();

            if (criteria.SortDir.Any()) {

                foreach (var kvp in criteria.SortDir) {

                    switch (kvp.Value) {
                        case ListSortDirection.Ascending:
                            query = query.OrderBy(r => kvp.Key);
                            break;

                        case ListSortDirection.Descending:
                            query = query.OrderBy(r => (kvp.Key + " descending"));
                            break;
                    }

                }

            }

            if (criteria.Page.HasValue) {

                query = query.Skip((criteria.Page.Value - 1) * criteria.PageSize.GetValueOrDefault());

            }

            if (criteria.PageSize.HasValue) {

                query = query.Take(criteria.PageSize.Value);

            }

            return new Paged<Dinosaur>(
                criteria.Page ?? 0, 
                criteria.PageSize ?? totalRecords, 
                totalRecords, 
                query
            );

        }

    }

}
