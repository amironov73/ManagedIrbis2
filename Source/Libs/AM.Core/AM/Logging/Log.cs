// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Log.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace AM.Logging
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class Log
    {
        #region Properties

        /// <summary>
        /// Logger.
        /// </summary>
        public static IAmLogger? Logger { get; private set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        private static readonly object SyncRoot = new object();

        #endregion

        #region Public methods

        /// <summary>
        /// Apply defaults for console application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void ApplyDefaultsForConsoleApplication()
        {
            var root = new TeeLogger();
            Logger = root;
            root.Loggers.AddRange
                (
                    new IAmLogger[]
                    {
                        new ConsoleLogger(),
                        new TraceLogger()
                    }
                );
        }

        /// <summary>
        /// Apply defaults for console application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void ApplyDefaultsForServiceApplication()
        {
            var root = new TeeLogger();
            Logger = root;
            root.Loggers.AddRange
                (
                    new IAmLogger[]
                    {
                        new TraceLogger()
                    }
                );
        }

        /// <summary>
        /// Apply defaults for console application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static void ApplyDefaultsForWindowedApplication()
        {
            var root = new TeeLogger();
            Logger = root;
            root.Loggers.AddRange
                (
                    new IAmLogger[]
                    {
                        new TraceLogger()
                    }
                );
        }

        /// <summary>
        /// Debug message.
        /// </summary>
        public static void Debug
            (
                string? text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                lock (SyncRoot)
                {
                    Logger?.Debug(text);
                }
            }
        }

        /// <summary>
        /// Error message.
        /// </summary>
        public static void Error
            (
                string? text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                lock (SyncRoot)
                {
                    Logger?.Error(text);
                }
            }
        }

        /// <summary>
        /// Fatal error message.
        /// </summary>
        public static void Fatal
            (
                string? text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                lock (SyncRoot)
                {
                    Logger?.Fatal(text);
                }
            }
        }

        /// <summary>
        /// Information message.
        /// </summary>
        public static void Info
            (
                string? text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                lock (SyncRoot)
                {
                    Logger?.Info(text);
                }
            }
        }

        /// <summary>
        /// Set new logger.
        /// </summary>
        /// <returns>Previous logger.</returns>
        public static IAmLogger? SetLogger
            (
                IAmLogger? logger
            )
        {
            lock (SyncRoot)
            {
                var result = Logger;

                Logger = logger;

                return result;
            }
        }

        /// <summary>
        /// Trace message.
        /// </summary>
        public static void Trace
            (
                string? text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                lock (SyncRoot)
                {
                    Logger?.Trace(text);
                }
            }
        }

        /// <summary>
        /// Trace the exception.
        /// </summary>
        public static void TraceException
            (
                string text,
                Exception exception
            )
        {
            var name = exception.GetType().Name;
            var fullText = $"{text}: {name}: {exception.Message}";

            Trace(fullText);
        }

        /// <summary>
        /// Warning message.
        /// </summary>
        public static void Warn
            (
                string? text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                lock (SyncRoot)
                {
                    Logger?.Warn(text);
                }
            }
        }

        #endregion
    }
}
