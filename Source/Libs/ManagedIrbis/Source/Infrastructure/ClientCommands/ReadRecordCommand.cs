// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadRecordCommand.cs -- read one record from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Read one record from the server.
    /// </summary>
    [PublicAPI]
    public sealed class ReadRecordCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Throw <see cref="IrbisNetworkException"/>
        /// when empty record received/decoded.
        /// </summary>
        public static bool ThrowOnEmptyRecord { get; set; } = true;

        /// <summary>
        /// Throw <see cref="VerificationException"/>
        /// when bad record received/decoded.
        /// </summary>
        public static bool ThrowOnVerify { get; set; } = true;

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Format.
        /// </summary>
        [CanBeNull]
        public string Format { get; set; }

        /// <summary>
        /// Need lock?
        /// </summary>
        public bool Lock { get; set; }

        /// <summary>
        /// MFN.
        /// </summary>
        public int Mfn { get; set; }

        /// <summary>
        /// Version.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Readed record.
        /// </summary>
        [CanBeNull]
        public MarcRecord Record { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadRecordCommand()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadRecordCommand
            (
                [NotNull] string database,
                int mfn
            )
        {
            Sure.NotNull(database, nameof(database));
            Sure.Positive(mfn, nameof(mfn));

            Mfn = mfn;
            Database = database;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadRecordCommand
            (
                int mfn
            )
        {
            Sure.Positive(mfn, nameof(mfn));

            Mfn = mfn;
        }

        #endregion

        #region Private members

        // Good codes for CheckResponse():
        // ERR_OLDREC_ABSENT = -201;
        // ERR_RECLOCKED = -602;
        // REC_DELETE = -603, -600;
        // REC_PHYS_DELETE = -605;
        private static int[] _goodCodes = { -201, -600, -602, -603, -605 };

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;
            ClientQuery query = CreateQuery(context, CommandCode.ReadRecord);
            query.AddAnsi(context.GetDatabase(Database));
            query.Add(Mfn);
            if (VersionNumber != 0)
            {
                query.Add(VersionNumber);
            }
            else
            {
                query.Add(Lock);
            }
            if (!string.IsNullOrEmpty(Format))
            {
                query.Add(Format);
            }

            ServerResponse response = BaseExecute(context);
            CheckResponse(response, _goodCodes);

            if (response.GetReturnCode() != -201)
            {
                MarcRecord record = new MarcRecord
                {
                    HostName = connection.Host,
                    Database = Database
                };

                record = ProtocolText.ParseResponseForReadRecord
                    (
                        response,
                        record
                    );
                if (ThrowOnVerify)
                {
                    record.Verify(ThrowOnVerify);
                }

                if (ThrowOnEmptyRecord)
                {
                    IrbisNetworkUtility.ThrowIfEmptyRecord
                        (
                            record,
                            response
                        );
                }

                Record = record;
            }
        }

        #endregion
    }
}
