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

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PlainTcp4Socket
        : IrbisSocket
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlainTcp4Socket(string host, int port)
        {
            Host = host;
            Port = port;
        }

        #endregion

        #region IrbisSocket members

        /// <summary>
        /// 
        /// </summary>
        public async override Task<ServerResponse> Transact(ClientQuery query)
        {
            using (var client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(Host, Port);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return null;
                }

                var socket = client.Client;
                var stream = client.GetStream();

                var length = query.GetLength();
                var prefix = Encoding.ASCII.GetBytes($"{length}\n");
                var chunks = query.GetChunks();
                chunks[0] = prefix;
                try
                {
                    foreach (var chunk in chunks)
                    {
                        await stream.WriteAsync(chunk);
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
                    await result.CopyFromAsync(stream, 2048);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return null;
                }

                // result.Debug(Out);

                return result;
            }

            #endregion
        }
    }
}
