using System;
using System.Windows.Forms;
using CP2077SaveEditor.Utils;
using CP2077SaveEditor.Views;

namespace CP2077SaveEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Log anything unexpected instead of crashing without a trace
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (_, e) => ReportUnhandled(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => ReportUnhandled(e.ExceptionObject as Exception);

            Theme.Initialize(AppConfig.Theme);

            Application.Run(new MainForm());
        }

        private static void ReportUnhandled(Exception exception)
        {
            var logFile = Logger.Error("Unhandled exception", exception);
            MessageBox.Show(
                $"An unexpected error occurred: {exception?.Message}{Environment.NewLine}{Environment.NewLine}" +
                $"Details were written to:{Environment.NewLine}{logFile}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
