﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SearchRawCommand.cs -- search records on IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using AM;
using AM.Text;

using JetBrains.Annotations;

using ManagedIrbis.Search;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    //
    // EXTRACT FROM OFFICIAL DOCUMENTATION
    //
    // db_name – имя базы данных
    // search_exp – поисковое выражение на языке ISIS
    // num_records – число возвращаемых записей,
    // если параметр 0, то возвращаются
    // MAX_POSTINGS_IN_PACKET записей.
    // first_record – номер первой возвращаемой записи
    // в общем списке найденных записей если параметр
    // 0 – возвращается только количество найденных записей.
    // BRIEF – формат для форматирования найденных записей
    // format – есть 4 варианта определить формат:
    // 1-й вариант  – строка формата;
    // 2-й вариант – имя файла формата расположенного
    // на сервере по 10 пути для базы данных db_name,
    // предваряемого символом @ (например, @brief);
    // 3-й вариант – символ @ - в этом случае производится
    // ОПТИМИЗИРОВАННОЕ форматирование,
    // имя формата определяется видом записи;
    // 4-й вариант – пустая строка. В этом случае
    // форматирование не производится.
    //
    // ВОЗВРАТ
    // Список строк.
    // В 1-й строке – код возврата, который определяется
    // общим результатом выполнения команды
    // – ZERO успешно, если нет – число меньше 0.
    // Если команда выполнена успешно, далее идут строки
    // в следующем виде:
    // 2-я строка – число найденных записей
    // Далее идет список строк:
    // MFN# результат_форматирования
    //

    /// <summary>
    /// Search records on IRBIS-server.
    /// </summary>
    [PublicAPI]
    public class SearchRawCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// First record offset.
        /// </summary>
        public int FirstRecord { get; set; }

        /// <summary>
        /// Format specification.
        /// </summary>
        [CanBeNull]
        public virtual string FormatSpecification { get; set; }

        /// <summary>
        /// Maximal MFN.
        /// </summary>
        public int MaxMfn { get; set; }

        /// <summary>
        /// Minimal MFN.
        /// </summary>
        public int MinMfn { get; set; }

        /// <summary>
        /// Number of records.
        /// </summary>
        public int NumberOfRecords { get; set; }

        /// <summary>
        /// Search query expression.
        /// </summary>
        [CanBeNull]
        public string SearchExpression { get; set; }

        /// <summary>
        /// Specification of sequential search.
        /// </summary>
        [CanBeNull]
        public string SequentialSpecification { get; set; }

        /// <summary>
        /// Use UTF8 encoding for
        /// <see cref="FormatSpecification"/>?
        /// </summary>
        public bool UtfFormat { get; set; }

        /// <summary>
        /// Found records
        /// </summary>
        [CanBeNull]
        public string[] Found { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchRawCommand()
        {
            FirstRecord = 1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchRawCommand
            (
                [NotNull] string expression
            )
        {
            Sure.NotNullNorEmpty(expression, nameof(expression));

            FirstRecord = 1;
            SearchExpression = expression;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Apply <see cref="SearchParameters"/>.
        /// </summary>
        public void ApplyParameters
            (
                [NotNull] SearchParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            Database = parameters.Database;
            FirstRecord = parameters.FirstRecord;
            FormatSpecification = parameters.FormatSpecification;
            MaxMfn = parameters.MaxMfn;
            MinMfn = parameters.MinMfn;
            NumberOfRecords = parameters.NumberOfRecords;
            SearchExpression = parameters.SearchExpression;
            SequentialSpecification
                = parameters.SequentialSpecification;
            UtfFormat = parameters.UtfFormat;
        }

        /// <summary>
        /// Clone the command.
        /// </summary>
        public SearchRawCommand Clone()
        {
            SearchRawCommand result = new SearchRawCommand
            {
                Database = Database,
                FirstRecord = FirstRecord,
                FormatSpecification = FormatSpecification,
                MaxMfn = MaxMfn,
                MinMfn = MinMfn,
                NumberOfRecords = NumberOfRecords,
                SearchExpression = SearchExpression,
                SequentialSpecification = SequentialSpecification,
                UtfFormat = UtfFormat
            };

            return result;
        }

        /// <summary>
        /// Gather <see cref="SearchParameters"/>
        /// from the record.
        /// </summary>
        [NotNull]
        public SearchParameters GatherParameters()
        {
            SearchParameters result = new SearchParameters
            {
                Database = Database,
                FirstRecord = FirstRecord,
                FormatSpecification = FormatSpecification,
                MaxMfn = MaxMfn,
                MinMfn = MinMfn,
                NumberOfRecords = NumberOfRecords,
                SearchExpression = SearchExpression,
                SequentialSpecification = SequentialSpecification,
                UtfFormat = UtfFormat
            };

            return result;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.Search);
            query.AddAnsi(context.GetDatabase(Database));

            string preparedQuery = IrbisSearchQuery.PrepareQuery(SearchExpression);
            query.AddUtf8(preparedQuery);

            query.Add(NumberOfRecords);
            query.Add(FirstRecord);

            string preparedFormat = IrbisFormat.PrepareFormat(FormatSpecification);
            query.Add
                (
                    new TextWithEncoding
                        (
                            UtfFormat
                            ? "!" + preparedFormat
                            : preparedFormat,

                            UtfFormat
                            ? IrbisEncoding.Utf8
                            : IrbisEncoding.Ansi
                        )
                );

            if (!string.IsNullOrEmpty(SequentialSpecification))
            {
                query.Add(MinMfn);
                query.Add(MaxMfn);

                string preparedSequential = IrbisFormat.PrepareFormat
                        (
                            SequentialSpecification
                        );
                if (!ReferenceEquals(preparedSequential, null) && preparedSequential.Length != 0)
                {
                    if (!preparedSequential.StartsWith("!"))
                    {
                        preparedSequential = "!if "
                            + preparedSequential
                            + " then '1' else '0'";
                    }

                    query.AddUtf8(preparedSequential);
                }
            }

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);

            Found = response.RemainingUtfStrings()
                .ToArray();
        }

        #endregion
    }
}
