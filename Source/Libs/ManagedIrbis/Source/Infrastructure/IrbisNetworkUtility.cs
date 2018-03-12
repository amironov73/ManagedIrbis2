﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisNetworkUtility.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System;
using System.Globalization;
using System.IO;
using System.Text;

using AM;
using AM.Logging;
using AM.Text;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Вспомогательные методы для формирования сетевых
    /// пакетов и их парсинга.
    /// </summary>
    [PublicAPI]
    public static class IrbisNetworkUtility
    {
        #region Private members

        private static void _DumpBytes
            (
                byte[] bytes,
                int offset,
                TextWriter writer
            )
        {
            int length = Math.Min(16, bytes.Length - offset);
            char[] chars = new char[16];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = ' ';
            }

            writer.Write("{0:X8} ", offset);

            for (int i = 0; i < length; i++)
            {
                writer.Write(" {0:X2}", bytes[offset + i]);
                if (i == 7)
                {
                    writer.Write(" ");
                }
            }
            for (int i = length; i < 16; i++)
            {
                writer.Write("   ");
                if (i == 7)
                {
                    writer.Write(" ");
                }
            }

            writer.Write("  ");

            Encoding encoding = IrbisEncoding.Ansi;

            for (int i = 0; i < length; i++)
            {
                byte b = bytes[offset + i];
                if (b > 32)
                {
                    encoding.GetChars
                        (
                            bytes,
                            offset + i,
                            1,
                            chars,
                            i
                        );
                }
            }
            writer.Write(chars);

            writer.WriteLine();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Dump bytes.
        /// </summary>
        public static void DumpBytes
            (
                [NotNull] byte[] bytes,
                [NotNull] TextWriter writer
            )
        {
            Sure.NotNull(bytes, nameof(bytes));
            Sure.NotNull(writer, nameof(writer));

            for (int offset = 0; offset < bytes.Length; offset += 16)
            {
                _DumpBytes(bytes, offset, writer);
            }

            writer.WriteLine();
        }

        /// <summary>
        /// Dump bytes.
        /// </summary>
        [NotNull]
        public static string DumpBytes
            (
                [NotNull] byte[] bytes
            )
        {
            Sure.NotNull(bytes, nameof(bytes));

            StringWriter writer = new StringWriter();
            DumpBytes
                (
                    bytes,
                    writer
                );
            return writer.ToString();
        }

        /// <summary>
        /// Записываем любой объект (диспетчеризация).
        /// </summary>
        [NotNull]
        public static Stream EncodeAny
            (
                [NotNull] this Stream stream,
                [CanBeNull] object anyObject
            )
        {
            // ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

            if (ReferenceEquals(anyObject, null))
            {
                // Do nothing
            }
            else if (anyObject is bool)
            {
                return stream.EncodeBoolean((bool)anyObject);
            }
            else if (anyObject is byte)
            {
                stream.WriteByte((byte)anyObject);
                return stream;
            }
            else if (anyObject is byte[])
            {
                return stream.EncodeBytes((byte[])anyObject);
            }
            else if (anyObject is int)
            {
                return stream.EncodeInt32((int)anyObject);
            }
            else if (anyObject is MarcRecord)
            {
                return stream.EncodeRecord((MarcRecord)anyObject);
            }
            else if (anyObject is string)
            {
                return stream.EncodeString((string)anyObject);
            }
            else if (anyObject is TextWithEncoding)
            {
                return stream.EncodeTextWithEncoding
                    (
                        (TextWithEncoding)anyObject
                    );
            }
            else if (anyObject is FileSpecification)
            {
                return stream.EncodeFileSpecification
                    (
                        (FileSpecification)anyObject
                    );
            }
            else if (anyObject is RecordReference)
            {
                return stream.EncodeRecordReference
                    (
                        (RecordReference)anyObject
                    );
            }
            else
            {
                return stream.EncodeObject(anyObject);
            }

            return stream;

            // ReSharper restore CanBeReplacedWithTryCastAndCheckForNull
        }

        /// <summary>
        /// Записываем булево значение в виде 0/1.
        /// </summary>
        [NotNull]
        public static Stream EncodeBoolean
            (
                [NotNull] this Stream stream,
                bool value
            )
        {
            byte b = (byte)(value ? '1' : '0');

            stream.WriteByte(b);

            return stream;
        }

        /// <summary>
        /// Записываем буфер.
        /// </summary>
        [NotNull]
        public static Stream EncodeBytes
            (
                [NotNull] this Stream stream,
                [CanBeNull] byte[] bytes
            )
        {
            if (!ReferenceEquals(bytes, null))
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            return stream;
        }

        /// <summary>
        /// Encode line delimiter.
        /// </summary>
        [NotNull]
        public static Stream EncodeDelimiter
            (
                [NotNull] this Stream stream
            )
        {
            stream.WriteByte((byte)ClientQuery.Delimiter);

            return stream;
        }

        /// <summary>
        /// Encode <see cref="FileSpecification"/>.
        /// </summary>
        [NotNull]
        public static Stream EncodeFileSpecification
            (
                [NotNull] this Stream stream,
                [NotNull] FileSpecification specification
            )
        {
            Sure.NotNull(stream, nameof(stream));
            Sure.NotNull(specification, nameof(specification));

            stream.EncodeString(specification.ToString());

            return stream;
        }

        /// <summary>
        /// Записываем целое.
        /// </summary>
        [NotNull]
        public static Stream EncodeInt32
            (
                [NotNull] this Stream stream,
                int value
            )
        {
            string text = value.ToString(CultureInfo.InvariantCulture);
            byte[] bytes = IrbisEncoding.Ansi.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        /// <summary>
        /// Записываем целое.
        /// </summary>
        [NotNull]
        public static Stream EncodeInt64
            (
                [NotNull] this Stream stream,
                long value
            )
        {
            string text = value.ToString(CultureInfo.InvariantCulture);
            byte[] bytes = IrbisEncoding.Ansi.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        /// <summary>
        /// Записываем произвольные объект.
        /// ToString + кодировка ANSI.
        /// </summary>
        [NotNull]
        public static Stream EncodeObject
            (
                [NotNull] this Stream stream,
                [CanBeNull] object obj
            )
        {
            if (!ReferenceEquals(obj, null))
            {
                string text = obj.ToString();
                return stream.EncodeString(text);
            }

            return stream;
        }

        /// <summary>
        /// Запись в кодировке UTF.
        /// </summary>
        [NotNull]
        public static Stream EncodeRecord
            (
                [NotNull] this Stream stream,
                [NotNull] MarcRecord record
            )
        {
            Sure.NotNull(stream, nameof(stream));
            Sure.NotNull(record, nameof(record));

            string text = ProtocolText.EncodeRecord(record);
            byte[] bytes = IrbisEncoding.Utf8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        /// <summary>
        /// <see cref="MarcRecord"/> with database prefix.
        /// </summary>
        public static Stream EncodeRecordReference
            (
                [NotNull] this Stream stream,
                [NotNull] RecordReference reference
            )
        {
            Sure.NotNull(stream, nameof(stream));
            Sure.NotNull(reference, nameof(reference));

            if (string.IsNullOrEmpty(reference.Database))
            {
                Log.Error
                    (
                        "IrbisNetworkUtility::EncodeRecordReference: "
                        + "database not specified"
                    );

                throw new IrbisException("database not specified");
            }

            if (ReferenceEquals(reference.Record, null))
            {
                Log.Error
                    (
                        "IrbisNetworkUtility::EncodeRecordReference: "
                        + "record is null"
                    );

                throw new IrbisException("record is null");
            }

            string text = reference.Database
                + IrbisText.IrbisDelimiter
                + ProtocolText.EncodeRecord(reference.Record);
            byte[] bytes = IrbisEncoding.Utf8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);

            return stream;
        }

        /// <summary>
        /// Записываем строку в кодировке ANSI.
        /// </summary>
        [NotNull]
        public static Stream EncodeString
            (
                [NotNull] this Stream stream,
                [CanBeNull] string text
            )
        {
            if (!string.IsNullOrEmpty(text))
            {
                byte[] bytes = IrbisEncoding.Ansi.GetBytes(text);

                stream.Write(bytes, 0, bytes.Length);
            }

            return stream;
        }

        /// <summary>
        /// Записываем строку в произвольной кодировке.
        /// </summary>
        public static Stream EncodeTextWithEncoding
            (
                [NotNull] this Stream stream,
                [CanBeNull] TextWithEncoding text
            )
        {
            if (!ReferenceEquals(text, null))
            {
                byte[] bytes = text.ToBytes();

                stream.Write(bytes, 0, bytes.Length);
            }

            return stream;
        }

        /// <summary>
        /// Записываем код АРМ.
        /// </summary>
        [NotNull]
        public static Stream EncodeWorkstation
            (
                [NotNull] this Stream stream,
                IrbisWorkstation workstation
            )
        {
            stream.WriteByte((byte)workstation);

            return stream;
        }

        /// <summary>
        /// Throw <see cref="IrbisNetworkException"/>
        /// if the record is empty.
        /// </summary>
        public static void ThrowIfEmptyRecord
            (
                [NotNull] MarcRecord record,
                [NotNull] ServerResponse response
            )
        {
            Sure.NotNull(record, nameof(record));
            Sure.NotNull(response, nameof(response));

            if (record.Fields.Count == 0)
            {
                byte[] bytes = response.GetAnswerCopy();
                string dump = DumpBytes(bytes);
                string message = string.Format
                    (
                        "Empty record detected:{0}{1}",
                        Environment.NewLine,
                        dump
                    );

                Log.Error
                    (
                        "IrbisNetworkUtility::ThrowIfEmptyRecord: "
                        + "empty record detected"
                    );

                IrbisNetworkException exception
                    = new IrbisNetworkException(message);
                BinaryAttachment attachment = new BinaryAttachment
                    (
                        "response",
                        bytes
                    );
                exception.Attach(attachment);
                throw exception;
            }
        }

        #endregion
    }
}
