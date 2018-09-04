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
        /// Run a command as Administrator.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="args">The arguments for the command.</param>
        /// <param name="stdout">The output from stdout.</param>
        /// <param name="stderr">The output from stderrt.</param>
        /// <returns></returns>
        /// 
        public Int32 RunAs(String command, String args, out List<string> stdout, out List<string> stderr) {

            var process = new System.Diagnostics.Process();
            List<string> output = new List<string>();
            List<string> error = new List<string>();

            process.StartInfo = new ProcessStartInfo() {
                Verb = "runas",
                FileName = command,
                Arguments = args,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new DataReceivedEventHandler(

                delegate (object sender, DataReceivedEventArgs e) {

                    if (!String.IsNullOrEmpty(e.Data)) {

                        output.Add(e.Data.Trim());

                    }

                }

            );

            process.ErrorDataReceived += new DataReceivedEventHandler(

                delegate (object sender, DataReceivedEventArgs e) {

                    if (!String.IsNullOrEmpty(e.Data)) {

                        error.Add(e.Data.Trim());

                    }

                }

            );

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            process.CancelOutputRead();
            process.CancelErrorRead();

            stdout = output;
            stderr = error;

            return process.ExitCode;

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
