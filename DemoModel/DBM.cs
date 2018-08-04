using System;
using System.Collections.Generic;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModel.Schema;

namespace DemoModel {

    public class DBM: IDBM {

        private readonly ILogger log = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;
        private readonly ILoggerFactory logFactory = null;

        public DBM(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.logFactory = logFactory;
            this.log = logFactory.Create(typeof(DBM));

        }
        
        public void Populate(XAS.Model.Context db) {

            // Seed the database.

            var repo = new Repositories(config, handler, logFactory, db);

            log.Info("Populating tables");

            List<Dinosaurs> dinosaurs = new List<Dinosaurs>() {
                new Dinosaurs { Id = 1, Name = "Kirekegarrd", Height = 0, Status = "Defaulted" },
                new Dinosaurs { Id = 2, Name = "Voltarie", Height = 10, Status = "Inflated" },
                new Dinosaurs { Id = 3, Name = "Plato", Height = 6, Status = "Unknown" },
                new Dinosaurs { Id = 4, Name = "Barney", Height = 20, Status = "Famous" }
            };

            repo.Dinosaurs.Populate(dinosaurs);
            repo.Save();

        }

    }

}
