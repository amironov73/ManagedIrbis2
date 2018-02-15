﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StringDictionary.cs -- simple string-string dictionary
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Simple "string-string" <see cref="Dictionary{T1,T2}"/>
    /// with saving-restoring facility.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class StringDictionary
        : Dictionary<string, string>,
        IHandmadeSerializable
    {
        #region Constants

        /// <summary>
        /// End-of-Dictionary mark.
        /// </summary>
        public const string EndOfDictionary = "*****";

        #endregion

        #region Public methods

        /// <summary>
        /// Loads <see cref="StringDictionary"/> from 
        /// the specified <see cref="StreamReader"/>.
        /// </summary>
        [NotNull]
        public static StringDictionary Load
            (
                [NotNull] TextReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            StringDictionary result = new StringDictionary();
            while (true)
            {
                string key;
                string value;
                if (((key = reader.ReadLine()) == null)
                    || key.StartsWith(EndOfDictionary)
                    || ((value = reader.ReadLine()) == null))
                {
                    break;
                }
                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Loads <see cref="StringDictionary"/> from the specified file.
        /// </summary>
        [NotNull]
        public static StringDictionary Load
            (
                [NotNull] string fileName,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));
            Sure.NotNull(encoding, nameof(encoding));

            using (TextReader reader = TextReaderUtility.OpenRead
                (
                    fileName,
                    encoding
                ))
            {
                return Load(reader);
            }
        }

        /// <summary>
        /// Saves the <see cref="StringDictionary"/> with specified writer.
        /// </summary>
        public void Save
            (
                [NotNull] TextWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            foreach (KeyValuePair<string, string> pair in this)
            {
                writer.WriteLine(pair.Key);
                writer.WriteLine(pair.Value);
            }

            writer.WriteLine(EndOfDictionary);
        }

        /// <summary>
        /// Saves the <see cref="StringDictionary"/> to specified file.
        /// </summary>
        public void Save
            (
                [NotNull] string fileName,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));
            Sure.NotNull(encoding, nameof(encoding));

            using (TextWriter writer = TextWriterUtility.Create
                (
                    fileName,
                    encoding
                ))
            {
                Save(writer);
            }
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream"/>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            Clear();

            int count = reader.ReadPackedInt32();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadNullableString();
                Add(key, value);
            }
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream"/>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer.WritePackedInt32(Count);
            foreach (KeyValuePair<string, string> pair in this)
            {
                writer.Write(pair.Key);
                writer.WriteNullable(pair.Value);
            }
        }

        #endregion
    }
}
