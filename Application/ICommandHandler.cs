using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAS.App {

    /// <summary>
    /// Interface for CommandHandler.
    /// </summary>
    /// 
    public interface ICommandHandler {

        /// <summary>
        /// Displays the help associated with the options.
        /// </summary>
        /// <param name="command">The command that help is needed.</param>
        /// <param name="options">An Options object.</param>
        /// 
        void DisplayHelp(String command, Options options);

    }

}
