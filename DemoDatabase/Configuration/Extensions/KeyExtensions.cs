using System;

using XAS.Core.Configuration;

namespace DemoDatabase.Configuration.Extensions {

    public static class KeyExtensions {

        // messages

        public static String Created(this Key junk) {
            return "Created";
        }

        public static String UnknownModel(this Key junk) {
            return "UnknownModel";
        }

        public static String Implement(this Key junk) {
            return "Implement";
        }

        public static String AddedTable(this Key junk) {
            return "AddedTable";
        }

        // config file

        public static String DatabaseModel(this Key junk) {
            return "model";
        }

    }

}
