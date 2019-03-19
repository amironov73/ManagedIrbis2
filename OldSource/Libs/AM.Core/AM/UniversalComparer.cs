// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UniversalComparer.cs -- универсальный компаратор
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Универсальный компаратор.
    /// </summary>
    [PublicAPI]
    public sealed class UniversalComparer<T>
        : IComparer<T>
    {
        #region Properties

        /// <summary>
        /// Используемый для сравнения делегат.
        /// </summary>
        [NotNull]
        public Func<T, T, int> Function { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public UniversalComparer
            (
                [NotNull] Func<T, T, int> function
            )
        {
            Sure.NotNull(function, nameof(function));

            Function = function;
        }

        #endregion

        #region IComparer<T> members

        /// <summary>
        /// Compares the specified values.
        /// </summary>
        public int Compare
            (
                T left,
                T right
            )
        {
            return Function(left, right);
        }

        #endregion
    }
}
