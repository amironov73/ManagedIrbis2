﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DebugLogger.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using SysDebug=System.Diagnostics.Debug;

#endregion

namespace AM.Logging
{
    /// <summary>
    /// <see cref="IAmLogger"/> using
    /// <see cref="SysDebug.WriteLine(string)"/>
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public sealed class DebugLogger
        : IAmLogger
    {
        #region IAmLogger members

        /// <inheritdoc cref="IAmLogger.Debug" />
        public void Debug
            (
                string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                SysDebug.WriteLine(text, "Debug");
            }
        }

        /// <inheritdoc cref="IAmLogger.Error" />
        public void Error
            (
                string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                SysDebug.WriteLine(text, "Error");
            }
        }

        /// <inheritdoc cref="IAmLogger.Fatal" />
        public void Fatal
            (
                string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                SysDebug.WriteLine(text, "Fatal");
            }
        }

        /// <inheritdoc cref="IAmLogger.Info" />
        public void Info
            (
                string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                SysDebug.WriteLine(text, "Info");
            }
        }

        /// <inheritdoc cref="IAmLogger.Trace" />
        public void Trace
            (
                string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                SysDebug.WriteLine(text, "Trace");
            }
        }

        /// <inheritdoc cref="IAmLogger.Warn" />
        public void Warn
            (
                string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                SysDebug.WriteLine(text, "Warn");
            }
        }

        #endregion
    }
}
