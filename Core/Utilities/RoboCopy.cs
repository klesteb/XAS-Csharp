using System;
using System.IO;
using System.Collections.Generic;

using XAS.Core.Logging;
using XAS.Core.Security;
using XAS.Core.Exceptions;
using XAS.Core.Configuration;
using XAS.Core.Configuration.Extensions;

namespace XAS.Core.Utilities {

    /// <summary>
    /// A class to run robocopy.exe
    /// </summary>
    /// 
    public class RoboCopy {

        private readonly ILogger log = null;
        private readonly String roboCopy = "";
        private readonly String lockName = "";
        private readonly ISecurity secure = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An IConfiguration object.</param>
        /// <param name="handler">An IErrorHandler object.</param>
        /// <param name="logFactory">An ILoggerFactory object.</param>
        /// 
        public RoboCopy(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            var key = config.Key;
            var section = config.Section;

            this.config = config;
            this.handler = handler;
            this.secure = new Secure();
            this.log = logFactory.Create(typeof(RoboCopy));

            this.lockName = config.GetValue(section.Environment(), key.LockName());
            this.roboCopy = Path.Combine(
                Environment.GetEnvironmentVariable("SystemRoot"), 
                "System32", 
                "Robocopy.exe"
            );

        }

        public Int32 Copy(String source, String destination, String filename) {

            String format = "\"{0}\" \"{1}\" \"{2}\" /np /r:1";
            String args = String.Format(format, source, destination, filename);
            String command = Environment.ExpandEnvironmentVariables(args);

            return DoCommand(command);

        }

        public Int32 Move(String source, String destination, String filename) {

            String format = "\"{0}\" \"{1}\" \"{2}\" /mov /np /r:1";
            String args = String.Format(format, source, destination, filename);
            String command = Environment.ExpandEnvironmentVariables(args);

            return DoCommand(command);

        }

        public Int32 Mirror(String source, String destination) {

            String format = "\"{0}\" \"{1}\" /mir /e /np /r:1";
            String args = String.Format(format, source, destination);
            String command = Environment.ExpandEnvironmentVariables(args);

            return DoCommand(command);

        }

        #region Private Methods

        private Int32 DoCommand(String command) {

            Int32 stat = 0;
            var stdout = new List<string>();
            var stderr = new List<string>();

            log.Debug(String.Format("command = {0} {1}", roboCopy, command));

            stat = secure.RunAs(roboCopy, command, out stdout, out stderr);
            CheckStatus(stat);

            if (stdout.Count > 0) {

                foreach (string line in stdout) {

                    log.Info(String.Format("    {0}", line));

                }

            }

            if (stderr.Count > 0) {

                foreach (string line in stderr) {

                    log.Error(String.Format("    {0}", line));

                }

            }

            return stat;

        }

        //
        // robocopy return codes.
        //
        // taken from: http://technet.microsoft.com/en-us/library/cc733145%28v=ws.10%29.aspx
        //
        // The return code from Robocopy is a bit map, defined as follows:
        //
        // Hex Decimal Meaning if set
        //
        // 0×10 16 Serious error. Robocopy did not copy any files.
        //
        // Either a usage error or an error due to insufficient access privileges
        // on the source or destination directories.
        //
        // 0×08 8 Some files or directories could not be copied
        //
        // (copy errors occurred and the retry limit was exceeded).
        //
        // Check these errors further.
        //
        // 0×04 4 Some Mismatched files or directories were detected.
        //
        // Examine the output log. Some housekeeping may be needed.
        //
        // 0×02 2 Some Extra files or directories were detected.
        //
        // Examine the output log for details.
        //
        // 0×01 1 One or more files were copied successfully (that is, new files have arrived).
        //
        // 0×00 0 No errors occurred, and no copying was done.
        // 
        // The source and destination directory trees are completely synchronized.
        //
        // Possible error codes:
        //
        // 0x800a0035 - file not found.
        // 0x800a004c - path not found.

        private void CheckStatus(Int32 rc) {

            var key = config.Key;
            var section = config.Section;

            if (Bitops.IsSet(rc, 16)) {

                log.ErrorMsg(key.RoboCopyLevel6());

            } else if (Bitops.IsSet(rc, 8)) {

                log.WarnMsg(key.RoboCopyLevel5());

            } else if (Bitops.IsSet(rc, 4)) {

                log.WarnMsg(key.RoboCopyLevel4());

            } else if (Bitops.IsSet(rc, 2)) {

                log.WarnMsg(key.RoboCopyLevel3());

            } else if (Bitops.IsSet(rc, 1)) {

                log.InfoMsg(key.RoboCopyLevel2());

            } else if (rc == 0) {

                log.WarnMsg(key.RoboCopyLevel1());

            }

        }
        
        #endregion

    }

}
