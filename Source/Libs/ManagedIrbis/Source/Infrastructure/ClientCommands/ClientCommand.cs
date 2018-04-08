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
        : IVerifiable
    {
        #region Properties

        ///// <summary>
        ///// Connection.
        ///// </summary>
        //[NotNull]
        //public IIrbisConnection Connection { get; private set; }

        ///// <summary>
        ///// Good return codes.
        ///// </summary>
        //public virtual int[] GoodReturnCodes => Array.Empty<int>();

        /// <summary>
        /// Relax (may be malformed) server response.
        /// </summary>
        public bool RelaxResponse { get; set; }

        ///// <summary>
        ///// Does the command require established connection?
        ///// </summary>
        //public virtual bool RequireConnection => true;

        ///// <summary>
        ///// Kind of the command.
        ///// </summary>
        //public virtual CommandKind Kind => CommandKind.None;

        #endregion

        #region Construction

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //protected ClientCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //{
        //    Sure.NotNull(connection, nameof(connection));

        //    Log.Trace(nameof(ClientCommand) + "::Constructor");

        //    Connection = connection;
        //}

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
        public ClientQuery CreateQuery(IIrbisConnection connection)
        {
            Log.Trace(nameof(ClientCommand) + "::" + nameof(CreateQuery));

            ClientQuery result = new ClientQuery
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
                    + answer.Length
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

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public virtual bool Verify(bool throwOnError) => true;

        #endregion
    }
}
