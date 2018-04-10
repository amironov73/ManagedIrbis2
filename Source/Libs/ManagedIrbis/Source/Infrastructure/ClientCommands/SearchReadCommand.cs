// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SearchReadCommand.cs -- search and read records from IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;
using System.Linq;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;
using ManagedIrbis.Search;

#endregion

// ReSharper disable ConvertClosureToMethodGroup

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Search and read records from IRBIS-server.
    /// </summary>
    [PublicAPI]
    public sealed class SearchReadCommand
        : SearchCommand
    {
        #region Properties

        /// <summary>
        /// Use Read command instead.
        /// </summary>
        /// <remarks>
        /// Дело в том, что методу SearchRead ИРБИС-сервер
        /// частенько возвращает «немодифицируемые» записи
        /// (он банально неправильно проставляет флаги
        /// и версию записи, а потом саботирует сохранение
        /// изменений в них). При считывании записи методом
        /// ReadRecord приходят гарантировано «модифицируемые»
        /// записи.
        /// (из переписки с пользователями)
        /// </remarks>
        public static bool UseReadInsteadOfFormat { get; set; }

        /// <summary>
        /// Format specification (always ALL).
        /// </summary>
        public override string FormatSpecification
        {
            get => UseReadInsteadOfFormat ? null : IrbisFormat.All;
            set
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Read records.
        /// </summary>
        [CanBeNull]
        public MarcRecord[] Records { get; set; }

        #endregion

        #region Private members

        private MarcRecord _ConvertRecord
            (
                IIrbisConnection connection,
                FoundItem item
            )
        {
            MarcRecord result = new MarcRecord
            {
                HostName = connection.Host,
                Database = Database ?? connection.Database
            };
            ProtocolText.ParseResponseForAllFormat
                (
                    item.Text.ThrowIfNull("item.Text"),
                    result
                );
            Debug.Assert
                (
                    item.Mfn == result.Mfn,
                    "item.Mfn == result.Mfn"
                );

            return result;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            string database = context.GetDatabase(Database);
            IIrbisConnection connection = context.Connection;
            base.Execute(context);
            ServerResponse response = context.Response.ThrowIfNull(nameof(context.Response));

            if (response.ReturnCode == 0)
            {
                if (UseReadInsteadOfFormat)
                {
                    int[] mfns = FoundItem.ConvertToMfn
                        (
                            Found.ThrowIfNull(nameof(Found))
                        );
                    Records = connection.ReadRecords(database, mfns);
                }
                else
                {
                    Records = Found
                        .ThrowIfNull(nameof(Found))
                        .AsParallel()
                        .AsOrdered()
                        .Select(item => _ConvertRecord(connection, item))
                        .ToArray();
                }
            }
        }

        #endregion
    }
}
