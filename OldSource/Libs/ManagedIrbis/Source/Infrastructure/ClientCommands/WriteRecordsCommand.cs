// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* WriteRecordCommand.cs -- create or update many records
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 *
 * TODO determine max MFN
 */

#region Using directives

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;
using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Create or update many records simultaneously.
    /// </summary>
    [PublicAPI]
    public sealed class WriteRecordsCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Need actualize?
        /// </summary>
        public bool Actualize { get; set; }

        /// <summary>
        /// Need lock?
        /// </summary>
        public bool Lock { get; set; }

        /// <summary>
        /// Records to write.
        /// </summary>
        [NotNull]
        public NonNullCollection<RecordReference> References { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public WriteRecordsCommand()
            //(
            //    [NotNull] IIrbisConnection connection
            //)
            //: base(connection)
        {
            References = new NonNullCollection<RecordReference>();
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            if (References.Count == 0)
            {
                Log.Error
                    (
                        "WriteRecordsCommand::CreateQuery: "
                        + Resources.WriteRecordsCommand_NoRecordsGiven
                    );

                throw new IrbisNetworkException(Resources.WriteRecordsCommand_NoRecordsGiven);
            }

            if (References.Count >= IrbisConstants.MaxPostings)
            {
                Log.Error
                    (
                        "WriteRecordsCommand::CreateQuery: "
                        + Resources.WriteRecordsCommand_TooManyRecords
                    );

                throw new IrbisNetworkException(Resources.WriteRecordsCommand_TooManyRecords);
            }

            ClientQuery query = CreateQuery(context, CommandCode.SaveRecordGroup);
            query
                .Add(Lock)
                .Add(Actualize);

            foreach (RecordReference reference in References)
            {
                if (ReferenceEquals(reference.Record, null))
                {
                    Log.Error
                        (
                            "WriteRecordsCommand::CreateQuery: "
                            + Resources.IrbisNetworkUtility_RecordIsNull
                        );

                    throw new IrbisException(Resources.IrbisNetworkUtility_RecordIsNull);
                }

                if (ReferenceEquals(reference.Database, null))
                {
                    reference.Database = reference.Record.Database;
                }

                if (ReferenceEquals(reference.Database, null))
                {
                    Log.Error
                    (
                        "WriteRecordsCommand::CreateQuery: "
                        + Resources.WriteRecordsCommand_DatabaseNotSet
                    );

                    throw new IrbisException(Resources.WriteRecordsCommand_DatabaseNotSet);
                }

                if (string.IsNullOrEmpty(reference.Record.Database))
                {
                    reference.Record.Database = reference.Database;
                }

                query.Add(reference);
            }

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);

            for (int i = 0; i < References.Count; i++)
            {
                ProtocolText.ParseResponseForWriteRecords
                    (
                        response,
                        References[i].Record
                    );

                References[i].Mfn = References[i].Record.Mfn;
            }
        }

        #endregion
    }
}
