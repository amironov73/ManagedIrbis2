// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ScopedLock.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Threading;

using JetBrains.Annotations;

#endregion

namespace AM.Threading
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class ScopedLock
        : IDisposable
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ScopedLock
            (
                Semaphore semaphore
            )
        {
            _semaphore = semaphore;
            _semaphore.WaitOne();
        }

        #endregion

        #region Private members

        private readonly Semaphore _semaphore;

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            _semaphore.Release();
        }

        #endregion
    }
}
