﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReadPostingsCommand.cs -- read posings for given term
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;
using ManagedIrbis.Search;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    //
    // EXTRACT FROM OFFICIAL DOCUMENTATION
    //
    // ПАРАМЕТРЫ
    // db_name – имя базы данных
    // ΤΕΡΜ – поисковый термин
    // num_postings – число возвращаемых ссылок.
    // Если данный параметр 0, то возвращаются
    // MAX_POSTINGS_IN_PACKET ссылок.
    // first_posting – возможно 2 варианта значений
    // для данного параметра:
    // 1-й вариант – число больше 0. Это номер 1-й возвращаемой
    // ссылки из общего списка ссылок данного термина;
    // 2-й вариант – если равно 0, то возвращается только
    // число ссылок данного термина.
    //
    // ВОЗВРАТ
    // список строк в следующей последовательности:
    // В 1-й строке – код возврата, который определяется тем,
    // найден ли заданный термин TERM в словаре – если найден
    // код возврата – ZERO, если нет – число меньше 0.
    //
    // Если термин найден в словаре и ИМЕЕТ ССЫЛКИ (не удален),
    // далее следуют строки в следующем формате:
    // MFN#TAG#OCC#CNT, например:
    //
    // 0
    // 1#200#1#1
    // 3#200#1#1
    //

    //
    // Как работает метод расформатирования ЗВЁЗДОЧКА
    //
    // Пусть у нас в каталоге IBIS есть две записи (в сильном сокращении):
    //
    // 0001062224
    // #963/1:_ ^I0130-6073
    // #920/1:_ ASP
    // #101/1:_ fre
    // #102/1:_ RU
    // #331/1:_ Статья на французском языке о великом русском полководце князе Александре Невском.
    // #463/1:_ ^wИ208/2009/7^CИностранные языки в школе^J2009^1С. 65-68^hN 7
    // #200/1:_ ^AAlexandre Nevski^FD. V. Matveenkov
    //
    // и
    //
    // 0001063267
    // #331/1:_ Текст на французском языке об Александре Невском.
    // #463/1:_ ^wИ208/2014/1^CИностранные языки в школе^J2014^1С. 58-61^h№ 1
    // #700/1:_ ^AБаженова^BН. Г.^Cкандидат педагогических наук^W01^4070^2a
    // #200/1:_ ^AAlexandre Nevski^FН. Г. Баженова
    //
    // Обе записи сформировали в инверсном файле поисковый
    // терм T=ALEXANDRE NEVSKI. Посмотрим, что делает ВЫБОР ПОЛЯ (ЗВЁЗДОЧКА).
    //
    // Посылаем запрос на список ссылок для данного термина
    // (команда I, только необходимые для понимания строки):
    //
    // IBIS  -- Имя базы данных
    // 2 -- Сколько ссылок выдать
    // 1 -- Начиная с какого номера
    // * -- Форматирование как ВЫБОР ПОЛЯ
    // T=ALEXANDRE NEVSKI -- Собственно терм
    //
    // Ответ сервера (только необходимые для понимания строки):
    //
    // 0
    // 1062224#200#1#1#^AAlexandre Nevski^FD. V. Matveenkov
    // 1063267#200#1#1#^AAlexandre Nevski^FН. Г. Баженова
    //
    // Т. е. после признака успешного выполнения (0) идут две строки следующего вида:
    //
    // MFN # Тег # Повторение поля # Номер термина в повторении # Результат расформатирования – содержимое первого повторения поля 200 (см. "Тег" и "Повторение поля" ранее в строке)
    //
    // Получается, что сервер просто приводит содержимое повторения
    // указанного в FST поля. Если там написано, например, "12251"
    // (как это сделано для дистрибутиве для словаря ключевых слов),
    // то он выдаёт пустую строку, т. к. полей с такой меткой просто нет в записи.
    //

    /// <summary>
    /// Read postings for given term.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("{" + nameof(Database) + "} {" +nameof(NumberOfPostings)
        + "} {" + nameof(FirstPosting) + "}")]
    public sealed class ReadPostingsCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// First posting to return.
        /// </summary>
        public int FirstPosting { get; set; }

        /// <summary>
        /// Format.
        /// </summary>
        [CanBeNull]
        public string Format { get; set; }

        /// <summary>
        /// Number of postings to return.
        /// </summary>
        public int NumberOfPostings { get; set; }

        /// <summary>
        /// Term.
        /// </summary>
        [CanBeNull]
        public string Term { get; set; }

        /// <summary>
        /// List of terms.
        /// </summary>
        [CanBeNull]
        public string[] ListOfTerms { get; set; }

        /// <summary>
        /// Postings.
        /// </summary>
        [CanBeNull]
        public TermPosting[] Postings { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadPostingsCommand()
        {
            FirstPosting = 1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadPostingsCommand
            (
                [NotNull] string term
            )
        {
            Sure.NotNullNorEmpty(term, nameof(term));

            Term = term;
            FirstPosting = 1;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReadPostingsCommand
            (
                [NotNull][ItemNotNull] IEnumerable<string> terms
            )
        {
            Sure.NotNull(terms, nameof(terms));

            ListOfTerms = terms.ToArray();
            FirstPosting = 1;
        }

        #endregion

        #region Private members

        // Good codes for CheckResponse():
        // TERM_NOT_EXISTS = -202;
        // TERM_LAST_IN_LIST = -203;
        // TERM_FIRST_IN_LIST = -204;
        private static int[] _goodCodes = { -202, -203, -204 };

        #endregion

        #region Public methods

        /// <summary>
        /// Apply <see cref="PostingParameters"/>
        /// to the command.
        /// </summary>
        public void ApplyParameters
            (
                [NotNull] PostingParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            Database = parameters.Database;
            FirstPosting = parameters.FirstPosting;
            Format = parameters.Format;
            NumberOfPostings = parameters.NumberOfPostings;
            Term = parameters.Term;
            ListOfTerms = parameters.ListOfTerms;
        }

        /// <summary>
        /// Gather <see cref="PostingParameters"/>
        /// from the command.
        /// </summary>
        [NotNull]
        public PostingParameters GatherParameters()
        {
            PostingParameters result = new PostingParameters
            {
                Database = Database,
                FirstPosting = FirstPosting,
                Format = Format,
                NumberOfPostings = NumberOfPostings,
                Term = Term,
                ListOfTerms = ListOfTerms
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
            ClientQuery query = CreateQuery(context, CommandCode.ReadPostings);
            query
                .AddAnsi(context.GetDatabase(Database))
                .Add(NumberOfPostings)
                .Add(FirstPosting)
                .Add(Format);

            if (string.IsNullOrEmpty(Term))
            {
                if (ReferenceEquals(ListOfTerms, null))
                {
                    Log.Error
                        (
                            nameof(ReadPostingsCommand) + "::" + nameof(Execute)
                            + ": " + Resources.ReadPostingsCommand_ListOfTermsNull
                        );

                    throw new IrbisException(Resources.ReadPostingsCommand_ListOfTermsNull);
                }

                foreach (string term in ListOfTerms)
                {
                    query.AddUtf8(term);
                }
            }
            else
            {
                query.AddUtf8(Term);
            }

            ServerResponse result = BaseExecute(context);
            CheckResponse(result, _goodCodes);
            Postings = TermPosting.Parse(result);
        }

        #endregion
    }
}
