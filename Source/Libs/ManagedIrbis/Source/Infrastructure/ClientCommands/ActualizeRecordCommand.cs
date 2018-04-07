// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ActualizeRecordCommand.cs -- actualize record on the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Actualize given record or whole database on the server.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("MFN={" + nameof(Mfn) + "}")]
    public class ActualizeRecordCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// MFN of record to actualize.
        /// </summary>
        /// <remarks>MFN = 0 means 'actualize whole database'.
        /// </remarks>
        public int Mfn { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="connection"></param>
        public ActualizeRecordCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.ActualizeRecord;

            string database = Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                    (
                        "ActualizeRecordCommand::CreateQuery: "
                        + Resources.IrbisNetworkUtility_DatabaseNotSpecified
                    );

                throw new IrbisException(Resources.IrbisNetworkUtility_DatabaseNotSpecified);
            }

            query.AddAnsi(database);
            query.Add(Mfn);

            ServerResponse result = Execute(query);

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="ClientCommand.Verify"/>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<ActualizeRecordCommand> verifier
                = new Verifier<ActualizeRecordCommand>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Database, "Database")
                .Assert(Mfn >= 0, "Mfn >= 0");

            return verifier.Result;
        }

        #endregion
    }
}
