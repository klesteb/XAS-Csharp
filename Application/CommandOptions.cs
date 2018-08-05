using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using XAS.Core;
using XAS.Core.Logging;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;
using XAS.App.Configuration.Extensions;

namespace XAS.App {

    // taken from https://stackoverflow.com/questions/491595/best-way-to-parse-command-line-arguments-in-c
    // with modifications.

    /// <summary>
    /// A class for parsing commands inside a tool. Based on Novell Options class (http://www.ndesk.org/Options).
    /// </summary>
    /// 
    public class CommandOptions: ICommandOptions {

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        private readonly ILogger log = null;
        private readonly IConfiguration config = null;
        private Dictionary<string, string> descriptions;
        private Dictionary<string, Func<String[], Boolean>> actions;

        /// <summary>
        /// Get/set the command prompt.
        /// </summary>
        /// 
        public String Prompt { get; set; }

        /// <summary>
        /// Get/Set wither we are processing a command file.
        /// </summary>
        /// 
        public Boolean InCommandFile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// 
        public CommandOptions(IConfiguration config, ILoggerFactory logFactory) {

            Prompt = "> ";

            descriptions = new Dictionary<string, string>();
            actions = new Dictionary<string, Func<string[], bool>>();

            this.config = config;
            this.InCommandFile = false;
 
            log = logFactory.Create(typeof(CommandOptions));

        }

        /// <summary>
        /// Adds a command option, a description of the options and an action to 
        /// perform when the command is found.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">A brief description of the command.</param>
        /// <param name="action">An action delegate that has one parameter - string[] args.</param>
        /// <returns>The current CommandOptions instance.</returns>
        /// 
        public CommandOptions Add(string name, string description, Func<String[], Boolean> action) {

            actions.Add(name, action);
            descriptions.Add(name, description);

            return this;

        }

        /// <summary>
        /// Process the shell commands.
        /// </summary>
        /// 
        public Int32 Process(params String[] args) {

            bool stat = false;
            var key = config.Key;
            var section = config.Section;
            string input = String.Join(" ", args);

            if (args.Length > 0) {

                if (!(stat = Dispatch(input))) {

                    log.Warn(config.GetValue(section.Messages(), key.UnknownCommand()));
                    log.Debug(String.Format("dispatch stat = {0}", stat));

                }

            } else {

                if (!System.Console.IsInputRedirected) {

                    System.Console.WriteLine("Help is available with the \"help\" command.");

                }

                for (;;) {

                    input = Input();

                    if (! String.IsNullOrEmpty(input)) {

                        if (input.StartsWith("@")) {

                            string[] parts = input.Split('@');

                            try {

                                FileInfo sourceFile = new FileInfo(parts[1]);
                                TextReader commandFileReader = new StreamReader(sourceFile.FullName);
                                System.Console.SetIn(commandFileReader);

                                InCommandFile = true;

                            } catch (IOException ex) {

                                log.Warn(ex.Message);

                            }

                        } else if ((input == "quit") || (input == "exit")) {

                            break;

                        } else if ((input == "cls") || (input == "clear")) {

                            System.Console.Clear();

                        } else if (input == "help") {

                            DisplayHelp();

                        } else {

                            if (!(stat = Dispatch(input))) {

                                log.Warn(config.GetValue(section.Messages(), key.UnknownCommand()));
                                log.Debug(String.Format("dispatch stat = {0}", stat));

                            }

                        }

                    } else {

                        if (System.Console.IsInputRedirected) {

                            // end-of-file on stdin detected

                            break;

                        } else if (InCommandFile) {

                            // end--of-file with command file detected

                            var stdin = System.Console.OpenStandardInput();
                            var stream = new StreamReader(stdin);
                            System.Console.SetIn(stream);

                            InCommandFile = false;

                        }

                    }

                }

            }

            return stat ? 0 : 1;

        }

        #region Private Methods

        /// <summary>
        /// Parses the command line and calls any actions associated with that command.
        /// </summary>
        /// <param name="commandLine">The text command, e.g "show databases."</param>
        /// 
        private bool Dispatch(string commandLine) {

            bool stat = false;
            string[] args = ParseCommandLine(commandLine);
            string arg = args[0];

            log.Debug(Utils.Dump(args));

            try {

                stat = actions[arg].Invoke(args.Skip(1).ToArray());

            } catch(Exception ex) {

                log.Debug(String.Format("Exception = {0}", ex.Message));

            }

            return stat;

        }

        private void DisplayHelp() {

            List<string> commandsText = this.GetCommandsText();

            System.Console.WriteLine("");
            System.Console.WriteLine("Internal Commands:");
            System.Console.WriteLine("    clear  - clear the screen.");
            System.Console.WriteLine("    cls    - clear the screen.");
            System.Console.WriteLine("    exit   - exit the shell.");
            System.Console.WriteLine("    help   - display this screen.");
            System.Console.WriteLine("    quit   - exit the shell.");
            System.Console.WriteLine("");
            System.Console.WriteLine("Additional Commands:");

            foreach (var line in commandsText) {

                System.Console.WriteLine("    {0}", line);

            }

            System.Console.WriteLine("");
            System.Console.WriteLine("Additional help is available with <command> --help.");
            System.Console.WriteLine("");

        }

        private List<string> GetCommandsText() {

            int length = 0;
            List<string> text = new List<string>();

            foreach (KeyValuePair<string, string> item in descriptions) {

                if (item.Key.Length > length) {

                    length = item.Key.Length;

                }

            }

            foreach (KeyValuePair<string, string> item in descriptions) {

                text.Add(String.Format("{0," + -(length) + "} - {1}.", item.Key, item.Value));

            }

            return text;

        }

        private String Input() {

            string line;

            if (! (System.Console.IsInputRedirected || InCommandFile)) {

                System.Console.Write(this.Prompt);

            }

            string buffer = System.Console.ReadLine();

            // buffer can be null if the EOF has been reached on 
            // stdin or the redirected command file.

            if (String.IsNullOrEmpty(buffer)) {

                line = buffer;

            } else {
            
                line = Environment.ExpandEnvironmentVariables(buffer);

            }

            // need to slow things down when redirected, causes problems 
            // with option processing.

            if (System.Console.IsInputRedirected || InCommandFile) {

                Thread.Sleep(100); 

            }

            return line;

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
