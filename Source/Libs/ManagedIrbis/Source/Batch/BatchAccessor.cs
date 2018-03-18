﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BatchAccessor.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.ImportExport;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Commands;


#endregion

namespace ManagedIrbis.Batch
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class BatchAccessor
    {
        #region Properties

        /// <summary>
        /// Throw <see cref="IrbisNetworkException"/>
        /// when empty record received/decoded.
        /// </summary>
        public static bool ThrowOnEmptyRecord { get; set; }

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Static constructor.
        /// </summary>
        static BatchAccessor()
        {
            ThrowOnEmptyRecord = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BatchAccessor
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Connection = connection;
        }

        #endregion

        #region Private members

        /// <summary>
        /// Throw <see cref="IrbisNetworkException"/>
        /// if the record is empty.
        /// </summary>
        private static void _ThrowIfEmptyRecord
            (
                [NotNull] MarcRecord record,
                [NotNull] string line
            )
        {
            Sure.NotNull(record, nameof(record));
            Sure.NotNull(line, nameof(line));

            if (ThrowOnEmptyRecord && record.Fields.Count == 0)
            {
                Log.Error
                    (
                        nameof(BatchAccessor) + "::" + nameof(_ThrowIfEmptyRecord)
                        + ": empty record detected"
                    );

                byte[] bytes = Encoding.UTF8.GetBytes(line);
                string dump = IrbisNetworkUtility.DumpBytes(bytes);
                string message = string.Format
                    (
                        "Empty record in BatchAccessor:{0}{1}",
                        Environment.NewLine,
                        dump
                    );

                IrbisNetworkException exception = new IrbisNetworkException(message);
                BinaryAttachment attachment = new BinaryAttachment
                    (
                        "response",
                        bytes
                    );
                exception.Attach(attachment);
                throw exception;
            }
        }

        private BlockingCollection<MarcRecord> _records;

        private void _ParseRecord
            (
                [NotNull] string line,
                [NotNull] string database
            )
        {
            if (!string.IsNullOrEmpty(line))
            {
                MarcRecord result = new MarcRecord
                {
                    HostName = Connection.Host,
                    Database = database
                };

                result = ProtocolText.ParseResponseForAllFormat
                    (
                        line,
                        result
                    );

                if (!ReferenceEquals(result, null))
                {
                    _ThrowIfEmptyRecord(result, line);

                    if (!result.Deleted)
                    {
                        result.Modified = false;
                        _records.Add(result);
                    }
                }
            }
        }

        private void _ParseRecord<T>
            (
                [NotNull] string line,
                [NotNull] string database,
                [NotNull] Func<MarcRecord, T> func,
                [NotNull] BlockingCollection<T> collection
            )
        {
            if (!string.IsNullOrEmpty(line))
            {
                MarcRecord record = new MarcRecord
                {
                    HostName = Connection.Host,
                    Database = database
                };

                record = ProtocolText.ParseResponseForAllFormat
                    (
                        line,
                        record
                    );

                if (!ReferenceEquals(record, null))
                {
                    if (!record.Deleted)
                    {
                        T result = func(record);

                        collection.Add(result);
                    }
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Read multiple records.
        /// </summary>
        [NotNull]
        public MarcRecord[] ReadRecords
            (
                [CanBeNull] string database,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(mfnList, nameof(mfnList));

            database = database.IfEmpty(Connection.Database)
                .ThrowIfNull(nameof(database));

            int[] array = mfnList.ToArray();

            if (array.Length == 0)
            {
                return Array.Empty<MarcRecord>();
            }

            if (array.Length == 1)
            {
                int mfn = array[0];

                MarcRecord record = Connection.ReadRecord
                    (
                        database,
                        mfn,
                        false,
                        null
                    );

                return new[] { record };
            }

            using (_records = new BlockingCollection<MarcRecord>(array.Length))
            {
                int[][] slices = array.Slice(1000).ToArray();

                foreach (int[] slice in slices)
                {
                    if (slice.Length == 1)
                    {
                        MarcRecord record = Connection.ReadRecord
                            (
                                database,
                                slice[0],
                                false,
                                null
                            );

                        _records.Add(record);
                    }
                    else
                    {
                        FormatCommand command
                            = Connection.CommandFactory.GetFormatCommand();
                        command.Database = database;
                        command.FormatSpecification = IrbisFormat.All;
                        command.MfnList.AddRange(slice);

                        Connection.ExecuteCommand(command);

                        string[] lines = command.FormatResult
                            .ThrowIfNullOrEmpty
                                (
                                    "command.FormatResult"
                                );

                        Debug.Assert
                            (
                                lines.Length == slice.Length,
                                "some records not retrieved"
                            );

                        Parallel.ForEach
                            (
                                lines,
                                line => _ParseRecord(line, database)
                            );
                    }
                }

                _records.CompleteAdding();

                return _records.ToArray();
            }
        }

        /// <summary>
        /// Read and transform multiple records.
        /// </summary>
        [NotNull]
        public T[] ReadRecords<T>
            (
                [CanBeNull] string database,
                [NotNull] IEnumerable<int> mfnList,
                [NotNull] Func<MarcRecord, T> func
            )
        {
            Sure.NotNull(mfnList, nameof(mfnList));

            database = database.IfEmpty(Connection.Database)
                .ThrowIfNull(nameof(database));

            int[] array = mfnList.ToArray();

            if (array.Length == 0)
            {
                return Array.Empty<T>();
            }

            if (array.Length == 1)
            {
                int mfn = array[0];

                MarcRecord record = Connection.ReadRecord
                    (
                        database,
                        mfn,
                        false,
                        null
                    );

                T result1 = func(record);

                return new[] { result1 };
            }

            using (BlockingCollection<T> collection
                = new BlockingCollection<T>(array.Length))
            {

                int[][] slices = array.Slice(1000).ToArray();

                foreach (int[] slice in slices)
                {
                    if (slice.Length == 1)
                    {
                        MarcRecord record = Connection.ReadRecord
                            (
                                database,
                                slice[0],
                                false,
                                null
                            );

                        _records.Add(record);
                    }
                    else
                    {
                        FormatCommand command = Connection.CommandFactory
                                .GetFormatCommand();
                        command.Database = database;
                        command.FormatSpecification = IrbisFormat.All;
                        command.MfnList.AddRange(slice);

                        Connection.ExecuteCommand(command);

                        string[] lines = command.FormatResult
                            .ThrowIfNullOrEmpty(nameof(command.FormatResult));

                        Debug.Assert
                            (
                                lines.Length == slice.Length,
                                "some records not retrieved"
                            );

                        Parallel.ForEach
                            (
                                lines,
                                line => _ParseRecord
                                    (
                                        line,
                                        database,
                                        func,

                                        // ReSharper disable once AccessToDisposedClosure
                                        collection
                                    )
                            );
                    }
                }

                collection.CompleteAdding();

                return collection.ToArray();
            }
        }

        #endregion
    }
}
