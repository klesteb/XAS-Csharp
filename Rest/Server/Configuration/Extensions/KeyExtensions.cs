using System;

using XAS.Core.Configuration;

namespace XAS.Rest.Server.Configuration.Extensions {

    /// <summary>
    /// Key extension for the DemoMicroServiceServer.
    /// </summary>
    /// 
    public static class KeyExtensions {

        // messages

        public static String DELETE(this Key junk) {
            return "DELETE";
        }

        public static String DELETE_NoDelete(this Key junk) {
            return "DELETE_NoDelete";
        }

        public static String GET(this Key junk) {
            return "GET";
        }

        public static String GETS(this Key junk) {
            return "GETS";
        }

        public static String OPTION(this Key junk) {
            return "OPTION";
        }

        public static String OPTIONS(this Key junk) {
            return "OPTIONS";
        }

        public static String OPTIONA(this Key junk) {
            return "OPTIONA";
        }

        public static String POST(this Key junk) {
            return "POST";
        }

        public static String POST_NoCreate(this Key junk) {
            return "POST_NoCreate";
        }

        public static String POST_NoValidate(this Key junk) {
            return "POST_NoValidate";
        }

        public static String PUT(this Key junk) {
            return "PUT";
        }

        public static String PUTA(this Key junk) {
            return "PUTA";
        }

        public static String PUT_NoDelete(this Key junk) {
            return "PUT_NoDelete";
        }

        public static String PUT_NoValidate(this Key junk) {
            return "PUT_NoValidate";
        }

        public static String PUT_NoUpdate(this Key junk) {
            return "PUT_NoUpdate";
        }

    }

}
