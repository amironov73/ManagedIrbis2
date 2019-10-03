// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MarcRecord.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Библиографическая запись. Состоит из произвольного количества полей.
    /// </summary>
    [PublicAPI]
    public sealed class MarcRecord
    {
        /// <summary>
        /// Имя базы данных, в которой хранится запись.
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// MFN записи.
        /// </summary>
        public int Mfn { get; set; }

        /// <summary>
        /// Версия записи.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Статус записи.
        /// </summary>
        public RecordStatus Status { get; set; }

        /// <summary>
        /// Список полей.
        /// </summary>
        public List<RecordField> Fields { get; } = new List<RecordField>();

        /// <summary>
        /// Добавление поля в запись.
        /// </summary>
        /// <returns>
        /// Свежедобавленное поле.
        /// </returns>
        public RecordField Add
            (
                int tag,
                string? value = null
            )
        {
            var result = new RecordField {Tag = tag, Value = value};
            Fields.Add(result);

            return result;
        }

        /// <summary>
        /// Очистка записи (удаление всех полей).
        /// </summary>
        /// <returns>
        /// Очищенную запись.
        /// </returns>
        public MarcRecord Clear()
        {
            Fields.Clear();

            return this;
        }

        /// <summary>
        /// Декодирование ответа сервера.
        /// </summary>
        public void Decode(string[] lines)
        {
            var first = lines[0].Split('#');
            Mfn = NumericUtility.ParseInt32(first[0]);
            Status = (RecordStatus) first[1].SafeToInt32();

            var second = lines[1].Split('#');
            Version = NumericUtility.ParseInt32(second[1]);

            for (int i = 2; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!string.IsNullOrEmpty(line))
                {
                    var field = new RecordField();
                    field.Decode(line);
                    Fields.Add(field);
                }
            }
        }

        /// <summary>
        /// Encode the record.
        /// </summary>
        public string Encode
            (
                string delimiter = IrbisText.IrbisDelimiter
            )
        {
            StringBuilder result = new StringBuilder(512);
            result.Append(Mfn.ToInvariantString())
                .Append('#')
                .Append(((int) Status).ToInvariantString())
                .Append(delimiter)
                .Append("0#")
                .Append(Version.ToInvariantString())
                .Append(delimiter);

            foreach (var field in Fields)
            {
                result.Append(field)
                    .Append(delimiter);
            }

            return result.ToString();
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return Encode("\n");
        }
    }
}
