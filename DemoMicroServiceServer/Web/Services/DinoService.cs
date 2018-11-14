using System;
using System.Collections.Generic;

using XAS.Model.Paging;
using XAS.Core.Logging;
using XAS.Model.Database;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoModel.Service;
using DemoModelCommon.DataStructures;
using DemoMicroServiceServer.Configuration.Extensions;

namespace DemoMicroServiceServer.Web.Services {

    /// <summary>
    /// Service interface to Dinosaurs.
    /// </summary>
    /// 
    public class DinoService: IDinoService {

        private object _critical = null;
        private readonly ILogger log = null;
        private readonly IManager manager = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly DemoModel.Service.Dinosaur dino = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public DinoService(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            _critical = new Object();

            this.config = config;
            this.handler = handler;

            var key = config.Key;
            var section = config.Section;
            string model = config.GetValue(section.Database(), key.Model(), "DemoDatabase");
           
            var context = new DemoModel.Context(null, model);
            var repository = new DemoModel.Repositories(config, handler, logFactory, context);

            this.manager = new Manager(context, repository);
            this.dino = new DemoModel.Service.Dinosaur(config, handler, logFactory);
            this.log = logFactory.Create(typeof(DinoService));

            log.Trace("Initialized DinoService()");

        }

        public DinosaurDTO Get(Int32 id) {

            var dto = new DinosaurDTO();

            using (var repo = manager.Repository as DemoModel.Repositories) {

                dto = dino.Get(repo, id);

            }

            return dto;

        }

        public DinosaurDTO Create(DinosaurPost binding) {

            Int32 id = 0;
            DinosaurDTO dto = null;

            using (var repo = manager.Repository as DemoModel.Repositories) {

                var dti = MoveBinding(repo, binding);

                if ((id = dino.Create(repo, dti)) > 0) {

                    dto = dino.Get(repo, id);

                }
                
            }

            return dto;

        }

        public DinosaurDTO Update(Int32 id, DinosaurUpdate binding) {

            DinosaurDTO dto = null;

            using (var repo = manager.Repository as DemoModel.Repositories) {

                var dti = MoveBinding(repo, binding);

                if (dino.Update(repo, id, dti)) {

                    dto = dino.Get(repo, id);

                }

            }

            return dto;

        }

        public Boolean Delete(Int32 id) {

            bool stat = false;

            using (var repo = manager.Repository as DemoModel.Repositories) {

                stat = dino.Delete(repo, id);

            }

            return stat;
        }

        public List<DinosaurDTO> List() {

            var dtos = new List<DinosaurDTO>();

            using (var repo = manager.Repository as DemoModel.Repositories) {

                dtos = dino.List(repo);

            }

            return dtos;

        }

        public IPagedList<DinosaurDTO> Paged(DinosaursPagedCriteria criteria) {

            var dtos = new Object();

            using (var repo = manager.Repository as DemoModel.Repositories) {

                dtos = dino.Paged(repo, criteria);

            }

            return dtos as PagedList<DinosaurDTO>;

        }

        #region Private Methods

        private DinosaurDTI MoveBinding(Repositories repo, DinosaurPost binding) {

            return new DinosaurDTI {
                Name = binding.Name,
                Status = binding.Status,
                Height = binding.Height
            };

        }

        #endregion

    }

}
