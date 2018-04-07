// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GblCommand.cs -- global correction for virtual record
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Gbl;
using ManagedIrbis.ImportExport;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    //
    // For explicitly specified statements layout is
    // (delimiter is \x1E\x1F):
    //
    // J         // code
    // IBIS      // database
    // 0         // not used
    // !0 ADD 4000 * 'OGO!' // statements (separated)
    // 0#0 0#0 700#^aИванов^bИ. И. 701#^aПетров^bП. П. // record (separated)
    //

    //
    // For filename layout is (delimiter is \x1E\x1F):
    //
    // J         // code
    // IBIS      // database
    // 0         // not used
    // @filename // without extension
    // 0#0 0#0 700#^aИванов^bИ. И. 701#^aПетров^bП. П. // record (separated)
    //

    //
    // Answer layout is (delimiter: \x1E)
    //
    // 0
    // 0#32 700#^aИванов^bИ. И. 701#^aПетров^bП. П.
    //

    //
    // Error layout is:
    // -8888
    // FORMAT_ERROR=99-OGO!...@@@IND_ERROR=6@@@
    //

    /// <summary>
    /// Global correction for virtual record.
    /// </summary>
    [PublicAPI]
    public class GblVirtualCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Server file name for GBL.
        /// </summary>
        [CanBeNull]
        public string FileName { get; set; }

        /// <summary>
        /// Update index?
        /// </summary>
        public bool Actualize { get; set; }

        /// <summary>
        /// Statements.
        /// </summary>
        [CanBeNull]
        public GblStatement[] Statements { get; set; }

        /// <summary>
        /// Record for correction.
        /// </summary>
        [CanBeNull]
        public MarcRecord Record { get; set; }

        /// <summary>
        /// Result.
        /// </summary>
        [CanBeNull]
        public MarcRecord Result { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public GblVirtualCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="ClientCommand.Execute()"/>
        public override ServerResponse Execute()
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.CorrectVirtualRecord;

            string database = Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                (
                    "GblVirtualCommand::CreateQuery: "
                    + "database not specified"
                );

                throw new IrbisException("database not specified");
            }
            query.AddAnsi(database);

            query.Add(Actualize);

            if (!string.IsNullOrEmpty(FileName))
            {
                query.AddAnsi(FileName);
            }
            else
            {
                string statements = GblUtility.EncodeStatements
                (
                    Statements.ThrowIfNull("Statements")
                );
                query.AddUtf8(statements);
            }

            query.Add(Record.ThrowIfNull("Record"));

            ServerResponse response = base.Execute(query);
            CheckResponse(response);

            string line = response.RequireUtfString();
            Result = Record.ThrowIfNull().Clone();
            ProtocolText.ParseResponseForGblFormat
                (
                    line,
                    Result
                );

            return response;
        }

        #endregion
    }
}
