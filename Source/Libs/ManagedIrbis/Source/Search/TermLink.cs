// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TermLink.cs -- term link
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using AM;
using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Search
{
    /// <summary>
    /// Term link.
    /// </summary>
    [PublicAPI]
    [XmlRoot("term-link")]
    [DebuggerDisplay("[{Mfn}] {Tag}/{Occurrence} {Index}")]
    public sealed class TermLink
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Properties

        /// <summary>
        /// Empty array of <see cref="TermLink"/>'s.
        /// </summary>
        public static readonly TermLink[] EmptyArray = new TermLink[0];

        /// <summary>
        /// MFN записи с искомым термом.
        /// </summary>
        [JsonProperty("mfn")]
        [XmlAttribute("mfn")]
        public int Mfn { get; set; }

        /// <summary>
        /// Тег поля с искомым термом.
        /// </summary>
        [JsonProperty("tag")]
        [XmlAttribute("tag")]
        public int Tag { get; set; }

        /// <summary>
        /// Повторение поля.
        /// </summary>
        [JsonProperty("occurrence")]
        [XmlAttribute("occurrence")]
        public int Occurrence { get; set; }

        /// <summary>
        /// Смещение от начала поля.
        /// </summary>
        [JsonProperty("index")]
        [XmlAttribute("index")]
        public int Index { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Clone the <see cref="TermLink"/>.
        /// </summary>
        public TermLink Clone()
        {
            return (TermLink) MemberwiseClone();
        }

        /// <summary>
        /// Dump the links.
        /// </summary>
        public static void Dump
            (
                IEnumerable<TermLink> links,
                TextWriter writer
            )
        {
            foreach (var link in links)
            {
                writer.WriteLine(link);
            }
        }

        /// <summary>
        /// Convert <see cref="TermPosting"/>
        /// to <see cref="TermLink"/>.
        /// </summary>
        public static TermLink FromPosting
            (
                TermPosting posting
            )
        {
            var result = new TermLink
            {
                Mfn = posting.Mfn,
                Tag = posting.Tag,
                Occurrence = posting.Occurrence,
                Index = posting.Count
            };

            return result;
        }

        /// <summary>
        /// Convert array of <see cref="TermPosting"/>
        /// to array of <see cref="TermLink"/>.
        /// </summary>
        [ItemNotNull]
        public static TermLink[] FromPostings
            (
                [ItemNotNull] TermPosting[] postings
            )
        {
            var result = new TermLink[postings.Length];
            for (int i = 0; i < postings.Length; i++)
            {
                result[i] = FromPosting(postings[i]);
            }

            return result;
        }

        /// <summary>
        /// Чтение ссылки из файла.
        /// </summary>
        public static TermLink Read
            (
                Stream stream
            )
        {
            var result = new TermLink
            {
                Mfn = stream.ReadInt32Network(),
                Tag = stream.ReadInt32Network(),
                Occurrence = stream.ReadInt32Network(),
                Index = stream.ReadInt32Network()
            };
            return result;
        }

        /// <summary>
        /// Convert array of <see cref="TermLink"/> into array of MFN.
        /// </summary>
        public static int[] ToMfn
            (
                TermLink[] links
            )
        {
            var result = new int[links.Length];
            for (var i = 0; i < links.Length; i++)
            {
                result[i] = links[i].Mfn;
            }

            return result;
        }

        /// <summary>
        /// Convert array of MFN into array of <see cref="TermLink"/>s.
        /// </summary>
        /// <returns></returns>
        public static TermLink[] FromMfn
            (
                int[] array
            )
        {
            var result = new TermLink[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = new TermLink
                {
                    Mfn = array[i]
                };
            }

            return result;
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream"/>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Mfn = reader.ReadPackedInt32();
            Tag = reader.ReadPackedInt32();
            Occurrence = reader.ReadPackedInt32();
            Index = reader.ReadPackedInt32();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream"/>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer
                .WritePackedInt32(Mfn)
                .WritePackedInt32(Tag)
                .WritePackedInt32(Occurrence)
                .WritePackedInt32(Index);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify"/>
        public bool Verify
            (
                bool throwOnError
            )
        {
            var verifier = new Verifier<TermLink>(this, throwOnError);

            verifier
                .Assert(Mfn > 0, "Mfn")
                .Assert(Tag > 0, "Tag")
                .Assert(Occurrence > 0, "Occurrence")
                .Assert(Index > 0, "Index");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            return $"[{Mfn}] {Tag}/{Occurrence} {Index}";
        }

        /// <summary>
        /// Compares this term link with another.
        /// </summary>
        public bool Equals
            (
                TermLink other
            )
        {
            return Mfn == other.Mfn
                && Tag == other.Tag
                && Occurrence == other.Occurrence
                && Index == other.Index;
        }

        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals
            (
                object? obj
            )
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var termLink = obj as TermLink;

            return !ReferenceEquals(termLink, null)
                   && Equals(termLink);
        }

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Mfn;
                hashCode = (hashCode*397) ^ Tag;
                hashCode = (hashCode*397) ^ Occurrence;
                hashCode = (hashCode*397) ^ Index;
                return hashCode;
            }
        }

        #endregion
    }
}
