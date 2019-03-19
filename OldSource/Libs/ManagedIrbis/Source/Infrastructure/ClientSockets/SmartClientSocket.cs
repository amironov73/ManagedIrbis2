// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SmartClientSocket.cs -- minimizes memory reallocation
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Net;
using System.Net.Sockets;

using AM;
using AM.IO;
using AM.Logging;
using AM.Net;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Client socket that minimizes memory reallocation.
    /// </summary>
    [PublicAPI]
    public sealed class SmartClientSocket
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

            if (ReferenceEquals(_address, null))
            {
                throw new IrbisNetworkException
                    (
                        Resources.CantResolveHost + host
                    );
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

        private static byte[] _SmartRead
            (
                [NotNull] NetworkStream stream
            )
        {
            byte[] head = new byte[10 * 1024];
            byte[] body, result;

            int readed1 = stream.Read(head, 0, head.Length);
            if (readed1 == 0)
            {
                Log.Error
                    (
                        nameof(SmartClientSocket) + "::" + nameof(_SmartRead)
                        + Resources.SmartRead_EmptyResponse
                    );

                throw new IrbisNetworkException(Resources.EmptyResponse);
            }

            // Ожидаемый ответ сервера:
            //
            // Команда
            // Идентификатор клиента
            // Порядковый номер
            // Длина ответа
            // Прочие данные

            ByteNavigator navigator = new ByteNavigator(head);
            navigator.SkipLine();
            navigator.SkipLine();
            navigator.SkipLine();

            string text = navigator.ReadLine();
            if (ReferenceEquals(text, null))
            {
                Log.Error
                    (
                        nameof(SmartClientSocket) + "::" + nameof(_SmartRead)
                        + Resources.CantReadFirstLineOfTheResponse
                    );

                return head;
            }

            if (!NumericUtility.TryParseInt32(text, out int length))
            {
                if (readed1 < head.Length)
                {
                    return head.GetSpan(0, readed1);
                }
                body = stream.ReadToEnd();

                result = ArrayUtility.Merge(head, body);

                return result;
            }

            int remaining = length + text.Length - readed1;
            if (remaining <= 0)
            {
                return head.GetSpan(0, readed1);
            }

            body = new byte[remaining];
            int readed2 = stream.Read(body, 0, remaining);
            if (readed2 != remaining)
            {
                Log.Error
                    (
                        nameof(SmartClientSocket) + "::" + nameof(_SmartRead)
                        + Resources.SmartRead_Expected
                        + remaining
                        + Resources.SmartRead_Readed
                        + readed2
                    );

                throw new IrbisNetworkException();
            }

            result = ArrayUtility.Merge(head, body);

            return result;
        }

        #endregion

        #region ClientSocket members

        /// <inheritdoc cref="ClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send request to server and receive answer.
        /// </summary>
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
                    foreach (byte[] bytes in context.RawQuery)
                    {
                        socket.Send(bytes);
                    }

                    NetworkStream stream = tcp.GetStream();
                    context.RawResponse = _SmartRead(stream);
                }
            }
        }

        #endregion
    }
}
