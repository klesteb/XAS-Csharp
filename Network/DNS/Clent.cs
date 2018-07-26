using System;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.Network.DNS {

    /// <summary>
    /// A class to interact with DNS servers.
    /// </summary>
    /// 
    public class Client {

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public Client(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.log = logFactory.Create(typeof(Client));

        }

        /// <summary>
        /// Sets the DNS server to query against.
        /// </summary>
        /// <param name="server">A DNS server to query.</param>
        /// <returns>The old DNS server.</returns>
        /// 
        public String SetDNSServer(String server) {

            var key = config.Key;
            var section = config.Section;

            if (String.IsNullOrEmpty(server)) {

                server = config.GetValue(section.Environment(), key.DnsDomain());

            }

            String dnsServer = Resolver.GetDnsServer();
            Resolver.SetDnsServer(server);

            return dnsServer;

        }

        /// <summary>
        /// Checks wither a server response on a port number.
        /// </summary>
        /// <param name="server">A server name.</param>
        /// <param name="port">The port to attach too.</param>
        /// <param name="timeout">A timeout for the connection attempt.</param>
        /// <returns>true if successful.</returns>
        /// 
        public Boolean IsAvailable(String server, Int32 port, Int32 timeout) {

            return Resolver.IsAvailable(server, port, timeout);

        }

        /// <summary>
        /// Find the SRV records for a particular query.
        /// </summary>
        /// <param name="query">A query to process.</param>
        /// <returns>A list of SrvDTO objects.</returns>
        /// 
        public List<SrvDTO> FindSRVRecords(String query) {

            return Resolver.GetSRVRecords(query);

        }

    }

}
