﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ClientSocketUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class ClientSocketUtility
    {
        #region Public methods

        /// <summary>
        /// Create socket of the given type
        /// and add it to the socket chain
        /// of the connection.
        /// </summary>
        [NotNull]
        public static ClientSocket CreateSocket
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string typeName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(typeName, nameof(typeName));

            Type socketType = Type.GetType(typeName, true)
                .ThrowIfNull(nameof(Type.GetType));
            ClientSocket result
                = (ClientSocket)Activator.CreateInstance
                (
                    type: socketType,
                    args: connection
                );
            connection.SetSocket(result);

            return result;
        }

        /// <summary>
        /// Create socket of the given type
        /// and add it to the socket chain
        /// of the connection.
        /// </summary>
        [NotNull]
        public static T CreateSocket<T>
            (
                [NotNull] IrbisConnection connection
            )
            where T: ClientSocket
        {
            Sure.NotNull(connection, nameof(connection));

            Type socketType = typeof(T);
            T result = (T)Activator.CreateInstance
                (
                    type: socketType,
                    args: connection
                );
            connection.SetSocket(result);

            return result;
        }

        /// <summary>
        /// Find given socket type in the socket chain.
        /// </summary>
        [CanBeNull]
        public static T FindSocket<T>
            (
                [NotNull] IrbisConnection connection
            )
            where T: ClientSocket
        {
            Sure.NotNull(connection, nameof(connection));

            T result = null;
            for (
                    ClientSocket socket = connection.Socket;
                    !ReferenceEquals(socket, null);
                    socket = socket.InnerSocket
                )
            {
                if (socket is T temp)
                {
                    result = temp;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Find or create (add to socket chain)
        /// given socket type.
        /// </summary>
        [NotNull]
        public static T FindOrCreateSocket<T>
            (
                [NotNull] IrbisConnection connection
            )
            where T: ClientSocket
        {
            Sure.NotNull(connection, nameof(connection));

            T result = FindSocket<T>(connection)
                ?? CreateSocket<T>(connection);

            return result;
        }

        /// <summary>
        /// Remove given socket from the socket chain.
        /// </summary>
        public static void RemoveSocket
            (
                [NotNull] IrbisConnection connection,
                [NotNull] ClientSocket socket
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(socket, nameof(socket));

            ClientSocket inner = socket.InnerSocket;

            if (ReferenceEquals(connection.Socket, socket))
            {
                inner = inner.ThrowIfNull(nameof(socket.InnerSocket));
                connection.SetSocket(inner);
            }
            else
            {
                for (
                        ClientSocket current = connection.Socket;
                        !ReferenceEquals(current, null);
                    )
                {
                    inner = current.InnerSocket;

                    if (ReferenceEquals(inner, socket))
                    {
                        current.InnerSocket = inner.InnerSocket;
                        break;
                    }

                    current = inner;
                }
            }
        }

        #endregion
    }
}
