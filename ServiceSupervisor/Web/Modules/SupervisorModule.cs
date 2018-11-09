using System;
using System.Collections.Generic;

using Nancy;
using Nancy.Hal;
using Nancy.Security;
using Nancy.Validation;
using Nancy.ModelBinding;

using XAS.Core;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Rest.Server.Errors;

using ServiceSupervisor.Web.Services;
using ServiceSupervisorCommon.DataStructures;

namespace ServiceSupervisor.Web.Modules {

    public class SupervisorModule: NancyModule {

        private readonly ILogger log = null;

        public const string root = "supervisor";

        public static Link Self = new Link("self", "/" + root + "/{name}", "Edit");
        public static Link Create = new Link("create", "/" + root + "/", "Create");
        public static Link Remove = new Link("delete", "/" + root + "/{name}", "Delete");
        public static Link Stop = new Link("stop", "/" + root + "/stop/{name}", "Stop");
        public static Link Start = new Link("start", "/" + root + "/start/{name}", "Start");
        public static Link Update = new Link("update", "/" + root + "/{name}", "Update");
        public static Link Save = new Link("save", "/" + root + "/save", "Save");
        public static Link List = new Link("list", "/" + root + "/list", "List");
        public static Link Paged = new Link("paged", "/" + root + "/{?page,pageSize,sortBy,sortDir}", "Paged");

        public SupervisorModule(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISupervised service): base(root) {

            log = logFactory.Create(typeof(SupervisorModule));
            log.Trace("Entering SupervisorModule()");

            Get["/"] = _ => {

                this.RequiresAuthentication();

                log.InfoMsg("GET", root, this.Context.CurrentUser.UserName);

                var criteria = this.Bind<Model.Services.Supervised.SupervisedPagedCriteria>();

                return Negotiate.WithModel(service.Paged(criteria));

            };


            Get["/{name}"] = p => {

                this.RequiresAuthentication();

                string name = p.name;
                SuperviseDTO data = null;

                log.InfoMsg("GET3", root, name, this.Context.CurrentUser.UserName);

                if ((data = service.Get(name)) != null) {

                    return Negotiate.WithModel(data);

                }

                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);

            };

            Get["/list"] = p => {

                this.RequiresAuthentication();

                List<SuperviseDTO> datum = null;

                log.InfoMsg("GET3", root, "list", this.Context.CurrentUser.UserName);

                if ((datum = service.List()) != null) {

                    return Negotiate.WithModel(datum);

                }

                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);

            };

            Post["/"] = _ => {

                this.RequiresAuthentication();

                log.InfoMsg("POST", root, this.Context.CurrentUser.UserName);

                SuperviseDTO data = null;
                var binding = this.Bind<SupervisePost>();
                log.Debug(String.Format("Binding: {0}", Utils.Dump(binding)));

                var results = this.Validate(binding);
                log.Debug(String.Format("Results: {0}", Utils.Dump(results)));

                if (results.IsValid) {

                    try {

                        if ((data = service.Create(binding)) != null) {

                            return Negotiate
                                .WithStatusCode(HttpStatusCode.Accepted)
                                .WithHeader("Location", String.Format("/{0}/{1}", root, data.Name));

                        }

                        log.WarnMsg("POST_NoCreate", root, this.Context.CurrentUser.UserName);

                        return Negotiate
                            .WithModel(ServiceErrorDefinition.NotAcceptable)
                            .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                            .WithHeader("Content-Type", "application/problem+json");

                    } catch(Exception ex) {

                        var junk = ServiceErrorDefinition.GeneralError;

                        handler.Exceptions(ex);

                        return Negotiate
                            .WithModel(ServiceErrorUtilities.ExtractFromException(ex, junk))
                            .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                            .WithHeader("Content-Type", "application/problem+json");

                    }

                } else {

                    log.WarnMsg("POST_NoValidate", root, this.Context.CurrentUser.UserName);

                    var validationError = ServiceErrorUtilities.ValidationErrors(results);

                    return Negotiate
                        .WithModel(validationError)
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                        .WithHeader("Content-Type", "application/problem+json");

                }

            };

            Put["/{name}"] = p => {

                this.RequiresAuthentication();

                string name = p.name;
                SuperviseDTO data = null;
                log.InfoMsg("PUT", root, name, this.Context.CurrentUser.UserName);

                var binding = this.Bind<SuperviseUpdate>();
                log.Debug(String.Format("Binding: {0}", Utils.Dump(binding)));

                var results = this.Validate(binding);
                log.Debug(String.Format("Results: {0}", Utils.Dump(results)));

                if (results.IsValid) {

                    try {

                        if ((data = service.Update(name, binding)) != null) {

                            return Negotiate
                                .WithStatusCode(HttpStatusCode.Accepted)
                                .WithHeader("Location", String.Format("/{0}/{1}", root, name));

                        }

                        log.WarnMsg("PUT_NoUpdate", root, name, this.Context.CurrentUser.UserName);

                        return Negotiate
                            .WithModel(ServiceErrorDefinition.NotAcceptable)
                            .WithHeader("Content-Type", "application/problem+json")
                            .WithStatusCode(HttpStatusCode.UnprocessableEntity);

                    } catch (Exception ex) {

                        var junk = ServiceErrorDefinition.GeneralError;

                        handler.Exceptions(ex);

                        return Negotiate
                            .WithModel(ServiceErrorUtilities.ExtractFromException(ex, junk))
                            .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                            .WithHeader("Content-Type", "application/problem+json");

                    }

                } else {

                    log.WarnMsg("PUT_NoValidate", root, name, this.Context.CurrentUser.UserName);

                    var validationError = ServiceErrorUtilities.ValidationErrors(results);

                    return Negotiate
                        .WithModel(validationError)
                        .WithHeader("Content-Type", "application/problem+json")
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity);

                }

            };

            Put["/start/{name}"] = p => {

                this.RequiresAuthentication();

                string name = p.name;
                log.InfoMsg("PUT4", root, "start", name, this.Context.CurrentUser.UserName);

                try {

                    if (service.Start(name)) {

                        return Negotiate
                            .WithStatusCode(HttpStatusCode.Accepted)
                            .WithHeader("Location", String.Format("/{0}/{1}", root, name));

                    }

                    log.WarnMsg("PUT_NoStart", root, "start", name, this.Context.CurrentUser.UserName);

                    return Negotiate
                        .WithModel(ServiceErrorDefinition.NotAcceptable)
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                        .WithHeader("Content-Type", "application/problem+json");

                } catch (Exception ex) {

                    var junk = ServiceErrorDefinition.GeneralError;

                    handler.Exceptions(ex);

                    return Negotiate
                        .WithModel(ServiceErrorUtilities.ExtractFromException(ex, junk))
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                        .WithHeader("Content-Type", "application/problem+json");

                }

            };

            Put["/stop/{name}"] = p => {

                this.RequiresAuthentication();

                string name = p.name;
                log.InfoMsg("PUT4", root, "stop", name, this.Context.CurrentUser.UserName);

                try {

                    if (service.Stop(name)) {

                        return Negotiate
                            .WithStatusCode(HttpStatusCode.Accepted)
                            .WithHeader("Location", String.Format("/{0}/{1}", root, name));

                    }

                    log.WarnMsg("PUT_NoStop", root, "stop", name, this.Context.CurrentUser.UserName);

                    return Negotiate
                        .WithModel(ServiceErrorDefinition.NotAcceptable)
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                        .WithHeader("Content-Type", "application/problem+json");

                } catch (Exception ex) {

                    var junk = ServiceErrorDefinition.GeneralError;

                    handler.Exceptions(ex);

                    return Negotiate
                        .WithModel(ServiceErrorUtilities.ExtractFromException(ex, junk))
                        .WithStatusCode(HttpStatusCode.UnprocessableEntity)
                        .WithHeader("Content-Type", "application/problem+json");

                }

            };

            Put["/save"] = p => {

                this.RequiresAuthentication();

                return Negotiate
                    .WithStatusCode(HttpStatusCode.Accepted);

            };

            Delete["/{name}"] = p => {

                this.RequiresAuthentication();

                string name = p.name;

                log.InfoMsg("DELETE", root, name, this.Context.CurrentUser.UserName);

                if (service.Delete(name)) {

                    return Negotiate.WithStatusCode(HttpStatusCode.NoContent);

                }

                log.WarnMsg("DELETE_NoDelete", root, name, this.Context.CurrentUser.UserName);

                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);

            };

            Options["/"] = p => {

                log.InfoMsg("OPTIONS", root, "anonymous");

                return Negotiate
                    .WithHeader("Allow", "GET, POST, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8");

            };

            Options["/save"] = p => {

                log.InfoMsg("OPTIONS3", root, "save", "anonymous");

                return Negotiate
                    .WithHeader("Allow", "PUT, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8");

            };

            Options["/{name}"] = p => {

                string name = p.name;

                log.InfoMsg("OPTIONS3", root, name, "anonymous");

                return Negotiate
                    .WithHeader("Allow", "GET, DELETE, PUT, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8");

            };

            Options["/start/{name}"] = p => {

                string name = p.name;

                log.InfoMsg("OPTIONS4", root, name, "start", "anonymous");

                return Negotiate
                    .WithHeader("Allow", "PUT, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8");

            };

            Options["/stop/{name}"] = p => {

                string name = p.name;

                log.InfoMsg("OPTIONS4", root, name, "stop", "anonymous");

                return Negotiate
                    .WithHeader("Allow", "PUT, OPTIONS")
                    .WithHeader("Accept", "application/json, application/hal+json")
                    .WithHeader("Accept-Charset", "UTF-8");

            };

        }

    }

}
