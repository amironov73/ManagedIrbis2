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

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.UpdateRecord);

            if (ReferenceEquals(Record, null))
            {
                Log.Error
                    (
                        nameof(WriteRecordCommand) + "::" + nameof(Execute)
                        + ": " + Resources.IrbisNetworkUtility_RecordIsNull
                    );

                throw new IrbisNetworkException(Resources.IrbisNetworkUtility_RecordIsNull);
            }

            string database = context.GetDatabase(Record.Database);
            query
                .Add(database)
                .Add(Lock)
                .Add(Actualize)
                .Add(Record);

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
            MaxMfn = response.ReturnCode;

            MarcRecord record = Record.ThrowIfNull("Record");

            record.Database = database;
            record.HostName = context.Connection.Host;

            if (!DontParseResponse)
            {
                ProtocolText.ParseResponseForWriteRecord(response, record);
            }
        }

        #endregion
    }
}
