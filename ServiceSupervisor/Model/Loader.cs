using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using XAS.Core.Extensions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

using ServiceSupervisor.Model.Schema;
using ServiceSupervisor.Configuration.Extensions;

namespace ServiceSupervisor.Model {

    public static class Loader {

        public static ConcurrentBag<SupervisedProcess> Database(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;
            var database = new ConcurrentBag<SupervisedProcess>();
            string domain = config.GetValue(section.Environment(), key.Domain());
            string workingDirectory = config.GetValue(section.Environment(), key.BinDir());

            var sections = config.GetSections();

            foreach (var process in sections) {

                if ((process != section.Application()) && (process != section.Web())) {

                    var items = config.GetSectionKeys(process);
                    var record = new SupervisedProcess { Name = process };

                    foreach (var item in items) {

                        if (item == key.SupervisorUsername()) {

                            record.Config.Username = config.GetValue(process, key.SupervisorUsername(), "");

                        } else if (item == key.SupervisorVerb()) {

                            record.Config.Verb = config.GetValue(process, key.SupervisorVerb(), "");

                        } else if (item == key.SupervisorDomain()) {

                            record.Config.Domain = config.GetValue(process, key.SupervisorDomain(), domain);

                        } else if (item == key.SupervisorPassword()) {

                            record.Config.Password = config.GetValue(process, key.SupervisorPassword(), "");

                        } else if (item == key.SupervisorAutoRestart()) {

                            record.Config.AutoRestart = config.GetValue(process, key.SupervisorAutoRestart(), "false").ToBoolean();

                        } else if (item == key.SupervisorExitRetries()) {

                            record.Config.ExitRetries = config.GetValue(process, key.SupervisorExitRetries(), "5").ToInt32();

                        } else if (item == key.SupervisorExitCodes()) {

                            var exitCodes = config.GetValue(process, key.SupervisorExitCodes(), "0,1");
                            record.Config.ExitCodes = exitCodes.ToInt32List();

                        } else if (item == key.SupervisorWorkingDirectory()) {

                            record.Config.WorkingDirectory = config.GetValue(process, key.SupervisorWorkingDirectory(), workingDirectory);

                        } else if (item == key.SupervisorEnvironment()) {

                            var env = config.GetValue(process, key.SupervisorEnvironment());
                            record.Config.Environment = env.ToKeyValuePairs();

                        }

                    }

                    database.Add(record);

                }

            }

            return database;

        }

    }

}
