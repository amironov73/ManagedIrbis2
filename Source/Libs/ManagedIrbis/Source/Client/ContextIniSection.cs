// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ContextIniSection.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Xml.Serialization;

using AM.IO;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

// ReSharper disable StringLiteralTypo

namespace ManagedIrbis.Client
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class ContextIniSection
        : AbstractIniSection
    {
        #region Constants

        /// <summary>
        /// Section name.
        /// </summary>
        public const string SectionName = "CONTEXT";

        #endregion

        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [XmlElement("database")]
        [JsonProperty("database")]
        public string? Database
        {
            get => Section["DBN"];
            set => Section["DBN"] = value;
        }

        /// <summary>
        /// Display format description.
        /// </summary>
        [XmlElement("format")]
        [JsonProperty("format")]
        public string? DisplayFormat
        {
            get => Section["PFT"];
            set => Section["PFT"] = value;
        }

        /// <summary>
        /// Current MFN.
        /// </summary>
        [XmlElement("mfn")]
        [JsonProperty("mfn")]
        public int Mfn
        {
            get => Section.GetValue("CURMFN", 0);
            set => Section.SetValue("CURMFN", value);
        }

        /// <summary>
        /// Password.
        /// </summary>
        [XmlElement("password")]
        [JsonProperty("password")]
        public string? Password
        {
            get => Section["UserPassword"] ?? Section["Password"];
            set => Section["UserPassword"] = value;
        }

        /// <summary>
        /// Query.
        /// </summary>
        [XmlElement("query")]
        [JsonProperty("query")]
        public string? Query
        {
            // TODO использовать UTF8

            get => Section["QUERY"];
            set => Section["QUERY"] = value;
        }

        /// <summary>
        /// Search prefix.
        /// </summary>
        [XmlElement("prefix")]
        [JsonProperty("prefix")]
        public string? SearchPrefix
        {
            get => Section["PREFIX"];
            set => Section["PREFIX"] = value;
        }

        /// <summary>
        /// User name.
        /// </summary>
        [XmlElement("username")]
        [JsonProperty("username")]
        public string? UserName
        {
            get => Section["UserName"];
            set => Section["UserName"] = value;
        }

        /// <summary>
        /// Worksheet code.
        /// </summary>
        [XmlElement("worksheet")]
        [JsonProperty("worksheet")]
        public string? Worksheet
        {
            get => Section["WS"];
            set => Section["WS"] = value;
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ContextIniSection()
            : base (SectionName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ContextIniSection
            (
                IniFile iniFile
            )
            : base(iniFile, SectionName)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ContextIniSection
            (
                IniFile.Section section
            )
            : base(section)
        {
        }

        #endregion
    }
}
