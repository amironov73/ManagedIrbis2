﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RecordFieldUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

using AM;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Formatting = Newtonsoft.Json.Formatting;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class RecordFieldUtility
    {
        #region Properties and fields

        /// <summary>
        /// Empty array of <see cref="RecordField"/>.
        /// </summary>
        [NotNull]
        public static readonly RecordField[] EmptyArray = new RecordField[0];

        #endregion

        #region Public methods

        /// <summary>
        /// Добавление подполя.
        /// </summary>
        [NotNull]
        public static RecordField AddNonEmptySubField
            (
                [NotNull] this RecordField field,
                char code,
                [CanBeNull] string value
            )
        {
            Sure.NotNull(field, "field");

            if (!string.IsNullOrEmpty(value))
            {
                field.AddSubField(code, value);
            }

            return field;
        }

        // ==========================================================

        /// <summary>
        /// Добавление подполей.
        /// </summary>
        [NotNull]
        public static RecordField AddSubFields
            (
                [NotNull] this RecordField field,
                [CanBeNull] IEnumerable<SubField> subFields
            )
        {
            Sure.NotNull(field, "field");

            if (!ReferenceEquals(subFields, null))
            {
                foreach (SubField subField in subFields)
                {
                    field.SubFields.Add(subField);
                }
            }

            return field;
        }

        /// <summary>
        /// Добавление подполей.
        /// </summary>
        [NotNull]
        public static RecordField AddSubFields
            (
                [NotNull] this RecordField field,
                [CanBeNull] SubField[] subFields
            )
        {
            Sure.NotNull(field, "field");

            if (!ReferenceEquals(subFields, null))
            {
                field.SubFields.AddRange(subFields);
            }

            return field;
        }

        // ==========================================================

        /// <summary>
        /// Все подполя.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] AllSubFields
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .SelectMany(field => field.SubFields)
                .NonNullItems()
                .ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Apply subfield value.
        /// </summary>
        [NotNull]
        public static RecordField ApplySubField
            (
                [NotNull] this RecordField field,
                char code,
                [CanBeNull] object value
            )
        {
            Sure.NotNull(field, "field");

            if (code == SubField.NoCode)
            {
                return field;
            }

            if (ReferenceEquals(value, null))
            {
                field.RemoveSubField(code);
            }
            else
            {
                SubField subField = field.GetFirstSubField(code);
                if (ReferenceEquals(subField, null))
                {
                    subField = new SubField(code);
                    field.SubFields.Add(subField);
                }
                subField.Value = value.ToString();
            }

            return field;
        }

        /// <summary>
        /// Apply subfield value.
        /// </summary>
        [NotNull]
        public static RecordField ApplySubField
            (
                [NotNull] this RecordField field,
                char code,
                bool value,
                [NotNull] string text
            )
        {
            Sure.NotNull(field, "field");
            Sure.NotNullNorEmpty(text, "text");

            if (code == SubField.NoCode)
            {
                return field;
            }

            if (value == false)
            {
                field.RemoveSubField(code);
            }
            else
            {
                SubField subField = field.GetFirstSubField(code);
                if (ReferenceEquals(subField, null))
                {
                    subField = new SubField(code);
                    field.SubFields.Add(subField);
                }
                subField.Value = text;
            }

            return field;
        }

        /// <summary>
        /// Apply subfield value.
        /// </summary>
        [NotNull]
        public static RecordField ApplySubField
            (
                [NotNull] this RecordField field,
                char code,
                [CanBeNull] string value
            )
        {
            Sure.NotNull(field, "field");

            if (code == SubField.NoCode)
            {
                return field;
            }

            if (string.IsNullOrEmpty(value))
            {
                field.RemoveSubField(code);
            }
            else
            {
                SubField subField = field.GetFirstSubField(code);
                if (ReferenceEquals(subField, null))
                {
                    subField = new SubField(code);
                    field.SubFields.Add(subField);
                }
                subField.Value = value;
            }

            return field;
        }

        // ==========================================================

        /// <summary>
        /// Отбор подполей с указанными кодами.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] FilterSubFields
            (
                [NotNull] this IEnumerable<SubField> subFields,
                params char[] codes
            )
        {
            Sure.NotNull(subFields, "subFields");

            return subFields
                .NonNullItems()
                .Where
                    (
                        subField => subField.Code.OneOf(codes)
                    )
                .ToArray();
        }

        /// <summary>
        /// Отбор подполей с указанными кодами.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] FilterSubFields
            (
                [NotNull] this RecordField field,
                params char[] codes
            )
        {
            Sure.NotNull(field, "field");

            return field.SubFields
                .FilterSubFields(codes);
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [CanBeNull]
        public static RecordField GetField
            (
                [NotNull] this RecordFieldCollection fields,
                int tag,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");

            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    if (occurrence == 0)
                    {
                        return fields[i];
                    }
                    occurrence--;
                }
            }

            return null;
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where(field => field.Tag == tag)
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this RecordFieldCollection fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            List<RecordField> result = null;
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<RecordField>();
                    }
                    result.Add(fields[i]);
                }
            }

            return ReferenceEquals(result, null)
                ? EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [CanBeNull]
        public static RecordField GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields)
            {
                if (field.Tag == tag)
                {
                    if (occurrence == 0)
                    {
                        return field;
                    }
                    occurrence--;
                }
            }

            return null;
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                params int[] tags
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where(field => field.Tag.OneOf(tags))
                .ToArray();
        }

        ///// <summary>
        ///// Фильтрация полей.
        ///// </summary>
        //[NotNull]
        //[ItemNotNull]
        //public static RecordField[] GetField
        //    (
        //        [NotNull] this RecordFieldCollection fields,
        //        params int[] tags
        //    )
        //{
        //    Code.NotNull(fields, "fields");

        //    List<RecordField> result = null;
        //    int count = fields.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        if (fields[i].Tag.OneOf(tags))
        //        {
        //            if (ReferenceEquals(result, null))
        //            {
        //                result = new List<RecordField>();
        //            }
        //            result.Add(fields[i]);
        //        }
        //    }

        //    return ReferenceEquals(result, null)
        //        ? EmptyArray
        //        : result.ToArray();
        //}

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [CanBeNull]
        public static RecordField GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] int[] tags,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tags, "tags");

            return fields
                .GetField(tags)
                .GetOccurrence(occurrence);
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [CanBeNull]
        public static RecordField GetField
            (
                [NotNull] this RecordFieldCollection fields,
                [NotNull] int[] tags,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tags, "tags");

            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag.OneOf(tags))
                {
                    if (occurrence == 0)
                    {
                        return fields[i];
                    }
                    occurrence--;
                }
            }

            return null;
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] Func<RecordField, bool> predicate
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(predicate, "predicate");

            return fields
                .NonNullItems()
                .Where(predicate)
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this RecordFieldCollection fields,
                [NotNull] Func<RecordField, bool> predicate
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(predicate, "predicate");

            List<RecordField> result = null;
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (predicate(fields[i]))
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<RecordField>();
                    }
                    result.Add(fields[i]);
                }
            }

            return ReferenceEquals(result, null)
                ? EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Выполнение неких действий над полями.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [CanBeNull] Action<RecordField> action
            )
        {
            Sure.NotNull(fields, "fields");

            RecordField[] result = fields.NonNullItems().ToArray();
            if (!ReferenceEquals(action, null))
            {
                foreach (RecordField field in result)
                {
                    action(field);
                }
            }

            return result;
        }

        /// <summary>
        /// Выполнение неких действий над полями и подполями.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [CanBeNull] Action<RecordField> fieldAction,
                [CanBeNull] Action<SubField> subFieldAction
            )
        {
            Sure.NotNull(fields, "fields");

            RecordField[] result = fields.NonNullItems().ToArray();
            if (!ReferenceEquals(fieldAction, null)
                || !ReferenceEquals(subFieldAction, null))
            {
                foreach (RecordField field in result)
                {
                    if (!ReferenceEquals(fieldAction, null))
                    {
                        fieldAction(field);
                    }

                    if (!ReferenceEquals(subFieldAction, null))
                    {
                        foreach (SubField subField in field.SubFields)
                        {
                            subFieldAction(subField);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Выполнение неких действий над подполями.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [CanBeNull] Action<SubField> action
            )
        {
            Sure.NotNull(fields, "fields");

            RecordField[] result = fields.NonNullItems().ToArray();
            if (!ReferenceEquals(action, null))
            {
                foreach (RecordField field in result)
                {
                    foreach (SubField subField in field.SubFields)
                    {
                        action(subField);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] Func<SubField, bool> predicate
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(predicate, "predicate");

            return fields
                .NonNullItems()
                .Where(field => field.SubFields.Any(predicate))
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] char[] codes,
                [NotNull] Func<SubField, bool> predicate
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(codes, "codes");
            Sure.NotNull(predicate, "predicate");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.SubFields
                            .NonNullItems()
                            .Any
                                (
                                    sub => sub.Code.OneOf(codes)
                                        && predicate(sub)
                                )
                    )
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] char[] codes,
                params string[] values
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(codes, "codes");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.SubFields
                            .NonNullItems()
                            .Any
                                (
                                    sub => sub.Code.OneOf(codes)
                                        && sub.Value.OneOf(values)
                                )
                    )
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                char code,
                [CanBeNull] string value
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.SubFields
                            .NonNullItems()
                            .Any
                                (
                                    sub => sub.Code.SameChar(code)
                                        && sub.Value.SameString(value)
                                )
                    )
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] int[] tags,
                [NotNull] char[] codes,
                [NotNull] string[] values
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tags, "tags");
            Sure.NotNull(codes, "codes");
            Sure.NotNull(values, "values");

            return fields
                .NonNullItems()
                .Where(field => field.Tag.OneOf(tags))
                .Where
                    (
                        field => field.SubFields
                            .Any
                                (
                                    sub => sub.Code.OneOf(codes)
                                        && sub.Value.OneOf(values)
                                )
                    )
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] Func<RecordField, bool> fieldPredicate,
                [NotNull] Func<SubField, bool> subPredicate
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(fieldPredicate, "fieldPredicate");
            Sure.NotNull(subPredicate, "subPredicate");

            return fields
                .NonNullItems()
                .Where(fieldPredicate)
                .Where(field => field.SubFields.Any(subPredicate))
                .ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Количество повторений поля.
        /// </summary>
        public static int GetFieldCount
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            int result = 0;

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Количество повторений поля.
        /// </summary>
        public static int GetFieldCount
            (
                [NotNull] this RecordFieldCollection fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            int result = 0;
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    result++;
                }
            }

            return result;
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetFieldRegex
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] string tagRegex
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tagRegex, "tagRegex");

            return fields
                .NonNullItems()
                .Where
                    (
                        field =>
                        {
                            string tag = field.Tag.ToInvariantString();

                            return !string.IsNullOrEmpty(tag)
                                && Regex.IsMatch
                                   (
                                       tag,
                                       tagRegex
                                   );
                        }
                    )
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [CanBeNull]
        public static RecordField GetFieldRegex
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] string tagRegex,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tagRegex, "tagRegex");

            return fields
                .GetFieldRegex(tagRegex)
                .GetOccurrence(occurrence);
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] GetFieldRegex
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] int[] tags,
                [NotNull] string textRegex
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tags, "tags");
            Sure.NotNull(textRegex, textRegex);

            return fields
                .GetField(tags)
                .Where
                    (
                        field => !ReferenceEquals
                            (
                                field.Value,
                                null
                            )
                    )
                .Where
                    (
                        field => Regex.IsMatch
                            (
                                field.Value,
                             textRegex
                         )
                    )
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [CanBeNull]
        public static RecordField GetFieldRegex
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] int[] tags,
                [NotNull] string textRegex,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tags, "tags");
            Sure.NotNull(textRegex, "textRegex");

            return fields
                .GetFieldRegex(tags, textRegex)
                .GetOccurrence(occurrence);
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        public static RecordField[] GetFieldRegex
            (
                [NotNull] this IEnumerable<RecordField> fields,
                [NotNull] int[] tags,
                [NotNull] char[] codes,
                [NotNull] string textRegex
            )
        {
            Sure.NotNull(fields, "fields");
            Sure.NotNull(tags, "tags");
            Sure.NotNull(codes, "codes");
            Sure.NotNull(textRegex, "textRegex");

            Regex regex = new Regex(textRegex);
            return fields
                .GetField(tags)
                .Where(field => field.FilterSubFields(codes)
                    .Where(sub => !ReferenceEquals(sub.Value, null))
                    .Any(sub => regex.IsMatch(sub.Value)))
                .ToArray();
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        public static RecordField GetFieldRegex
            (
                this IEnumerable<RecordField> fields,
                int[] tags,
                char[] codes,
                string textRegex,
                int occurrence
            )
        {
            return fields
                .GetFieldRegex(tags, codes, textRegex)
                .GetOccurrence(occurrence);
        }

        // ==========================================================

        /// <summary>
        /// Получение значения поля.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static string[] GetFieldValue
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Select
                    (
                        field => field.Value
                    )
                .NonEmptyLines()
                .ToArray();
        }

        /// <summary>
        /// Получение значения поля.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static string[] GetFieldValue
            (
                [NotNull] this RecordFieldCollection fields
            )
        {
            Sure.NotNull(fields, "fields");

            List<string> result = null;
            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                string value = fields[i].Value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<string>();
                    }
                    result.Add(value);
                }
            }

            return ReferenceEquals(result, null)
                ? StringUtility.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Получение значения поля.
        /// </summary>
        [CanBeNull]
        public static string GetFieldValue
            (
                [CanBeNull] this RecordField field
            )
        {
            return ReferenceEquals(field, null)
                ? null
                : field.Value;
        }

        /// <summary>
        /// Непустые значения полей с указанным тегом.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static string[] GetFieldValue
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            List<string> result = new List<string>();

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag
                    && !string.IsNullOrEmpty(field.Value))
                {
                    result.Add(field.Value);
                }
            }

            return result.ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Первое вхождение поля с указанным тегом.
        /// </summary>
        [CanBeNull]
        public static RecordField GetFirstField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    return field;
                }
            }

            return null;
        }

        /// <summary>
        /// Первое вхождение поля с указанным тегом.
        /// </summary>
        [CanBeNull]
        public static RecordField GetFirstField
            (
                [NotNull] this RecordFieldCollection fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    return fields[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Первое вхождение поля с любым из перечисленных тегов.
        /// </summary>
        [CanBeNull]
        public static RecordField GetFirstField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                params int[] tags
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag.OneOf(tags))
                {
                    return field;
                }
            }

            return null;
        }

        /// <summary>
        /// Первое вхождение поля с любым из перечисленных тегов.
        /// </summary>
        [CanBeNull]
        public static RecordField GetFirstField
            (
                [NotNull] this RecordFieldCollection fields,
                params int[] tags
            )
        {
            Sure.NotNull(fields, "fields");

            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag.OneOf(tags))
                {
                    return fields[i];
                }
            }

            return null;
        }

        // ==========================================================

        ///// <summary>
        ///// Значение первого поля с указанным тегом или <c>null</c>.
        ///// </summary>
        //[CanBeNull]
        //public static string GetFirstFieldValue
        //    (
        //        [NotNull] this IEnumerable<RecordField> fields,
        //        int tag
        //    )
        //{
        //    Code.NotNull(fields, "fields");

        //    foreach (RecordField field in fields)
        //    {
        //        if (field.Tag == tag)
        //        {
        //            return field.Value;
        //        }
        //    }

        //    return null;
        //}

        /// <summary>
        /// Значение первого поля с указанным тегом или <c>null</c>.
        /// </summary>
        [CanBeNull]
        public static string GetFirstFieldValue
            (
                [NotNull] this RecordFieldCollection fields,
                int tag
            )
        {
            Sure.NotNull(fields, "fields");

            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    return fields[i].Value;
                }
            }

            return null;
        }

        // ==========================================================

        /// <summary>
        /// Gets the first subfield.
        /// </summary>
        [CanBeNull]
        public static SubField GetFirstSubField
            (
                [NotNull] this RecordField field,
                char code
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    return subFields[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Первое вхождение подполя, соответствующего указанным
        /// критериям.
        /// </summary>
        [CanBeNull]
        public static SubField GetFirstSubField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag,
                char code
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    SubFieldCollection subFields = field.SubFields;
                    int count = subFields.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (subFields[i].Code.SameChar(code))
                        {
                            return subFields[i];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Первое вхождение подполя, соответствующего указанным
        /// критериям.
        /// </summary>
        [CanBeNull]
        public static SubField GetFirstSubField
            (
                [NotNull] this RecordFieldCollection fields,
                int tag,
                char code
            )
        {
            Sure.NotNull(fields, "fields");

            int count = fields.Count;
            for (int i = 0; i < count; i++)
            {
                RecordField field = fields[i];
                if (field.Tag == tag)
                {
                    foreach (SubField subField in field.SubFields)
                    {
                        if (subField.Code.SameChar(code))
                        {
                            return subField;
                        }
                    }
                }
            }

            return null;
        }

        // ==========================================================

        /// <summary>
        /// Получение текста указанного подполя
        /// </summary>
        [CanBeNull]
        public static string GetFirstSubFieldValue
            (
                [NotNull] this RecordField field,
                char code
            )
        {
            Sure.NotNull(field, "field");

            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    return subFields[i].Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Значение первого подполя с указанными тегом и кодом
        /// или <c>null</c>.
        /// </summary>
        [CanBeNull]
        public static string GetFirstSubFieldValue
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag,
                char code
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    foreach (SubField subField in field.SubFields)
                    {
                        if (subField.Code.SameChar(code))
                        {
                            return subField.Value;
                        }
                    }
                }
            }

            return null;
        }

        // ==========================================================

        /// <summary>
        /// Перечень подполей с указанным кодом.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] GetSubField
            (
                [NotNull] this RecordField field,
                char code
            )
        {
            Sure.NotNull(field, "field");

            List<SubField> result = null;
            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<SubField>();
                    }
                    result.Add(subFields[i]);
                }
            }

            return ReferenceEquals(result, null)
                ? SubFieldUtility.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Указанное повторение подполя с данным кодом.
        /// </summary>
        [CanBeNull]
        public static SubField GetSubField
            (
                [NotNull] this RecordField field,
                char code,
                int occurrence
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    if (occurrence == 0)
                    {
                        return subFields[i];
                    }
                    occurrence--;
                }
            }

            return null;
        }

        /// <summary>
        /// Получение подполей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] GetSubField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                char code
            )
        {
            Sure.NotNull(fields, "fields");

            List<SubField> result = null;
            foreach (RecordField field in fields.NonNullItems())
            {
                SubFieldCollection subFields = field.SubFields;
                int count = subFields.Count;
                for (int i = 0; i < count; i++)
                {
                    if (subFields[i].Code.SameChar(code))
                    {
                        if (ReferenceEquals(result, null))
                        {
                            result = new List<SubField>();
                        }
                        result.Add(subFields[i]);
                    }
                }
            }

            return ReferenceEquals(result, null)
                ? SubFieldUtility.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Получение подполей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] GetSubField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                params char[] codes
            )
        {
            Sure.NotNull(fields, "fields");

            List<SubField> result = null;
            foreach (RecordField field in fields.NonNullItems())
            {
                SubFieldCollection subFields = field.SubFields;
                int count = subFields.Count;
                for (int i = 0; i < count; i++)
                {
                    if (subFields[i].Code.OneOf(codes))
                    {
                        if (ReferenceEquals(result, null))
                        {
                            result = new List<SubField>();
                        }
                        result.Add(subFields[i]);
                    }
                }
            }

            return ReferenceEquals(result, null)
                ? SubFieldUtility.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Получение подполей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] GetSubField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag,
                char code
            )
        {
            Sure.NotNull(fields, "fields");

            List<SubField> result = new List<SubField>();

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    foreach (SubField subField in field.SubFields)
                    {
                        if (subField.Code.SameChar(code))
                        {
                            result.Add(subField);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Получение подполей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static SubField[] GetSubField
            (
                [NotNull] this RecordFieldCollection fields,
                [CanBeNull] string tag,
                char code
            )
        {
            Sure.NotNull(fields, "fields");

            List<SubField> result = null;
            int fieldCount = fields.Count;
            for (int i = 0; i < fieldCount; i++)
            {
                RecordField field = fields[i];
                SubFieldCollection subFields = field.SubFields;
                int subCount = subFields.Count;
                for (int j = 0; j < subCount; j++)
                {
                    if (subFields[j].Code.SameChar(code))
                    {
                        if (ReferenceEquals(result, null))
                        {
                            result = new List<SubField>();
                        }
                        result.Add(subFields[j]);
                    }
                }
            }

            return ReferenceEquals(result, null)
                ? SubFieldUtility.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Получение подполя.
        /// </summary>
        [CanBeNull]
        public static SubField GetSubField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag,
                int fieldOccurrence,
                char code,
                int subOccurrence
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    if (fieldOccurrence == 0)
                    {
                        SubFieldCollection subFields = field.SubFields;
                        int subCount = subFields.Count;
                        for (int j = 0; j < subCount; j++)
                        {
                            if (subFields[j].Code.SameChar(code))
                            {
                                if (subOccurrence == 0)
                                {
                                    return subFields[j];
                                }
                                subOccurrence--;
                            }
                        }

                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Получение подполя.
        /// </summary>
        [CanBeNull]
        public static SubField GetSubField
            (
                [NotNull] this RecordFieldCollection fields,
                int tag,
                int fieldOccurrence,
                char code,
                int subOccurrence
            )
        {
            Sure.NotNull(fields, "fields");

            int fieldCount = fields.Count;
            for (int i = 0; i < fieldCount; i++)
            {
                RecordField field = fields[i];
                if (field.Tag == tag)
                {
                    if (fieldOccurrence == 0)
                    {
                        SubFieldCollection subFields = field.SubFields;
                        int subCount = subFields.Count;
                        for (int j = 0; j < subCount; j++)
                        {
                            if (subFields[j].Code.SameChar(code))
                            {
                                if (subOccurrence == 0)
                                {
                                    return subFields[j];
                                }
                                subOccurrence--;
                            }
                        }

                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Получение подполя.
        /// </summary>
        public static SubField GetSubField
            (
                [NotNull] this IEnumerable<RecordField> fields,
                int tag,
                char code,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");

            foreach (RecordField field in fields.NonNullItems())
            {
                if (field.Tag == tag)
                {
                    SubFieldCollection subFields = field.SubFields;
                    int subCount = subFields.Count;
                    for (int j = 0; j < subCount; j++)
                    {
                        if (subFields[j].Code.SameChar(code))
                        {
                            if (occurrence == 0)
                            {
                                return subFields[j];
                            }
                            occurrence--;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Получение подполя.
        /// </summary>
        [CanBeNull]
        public static SubField GetSubField
            (
                [NotNull] this RecordFieldCollection fields,
                int tag,
                char code,
                int occurrence
            )
        {
            Sure.NotNull(fields, "fields");

            int fieldCount = fields.Count;
            for (int i = 0; i < fieldCount; i++)
            {
                RecordField field = fields[i];
                if (field.Tag == tag)
                {
                    SubFieldCollection subFields = field.SubFields;
                    int subCount = subFields.Count;
                    for (int j = 0; j < subCount; j++)
                    {
                        if (subFields[j].Code.SameChar(code))
                        {
                            if (occurrence == 0)
                            {
                                return subFields[j];
                            }
                            occurrence--;
                        }
                    }
                }
            }

            return null;
        }

        // ==========================================================

        /// <summary>
        /// Получение текста указанного подполя.
        /// </summary>
        [CanBeNull]
        public static string GetSubFieldValue
        (
            [NotNull] this RecordField field,
            char code,
            int occurrence
        )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    if (occurrence == 0)
                    {
                        return subFields[i].Value;
                    }
                    occurrence--;
                }
            }

            return null;
        }

        // ==========================================================

        /// <summary>
        /// Непустые значения подполей с указанными тегом и кодом.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static string[] GetSubFieldValue
        (
            [NotNull] this IEnumerable<RecordField> fields,
            int tag,
            char code
        )
        {
            Sure.NotNull(fields, "fields");

            List<string> result = new List<string>();

            foreach (RecordField field in fields)
            {
                if (field.Tag == tag)
                {
                    foreach (SubField subField in field.SubFields)
                    {
                        if (subField.Code.SameChar(code)
                            && !string.IsNullOrEmpty(subField.Value))
                        {
                            result.Add(subField.Value);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Есть хотя бы одно подполе с указанным кодом?
        /// </summary>
        public static bool HaveSubField
            (
                [NotNull] this RecordField field,
                char code
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Есть хотя бы одно поле с любым из указанных кодов?
        /// </summary>
        public static bool HaveSubField
            (
                [NotNull] this RecordField field,
                params char[] codes
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.OneOf(codes))
                {
                    return true;
                }
            }

            return false;
        }

        // ==========================================================

        /// <summary>
        /// Нет ни одного подполя с указанным кодом?
        /// </summary>
        public static bool HaveNotSubField
            (
                [NotNull] this RecordField field,
                char code
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.SameChar(code))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Нет ни одного подполя с указанными кодами?
        /// </summary>
        public static bool HaveNotSubField
            (
                [NotNull] this RecordField field,
                params char[] codes
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                if (subFields[i].Code.OneOf(codes))
                {
                    return false;
                }
            }

            return true;
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] NotNullTag
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.Tag != 0
                    )
                .ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] NotNullValue
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => !string.IsNullOrEmpty(field.Value)
                    )
                .ToArray();
        }

        // ==========================================================

        ///// <summary>
        ///// Удаляем подполе.
        ///// </summary>
        ///// <remarks>Удаляет все повторения подполей
        ///// с указанным кодом.
        ///// </remarks>
        //[NotNull]
        //public static RecordField RemoveSubField
        //    (
        //        [NotNull] this RecordField field,
        //        char code
        //    )
        //{
        //    SubField[] found = field.SubFields
        //        .FindAll(_ => char.ToLowerInvariant(_.Code) == code)
        //        .ToArray();

        //    foreach (SubField subField in found)
        //    {
        //        field.SubFields.Remove(subField);
        //    }

        //    return field;
        //}

        // ==========================================================

        /// <summary>
        /// Меняем значение подполя.
        /// </summary>
        [NotNull]
        public static RecordField ReplaceSubField
            (
                [NotNull] this RecordField field,
                char code,
                [CanBeNull] string oldValue,
                [CanBeNull] string newValue
            )
        {
            Sure.NotNull(field, "field");

            SubFieldCollection subFields = field.SubFields;
            int count = subFields.Count;
            for (int i = 0; i < count; i++)
            {
                SubField subField = subFields[i];
                if (subField.Code.SameChar(code))
                {
                    if (subField.Value.SameStringSensitive(oldValue))
                    {
                        subField.SetValue(newValue);
                    }
                }
            }

            return field;
        }

        // ==========================================================

        /// <summary>
        /// Меняем значение подполя.
        /// </summary>
        [NotNull]
        public static RecordField ReplaceSubField
            (
                [NotNull] this RecordField field,
                char code,
                [CanBeNull] string newValue,
                bool ignoreCase
            )
        {
            string oldValue = field.GetSubFieldValue
                (
                    code,
                    0
                );
            bool changed = string.Compare
                (
                    oldValue,
                    newValue,
                    StringComparison.CurrentCultureIgnoreCase
                ) != 0;

            if (changed)
            {
                field.SetSubField(code, newValue);
            }

            return field;

        }

        // ==========================================================

        /// <summary>
        /// Get unknown subfields.
        /// </summary>
        [CanBeNull]
        [ItemNotNull]
        public static SubField[] GetUnknownSubFields
            (
                [NotNull] this IEnumerable<SubField> subFields,
                [NotNull] string knownCodes
            )
        {
            Sure.NotNull(subFields, "subFields");
            Sure.NotNullNorEmpty(knownCodes, "knownCodes");

            List<SubField> result = null;
            foreach (SubField subField in subFields)
            {
                if (!subField.Code.OneOf(knownCodes))
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<SubField>();
                    }
                    result.Add(subField);
                }
            }

            return ReferenceEquals(result, null)
                ? SubFieldUtility.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Get unknown subfields.
        /// </summary>
        [CanBeNull]
        [ItemNotNull]
        public static SubField[] GetUnknownSubFields
            (
                [NotNull] this SubFieldCollection subFields,
                [NotNull] string knownCodes
            )
        {
            Sure.NotNull(subFields, "subFields");
            Sure.NotNullNorEmpty(knownCodes, "knownCodes");

            List<SubField> result = null;
            for (int i = 0; i < subFields.Count; i++)
            {
                if (!subFields[i].Code.OneOf(knownCodes))
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<SubField>();
                    }
                    result.Add(subFields[i]);
                }
            }

            return ReferenceEquals(result, null)
                ? SubFieldUtility.EmptyArray
                : result.ToArray();
        }

        // ==========================================================

#if !WINMOBILE && !PocketPC

        /// <summary>
        /// Convert the field to <see cref="JObject"/>.
        /// </summary>
        [NotNull]
        public static JObject ToJObject
            (
                [NotNull] this RecordField field
            )
        {
            Sure.NotNull(field, "field");

            JObject result = JObject.FromObject(field);

            return result;
        }

        /// <summary>
        /// Convert the field to JSON.
        /// </summary>
        [NotNull]
        public static string ToJson
            (
                [NotNull] this RecordField field
            )
        {
            Sure.NotNull(field, "field");

            string result = JObject.FromObject(field)
                .ToString(Formatting.None);

            return result;
        }

        /// <summary>
        /// Restore field from <see cref="JObject"/>.
        /// </summary>
        [NotNull]
        public static RecordField FromJObject
            (
                [NotNull] JObject jObject
            )
        {
            Sure.NotNull(jObject, "jObject");

            RecordField result = jObject.ToObject<RecordField>();

            return result;
        }

        /// <summary>
        /// Restore subfield from JSON.
        /// </summary>
        public static RecordField FromJson
            (
                [NotNull] string text
            )
        {
            Sure.NotNullNorEmpty(text, "text");

            RecordField result = JsonConvert.DeserializeObject<RecordField>(text);

            return result;
        }

#endif

        // ==========================================================

        /// <summary>
        /// Парсинг текстового представления поля
        /// </summary>
        public static RecordField Parse
            (
                string tag,
                string body
            )
        {
            RecordField result = new RecordField(tag);

            int first = body.IndexOf(RecordField.Delimiter);
            if (first != 0)
            {
                if (first < 0)
                {
                    result.Value = body;
                    body = string.Empty;
                }
                else
                {
                    result.Value = body.Substring
                        (
                            0,
                            first
                        );
                    body = body.Substring(first);
                }
            }

            var code = (char)0;
            var value = new StringBuilder();
            foreach (char c in body)
            {
                if (c == RecordField.Delimiter)
                {
                    if (code != '\0')
                    {
                        result.AddSubField
                            (
                                code,
                                value
                            );
                    }
                    value.Length = 0;
                    code = (char)0;
                }
                else
                {
                    if (code == 0)
                    {
                        code = c;
                    }
                    else
                    {
                        value.Append(c);
                    }
                }
            }

            if (code != (char)0)
            {
                result.AddSubField
                (
                    code,
                    value
                );
            }

            return result;
        }

        /// <summary>
        /// Парсинг строкового представления поля.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public static RecordField Parse
            (
                string line
            )
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }
            string[] parts = line.SplitFirst('#');
            string tag = parts[0];
            string body = parts[1];
            return Parse
                (
                    tag,
                    body
                );
        }

        /// <summary>
        /// Converts the field to XML.
        /// </summary>
        [NotNull]
        public static string ToXml
            (
                [NotNull] this RecordField field
            )
        {
            Sure.NotNull(field, "field");

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                NewLineOnAttributes = false,
                Indent = true,
                CloseOutput = true
            };
            StringWriter writer = new StringWriter();
            XmlWriter xml = XmlWriter.Create(writer, settings);
            XmlSerializer serializer = new XmlSerializer
                (
                    typeof(RecordField)
                );
            serializer.Serialize(xml, field);

            return writer.ToString();
        }

        /// <summary>
        /// Restore the field from XML.
        /// </summary>
        [NotNull]
        public static RecordField FromXml
            (
                [NotNull] string text
            )
        {
            Sure.NotNullNorEmpty(text, "text");

            XmlSerializer serializer = new XmlSerializer(typeof(RecordField));
            StringReader reader = new StringReader(text);
            RecordField result = (RecordField)serializer.Deserialize(reader);

            return result;
        }

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] WithNullTag
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.Tag == 0
                    )
                .ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static RecordField[] WithNullValue
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field =>
                            string.IsNullOrEmpty(field.Value)
                    )
                .ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        public static RecordField[] WithoutSubFields
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.SubFields.Count == 0
                    )
                .ToArray();
        }

        // ==========================================================

        /// <summary>
        /// Фильтрация полей.
        /// </summary>
        [NotNull]
        public static RecordField[] WithSubFields
            (
                [NotNull] this IEnumerable<RecordField> fields
            )
        {
            Sure.NotNull(fields, "fields");

            return fields
                .NonNullItems()
                .Where
                    (
                        field => field.SubFields.Count != 0
                    )
                .ToArray();
        }

        #endregion
    }
}

