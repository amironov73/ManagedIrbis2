// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RecordField.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class RecordField
    {
        /// <summary>
        /// Метка поля.
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// Значение поля до первого разделителя.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Список подполей.
        /// </summary>
        public List<SubField> Subfields { get; } = new List<SubField>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public RecordField()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public RecordField
            (
                int tag,
                string? value = default
            )
        {
            Tag = tag;
            Value = value;
        }

        /// <summary>
        ///
        /// </summary>
        public RecordField Add
            (
                char code,
                string value
            )
        {
            var subfield = new SubField {Code = code, Value = value};
            Subfields.Add(subfield);

            return this;
        }

        /// <summary>
        ///
        /// </summary>
        public RecordField Clear()
        {
            Value = null;
            Subfields.Clear();

            return this;
        }

        /// <summary>
        ///
        /// </summary>
        public void Decode(string line)
        {
            var parts = line.Split('#', 2, StringSplitOptions.None);
            Tag = NumericUtility.ParseInt32(parts[0]);
            string body = parts[1];
            if (body[0] != '^')
            {
                int index = body.IndexOf('^');
                if (index < 0)
                {
                    Value = body;
                    return;
                }

                Value = body.Substring(0, index);
                body = body.Substring(index);
            }

            int offset = 1;
            bool flag = true;
            while (flag)
            {
                string one;
                int index = body.IndexOf('^', offset);
                if (index < 0)
                {
                    one = body.Substring(offset);
                    flag = false;
                }
                else
                {
                    one = body.Substring(offset, index - offset);
                    offset = index + 1;
                }

                var subfield = new SubField();
                subfield.Decode(one);
                Subfields.Add(subfield);
            }
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            int length = 4 + (Value?.Length ?? 0)
                + Subfields.Sum(sf => (sf.Value?.Length ?? 0) + 2);
            StringBuilder result = new StringBuilder(length);
            result.Append(Tag.ToInvariantString())
                .Append('#')
                .Append(Value);
            foreach (var subfield in Subfields)
            {
                result.Append(subfield);
            }

            return result.ToString();
        }
    }
}
