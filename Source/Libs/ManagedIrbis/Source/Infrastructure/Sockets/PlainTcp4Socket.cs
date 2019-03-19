// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PlainTcp4Socket.cs --
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

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PlainTcp4Socket
        : IrbisSocket
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

        /// <summary>
        /// 
        /// </summary>
        public override async Task<ServerResponse> Transact(ClientQuery query)
        {
            if (Connection.Cancellation.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            using (var client = new TcpClient())
            {
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
                var prefix = Encoding.ASCII.GetBytes(length.ToInvariantString() + "\n");
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
                    if (Connection.Cancellation.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    await result.PullDataAsync(stream, 2048, Connection.Cancellation);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return null;
                }

                return result;
            }

            #endregion
        }
    }
}
