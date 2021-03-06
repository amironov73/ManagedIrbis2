﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisNetworkException.cs -- exception during IRBIS network communication
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
    /// Exception during IRBIS network communication.
    /// </summary>
    [PublicAPI]
    public sealed class IrbisNetworkException
        : ArsMagnaException
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisNetworkException()
        {
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisNetworkException
            (
                string message
            )
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisNetworkException
            (
                string message,
                Exception innerException
            )
            : base(message, innerException)
        {
        }
    }
}
