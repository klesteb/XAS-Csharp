using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace XAS.Core {
    
    /// <summary>
    /// General purpose utilities.
    /// </summary>
    /// 
    public static class Utils {

        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

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

        /// <summary>
        /// Parse a string into command line arguments.
        /// </summary>
        /// <param name="commandLine">A string.</param>
        /// <returns>A string array.</returns>
        /// <remarks>
        /// This will parse a string into command line arguments just like the 
        /// .NET runtime does. It will also expand any environment variables 
        /// found in the string.
        /// </remarks>
        /// 
        public static String[] ParseCommandLine(String commandLine) {

            // expand any environment variables in the command line.

            string cmdLine = Environment.ExpandEnvironmentVariables(commandLine);

            // taken from: https://github.com/wyattoday/wyupdate/blob/master/Util/CmdLineToArgvW.cs
            // returns a parsed command line like what Environment.GetCommandLineArgs() returns

            IntPtr ptrToSplitArgs = CommandLineToArgvW(cmdLine, out int numberOfArgs);

            // CommandLineToArgvW returns NULL upon failure.

            if (ptrToSplitArgs == IntPtr.Zero) {

                throw new ArgumentException("Unable to split argument.", new Win32Exception());

            }

            // Make sure the memory ptrToSplitArgs to is freed, even upon failure.

            try {

                string[] splitArgs = new string[numberOfArgs];

                // ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
                // Copy each of these strings into our split argument array.

                for (int i = 0; i < numberOfArgs; i++) {

                    splitArgs[i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));

                }

                return splitArgs;

            } finally {

                // Free memory obtained by CommandLineToArgW.

                LocalFree(ptrToSplitArgs);

            }

        }

    }

}
