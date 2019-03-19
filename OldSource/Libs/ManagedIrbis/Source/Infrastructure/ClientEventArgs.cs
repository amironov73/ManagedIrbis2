// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ClientEventArgs.cs --
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
    /// Аргументы для событий, возникающих на клиенте
    /// при отправке на сервер запросов и при обработке
    /// ответов сервера.
    /// </summary>
    [PublicAPI]
    public sealed class ClientEventArgs
        : EventArgs
    {
        #region Properties

        /// <summary>
        /// Context.
        /// </summary>
        [NotNull]
        public ClientContext Context { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClientEventArgs
            (
                [NotNull] ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            Context = context;
        }

        #endregion
    }
}
