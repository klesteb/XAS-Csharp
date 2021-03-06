﻿using System;

using Nancy;
using Nancy.Hal;
using Nancy.Security;
using Nancy.Validation;
using Nancy.ModelBinding;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Rest.Server.Errors;

using DemoModel.Service;
using DemoModelCommon.DataStructures;
using DemoMicroServiceServer.Web.Services;

namespace DemoMicroServiceServer.Web.Modules {

    public class DinosaurModule: NancyModule {

        private readonly ILogger log = null;

        public static Link Self = new Link("self", "/dinosaurs/{id}", "Edit");
        public static Link Create = new Link("create", "/dinosaurs", "Create");
        public static Link Update = new Link("update", "/dinosaurs/{id}", "Update");
        public static Link Remove = new Link("delete", "/dinosaurs/{id}", "Delete");
        public static Link Dinosaurs = new Link("dinosaurs", "/dinosaurs", "Dinosaurs");
        public static Link Paged = new Link("paged", "/dinosaurs/{?page,pageSize,sortBy,sortDir}", "List");

        public const string root = "dinosaurs";

        public DinosaurModule(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, IDinoService dinoService): base(root) {

            log = logFactory.Create(typeof(DinosaurModule));
            log.Trace("Entering DinosaurModule()");

            Get["/"] = _ => {

                this.RequiresAuthentication();

                log.Debug(String.Format("Processing GET(/) for {0}", this.Context.CurrentUser.UserName));

                var criteria = this.Bind<DinosaursPagedCriteria>();
                return Negotiate
                    .WithModel(dinoService.Paged(criteria));

            };

            Get["/list"] = _ => {

                this.RequiresAuthentication();

                log.Debug(String.Format("Processing GET(/list) for {0}", this.Context.CurrentUser.UserName));

                return Negotiate
                    .WithModel(dinoService.List());

            };

            Get["/{id:int}"] = p => {

                this.RequiresAuthentication();

                log.Debug(String.Format("Processing GET(/{0}) for {1}", p.id, this.Context.CurrentUser.UserName));

                DinosaurDTO dino = null;

                if ((dino = dinoService.Get(p.id)) == null) {

                    return Negotiate
                        .WithStatusCode(HttpStatusCode.NotFound);

                }

                return Negotiate
                    .WithModel(dino);

            };

            Post["/"] = _ => {

                this.RequiresAuthentication();

                log.Debug(String.Format("Processing POST(/) for {0}", this.Context.CurrentUser.UserName));

                var binding = this.Bind<DinosaurPost>();
                var results = this.Validate(binding);

                if (results.IsValid) {

                    var dino = dinoService.Create(binding);

                    return Negotiate
                        .WithModel(dino)
                        .WithStatusCode(HttpStatusCode.Accepted)
                        .WithHeader("Location", String.Format("/{0}/{1}", root, dino.Id));

                } else {

                    var validationError = ServiceErrorUtilities.ValidationErrors(results);

                    return Negotiate
                        .WithModel(validationError)
                        .WithHeader("Content-Type", "application/problem+json")
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity);

                }

            };

            Put["/{id:int}"] = p => {

                this.RequiresAuthentication();

                log.Debug(String.Format("Processing PUT(/{0}) for {1}", p.id, this.Context.CurrentUser.UserName));

                Int32 id = p.id;
                var binding = this.Bind<DinosaurUpdate>();
                var results = this.Validate(binding);

                if (results.IsValid) {

                    var dino = dinoService.Update(id, binding);

                    return Negotiate
                         .WithModel(dino)
                         .WithStatusCode(HttpStatusCode.Accepted)
                         .WithHeader("Location", String.Format("/{0}/{1}", root, dino.Id));

                } else {

                    var validationError = ServiceErrorUtilities.ValidationErrors(results);

                    return Negotiate
                        .WithModel(validationError)
                        .WithHeader("Content-Type", "application/problem+json")
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity);

                }

            };

            Delete["/{id:int}"] = p => {

                this.RequiresAuthentication();

                Int32 id = p.id;

                log.Debug(String.Format("Processing DELETE(/{0}) for {1}", id, this.Context.CurrentUser.UserName));

                dinoService.Delete(id);

                return Negotiate
                    .WithStatusCode(HttpStatusCode.NoContent);

            };

            Options["/"] = p => {

                log.Debug(String.Format("Processing OPTIONS(/) for {0}", "anonymous"));

                return Negotiate
                    .WithHeader("Allow", "GET, POST, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8")
                ;

            };

            Options["/{id:int}"] = p => {

                log.Debug(String.Format("Processing OPTIONS(/{0}) for {1}", p.id, "anonymous"));

                return Negotiate
                    .WithHeader("Allow", "GET, DELETE, PUT, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8")
                ;

            };

        }

    }

}
