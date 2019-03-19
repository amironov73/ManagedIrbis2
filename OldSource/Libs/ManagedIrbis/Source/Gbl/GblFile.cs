// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GblFile.cs -- GBL file
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using AM;
using AM.Collections;
using AM.IO;
using AM.Logging;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Gbl
{
    /// <summary>
    /// GBL file
    /// </summary>
    [PublicAPI]
    [XmlRoot("gbl")]
    public sealed class GblFile
        : IHandmadeSerializable,
        IVerifiable
    {
        #region Properties

        /// <summary>
        /// File name.
        /// </summary>
        [CanBeNull]
        [XmlIgnore]
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// Items.
        /// </summary>
        [NotNull]
        [XmlElement("item")]
        [JsonProperty("items")]
        public NonNullCollection<GblStatement> Statements { get; }

        /// <summary>
        /// Signature.
        /// </summary>
        [NotNull]
        [XmlElement("parameter")]
        [JsonProperty("parameters")]
        public NonNullCollection<GblParameter> Parameters { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor
        /// </summary>
        public GblFile()
        {
            Parameters = new NonNullCollection<GblParameter>();
            Statements = new NonNullCollection<GblStatement>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Parse local file.
        /// </summary>
        [NotNull]
        public static GblFile ParseLocalFile
            (
                [NotNull] string fileName,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(fileName, nameof(fileName));
            Sure.NotNull(encoding, nameof(encoding));

            using (StreamReader reader = TextReaderUtility.OpenRead
                (
                    fileName,
                    encoding
                ))
            {
                GblFile result = ParseStream(reader);

                return result;
            }
        }

        /// <summary>
        /// Parse specified stream.
        /// </summary>
        [NotNull]
        public static GblFile ParseStream
            (
                [NotNull] TextReader reader
            )
        {
            GblFile result = new GblFile();

            string line = reader.RequireLine();
            int count = int.Parse(line);
            for (int i = 0; i < count; i++)
            {
                GblParameter parameter = GblParameter.ParseStream(reader);
                result.Parameters.Add(parameter);
            }

            while (true)
            {
                GblStatement statement = GblStatement.ParseStream(reader);
                if (statement == null)
                {
                    break;
                }
                result.Statements.Add(statement);
            }

            return result;
        }

        /// <summary>
        /// Should JSON serialize <see cref="Parameters"/>.
        /// </summary>
        public bool ShouldSerializeParameters()
        {
            return Parameters.Count != 0;
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream"/>
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            FileName = reader.ReadNullableString();
            reader.ReadCollection(Parameters);
            reader.ReadCollection(Statements);
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream"/>
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer.WriteNullable(FileName);
            writer.Write(Parameters);
            writer.Write(Statements);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify"/>
        public bool Verify
            (
                bool throwOnError
            )
        {
            bool result = Statements.Count != 0;

            if (result)
            {
                result = Statements.All
                    (
                        item => item.Verify(throwOnError)
                    );
            }

            if (result
                && Parameters.Count != 0)
            {
                result = Parameters.All
                    (
                        parameter => parameter.Verify(throwOnError)
                    );
            }

            if (!result)
            {
                Log.Error
                    (
                        "GblFile::Verify: "
                        + "verification error"
                    );

                if (throwOnError)
                {
                    throw new VerificationException();
                }
            }

            return result;
        }

        #endregion

        #region Object members

        #endregion
    }
}

