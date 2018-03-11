﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadTermsCommand.cs -- read terms from the search index
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;

using AM;
using AM.Logging;

using CodeJam;

using JetBrains.Annotations;

using ManagedIrbis.Search;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Infrastructure.Commands
{
    //
    // EXTRACT FROM OFFICIAL DOCUMENTATION
    //
    // db_name – имя базы данных
    // ΤΕΡΜ – поисковый термин
    // num_terms – число возвращаемых терминов.
    // Если данный параметр 0, то возвращаются MAX_POSTINGS_IN_PACKET
    // терминов.
    // format – есть 5 вариантов определить формат:
    // 1-й вариант – строка формата;
    // 2-й вариант – имя файла формата расположенного
    // на сервере по 10 пути для базы данных db_name,
    // предваряемого символом @ (например @brief);
    // 3-й вариант – символ @ - в этом случае производится
    // ОПТИМИЗИРОВАННОЕ форматирование, имя формата определяется
    // видом записи;
    // 4-й вариант – символ * - в этом случае производится
    // форматирование как ВЫБОР ПОЛЯ, соответствующего 1-й
    // ссылке каждого термина (например для ссылки в виде
    // 1.200.2.3 берется 2-е[осс] повторение 200-го[метка] поля).
    // 5-й вариант – пустая строка. В этом случае возвращается
    // только список терминов.
    //
    // При любом варианте перед форматированием сервер проделывает
    // следующую операцию - в любом формате специальное сочетание
    // символов вида *** (3 звездочки) заменяется на значение
    // метки поля, взятого из 1-й ссылки для данного термина
    // (например, для ссылки 1.200.1.1 формат вида v***  будет
    // заменен на v200).
    // 
    // ВОЗВРАТ
    // список строк в следующей последовательности:
    // В 1-й строке – код возврата, который определяется тем,
    // найден ли заданный термин TERM в словаре – если найден
    // код возврата – ZERO, если нет – число меньше 0.
    // Далее следуют строки в следующем формате
    // Число ссылок #30
    // Ссылка#30TERMi#30результат_форматирования
    // ИЛИ
    // ТЕРМИН СЛОВАРЯ (если задан пустой формат)
    //

    //
    // О четвёртом варианте форматирования
    // см. ReadPostingsCommand
    //

    /// <summary>
    /// Read terms from the search index.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    [DebuggerDisplay("{Database} {NumberOfTerms} "
        + "{ReverseOrder} {StartTerm}")]
    public sealed class ReadTermsCommand
        : AbstractCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Number of terms to return.
        /// </summary>
        public int NumberOfTerms { get; set; }

        /// <summary>
        /// Reverse order?
        /// </summary>
        public bool ReverseOrder { get; set; }

        /// <summary>
        /// Start term.
        /// </summary>
        [CanBeNull]
        public string StartTerm { get; set; }

        /// <summary>
        /// Format.
        /// </summary>
        [CanBeNull]
        public string Format { get; set; }

        /// <summary>
        /// Terms.
        /// </summary>
        [CanBeNull]
        public TermInfo[] Terms { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadTermsCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
            _terms = new List<TermInfo>();
        }

        #endregion

        #region Private members

        private readonly List<TermInfo> _terms;

        #endregion

        #region Public methods

        /// <summary>
        /// Apply the <see cref="TermParameters"/>
        /// to the command.
        /// </summary>
        public void ApplyParameters
            (
                [NotNull] TermParameters parameters
            )
        {
            Code.NotNull(parameters, "parameters");

            Database = parameters.Database;
            NumberOfTerms = parameters.NumberOfTerms;
            ReverseOrder = parameters.ReverseOrder;
            StartTerm = parameters.StartTerm;
            Format = parameters.Format;
        }

        /// <summary>
        /// Create <see cref="TermParameters"/>
        /// from the command.
        /// </summary>
        [NotNull]
        public TermParameters GatherParameters()
        {
            TermParameters result = new TermParameters
            {
                Database = Database,
                NumberOfTerms = NumberOfTerms,
                ReverseOrder = ReverseOrder,
                StartTerm = StartTerm,
                Format = Format
            };

            return result;
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="AbstractCommand.GoodReturnCodes" />
        public override int[] GoodReturnCodes
        {
            // TERM_NOT_EXISTS = -202;
            // TERM_LAST_IN_LIST = -203;
            // TERM_FIRST_IN_LIST = -204;
            get { return new[] { -202, -203, -204 }; }
        }

        /// <inheritdoc cref="AbstractCommand.CreateQuery" />
        public override ClientQuery CreateQuery()
        {
            ClientQuery result =  base.CreateQuery();
            result.CommandCode = ReverseOrder
                ? CommandCode.ReadTermsReverse
                : CommandCode.ReadTerms;

            string database = Database
                ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                    (
                        "ReadTermsCommand::CreateQuery: "
                        + "database not specified"
                    );

                throw new IrbisException("database not specified");
            }

            string preparedFormat = IrbisFormat.PrepareFormat
                (
                    Format
                );

            result
                .AddAnsi(database)
                .AddUtf8(StartTerm)
                .Add(NumberOfTerms)
                .AddAnsi(preparedFormat);

            return result;
        }

        /// <inheritdoc cref="AbstractCommand.Execute" />
        public override ServerResponse Execute
            (
                ClientQuery query
            )
        {
            Code.NotNull(query, "query");

            ServerResponse result = base.Execute(query);
            CheckResponse(result);

            // ReSharper disable CoVariantArrayConversion
            Terms = string.IsNullOrEmpty(Format)
                ? TermInfo.Parse(result)
                : TermInfoEx.ParseEx(result);
            // ReSharper restore CoVariantArrayConversion

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
            Verifier<ReadTermsCommand> verifier
                = new Verifier<ReadTermsCommand>
                    (
                        this,
                        throwOnError
                    );

            verifier
                .Assert(NumberOfTerms >= 0, "NumberOfTerms")
                .Assert(base.Verify(throwOnError));

            return verifier.Result;
        }

        #endregion
    }
}
