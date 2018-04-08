// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadRecordCommand.cs -- read one record from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Logging;

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
        public static bool ThrowOnEmptyRecord { get; set; }

        /// <summary>
        /// Throw <see cref="VerificationException"/>
        /// when bad record received/decoded.
        /// </summary>
        public static bool ThrowOnVerify { get; set; }

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
        /// Static constructor.
        /// </summary>
        static ReadRecordCommand()
        {
            ThrowOnEmptyRecord = true;
            ThrowOnVerify = true;
        }

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public ReadRecordCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //    : base(connection)
        //{
        //}

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;

            ClientQuery query = CreateQuery(connection);
            query.CommandCode = CommandCode.ReadRecord;

            string database = Database ?? connection.Database;

            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                (
                    "ReadRecordCommand::CreateQuery: "
                    + "database not specified"
                );

                throw new IrbisNetworkException("database not specified");
            }

            query.Arguments.Add(database);
            query.Arguments.Add(Mfn);
            if (VersionNumber != 0)
            {
                query.Arguments.Add(VersionNumber);
            }
            else
            {
                query.Arguments.Add(Lock);
            }
            if (!string.IsNullOrEmpty(Format))
            {
                query.Arguments.Add(Format);
            }

            ServerResponse result = Execute(connection, query);

            // Check whether no records read
            if (result.GetReturnCode() != -201)
            {
                MarcRecord record = new MarcRecord
                {
                    HostName = connection.Host,
                    Database = Database
                };

                record = ProtocolText.ParseResponseForReadRecord
                    (
                        result,
                        record
                    );
                record.Verify(ThrowOnVerify);

                if (ThrowOnEmptyRecord)
                {
                    IrbisNetworkUtility.ThrowIfEmptyRecord
                        (
                            record,
                            result
                        );
                }

                Record = record;
            }

            return result;
        }

        ///// <inheritdoc cref="ClientCommand.GoodReturnCodes"/>
        //public override int[] GoodReturnCodes
        //{
        //    // Record can be logically deleted
        //    // or blocked. It's normal.
        //    get { return new[] { -201, -600, -602, -603 }; }
        //}

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            bool result = !string.IsNullOrEmpty(Database)
                && Mfn > 0;

            if (result)
            {
                result = base.Verify(throwOnError);
            }

            if (!result)
            {
                Log.Error
                    (
                        "ReadRecordCommand::Verify: "
                        + "verification failed"
                    );

                if (throwOnError)
                {
                    throw new VerificationException();
                }
            }

            return result;
        }

        #endregion
    }
}
