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

        // TODO remove RelaxResponse

        /// <summary>
        /// Relax (may be malformed) server response.
        /// </summary>
        public bool RelaxResponse { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Check the server response.
        /// </summary>
        protected void CheckResponse
            (
                [NotNull] ServerResponse response,
                params int[] goodCodes
            )
        {
            Sure.NotNull(response, nameof(response));

            int returnCode = response.ReturnCode;
            if (returnCode < 0)
            {
                if (!goodCodes.Contains(returnCode))
                {
                    Log.Error
                        (
                            nameof(ClientCommand) + "::" + nameof(CheckResponse)
                            + ": code="
                            + returnCode
                        );

                    throw new IrbisException(returnCode);
                }
            }
        }

        ///// <summary>
        ///// Create client query.
        ///// </summary>
        //public ClientQuery CreateQuery
        //    (
        //        [NotNull] IIrbisConnection connection,
        //        [NotNull] string commandCode
        //    )
        //{
        //    Sure.NotNull(connection, nameof(connection));
        //    Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

        //    Log.Trace(nameof(ClientCommand) + "::" + nameof(CreateQuery));

        //    ClientQuery result = new ClientQuery(commandCode)
        //    {
        //        Workstation = connection.Workstation,
        //        ClientID = connection.ClientID,
        //        CommandNumber = 1,
        //        UserLogin = connection.Username,
        //        UserPassword = connection.Password
        //    };

        //    //if (Connection is IrbisConnection connection)
        //    //{
        //    //    result.CommandNumber = connection.IncrementCommandNumber();
        //    //}

        //    return result;
        //}

        /// <summary>
        /// Create client query.
        /// </summary>
        public ClientQuery CreateQuery
            (
                [NotNull] ClientContext context,
                [NotNull] string commandCode
            )
        {
            Sure.NotNull(context, nameof(context));
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            Log.Trace(nameof(ClientCommand) + "::" + nameof(CreateQuery));

            IIrbisConnection connection = context.Connection;
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

            context.Query = result;

            return result;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        public abstract void Execute
            (
                [NotNull] ClientContext context
            );

        /// <summary>
        /// Execute the query.
        /// </summary>
        protected ServerResponse BaseExecute
            (
                [NotNull] ClientContext context
            )
        {
            ClientQuery query = context.Query.ThrowIfNull(nameof(context.Query));
            byte[] request = query.EncodePacket();
            context.RawQuery = request;
            IIrbisConnection connection = context.Connection;
            connection.Socket.ExecuteRequest(context);
            byte[] answer = context.RawResponse.ThrowIfNull(nameof(context.RawResponse));

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
            context.Response = result;

            return result;
        }

        #endregion
    }
}
