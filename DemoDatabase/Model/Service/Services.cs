using System;
using System.Collections.Generic;

using DemoDatabase.Model.Database;

namespace DemoDatabase.Model.Service {

    public static class Services {

        public static ServerDTO GetSite(Repositories repo, String site) {

            ServerDTO dto = null;
            Servers server = null;

            var serverKey = repo.ServersAttributes.Find(r =>
                (r.Attribute.Type == "Site" && r.Attribute.Name == site)
            ).ServerKey;

            server = repo.Servers.Find(r => (r.Id == serverKey));
            dto = NewServerDTO(repo, server);

            return dto;
                        
        }

        public static ServerDTO GetServer(Repositories repo, String name) {

            ServerDTO dto = null;
            Servers server = null;

            if ((server = repo.Servers.Find(r => (r.Name == name))) != null) {

                dto = NewServerDTO(repo, server);

            }

            return dto;
        
        }
        
        public static List<ServerDTO> GetServers(Repositories repo, String group, String target) {

            var dtos = new List<ServerDTO>();
            Int32 groupKey = repo.Groups.Find(r => (r.Name == group)).Id;
            Int32 targetKey = repo.Targets.Find(r => (r.Name == target)).Id;

            foreach (var t in repo.GroupsTargets.Search(r => (r.GroupKey == groupKey && r.TargetKey == targetKey))) {

                foreach (var s in repo.TargetsServers.Search(r => (r.TargetKey == t.TargetKey))) {

                    Servers server = null;

                    if ((server = repo.Servers.Find(r => (r.Id == s.ServerKey))) != null) {

                        dtos.Add(NewServerDTO(repo, server));

                    }

                }

            }
            
            return dtos;

        }

        public static List<ServerDTO> GetServers(Repositories repo, String group) {

            var dtos = new List<ServerDTO>();
            Int32 groupKey = repo.Groups.Find(r => (r.Name == group)).Id;

            foreach (var t in repo.GroupsTargets.Search(r => (r.GroupKey == groupKey))) {

                foreach (var s in repo.TargetsServers.Search(r => (r.TargetKey == t.TargetKey))) {

                    Servers server = null;

                    if ((server = repo.Servers.Find(r => (r.Id == s.ServerKey))) != null) {

                        dtos.Add(NewServerDTO(repo, server));

                    }

                }

            }

            return dtos;

        }

        public static List<GroupDTO> GetGroups(Repositories repo) {

            var dtos = new List<GroupDTO>();
            var groups = repo.Groups.Search();

            foreach (var group in groups) {

                var targets = new List<TargetDTO>();
                var groupsTargets = repo.GroupsTargets.Search(r => (r.GroupKey == group.Id));

                foreach (var target in groupsTargets) {

                    var t = repo.Targets.Find(r => (r.Id == target.TargetKey));

                    targets.Add(new TargetDTO {
                        Name = t.Name,
                        Id = t.Id
                    });

                }

                dtos.Add(new GroupDTO {
                    Name = group.Name,
                    Targets = targets
                });

            }

            return dtos;

        }

        public static Int32 AddAttribute(Repositories repo, String type, String name, String strValue, Int32 numValue) {
                
            Attributes attribute = new Attributes {
                Type = type,
                Name = name,
                NumValue = numValue,
                StrValue = strValue
            };

            repo.Attributes.Create(attribute);
            repo.Save();

            return attribute.Id;

        }

        public static Boolean DelAttribute(Repositories repo, Int32 id) {

            bool stat = false;

            foreach (var attribute in repo.ServersAttributes.Search(r => (r.AttributeKey == id))) {

                repo.ServersAttributes.Delete(attribute.Id);

            }

            repo.Attributes.Delete(id);
            repo.Save();

            return stat;

        }

        public static Boolean UpdAttribute(Repositories repo, Int32 id, String type, String name, String strValue, Int32 numValue) {

            bool stat = false;
            Attributes attribute = null;

            if ((attribute = repo.Attributes.Find(r => (r.Id == id))) != null) {

                stat = true;

                attribute.Type = type;
                attribute.Name = name;
                attribute.StrValue = strValue;
                attribute.NumValue = numValue;

                repo.Attributes.Update(attribute);
                repo.Save();

            }

            return stat;

        }

        public static Int32 AddServer(Repositories repo, String name) {

            Servers server = new Servers { 
                Name = name
            };

            repo.Servers.Create(server);
            repo.Save();

            return server.Id;

        }

        public static Boolean DelServer(Repositories repo, Int32 id) {

            bool stat = true;

            foreach (var target in repo.TargetsServers.Search(r => (r.ServerKey == id))) {

                repo.TargetsServers.Delete(target.Id);

            }
            repo.Servers.Delete(id);
            repo.Save();

            return stat;

        }

        public static Boolean UpdServer(Repositories repo, Int32 id, String name) {

            bool stat = false;
            Servers server = null;

            if ((server = repo.Servers.Find(r => (r.Id == id))) != null) {

                stat = true;
                server.Name = name;

                repo.Servers.Update(server);
                repo.Save();

            }

            return stat;

        }

        public static Int32 AddGroup(Repositories repo, String name) {

            Groups group = new Groups {
                Name = name
            };

            repo.Groups.Create(group);
            repo.Save();

            return group.Id;

        }

        public static Boolean DelGroup(Repositories repo, Int32 id) {

            bool stat = true;

            foreach (var group in repo.GroupsTargets.Search(r => (r.GroupKey == id))) {

                repo.GroupsTargets.Delete(group.Id);

            }
            
            repo.Groups.Delete(id);
            repo.Save();

            return stat;

        }

        public static Boolean UpdGroup(Repositories repo, Int32 id, String name) {

            bool stat = false;
            Groups group = null;

            if ((group = repo.Groups.Find(r => (r.Id == id))) != null) {

                stat = true;
                group.Name = name;

                repo.Groups.Update(group);
                repo.Save();

            }

            return stat;

        }

        public static Int32 AddTarget(Repositories repo, String name) {

            Targets target = new Targets {
                Name = name
            };

            repo.Targets.Create(target);
            repo.Save();

            return target.Id;

        }

        public static Boolean DelTarget(Repositories repo, Int32 id) {

            bool stat = true;

            foreach (var group in repo.GroupsTargets.Search(r => (r.TargetKey == id))) {

                repo.GroupsTargets.Delete(group.Id);

            }

            foreach (var target in repo.TargetsServers.Search(r => (r.TargetKey == id))) {

                repo.TargetsServers.Delete(target.Id);

            }

            repo.Targets.Delete(id);
            repo.Save();

            return stat;

        }

        public static Boolean UpdTarget(Repositories repo, Int32 id, String name) {

            bool stat = false;
            Targets target = null;

            if ((target = repo.Targets.Find(r => (r.Id == id))) != null) {

                stat = true;
                target.Name = name;

                repo.Targets.Update(target);
                repo.Save();

            }

            return stat;

        }

        public static Boolean AddAttributeToServer(Repositories repo, Int32 serverKey, Int32 attributeKey) {

            bool stat = true;
            ServersAttributes serversAttributes = new ServersAttributes { 
                AttributeKey = attributeKey,
                ServerKey =serverKey
            };

            repo.ServersAttributes.Create(serversAttributes);
            repo.Save();

            return stat;

        }

        public static Boolean DelAttributeFromServer(Repositories repo, Int32 serverKey, Int32 attributeKey) {

            bool stat = true;
            ServersAttributes serversAttributes = null;

            if ((serversAttributes = repo.ServersAttributes.Find(r =>
                ((r.ServerKey == serverKey) && (r.AttributeKey == attributeKey)))) != null) {

                stat = true;

                repo.ServersAttributes.Delete(serversAttributes.Id);
                repo.Save();

            }

            return stat;

        }

        public static Boolean AddServerToTarget(Repositories repo, Int32 serverKey, Int32 targetKey) {

            bool stat = true;
            TargetsServers targetsServers = new TargetsServers { 
                ServerKey = serverKey,
                TargetKey = targetKey
            };

            repo.TargetsServers.Create(targetsServers);
            repo.Save();

            return stat;

        }

        public static Boolean DelServerFromTarget(Repositories repo, Int32 serverKey, Int32 targetKey) {

            bool stat = false;
            TargetsServers targetsServers = null;

            if ((targetsServers = repo.TargetsServers.Find(r => 
                ((r.ServerKey == serverKey) && (r.TargetKey == targetKey)))) != null) {

                stat = true;

                repo.TargetsServers.Delete(targetsServers.Id);
                repo.Save();

            }

            return stat;

        }

        public static Boolean AddTargetToGroup(Repositories repo, Int32 targetKey, Int32 groupKey) {

            bool stat = true;
            GroupsTargets groupsTargets = new GroupsTargets {
                GroupKey = groupKey,
                TargetKey = targetKey
            };

            repo.GroupsTargets.Create(groupsTargets);
            repo.Save();

            return stat;

        }

        public static Boolean DelTargetFromGroup(Repositories repo, Int32 targetKey, Int32 groupKey) {

            bool stat = false;
            GroupsTargets groupsTargets = null;

            if ((groupsTargets = repo.GroupsTargets.Find(r =>
                ((r.GroupKey == groupKey) && (r.TargetKey == targetKey)))) != null) {

                stat = true;

                repo.GroupsTargets.Delete(groupsTargets.Id);
                repo.Save();

            }

            return stat;

        }


        private static ServerDTO NewServerDTO(Repositories repo, Servers server) {

            return new ServerDTO {
                Name = server.Name,
                Id = server.Id,
                Attributes = GetAttributes(repo, server.Id)
            };

        }
        
        private static List<AttributeDTO> GetAttributes(Repositories repo, int key) {

            var attributes = new List<AttributeDTO>();

            foreach (var a in repo.ServersAttributes.Search(r => r.ServerKey == key)) {

                Attributes attribute = null;

                if ((attribute = repo.Attributes.Find(r => r.Id == a.AttributeKey)) != null) {

                    attributes.Add(new AttributeDTO {
                        Id = attribute.Id,
                        Type = attribute.Type,
                        Name = attribute.Name,
                        StrValue = attribute.StrValue,
                        NumValue = attribute.NumValue
                    });

                }

            }

            return attributes;

        }

    }

}
