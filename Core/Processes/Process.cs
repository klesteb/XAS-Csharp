using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace XAS.Core.Processes {

    public static class Process {

        /// <summary>
        /// Run a command and capture the output.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="stdout">The output from stdout.</param>
        /// <param name="stderr">The output from stderr.</param>
        /// <returns>The exit code returned from the command.</returns>
        /// 
        public static Int32 Run(String command, out List<String> stdout, out List<String> stderr) {

            List<string> output = new List<string>();
            List<string> error = new List<string>();
            var process = new System.Diagnostics.Process();

            string[] args = Utils.ParseCommandLine(command);
            int length = args.Length - 1;

            process.StartInfo = new ProcessStartInfo() {
                FileName = args[0],
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = String.Join(" ", args.Skip(1).Take(length).ToArray()),
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
        /// Run a command as Administrator and capture the output.
        /// </summary>
        /// <param name="command">The command to run.</param>
        /// <param name="stdout">The output from stdout.</param>
        /// <param name="stderr">The output from stderr.</param>
        /// <returns>The exit code returned from the command.</returns>
        /// <remarks>
        /// This only works if you have Administrator rights.
        /// </remarks>
        /// 
        public static Int32 RunAs(String command, out List<String> stdout, out List<String> stderr) {

            List<string> output = new List<string>();
            List<string> error = new List<string>();
            var process = new System.Diagnostics.Process();

            string[] args = Utils.ParseCommandLine(command);
            int length = args.Length - 1;

            process.StartInfo = new ProcessStartInfo() {
                Verb = "runas",
                FileName = args[0],
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = String.Join(" ", args.Skip(1).Take(length).ToArray()),
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

    }

}
