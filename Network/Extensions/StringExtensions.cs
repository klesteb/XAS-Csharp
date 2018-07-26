using System;
using FluentFTP;

namespace XAS.Network.Extensions {

    /// <summary>
    /// String extensions.
    /// </summary>
    /// 
    public static class StringExtensions {

        /// <summary>
        /// Convert a string to a FtpDataConnectionType.
        /// </summary>
        /// <param name="type">A string representation of the data connection type.</param>
        /// <returns>A FtpDataConnectionType.</returns>
        /// 
        public static FtpDataConnectionType ToDataConnectinonType(this String type) {

            FtpDataConnectionType dataType = FtpDataConnectionType.AutoPassive;

            switch (type.ToLower()) {
                case "autoactive":
                    dataType = FtpDataConnectionType.AutoActive;
                    break;
                case "eprt":
                    dataType = FtpDataConnectionType.EPRT;
                    break;
                case "epsv":
                    dataType = FtpDataConnectionType.EPSV;
                    break;
                case "pasv":
                    dataType = FtpDataConnectionType.PASV;
                    break;
                case "pasvex":
                    dataType = FtpDataConnectionType.PASVEX;
                    break;
                case "port":
                    dataType = FtpDataConnectionType.PORT;
                    break;
            }

            return dataType;

        }

        /// <summary>
        /// A convertion method.
        /// </summary>
        /// <param name="type">A string representation of a FtpEncryptionMode</param>
        /// <returns>A FtpEncryptionMode</returns>
        /// 
        public static FtpEncryptionMode ToEncryptionMode(this String type) {

            FtpEncryptionMode mode = FtpEncryptionMode.None;

            switch (type.ToLower()) {
                case "explicit":
                    mode = FtpEncryptionMode.Explicit;
                    break;
                case "implicit":
                    mode = FtpEncryptionMode.Implicit;
                    break;
            }

            return mode;

        }

    }

}
