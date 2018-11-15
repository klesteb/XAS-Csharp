using System;
using System.Collections.Generic;

using XAS.Core.Extensions;

namespace ServiceSupervisor {

    public static class Utils {

        public static List<Int32> ParseExitCodes(String buffer) {

            var exitCodes = new List<Int32>();

            if (buffer != "") {

                String[] codes = buffer.Split(',');

                foreach (string code in codes) {

                    exitCodes.Add(code.ToInt32());

                }

            }

            return exitCodes;

        }

        public static Dictionary<String, String> ParseEnvironment(String buffer) {

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
