using System;
using System.Collections.Generic;

using XAS.Core.Extensions;

namespace ServiceSpooler.Extensions {

    /// <summary>
    /// String extensions.
    /// </summary>
    /// 
    public static class StringExtensions {

        /// <summary>
        /// A formatted string.
        /// </summary>
        /// <param name="buffer">A formatted string.</param>
        /// <returns>A list of Int32 objects.</returns>
        /// <remarks>
        /// The string needs to be in this format:
        ///     0,1,2
        /// </remarks>
        public static List<Int32> ParseExitCodes(this String buffer) {

            var exitCodes = new List<Int32>();

            if (buffer != "") {

                String[] codes = buffer.Split(',');

                foreach (string code in codes) {

                    exitCodes.Add(code.ToInt32());

                }

            }

            return exitCodes;

        }

        /// <summary>
        /// Parse a Environment string.
        /// </summary>
        /// <param name="buffer">A formatted string.</param>
        /// <returns>A Dictionary of key/value pairs.</returns>
        /// <remarks>
        /// The string needs to be in this format:
        ///     key=value;key=value;
        /// </remarks>
        /// 
        public static Dictionary<String, String> ParseEnvironment(this String buffer) {

            var environment = new Dictionary<String, String>();

            if (buffer != "") {

                String[] chunks = buffer.Split(';');

                foreach (string chunk in chunks) {

                    String[] parts = chunk.Split('=');

                    environment.Add(parts[0].Trim(), parts[1].Trim());

                }

            }

            return environment;

        }

    }

}
