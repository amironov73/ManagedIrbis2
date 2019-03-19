// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BatchRecordFormatter.cs -- batch record formatter
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using AM;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Batch
{
    /// <summary>
    /// Batch formatter for <see cref="MarcRecord"/>.
    /// </summary>
    [PublicAPI]
    public sealed class BatchRecordFormatter
        : IEnumerable<string>
    {
        #region Events

        /// <summary>
        /// Raised on batch reading.
        /// </summary>
        public event EventHandler BatchRead;

        /// <summary>
        /// Raised when exception occurs.
        /// </summary>
        [CanBeNull]
        public event EventHandler<ExceptionEventArgs<Exception>> Exception;

        #endregion

        #region Properties

        /// <summary>
        /// Batch size.
        /// </summary>
        public int BatchSize { get; private set; }

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        /// <summary>
        /// Database name.
        /// </summary>
        [NotNull]
        public string Database { get; private set; }

        /// <summary>
        /// Format.
        /// </summary>
        [NotNull]
        public string Format { get; private set; }

        /// <summary>
        /// Total number of records formatted.
        /// </summary>
        public int RecordsFormatted { get; private set; }

        /// <summary>
        /// Number of records to format.
        /// </summary>
        public int TotalRecords { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public BatchRecordFormatter
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int batchSize,
                [NotNull] IEnumerable<int> range
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(format, nameof(format));
            Sure.NotNull(range, nameof(range));
            if (batchSize < 1)
            {
                Log.Error
                    (
                        nameof(BatchRecordFormatter) + "::Constructor: "
                        + nameof(batchSize) + "="
                        + batchSize
                    );

                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            Connection = connection;
            Database = database;
            BatchSize = batchSize;
            Format = format;

            _packages = range.Slice(batchSize).ToArray();
            TotalRecords = _packages.Sum(p => p.Length);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BatchRecordFormatter
            (
                [NotNull] string connectionString,
                [NotNull] string database,
                [NotNull] string format,
                int batchSize,
                [NotNull] IEnumerable<int> range
            )
        {
            Sure.NotNullNorEmpty(connectionString, nameof(connectionString));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(format, nameof(format));
            Sure.NotNull(range, nameof(range));
            if (batchSize < 1)
            {
                Log.Error
                    (
                        nameof(BatchRecordFormatter) + "::Constructor: "
                        + "batchSize="
                        + batchSize
                    );

                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            Connection = ConnectionFactory.CreateConnection(connectionString);
            _ownConnection = true;
            Database = database;
            BatchSize = batchSize;
            Format = format;

            _packages = range.Slice(batchSize).ToArray();
            TotalRecords = _packages.Sum(p => p.Length);
        }

        #endregion

        #region Private members

        private readonly bool _ownConnection;

        private readonly int[][] _packages;

        private bool _HandleException
            (
                Exception exception
            )
        {
            EventHandler<ExceptionEventArgs<Exception>> handler = Exception;

            if (ReferenceEquals(handler, null))
            {
                return false;
            }

            ExceptionEventArgs<Exception> arguments
                = new ExceptionEventArgs<Exception>(exception);
            handler(this, arguments);

            return arguments.Handled;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Read interval of records
        /// </summary>
        [NotNull]
        public static IEnumerable<string> Interval
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int firstMfn,
                int lastMfn,
                int batchSize
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(format, nameof(format));
            Sure.Positive(firstMfn, nameof(firstMfn));
            Sure.Positive(lastMfn, nameof(lastMfn));
            if (batchSize < 1)
            {
                Log.Error
                    (
                        "BatchRecordFormatter::Interval: "
                        + "batchSize="
                        + batchSize
                    );

                throw new ArgumentOutOfRangeException("batchSize");
            }

            int maxMfn = connection.GetMaxMfn(database) - 1;
            if (maxMfn == 0)
            {
                return StringUtility.EmptyArray;
            }

            lastMfn = Math.Min(lastMfn, maxMfn);
            if (firstMfn > lastMfn)
            {
                return StringUtility.EmptyArray;
            }

            BatchRecordFormatter result = new BatchRecordFormatter
                (
                    connection,
                    database,
                    format,
                    batchSize,
                    Enumerable.Range(firstMfn, lastMfn - firstMfn + 1)
                );

            return result;
        }

        /// <summary>
        /// Считывает все записи сразу.
        /// </summary>
        [NotNull]
        public List<string> FormatAll()
        {
            List<string> result = new List<string>(TotalRecords);

            foreach (string record in this)
            {
                result.Add(record);
            }

            return result;
        }

        /// <summary>
        /// Search and format records.
        /// </summary>
        [NotNull]
        public static IEnumerable<string> Search
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                [NotNull] string searchExpression,
                int batchSize
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(format, nameof(format));
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));
            if (batchSize < 1)
            {
                Log.Error
                    (
                        "BatchRecordFormatter::Search: "
                        + "batchSize="
                        + batchSize
                    );

                throw new ArgumentOutOfRangeException("batchSize");
            }

            int[] found = connection.Search(searchExpression);
            if (found.Length == 0)
            {
                return new string[0];
            }

            BatchRecordFormatter result = new BatchRecordFormatter
                (
                    connection,
                    database,
                    format,
                    batchSize,
                    found
                );

            return result;
        }

        /// <summary>
        /// Format whole database
        /// </summary>
        [NotNull]
        public static IEnumerable<string> WholeDatabase
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int batchSize
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(format, nameof(format));
            if (batchSize < 1)
            {
                Log.Error
                    (
                        nameof(BatchRecordFormatter) + "::" + nameof(WholeDatabase)
                        + ": " + nameof(batchSize) + "="
                        + batchSize
                    );

                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            int maxMfn = connection.GetMaxMfn(database) - 1;
            if (maxMfn == 0)
            {
                return Array.Empty<string>();
            }

            BatchRecordFormatter result = new BatchRecordFormatter
                (
                    connection,
                    database,
                    format,
                    batchSize,
                    Enumerable.Range(1, maxMfn)
                );

            return result;
        }

        #endregion

        #region IEnumerable members

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<string> GetEnumerator()
        {
            Log.Trace(nameof(BatchRecordFormatter) + "::" + nameof(GetEnumerator) + ": start");

            foreach (int[] package in _packages)
            {
                string[] records = Connection.FormatRecords
                    (
                        Database,
                        Format,
                        package
                    );

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!ReferenceEquals(records, null))
                {
                    RecordsFormatted += records.Length;
                    BatchRead.Raise(this);
                    foreach (string record in records)
                    {
                        yield return record;
                    }
                }
            }

            Log.Trace(nameof(BatchRecordFormatter) + "::" + nameof(GetEnumerator) + ": end");

            if (_ownConnection)
            {
                Connection.Dispose();
            }
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

