using System;

namespace XAS.Core {
    
    /// <summary>
    /// General purpose utilities.
    /// </summary>
    /// 
    public static class Utils {

        /// <summary>
        /// Dump the contents of a object in JSON format.
        /// </summary>
        /// <param name="obj">The object to dump.</param>
        /// <returns>JSON formatted string.</returns>
        /// 
        public static String Dump(Object obj) {

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented).ToString();

        }

        /// <summary>
        /// Release resources aquired from COM objects,
        /// </summary>
        /// <param name="comObject">The COM object to release.</param>
        /// 
        public static void ReleaseComObject(object comObject) {

            if (comObject != null) {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(comObject);

            }

        }

    }

}
