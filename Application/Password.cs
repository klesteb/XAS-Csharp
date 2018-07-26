using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace XAS.App {

    // taken from: https://stackoverflow.com/questions/3404421/password-masking-console-application
    // with modifications.

    /// <summary>
    /// Set up the console to be able to read a password with noecho mode.
    /// </summary>
    /// 
    public static class Password {

        private enum StdHandle {
            Input = -10,
            Output = -11,
            Error = -12,
        }

        private enum ConsoleMode {
            ENABLE_ECHO_INPUT = 4
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(StdHandle nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        /// <summary>
        /// Read the password from the console.
        /// </summary>
        /// <param name="prompt">Optional prompt, defaults to "Password: ".</param>
        /// <returns>The entered password.</returns>
        /// 
        public static string Read(String prompt = "Password: ") {

            IntPtr stdInputHandle = GetStdHandle(StdHandle.Input);

            if (stdInputHandle == IntPtr.Zero) {

                throw new InvalidOperationException("No console input");

            }

            int previousConsoleMode;

            if (!GetConsoleMode(stdInputHandle, out previousConsoleMode)) {

                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not get console mode.");

            }

            // disable console input echo

            if (!SetConsoleMode(stdInputHandle, previousConsoleMode & ~(int)ConsoleMode.ENABLE_ECHO_INPUT)) {

                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not disable console input echo.");

            }

            // just read the password using standard Console.ReadLine()

            System.Console.Write(prompt);
            string password = System.Console.ReadLine();
            System.Console.WriteLine("");

            // reset console mode to previous

            if (!SetConsoleMode(stdInputHandle, previousConsoleMode)) {

                throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not reset console mode.");

            }

            return password;

        }

    }

}