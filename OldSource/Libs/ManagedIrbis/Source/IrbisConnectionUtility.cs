﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisConnectionUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.Configuration;
using AM.IO;
using AM.Logging;
using AM.Text;

using JetBrains.Annotations;

using ManagedIrbis.Batch;
using ManagedIrbis.Menus;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.ClientCommands;
using ManagedIrbis.Infrastructure.Sockets;
using ManagedIrbis.Search;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class IrbisConnectionUtility
    {
        #region Properties

        /// <summary>
        /// Default connection string.
        /// </summary>
        [CanBeNull]
        public static string DefaultConnectionString { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Актуализация всех неактуализированных записей
        /// в указанной базе данных.
        /// </summary>
        public static void ActualizeDatabase
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));

            connection.ActualizeRecord
                (
                    database,
                    0
                );
        }

        // ========================================================

        /// <summary>
        /// Delete specified file on the server.
        /// </summary>
        /// <remarks>
        /// <para>Accepts wildcards.</para>
        /// <para>Requires server version 2010.1 or newer.</para>
        /// </remarks>
        public static void DeleteAnyFile
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(fileName, nameof(fileName));

            connection.RequireServerVersion("2010.1", true);

            connection.FormatRecord
                (
                    "&uf('+9K" + fileName + "')",
                    1
                );
        }

        // =========================================================

        /// <summary>
        /// Delete given record (mark as deleted on the server).
        /// </summary>
        public static void DeleteRecord
            (
                [NotNull] this IIrbisConnection connection,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));

            MarcRecord record = connection.ReadRecord(mfn);
            if (!record.Deleted)
            {
                record.Deleted = true;
                connection.WriteRecord(record);
            }
        }

        /// <summary>
        /// Delete given record (mark as deleted on the server).
        /// </summary>
        public static void DeleteRecord
            (
                [NotNull] this IIrbisConnection connection,
                int mfn,
                bool dontParseResponse
            )
        {
            Sure.NotNull(connection, nameof(connection));

            MarcRecord record = connection.ReadRecord(mfn);
            if (!record.Deleted)
            {
                record.Deleted = true;
                connection.WriteRecord
                    (
                        record,
                        dontParseResponse
                    );
            }
        }

        // ========================================================

        /// <summary>
        /// Delete some records (mark as deleted on the server).
        /// </summary>
        public static void DeleteRecords
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(mfnList, nameof(mfnList));

            MarcRecord[] records = connection.ReadRecords
                (
                    database,
                    mfnList
                );

            if (records.Length == 0)
            {
                return;
            }

            MarcRecord[] liveRecords = records
                .Where(record => !record.Deleted)
                .ToArray();

            foreach (MarcRecord record in liveRecords)
            {
                record.Deleted = true;
                connection.WriteRecord(record);
            }
        }

        // ========================================================

        /// <summary>
        /// Execute arbitrary command.
        /// </summary>
        [NotNull]
        public static ServerResponse ExecuteArbitraryCommand
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string commandCode,
                params object[] arguments
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            UniversalCommand command = new UniversalCommand
                (
                    commandCode,
                    arguments
                )
            {
                AcceptAnyResponse = true
            };

            ServerResponse result = connection.ExecuteCommand(command);

            return result;
        }

        // =========================================================

        /// <summary>
        /// Sequential search.
        /// </summary>
        [NotNull]
        public static MarcRecord[] ExtendedSearch
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] SearchParameters parameters
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(parameters, nameof(parameters));

            if (string.IsNullOrEmpty(parameters.FormatSpecification))
            {
                parameters.FormatSpecification = IrbisFormat.All;
            }

            SearchReadCommand command = new SearchReadCommand();
            command.ApplyParameters(parameters);

            connection.ExecuteCommand(command);

            MarcRecord[] records = command.Records.ThrowIfNull(nameof(command.Records));

            if (!string.IsNullOrEmpty(parameters.FilterSpecification))
            {
                return records;
            }

            throw new NotImplementedException();

            //using (IrbisProvider provider = new ConnectedClient(connection))
            //using (PftFormatter formatter = new PftFormatter())
            //{
            //    formatter.SetProvider(provider);
            //    string specification = parameters.FilterSpecification.ThrowIfNull();

            //    if (!specification.StartsWith("if"))
            //    {
            //        specification = string.Format
            //            (
            //                "if {0} then '1' else '0'",
            //                specification
            //            );
            //    }

            //    formatter.ParseProgram(specification);

            //    List<MarcRecord> result = new List<MarcRecord>(records.Length);

            //    foreach (MarcRecord record in records)
            //    {
            //        string text = formatter.FormatRecord(record);
            //        if (text.SameString("1"))
            //        {
            //            result.Add(record);
            //        }
            //    }

            //    return result.ToArray();
            //}
        }

        // ========================================================

        /// <summary>
        /// Format specified record.
        /// </summary>
        public static string FormatRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(format, nameof(format));
            Sure.Positive(mfn, nameof(mfn));

            FormatCommand command = new FormatCommand
            {
                Database = database,
                FormatSpecification = format,
                UtfFormat = true
            };
            command.MfnList.Add(mfn);

            connection.ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty(nameof(command.FormatResult))
                [0];

            return result;
        }

        // ========================================================

        /// <summary>
        /// Format specified record using UTF8 encoding.
        /// </summary>
        public static string FormatUtf8
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string format,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(format, nameof(format));
            Sure.Positive(mfn, nameof(mfn));

            FormatCommand command = new FormatCommand
            {
                FormatSpecification = format,
                UtfFormat = true
            };
            command.MfnList.Add(mfn);

            connection.ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty(nameof(command.FormatResult))
                [0];

            return result;
        }

        /// <summary>
        /// Format specified record using UTF8 encoding.
        /// </summary>
        public static string FormatUtf8
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string format,
                [NotNull] MarcRecord record
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(format, nameof(format));
            Sure.NotNull(record, nameof(record));

            FormatCommand command = new FormatCommand
            {
                FormatSpecification = format,
                UtfFormat = true,
                VirtualRecord = record
            };

            connection.ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty(nameof(command.FormatResult))
                [0];

            return result;

        }

        /// <summary>
        /// Format specified records using UTF8 encoding.
        /// </summary>
        [NotNull]
        public static string[] FormatUtf8
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string format,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(mfnList, nameof(mfnList));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(format, nameof(format));

            FormatCommand command = new FormatCommand
            {
                Database = database,
                FormatSpecification = format,
                UtfFormat = true
            };
            command.MfnList.AddRange(mfnList);

            if (command.MfnList.Count == 0)
            {
                return StringUtility.EmptyArray;
            }

            connection.ExecuteCommand(command);

            string[] result = command.FormatResult
                .ThrowIfNull(nameof(command.FormatResult));

            return result;
        }

        // ========================================================

        /// <summary>
        /// Получаем строку подключения в app.settings.
        /// </summary>
        public static string GetStandardConnectionString()
        {
            if (!ReferenceEquals(DefaultConnectionString, null))
            {
                return DefaultConnectionString;
            }

            return ConfigurationUtility.FindSetting
                (
                    ListStandardConnectionStrings()
                );
        }

        // ========================================================

        /// <summary>
        /// Получаем уже подключенного клиента.
        /// </summary>
        /// <exception cref="IrbisException">
        /// Если строка подключения в app.settings не найдена.
        /// </exception>
        [ExcludeFromCodeCoverage]
        public static IrbisConnection GetClientFromConfig()
        {
            string connectionString = GetStandardConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new IrbisException("Connection string not specified!");
            }

            IrbisConnection result = new IrbisConnection(connectionString);

            return result;
        }

        // =========================================================

        /// <summary>
        /// Get list of the databases.
        /// </summary>
        [NotNull]
        public static DatabaseInfo[] ListDatabases
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string listFile
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(listFile, nameof(listFile));

            string menuFile = connection.ReadTextFile(IrbisPath.Data, listFile);
            string[] lines = menuFile.ThrowIfNull(nameof(menuFile)).SplitLines();
            DatabaseInfo[] result = DatabaseInfo.ParseMenu(lines);

            return result;
        }

        /// <summary>
        /// Get list of the databases.
        /// </summary>
        [NotNull]
        public static DatabaseInfo[] ListDatabases
            (
                [NotNull] this IIrbisConnection connection
            )
        {
            return ListDatabases(connection, IrbisConstants.AdministratorDatabaseList);
        }

        // ========================================================

        /// <summary>
        /// Стандартные наименования для ключа строки подключения
        /// к серверу ИРБИС64.
        /// </summary>
        public static string[] ListStandardConnectionStrings()
        {
            return new[]
            {
                "irbis-connection",
                "irbis-connection-string",
                "irbis64-connection",
                "irbis64",
                "connection-string"
            };
        }

        // ========================================================

        /// <summary>
        /// Lock the record on the server.
        /// </summary>
        public static void LockRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));

            ReadRawRecord
                (
                    connection,
                    database,
                    mfn,
                    true,
                    null
                );
        }

        // ========================================================

        /// <summary>
        /// Lock some records on the server.
        /// </summary>
        public static void LockRecords
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] int[] mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(mfnList, nameof(mfnList));

            foreach (int mfn in mfnList)
            {
                ReadRawRecord
                    (
                        connection,
                        database,
                        mfn,
                        true,
                        null
                    );
            }
        }

        // ========================================================

        /// <summary>
        /// Read any binary file from the server.
        /// </summary>
        /// <remarks>
        /// Works with version 2010.1 and newer.
        /// </remarks>
        [CanBeNull]
        public static byte[] ReadAnyBinaryFile
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string serverPath
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(serverPath, nameof(serverPath));

            connection.RequireServerVersion("2010.1", true);

            string text = connection.FormatRecord
                (
                    "&uf('+9J" + serverPath + "')",
                    1
                );

            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return null;
            }

            RecordField field = RecordField.Parse(1, text);
            text = field.GetFirstSubFieldValue('b');
            byte[] result = IrbisUtility.DecodePercentString(text);

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read (almost) any text file from the server.
        /// </summary>
        /// <remarks>
        /// Works with version 2010.1 and newer.
        /// </remarks>
        [CanBeNull]
        public static string ReadAnyTextFile
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string serverPath,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(serverPath, nameof(serverPath));
            Sure.NotNull(encoding, nameof(encoding));

            connection.RequireServerVersion("2010.1", true);

            string result = connection.FormatRecord
                (
                    "&uf('+9J" + serverPath + "')",
                    1
                );

            if (!ReferenceEquals(result, null) && result.Length != 0)
            {
                RecordField field = RecordField.Parse(1, result);
                result = field.GetFirstSubFieldValue('b');
                byte[] bytes = IrbisUtility.DecodePercentString(result);
                result = encoding.GetString(bytes);
            }

            return result;
        }

        /// <summary>
        /// Read (almost) any text file from the server.
        /// </summary>
        /// <remarks>
        /// Uses ANSI encoding.
        /// </remarks>
        [CanBeNull]
        public static string ReadAnyTextFile
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string serverPath
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(serverPath, nameof(serverPath));

            connection.RequireServerVersion("2010.1", true);

            string result = connection.FormatRecord
                (
                    "&uf('+9J" + serverPath + "')",
                    1
                );

            if (!ReferenceEquals(result, null) && result.Length != 0)
            {
                RecordField field = RecordField.Parse(1, result);
                result = field.GetFirstSubFieldValue('b');
                byte[] bytes = IrbisUtility.DecodePercentString(result);
                result = IrbisEncoding.Ansi.GetString(bytes);
            }

            return result;
        }

        /// <summary>
        /// Read INI-file from server.
        /// </summary>
        [NotNull]
        public static IniFile ReadIniFile
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            FileSpecification fileSpecification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    connection.Database,
                    fileName
                );
            string text = connection.ReadTextFile(fileSpecification)
                .ThrowIfNull("text");
            IniFile result = new IniFile
            {
                FileName = fileName
            };
            try
            {
                using (TextReader reader = new StringReader(text))
                {
                    result.Read(reader);
                }
            }
            catch (Exception exception)
            {
                Log.TraceException
                    (
                        nameof(IrbisConnectionUtility) + "::" + nameof(ReadIniFile),
                        exception
                    );

                result.Dispose();
                throw;
            }

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read menu from server.
        /// </summary>
        [NotNull]
        public static MenuFile ReadMenu
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] FileSpecification fileSpecification
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(fileSpecification, nameof(fileSpecification));

            string text = connection.ReadTextFile(fileSpecification);
            MenuFile result = MenuFile.ParseServerResponse(text.ThrowIfNull("text"));

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read menu from server.
        /// </summary>
        [NotNull]
        public static MenuFile ReadMenu
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            FileSpecification fileSpecification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    connection.Database,
                    fileName
                );
            string text = connection.ReadTextFile(fileSpecification);
            MenuFile result = MenuFile.ParseServerResponse
                (
                    text.ThrowIfNull("text")
                );

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read server representation of record from server.
        /// </summary>
        [CanBeNull]
        public static RawRecord ReadRawRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));

            ReadRawRecordCommand command = new ReadRawRecordCommand
            {
                Database = database,
                Mfn = mfn
            };

            connection.ExecuteCommand(command);

            return command.RawRecord;
        }

        // ========================================================

        /// <summary>
        /// Read server representation of record from server.
        /// </summary>
        [CanBeNull]
        public static RawRecord ReadRawRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                int mfn,
                bool lockFlag,
                [CanBeNull] string format
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));

            ReadRawRecordCommand command = new ReadRawRecordCommand
            {
                Database = database,
                Mfn = mfn,
                Lock = lockFlag,
                Format = format
            };

            connection.ExecuteCommand(command);

            return command.RawRecord;
        }

        // ========================================================

        /// <summary>
        /// Read server representation of record from server.
        /// </summary>
        [CanBeNull]
        public static RawRecord ReadRawRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                int mfn,
                int version,
                [CanBeNull] string format
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));

            ReadRawRecordCommand command = new ReadRawRecordCommand
            {
                Database = database,
                Mfn = mfn,
                VersionNumber = version,
                Format = format
            };

            connection.ExecuteCommand(command);

            return command.RawRecord;
        }

        // ========================================================

        /// <summary>
        /// Read server representation of record from server.
        /// </summary>
        [NotNull]
        public static RawRecord[] ReadRawRecords
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] int[] mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(mfnList, nameof(mfnList));

            if (mfnList.Length == 0)
            {
                return Array.Empty<RawRecord>();
            }

            if (mfnList.Length == 1)
            {
                return new[]
                {
                    connection.ReadRawRecord
                       (
                            database,
                            mfnList[0]
                       )
                };
            }

            List<object> arguments = new List<object>
                (
                    mfnList.Length + 3
                )
            {
                database,
                IrbisFormat.All,
                mfnList.Length
            };
            foreach (int mfn in mfnList)
            {
                arguments.Add(mfn);
            }

            UniversalCommand command = new UniversalCommand
                (
                    CommandCode.FormatRecord,
                    arguments.ToArray()
                )
            {
                AcceptAnyResponse = true
            };

            ServerResponse response = connection.ExecuteCommand(command);

            List<string> lines = response.RemainingUtfStrings();
            lines = lines.GetRange(1, lines.Count - 1);

            BlockingCollection<RawRecord> records
                = new BlockingCollection<RawRecord>(lines.Count);

            Parallel.ForEach
                (
                    lines,
                    line =>
                    {
                        RawRecord record = RawRecord.Parse(line);
                        record.Database = database;
                        records.Add(record);
                    }
                );

            return records.ToArray();
        }

        // ========================================================

        /// <summary>
        /// Read multiple records.
        /// </summary>
        [NotNull]
        public static MarcRecord[] ReadRecords
            (
                [NotNull] this IIrbisConnection connection,
                [CanBeNull] string database,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(mfnList, nameof(mfnList));

            BatchAccessor batch = new BatchAccessor(connection);
            MarcRecord[] result = batch.ReadRecords
                (
                    database,
                    mfnList
                );

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read search scenario from server.
        /// </summary>
        [NotNull]
        public static SearchScenario[] ReadSearchScenario
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using (IniFile iniFile = ReadIniFile(connection, fileName))
            {
                SearchScenario[] result = SearchScenario.ParseIniFile(iniFile);

                return result;
            }
        }

        // ========================================================

        /// <summary>
        /// Remove logging from socket.
        /// </summary>
        public static void RemoveLogging
            (
                [NotNull] this IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            if (connection.Socket is LoggingClientSocket oldSocket)
            {
                ClientSocket newSocket = oldSocket.InnerSocket
                    .ThrowIfNull(nameof(oldSocket.InnerSocket));
                connection.SetSocket(newSocket);
            }
        }

        // ========================================================

        /// <summary>
        /// Retrieve history for given record.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static MarcRecord[] RecordHistory
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                int mfn,
                [CanBeNull] string format
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.Positive(mfn, nameof(mfn));

            List<MarcRecord> result = new List<MarcRecord>();
            MarcRecord record = connection.ReadRecord
                (
                    database,
                    mfn,
                    false,
                    format
                );
            result.Add(record);

            for (int version = 2; ; version++)
            {
                record = connection.ReadRecord
                    (
                        database,
                        mfn,
                        version,
                        format
                    );
                if (ReferenceEquals(record, null))
                {
                    break;
                }
                result.Add(record);
            }

            return result.ToArray();
        }

        // ========================================================

        /// <summary>
        /// Чтение записи.
        /// </summary>
        [NotNull]
        public static MarcRecord ReadRecord
            (
                [NotNull] this IIrbisConnection connection,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));

            MarcRecord result = connection.ReadRecord
                (
                    connection.Database,
                    mfn,
                    false,
                    null
                );

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read text file from the server.
        /// </summary>
        [CanBeNull]
        public static string ReadTextFile
            (
                [NotNull] this IIrbisConnection connection,
                IrbisPath path,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            FileSpecification fileSpecification = new FileSpecification
                (
                    path,
                    connection.Database,
                    fileName
                );

            string result = connection.ReadTextFile(fileSpecification);

            return result;
        }

        // ========================================================

        /// <summary>
        /// Require minimal client version.
        /// </summary>
        public static bool RequireClientVersion
            (
                [CanBeNull] this IIrbisConnection connection,
                [NotNull] string minimalVersion,
                bool throwException
            )
        {
            Sure.NotNullNorEmpty(minimalVersion, nameof(minimalVersion));

            Version requiredVersion = new Version(minimalVersion);
            Version actualVersion = IrbisConnection.ClientVersion;
            bool result = actualVersion.CompareTo(requiredVersion) >= 0;

            if (!result && throwException)
            {
                string message = string.Format
                    (
                        "Required client version {0}, found version {1}",
                        minimalVersion,
                        actualVersion
                    );
                throw new IrbisException(message);
            }

            return result;
        }

        // ========================================================

        /// <summary>
        /// Require minimal server version.
        /// </summary>
        public static bool RequireServerVersion
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string minimalVersion,
                bool throwException
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(minimalVersion, nameof(minimalVersion));

            IrbisVersion actualVersion = connection.ServerVersion
                ?? connection.GetServerVersion();
            bool result = string.CompareOrdinal
                (
                    actualVersion.Version,
                    "64." + minimalVersion
                ) >= 0;

            if (!result && throwException)
            {
                string message = string.Format
                    (
                        "Required server version {0}, found version {1}",
                        minimalVersion,
                        actualVersion.Version
                    );
                throw new IrbisException(message);
            }

            return result;
        }

        // ========================================================

        /// <summary>
        /// Search for records with formattable expression.
        /// </summary>
        [NotNull]
        [StringFormatMethod("format")]
        public static int[] Search
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string format,
                params object[] args
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(format, nameof(format));

            string expression = string.Format
                (
                    format,
                    args
                );

            return connection.Search(expression);
        }

        // ========================================================

        /// <summary>
        /// Определение количества записей, которые будут
        /// найдены по указанному выражению.
        /// </summary>
        public static int SearchCount
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string searchExpression
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));

            SearchCommand command = new SearchCommand
            {
                Database = connection.Database,
                SearchExpression = searchExpression,
                FirstRecord = 0
            };

            connection.ExecuteCommand(command);

            int result = command.FoundCount;

            return result;
        }

        // ========================================================

        /// <summary>
        /// Поиск с одновременным расформатированием.
        /// Для формата используется кодировка ANSI.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static FoundItem[] SearchFormat
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string searchExpression,
                [NotNull] string formatSpecification
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));
            Sure.NotNullNorEmpty(formatSpecification, nameof(formatSpecification));

            SearchCommand command = new SearchCommand
            {
                Database = connection.Database,
                SearchExpression = searchExpression,
                FormatSpecification = formatSpecification
            };

            connection.ExecuteCommand(command);

            FoundItem[] result = command.Found
                .ThrowIfNull(nameof(command.Found))
                .ToArray();

            return result;
        }

        /// <summary>
        /// Поиск с одновременным расформатированием.
        /// Для формата используется кодировка UTF-8.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static FoundItem[] SearchFormatUtf8
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string searchExpression,
                [NotNull] string formatSpecification
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(searchExpression, nameof(searchExpression));
            Sure.NotNullNorEmpty(formatSpecification, nameof(formatSpecification));

            SearchCommand command = new SearchCommand
            {
                Database = connection.Database,
                SearchExpression = searchExpression,
                FormatSpecification = formatSpecification,
                UtfFormat = true
            };

            connection.ExecuteCommand(command);

            FoundItem[] result = command.Found
                .ThrowIfNull(nameof(command.Found))
                .ToArray();

            return result;
        }

        // ========================================================

        /// <summary>
        /// Raw search.
        /// </summary>
        [NotNull]
        public static string[] SearchRaw
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] SearchParameters parameters
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(parameters, nameof(parameters));

            SearchRawCommand command = new SearchRawCommand();
            command.ApplyParameters(parameters);

            connection.ExecuteCommand(command);

            string[] result = command.Found
                .ThrowIfNull(nameof(command.Found));

            return result;
        }

        // ========================================================

        /// <summary>
        /// Загрузка записей по результатам поиска.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        [StringFormatMethod("format")]
        public static MarcRecord[] SearchRead
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string format,
                params object[] args
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(format, nameof(format));

            string expression = string.Format(format, args);
            SearchReadCommand command = new SearchReadCommand
            {
                Database = connection.Database,
                SearchExpression = expression
            };

            connection.ExecuteCommand(command);

            MarcRecord[] result = command.Records.ThrowIfNull(nameof(command.Records));

            return result;
        }

        // ========================================================

        /// <summary>
        /// Загрузка одной записи по результатам поиска.
        /// </summary>
        [CanBeNull]
        [StringFormatMethod("format")]
        public static MarcRecord SearchReadOneRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string format,
                params object[] args
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(format, nameof(format));

            string expression = string.Format(format, args);
            SearchReadCommand command = new SearchReadCommand
            {
                Database = connection.Database,
                SearchExpression = expression,
                NumberOfRecords = 1
            };

            connection.ExecuteCommand(command);

            MarcRecord result = command.Records
                .ThrowIfNull(nameof(command.Records))
                .GetItem(0);

            return result;
        }

        // ========================================================

        /// <summary>
        /// Raw sequential search.
        /// </summary>
        [NotNull]
        public static string[] SequentialSearchRaw
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string expression,
                int firstRecord,
                int numberOfRecords,
                int minMfn,
                int maxMfn,
                [NotNull] string sequential,
                [CanBeNull] string format
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(expression, nameof(expression));
            Sure.NotNullNorEmpty(sequential, nameof(sequential));

            throw new NotImplementedException();

            //UniversalCommand command = connection.CommandFactory.CreateUniversalCommand
            //    (
            //        CommandCode.Search,
            //        database,
            //        new TextWithEncoding(expression, IrbisEncoding.Utf8),
            //        numberOfRecords,
            //        firstRecord,
            //        new TextWithEncoding(format, IrbisEncoding.Ansi),
            //        minMfn,
            //        maxMfn,
            //        new TextWithEncoding(sequential, IrbisEncoding.Utf8)
            //    );

            //ServerResponse response = connection.ExecuteCommand(command);

            //List<string> result = response.RemainingUtfStrings();

            //return result.ToArray();
        }

        // ========================================================

        /// <summary>
        /// Undelete given record (mark as live on the server).
        /// </summary>
        public static void UndeleteRecord
            (
                [NotNull] this IIrbisConnection connection,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));

            MarcRecord record = connection.ReadRecord(mfn);
            if (record.Deleted)
            {
                record.Deleted = false;
                connection.WriteRecord(record);
            }
        }

        // ========================================================

        /// <summary>
        /// Undelete some records (mark as live on the server).
        /// </summary>
        public static void UndeleteRecords
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(mfnList, nameof(mfnList));

            MarcRecord[] records = connection.ReadRecords
                (
                    database,
                    mfnList
                );

            if (records.Length == 0)
            {
                return;
            }

            MarcRecord[] deletedRecords = records
                .Where(record => record.Deleted)
                .ToArray();

            foreach (MarcRecord record in deletedRecords)
            {
                record.Deleted = false;
                connection.WriteRecord(record);
            }
        }

        // ========================================================

        /// <summary>
        /// Unlock record through E command.
        /// </summary>
        public static bool UnlockRecordAlternative
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string databaseName,
                int mfn
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));

            ServerResponse response = connection.ExecuteArbitraryCommand
                (
                    "E",
                    databaseName,
                    mfn
                );
            bool result = response.GetReturnCode() >= 0;

            return result;
        }

        // ========================================================

        /// <summary>
        /// Write record in raw representation.
        /// </summary>
        [NotNull]
        public static string WriteRawRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string record,
                bool lockFlag,
                bool actualize
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNullNorEmpty(record, nameof(record));

            throw new NotImplementedException();

            //UniversalCommand command = connection.CommandFactory.CreateUniversalCommand
            //    (
            //        CommandCode.UpdateRecord,
            //        database,
            //        lockFlag,
            //        actualize,
            //        new TextWithEncoding
            //            (
            //                record,
            //                IrbisEncoding.Utf8
            //            )
            //    );
            //ServerResponse response = connection.ExecuteCommand(command);

            //string result = response.RemainingUtfText();

            //return result;
        }

        // ========================================================

        /// <summary>
        /// Write record in raw representation.
        /// </summary>
        [NotNull]
        public static string[] WriteRawRecords
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] string database,
                [NotNull] string[] records,
                bool lockFlag,
                bool actualize
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(records, nameof(records));

            throw new NotImplementedException();

            //if (records.Length == 0)
            //{
            //    return StringUtility.EmptyArray;
            //}

            //List<object> arguments = new List<object>
            //    (
            //        records.Length + 2
            //    )
            //{
            //    lockFlag,
            //    actualize
            //};
            //foreach (string record in records)
            //{
            //    string reference = database
            //        + IrbisText.IrbisDelimiter
            //        + record;
            //    arguments.Add
            //        (
            //            new TextWithEncoding
            //                (
            //                    reference,
            //                    IrbisEncoding.Utf8
            //                )
            //        );
            //}

            //UniversalCommand command = connection.CommandFactory.CreateUniversalCommand
            //    (
            //        CommandCode.SaveRecordGroup,
            //        arguments.ToArray()
            //    );

            //ServerResponse response = connection.ExecuteCommand(command);

            //List<string> result = response.RemainingUtfStrings();

            //return result.ToArray();
        }

        // ========================================================

        /// <summary>
        /// Create or update existing record in the database.
        /// </summary>
        [NotNull]
        public static MarcRecord WriteRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] MarcRecord record
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(record, nameof(record));

            return connection.WriteRecord
                (
                    record,
                    false,
                    true
                );
        }

        /// <summary>
        /// Create or update existing record in the database.
        /// </summary>
        [NotNull]
        public static MarcRecord WriteRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] MarcRecord record,
                bool lockFlag,
                bool actualize
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(record, nameof(record));

            return connection.WriteRecord
                (
                    record,
                    lockFlag,
                    actualize,
                    false
                );
        }

        /// <summary>
        /// Create or update existing record in the database.
        /// </summary>
        [NotNull]
        public static MarcRecord WriteRecord
            (
                [NotNull] this IIrbisConnection connection,
                [NotNull] MarcRecord record,
                bool dontParseResponse
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(record, nameof(record));

            return connection.WriteRecord
                (
                    record,
                    false,
                    true,
                    dontParseResponse
                );
        }

        #endregion
    }
}
