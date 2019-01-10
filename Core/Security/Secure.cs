using System;
using System.Linq;
using System.Security;
using System.Diagnostics;
using System.Security.Principal;
using System.Collections.Generic;

using XAS.Core.Exceptions;

namespace XAS.Core.Security {

    /// <summary>
    /// Utilities for Security.
    /// </summary>
    /// 
    public class Secure: ISecurity {

        /// <summary>
        /// Checks to see if the process is running with elevated privileges, i.e. Administrator.
        /// </summary>
        /// 
        public Boolean IsElevated {

            get {

                WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            }

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public Secure() { }
                
        /// <summary>
        /// Rerun the current process as "Administrator".
        /// </summary>
        /// 
        public void RunElevated() {

            string[] args = Environment.GetCommandLineArgs();
            int length = args.Length - 1;
            var process = new System.Diagnostics.Process();

            process.StartInfo = new ProcessStartInfo() {
                Verb = "runas",
                FileName = args[0],
                Arguments = String.Join(" ", args.Skip(1).Take(length).ToArray()),
                WindowStyle = ProcessWindowStyle.Hidden,
                // UseShellExecute = false,
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
                // RedirectStandardInput = true
            };

            process.Start();
            process.WaitForExit();

        }

        /// <summary>
        /// Make a Secure String out of an insecure string.
        /// </summary>
        /// <param name="inSecureString">A regular string.</param>
        /// <returns>A SecureString object.</returns>
        /// 
        public SecureString MakeSecureString(string inSecureString) {

            System.Security.SecureString secureString = new System.Security.SecureString();

            for (int x = 0; x < inSecureString.Length; x++) {

                secureString.AppendChar(inSecureString[x]);

            }

            return secureString;

        }

    }

}
