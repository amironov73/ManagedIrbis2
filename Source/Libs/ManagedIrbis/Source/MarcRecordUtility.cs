﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MarcRecordUtility.cs -- extensions for MarcRecord
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using AM;


using JetBrains.Annotations;

using ManagedIrbis.ImportExport;
using ManagedIrbis.Infrastructure;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using YamlDotNet;
using YamlDotNet.Serialization;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Extension methods for <see cref="MarcRecord"/>.
    /// </summary>
    [PublicAPI]
    public static class MarcRecordUtility
    {
        #region Public methods

        /// <summary>
        /// Add the field to the record.
        /// </summary>
        [NotNull]
        public static MarcRecord AddField
            (
                [NotNull] this MarcRecord record,
                [NotNull] RecordField field
            )
        {
            Sure.NotNull(record, "record");
            Sure.NotNull(field, "field");

            record.Fields.Add(field);

            return record;
        }

        /// <summary>
        /// Add the field to the record.
        /// </summary>
        [NotNull]
        public static MarcRecord AddField
            (
                [NotNull] this MarcRecord record,
                int tag,
                [NotNull] object value
            )
        {
            Sure.NotNull(record, "record");
            Sure.NotNull(value, "value");

            RecordField field = value as RecordField;
            if (!ReferenceEquals(field, null))
            {
                Debug.Assert(tag == field.Tag, "tag == field.Tag");
            }
            else
            {
                string text = value.ToString();
                if (!string.IsNullOrEmpty(text))
                {
                    field = RecordField.Parse(tag, value.ToString());
                }
                else
                {
                    field = new RecordField(tag);
                }
            }
            record.Fields.Add(field);

            return record;
        }

        /// <summary>
        /// Add non-empty field.
        /// </summary>
        [NotNull]
        public static MarcRecord AddNonEmptyField
            (
                [NotNull] this MarcRecord record,
                int tag,
                [CanBeNull] object value
            )
        {
            Sure.NotNull(record, "record");

            if (!ReferenceEquals(value, null))
            {
                RecordField field = value as RecordField;
                if (!ReferenceEquals(field, null))
                {
                    Debug.Assert(tag == field.Tag, "tag == field.Tag");
                    record.Fields.Add(field);
                }
                else
                {
                    string text = value.ToString();
                    if (!string.IsNullOrEmpty(text))
                    {
                        field = new RecordField(tag, text);
                        record.Fields.Add(field);
                    }
                }
            }

            return record;
        }

        /// <summary>
        /// Begin update the record.
        /// </summary>
        [NotNull]
        public static MarcRecord BeginUpdate
            (
                [NotNull] this MarcRecord record
            )
        {
            record.Fields.BeginUpdate();

            return record;
        }

        /// <summary>
        /// Begin update the record.
        /// </summary>
        [NotNull]
        public static MarcRecord BeginUpdate
            (
                [NotNull] this MarcRecord record,
                int delta
            )
        {
            record.Fields.BeginUpdate();
            record.Fields.AddCapacity(delta);
            record.Modified = false;

            return record;
        }

        /// <summary>
        /// End of the record update.
        /// </summary>
        [NotNull]
        public static MarcRecord EndUpdate
            (
                [NotNull] this MarcRecord record
            )
        {
            record.Fields.EndUpdate();

            return record;
        }

        /// <summary>
        /// Есть хотя бы одно поле с указанными тегами?
        /// </summary>
        public static bool HaveField
            (
                [NotNull] this MarcRecord record,
                params int[] tags
            )
        {
            RecordFieldCollection fields = record.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Tag.OneOf(tags))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Есть хотя бы одно поле с указанным тегом?
        /// </summary>
        public static bool HaveField
            (
                [NotNull] this MarcRecord record,
                int tag
            )
        {
            RecordFieldCollection fields = record.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Нет ни одного поля с указанными тегами?
        /// </summary>
        public static bool HaveNotField
        (
            this MarcRecord record,
            params int[] tags
        )
        {
            RecordFieldCollection fields = record.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Tag.OneOf(tags))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Нет ни одного поля с указанным тегом?
        /// </summary>
        public static bool HaveNotField
            (
                this MarcRecord record,
                int tag
            )
        {
            RecordFieldCollection fields = record.Fields;
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse ALL-formatted records in server response.
        /// </summary>
        [NotNull]
        public static MarcRecord[] ParseAllFormat
            (
                [NotNull] string database,
                [NotNull] ServerResponse response
            )
        {
            Sure.NotNullNorEmpty(database, "database");
            Sure.NotNull(response, "response");

            List<MarcRecord> result = new List<MarcRecord>();

            while (true)
            {
                MarcRecord record = new MarcRecord
                {
                    HostName = response.Connection.Host,
                    Database = database
                };
                record = ProtocolText.ParseResponseForAllFormat
                    (
                        response,
                        record
                    );
                if (ReferenceEquals(record, null))
                {
                    break;
                }
                result.Add(record);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Parse ALL-formatted records in server response.
        /// </summary>
        [NotNull]
        public static MarcRecord[] ParseAllFormat
            (
                [NotNull] string database,
                [NotNull] IIrbisConnection connection,
                [NotNull] string[] lines
            )
        {
            Sure.NotNullNorEmpty(database, "database");
            Sure.NotNull(connection, "connection");
            Sure.NotNull(lines, "lines");

            List<MarcRecord> result = new List<MarcRecord>();

            foreach (string line in lines)
            {
                MarcRecord record = new MarcRecord
                {
                    HostName = connection.Host,
                    Database = database
                };
                record = ProtocolText.ParseResponseForAllFormat
                    (
                        line,
                        record
                    );

                // coverity[NULL_RETURNS]
                result.Add(record.ThrowIfNull("record"));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Remove all the fields with specified tag.
        /// </summary>
        [NotNull]
        public static MarcRecord RemoveField
            (
                [NotNull] this MarcRecord record,
                int tag
            )
        {
            Sure.NotNull(record, "record");

            RecordField[] found = record.Fields.GetField(tag);
            foreach (RecordField field in found)
            {
                record.Fields.Remove(field);
            }

            return record;
        }

        /// <summary>
        /// Replace fields with specified tag.
        /// </summary>
        [NotNull]
        public static MarcRecord ReplaceField
            (
                [NotNull] this MarcRecord record,
                int tag,
                [NotNull] IEnumerable<RecordField> newFields
            )
        {
            Sure.NotNull(record, "record");
            Sure.NotNull(newFields, "newFields");

            record.RemoveField(tag);
            record.Fields.AddRange(newFields);

            return record;
        }

        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Устанавливает значение только для
        /// первого повторения поля (если в записи их несколько)!
        /// </remarks>
        [NotNull]
        public static MarcRecord SetField
            (
                [NotNull] this MarcRecord record,
                int tag,
                [CanBeNull] string value
            )
        {
            Sure.NotNull(record, "record");
            Sure.Positive(tag, "tag");

            RecordField field = record.Fields.GetFirstField(tag);

            if (ReferenceEquals(field, null))
            {
                field = new RecordField(tag);
                record.Fields.Add(field);
            }

            field.SubFields.Clear();
            field.Value = value;

            return record;
        }

        /// <summary>
        /// Установка поля.
        /// </summary>
        [NotNull]
        public static MarcRecord SetField
            (
                [NotNull] this MarcRecord record,
                int tag,
                int occurrence,
                string newText
            )
        {
            Sure.NotNull(record, "record");
            Sure.Positive(tag, "tag");

            RecordField field = record.Fields.GetField(tag, occurrence);

            if (!ReferenceEquals(field, null))
            {
                field.SubFields.Clear();
                field.Value = newText;
            }

            return record;
        }

        /// <summary>
        /// Установка подполя.
        /// </summary>
        [NotNull]
        public static MarcRecord SetSubField
            (
                [NotNull] this MarcRecord record,
                int tag,
                char code,
                [CanBeNull] string newValue
            )
        {
            Sure.NotNull(record, "record");
            Sure.Positive(tag, "tag");

            RecordField field = record.Fields.GetFirstField(tag);

            if (ReferenceEquals(field, null))
            {
                field = new RecordField(tag);
                record.Fields.Add(field);
            }

            field.SetSubField(code, newValue);

            return record;
        }

        /// <summary>
        /// Установка подполя.
        /// </summary>
        [NotNull]
        public static MarcRecord SetSubField
            (
                [NotNull] this MarcRecord record,
                int tag,
                int fieldOccurrence,
                char code,
                int subFieldOccurrence,
                [CanBeNull] string newValue
            )
        {
            Sure.NotNull(record, "record");
            Sure.Positive(tag, "tag");

            RecordField field = record.Fields.GetField(tag, fieldOccurrence);

            if (!ReferenceEquals(field, null))
            {
                SubField subField = field.GetSubField(code, subFieldOccurrence);
                if (!ReferenceEquals(subField, null))
                {
                    subField.Value = newValue;
                }
            }

            return record;
        }

        /// <summary>
        /// Convert the <see cref="MarcRecord"/> to JSON.
        /// </summary>
        [NotNull]
        public static string ToJson
            (
                [NotNull] this MarcRecord record
            )
        {
            Sure.NotNull(record, "record");

#if WINMOBILE || PocketPC

            throw new System.NotImplementedException();

#else

            string result = JObject.FromObject(record)
                .ToString(Formatting.None);

            return result;

#endif
        }

        /// <summary>
        /// Convert the <see cref="MarcRecord"/> to YAML.
        /// </summary>
        [NotNull]
        public static string ToYaml
            (
                [NotNull] this MarcRecord record
            )
        {
            Sure.NotNull(record, "record");

#if !CLASSIC && !NETCORE

            throw new System.NotImplementedException();

#else

            Serializer serializer = new Serializer();
            string result = serializer.Serialize(record);

            return result;

#endif
        }

        #endregion
    }
}
