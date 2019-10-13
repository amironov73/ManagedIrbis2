// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisTreeFile.cs -- TRE files handling
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using AM;
using AM.Collections;
using AM.IO;
using AM.Logging;
using AM.Runtime;

using JetBrains.Annotations;

using ManagedIrbis.Menus;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// TRE files handling
    /// </summary>
    [PublicAPI]
    public sealed class IrbisTreeFile
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Constants

        /// <summary>
        /// Tabulation
        /// </summary>
        public const char Indent = '\x09';

        #endregion

        #region Nested classes

        /// <summary>
        /// Tree item
        /// </summary>
        [PublicAPI]
        [DebuggerDisplay("{" + nameof(Value) + "}")]
        public sealed class Item
            : IHandmadeSerializable,
            IVerifiable
        {
            #region Properties

            /// <summary>
            /// Children.
            /// </summary>
            [ItemNotNull]
            [JsonProperty("children")]
            public NonNullCollection<Item> Children { get; }

            /// <summary>
            /// Delimiter.
            /// </summary>
            public static string? Delimiter
            {
                get => _delimiter;
                set => SetDelimiter(value);
            }

            /// <summary>
            /// Prefix.
            /// </summary>
            [JsonIgnore]
            public string? Prefix => _prefix;

            /// <summary>
            /// Suffix.
            /// </summary>
            [JsonIgnore]
            public string? Suffix => _suffix;

            /// <summary>
            /// Value.
            /// </summary>
            [JsonProperty("value")]
            public string? Value
            {
                get => _value;
                set => SetValue(value);
            }

            #endregion

            #region Construction

            /// <summary>
            /// Constructor.
            /// </summary>
            public Item()
            {
                Children = new NonNullCollection<Item>();
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Item
                (
                    string? value
                )
                : this()
            {
                SetValue(value);
            }

            #endregion

            #region Private members

            private static string? _delimiter = " - ";

            private string? _prefix, _suffix, _value;

            internal int Level;

            #endregion

            #region Public methods

            /// <summary>
            /// Add child.
            /// </summary>
            public Item AddChild
                (
                    string? value
                )
            {
                var result = new Item(value);
                Children.Add(result);

                return result;
            }

            /// <summary>
            /// Set the delimiter.
            /// </summary>
            public static void SetDelimiter
                (
                    string? value
                )
            {
                _delimiter = value;
            }

            /// <summary>
            /// Set the value.
            /// </summary>
            public void SetValue
                (
                    string? value
                )
            {
                //Sure.NotNullNorEmpty(value, nameof(value));

                _value = value;
                _prefix = null;
                _suffix = null;

                if (!string.IsNullOrEmpty(Delimiter)
                    && !ReferenceEquals(value, null)
                    && value.Length != 0)
                {
                    var parts = value.Split
                        (
                            new [] {Delimiter},
                            2,
                            StringSplitOptions.None
                        );

                    _prefix = parts[0];
                    if (parts.Length != 1)
                    {
                        _suffix = parts[1];
                    }
                }
            }

            /// <summary>
            /// Convert to array of menu items.
            /// </summary>
            public MenuEntry[] ToMenu()
            {
                var result = new List<MenuEntry>
                {
                    new MenuEntry
                    {
                        Code = Prefix,
                        Comment = Suffix
                    }
                };

                foreach (var child in Children)
                {
                    result.AddRange(child.ToMenu());
                }

                return result.ToArray();
            }

            /// <summary>
            /// Walk over the tree.
            /// </summary>
            public void Walk
                (
                    Action<Item> action
                )
            {
                action(this);
                foreach (var child in Children)
                {
                    child.Walk(action);
                }
            }

            #endregion

            #region IHandmadeSerializable members

            /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
            public void RestoreFromStream
                (
                    BinaryReader reader
                )
            {
                Value = reader.ReadNullableString();
                reader.ReadCollection(Children);
            }

            /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
            public void SaveToStream
                (
                    BinaryWriter writer
                )
            {
                writer.WriteNullable(Value);
                writer.Write(Children);
            }

            #endregion

            #region IVerifiable members

            /// <inheritdoc cref="IVerifiable.Verify" />
            public bool Verify
                (
                    bool throwException
                )
            {
                var result = !string.IsNullOrEmpty(Value);

                if (result && Children.Count != 0)
                {
                    result = Children.All
                        (
                            child => child.Verify(throwException)
                        );
                }

                if (!result)
                {
                    Log.Error
                        (
                            nameof(IrbisTreeFile) + "::" + nameof(Verify)
                            + ": verification error"
                        );

                    if (throwException)
                    {
                        throw new VerificationException();
                    }
                }

                return result;
            }


            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// File name.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Root items.
        /// </summary>
        [ItemNotNull]
        public NonNullCollection<Item> Roots { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisTreeFile()
        {
            Roots = new NonNullCollection<Item>();
        }

        #endregion

        #region Private members

        /// <summary>
        /// Determines indent level of the string.
        /// </summary>
        private static int CountIndent
            (
                string line
            )
        {
            var result = 0;

            foreach (var c in line)
            {
                if (c == Indent)
                {
                    result++;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private static int _ArrangeLevel
            (
                List<Item> items,
                int level,
                int index,
                int count
            )
        {
            var next = index + 1;
            var level2 = level + 1;

            while (next < count)
            {
                if (items[next].Level <= level)
                {
                    break;
                }

                if (items[next].Level == level2)
                {
                    items[index].Children.Add(items[next]);
                }

                next++;
            }

            return next;
        }

        private static void _ArrangeLevel
            (
                List<Item> items,
                int level
            )
        {
            var count = items.Count;
            var index = 0;

            while (index < count)
            {
                var next = _ArrangeLevel
                    (
                        items,
                        level,
                        index,
                        count
                    );

                index = next;
            }
        }

        private static void _WriteLevel
            (
                TextWriter writer,
                NonNullCollection<Item> items,
                int level
            )
        {
            foreach (var item in items)
            {
                for (var i = 0; i < level; i++)
                {
                    writer.Write(Indent);
                }
                writer.WriteLine(item.Value);

                _WriteLevel
                    (
                        writer,
                        item.Children,
                        level + 1
                    );
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add root item.
        /// </summary>
        public Item AddRoot
            (
                string? value
            )
        {
            var result = new Item(value);
            Roots.Add(result);

            return result;
        }

        /// <summary>
        /// Parse specified stream.
        /// </summary>
        [MustUseReturnValue]
        public static IrbisTreeFile ParseStream
            (
                TextReader reader
            )
        {
            var result = new IrbisTreeFile();

            var list = new List<Item>();
            var line = reader.ReadLine();
            if (ReferenceEquals(line, null))
            {
                goto DONE;
            }

            if (CountIndent(line) != 0)
            {
                Log.Error
                    (
                        nameof(IrbisTreeFile) + "::" + nameof(ParseStream)
                        + ": indent != 0"
                    );

                throw new FormatException();
            }
            list.Add(new Item(line));

            var currentLevel = 0;
            while ((line = reader.ReadLine()) != null)
            {
                var level = CountIndent(line);
                if (level > currentLevel + 1)
                {
                    Log.Error
                        (
                            nameof(IrbisTreeFile) + "::" + nameof(ParseStream)
                            + ": level > currentLevel + 1"
                        );

                    throw new FormatException();
                }
                currentLevel = level;
                line = line.TrimStart(Indent);
                var item = new Item(line)
                {
                    Level = currentLevel
                };
                list.Add(item);
            }

            var maxLevel = list.Max(item => item.Level);
            for (var level = 0; level < maxLevel; level++)
            {
                _ArrangeLevel(list, level);
            }

            var roots = list.Where(item => item.Level == 0);
            result.Roots.AddRange(roots);

DONE:
            return result;
        }

        /// <summary>
        /// Read local file.
        /// </summary>
        [MustUseReturnValue]
        public static IrbisTreeFile ReadLocalFile
            (
                string fileName,
                Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using var reader = TextReaderUtility.OpenRead
                (
                    fileName,
                    encoding
                );
            var result = ParseStream(reader);
            result.FileName = Path.GetFileName(fileName);

            return result;
        }

        /// <summary>
        /// Save to text stream.
        /// </summary>
        public void Save
            (
                TextWriter writer
            )
        {
            _WriteLevel
                (
                    writer,
                    Roots,
                    0
                );
        }

        /// <summary>
        /// Save to local file.
        /// </summary>
        public void SaveToLocalFile
            (
                string fileName,
                Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using var writer = TextWriterUtility.Create
                (
                    fileName,
                    encoding
                );
            Save(writer);
        }

        /// <summary>
        /// Convert tree to menu.
        /// </summary>
        public MenuFile ToMenu()
        {
            var result = new MenuFile();

            foreach (var root in Roots)
            {
                result.Entries.AddRange(root.ToMenu());
            }

            return result;
        }

        /// <summary>
        /// Walk over the tree.
        /// </summary>
        public void Walk
            (
                Action<Item> action
            )
        {
            foreach (var child in Roots)
            {
                child.Walk(action);
            }
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            FileName = reader.ReadNullableString();
            reader.ReadCollection(Roots);
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer.WriteNullable(FileName);
            writer.Write(Roots);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwException
            )
        {
            var result = Roots.Count != 0
                && Roots.All
                    (
                        root => root.Verify(throwException)
                    );

            return result;
        }

        #endregion
    }
}
