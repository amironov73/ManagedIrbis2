// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NullConsole.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

#endregion

namespace AM.ConsoleIO
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class NullConsole
        : IConsoleDriver
    {
        #region IConsoleDriver members

        /// <inheritdoc cref="IConsoleDriver.BackgroundColor" />
        public ConsoleColor BackgroundColor { get; set; }

        /// <inheritdoc cref="IConsoleDriver.ForegroundColor" />
        public ConsoleColor ForegroundColor { get; set; }

        /// <inheritdoc cref="IConsoleDriver.KeyAvailable" />
        public bool KeyAvailable => false;

        /// <inheritdoc cref="IConsoleDriver.Title" />
        public string? Title { get; set; }

        /// <inheritdoc cref="IConsoleDriver.Clear" />
        public void Clear()
        {
            // Nothing to do here
        }

        /// <inheritdoc cref="IConsoleDriver.ReadKey" />
        public ConsoleKeyInfo ReadKey
            (
                bool intercept
            )
        {
            return new ConsoleKeyInfo();
        }

        /// <inheritdoc cref="IConsoleDriver.Read" />
        public int Read()
        {
            return -1;
        }

        /// <inheritdoc cref="IConsoleDriver.ReadLine" />
        public string? ReadLine()
        {
            return null;
        }

        /// <inheritdoc cref="IConsoleDriver.Write" />
        public void Write
            (
                string? text
            )
        {
            // Nothing to do here
        }

        /// <inheritdoc cref="IConsoleDriver.WriteLine" />
        public void WriteLine()
        {
            // Nothing to do here
        }

        #endregion
    }
}
