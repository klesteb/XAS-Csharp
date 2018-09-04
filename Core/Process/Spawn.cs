using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;


using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;

namespace XAS.Core.Process {

    /// <summary>
    /// Spawn a process.
    /// </summary>
    /// 
    public class Spawn {

        private Int32 exitCode = 0;

        private readonly ILogger log = null;
        private readonly ISecurity secure = null;
        private readonly SpawnInfo spawnInfo = null;
        private readonly IConfiguration config = null;
        private readonly IErrorHandler handler = null;
        private readonly System.Diagnostics.Process process = null;
        private readonly System.Diagnostics.ProcessStartInfo processInfo = null;

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="handler"></param>
        /// <param name="logFactory"></param>
        /// <param name="spawnInfo"></param>
        /// 
        public Spawn(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, SpawnInfo spawnInfo) {
 
            this.config = config;
            this.handler = handler;
            this.spawnInfo = spawnInfo;
            this.secure = new Secure();

            this.log = logFactory.Create(typeof(Spawn));

            // parse the command line

            string cmdLine = Environment.ExpandEnvironmentVariables(spawnInfo.Command);
            string[] args = ParseCommandLine(cmdLine);
            int length = args.Length - 1;

            // set up the process

            this.process = new System.Diagnostics.Process();
            this.processInfo = new System.Diagnostics.ProcessStartInfo() {
                FileName = args[0],
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = spawnInfo.WorkingDirectory,
                Arguments = String.Join(" ", args.Skip(1).Take(length).ToArray()),
            };

            // set user context

            if ((!String.IsNullOrEmpty(spawnInfo.Username) && (!String.IsNullOrEmpty(spawnInfo.Password)))) {

                processInfo.LoadUserProfile = true;
                processInfo.Domain = spawnInfo.Domain;
                processInfo.UserName = spawnInfo.Username;
                processInfo.Password = secure.MakeSecureString(spawnInfo.Password);

            }

            // add environment variables

            foreach (KeyValuePair<String, String> env in spawnInfo.Environment) {

                // replace existing environment variables

                if (processInfo.Environment.ContainsKey(env.Key)) {

                    processInfo.Environment.Remove(env.Key);

                }

                processInfo.Environment.Add(env.Key, env.Value);

            }

            process.EnableRaisingEvents = true;
            process.StartInfo = processInfo;

            if (spawnInfo.StderrHandler != null) {

                process.ErrorDataReceived += spawnInfo.StderrHandler;

            } else {

                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {

                    if (!String.IsNullOrEmpty(e.Data)) {

                        log.Error(e.Data.Trim());

                    }

                };

            }

            if (spawnInfo.StdoutHandler != null) {

                process.OutputDataReceived += spawnInfo.StdoutHandler;

            } else {

                process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {

                    if (!String.IsNullOrEmpty(e.Data)) {

                        log.Info(e.Data.Trim());

                    }

                };

            }

            if (spawnInfo.ExitHandler != null) {

                process.Exited += ExitHandler;
                process.Exited += spawnInfo.ExitHandler;

            } else {

                process.Exited += ExitHandler;

            }

        }

        public void Start() {

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

        }

        public void Stop() {

            process.CloseMainWindow();
            process.CancelOutputRead();
            process.CancelErrorRead();
            process.Close();

        }

        public Boolean Stat() {

            bool stat = false;

            try {
            
                var junk = System.Diagnostics.Process.GetProcessById(process.Id);
                stat = true;

            } catch { }

            return stat;

        }

        public void Pause() {
        
        }

        public void Resume() {
        
        }

        public void Kill() {

            process.Kill();
            process.CancelOutputRead();
            process.CancelErrorRead();
            process.Close();

        }

        public Int32 ExitCode() {

            return exitCode;

        }

        #region Private Methods

        private void ExitHandler(object sender, EventArgs e) {

            exitCode = process.ExitCode;

        }

        private string[] ParseCommandLine(string commandLine) {

            // taken from: https://github.com/wyattoday/wyupdate/blob/master/Util/CmdLineToArgvW.cs
            // returns a parsed command line like what Environment.GetCommandLineArgs() returns

            IntPtr ptrToSplitArgs = CommandLineToArgvW(commandLine, out int numberOfArgs);

            // CommandLineToArgvW returns NULL upon failure.

            if (ptrToSplitArgs == IntPtr.Zero) {

                throw new ArgumentException("Unable to split argument.", new Win32Exception());

            }

            // Make sure the memory ptrToSplitArgs to is freed, even upon failure.

            try {

                string[] splitArgs = new string[numberOfArgs];

                // ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
                // Copy each of these strings into our split argument array.

                for (int i = 0; i < numberOfArgs; i++) {

                    splitArgs[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));

                }

                return splitArgs;

            } finally {

                // Free memory obtained by CommandLineToArgW.

                LocalFree(ptrToSplitArgs);

            }

        }

        #endregion

    }

}
