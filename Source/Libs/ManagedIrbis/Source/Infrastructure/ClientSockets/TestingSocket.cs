// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TestingSocket.cs -- dummy socket for unit-testing
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Dummy socket for unit-testing.
    /// </summary>
    [PublicAPI]
    public sealed class TestingSocket
        : ClientSocket
    {
        #region Properties

        /// <summary>
        /// Actual request.
        /// </summary>
        [CanBeNull]
        public byte[] ActualRequest { get; set; }

        /// <summary>
        /// Answer.
        /// </summary>
        [CanBeNull]
        public byte[] Response { get; set; }

        /// <summary>
        /// Expected request.
        /// </summary>
        [CanBeNull]
        public byte[] ExpectedRequest { get; set; }

        #endregion

        #region ClientSocket members

        /// <inheritdoc cref="ClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ClientSocket.ExecuteRequest" />
        public override void ExecuteRequest
            (
                ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            byte[] query = context.RawQuery.ThrowIfNull(nameof(context.RawQuery));
            ActualRequest = query;

            if (!ReferenceEquals(ExpectedRequest, null))
            {
                if (!ArrayUtility.Coincide
                    (
                        firstArray: ExpectedRequest,
                        firstOffset: 0,
                        secondArray: query,
                        secondOffset: 0,
                        length: query.Length
                    ))
                {
                    throw new IrbisNetworkException();
                }
            }

            byte[] answer = Response ?? throw new IrbisNetworkException();
            context.RawResponse = answer;
        }

        #endregion
    }
}
