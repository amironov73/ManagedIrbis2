// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BrokenSocket.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Сокет, время от времени выдающий ошибку связи
    /// (для тестирования).
    /// </summary>
    [PublicAPI]
    public sealed class BrokenSocket
        : ClientSocket
    {
        #region Constants

        /// <summary>
        /// Default value for <see cref="Probability"/> property.
        /// </summary>
        public const double DefaultProbability = 0.07;

        #endregion

        #region Properties

        /// <summary>
        /// Probability of error event.
        /// </summary>
        public double Probability { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public BrokenSocket
            (
                [NotNull] ClientSocket innerSocket
            )
        {
            Sure.NotNull(innerSocket, nameof(innerSocket));

            Probability = DefaultProbability;
            InnerSocket = innerSocket;

            _random = new Random();
        }

        #endregion

        #region Private members

        private readonly Random _random;

        #endregion

        #region ClientSocket members

        /// <inheritdoc cref="ClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            InnerSocket.ThrowIfNull(nameof(InnerSocket)).AbortRequest();
        }

        /// <inheritdoc cref="ClientSocket.ExecuteRequest" />
        public override void ExecuteRequest
            (
                ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            double probability = Probability;
            if (probability > 0.0
                && probability < 1.0)
            {
                double value = _random.NextDouble();
                if (value < probability)
                {
                    throw new IrbisNetworkException(Resources.BrokenNetworkEvent);
                }
            }

            InnerSocket
                .ThrowIfNull(nameof(InnerSocket))
                .ExecuteRequest(context);
        }

        #endregion
    }
}
