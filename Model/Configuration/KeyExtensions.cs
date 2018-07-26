using System;

using XAS.Core.Configuration;

namespace XAS.Model.Configuration {

    /// <summary>
    /// Configuration Key extensions for Model.
    /// </summary>
    /// 
    public static class KeyExtensions {

        public static String RepositoriesUpdateErrors(this Key junk) {
            return "RepositoriesUpdateErrors";
        }

        public static String RepositoriesValidationErrors(this Key junk) {
            return "RepositoriesValidationErrors";
        }

        public static String RepositoriesValidationProperties(this Key junk) {
            return "RepositoriesValidationProperties";
        }

    }

}
