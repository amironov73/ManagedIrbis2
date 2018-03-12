﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AbstractCommand.cs -- abstract command of IRBIS protocol
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Logging;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Infrastructure.Commands
{
    /// <summary>
    /// Abstract command of IRBIS protocol.
    /// </summary>
    [PublicAPI]
    public abstract class AbstractCommand
        : IVerifiable
    {
        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        /// <summary>
        /// Good return codes.
        /// </summary>
        public virtual int[] GoodReturnCodes
        {
            get
            {
                return new int[0];
            }
        }

        /// <summary>
        /// Relax (may be malformed) server response.
        /// </summary>
        public bool RelaxResponse { get; set; }

        /// <summary>
        /// Does the command require established connection?
        /// </summary>
        public virtual bool RequireConnection { get { return true; } }

        /// <summary>
        /// Kind of the command.
        /// </summary>
        public virtual CommandKind Kind { get { return CommandKind.None; } }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractCommand
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Log.Trace(nameof(AbstractCommand) + "::Constructor");

            Connection = connection;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Check the server response.
        /// </summary>
        public virtual void CheckResponse
            (
                [NotNull] ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            int returnCode = response.ReturnCode;
            if (returnCode < 0)
            {
                int[] goodCodes = GoodReturnCodes;

                if (!goodCodes.Contains(returnCode))
                {
                    Log.Error
                        (
                            nameof(AbstractCommand) + "::" + nameof(CheckResponse)
                            + ": code="
                            + returnCode
                        );

                    throw new IrbisException(returnCode);
                }
            }
        }

        /// <summary>
        /// Create client query.
        /// </summary>
        public virtual ClientQuery CreateQuery()
        {
            Log.Trace("AbstractCommand::CreateQuery");

            // TODO fix it!

            ClientQuery result = new ClientQuery(Connection)
            {
                Workstation = Connection.Workstation,
                ClientID = Connection.ClientID,
                CommandNumber = 1,
                UserLogin = Connection.Username,
                UserPassword = Connection.Password
            };
            IrbisConnection connection = Connection as IrbisConnection;
            if (!ReferenceEquals(connection, null))
            {
                result.CommandNumber = connection.IncrementCommandNumber();
            }

            return result;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        [NotNull]
        public virtual ServerResponse Execute
            (
                [NotNull] ClientQuery query
            )
        {
            Sure.NotNull(query, nameof(query));

            Log.Trace("AbstractCommand::Execute");

            byte[] request = query.EncodePacket();
            byte[] answer = Connection.Socket
                .ExecuteRequest(request);

            Log.Trace
                (
                    "AbstractCommand::Execute: answer.Length="
                    + answer.Length
                );

            ServerResponse result = new ServerResponse
                (
                    Connection,
                    answer,
                    request,
                    RelaxResponse
                );

            return result;
        }

        /// <summary>
        /// Parse client query.
        /// </summary>
        public virtual void ParseClientQuery
            (
                [NotNull] byte[] clientQuery
            )
        {
            Sure.NotNull(clientQuery, nameof(clientQuery));
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public virtual bool Verify
            (
                bool throwOnError
            )
        {
            return true;
        }

        #endregion
    }
}
