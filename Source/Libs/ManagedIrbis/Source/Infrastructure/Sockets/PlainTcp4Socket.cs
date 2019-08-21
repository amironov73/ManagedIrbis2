// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PlainTcp4Socket.cs -- plain BSD-socket for TCP v4
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using AM;

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Простой BSD-сокет TCP/IP v4.
    /// </summary>
    public sealed class PlainTcp4Socket
        : ClientSocket
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlainTcp4Socket
            (
                [NotNull] IrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region IrbisSocket members

        /// <inheritdoc cref="ClientSocket.TransactAsync"/>
        public override async Task<ServerResponse?> TransactAsync
            (
                ClientQuery query
            )
        {
            Connection.Cancellation.ThrowIfCancellationRequested();

            using var client = new TcpClient();
            try
            {
                await client.ConnectAsync(Connection.Host, Connection.Port);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            var socket = client.Client;
            var stream = client.GetStream();

            var length = query.GetLength();
            var prefix = Encoding.ASCII.GetBytes
                (
                    length.ToInvariantString() + "\n"
                );
            var chunks = query.GetChunks();
            chunks[0] = prefix;
            try
            {
                foreach (var chunk in chunks)
                {
                    Connection.Cancellation.ThrowIfCancellationRequested();
                    await stream.WriteAsync(chunk, Connection.Cancellation);
                }

                // await stream.FlushAsync();
                socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            var result = new ServerResponse();
            try
            {
                Connection.Cancellation.ThrowIfCancellationRequested();
                await result.PullDataAsync
                    (
                        stream,
                        2048,
                        Connection.Cancellation
                    );
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            return result;
        } // method TransactAsync

        /// <inheritdoc cref="ClientSocket.Transact"/>
        public override ServerResponse? Transact
            (
                ClientQuery query
            )
        {
            Connection.Cancellation.ThrowIfCancellationRequested();

            using var client = new TcpClient();
            try
            {
                client.Connect(Connection.Host, Connection.Port);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            var socket = client.Client;
            var stream = client.GetStream();

            var length = query.GetLength();
            var prefix = Encoding.ASCII.GetBytes
                (
                    length.ToInvariantString() + "\n"
                );
            var chunks = query.GetChunks();
            chunks[0] = prefix;
            try
            {
                foreach (var chunk in chunks)
                {
                    if (Connection.Cancellation.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    stream.Write(chunk);
                }

                // await stream.FlushAsync();
                socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            var result = new ServerResponse();
            try
            {
                if (Connection.Cancellation.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                result.PullData
                (
                    stream,
                    2048
                );
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            return result;
        } // method Transact

        #endregion

    } // class PlainTcp4Socket
}
