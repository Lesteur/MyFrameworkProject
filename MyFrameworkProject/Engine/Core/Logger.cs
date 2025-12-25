using System;

namespace MyFrameworkProject.Engine.Core
{
    /// <summary>
    /// Provides a simple logging system for outputting categorized messages to the console.
    /// Supports three log levels: Info, Warning, and Error.
    /// This is a static utility class intended for debugging and diagnostic purposes.
    /// </summary>
    public static class Logger
    {
        #region Public Methods - Logging

        /// <summary>
        /// Logs an informational message to the console.
        /// Use this for general information about application flow and state.
        /// </summary>
        /// <param name="message">The informational message to log.</param>
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// Use this for potentially problematic situations that don't prevent execution.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warning(string message)
        {
            Write("WARN", message);
        }

        /// <summary>
        /// Logs an error message to the console.
        /// Use this for error conditions and exceptions that require attention.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            Write("ERROR", message);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes a formatted log message to the console with the specified level prefix.
        /// Output format: [LEVEL] message
        /// </summary>
        /// <param name="level">The log level prefix (e.g., "INFO", "WARN", "ERROR").</param>
        /// <param name="message">The message to log.</param>
        private static void Write(string level, string message)
        {
            Console.WriteLine($"[{level}] {message}");
        }

        #endregion
    }
}