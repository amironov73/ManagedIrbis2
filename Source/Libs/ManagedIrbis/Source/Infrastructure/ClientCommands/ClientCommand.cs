// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ClientCommand.cs -- abstract command of IRBIS protocol
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Linq;

using AM;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Abstract command of IRBIS protocol.
    /// </summary>
    [PublicAPI]
    public abstract class ClientCommand
    {
        #region Properties

        /// <summary>
        /// Relax (may be malformed) server response.
        /// </summary>
        public bool RelaxResponse { get; set; }

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
            //Sure.NotNull(response, nameof(response));

            //int returnCode = response.ReturnCode;
            //if (returnCode < 0)
            //{
            //    int[] goodCodes = GoodReturnCodes;

            //    if (!goodCodes.Contains(returnCode))
            //    {
            //        Log.Error
            //            (
            //                nameof(ClientCommand) + "::" + nameof(CheckResponse)
            //                + ": code="
            //                + returnCode
            //            );

            //        throw new IrbisException(returnCode);
            //    }
            //}
        }

        /// <summary>
        /// Create client query.
        /// </summary>
        public ClientQuery CreateQuery
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string commandCode
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            Log.Trace(nameof(ClientCommand) + "::" + nameof(CreateQuery));

            ClientQuery result = new ClientQuery(commandCode)
            {
                Workstation = connection.Workstation,
                ClientID = connection.ClientID,
                CommandNumber = 1,
                UserLogin = connection.Username,
                UserPassword = connection.Password
            };

            //if (Connection is IrbisConnection connection)
            //{
            //    result.CommandNumber = connection.IncrementCommandNumber();
            //}

            return result;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        [NotNull]
        public abstract ServerResponse Execute
            (
                [NotNull] ClientContext context
            );

        /// <summary>
        /// Execute the query.
        /// </summary>
        [NotNull]
        protected ServerResponse Execute
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] ClientQuery query
            )
        {
            query.Verify(true);

            byte[] request = query.EncodePacket();
            byte[] answer = connection.Socket.ExecuteRequest(request);

            Log.Trace
                (
                    nameof(ClientCommand) + "::" + nameof(Execute)
                    + ": answer.Length="
                    + answer.Length.ToInvariantString()
                );

            ServerResponse result = new ServerResponse
                (
                    connection,
                    answer,
                    request,
                    RelaxResponse
                );

            return result;
        }

        #endregion
    }
}
