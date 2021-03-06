﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* XRange.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using T = System.Int32;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>
    /// <code>
    /// foreach ( int i in new XRange ( 10, 50 ) )
    /// {
    ///  Console.WriteLine ( "Number: {0}", i );
    /// }
    /// </code>
    /// </example>
    [PublicAPI]
    public sealed class XRange
        : IEnumerable<T>
    {
        #region Properties

        /// <summary>
        /// Gets the length.
        /// </summary>
        public T Length { get; }

        /// <summary>
        /// Gets the start.
        /// </summary>
        public T Start { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public XRange
            (
                T length
            )
        {
            Start = 0;
            Length = length;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public XRange
            (
                T start,
                T length
            )
        {
            Start = start;
            Length = length;
        }

        #endregion

        #region IEnumerable<T> members

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            for (T i = 0; i < Length; i++)
            {
                yield return Start + i;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc cref="IEnumerable.GetEnumerator" />
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        #endregion
    }
}
