using System;

using XAS.Core.Configuration;

namespace DemoMicroServiceServer.Configuration.Extensions {

    /// <summary>
    /// Key extension for the DemoMicroServiceServer.
    /// </summary>
    /// 
    public static class KeyExtensions {

        public static String GET(this Key junk) {
            return "Processing GET(/{0}) for {1}";
        }

        public static String GETS(this Key junk) {
            return "Processing GET(/{0}/{1}) for {2}";
        }

        public static String OPTION(this Key junk) {
            return "Processing OPTIONS(/{0}/{1}) for {2}";
        }

        public static String OPTIONS(this Key junk) {
            return "Processing OPTIONS(/{0}) for {1}";
        }

        public static String Address(this Key junk) {
            return "Address";
        }

        public static String EnableClientCertificates(this Key junk) {
            return "EnableClentCertificates";
        }

        public static String WebRootPath(this Key junk) {
            return "WebRootPath";
        }

    }

}
