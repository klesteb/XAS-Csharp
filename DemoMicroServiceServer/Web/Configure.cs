
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

using Nancy.Hal.Configuration;

using XAS.Model.Paging;
using XAS.Rest.Server.Model;
using XAS.Rest.Server.Modules;
using XAS.Rest.Server.Repository;

using DemoModelCommon.DataStructures;
using DemoMicroServiceServer.Web.Modules;

namespace DemoMicroServiceServer.Web {

    /// <summary>
    /// Configure the HAL links and resources.
    /// </summary>
    ///
    public static class Configure {

        /// <summary>
        /// Map out the resources used by the interface.
        /// </summary>
        /// <returns>A ResourceConfiguration object.</returns>
        /// 
        public static ResourceConfiguration ResourceConfiguration() {

            var config = new ResourceConfiguration {
                Resources = new List<Resource> {
                    new Resource {
                        Href = "/dinosaurs",
                        Authenticate = true,
                        Mappings = new List<Mapping> {
                            new Mapping { Rel = "create", Action = "create" },
                            new Mapping { Rel = "paged", Action = "paged" },
                            new Mapping { Rel = "self", Action = "self" }
                        },
                        Actions = new List<XAS.Rest.Server.Model.Action> {
                            new XAS.Rest.Server.Model.Action {
                                Name = "create",
                                Method = "POST",
                                Type = "application/json",
                                Accept = "application/hal+json",
                                Properties = new List<Property> {
                                    new Property { Id = "name", Name = "name", Type = "text" },
                                    new Property { Id = "heightInFeet", Name = "heightInFeet", Type = "number", Default = "1"},
                                    new Property { Id = "status", Name = "status", Type = "text" }
                                }
                            },
                            new XAS.Rest.Server.Model.Action {
                                Name = "paged",
                                Method = "GET",
                                Accept = "application/hal+json",
                                Parameters = new List<Parameter> {
                                    new Parameter { Name = "page", Type = "number", Default = "1" },
                                    new Parameter { Name = "pageSize", Type = "number", Default = "25", },
                                    new Parameter { Name = "keywords", Type = "list" , Possible = "Name,HeightInFeet,Status" },
                                    new Parameter { Name = "sortBy", Type = "list", Possible = "Name,HeightInFeet,Status" },
                                    new Parameter { Name = "sortDir", Type = "string", Default = "asc", Possible = "asc,desc" }
                                },
                                Fields = new List<Field> {
                                    new Field { Name = "name", Type = "text" },
                                    new Field { Name = "heightInFeet", Type = "number" },
                                    new Field { Name = "status", Type = "text" }
                                }
                            },
                            new XAS.Rest.Server.Model.Action {
                                Name = "self",
                                Method = "GET",
                                Accept = "application/hal+json",
                                Parameters = new List<Parameter> {
                                    new Parameter { Name = "id", Type = "number" }
                                },
                                Fields = new List<Field> {
                                    new Field { Name = "name", Type = "text" },
                                    new Field { Name = "heightInFeet", Type = "number" },
                                    new Field { Name = "status", Type = "text" }
                                }
                            }
                        }
                    },
                    new Resource {
                        Href = "/dinosaurs/{id}",
                        Authenticate = true,
                        Templates = new List<Template> {
                            new Template { Name = "id", Type = "number" }
                        },
                        Mappings = new List<Mapping> {
                            new Mapping { Rel = "delete", Action = "delete" },
                            new Mapping { Rel = "update", Action = "update" },
                            new Mapping { Rel = "self", Action = "self" }
                        },
                        Actions = new List<XAS.Rest.Server.Model.Action> {
                            new XAS.Rest.Server.Model.Action {
                                Name = "delete",
                                Method = "DELETE",
                                Accept = "application/hal+json",
                                Parameters = new List<Parameter> {
                                    new Parameter { Name = "id", Type = "number" }
                                }
                            },
                            new XAS.Rest.Server.Model.Action {
                                Name = "self",
                                Method = "GET",
                                Accept = "application/hal+json",
                                Parameters = new List<Parameter> {
                                    new Parameter { Name = "id", Type = "number" }
                                },
                                Fields = new List<Field> {
                                    new Field { Name = "name", Type = "text" },
                                    new Field { Name = "heightInFeet", Type = "number" },
                                    new Field { Name = "status", Type = "text" }
                                }
                            },
                            new XAS.Rest.Server.Model.Action {
                                Name = "update",
                                Method = "PUT",
                                Type = "application/json",
                                Accept = "application/hal+json",
                                Parameters = new List<Parameter> {
                                    new Parameter { Name = "id", Type = "number" }
                                },
                                Properties = new List<Property> {
                                    new Property { Id = "name", Name = "name", Type = "text" },
                                    new Property { Id = "heightInFeet", Name = "heightInFeet", Type = "number", Default = "1"},
                                    new Property { Id = "status", Name = "status", Type = "text" }
                                }
                            }
                        }
                    }
                }
            };

            return config;

        }

        /// <summary>
        /// Configure the HAL links.
        /// </summary>
        /// <returns>A HalConfiguration object.</returns>
        /// 
        public static HalConfiguration HypermediaConfiguration() {

            var config = new HalConfiguration();

            config.For<ResourceConfiguration>()
                .Links(RootModule.Self.CreateLink("self"))
                .Links(DinosaurModule.Dinosaurs.CreateLink("dinosaurs"))
            ;

            config.For<DinosaurDTO>()
                .Links((model) => DinosaurModule.Self.CreateLink("self", new {
                    id = model.Id
                }))
                .Links((model) => DinosaurModule.Update.CreateLink("update", new {
                    id = model.Id
                }))
                .Links((model) => DinosaurModule.Remove.CreateLink("delete", new {
                    id = model.Id
                }))
            ;

            config.For<PagedList<DinosaurDTO>>()
                .Links(RootModule.Root.CreateLink("root"))
                .Links(DinosaurModule.Create.CreateLink("create"))
                .Embeds("dinosaurs", (x) => x.Data)
                .Links(
                    (model, ctx) => DinosaurModule.Paged.CreateLink(
                        "self",
                        new {
                            page = model.PageNumber,
                            pageSize = model.PageSize,
                            sortBy = (model.SortedBy != null) ? string.Join(",", model.SortedBy) : "",
                            sortDir = string.Join("", model.SortedDir.Select(kvp => (kvp.Value == ListSortDirection.Ascending) ? "asc" : "desc"))
                        }
                    )
                )
                .Links(
                    (model, ctx) => DinosaurModule.Paged.CreateLink(
                        "prev",
                        new {
                            page = model.PageNumber - 1,
                            pageSize = model.PageSize,
                            sortBy = (model.SortedBy != null) ? string.Join(",", model.SortedBy) : "",
                            sortDir = string.Join("", model.SortedDir.Select(kvp => (kvp.Value == ListSortDirection.Ascending) ? "asc" : "desc"))
                        }
                    ),
                    (model, ctx) => model.PageNumber > 1
                )
                .Links(
                    (model, ctx) => DinosaurModule.Paged.CreateLink(
                        "next",
                        new {
                            page = model.PageNumber + 1,
                            pageSize = model.PageSize,
                            sortBy = (model.SortedBy != null) ? string.Join(",", model.SortedBy) : "",
                            sortDir = string.Join("", model.SortedDir.Select(kvp => (kvp.Value == ListSortDirection.Ascending) ? "asc" : "desc"))
                        }
                    ),
                    (model, ctx) => model.PageNumber < model.TotalPages
                )
            ;

            return config;

        }

    }

}
