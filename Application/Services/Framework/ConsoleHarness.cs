using System;

namespace XAS.App.Services.Framework {

    // taken from: http://geekswithblogs.net/BlackRabbitCoder/archive/2011/03/01/c-toolbox-debug-able-self-installable-windows-service-template-redux.aspx
    // with modifications

    /// <summary>
    /// Run a service as a console application.
    /// </summary>
    /// 
    public static class ConsoleHarness {

        // Run a service from the console given a service implementation

        /// <summary>
        /// Run the service.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <param name="service">An IWindowsService object.</param>
        /// 
        public static void Run(string[] args, IWindowsService service) {

            ConsoleKeyInfo cki;
            string serviceName = service.GetType().Name;

            // simulate starting the windows service

            service.OnStart(args);

            // let it run as long as Q is not pressed

            do {

                WriteToConsole(ConsoleColor.Yellow, "Running {0}: Enter either [Q]uit, [P]ause, [R]esume: ", serviceName);
                cki = System.Console.ReadKey(true);

                switch (cki.Key) {
                    case ConsoleKey.P:
                        service.OnPause();
                        break;

                    case ConsoleKey.R:
                        service.OnContinue();
                        break;

                    case ConsoleKey.Q:
                        break;
                }

            } while (cki.Key != ConsoleKey.Q);

            // shutdown

            service.OnShutdown();

        }

        // Helper method to write a message to the console at the given foreground color.

        internal static void WriteToConsole(ConsoleColor foregroundColor, string format,
                params object[] formatArguments) {

            System.ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;

            System.Console.WriteLine(format, formatArguments);
            System.Console.Out.Flush();

            System.Console.ForegroundColor = originalColor;

        }

    }

}
