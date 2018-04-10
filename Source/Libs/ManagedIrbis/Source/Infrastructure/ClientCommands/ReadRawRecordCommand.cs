// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadRawRecordCommand.cs -- read one record from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Read one record from the server.
    /// </summary>
    [PublicAPI]
    public sealed class ReadRawRecordCommand
        : ClientCommand
    {
        #region Properties

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
        public RawRecord RawRecord { get; set; }

        #endregion

        #region Construction

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public ReadRawRecordCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //    : base(connection)
        //{
        //}

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
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
                string[] lines = result
                    .RemainingUtfStrings()
                    .ToArray();

                RawRecord = RawRecord.Parse(lines);
                RawRecord.Mfn = Mfn;
                RawRecord.Database = Database ?? connection.Database;
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
