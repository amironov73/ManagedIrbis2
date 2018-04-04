// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConnectedFormatter.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.Pft;

#endregion

namespace ManagedIrbis.Client
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class ConnectedFormatter
        : IPftFormatter
    {
        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        /// <summary>
        /// Format source.
        /// </summary>
        [CanBeNull]
        public string Source { get; set; }

        /// <inheritdoc cref="IPftFormatter.SupportsExtendedSyntax" />
        public bool SupportsExtendedSyntax => false;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectedFormatter
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Connection = connection;
        }

        #endregion

        #region IPftFormatter members

        /// <inheritdoc cref="IPftFormatter.FormatRecord(MarcRecord)" />
        public string FormatRecord
            (
                MarcRecord record
            )
        {
            if (ReferenceEquals(record, null)
                || ReferenceEquals(Source, null)
                || Source.Length == 0)
            {
                return string.Empty;
            }

            string result = Connection.FormatRecord(Source, record)
                ?? string.Empty;

            return result;
        }

        /// <inheritdoc cref="IPftFormatter.FormatRecord(Int32)" />
        public string FormatRecord
            (
                int mfn
            )
        {
            Sure.Positive(mfn, nameof(mfn));

            if (ReferenceEquals(Source, null) || Source.Length == 0)
            {
                return string.Empty;
            }

            string result = Connection.FormatRecord(Source, mfn)
                ?? string.Empty;

            return result;
        }

        /// <inheritdoc cref="IPftFormatter.FormatRecords" />
        public string[] FormatRecords
            (
                int[] mfns
            )
        {
            Sure.NotNull(mfns, nameof(mfns));

            string source = Source.ThrowIfNull(nameof(Source));
            string[] result = Connection.FormatRecords
                (
                    database: Connection.Database,
                    format: source,
                    mfnList: mfns
                );

            return result;
        }

        /// <inheritdoc cref="IPftFormatter.ParseProgram" />
        public void ParseProgram
            (
                string source
            )
        {
            Source = source;
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            // Nothing to do here
        }

        #endregion
    }
}
