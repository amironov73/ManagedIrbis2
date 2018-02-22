﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UnicodeRange.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Xml.Serialization;

using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;
// ReSharper disable All

#endregion

namespace AM.Text
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    [XmlRoot("range")]
    public sealed class UnicodeRange
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Static 

        /// <summary>
        /// Control characters.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange ControlCharacters
            = new UnicodeRange("Control characters", '\x0000', '\x001F');

        /// <summary>
        /// Basic Latin.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange BasicLatin
            = new UnicodeRange("Basic Latin", '\x0020', '\x007F');

        /// <summary>
        /// Latin1 supplement.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange Latin1Supplement
            = new UnicodeRange("Latin Supplement", '\x0080', '\x00FF');

        /// <summary>
        /// Latin extended.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange LatinExtended
            = new UnicodeRange("Latin Extended", '\x0100', '\x024F');

        /// <summary>
        /// Cyrillic.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange Cyrillic
            = new UnicodeRange("Cyrillic", '\x0400', '\x04FF');

        /// <summary>
        /// Cyrillic supplement.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange CyrillicSupplement
            = new UnicodeRange("Cyrillic Supplement", '\x0500', '\x052F');

        /// <summary>
        /// Russian.
        /// </summary>
        [NotNull]
        public static readonly UnicodeRange Russian
            = new UnicodeRange("Russian", '\x0410', '\x0451');

        #endregion

        #region Properties

        /// <summary>
        /// Name.
        /// </summary>
        [CanBeNull]
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// From.
        /// </summary>
        [XmlAttribute("from")]
        [JsonProperty("from")]
        public char From { get; private set; }

        /// <summary>
        /// To (including).
        /// </summary>
        [XmlAttribute("to")]
        [JsonProperty("to")]
        public char To { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnicodeRange()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnicodeRange
            (
                [NotNull] string name,
                char fromChar,
                char toChar
            )
        {
            Sure.NotNull(name, nameof(name));

            if (fromChar > toChar)
            {
                throw new ArgumentException();
            }

            Name = name;
            From = fromChar;
            To = toChar;
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

            Name = reader.ReadNullableString();
            From = reader.ReadChar();
            To = reader.ReadChar();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer.WriteNullable(Name);
            writer.Write(From);
            writer.Write(To);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify(bool throwOnError)
        {
            Verifier<UnicodeRange> verifier
                = new Verifier<UnicodeRange>(this, throwOnError);

            verifier
                .NotNullNorEmpty(Name, "Name")
                .Assert(From <= To, "From <= To");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return Name.ToVisibleString()
                   + ": "
                   + ((int)From).ToInvariantString()
                   + "-"
                   + ((int)To).ToInvariantString();
        }

        #endregion
    }
}
