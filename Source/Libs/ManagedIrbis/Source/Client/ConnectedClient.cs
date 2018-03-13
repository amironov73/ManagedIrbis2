﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConnectedClient.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AM;
using AM.IO;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis.Batch;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Menus;
using ManagedIrbis.Pft;
using ManagedIrbis.Search;

#endregion

namespace ManagedIrbis.Client
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public class ConnectedClient
        : IrbisProvider
    {
        #region Properties

        /// <inheritdoc cref="IrbisProvider.BusyState" />
        public override BusyState BusyState => Connection.Busy;

        /// <inheritdoc cref="IrbisProvider.Connected" />
        public override bool Connected => Connection.Connected;

        /// <inheritdoc cref="IrbisProvider.Database" />
        public override string Database
        {
            get => Connection.Database;
            set => Connection.Database = value;
        }

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectedClient()
        {
            _ownConnection = true;
            Connection = new IrbisConnection();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectedClient
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            _ownConnection = false;
            Connection = connection;
        }

        #endregion

        #region Private members

        private readonly bool _ownConnection;

        #endregion

        #region Public methods

        /// <summary>
        /// Connect.
        /// </summary>
        public void Connect()
        {
            Connection.Connect();
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect()
        {
            Connection.Dispose();
        }

        /// <summary>
        /// Parse connection string.
        /// </summary>
        public void ParseConnectionString
            (
                [NotNull] string connectionString
            )
        {
            Sure.NotNullNorEmpty(connectionString, nameof(connectionString));

            Connection.ParseConnectionString(connectionString);
        }

        #endregion

        #region IrbisProvider members

        /// <inheritdoc cref="IrbisProvider.AcquireFormatter" />
        public override IPftFormatter AcquireFormatter()
        {
            return new ConnectedFormatter(Connection);
        }

        /// <inheritdoc cref="IrbisProvider.Configure" />
        public override void Configure
            (
                string configurationString
            )
        {
            Sure.NotNullNorEmpty(configurationString, nameof(configurationString));

            Connection.ParseConnectionString(configurationString);
            Connection.Connect();
        }

        /// <inheritdoc cref="IrbisProvider.ExactSearchLinks"/>
        public override TermLink[] ExactSearchLinks
            (
                string term
            )
        {
            Sure.NotNullNorEmpty(term, nameof(term));

            PostingParameters parameters = new PostingParameters
            {
                Database = Connection.Database,
                Term = term
            };
            TermPosting[] postings = Connection.ReadPostings(parameters);
            TermLink[] result = TermLink.FromPostings(postings);

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.FormatRecord" />
        public override string FormatRecord
            (
                MarcRecord record,
                string format
            )
        {
            Sure.NotNull(record, nameof(record));
            Sure.NotNull(format, nameof(format));

            string result = Connection.FormatRecord
                (
                    format,
                    record
                );

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.FormatRecords" />
        public override string[] FormatRecords
            (
                int[] mfns,
                string format
            )
        {
            string[] result = Connection.FormatRecords
                (
                    Database,
                    format,
                    mfns
                );

            return result;
        }


        /// <inheritdoc cref="IrbisProvider.GetAlphabetTable" />
        public override IrbisAlphabetTable GetAlphabetTable()
        {
            return new IrbisAlphabetTable(Connection);
        }

        /// <inheritdoc cref="IrbisProvider.GetCatalogState" />
        public override CatalogState GetCatalogState
            (
                string database
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));

            var lines = BatchRecordFormatter.WholeDatabase
                (
                    Connection,
                    database,
                    "&uf(\'G0$\',&uf(\'+0\'))",
                    1000
                );

            CatalogState result = new CatalogState
            {
                Database = database,
                Date = PlatformAbstraction.Value.Today(),
                MaxMfn = Connection.GetMaxMfn(database)
            };

            List <RecordState> records
                = new List<RecordState>(result.MaxMfn);

            foreach (string line in lines)
            {
                RecordState record
                    = RecordState.ParseServerAnswer(line);
                if (record.Mfn != 0)
                {
                    records.Add(record);
                }
            }

            result.Records = records.ToArray();

            DatabaseInfo info
                = Connection.GetDatabaseInfo(database);
            result.LogicallyDeleted
                = info.LogicallyDeletedRecords
                .ThrowIfNull("info.LogicallyDeletedRecords")
                .Where(mfn => mfn != 0)
                .ToArray();

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.GetMaxMfn" />
        public override int GetMaxMfn()
        {
            return Connection.GetMaxMfn();
        }

        /// <inheritdoc cref="IrbisProvider.GetStopWords" />
        public override IrbisStopWords GetStopWords()
        {
            return IrbisStopWords.FromServer(Connection);
        }

        /// <inheritdoc cref="IrbisProvider.NoOp" />
        public override void NoOp()
        {
            Connection.NoOp();
        }

        /// <inheritdoc cref="IrbisProvider.ReadFile" />
        public override string ReadFile
            (
                FileSpecification fileSpecification
            )
        {
            return Connection.ReadTextFile(fileSpecification);
        }

        /// <inheritdoc cref="IrbisProvider.ReadIniFile" />
        public override IniFile ReadIniFile
            (
                FileSpecification fileSpecification
            )
        {
            string content = ReadFile(fileSpecification);
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            IniFile result = new IniFile();
            using (StringReader reader = new StringReader(content))
            {
                result.Read(reader);
            }

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReadRecord" />
        public override MarcRecord ReadRecord
            (
                int mfn
            )
        {
            return Connection.ReadRecord(mfn);
        }

        /// <inheritdoc cref="IrbisProvider.ReadMenuFile" />
        public override MenuFile ReadMenuFile
            (
                FileSpecification fileSpecification
            )
        {
            Sure.NotNull(fileSpecification, nameof(fileSpecification));

            return Connection.ReadMenu(fileSpecification);
        }

        /// <inheritdoc cref="IrbisProvider.ReadTerms" />
        public override TermInfo[] ReadTerms
            (
                TermParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            TermInfo[] result = Connection
                .ReadTerms(parameters)
                .Where(term => term.Count != 0)
                .ToArray();

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.Reconnect" />
        public override void Reconnect()
        {
            Connection.Reconnect();
        }

        /// <inheritdoc cref="IrbisProvider.ReleaseFormatter" />
        public override void ReleaseFormatter
            (
                IPftFormatter formatter
            )
        {
            // Nothing to do here
        }

        /// <inheritdoc cref="IrbisProvider.Search" />
        public override int[] Search
            (
                string expression
            )
        {
            if (ReferenceEquals(expression, null) || expression.Length == 0)
            {
                return Array.Empty<int>();
            }

            int[] result = Connection.Search(expression);

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.WriteRecord" />
        public override void WriteRecord
            (
                MarcRecord record
            )
        {
            Sure.NotNull(record, nameof(record));

            Connection.WriteRecord(record, false, true);
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IrbisProvider.Dispose" />
        public override void Dispose()
        {
            base.Dispose();

            if (_ownConnection)
            {
                Connection.Dispose();
            }
        }

        #endregion
    }
}
