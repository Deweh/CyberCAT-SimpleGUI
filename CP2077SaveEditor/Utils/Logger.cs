using System;
using System.IO;

namespace CP2077SaveEditor.Utils
{
    /// <summary>
    /// Appends errors to logs\yyyy-MM-dd.log next to the executable, so users can attach them to bug reports.
    /// </summary>
    public static class Logger
    {
        private static readonly object s_lock = new();

        public static string LogDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "logs");

        public static string CurrentLogFile => Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");

        /// <summary>
        /// Writes the entry and returns the log file path. Never throws, logging must not cause new errors.
        /// </summary>
        public static string Error(string context, Exception exception = null)
        {
            var path = CurrentLogFile;

            try
            {
                lock (s_lock)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(path,
                        $"[{DateTime.Now:HH:mm:ss}] {context}{Environment.NewLine}" +
                        (exception != null ? exception + Environment.NewLine : "") +
                        Environment.NewLine);
                }
            }
            catch (Exception)
            {
                // Nothing sensible left to do
            }

            return path;
        }
    }
}
