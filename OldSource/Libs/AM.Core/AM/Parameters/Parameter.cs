// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Parameter.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using AM.Collections;
using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace AM.Parameters
{
    /// <summary>
    /// Parameter.
    /// </summary>
    [PublicAPI]
    [XmlRoot("parameter")]
    [DebuggerDisplay("{Name}={Value}")]
    public sealed class Parameter
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Properties

        /// <summary>
        /// Name.
        /// </summary>
        [CanBeNull]
        [XmlAttribute("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        /// <remarks>Can be <c>string.Empty</c>.</remarks>
        [CanBeNull]
        [XmlAttribute("value")]
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Values.
        /// </summary>
        [NotNull]
        public NonNullCollection<string> Values { get; private set; }

        #endregion

        #region Construciton

        /// <summary>
        /// Constructor.
        /// </summary>
        public Parameter()
        {
            Values = new NonNullCollection<string>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Parameter
            (
                [NotNull] string name,
                [CanBeNull] string value
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            Name = name;
            Value = value ?? string.Empty;
            Values = new NonNullCollection<string>
            {
                Value
            };
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
            Value = reader.ReadNullableString();
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
                .WriteNullable(Value);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<Parameter> verifier = new Verifier<Parameter>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Name, "Name")
                .NotNull(Value, "Value");

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        [Pure]
        public override string ToString()
        {
            return $"{Name}={Value}";
        }

        #endregion
    }
}
