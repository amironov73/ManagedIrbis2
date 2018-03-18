﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ServerConfiguration.cs --
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
using System.Threading.Tasks;
using System.Xml.Serialization;

using AM;
using AM.Collections;
using AM.IO;
using AM.Runtime;
using AM.Text;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Server
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    [XmlRoot("configuration")]
    public sealed class ServerConfiguration
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Properties

        /// <summary>
        /// Path for AlphabetTable (without extension).
        /// </summary>
        [CanBeNull]
        [XmlElement("alphabetTablePath")]
        [JsonProperty("alphabetTablePath", NullValueHandling = NullValueHandling.Ignore)]
        public string AlphabetTablePath { get; set; }

        /// <summary>
        /// Data path.
        /// </summary>
        [CanBeNull]
        [XmlElement("dataPath")]
        [JsonProperty("dataPath", NullValueHandling = NullValueHandling.Ignore)]
        public string DataPath { get; set; }

        /// <summary>
        /// System path.
        /// </summary>
        [CanBeNull]
        [XmlElement("systemPath")]
        [JsonProperty("systemPath", NullValueHandling = NullValueHandling.Ignore)]
        public string SystemPath { get; set; }

        /// <summary>
        /// Path for UpperCaseTable (without extension).
        /// </summary>
        [CanBeNull]
        [XmlElement("upperCaseTable")]
        [JsonProperty("upperCaseTable", NullValueHandling = NullValueHandling.Ignore)]
        public string UpperCaseTable { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Create server configuration from INI-file.
        /// </summary>
        [NotNull]
        public static ServerConfiguration FromIniFile
            (
                [NotNull] ServerIniFile iniFile
            )
        {
            Sure.NotNull(iniFile, nameof(iniFile));

            ServerConfiguration result = new ServerConfiguration
            {
                SystemPath = iniFile.SystemPath,
                DataPath = iniFile.DataPath,
                AlphabetTablePath = iniFile.AlphabetTablePath,
                UpperCaseTable = iniFile.UpperCaseTable
            };

            return result;
        }

        /// <summary>
        /// Create server configuration from INI file.
        /// </summary>
        [NotNull]
        public static ServerConfiguration FromIniFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using (IniFile iniFile = new IniFile
                (
                    fileName,
                    IrbisEncoding.Ansi,
                    false
                ))
            {
                ServerIniFile serverIni = new ServerIniFile(iniFile);
                ServerConfiguration result = FromIniFile(serverIni);

                return result;
            }
        }

        /// <summary>
        /// Create server configuration from path.
        /// </summary>
        [NotNull]
        public static ServerConfiguration FromPath
            (
                [NotNull] string path
            )
        {
            Sure.NotNullNorEmpty(path, nameof(path));

            string systemPath = Path.GetFullPath(path);
            systemPath = PathUtility.StripTrailingBackslash(systemPath);

            ServerConfiguration result = new ServerConfiguration
            {
                SystemPath = systemPath
                    + Path.DirectorySeparatorChar,
                DataPath = Path.Combine
                    (
                        systemPath,
                        "DATAI"
                        + Path.DirectorySeparatorChar
                    ),
                AlphabetTablePath = Path.Combine(systemPath, "isisacw"),
                UpperCaseTable = Path.Combine(systemPath, "isisucw")
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

            AlphabetTablePath = reader.ReadNullableString();
            DataPath = reader.ReadNullableString();
            SystemPath = reader.ReadNullableString();
            UpperCaseTable = reader.ReadNullableString();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer
                .WriteNullable(AlphabetTablePath)
                .WriteNullable(DataPath)
                .WriteNullable(SystemPath)
                .WriteNullable(UpperCaseTable);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify(bool throwOnError)
        {
            Verifier<ServerConfiguration> verifier
                = new Verifier<ServerConfiguration>(this, throwOnError);

            // IRBIS64 doesn't use external upper case table

            verifier
                .DirectoryExist(SystemPath, "SystemPath")
                .DirectoryExist(DataPath, "DataPath")
                .NotNullNorEmpty(AlphabetTablePath, "AlphabetTablePath");
                //.NotNullNorEmpty(UpperCaseTable, "UpperCaseTable");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return SystemPath.ToVisibleString();
        }

        #endregion
    }
}
