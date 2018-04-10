// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SimpleClientSocket.cs -- naive client socket implementation
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using AM;
using AM.Net;
using AM.Threading;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Naive client socket implementation.
    /// </summary>
    [PublicAPI]
    public sealed class SimpleClientSocket
        : ClientSocket
    {
        #region Private members

        private IPAddress _address;

        private void _ResolveHostAddress
            (
                [NotNull] string host
            )
        {
            Sure.NotNullNorEmpty(host, nameof(host));

            if (ReferenceEquals(_address, null))
            {
                _address = SocketUtility.ResolveAddressIPv4(host);
            }
        }

        private TcpClient _GetTcpClient
            (
                int port
            )
        {
            TcpClient result = new TcpClient();

            // TODO some setup?
            result.Connect(_address, port);

            return result;
        }

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

            IIrbisConnection connection = context.Connection;
            _ResolveHostAddress(connection.Host);

            using (new BusyGuard(Busy))
            {
                using (TcpClient tcp = _GetTcpClient(connection.Port))
                {
                    Socket socket = tcp.Client;
                    socket.Send(context.RawQuery);

                    ExecutionEngine engine = connection.Executive;
                    MemoryStream stream = engine.GetMemoryStream(GetType());
                    context.RawResponse = socket.ReceiveToEnd(stream);
                    engine.ReportMemoryUsage
                        (
                            consumer: GetType(),
                            memoryUsage: context.RawResponse.Length
                        );
                }
            }
        }

        #endregion
    }
}
