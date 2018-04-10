﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;
            ClientQuery query = CreateQuery(connection, CommandCode.ReadRecord);
            query.Arguments.Add(context.GetDatabase(Database));
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

            ServerResponse result = BaseExecute(context);

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
        }

        ///// <inheritdoc cref="ClientCommand.GoodReturnCodes"/>
        //public override int[] GoodReturnCodes
        //{
        //    // Record can be logically deleted
        //    // or blocked. It's normal.
        //    get { return new[] { -201, -600, -602, -603 }; }
        //}

        #endregion
    }
}
