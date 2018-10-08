using System;
using System.IO;
using System.Collections;

using IWshRuntimeLibrary;

using XAS.Core.Logging;
using XAS.Core.Exceptions;
using XAS.Core.Extensions;
using XAS.Core.Configuration;

namespace XAS.Core.Utilities {

    /// <summary>
    /// A class to manipulate Windows Shares.
    /// </summary>
    /// 
    public class Shares: IDisposable {

        private readonly ILogger log = null;
        private readonly WshNetwork wsh = null;
        private readonly IErrorHandler handler = null;
        private readonly IConfiguration config = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This initializes the WshNetwork COM object and the logger.
        /// </remarks>
        /// 
        public Shares(IConfiguration config, IErrorHandler handler, ILoggerFactory logFactory) {

            this.config = config;
            this.handler = handler;
            this.wsh = new WshNetwork();
            this.log = logFactory.Create(typeof(Shares));

        }

        /// <summary>
        /// Attach to a network share.
        /// </summary>
        /// <param name="share">Mandatory parameter, name of the share.</param>
        /// <param name="username">Optional parameter, username for connecting to the share, must include domain.</param>
        /// <param name="password">Optional parameter, password for connecting to the share.</param>
        /// <param name="drive">Optional parameter, a drive letter to associate with the share.</param>
        /// <param name="presistent">Optional parameter, defaults to false.</param>
        /// <returns>Returns true on success.</returns>
        /// 
        public Boolean Attach(String share, String username = "", String password = "", String drive = "", Boolean presistent = false) {

            wsh.MapNetworkDrive(drive, share, presistent, username, password);

            return true;

        }

        /// <summary>
        /// Detach from a share.
        /// </summary>
        /// <param name="share">The share to detach from.</param>
        /// <param name="force">Optional parameter, wither to force the detachment, defaults to false.</param>
        /// <param name="update">Optional paramter, wither to update the user profile, defaults to false.</param>
        /// <returns>Returns true on success.</returns>
        ///
        public Boolean Detach(String share, Boolean force = false, Boolean update = false) {

            wsh.RemoveNetworkDrive(share, force, update);

            return true;
                        
        }

        /// <summary>
        /// Check to see if a share is already mounted.
        /// </summary>
        /// <param name="share">The share to check.</param>
        /// <returns>Returns true, if the share exists.</returns>
        /// 
        public Boolean Exists(String share) {

            string path = Path.GetPathRoot(share);

            return Directory.Exists(path);

        }

        /// <summary>
        /// Returns the next available drive on the system.
        /// </summary>
        /// <returns>Returns a drive letter or null.</returns>
        /// 
        public String GetNextDrive(String start = "C:\\") {

            string drv = "";
            string wanted = start.ToUpper();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            ArrayList drives = new ArrayList(23) {
                "D:\\", "E:\\", "F:\\", "G:\\", "H:\\", "I:\\", "J:\\", "K:\\", "L:\\", "M:\\", "N:\\",
                "O:\\", "P:\\", "Q:\\", "R:\\", "S:\\", "T:\\", "U:\\", "V:\\", "W:\\", "X:\\", "Y:\\", "Z:\\"
            };

            // filter known drives from the list

            for (int i = 0; i < allDrives.Length; i++) {

                if (drives.Contains(allDrives[i].Name)) {

                    drives.Remove(allDrives[i].Name);

                }

            }

            // find next available drive to use

            for (int i = 0; i < drives.Count; i++) {

                string drive = drives[i].ToString();

                if ((String.CompareOrdinal(drive, wanted)) > 0) {

                    drv = drive.TrimIfEndsWith("\\");
                    break;

                }

            }

            return drv;

        }

        /// <summary>
        /// Gets the drive letter associated with a mounted share.
        /// </summary>
        /// <param name="share">The share to use.</param>
        /// <returns>Returns a drive letter or null.</returns>
        /// 
        public String GetDriveFromShare(String share) {

            string drv;
            string shr;
            string drive = "";
            IWshCollection drives = null;

            try {

                drives = wsh.EnumNetworkDrives();

                for (int i = 0; i < drives.Count(); i += 2) {

                    drv = drives.Item(i);
                    shr = drives.Item(i + 1);

                    if (share == shr) {

                        drive = drv;
                        break;

                    }

                }

            } catch {

                throw;

            } finally {

                Utils.ReleaseComObject(drives);

            }

            return drive;
                            
        }

        /// <summary>
        /// Gets the share from the associated drive letter.
        /// </summary>
        /// <param name="drive">The drive to use.</param>
        /// <returns>Returns the share or null.</returns>
        /// 
        public String GetShareFromDrive(String drive) {

            string drv;
            string shr;
            string share = "";
            IWshCollection drives = null;

            try {

                drives = wsh.EnumNetworkDrives();

                for (int i = 0; i < drives.Count(); i += 2) {

                    drv = drives.Item(i);
                    shr = drives.Item(i + 1);

                    if (drive == drv) {

                        share = shr;
                        break;

                    }

                }

            } catch {

                throw;

            } finally {

                Utils.ReleaseComObject(drives);

            }

            return share;

        }

        /// <summary>
        /// Checks to see if a path is a share.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>Returns true, if it is a share.</returns>
        /// 
        public Boolean IsShare(String path) {

            return path.StartsWith("\\\\");

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Generic dispose.
        /// </summary>
        /// <param name="disposing"></param>

        protected virtual void Dispose(bool disposing) {

            if (!disposedValue) {

                if (disposing) {

                    // TODO: dispose managed state (managed objects).

                    this.wsh = null;

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Utils.ReleaseComObject(wsh);

                disposedValue = true;

            }

        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Shares() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// Generic dispose.
        /// </summary>

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion

    }

}
