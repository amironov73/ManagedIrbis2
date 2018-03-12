// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GblSettings.cs -- settings for GBL
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.IO;
using System.Linq;
using AM;
using AM.Collections;
using AM.IO;
using AM.Runtime;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace ManagedIrbis.Gbl
{
    /// <summary>
    /// Settings for GBL execution.
    /// </summary>
    [PublicAPI]
    public sealed class GblSettings
        : IVerifiable,
        IHandmadeSerializable
    {
        #region Properties

        /// <summary>
        /// Actualize records after processing.
        /// </summary>
        [JsonProperty("actualize")]
        public bool Actualize { get; set; }

        /// <summary>
        /// Process 'autoin.gbl'.
        /// </summary>
        [JsonProperty("autoin")]
        public bool Autoin { get; set; }

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        [JsonProperty("database")]
        public string Database { get; set; }

        /// <summary>
        /// File name.
        /// </summary>
        [CanBeNull]
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// First record MFN.
        /// </summary>
        [JsonProperty("firstRecord")]
        public int FirstRecord { get; set; }

        /// <summary>
        /// Provide formal control.
        /// </summary>
        [JsonProperty("formalControl")]
        public bool FormalControl { get; set; }

        /// <summary>
        /// Maximal MFN.
        /// </summary>
        /// <remarks>0 means 'all records in the database'.
        /// </remarks>
        [JsonProperty("maxMfn")]
        public int MaxMfn { get; set; }

        /// <summary>
        /// List of MFN to process.
        /// </summary>
        [CanBeNull]
        [JsonProperty("mfnList")]
        public int[] MfnList { get; set; }

        /// <summary>
        /// Minimal MFN.
        /// </summary>
        /// <remarks>0 means 'all records in the database'.
        /// </remarks>
        [JsonProperty("minMfn")]
        public int MinMfn { get; set; }

        /// <summary>
        /// Number of records to process.
        /// </summary>
        [JsonProperty("numberOfRecords")]
        public int NumberOfRecords { get; set; }

        /// <summary>
        /// Search expression.
        /// </summary>
        [CanBeNull]
        [JsonProperty("searchExpression")]
        public string SearchExpression { get; set; }

        /// <summary>
        /// Statements.
        /// </summary>
        [NotNull]
        [JsonProperty("statements")]
        public NonNullCollection<GblStatement> Statements
        {
            get;
            private set;
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public GblSettings()
        {
            Actualize = true;
            Autoin = false;
            FirstRecord = 1;
            FormalControl = false;
            MaxMfn = 0;
            MinMfn = 0;
            Statements = new NonNullCollection<GblStatement>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GblSettings
            (
                [NotNull] IrbisConnection connection
            )
            : this()
        {
            Sure.NotNull(connection, nameof(connection));

            Database = connection.Database;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GblSettings
            (
                [NotNull] IrbisConnection connection,
                [NotNull] IEnumerable<GblStatement> statements
            )
            : this(connection)
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(statements, nameof(statements));

            Statements.AddRange(statements);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Restore settings from JSON string.
        /// </summary>
        public static GblSettings FromJson
            (
                [NotNull] string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            GblSettings result = JsonConvert
                .DeserializeObject<GblSettings>(text);

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given interval of MFN.
        /// </summary>
        [NotNull]
        public static GblSettings ForInterval
            (
                [NotNull] IrbisConnection connection,
                int minMfn,
                int maxMfn,
                [NotNull] IEnumerable<GblStatement> statements
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(statements, nameof(statements));

            GblSettings result = new GblSettings(connection, statements)
            {
                MinMfn = minMfn,
                MaxMfn = maxMfn
            };

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given interval of MFN.
        /// </summary>
        [NotNull]
        public static GblSettings ForInterval
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                int minMfn,
                int maxMfn,
                [NotNull] IEnumerable<GblStatement> statements
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(statements, nameof(statements));

            GblSettings result = new GblSettings(connection, statements)
            {
                Database = database,
                MinMfn = minMfn,
                MaxMfn = maxMfn
            };

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given list of MFN.
        /// </summary>
        [NotNull]
        public static GblSettings ForList
            (
                [NotNull] IrbisConnection connection,
                [NotNull] IEnumerable<int> mfnList,
                [NotNull] IEnumerable<GblStatement> statements
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(mfnList, nameof(mfnList));
            Sure.NotNull(statements, nameof(statements));

            GblSettings result = new GblSettings(connection, statements)
            {
                MfnList = mfnList.ToArray()
            };

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given list of MFN.
        /// </summary>
        [NotNull]
        public static GblSettings ForList
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] IEnumerable<int> mfnList,
                [NotNull] IEnumerable<GblStatement> statements
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(mfnList, nameof(mfnList));
            Sure.NotNull(statements, nameof(statements));

            GblSettings result = new GblSettings(connection, statements)
            {
                Database = database,
                MfnList = mfnList.ToArray()
            };

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given list of MFN.
        /// </summary>
        [NotNull]
        public static GblSettings ForList
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(mfnList, nameof(mfnList));

            GblSettings result = new GblSettings(connection)
            {
                Database = database,
                MfnList = mfnList.ToArray()
            };

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given searchExpression.
        /// </summary>
        [NotNull]
        public static GblSettings ForSearchExpression
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string searchExpression,
                [NotNull] IEnumerable<GblStatement> statements
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));
            Sure.NotNull(statements, nameof(statements));

            GblSettings result = new GblSettings(connection, statements)
            {
                SearchExpression = searchExpression
            };

            return result;
        }

        /// <summary>
        /// Create <see cref="GblSettings"/>
        /// for given searchExpression.
        /// </summary>
        [NotNull]
        public static GblSettings ForSearchExpression
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string database,
                [NotNull] string searchExpression,
                [NotNull] IEnumerable<GblStatement> statements
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));
            Sure.NotNull(statements, nameof(statements));

            GblSettings result = new GblSettings(connection, statements)
            {
                Database = database,
                SearchExpression = searchExpression
            };

            return result;
        }

        /// <summary>
        /// Set (server) file name.
        /// </summary>
        [NotNull]
        public GblSettings SetFileName
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            FileName = fileName;

            return this;
        }

        /// <summary>
        /// Set first record and number of records
        /// to process.
        /// </summary>
        [NotNull]
        public GblSettings SetRange
            (
                int firstRecord,
                int numberOfRecords
            )
        {
            Sure.NonNegative(firstRecord, nameof(firstRecord));
            Sure.NonNegative(numberOfRecords, nameof(numberOfRecords));

            FirstRecord = firstRecord;
            NumberOfRecords = numberOfRecords;

            return this;
        }

        /// <summary>
        /// Set search expression.
        /// </summary>
        [NotNull]
        public GblSettings SetSearchExpression
            (
                [NotNull] string searchExpression
            )
        {
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));

            SearchExpression = searchExpression;

            return this;
        }

        /// <summary>
        /// Convert settings to JSON string.
        /// </summary>
        [NotNull]
        public string ToJson()
        {
            string result = JObject.FromObject(this)
                .ToString(Formatting.None);

            return result;
        }

        #endregion

        #region IHandmadeSerializable

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            Actualize = reader.ReadBoolean();
            Autoin = reader.ReadBoolean();
            Database = reader.ReadNullableString();
            FileName = reader.ReadNullableString();
            FirstRecord = reader.ReadPackedInt32();
            FormalControl = reader.ReadBoolean();
            MaxMfn = reader.ReadPackedInt32();
            MfnList = reader.ReadNullableInt32Array();
            MinMfn = reader.ReadPackedInt32();
            NumberOfRecords = reader.ReadPackedInt32();
            SearchExpression = reader.ReadNullableString();
            Statements = reader.ReadNonNullCollection<GblStatement>();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            writer.Write(Actualize);
            writer.Write(Autoin);
            writer.WriteNullable(Database);
            writer.WriteNullable(FileName);
            writer.WritePackedInt32(FirstRecord);
            writer.Write(FormalControl);
            writer.WritePackedInt32(MaxMfn);
            writer.WriteNullableArray(MfnList);
            writer.WritePackedInt32(MinMfn);
            writer.WritePackedInt32(NumberOfRecords);
            writer.WriteNullable(SearchExpression);
            writer.Write(Statements);
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<GblSettings> verifier = new Verifier<GblSettings>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Database, "Database")
                .Assert(Statements.Count != 0, "Statements");

            foreach (GblStatement statement in Statements)
            {
                statement.Verify(throwOnError);
            }

            return verifier.Result;
        }

        #endregion
    }
}
