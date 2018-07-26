using System;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;
using XAS.Core.Logging;
using XAS.Core.Spooling;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

namespace XAS.Core.Alerting {

    /// <summary>
    /// A class to send "alerts" to a centralized management platform.
    /// </summary>
    ///
    public class Alert: IAlerting {

        private readonly ILogger log = null;
        private readonly ISpooler spooler = null;
        private readonly IConfiguration config = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Alert(IConfiguration config, ILoggerFactory logFactory, ISpooler spooler) {

            this.config = config;
            this.spooler = spooler;

            log = logFactory.Create(typeof(Alert));

        }

        /// <summary>
        /// Send an alert.
        /// </summary>
        /// <param name="message">The alert to send.</param>
        /// 
        public void Send(String message) {

            byte[] output;
            string data;
            string format = "[{0}] - {1}";

            var key = config.Key;
            DateTime dt = DateTime.Now;
            var section = config.Section;
            string dateTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
            string timeStamp = dt.ToISO8601();

            Dictionary<string, string> json = new Dictionary<string, string>();

            log.Trace("Entering Send()");

            // using logstash formatted json

            json.Add("@timestamp", timeStamp);
            json.Add("@version", "1");
            json.Add("@message", String.Format(format, dateTime, message));
            json.Add("type", "xas-alerts");
            json.Add("message", message);
            json.Add("hostname", config.GetValue(section.Environment(), key.Host()));
            json.Add("priority", config.GetValue(section.Environment(), key.Priority()));
            json.Add("facility", config.GetValue(section.Environment(), key.Facility()));
            json.Add("process", config.GetValue(section.Environment(), key.Script()));
            json.Add("tid", "main");
            json.Add("pid", config.GetValue(section.Environment(), key.Pid()));

            data = JsonConvert.SerializeObject(json);
            output = Encoding.UTF8.GetBytes(data);

            spooler.Write(output);

            log.Trace("Leaving Send()");

        }

    }

}
