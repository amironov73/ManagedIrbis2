// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ExceptionEventArgsT.cs -- information about exception
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Information about exception.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("{Exception} {Handled}")]
    public sealed class ExceptionEventArgs<T>
        : EventArgs
        where T: Exception
    {
        #region Properties

        /// <summary>
        /// Exception.
        /// </summary>
        [NotNull]
        public T Exception { get; }

        /// <summary>
        /// Handled?
        /// </summary>
        public bool Handled { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExceptionEventArgs
            (
                [NotNull] T exception
            )
        {
            Sure.NotNull(exception, nameof(exception));

            Exception = exception;
        }

        #endregion
    }
}
