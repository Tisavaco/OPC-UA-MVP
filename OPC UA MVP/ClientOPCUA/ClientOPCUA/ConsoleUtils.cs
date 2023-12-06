using Microsoft.Extensions.Logging;
using Opc.Ua;
using System.Text;
using static Opc.Ua.Utils;

namespace ClientOPCUA
{
    public static class ConsoleUtils
    {
        
        

        /// <summary>
        /// Configure the logging providers.
        /// </summary>
        /// <remarks>
        /// Replaces the Opc.Ua.Core default ILogger with a
        /// Microsoft.Extension.Logger with a Serilog file, debug and console logger.
        /// The debug logger is only enabled for debug builds.
        /// The console logger is enabled by the logConsole flag at the consoleLogLevel.
        /// The file logger uses the setting in the ApplicationConfiguration.
        /// The Trace logLevel is chosen if required by the Tracemasks.
        /// </remarks>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="context">The context name for the logger. </param>
        /// <param name="logConsole">Enable logging to the console.</param>
        /// <param name="consoleLogLevel">The LogLevel to use for the console/debug.<
        /// /param>
        

        /// <summary>
        /// Create an event which is set if a user
        /// enters the Ctrl-C key combination.
        /// </summary>
        public static ManualResetEvent CtrlCHandler(CancellationTokenSource cts = default)
        {
            var quitEvent = new ManualResetEvent(false);
            try
            {
                Console.CancelKeyPress += (_, eArgs) => {
                    cts.Cancel();
                    quitEvent.Set();
                    eArgs.Cancel = true;
                };
            }
            catch
            {
                // intentionally left blank
            }
            return quitEvent;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Utils.LogCritical("Unhandled Exception: {0} IsTerminating: {1}", args.ExceptionObject, args.IsTerminating);
        }

        private static void Unobserved_TaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            Utils.LogCritical("Unobserved Exception: {0} Observed: {1}", args.Exception, args.Observed);
        }

    }
}

