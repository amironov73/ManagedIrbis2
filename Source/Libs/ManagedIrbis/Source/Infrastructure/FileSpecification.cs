﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FileSpecification.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: good
 */

#region Using directives

using System;
using System.Diagnostics;
using System.IO;

using AM;
using AM.IO;
using AM.Runtime;
using AM.Text;

using JetBrains.Annotations;

#endregion

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace ManagedIrbis.Infrastructure
{
    //
    // Путь на файл Αpath.Adbn.Afilename
    // Αpath – код путей:
    // 0 – общесистемный путь.
    // 1 – путь размещения сведений о базах данных сервера ИРБИС64
    // 2 – путь на мастер-файл базы данных.
    // 3 – путь на словарь базы данных.
    // 10 – путь на параметрию базы данных.
    // Adbn – имя базы данных
    // Afilename – имя требуемого файла с расширением
    // В случае чтения ресурса по пути 0 и 1 имя базы данных не задается.
    //

    /// <summary>
    /// File name specification in IRBIS64.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("{" + nameof(Path) + "} {" + nameof(Database)
                     + "} FileName={" + nameof(FileName) + "}")]
    public sealed class FileSpecification
        : IHandmadeSerializable,
        IVerifiable,
        IEquatable<FileSpecification>
    {
        #region Properties

        /// <summary>
        /// Is the file binary or text?
        /// </summary>
        public bool BinaryFile { get; set; }

        /// <summary>
        /// Path.
        /// </summary>
        public IrbisPath Path { get; set; }

        /// <summary>
        /// Database name.
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// File name.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// File content (when we want write the file).
        /// </summary>
        public string? Content { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileSpecification()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileSpecification
            (
                IrbisPath path,
                string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            Path = path;
            FileName = fileName;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileSpecification
            (
                IrbisPath path,
                string? database,
                string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            Path = path;
            Database = database;
            FileName = fileName;
        }

        #endregion

        #region Private members

        private static bool _CompareDatabases
            (
                string? first,
                string? second
            )
        {
            if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second))
            {
                return true;
            }

            return StringUtility.CompareNoCase(first, second) == 0;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Parse the text specification.
        /// </summary>
        public static FileSpecification Parse
            (
                string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            var navigator = new TextNavigator(text);
            var path = NumericUtility.ParseInt32
                (
                    navigator.ReadTo(".")
                );
            var database = navigator.ReadTo(".").ToString().EmptyToNull();
            var fileName = navigator.GetRemainingText().ToString();
            var binaryFile = fileName.StartsWith("@");
            if (binaryFile)
            {
                fileName = fileName.Substring(1);
            }

            string? content = null;
            var position = fileName.IndexOf("&", StringComparison.InvariantCulture);
            if (position >= 0)
            {
                content = fileName.Substring(position + 1);
                fileName = fileName.Substring(0, position);
            }
            var result = new FileSpecification
            {
                BinaryFile = binaryFile,
                Path = (IrbisPath)path,
                Database = database,
                FileName = fileName,
                Content = content
            };


            return result;
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            BinaryFile = reader.ReadBoolean();
            Path = (IrbisPath)reader.ReadPackedInt32();
            Database = reader.ReadNullableString();
            FileName = reader.ReadNullableString();
            Content = reader.ReadNullableString();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer.Write(BinaryFile);
            writer
                .WritePackedInt32((int)Path)
                .WriteNullable(Database)
                .WriteNullable(FileName)
                .WriteNullable(Content);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<FileSpecification> verifier = new Verifier<FileSpecification>
                    (
                        this,
                        throwOnError
                    );

            verifier
                .NotNullNorEmpty(FileName, nameof(FileName));

            if (Path != IrbisPath.System
                && Path != IrbisPath.Data)
            {
                verifier.NotNullNorEmpty(Database, nameof(Database));
            }

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Compare with other <see cref="FileSpecification"/>
        /// instance.
        /// </summary>
        public bool Equals
            (
                FileSpecification other
            )
        {
            other = other.ThrowIfNull();

            return Path == other.Path
                   && _CompareDatabases(Database, other.Database)
                   && FileName.SameString(other.FileName);
        }

        /// <inheritdoc cref="object.Equals(object)" />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is FileSpecification other && Equals(other);
        }

        /// <inheritdoc cref="object.GetHashCode" />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Path;
                hashCode = (hashCode * 397)
                    ^ (Database != null ? Database.GetHashCode() : 0);
                hashCode = (hashCode * 397)
                    ^ (FileName != null ? FileName.GetHashCode() : 0);

                return hashCode;
            }
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            var fileName = FileName;
            if (BinaryFile)
            {
                fileName = "@" + fileName;
            }
            else
            {
                if (!ReferenceEquals(Content, null))
                {
                    fileName = "&" + fileName;
                }
            }

            string result;

            switch (Path)
            {
                case IrbisPath.System:
                case IrbisPath.Data:
                    result = string.Format
                        (
                            "{0}..{1}",
                            (int)Path,
                            fileName
                        );
                    break;

                default:
                    result = string.Format
                        (
                            "{0}.{1}.{2}",
                            (int)Path,
                            Database,
                            fileName
                        );
                    break;
            }

            if (!ReferenceEquals(Content, null))
            {
                result = result + "&" + IrbisText.WindowsToIrbis(Content);
            }

            return result;
        }

        #endregion
    }
}
