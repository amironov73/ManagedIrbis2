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

        private static int _Encode(byte[] bytes, int pos, string str, Encoding encoding)
        {
            if (!ReferenceEquals(str, null))
            {
                byte[] encoded = encoding.GetBytes(str);
                for (int i = 0; i < encoded.Length; pos++, i++)
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
        [CanBeNull]
        public static MarcRecord ReadRecord
            (
                [NotNull] Stream stream,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(stream, nameof(stream));
            Sure.NotNull(encoding, nameof(encoding));

            MarcRecord result = new MarcRecord();

            // Считываем длину записи
            byte[] marker = new byte[5];
            if (stream.Read(marker, 0, marker.Length) != marker.Length)
            {
                return null;
            }

            // а затем и ее остаток
            int recordLength = FastNumber.ParseInt32(marker, 0, marker.Length);
            byte[] record = new byte[recordLength];
            int need = recordLength - marker.Length;
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

            int lengthOfLength = FastNumber.ParseInt32(record, 20, 1);
            int lengthOfOffset = FastNumber.ParseInt32(record, 21, 1);
            int additionalData = FastNumber.ParseInt32(record, 22, 1);
            int directoryLength = 3 + lengthOfLength + lengthOfOffset
                                  + additionalData;

            int indicatorLength = FastNumber.ParseInt32(record, 10, 1);
            int baseAddress = FastNumber.ParseInt32(record, 12, 5);

            // Пошли по полям при помощи справочника
            for (int directory = MarkerLength; ; directory += directoryLength)
            {
                // Переходим к следующему полю.
                // Если нарвались на разделитель, значит, справочник закончился
                if (record[directory] == FieldDelimiter)
                {
                    break;
                }

                int tag = FastNumber.ParseInt32(record, directory, 3);
                int fieldLength = FastNumber.ParseInt32
                    (
                        record,
                        directory + 3,
                        lengthOfLength
                    );
                int fieldOffset = baseAddress + FastNumber.ParseInt32
                    (
                        record,
                        directory + 3 + lengthOfLength,
                        lengthOfOffset
                    );
                RecordField field = new RecordField(tag);
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
                    int start = fieldOffset + indicatorLength;
                    int stop = fieldOffset + fieldLength - indicatorLength + 1;
                    int position = start;

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
                        SubField subField = new SubField
                            (
                                (char)record[start + 1],
                                encoding.GetString
                                    (
                                        record,
                                        start + 2,
                                        position - start - 2
                                    )
                            );
                        field.SubFields.Add(subField);
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
                [NotNull] MarcRecord record,
                [NotNull] Stream stream,
                [NotNull] Encoding encoding
            )
        {
            int recordLength = MarkerLength;
            int dictionaryLength = 1; // С учетом ограничителя справочника
            int[] fieldLength = new int[record.Fields.Count]; // Длины полей

            // Сначала пытаемся подсчитать полную длину
            for (int i = 0; i < record.Fields.Count; i++)
            {
                dictionaryLength += 12; // Одна статья справочника
                RecordField field = record.Fields[i];
                if (field.Tag <= 0 || field.Tag >= 1000)
                {
                    throw new IrbisException
                        (
                            "Wrong field: " + field.Tag.ToInvariantString()
                        );
                }

                int fldlen = 0;
                if (field.IsFixed)
                {
                    // В фиксированном поле не бывает подполей.
                    fldlen += encoding.GetByteCount(field.Value ?? string.Empty);
                }
                else
                {
                    fldlen += RecordField.IndicatorCount; // Индикаторы
                    fldlen += encoding.GetByteCount(field.Value ?? string.Empty);
                    for (int j = 0; j < field.SubFields.Count; j++)
                    {
                        SubField subField = field.SubFields[j];
                        if (!SubFieldCode.IsValidCode(subField.Code))
                        {
                            throw new IrbisException
                                (
                                    "bad subfield code: " + subField.Code
                                );
                        }

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
            int dictionaryPosition = IsoMarker.MarkerLength;
            int baseAddress = IsoMarker.MarkerLength + dictionaryLength;
            int currentAddress = baseAddress;
            byte[] bytes = new byte[recordLength];

            // Кодируем маркер
            for (int i = 0; i <= baseAddress; i++)
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
            for (int i = 0; i < record.Fields.Count; i++, dictionaryPosition += 12)
            {
                // Кодируем справочник
                RecordField field = record.Fields[i];
                _Encode(bytes, dictionaryPosition, 3, field.Tag);
                _Encode(bytes, dictionaryPosition + 3, 4, fieldLength[i]);
                _Encode(bytes, dictionaryPosition + 7, 5, currentAddress - baseAddress);

                // Кодируем поле
                if (field.IsFixed)
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
#if WITH_INDICATORS

                    chars[currentAddress++] = (byte)fld.Indicator1.Value[0];
                    chars[currentAddress++] = (byte)fld.Indicator2.Value[0];

#else

                    bytes[currentAddress++] = (byte)' ';
                    bytes[currentAddress++] = (byte)' ';

#endif

                    currentAddress = _Encode
                        (
                            bytes,
                            currentAddress,
                            field.Value,
                            encoding
                        );

                    for (int j = 0; j < field.SubFields.Count; j++)
                    {
                        bytes[currentAddress++] = SubfieldDelimiter;
                        bytes[currentAddress++] = (byte)field.SubFields[j].Code;
                        currentAddress = _Encode
                            (
                                bytes,
                                currentAddress,
                                field.SubFields[j].Value,
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
