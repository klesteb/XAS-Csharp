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

        public static ConcurrentBag<Supervise> Database(IConfiguration config) {

            var key = config.Key;
            var section = config.Section;
            var database = new ConcurrentBag<Supervise>();

            var sections = config.GetSections();

            foreach (var process in sections) {

                if ((process != section.Application()) && (process != section.Web())) {

                    var items = config.GetSectionKeys(process);
                    var record = new Supervise {
                        Name = process,
                        Verb = "RunAs",
                        Domain = "",
                        Username = "",
                        Password = "",
                        RetryCount = 0,
                        Status = RunStatus.Stopped,
                        AutoStart = false,
                        AutoRestart = false,
                        ExitRetries = 5,
                        ExitCodes = new List<Int32> { 0, 1 },
                        WorkingDirectory = config.GetValue(section.Environment(), key.BinDir()),
                        Environment = new Dictionary<String, String>()
                    };

                    foreach (var item in items) {

                        if (item == key.SupervisorUsername()) {

                            record.Username = config.GetValue(process, key.SupervisorUsername());

                        } else if (item == key.SupervisorVerb()) {

                            record.Verb = config.GetValue(process, key.SupervisorVerb());

                        } else if (item == key.SupervisorDomain()) {

                            record.Domain = config.GetValue(process, key.SupervisorDomain());

                        } else if (item == key.SupervisorPassword()) {

                            record.Password = config.GetValue(process, key.SupervisorPassword());

                        } else if (item == key.SupervisorAutoStart()) {

                            record.AutoStart = config.GetValue(process, key.SupervisorAutoStart()).ToBoolean();

                        } else if (item == key.SupervisorAutoRestart()) {

                            record.AutoRestart = config.GetValue(process, key.SupervisorAutoRestart()).ToBoolean();

                        } else if (item == key.SupervisorExitRetries()) {

                            record.ExitRetries = config.GetValue(process, key.SupervisorExitRetries()).ToInt32();

                        } else if (item == key.SupervisorExitCodes()) {

                            var exitCodes = config.GetValue(process, key.SupervisorExitCodes());
                            record.ExitCodes = Utils.ParseExitCodes(exitCodes);

                        } else if (item == key.SupervisorWorkingDirectory()) {

                            record.WorkingDirectory = config.GetValue(process, key.SupervisorWorkingDirectory());

                        } else if (item == key.SupervisorEnvironment()) {

                            var env = config.GetValue(process, key.SupervisorEnvironment());
                            record.Environment = Utils.ParseEnvironment(env);

                        }

                    }

                    database.Add(record);

                }

            }

            return database;

        }

    }

}
