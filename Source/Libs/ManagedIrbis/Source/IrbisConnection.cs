// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisConnection.cs -- client for IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Sockets;
using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Подключение к серверу ИРБИС64.
    /// </summary>
    [PublicAPI]
    public sealed class IrbisConnection
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; } = 6666;

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Database { get; set; } = "IBIS";

        /// <summary>
        /// 
        /// </summary>
        public string Workstation { get; set; } = "C";

        /// <summary>
        /// 
        /// </summary>
        public int ClientId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int QueryId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ServerVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object IniFile { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Interval { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Connected { get; private set; }

        #endregion

        #region Private members

        private bool _debug = false;

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> Connect()
        {
            if (Connected)
            {
                return true;
            }

        AGAIN:
            ClientId = new Random().Next(100000, 999999);
            QueryId = 1;
            var query = new ClientQuery(this, "A");
            query.AddAnsi(Username).NewLine();
            query.AddAnsi(Password);

            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            if (response.GetReturnCode() == -3337)
            {
                goto AGAIN;
            }

            if (response.ReturnCode < 0)
            {
                return false;
            }

            Connected = true;
            ServerVersion = response.ServerVersion;
            Interval = response.ReadInteger();
            // TODO Read INI-file

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> Disconnect()
        {
            if (!Connected)
            {
                return true;
            }

            var query = new ClientQuery(this, "B");
            query.AddAnsi(Username);
            try
            {
                await Execute(query);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            Connected = false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ServerResponse> Execute(ClientQuery query)
        {
            IrbisSocket socket = new PlainTcp4Socket(Host, Port);
            ServerResponse result;
            try
            {
                if (_debug)
                {
                    query.Debug(Console.Out);
                }

                result = await socket.Transact(query);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            if (ReferenceEquals(result, null))
            {
                return null;
            }

            if (_debug)
            {
                result.Debug(Console.Out);
            }

            result.Parse();
            QueryId++;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ParseConnectionString
            (
                [CanBeNull] string connectionString
            )
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }

            var pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                if (!pair.Contains('='))
                {
                    continue;
                }

                var parts = pair.Split('=', 2);
                var name = parts[0].Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var value = parts[1].Trim();

                switch (name)
                {
                    case "host":
                    case "server":
                    case "address":
                        Host = value;
                        break;

                    case "port":
                        Port = NumericUtility.ParseInt32(value);
                        break;

                    case "user":
                    case "username":
                    case "name":
                    case "login":
                    case "account":
                        Username = value;
                        break;

                    case "password":
                    case "pwd":
                    case "secret":
                        Password = value;
                        break;

                    case "db":
                    case "database":
                    case "base":
                    case "catalog":
                        Database = value;
                        break;

                    case "arm":
                    case "workstation":
                        Workstation = value;
                        break;

                    case "debug":
                        _debug = true;
                        break;

                    default:
                        throw new IrbisException($"Unknown key {name}");
                }
            }
        }

        #endregion
    }
}
