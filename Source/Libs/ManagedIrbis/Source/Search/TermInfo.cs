// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TermInfo.cs -- term info
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using AM;
using AM.Collections;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

// ReSharper disable CommentTypo

namespace ManagedIrbis.Search
{
    /// <summary>
    /// Search term info
    /// </summary>
    [PublicAPI]
    [XmlRoot("term")]
    [DebuggerDisplay("[{Count}] {Text}")]
    public class TermInfo
        : //IHandmadeSerializable,
        IVerifiable
    {
        #region Properties

        /// <summary>
        /// Empty array.
        /// </summary>
        public static readonly TermInfo[] EmptyArray = new TermInfo[0];

        /// <summary>
        /// Количество ссылок.
        /// </summary>
        [XmlAttribute("count")]
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Поисковый термин.
        /// </summary>
        [XmlAttribute("text")]
        [JsonProperty("text")]
        public string? Text { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Clone the <see cref="TermInfo"/>.
        /// </summary>
        public TermInfo Clone()
        {
            return (TermInfo) MemberwiseClone();
        }

        /// <summary>
        /// Разбор ответа сервера.
        /// </summary>
        [ItemNotNull]
        public static TermInfo[] Parse
            (
                IEnumerable<string> response
            )
        {
            var result = new LocalList<TermInfo>();

            var regex = new Regex(@"^(\d+)\#(.+)$");
            foreach (var line in response)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                var match = regex.Match(line);
                if (match.Success)
                {
                    var item = new TermInfo
                    {
                        Count = int.Parse(match.Groups[1].Value),
                        Text = match.Groups[2].Value
                    };
                    result.Add(item);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Should serialize the <see cref="Text"/> field?
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool ShouldSerializeText()
        {
            return !string.IsNullOrEmpty(Text);
        }

        /// <summary>
        /// Should serialize the <see cref="Count"/> field?
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool ShouldSerializeCount()
        {
            return Count != 0;
        }

        /// <summary>
        /// Trim prefix from terms.
        /// </summary>
        public static TermInfo[] TrimPrefix
            (
                [ItemNotNull] TermInfo[] terms,
                string prefix
            )
        {
            var prefixLength = prefix.Length;
            var result = new List<TermInfo>(terms.Length);
            if (prefixLength == 0)
            {
                foreach (var term in terms)
                {
                    result.Add(term.Clone());
                }
            }
            else
            {
                foreach (var term in terms)
                {
                    var item = term.Text;
                    if (!string.IsNullOrEmpty(item) && item.StartsWith(prefix))
                    {
                        item = item.Substring(prefixLength);
                    }
                    var clone = term.Clone();
                    clone.Text = item;
                    result.Add(clone);
                }
            }

            return result.ToArray();
        }

        #endregion

        //#region IHandmadeSerializable members

        ///// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        //public virtual void RestoreFromStream
        //    (
        //        BinaryReader reader
        //    )
        //{
        //    Count = reader.ReadPackedInt32();
        //    Text = reader.ReadNullableString();
        //}

        ///// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        //public virtual void SaveToStream
        //    (
        //        BinaryWriter writer
        //    )
        //{
        //    writer
        //        .WritePackedInt32(Count)
        //        .WriteNullable(Text);
        //}

        //#endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public virtual bool Verify
            (
                bool throwOnError
            )
        {
            var verifier
                = new Verifier<TermInfo>(this, throwOnError);

            verifier
                .NotNullNorEmpty(Text, "text")
                .Assert(Count >= 0, "Count");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return $"{Count}#{Text.ToVisibleString()}";
        }

        #endregion
    }
}
