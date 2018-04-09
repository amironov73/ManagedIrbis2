// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UniversalCommand.cs -- command with unfixed functionality
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Command with unfixed functionality.
    /// </summary>
    [PublicAPI]
    public sealed class UniversalCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Accept any server response.
        /// </summary>
        public bool AcceptAnyResponse { get; set; }

        /// <summary>
        /// Arguments.
        /// </summary>
        [CanBeNull]
        public object[] Arguments { get; private set; }

        /// <summary>
        /// Command code
        /// </summary>
        [NotNull]
        public string CommandCode { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public UniversalCommand
            (
                //[NotNull] IIrbisConnection connection,
                [NotNull] string commandCode
            )
            //: base(connection)
        {
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            CommandCode = commandCode;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UniversalCommand
            (
                // [NotNull] IIrbisConnection connection,
                [NotNull] string commandCode,
                params object[] arguments
            )
            : this (commandCode)
        {
            Arguments = arguments;
        }

        #endregion

        #region ClientCommand members

        ///// <summary>
        ///// Check the server response.
        ///// </summary>
        //public override void CheckResponse
        //    (
        //        ServerResponse response
        //    )
        //{
        //    if (!AcceptAnyResponse)
        //    {
        //        base.CheckResponse(response);
        //    }
        //}

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;
            ClientQuery query = CreateQuery(connection, CommandCode);
            if (!ReferenceEquals(Arguments, null))
            {
                query.Arguments.AddRange(Arguments);
            }

            ServerResponse result = Execute(connection, query);

            return result;
        }

        #endregion
    }
}
