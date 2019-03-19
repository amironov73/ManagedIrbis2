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

using JetBrains.Annotations;

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
        public ActualizeRecordCommand()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActualizeRecordCommand
            (
                [CanBeNull] string database,
                int mfn
            )
        {
            Sure.NonNegative(mfn, nameof(mfn));

            Database = database;
            Mfn = mfn;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.ActualizeRecord);
            query.AddAnsi(context.GetDatabase(Database));
            query.Add(Mfn);
            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
