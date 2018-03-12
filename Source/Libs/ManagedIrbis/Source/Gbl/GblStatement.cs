﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GblStatement.cs -- GBL file item
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using AM;
using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Gbl
{
    //
    // EXTRACT FROM OFFICIAL DOCUMENTATION
    //
    // Файл задания на пакетную корректировку
    // представляет собой текстовый файл с расширением GBL,
    // содержит последовательность операторов корректировки,
    // каждый из которых состоит из нескольких строк.
    //
    // Операторы выполняются в порядке их следования,
    // причем каждый оператор использует значения полей
    // и/или подполей измененных, возможно, предыдущими операторами.
    //
    // Первая строка файла задания – это число, задающее
    // количество параметров, используемых в операторах корректировки.
    // 
    // Последующие пары строк, число пар должно быть равно
    // количеству параметров, используются программой
    // глобальной корректировки.
    // 
    // Первая строка пары - значение параметра или пусто,
    // если пользователю предлагается задать его значение
    // перед выполнением корректировки. В этой строке можно
    // задать имя файла меню (с расширением MNU)
    // или имя рабочего листа подполей (с расширением Wss),
    // которые будут поданы для выбора значения параметра.
    // Вторая строка пары – наименование параметра,
    // которое появится в названии столбца, задающего параметр.
    //
    // Группы строк, описывающих операторы корректировки
    // Далее следуют группы строк, описывающих операторы корректировки.
    //
    // Первая строка каждой группы – это имя оператора,
    // которое может иметь одно из значений: ADD, REP, CHA, CHAC,
    // DEL, DELR, UNDEL, CORREC, NEWMFN, END, IF, FI, ALL,
    // EMPTY, REPEAT, UNTIL, //.
    //
    // Количество строк, описывающих оператор, зависит от его назначения.
    // Операторы ADD, REP, CHA, CHAC, DEL описываются пятью строками,
    // в которых задаются  следующие элементы:
    // ИМЯ ОПЕРАТОРА
    // МЕТКА ПОЛЯ/ПОДПОЛЯ: число, обозначающее метку поля,
    // + разделитель подполя + обозначение подполя.
    // Разделитель подполя с обозначением могут отсутствовать
    // ПОВТОРЕНИЕ ПОЛЯ
    // * - если корректируются все повторения
    // F - если используется корректировка по формату
    // N (число) – если корректируется N-ое повторение поля
    // L – если корректируется последнее повторение поля
    // L-N ( число) – если корректируется N-ое с конца повторение поля
    // ФОРМАТ 1 – формат
    // ФОРМАТ 2 - формат
    //
    // Для каждого конкретного оператора элементы ФОРМАТ 1
    // и ФОРМАТ 2 имеют свое назначение. Некоторые из элементов
    // могут не задаваться, когда в конкретной конфигурации
    // они не имеют смысла. Тогда соответствующая строка
    // в задании должна быть пустой или занята символом-заполнителем,
    // как это формирует программа глобальной корректировки.
    //
    // Содержимое строк остальных операторов определяется
    // их назначением и представлено в описании операторов.

    /// <summary>
    /// GBL file item.
    /// </summary>
    [PublicAPI]
    [XmlRoot("gbl-statement")]
    [DebuggerDisplay("{Command} {Parameter1} {Parameter2}")]
    public sealed class GblStatement
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Constants

        /// <summary>
        /// Разделитель элементов
        /// </summary>
        public const string Delimiter = "\x1F\x1E";

        #endregion

        #region Properties

        /// <summary>
        /// Команда (оператор), например, ADD или DEL.
        /// </summary>
        [CanBeNull]
        [XmlElement("command")]
        [JsonProperty("command")]
        public string Command { get; set; }

        /// <summary>
        /// Первый параметр, как правило, спецификация поля/подполя.
        /// </summary>
        [CanBeNull]
        [XmlElement("parameter1")]
        [JsonProperty("parameter1")]
        public string Parameter1 { get; set; }

        /// <summary>
        /// Второй параметр, как правило, спецификация повторения.
        /// </summary>
        [CanBeNull]
        [XmlElement("parameter2")]
        [JsonProperty("parameter2")]
        public string Parameter2 { get; set; }

        /// <summary>
        /// Первый формат, например, выражение для замены.
        /// </summary>
        [CanBeNull]
        [XmlElement("format1")]
        [JsonProperty("format1")]
        public string Format1 { get; set; }

        /// <summary>
        /// Второй формат, например, заменяющее выражение.
        /// </summary>
        [CanBeNull]
        [XmlElement("format2")]
        [JsonProperty("format2")]
        public string Format2 { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Encode for protocol.
        /// </summary>
        [NotNull]
        public string EncodeForProtocol()
        {
            StringBuilder result = new StringBuilder();

            result.Append(Command);
            result.Append(Delimiter);
            result.Append(Parameter1);
            result.Append(Delimiter);
            result.Append(Parameter2);
            result.Append(Delimiter);
            result.Append(Format1);
            result.Append(Delimiter);
            result.Append(Format2);
            result.Append(Delimiter);

            return result.ToString();
        }

        /// <summary>
        /// Parse the stream.
        /// </summary>
        [CanBeNull]
        public static GblStatement ParseStream
            (
                [NotNull] TextReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            string command = reader.ReadLine();
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }

            GblStatement result = new GblStatement
            {
                Command = command.Trim(),
                Parameter1 = reader.RequireLine(),
                Parameter2 = reader.RequireLine(),
                Format1 = reader.RequireLine(),
                Format2 = reader.RequireLine()
            };

            return result;
        }

        /// <summary>
        /// Should JSON serialize <see cref="Format1"/>?
        /// </summary>
        public bool ShouldSerializeFormat1()
        {
            return !string.IsNullOrEmpty(Format1);
        }

        /// <summary>
        /// Should JSON serialize <see cref="Format2"/>?
        /// </summary>
        public bool ShouldSerializeFormat2()
        {
            return !string.IsNullOrEmpty(Format2);
        }

        /// <summary>
        /// Should JSON serialize <see cref="Parameter1"/>?
        /// </summary>
        public bool ShouldSerializeParameter1()
        {
            return !string.IsNullOrEmpty(Parameter1);
        }

        /// <summary>
        /// Should JSON serialize <see cref="Parameter2"/>?
        /// </summary>
        public bool ShouldSerializeParameter2()
        {
            return !string.IsNullOrEmpty(Parameter2);
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            Command = reader.ReadNullableString();
            Parameter1 = reader.ReadNullableString();
            Parameter2 = reader.ReadNullableString();
            Format1 = reader.ReadNullableString();
            Format2 = reader.ReadNullableString();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer.WriteNullable(Command);
            writer.WriteNullable(Parameter1);
            writer.WriteNullable(Parameter2);
            writer.WriteNullable(Format1);
            writer.WriteNullable(Format2);
        }

        #endregion

        #region IVerifiable

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<GblStatement> verifier = new Verifier<GblStatement>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Command, "Command");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return string.Format
                (
                    "Command: {0},{5}"
                    + "Parameter1: {1},{5}"
                    + "Parameter2: {2},{5}"
                    + "Format1: {3},{5}"
                    + "Format2: {4}",
                    Command,
                    Parameter1,
                    Parameter2,
                    Format1,
                    Format2,
                    Environment.NewLine
                );
        }

        #endregion
    }
}
