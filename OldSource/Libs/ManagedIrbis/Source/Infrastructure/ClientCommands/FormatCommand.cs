﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FormatCommand.cs -- format records on IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System.Collections.Generic;

using AM;
using AM.Logging;
using AM.Text;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    //
    // EXTRACT FROM OFFICIAL DOCUMENTATION
    //
    // db_name – имя базы данных
    // MFN – номер записи в базе данных db_name
    // format – есть 5 вариантов определить формат:
    // 1-й вариант  – строка формата;
    // 2-й вариант – имя файла формата расположенного
    // на сервере по 10 пути для базы данных db_name,
    // предваряемого символом @ (например @brief);
    // 3-й вариант – символ @ - в этом случае производится
    // ОПТИМИЗИРОВАННОЕ форматирование,
    // имя формата определяется видом записи;
    // 4-й вариант – символ * - в этом случае производится
    // форматирование как ВЫБОР ПОЛЯ, соответствующего
    // 1-й ссылке каждого термина (например для ссылки
    // в виде 1.200.2.3 берется 2-е[осс] повторение
    // 200-го[метка] поля).
    // 5-й вариант – пустая строка. В этом случае
    // возвращается только список терминов.
    // При любом варианте перед форматированием сервер
    // проделывает следующую операцию - в любом формате
    // специальное сочетание символов вида *** (3 звездочки)
    // заменяется на значение метки поля, взятого
    // из 1-й ссылки для данного термина (например,
    // для ссылки 1.200.1.1 формат вида v***  будет заменен
    // на v200).
    //

    /// <summary>
    /// Format records on IRBIS-server.
    /// </summary>
    [PublicAPI]
    public class FormatCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Format specification.
        /// </summary>
        [CanBeNull]
        public string FormatSpecification { get; set; }

        /// <summary>
        /// List of MFNs to format.
        /// </summary>
        [NotNull]
        public List<int> MfnList { get; } = new List<int>();

        /// <summary>
        /// Virtual record to format.
        /// </summary>
        [CanBeNull]
        public MarcRecord VirtualRecord { get; set; }

        /// <summary>
        /// Use UTF-8 for <see cref="FormatSpecification"/>.
        /// </summary>
        public bool UtfFormat { get; set; }

        /// <summary>
        /// Result of the command.
        /// </summary>
        [CanBeNull]
        public string[] FormatResult { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Get format result from server response.
        /// </summary>
        [NotNull]
        public static string[] GetFormatResult
            (
                [NotNull] ServerResponse response,
                int itemCount
            )
        {
            Sure.NotNull(response, nameof(response));

            List<string> result = new List<string>();

            if (itemCount == 1)
            {
                string line = response.RemainingUtfText();
                if (!string.IsNullOrEmpty(line))
                {
                    line = line.Trim();
                }
                result.Add(line);
            }
            else
            {
                while (true)
                {
                    string line = response.GetUtfString();
                    if (ReferenceEquals(line, null))
                    {
                        break;
                    }
                    int index = line.IndexOf('#');
                    if (index > 0)
                    {
                        string mfnPart = line.Substring(0, index);
                        int mfn = mfnPart.SafeToInt32();
                        if (mfn > 0)
                        {
                            line = line.Substring(index + 1);
                        }
                    }

                    line = IrbisText.IrbisToWindows(line);
                    if (!ReferenceEquals(line, null) && line.Length != 0)
                    {
                        line = line.Trim();
                    }
                    result.Add(line);
                }
            }

            return result.ToArray();
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)"/>
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.FormatRecord);
            query.Add(context.GetDatabase(Database));
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

            if (MfnList.Count >= IrbisConstants.MaxPostings)
            {
                Log.Error
                    (
                        nameof(FormatCommand) + "::" + nameof(CreateQuery)
                        + ": too many MFNs: "
                        + MfnList.Count
                    );

                throw new IrbisNetworkException("too many MFNs");
            }

            if (MfnList.Count == 0)
            {
                query.Add(-2);
                query.Add(VirtualRecord);
            }
            else
            {
                query.Add(MfnList.Count);
                foreach (int mfn in MfnList)
                {
                    query.Add(mfn);
                }
            }

            ServerResponse response = BaseExecute(context);
            if (!string.IsNullOrEmpty(FormatSpecification))
            {
                response.GetReturnCode();
            }

            int count = 1;
            if (VirtualRecord == null)
            {
                count = MfnList.Count;
            }

            FormatResult = GetFormatResult(response, count);
        }

        #endregion
    }
}
