// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DatabaseInfo.cs -- информация о базе данных ИРБИС
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


using AM;
using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Информация о базе данных ИРБИС
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("{" + nameof(Name) + "} {" + nameof(Description) + "}")]
    public sealed class DatabaseInfo
        : IHandmadeSerializable
    {
        #region Constants

        /// <summary>
        /// Разделитель элементов
        /// </summary>
        public const char ItemDelimiter = (char)0x1E;

        #endregion

        #region Properties

        /// <summary>
        /// Имя базы данных.
        /// </summary>
        [CanBeNull]
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Описание базы данных
        /// </summary>
        [CanBeNull]
        [XmlAttribute("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Максимальный MFN.
        /// </summary>
        [XmlAttribute("maxMfn")]
        [JsonProperty("maxMfn", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int MaxMfn { get; set; }

        /// <summary>
        /// Список логически удаленных записей.
        /// </summary>
        [CanBeNull]
        [XmlArrayItem("mfn")]
        [XmlArray("logicallyDeleted")]
        [JsonProperty("logicallyDeleted", NullValueHandling = NullValueHandling.Ignore)]
        public int[] LogicallyDeletedRecords { get; set; }

        /// <summary>
        /// Список физически удаленных записей.
        /// </summary>
        [CanBeNull]
        [XmlArrayItem("mfn")]
        [XmlArray("physicallyDeleted")]
        [JsonProperty("physicallyDeleted", NullValueHandling = NullValueHandling.Ignore)]
        public int[] PhysicallyDeletedRecords { get; set; }

        /// <summary>
        /// Список неактуализированных записей.
        /// </summary>
        [CanBeNull]
        [XmlArrayItem("mfn")]
        [XmlArray("nonActualizedRecords")]
        [JsonProperty("nonActualizedRecords", NullValueHandling = NullValueHandling.Ignore)]
        public int[] NonActualizedRecords { get; set; }

        /// <summary>
        /// Список заблокированных записей.
        /// </summary>
        [CanBeNull]
        [XmlArrayItem("mfn")]
        [XmlArray("lockedRecords")]
        [JsonProperty("lockedRecords", NullValueHandling = NullValueHandling.Ignore)]
        public int[] LockedRecords { get; set; }

        /// <summary>
        /// Флаг монопольной блокировки базы данных.
        /// </summary>
        [XmlAttribute("databaseLocked")]
        [JsonProperty("databaseLocked", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool DatabaseLocked { get; set; }

        /// <summary>
        /// База данных только для чтения.
        /// </summary>
        [XmlAttribute("readOnly")]
        [JsonProperty("readOnly", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ReadOnly { get; set; }

        #endregion

        #region Private members

        [NotNull]
        private static int[] _ParseLine
            (
                [CanBeNull] string text
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return Array.Empty<int>();
            }

            string[] items = text.Split(ItemDelimiter);
            int[] result = items.Select(FastNumber.ParseInt32).ToArray();
            Array.Sort(result);

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Describe the database.
        /// </summary>
        public string Describe()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat
                (
                    "Name: {0}",
                    Name.ToVisibleString()
                );
            result.AppendLine();

            result.AppendFormat
                (
                    "Description: {0}",
                    Description.ToVisibleString()
                );
            result.AppendLine();

            if (!ReferenceEquals(LogicallyDeletedRecords, null))
            {
                result.Append("Logically deleted records: ");
                result.AppendLine(NumericUtility.CompressRange
                    (
                        LogicallyDeletedRecords
                    ));
            }

            if (!ReferenceEquals(PhysicallyDeletedRecords, null))
            {
                result.Append("Physically deleted records: ");
                result.AppendLine(NumericUtility.CompressRange
                    (
                        PhysicallyDeletedRecords
                    ));
            }

            if (!ReferenceEquals(NonActualizedRecords, null))
            {
                result.Append("Non-actualized records: ");
                result.AppendLine(NumericUtility.CompressRange
                    (
                        NonActualizedRecords
                    ));
            }

            if (!ReferenceEquals(LockedRecords, null))
            {
                result.Append("Locked records: ");
                result.AppendLine(NumericUtility.CompressRange
                    (
                        LockedRecords
                    ));
            }

            result.AppendFormat("Max MFN: {0}", MaxMfn);
            result.AppendLine();

            result.AppendFormat("Read-only: {0}", ReadOnly);
            result.AppendLine();

            result.AppendFormat
                (
                    "Database locked: {0}",
                    DatabaseLocked
                );
            result.AppendLine();

            return result.ToString();
        }

        /// <summary>
        /// Разбор ответа сервера.
        /// </summary>
        [NotNull]
        public static DatabaseInfo ParseServerResponse
            (
                [NotNull] ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            DatabaseInfo result = new DatabaseInfo
            {
                PhysicallyDeletedRecords = _ParseLine(response.GetAnsiString()),
                LogicallyDeletedRecords = _ParseLine(response.GetAnsiString()),
                NonActualizedRecords = _ParseLine(response.GetAnsiString()),
                LockedRecords = _ParseLine(response.GetAnsiString()),
                MaxMfn = _ParseLine(response.GetAnsiString()).GetItem(0, 0),
                DatabaseLocked = _ParseLine(response.GetAnsiString()).GetItem(0, 0) != 0
            };

            return result;
        }

        /// <summary>
        /// Разбор файла меню
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static DatabaseInfo[] ParseMenu
            (
                [NotNull] string[] text
            )
        {
            Sure.NotNull(text, nameof(text));

            List<DatabaseInfo> result = new List<DatabaseInfo>();

            for (int i = 0; i < text.Length; i += 2)
            {
                string name = text[i];
                if (string.IsNullOrEmpty(name)
                    || name.StartsWith("*"))
                {
                    break;
                }
                bool readOnly = false;
                if (name.StartsWith("-"))
                {
                    name = name.Substring(1);
                    readOnly = true;
                }
                string description = text[i + 1];
                DatabaseInfo oneBase = new DatabaseInfo
                {
                    Name = name,
                    Description = description,
                    ReadOnly = readOnly
                };
                result.Add(oneBase);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Should serialize <see cref="MaxMfn"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeMaxMfn()
        {
            return MaxMfn != 0;
        }

        /// <summary>
        /// Should serialize <see cref="LogicallyDeletedRecords"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeLogicallyDeletedRecords()
        {
            return !ReferenceEquals(LogicallyDeletedRecords, null);
        }

        /// <summary>
        /// Should serialize <see cref="PhysicallyDeletedRecords"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializePhysicallyDeletedRecords()
        {
            return !ReferenceEquals(PhysicallyDeletedRecords, null);
        }

        /// <summary>
        /// Should serialize <see cref="NonActualizedRecords"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        public bool ShouldSerializeNonActualizedRecords()
        {
            return !ReferenceEquals(NonActualizedRecords, null);
        }

        /// <summary>
        /// Should serialize <see cref="LockedRecords"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeLockedRecords()
        {
            return !ReferenceEquals(LockedRecords, null);
        }

        /// <summary>
        /// Should serialize <see cref="DatabaseLocked"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDatabaseLocked()
        {
            return DatabaseLocked;
        }

        /// <summary>
        /// Should serialize <see cref="ReadOnly"/>?
        /// </summary>
        [ExcludeFromCodeCoverage]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReadOnly()
        {
            return ReadOnly;
        }

        #endregion

        #region IHandmadeSerializable membrs

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            Name = reader.ReadNullableString();
            Description = reader.ReadNullableString();
            MaxMfn = reader.ReadPackedInt32();
            LogicallyDeletedRecords = reader.ReadNullableInt32Array();
            PhysicallyDeletedRecords = reader.ReadNullableInt32Array();
            NonActualizedRecords = reader.ReadNullableInt32Array();
            LockedRecords = reader.ReadNullableInt32Array();
            DatabaseLocked = reader.ReadBoolean();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer
                .WriteNullable(Name)
                .WriteNullable(Description)
                .WritePackedInt32(MaxMfn)
                .WriteNullableArray(LogicallyDeletedRecords)
                .WriteNullableArray(PhysicallyDeletedRecords)
                .WriteNullableArray(NonActualizedRecords)
                .WriteNullableArray(LockedRecords)
                .Write(DatabaseLocked);
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Description))
            {
                return Name.ToVisibleString();
            }

            return $"{Name} - {Description}";
        }

        #endregion
    }
}
