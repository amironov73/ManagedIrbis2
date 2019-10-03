// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IAmLogger.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace AM.Logging
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public interface IAmLogger
    {
        /// <summary>
        /// Debug.
        /// </summary>
        void Debug
            (
                string text
            );

        /// <summary>
        /// Error.
        /// </summary>
        void Error
            (
                string text
            );

        /// <summary>
        /// Fatal error.
        /// </summary>
        void Fatal
            (
                string text
            );

        /// <summary>
        /// Information message.
        /// </summary>
        void Info
            (
                string text
            );

        /// <summary>
        /// Trace.
        /// </summary>
        void Trace
            (
                string text
            );

        /// <summary>
        /// Warning.
        /// </summary>
        void Warn
            (
                string text
            );
    }
}
