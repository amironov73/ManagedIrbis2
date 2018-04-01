// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SimpleClientSocket.cs -- naive client socket implementation
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

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
        : AbstractClientSocket
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimpleClientSocket
            (
                [NotNull] IrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

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

        private TcpClient _GetTcpClient()
        {
            TcpClient result = new TcpClient();

            // TODO some setup?

            result.Connect(_address, Connection.Port);

            return result;
        }

        #endregion

        #region AbstractClientSocket members

        /// <inheritdoc cref="AbstractClientSocket.AbortRequest"/>
        public override void AbortRequest()
        {
            // TODO do something?
        }

        /// <inheritdoc cref="AbstractClientSocket.ExecuteRequest"/>
        public override byte[] ExecuteRequest
            (
                byte[] request
            )
        {
            Sure.NotNull(request, nameof(request));

            IrbisConnection connection = Connection as IrbisConnection;
            if (!ReferenceEquals(connection, null))
            {
                connection.RawClientRequest = request;
            }

            _ResolveHostAddress(Connection.Host);

            using (new BusyGuard(Busy))
            {
                using (TcpClient client = _GetTcpClient())
                {
                    Socket socket = client.Client;
                    socket.Send(request);

                    MemoryStream stream = Connection.Executive
                        .GetMemoryStream(GetType());
                    byte[] result = socket.ReceiveToEnd(stream);
                    Connection.Executive.ReportMemoryUsage
                        (
                            consumer: GetType(),
                            memoryUsage: result.Length
                        );

                    if (!ReferenceEquals(connection, null))
                    {
                        connection.RawServerResponse = result;
                    }

                    return result;
                }
            }
        }

        #endregion
    }
}
