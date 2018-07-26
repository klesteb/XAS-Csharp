using System;
using System.Collections.Generic;

using XAS.Core.Logging;

namespace XAS.App {

    /// <summary>
    /// A base class for handling shells.
    /// </summary>
    /// 
    public class CommandHandler: ICommandHandler {

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public CommandHandler() { }

        /// <summary>
        /// Display a short help screen for the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="options"></param>
        /// 
        public void DisplayHelp(String command, Options options) {

            List<string> optionsText = this.GetOptionsText(options);

            System.Console.WriteLine("");
            System.Console.WriteLine("Usage: {0}", command);
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
            System.Console.WriteLine("behavior.");
            System.Console.WriteLine("");

        }

        /// <summary>
        /// Returns the text for the options.
        /// </summary>
        /// <returns>A List of strings.</returns>
        /// 
        private List<string> GetOptionsText(Options options) {

            int length = 0;
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

    }

}
