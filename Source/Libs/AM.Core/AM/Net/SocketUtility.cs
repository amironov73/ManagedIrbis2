﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SocketUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using AM.Core.Properties;
using AM.Logging;

using JetBrains.Annotations;


#endregion

namespace AM.Net
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class SocketUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Resolve IPv4 address
        /// </summary>
        /// <returns>Resolved IP address of the host.</returns>
        [NotNull]
        public static IPAddress ResolveAddressIPv4
            (
                [NotNull] string address
            )
        {
            Sure.NotNull(address, nameof(address));

            if (address.OneOf("localhost", "local", "(local)"))
            {
                return IPAddress.Loopback;
            }

            IPAddress result = null;

            try
            {
                result = IPAddress.Parse(address);
                if (result.AddressFamily != AddressFamily.InterNetwork)
                {
                    Log.Error
                        (
                            nameof(SocketUtility) + "::" + nameof(ResolveAddressIPv4)
                            + Resources.AddressMustBeIPv4ButGiven
                            + result.AddressFamily
                        );

                    throw new ArsMagnaException(Resources.AddressMustBeIPv4);
                }
            }
            catch
            {
                IPHostEntry entry = Dns.GetHostEntry(address);

                if (entry?.AddressList != null && entry.AddressList.Length != 0)
                {
                    IPAddress[] addresses = entry.AddressList
                        .Where(item => item.AddressFamily == AddressFamily.InterNetwork)
                        .ToArray();

                    if (addresses.Length == 0)
                    {
                        Log.Error
                            (
                                nameof(SocketUtility) + "::" + nameof(ResolveAddressIPv4)
                                + Resources.CantResolveIPv4Address2
                            );

                        throw new ArsMagnaException(Resources.CantResolveIPv4Address);
                    }

                    result = addresses.Length == 1
                        ? addresses[0]
                        : addresses[new Random().Next(addresses.Length)];
                }
            }

            if (ReferenceEquals(result, null))
            {
                Log.Error
                    (
                        nameof(SocketUtility) + "::" + nameof(ResolveAddressIPv4)
                        + Resources.CantResolveAddress2
                    );

                throw new ArsMagnaException(Resources.CantResolveAddress);
            }

            return result;
        }

        /// <summary>
        /// Resolve IPv6 address
        /// </summary>
        /// <returns>Resolved IP address of the host.</returns>
        [NotNull]
        public static IPAddress ResolveAddressIPv6
            (
                [NotNull] string address
            )
        {
            Sure.NotNull(address, nameof(address));

            if (address.OneOf("localhost", "local", "(local)"))
            {
                return IPAddress.IPv6Loopback;
            }

            IPAddress result = null;

            try
            {
                result = IPAddress.Parse(address);
                if (result.AddressFamily != AddressFamily.InterNetworkV6)
                {
                    Log.Error
                        (
                            nameof(SocketUtility) + "::" + nameof(ResolveAddressIPv6)
                            + Resources.AddressMustBeIPv6ButGiven
                            + result.AddressFamily
                        );

                    throw new Exception(Resources.AddressMustBeIPv6);
                }
            }
            catch
            {
                IPHostEntry entry = Dns.GetHostEntry(address);

                if (entry?.AddressList != null && entry.AddressList.Length != 0)
                {
                    IPAddress[] addresses = entry.AddressList
                        .Where(item => item.AddressFamily == AddressFamily.InterNetworkV6)
                        .ToArray();

                    if (addresses.Length == 0)
                    {
                        Log.Error
                            (
                                nameof(SocketUtility) + "::" + nameof(ResolveAddressIPv6)
                                + Resources.CantResolveIPv6Address2
                            );

                        throw new ArsMagnaException(Resources.CantResolveIPv6Address);
                    }

                    result = addresses.Length == 1
                        ? addresses[0]
                        : addresses[new Random().Next(addresses.Length)];
                }
            }

            if (ReferenceEquals(result, null))
            {
                Log.Error
                    (
                        nameof(SocketUtility) + "::" + nameof(ResolveAddressIPv6)
                        + Resources.CantResolveAddress2
                    );

                throw new ArsMagnaException(Resources.CantResolveAddress);
            }

            return result;
        }

        /// <summary>
        /// Receive specified amount of data from the socket.
        /// </summary>
        [NotNull]
        public static byte[] ReceiveExact
            (
                [NotNull] this Socket socket,
                int dataLength
            )
        {
            Sure.NotNull(socket, nameof(socket));
            Sure.NonNegative(dataLength, nameof(dataLength));

            using (MemoryStream result = new MemoryStream(dataLength))
            {
                byte[] buffer = new byte[32 * 1024];

                while (dataLength > 0)
                {
                    int readed = socket.Receive(buffer);

                    if (readed <= 0)
                    {
                        Log.Error
                            (
                                nameof(SocketUtility) + "::" + nameof(ReceiveExact)
                                + Resources.ErrorReadingSocket
                            );

                        throw new ArsMagnaException(Resources.SocketReadingError);
                    }

                    result.Write(buffer, 0, readed);

                    dataLength -= readed;
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Read from the socket as many data as possible.
        /// </summary>
        [NotNull]
        public static byte[] ReceiveToEnd
            (
                [NotNull] this Socket socket
            )
        {
            Sure.NotNull(socket, nameof(socket));

            using (MemoryStream stream = new MemoryStream())
            {
                return socket.ReceiveToEnd(stream);
            }
        }

        /// <summary>
        /// Read from the socket as many data as possible.
        /// </summary>
        [NotNull]
        public static byte[] ReceiveToEnd
            (
                [NotNull] this Socket socket,
                [NotNull] MemoryStream stream
            )
        {
            Sure.NotNull(socket, nameof(socket));
            Sure.NotNull(stream, nameof(stream));

            byte[] buffer = new byte[32 * 1024];

            while (true)
            {
                int readed = socket.Receive(buffer);

                if (readed < 0)
                {
                    Log.Error
                        (
                            nameof(SocketUtility) + "::" + nameof(ReceiveToEnd)
                            + Resources.ErrorReadingSocket
                        );

                    throw new ArsMagnaException(Resources.SocketReadingError);
                }

                if (readed == 0)
                {
                    break;
                }

                stream.Write(buffer, 0, readed);
            }

            return stream.ToArray();
        }


        #endregion
    }

}
