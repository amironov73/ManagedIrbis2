﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NativeRecord.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

using AM;
using AM.Collections;

using JetBrains.Annotations;

using ManagedIrbis;

#endregion

namespace IrbisInterop
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class NativeRecord
    {
        #region Properties

        /// <summary>
        /// MFN.
        /// </summary>
        public int Mfn { get; set; }

        /// <summary>
        /// Fields collection.
        /// </summary>
        [NotNull]
        public NonNullCollection<NativeField> Fields { get; private set; }

        /// <summary>
        /// Version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Status.
        /// </summary>
        public int Status { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public NativeRecord()
        {
            Fields = new NonNullCollection<NativeField>();
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Convert <see cref="MarcRecord"/>
        /// to <see cref="NativeRecord"/>.
        /// </summary>
        [NotNull]
        public static NativeRecord FromMarcRecord
            (
                [NotNull] MarcRecord record
            )
        {
            Sure.NotNull(record, nameof(record));

            NativeRecord result = new NativeRecord
            {
                Mfn = record.Mfn,
                Version = record.Version,
                Status = (int) record.Status
            };

            foreach (RecordField field in record.Fields)
            {
                result.Fields.Add
                    (
                        NativeField.FromRecordField(field)
                    );
            }

            return result;
        }

        /// <summary>
        /// Get fields by the tag.
        /// </summary>
        [NotNull]
        public NativeField[] GetField
            (
                int tag
            )
        {
            NonNullCollection<NativeField> fields = Fields;
            int count = fields.Count;
            List<NativeField> result = null;
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Tag == tag)
                {
                    if (ReferenceEquals(result, null))
                    {
                        result = new List<NativeField>();
                    }
                    result.Add(fields[i]);
                }
            }

            return ReferenceEquals(result, null)
                ? NativeField.EmptyArray
                : result.ToArray();
        }

        /// <summary>
        /// Get first field with given tag.
        /// </summary>
        [CanBeNull]
        public NativeField GetFirstField
            (
                int tag
            )
        {
            NonNullCollection<NativeField> fields = Fields;
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
        /// Parse memory representation of rectord.
        /// </summary>
        [NotNull]
        public static NativeRecord ParseMemory
            (
                [NotNull] byte[] memory
            )
        {
            Sure.NotNull(memory, nameof(memory));

            NativeRecord result = new NativeRecord
            {
                Mfn = BitConverter.ToInt32(memory, 0)
            };

            int totalLength = BitConverter.ToInt32(memory, 4);
            if (totalLength < memory.Length)
            {
                throw new IrbisException();
            }

            // ReSharper disable UnusedVariable

            // Always zero. Padding?
            int unknown1 = BitConverter.ToInt32(memory, 8);
            int unknown2 = BitConverter.ToInt32(memory, 12);

            // ReSharper restore UnusedVariable

            int dataOffset = BitConverter.ToInt32(memory, 0x10);
            int fieldCount = BitConverter.ToInt32(memory, 0x14);
            result.Version = BitConverter.ToInt32(memory, 0x14);
            result.Status = BitConverter.ToInt32(memory, 0x1C);
            int offset = 0x20;
            Encoding encoding = new UTF8Encoding(false, true);
            int end = 0;

            for (int index = 0; index < fieldCount; index++)
            {
                int tag = BitConverter.ToInt32(memory, offset);
                int fieldOffset = BitConverter.ToInt32(memory, offset + 4);
                if (fieldOffset != end)
                {
                    throw new IrbisException();
                }
                int fieldLentgh = BitConverter.ToInt32(memory, offset + 8);
                end = fieldOffset + fieldLentgh;
                string text = encoding.GetString
                    (
                        memory,
                        dataOffset + fieldOffset,
                        fieldLentgh
                    );

                NativeField field = new NativeField
                {
                    Tag = tag,
                    Value = text
                };
                result.Fields.Add(field);

                offset += 12;
            }

            return result;
        }

        /// <summary>
        /// Convert <see cref="NativeRecord"/>
        /// to <see cref="MarcRecord"/>.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public MarcRecord ToMarcRecord()
        {
            MarcRecord result = new MarcRecord()
            {
                Mfn = Mfn,
                Version = Version,
                Status = (RecordStatus) Status
            };

            foreach (NativeField field in Fields)
            {
                result.Fields.Add
                    (
                        field.ToRecordField()
                    );
            }

            return result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat
                (
                    "MFN={0}, Version={1}, Status={2}",
                    Mfn,
                    Version,
                    Status
                );
            result.AppendLine();

            foreach (NativeField field in Fields)
            {
                result.Append(field);
                result.AppendLine();
            }

            return result.ToString();
        }

        #endregion
    }
}
