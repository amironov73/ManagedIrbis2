﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FoundItem.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using AM;
using AM.Collections;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

using Newtonsoft.Json;

#endregion

// ReSharper disable StringLiteralTypo

namespace ManagedIrbis.Search
{
    /// <summary>
    /// Found item.
    /// </summary>
    [PublicAPI]
    [XmlRoot("item")]
    [DebuggerDisplay("{Mfn} {Text}")]
    public sealed class FoundItem
        : // IHandmadeSerializable,
        IVerifiable
    {
        #region Constants

        /// <summary>
        /// Delimiter.
        /// </summary>
        public const char Delimiter = '#';

        #endregion

        #region Properties

        /// <summary>
        /// Text.
        /// </summary>
        [XmlAttribute("text")]
        [JsonProperty("text")]
        [Description("Библиографическое описание")]
        [DisplayName("Библиографическое описание")]
        public string? Text { get; set; }

        /// <summary>
        /// MFN.
        /// </summary>
        [XmlAttribute("mfn")]
        [JsonProperty("mfn")]
        [Description("MFN")]
        [DisplayName("MFN")]
        public int Mfn { get; set; }

        /// <summary>
        /// Associated record.
        /// </summary>
        [XmlElement("record")]
        [JsonProperty("record")]
        [Browsable(false)]
        public MarcRecord? Record { get; set; }

        /// <summary>
        /// Is selected?
        /// </summary>
        [XmlAttribute("selected")]
        [JsonProperty("selected")]
        [Description("Пометка")]
        [DisplayName("Пометка")]
        public bool Selected { get; set; }

        /// <summary>
        /// Arbitrary user data.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        [Browsable(false)]
        public object? UserData { get; set; }

        #endregion

        #region Private members

        private static readonly char[] _delimiters = { Delimiter };

        #endregion

        #region Public methods

        /// <summary>
        /// Convert to MFN array.
        /// </summary>
        public static int[] ConvertToMfn
            (
                [ItemNotNull] FoundItem[] found
            )
        {
            var result = new int[found.Length];
            for (var i = 0; i < found.Length; i++)
            {
                result[i] = found[i].Mfn;
            }

            return result;
        }

        /// <summary>
        /// Convert to string array.
        /// </summary>
#nullable disable
        [ItemCanBeNull]
        public static string[] ConvertToText
            (
                [ItemNotNull] FoundItem[] found
            )
        {
            var result = new string[found.Length];
            for (var i = 0; i < found.Length; i++)
            {
                result[i] = found[i].Text.EmptyToNull();
            }

            return result;
        }
#nullable restore

        /// <summary>
        /// Parse text line.
        /// </summary>
        public static FoundItem ParseLine
            (
                string line
            )
        {
            Sure.NotNullNorEmpty(line, nameof(line));

            var parts = line.Split(_delimiters, 2);
            var result = new FoundItem
            {
                Mfn = int.Parse(parts[0])
            };
            if (parts.Length > 1)
            {
                var text = parts[1].EmptyToNull();
                text = IrbisText.IrbisToWindows(text);
                result.Text = text;
            }

            return result;
        }

        /// <summary>
        /// Parse server response.
        /// </summary>
        [ItemNotNull]
        public static FoundItem[] ParseServerResponse
            (
                IEnumerable<string> response,
                int sizeHint
            )
        {
            var result = new LocalList<FoundItem>(sizeHint);
            try
            {
                foreach (var line in response)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var item = ParseLine(line);
                        result.Add(item);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Should serialize the <see cref="Text"/> field?
        /// </summary>
        public bool ShouldSerializeText()
        {
            return !string.IsNullOrEmpty(Text);
        }

        /// <summary>
        /// Should serialize the <see cref="Selected"/> field?
        /// </summary>
        public bool ShouldSerializeSelected()
        {
            return Selected;
        }

        #endregion

        //#region IHandmadeSerializable members

        ///// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        //public void RestoreFromStream
        //    (
        //        BinaryReader reader
        //    )
        //{
        //    Sure.NotNull(reader, "reader");

        //    Mfn = reader.ReadPackedInt32();
        //    Text = reader.ReadNullableString();
        //    Record = reader.RestoreNullable<MarcRecord>();
        //    Selected = reader.ReadBoolean();
        //}

        ///// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        //public void SaveToStream
        //    (
        //        BinaryWriter writer
        //    )
        //{
        //    Sure.NotNull(writer, "writer");

        //    writer
        //        .WritePackedInt32(Mfn)
        //        .WriteNullable(Text)
        //        .WriteNullable(Record)
        //        .Write(Selected);
        //}

        //#endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            var verifier = new Verifier<FoundItem>
                (
                    this,
                    throwOnError
                );

            verifier
                .Assert(Mfn > 0, "Mfn > 0")
                .NotNullNorEmpty(Text, "Text");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return string.Format
                (
                    "[{0}] {1}",
                    Mfn,
                    Text.ToVisibleString()
                );
        }

        #endregion
    }
}
