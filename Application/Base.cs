using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.App.Configuration.Extensions;

namespace XAS.App {

    /// <summary>
    /// A base class for programs to inherit from. It provides common methods.
    /// </summary>
    ///
    public class Base {

        private ILogger log = null;
        protected readonly ISecurity security = null;
        protected readonly IConfiguration config = null;
        protected readonly IErrorHandler handler = null;
        protected readonly ILoggerFactory logFactory = null;

        // allows the console app to close the console window.
        // taken from: https://stackoverflow.com/questions/2686289/how-to-run-a-net-console-application-in-the-background

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_SHOWMINIMIZED = 2;

        /// <summary>
        /// Returns the version number from the main entry assembly.
        /// </summary>
        ///
        public String Version {
            get {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// Initalize the class.
        /// </summary>
        ///
        public Base(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory, ISecurity security) {

            this.config = config;
            this.handler = handler;
            this.security = security;
            this.logFactory = logFactory;
            this.log = logFactory.Create(typeof(Base));

            var key = config.Key;
            var section = config.Section;
         
            // default unhanlded exception handler

            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs args) {

                Exception ex = args.ExceptionObject as Exception;

                handler.Exceptions(ex);

            };

            // Load an assembly from a different path, then the applications directory or a sub directory thereof.

            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args) {

                log.Debug(String.Format("Looking for {0}", args.Name));

                // taken from: https://stackoverflow.com/questions/1892492/set-custom-path-to-referenced-dlls
                // with modifications

                Assembly assembly = null;
                string assemblyFile = (args.Name.Contains(','))
                            ? args.Name.Substring(0, args.Name.IndexOf(','))
                            : args.Name;

                assemblyFile += ".dll";

                string path = config.GetValue(section.Environment(), key.LibDir());
                string targetPath = Path.Combine(path, assemblyFile);
                log.Debug(String.Format("loading: {0}", targetPath));

                if (File.Exists(targetPath)) {

                    assembly = Assembly.LoadFrom(targetPath);

                }

                return assembly;

            };

        }

        /// <summary>
        /// Process configuration options.
        /// </summary>
        /// <remarks>
        /// This works by checking for a configuration file for the program. If found
        /// it will be loaded. Any command line options will override what is in the
        /// configuration file.
        /// </remarks>
        /// <param name="args">An array of options.</param>
        /// <returns>Parsed parameters.</returns>
        ///
        public String[] ProcessOptions(String[] args) {

            Options options = this.GetOptions();
            List<string> parameters = options.Parse(args).ToList();

            return parameters.ToArray();

        }

        /// <summary>
        /// Diplays a simple usage string.
        /// </summary>
        ///
        public void DisplayUsage() {

            string usage = GetUsage();

            System.Console.WriteLine(usage);
            Environment.Exit(1);

        }

        #region Virtual Methods

        /// <summary>
        /// Main entry point for a command line program.
        /// </summary>
        /// <param name="args">An array of options.</param>
        /// <returns>A return code that can be passed back to the command processor.</returns>
        ///
        public virtual Int32 RunApp(string[] args) {

            log.Info("Passed args:");

            foreach (string arg in args) {

                log.Info(String.Format("  {0}", arg));

            }

            return 0;

        }

        /// <summary>
        /// A help method to return a brief usage string.
        /// </summary>
        /// <remarks>
        /// This should be a simple one line string. There are other methods to return detailed help 
        /// and documentation of the program.
        /// </remarks>
        /// <returns>A simple help message.</returns>
        ///
        public virtual String GetUsage() {

            var key = config.Key;
            var section = config.Section;

            return String.Format("Usage: {0} [--help] [--manual] [--version]", config.GetValue(section.Application(), key.Script()));

        }

        /// <summary>
        /// Return documentation for the program.
        /// </summary>
        /// <remarks>
        /// This method should be overridden and used to return useful documentation for the program.
        /// </remarks>
        /// <returns>Formated text.</returns>
        ///
        public virtual String[] GetManual() {

            string[] text = { "No manual provided." };

            return text;

        }

        /// <summary>
        /// Defines a standard set of command line options. 
        /// </summary>
        /// <remarks>
        /// This method should be overridden and then have the base method called to return the 
        /// standardized options. These returned options can be added to for further command line
        /// processing. Thus keeping a unified command line experience.
        /// </remarks>
        /// <returns>An Options object.</returns>
        ///
        public virtual Options GetOptions() {

            var key = config.Key;
            var section = config.Section;

            string confFile = config.GetValue(section.Environment(), key.LogConf());
            string logFile = config.GetValue(section.Environment(), key.LogFile());

            Options options = new Options(config) {
                { "alerts", "toggles the sending of alerts", (v) => {
                        if (String.IsNullOrEmpty(v)) {
                            config.UpdateKey(section.Environment(), key.Alerts(), "false");
                        } else {
                            config.UpdateKey(section.Environment(), key.Alerts(), "true");
                        }
                    }
                },
                { "debug", "toggles debug output to the log", (v) => {
                        if (String.IsNullOrEmpty(v)) {
                            log.SetLevel(LogLevel.Info);
                            config.UpdateKey(section.Environment(), key.Debug(), "false");
                            config.UpdateKey(section.Environment(), key.Trace(), "false");
                            config.UpdateKey(section.Environment(), key.LogLevel(), LogLevel.Info.ToString());
                        } else {
                            log.SetLevel(LogLevel.Debug);
                            config.UpdateKey(section.Environment(), key.Debug(), "true");
                            config.UpdateKey(section.Environment(), key.LogLevel(), LogLevel.Debug.ToString());
                        }
                    }
                },
                { "trace", "toggles trace output to the log", (v) => {
                        if (String.IsNullOrEmpty(v)) {
                            log.SetLevel(LogLevel.Info);
                            config.UpdateKey(section.Environment(), key.Debug(), "false");
                            config.UpdateKey(section.Environment(), key.Trace(), "false");
                            config.UpdateKey(section.Environment(), key.LogLevel(), LogLevel.Info.ToString());
                        } else {
                            log.SetLevel(LogLevel.Trace);
                            config.UpdateKey(section.Environment(), key.Debug(), "true");
                            config.UpdateKey(section.Environment(), key.Trace(), "true");
                            config.UpdateKey(section.Environment(), key.LogLevel(), LogLevel.Trace.ToString());
                        }
                    }
                },
                { "version|v", "outputs the programs version", (v) => {
                        System.Console.WriteLine("Version: v{0}", this.Version);
                        Environment.Exit(0);
                    }
                },
                { "help|h|?", "outputs a simple help message", (v) => {
                        DisplayHelp();
                        Environment.Exit(0);
                    }
                },
                { "manual", "outputs the progams manual", (v) => {
                        DisplayManual();
                        Environment.Exit(0);
                    }
                },
                { "facility=", "sets the alerts facility, default \"system\"", (v) => {
                        string facility = v.ToLower();
                        config.UpdateKey(section.Environment(), key.Facility(), facility);
                    }
                },
                { "priority=", "sets the alerts priority, default \"low\"", (v) => {
                        string  priority = v.ToLower();
                        config.UpdateKey(section.Environment(), key.Priority(), priority);
                    }
                },
                { "log-type=", "toggles the log type, default \"console\", possible \"file\", \"json\" or \"event\"", (v) => {
                        string logType = v.ToLower();
                        if (logType == "console" || logType == "file" || logType == "json" || logType == "event") { 
                            config.UpdateKey(section.Environment(), key.LogType(), v);
                            log.Close();
                            log = logFactory.Create(typeof(Base));
                        } else {
                            log.ErrorMsg(key.LogFile(), v);
                            Environment.Exit(1);
                        }
                    }
                },
                { "log-file=", "names the log file to use, default \"" + logFile + "\"", (v) => {
                        config.UpdateKey(section.Environment(), key.LogFile(), v);
                        config.UpdateKey(section.Environment(), key.LogType(), LogType.File.ToString().ToLower());
                        log.Close();
                        log = logFactory.Create(typeof(Base));
                    }
                },
                { "log-conf=", "alternative logging configuration file, default \"" + confFile + "\"", (v) => {
                        if (File.Exists(v)) {
                            config.UpdateKey(section.Environment(), key.LogConf(), v);
                            log.Close();
                            log = logFactory.Create(typeof(Base));
                        } else {
                            log.ErrorMsg(key.FileMissing(), v);
                            Environment.Exit(1);
                        }
                    }
                }
            };

            return options;

        }

        /// <summary>
        /// Returns the text for the options.
        /// </summary>
        /// <returns>A List of strings.</returns>
        /// 
        public List<string> GetOptionsText() {

            int length = 0;
            Options options = this.GetOptions();
            List<string> text = new List<string>();

            foreach (Option option in options) {

                if (option.Prototype.Length > length) {

                    length = option.Prototype.Length;

                }

            }

            for (int i = 0; i < options.Count; i++) {

                text.Add(String.Format("-{0," + -(length) + "}  - {1}.", options[i].Prototype, options[i].Description));

            }

            return text;

        }

        /// <summary>
        /// A signal handler.
        /// </summary>
        /// <remarks>
        /// This should be overridden is special handling needs to be done when a ^C or ^BREAK is presses.
        /// </remarks>
        /// 
        public virtual void SignalHandler() {

        }

        /// <summary>
        /// Keyboard handler for ^c and ^Break.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The recieved event.</param>
        /// 
        public void OnConsoleKeyPress(object sender, ConsoleCancelEventArgs e) {

            var key = config.Key;
            var section = config.Section;

            // taken from https://www.codeproject.com/articles/16164/managed-application-shutdown
            // with modifications. surprising how hard this is. 

            if (e.SpecialKey == ConsoleSpecialKey.ControlBreak) {

                Thread t = new Thread(delegate () {

                    SignalHandler();

                    string format = config.GetValue(section.Messages(), key.ProcessInterrupt());
                    log.WarnMsg(format, "ControlBreak");

                    Environment.Exit(1);

                });

                t.Start();
                t.Join();

            }

            if (e.SpecialKey == ConsoleSpecialKey.ControlC) {

                e.Cancel = true;

                Thread t = new Thread(delegate () {

                    SignalHandler();

                    string format = config.GetValue(section.Messages(), key.ProcessInterrupt());
                    log.WarnMsg(format, "ControlC");

                    Environment.Exit(2);

                });

                t.Start();

            }

        }

        #endregion

        #region Private Methods

        private void DisplayHelp() {

            String usage = this.GetUsage();
            Options options = this.GetOptions();
            List<string> optionsText = this.GetOptionsText();

            System.Console.WriteLine("");
            System.Console.WriteLine(usage);
            System.Console.WriteLine("");
            System.Console.WriteLine("  Options:");

            foreach (var line in optionsText) {

                System.Console.WriteLine("    {0}", line);

            }

            System.Console.WriteLine("");
            System.Console.WriteLine("Options can begin with a \"-\",\"--\" or \"/\". Single character options must ");
            System.Console.WriteLine("start with a \"-\" and can be stacked. Options with a trailing \"=\" have required");
            System.Console.WriteLine("arguments, options with a trailing \":\", have optional arguments. Options that are");
            System.Console.WriteLine("being used to toggle functionality can have a trailing \"-\" or \"+\" to force boolean");
            System.Console.WriteLine("behavior. Option processing ends with \"-- \" and the rest of the commadline is");
            System.Console.WriteLine("passed verbatium.");
            System.Console.WriteLine("");

        }

        private void DisplayManual() {

            string[] text = this.GetManual();

            foreach (string line in text) {

                System.Console.WriteLine("{0}", line);

            }

        }

        #endregion

    }

}
