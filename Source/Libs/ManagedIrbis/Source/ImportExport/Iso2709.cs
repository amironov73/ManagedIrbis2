﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Iso2709.cs -- ISO 2709 format record import and export
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;
using System.IO;
using System.Text;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

// ReSharper disable ForCanBeConvertedToForeach

namespace ManagedIrbis.ImportExport
{
    /// <summary>
    /// ISO 2709 format record import and export.
    /// </summary>
    [PublicAPI]
    public static class Iso2709
    {
        #region Constants

        /// <summary>
        /// Marker length.
        /// </summary>
        public const int MarkerLength = 24;

        /// <summary>
        /// Record delimiter.
        /// </summary>
        public const byte RecordDelimiter = 0x1D;

        /// <summary>
        /// Field delimiter.
        /// </summary>
        public const byte FieldDelimiter = 0x1E;

        /// <summary>
        /// Subfield delimiter.
        /// </summary>
        public const byte SubfieldDelimiter = 0x1F;

        #endregion

        #region Private members

        private static void _Encode(byte[] bytes, int pos, int len, int val)
        {
            unchecked
            {
                len--;
                for (pos += len; len >= 0; len--)
                {
                    bytes[pos] = (byte)(val % 10 + (byte)'0');
                    val /= 10;
                    pos--;
                }
            }
        }

        private static int _Encode(byte[] bytes, int pos, string? str, Encoding encoding)
        {
            if (!ReferenceEquals(str, null))
            {
                var encoded = encoding.GetBytes(str);
                for (var i = 0; i < encoded.Length; pos++, i++)
                {
                    bytes[pos] = encoded[i];
                }
            }

            return pos;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Разбор 2709.
        /// </summary>
        public static MarcRecord? ReadRecord
            (
                Stream stream,
                Encoding encoding
            )
        {
            var result = new MarcRecord();

            // Считываем длину записи
            var marker = new byte[5];
            if (stream.Read(marker, 0, marker.Length) != marker.Length)
            {
                return null;
            }

            // а затем и ее остаток
            var recordLength = FastNumber.ParseInt32(marker, 0, marker.Length);
            var record = new byte[recordLength];
            var need = recordLength - marker.Length;
            if (stream.Read(record, marker.Length, need) != need)
            {
                return null;
            }

            // Простая проверка, что мы имеем дело
            // с нормальной ISO-записью
            if (record[recordLength - 1] != RecordDelimiter)
            {
                return null;
            }

            var lengthOfLength = FastNumber.ParseInt32(record, 20, 1);
            var lengthOfOffset = FastNumber.ParseInt32(record, 21, 1);
            var additionalData = FastNumber.ParseInt32(record, 22, 1);
            var directoryLength = 3 + lengthOfLength + lengthOfOffset
                                  + additionalData;

            var indicatorLength = FastNumber.ParseInt32(record, 10, 1);
            var baseAddress = FastNumber.ParseInt32(record, 12, 5);

            // Пошли по полям при помощи справочника
            for (var directory = MarkerLength; ; directory += directoryLength)
            {
                // Переходим к следующему полю.
                // Если нарвались на разделитель, значит, справочник закончился
                if (record[directory] == FieldDelimiter)
                {
                    break;
                }

                var tag = FastNumber.ParseInt32(record, directory, 3);
                var fieldLength = FastNumber.ParseInt32
                    (
                        record,
                        directory + 3,
                        lengthOfLength
                    );
                var fieldOffset = baseAddress + FastNumber.ParseInt32
                    (
                        record,
                        directory + 3 + lengthOfLength,
                        lengthOfOffset
                    );
                var field = new RecordField { Tag = tag };
                result.Fields.Add(field);
                if (tag < 10)
                {
                    // Фиксированное поле
                    // не может содержать подполей и индикаторов
                    field.Value = encoding.GetString
                        (
                            record,
                            fieldOffset,
                            fieldLength - 1
                        );
                }
                else
                {
                    // Поле переменной длины
                    // Содержит два однобайтных индикатора
                    // может содерджать подполя

                    // пропускаем индикаторы
                    var start = fieldOffset + indicatorLength;
                    var stop = fieldOffset + fieldLength - indicatorLength + 1;
                    var position = start;

                    // Ищем значение поля до первого разделителя
                    while (position < stop)
                    {
                        if (record[start] == SubfieldDelimiter)
                        {
                            break;
                        }
                        position++;
                    }

                    // Если есть текст до первого разделителя, запоминаем его
                    if (position != start)
                    {
                        field.Value = encoding.GetString
                            (
                                record,
                                start,
                                position - start
                            );
                    }

                    // Просматриваем подполя
                    start = position;
                    while (start < stop)
                    {
                        position = start + 1;
                        while (position < stop)
                        {
                            if (record[position] == SubfieldDelimiter)
                            {
                                break;
                            }
                            position++;
                        }
                        var subField = new SubField
                            {
                                Code = (char)record[start + 1],
                                Value = encoding.GetString
                                    (
                                        record,
                                        start + 2,
                                        position - start - 2
                                    )
                            };
                        field.Subfields.Add(subField);
                        start = position;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Выводит запись в ISO-поток.
        /// </summary>
        public static void WriteRecord
            (
                MarcRecord record,
                Stream stream,
                Encoding encoding
            )
        {
            var recordLength = MarkerLength;
            var dictionaryLength = 1; // С учетом ограничителя справочника
            var fieldLength = new int[record.Fields.Count]; // Длины полей

            // Сначала пытаемся подсчитать полную длину
            for (var i = 0; i < record.Fields.Count; i++)
            {
                dictionaryLength += 12; // Одна статья справочника
                var field = record.Fields[i];
                if (field.Tag <= 0 || field.Tag >= 1000)
                {
                    throw new IrbisException
                        (
                            "Wrong field: " + field.Tag.ToInvariantString()
                        );
                }

                var fldlen = 0;
                if (field.Tag < 10)
                {
                    // В фиксированном поле не бывает подполей.
                    fldlen += encoding.GetByteCount(field.Value ?? string.Empty);
                }
                else
                {
                    fldlen += 2; // RecordField.IndicatorCount; // Индикаторы
                    fldlen += encoding.GetByteCount(field.Value ?? string.Empty);
                    for (var j = 0; j < field.Subfields.Count; j++)
                    {
                        SubField subField = field.Subfields[j];
//                        if (!SubFieldCode.IsValidCode(subField.Code))
//                        {
//                            throw new IrbisException
//                                (
//                                    "bad subfield code: " + subField.Code
//                                );
//                        }

                        fldlen += 2; // Признак подполя и его код
                        fldlen += encoding.GetByteCount(subField.Value ?? string.Empty);
                    }
                }

                fldlen++; // Разделитель полей
                if (fldlen >= 10000)
                {
                    throw new IrbisException(Resources.RecordTooLong);
                }

                fieldLength[i] = fldlen;
                recordLength += fldlen;
            }
            recordLength += dictionaryLength; // Справочник
            recordLength++; // Разделитель записей

            if (recordLength >= 100000)
            {
                // Слишком длинная запись
                throw new IrbisException(Resources.RecordTooLong);
            }

            // Приступаем к кодированию
            var dictionaryPosition = IsoMarker.MarkerLength;
            var baseAddress = IsoMarker.MarkerLength + dictionaryLength;
            var currentAddress = baseAddress;
            var bytes = new byte[recordLength];

            // Кодируем маркер
            for (var i = 0; i <= baseAddress; i++)
            {
                bytes[i] = (byte)' ';
            }
            _Encode(bytes, 0, 5, recordLength);
            _Encode(bytes, 12, 5, baseAddress);

            bytes[5] = (byte)'n';  // Record status
            bytes[6] = (byte)'a';  // Record type
            bytes[7] = (byte)'m';  // Bibligraphical index
            bytes[8] = (byte)'2';
            bytes[10] = (byte)'2';
            bytes[11] = (byte)'2';
            bytes[17] = (byte)' '; // Bibliographical level
            bytes[18] = (byte)'i'; // Cataloging rules
            bytes[19] = (byte)' '; // Related record
            bytes[20] = (byte)'4'; // Field length
            bytes[21] = (byte)'5'; // Field offset
            bytes[22] = (byte)'0';

            // Конец справочника
            bytes[baseAddress - 1] = FieldDelimiter;

            // Проходим по полям
            for (var i = 0; i < record.Fields.Count; i++, dictionaryPosition += 12)
            {
                // Кодируем справочник
                var field = record.Fields[i];
                _Encode(bytes, dictionaryPosition, 3, field.Tag);
                _Encode(bytes, dictionaryPosition + 3, 4, fieldLength[i]);
                _Encode(bytes, dictionaryPosition + 7, 5, currentAddress - baseAddress);

                // Кодируем поле
                if (field.Tag < 10)
                {
                    // В фиксированном поле не бывает подполей и индикаторов.
                    currentAddress = _Encode
                        (
                            bytes,
                            currentAddress,
                            field.Value,
                            encoding
                        );
                }
                else
                {
                    // Индикаторы
                    bytes[currentAddress++] = (byte)' ';
                    bytes[currentAddress++] = (byte)' ';

                    currentAddress = _Encode
                        (
                            bytes,
                            currentAddress,
                            field.Value,
                            encoding
                        );

                    for (var j = 0; j < field.Subfields.Count; j++)
                    {
                        bytes[currentAddress++] = SubfieldDelimiter;
                        bytes[currentAddress++] = (byte)field.Subfields[j].Code;
                        currentAddress = _Encode
                            (
                                bytes,
                                currentAddress,
                                field.Subfields[j].Value,
                                encoding
                            );
                    }
                }

                // Ограничитель поля
                bytes[currentAddress++] = FieldDelimiter;
            }

            Debug.Assert(currentAddress == recordLength - 1);

            // Конец записи
            bytes[recordLength - 1] = RecordDelimiter;

            // Собственно записываем
            stream.Write(bytes, 0, bytes.Length);
        }

        #endregion
    }
}
