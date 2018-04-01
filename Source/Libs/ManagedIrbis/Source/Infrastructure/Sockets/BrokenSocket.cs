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
        : AbstractClientSocket
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
                [NotNull] IrbisConnection connection,
                [NotNull] AbstractClientSocket innerSocket
            )
            : base(connection)
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(innerSocket, nameof(innerSocket));

            Probability = DefaultProbability;
            InnerSocket = innerSocket;

            _random = new Random();
        }

        #endregion

        #region Private members

        private readonly Random _random;

        #endregion

        #region AbstractClientSocket members

        /// <inheritdoc cref="AbstractClientSocket.AbortRequest"/>
        public override void AbortRequest()
        {
            InnerSocket.ThrowIfNull().AbortRequest();
        }

        /// <inheritdoc cref="AbstractClientSocket.ExecuteRequest"/>
        public override byte[] ExecuteRequest
            (
                byte[] request
            )
        {
            Sure.NotNull(request, nameof(request));

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

            byte[] result = InnerSocket.ThrowIfNull().ExecuteRequest(request);

            return result;
        }

        #endregion
    }
}
