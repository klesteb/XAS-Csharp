using System;
using System.Collections.Generic;

using TaskScheduler;

using XAS.Model;
using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

using DemoDatabase.Model.Database;

namespace DemoDatabase.Model {

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

            var authentication = new List<Authentication> {
                new Authentication { Domain = "WISE", Username = "relmgr", Password = "password" },
                new Authentication { Domain = "SKY", Username = "relmgr", Password = "password" },
                new Authentication { Domain = "", Username = "wsipc", Password = "password" },
                new Authentication { Domain = "", Username = "partnerload" , Password = "password" }
            };

            repo.Authentication.Populate(authentication);
            repo.Save();

            log.Info("Populated the Authentication table");

            var attributes = new List<Attributes> {
                new Attributes {
                  Type = "Schedule",
                  Name = "Schedulable",
                  NumValue = Convert.ToInt32(true)
                },
                new Attributes {
                  Type = "Schedule",
                  Name = "NonSchedulable",
                  NumValue = Convert.ToInt32(false)
                },
                new Attributes {
                    Type = "Site",
                    Name = "qmlativ"
                },
                new Attributes {
                    Type = "Site",
                    Name = "partnerload"
                },
                new Attributes { 
                    Type = "DataConnection", 
                    Name = "AutoActive", 
                    NumValue =  (Int32)FluentFTP.FtpDataConnectionType.AutoActive 
               },
               new Attributes { 
                   Type = "DataConnection", 
                   Name = "AutoPassive", 
                   NumValue = (Int32)FluentFTP.FtpDataConnectionType.AutoPassive 
               },
               new Attributes { 
                    Type = "DataConnection", 
                    Name = "EPRT", 
                    NumValue = (Int32)FluentFTP.FtpDataConnectionType.EPRT 
               },
                new Attributes {
                    Type = "DataConnection",
                    Name = "EPSV",
                    NumValue = (Int32)FluentFTP.FtpDataConnectionType.EPSV
                },
                new Attributes {
                    Type = "DataConnection",
                    Name = "PASV",
                    NumValue = (Int32)FluentFTP.FtpDataConnectionType.PASV
                },
                new Attributes {
                    Type = "DataConnection",
                    Name = "PASVEX",
                    NumValue = (Int32)FluentFTP.FtpDataConnectionType.PASVEX
                },
                new Attributes {
                    Type = "DataConnection",
                    Name = "PORT",
                    NumValue = (Int32)FluentFTP.FtpDataConnectionType.PORT
                },
                new Attributes {
                    Type = "EncryptionMode",
                    Name = "None",
                    NumValue = (Int32)FluentFTP.FtpEncryptionMode.None
                },
                new Attributes {
                    Type = "EncryptionMode",
                    Name = "Explicit",
                    NumValue =  (Int32)FluentFTP.FtpEncryptionMode.Explicit
                },
                new Attributes {
                    Type = "EncryptionMode",
                    Name = "Implicit",
                    NumValue =  (Int32)FluentFTP.FtpEncryptionMode.Implicit
                },
                new Attributes {
                    Type = "RunLevels",
                    Name = "LUA",
                    NumValue = (Int32)_TASK_RUNLEVEL.TASK_RUNLEVEL_LUA
                },
                new Attributes {
                    Type = "RunLevels",
                    Name = "Highest",
                    NumValue = (Int32)_TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST
                },
                new Attributes {
                    Type = "LogonTypes",
                    Name = "None",
                    NumValue = (Int32)_TASK_LOGON_TYPE.TASK_LOGON_NONE
                },
                new Attributes {
                    Type = "LogonTypes",
                    Name = "S4U",
                    NumValue = (Int32)_TASK_LOGON_TYPE.TASK_LOGON_S4U
                },
                new Attributes {
                    Type = "LogonTypes",
                    Name = "InteractiveToken",
                    NumValue = (Int32)_TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN
                },
                new Attributes {
                    Type = "LogonTypes",
                    Name = "Group",
                    NumValue = (Int32)_TASK_LOGON_TYPE.TASK_LOGON_GROUP
                },
                new Attributes {
                    Type = "LogonTypes",
                    Name = "ServiceAccount",
                    NumValue = (Int32)_TASK_LOGON_TYPE.TASK_LOGON_SERVICE_ACCOUNT
                },
                new Attributes {
                    Type = "LogonTypes",
                    Name = "InteractiveTokenOrPassword",
                    NumValue = (Int32)_TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN_OR_PASSWORD
                },
                new Attributes {
                    Type = "SSL",
                    Name = "Secure",
                    NumValue = Convert.ToInt32(true)
                },
                new Attributes {
                    Type = "SSL",
                    Name = "VerifyPeer",
                    NumValue = Convert.ToInt32(false)
                },
                new Attributes {
                    Type = "FTPS",
                    Name = "qmlativ",
                    NumValue = 21
                },
                new Attributes {
                    Type = "FTPS",
                    Name = "partnerload",
                    NumValue = 21
                },
                new Attributes {
                    Type = "source",
                    Name = "qmlativ",
                    StrValue = "/wsipc/rma",
                },
                new Attributes {
                    Type = "destination",
                    Name = "qmlativ",
                    StrValue = "D:\\source-code\\"
                },
                new Attributes {
                    Type = "source",
                    Name = "partnerload",
                    StrValue = "/partnerload",
                },
                new Attributes {
                    Type = "destination",
                    Name = "partnerload",
                    StrValue = "D:\\source-code\\"
                },

            };

            repo.Attributes.Populate(attributes);
            repo.Save();

            log.Info("Populated the Attributes table");

            var authenticationKey = repo.Authentication.Find(r => (r.Domain == "WISE" && r.Username == "relmgr")).Id;

            List<Servers> servers = new List<Servers> {
                new Servers {
                    Name = "wsipc-rm-11",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "final-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "ftp.skyward.com",
                    Authentication = repo.Authentication.Find(r => (r.Username == "wsipc")),
                },
                new Servers {
                    Name = "ftp.skyward.com",
                    Authentication = repo.Authentication.Find(r => (r.Username == "partnerload")),
                },
                new Servers {
                    Name = "bethel-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "cpark-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "egreen-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "egreen-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd101-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd101-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd105-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd105-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd112-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd112-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd113-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd113-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd114-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd114-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd171-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd171-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd189-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "esd189-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "frankl-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "kent-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-sfagt-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-16",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-13",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-14",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-11",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "yakima-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "saas-code-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "denali-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "est-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "lkwash-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "saas-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-12",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-17",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-15",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-19",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-18",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-20",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "wsipc-cas-21",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "mst-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "mst-cas-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "akst-cas-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "est-cas-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "tla-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "tla-cas-11",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "tla-prog-02",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "tlapre-cas-11",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "tlapre-cas-12",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "tlapre-cas-13",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "prdcopy-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "sysnsr1-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "sysnsr2-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "finnsr-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "system-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "beta-prog-01",
                    AuthenticationKey = authenticationKey,
                },
                new Servers {
                    Name = "dmt-prog-01",
                    AuthenticationKey = authenticationKey,
                }
            };

            repo.Servers.Populate(servers);
            repo.Save();

            log.Info("Populated the Servers table");

            var targets = new List<Targets> {
                new Targets { Name = "dmt" },
                new Targets { Name = "tla" },
                new Targets { Name = "preview" },
                new Targets { Name = "dev" },
                new Targets { Name = "final" },
                new Targets { Name = "system" },
                new Targets { Name = "beta" },
                new Targets { Name = "finnsr" },
                new Targets { Name = "sysnsr1" },
                new Targets { Name = "sysnsr2" },
                new Targets { Name = "prdcpy" },
                new Targets { Name = "est" },
                new Targets { Name = "mst" },
                new Targets { Name = "pst" },
                new Targets { Name = "akst" },
                new Targets { Name = "partnerload" },
                new Targets { Name = "qmlativ" }
            };

            repo.Targets.Populate(targets);
            repo.Save();

            log.Info("Populated the Targets table");

            var groups = new List<Groups> {
                new Groups { Name = "production" },
                new Groups { Name = "tla" },
                new Groups { Name = "dev" },
                new Groups { Name = "skyward" }
            };

            repo.Groups.Populate(groups);
            repo.Save();

            log.Info("Populated the Groups table");
            log.Info("Populating the join tables");

            var groupsTargets = new List<GroupsTargets> {
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "skyward").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "qmlativ").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "skyward").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "partnerload").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "tla").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "tla").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "tla").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "preview").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "dev").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "final").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "dev").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "system").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "dev").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "beta").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "production").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "tla").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "production").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "preview").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "production").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "est").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "production").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "mst").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "production").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "akst").Id
                },
                new GroupsTargets {
                    GroupKey = repo.Groups.Find(r => r.Name == "production").Id,
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id
                }
            };

            repo.GroupsTargets.Populate(groupsTargets);
            repo.Save();

            log.Info("Populated the GroupsTargets join table");

            var siteQmlativ = repo.Servers.Find(r => (r.Name == "ftp.skyward.com" && r.Authentication.Username == "wsipc")).Id;
            var sitePartnerload = repo.Servers.Find(r => (r.Name == "ftp.skyward.com" && r.Authentication.Username == "partnerload")).Id;

            var targetsServers = new List<TargetsServers> {
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "tla").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "tla-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "tla").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "tla-cas-11").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "preview").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "tla-prog-02").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "preview").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "tlapre-cas-11").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "preview").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "tlapre-cas-12").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "preview").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "tlapre-cas-13").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "final").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "final-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "beta").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "beta-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "system").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "system-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "sysnsr1").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "sysnsr1-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "sysnsr2").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "sysnsr2-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "finnsr").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "finnsr-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "prdcpy").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "prdcopy-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "dmt").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "dmt-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "est").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "est-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "mst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "mst-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "mst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "mst-cas-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "akst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "akst-cas-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "akst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "denali-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "bethel-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "cpark-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "egreen-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "egreen-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd101-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd101-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd105-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd105-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd112-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd112-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd113-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd113-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd114-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd114-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd171-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd171-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd189-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "esd189-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "frankl-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "kent-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-sfagt-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-16").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-13").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-14").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-11").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "yakima-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "saas-code-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "lkwash-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "saas-prog-01").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-12").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-17").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-15").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-19").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-18").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-20").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "pst").Id,
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-21").Id
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "qmlativ").Id,
                    ServerKey = siteQmlativ
                },
                new TargetsServers {
                    TargetKey = repo.Targets.Find(r => r.Name == "partnerload").Id,
                    ServerKey = sitePartnerload
                }

            };

            repo.TargetsServers.Populate(targetsServers);
            repo.Save();

            log.Info("Populated the TargetsServers join table");

            var serversAttrbutes = new List<ServersAttributes> {
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-rm-11").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "final-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "bethel-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "cpark-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "egreen-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "egreen-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd101-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd101-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd105-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd105-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd112-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd112-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd113-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd113-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd114-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd114-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd171-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd171-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd189-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "esd189-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "frankl-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "kent-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-sfagt-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-16").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-14").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-13").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-11").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "yakima-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "saas-code-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "denali-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "est-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "lkwash-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "saas-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-12").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-17").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-15").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-19").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-18").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-20").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "wsipc-cas-21").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "mst-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "mst-cas-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "akst-cas-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "est-cas-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "tla-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "tla-cas-11").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "tla-prog-02").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "tlapre-cas-11").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "tlapre-cas-12").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "tlapre-cas-13").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "prdcopy-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "sysnsr1-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "sysnsr2-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "finnsr-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "dmt-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "beta-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                },
                new ServersAttributes {
                    ServerKey = repo.Servers.Find(r => r.Name == "system-prog-01").Id,
                    AttributeKey = repo.Attributes.Find(r => r.Type == "Schedule" && r.Name == "Schedulable").Id
                }
            };

            repo.ServersAttributes.Populate(serversAttrbutes);
            repo.Save();

            serversAttrbutes = new List<ServersAttributes> {
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "Site" && r.Name == "qmlativ")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "FTPS" && r.Name == "qmlativ")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "source" && r.Name == "qmlativ")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "destination" && r.Name == "qmlativ")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "SSL" && r.Name == "Secure")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "SSL" && r.Name == "VerifyPeer")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "EncryptionMode" && r.Name == "Implicit")).Id
                },
                new ServersAttributes {
                    ServerKey = siteQmlativ,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "DataConnection" && r.Name == "AutoPassive")).Id
                }
            };

            repo.ServersAttributes.Populate(serversAttrbutes);
            repo.Save();

            serversAttrbutes = new List<ServersAttributes> {
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "Site" && r.Name == "partnerload")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "FTPS" && r.Name == "partnerload")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "source" && r.Name == "partnerload")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "destination" && r.Name == "partnerload")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "SSL" && r.Name == "Secure")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "SSL" && r.Name == "VerifyPeer")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "EncryptionMode" && r.Name == "Implicit")).Id
                },
                new ServersAttributes {
                    ServerKey = sitePartnerload,
                    AttributeKey = repo.Attributes.Find(r => (r.Type == "DataConnection" && r.Name == "AutoPassive")).Id
                }
            };

            repo.ServersAttributes.Populate(serversAttrbutes);
            repo.Save();

            log.Info("Populated the ServersAttributes join table");

        }

    }

}
