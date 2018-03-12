// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ExecutionEvenArgs.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Event arguments.
    /// </summary>
    [PublicAPI]
    public sealed class ExecutionEventArgs
        : EventArgs
    {
        #region Properties

        /// <summary>
        /// Context.
        /// </summary>
        [NotNull]
        public ExecutionContext Context { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExecutionEventArgs
            (
                [NotNull] ExecutionContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            Context = context;
        }

        #endregion
    }
}
