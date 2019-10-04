// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BusyGuard.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM.Logging;

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo

namespace AM.Threading
{
    /// <summary>
    /// Обёртка для ожидания и освобождения <see cref="BusyState"/>.
    /// </summary>
    [PublicAPI]
    public struct BusyGuard
        : IDisposable
    {
        #region Properties

        /// <summary>
        /// State.
        /// </summary>
        public BusyState State { get; }

        /// <summary>
        /// Timeout.
        /// </summary>
        public TimeSpan Timeout { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public BusyGuard
            (
                BusyState state
            )
        {
            State = state;
            Timeout = TimeSpan.Zero;

            _Grab();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BusyGuard
            (
                BusyState state,
                TimeSpan timeout
            )
        {
            State = state;
            Timeout = timeout;

            _Grab();
        }

        #endregion

        #region Private members

        private void _Grab()
        {
            if (Timeout.IsZeroOrLess())
            {
                State.WaitAndGrab();
            }
            else
            {
                if (!State.WaitAndGrab(Timeout))
                {
                    Log.Error
                        (
                            nameof(BusyGuard)
                            + "::"
                            + nameof(_Grab)
                            + ": "
                            + "timeout"
                        );

                    throw new TimeoutException();
                }
            }
        }

        #endregion

        #region Public methods

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            State.SetState(false);
        }

        #endregion
    }
}
