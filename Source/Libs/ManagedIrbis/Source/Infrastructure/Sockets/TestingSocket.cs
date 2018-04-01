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
        : AbstractClientSocket
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

        /// <inheritdoc cref="AbstractClientSocket.RequireConnection" />
        public override bool RequireConnection => false;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestingSocket
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region AbstractClientSocket members

        /// <inheritdoc cref="AbstractClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="AbstractClientSocket.ExecuteRequest" />
        public override byte[] ExecuteRequest
            (
                byte[] request
            )
        {
            Sure.NotNull(request, nameof(request));

            ActualRequest = request;

            if (!ReferenceEquals(ExpectedRequest, null))
            {
                if (!ArrayUtility.Coincide
                    (
                        firstArray: ExpectedRequest,
                        firstOffset: 0,
                        secondArray: request,
                        secondOffset: 0,
                        length: request.Length
                    ))
                {
                    throw new IrbisNetworkException();
                }
            }

            byte[] answer = Response;
            if (ReferenceEquals(answer, null))
            {
                throw new IrbisNetworkException();
            }

            return answer;
        }

        #endregion
    }
}
