// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* WriteRecordCommand.cs -- create or update record
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 * TODO read raw record
 */

#region Using directives

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;
using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Create of update existing record in the database.
    /// </summary>
    [PublicAPI]
    public sealed class WriteRecordCommand
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
        /// New max MFN (result of command execution).
        /// </summary>
        public int MaxMfn { get; set; }

        /// <summary>
        /// Don't parse server response.
        /// </summary>
        public bool DontParseResponse { get; set; }

        /// <summary>
        /// Record to write.
        /// </summary>
        [CanBeNull]
        public MarcRecord Record { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public WriteRecordCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="ClientCommand.Execute()" />
        public override ServerResponse Execute()
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.UpdateRecord;

            if (ReferenceEquals(Record, null))
            {
                Log.Error
                (
                    "WriteRecordCommand::CreateQuery: "
                    + Resources.IrbisNetworkUtility_RecordIsNull
                );

                throw new IrbisNetworkException(Resources.IrbisNetworkUtility_RecordIsNull);
            }

            string database = Record.Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                (
                    "WriteRecordCommand::CreateQuery: "
                    + Resources.WriteRecordsCommand_DatabaseNotSet
                );

                throw new IrbisNetworkException(Resources.WriteRecordsCommand_DatabaseNotSet);
            }

            query
                .Add(database)
                .Add(Lock)
                .Add(Actualize)
                .Add(Record);

            ServerResponse result = Execute(query);

            MaxMfn = result.GetReturnCode();

            MarcRecord record = Record.ThrowIfNull("Record");

            record.Database = database;
            record.HostName = Connection.Host;

            if (!DontParseResponse)
            {
                ProtocolText.ParseResponseForWriteRecord
                    (
                        result,
                        record
                    );
            }

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<WriteRecordCommand> verifier
                = new Verifier<WriteRecordCommand>
                    (
                        this,
                        throwOnError
                    );

            verifier
                .NotNull(Record, "Record")
                .Assert(base.Verify(throwOnError));

            return verifier.Result;
        }

        #endregion
    }
}
