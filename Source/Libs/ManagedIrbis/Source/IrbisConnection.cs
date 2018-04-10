// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisConnection.cs -- client for IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;

using AM;
using AM.Collections;
using AM.IO;
using AM.Logging;
using AM.Runtime;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;
using ManagedIrbis.Gbl;
using ManagedIrbis.Infrastructure.ClientCommands;
using ManagedIrbis.Infrastructure.Sockets;
using ManagedIrbis.Properties;
using ManagedIrbis.Search;

#endregion

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable VirtualMemberCallInConstructor

namespace ManagedIrbis
{
    /// <summary>
    /// Client for IRBIS-server.
    /// </summary>
    [PublicAPI]
    public class IrbisConnection
        : IIrbisConnection
    {
        #region Constants

        /// <summary>
        /// Таймаут получения ответа от сервера по умолчанию.
        /// </summary>
        public const int DefaultTimeout = 30000;

        #endregion

        #region Events

        /// <summary>
        /// Вызывается перед уничтожением объекта.
        /// </summary>
        public event EventHandler Disposing;

        #endregion

        #region Properties

        // TODO Implement properly

        /// <summary>
        /// Версия клиента.
        /// </summary>
        public static Version ClientVersion = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version;

        /// <summary>
        /// Признак занятости клиента.
        /// </summary>
        [NotNull]
        public BusyState Busy { get; private set; }

        /// <summary>
        /// Адрес сервера.
        /// </summary>
        /// <value>Адрес сервера в цифровом виде.</value>
        [NotNull]
        public string Host
        {
            get => _host;
            set
            {
                Sure.NotNullNorEmpty(value, nameof(value));

                ThrowIfConnected();
                _host = value;
            }
        }

        /// <summary>
        /// Порт сервера.
        /// </summary>
        /// <value>Порт сервера (по умолчанию 6666).</value>
        public int Port
        {
            get => _port;
            set
            {
                Sure.Positive(value, nameof(value));

                ThrowIfConnected();
                _port = value;
            }
        }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        /// <value>Имя пользователя.</value>
        [NotNull]
        public string Username
        {
            get => _username;
            set
            {
                Sure.NotNullNorEmpty(value, nameof(value));

                ThrowIfConnected();
                _username = value;
            }
        }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        /// <value>Пароль пользователя.</value>
        [NotNull]
        public string Password
        {
            get => _password;
            set
            {
                Sure.NotNullNorEmpty(value, nameof(value));

                ThrowIfConnected();
                _password = value;
            }
        }

        /// <summary>
        /// Имя базы данных.
        /// </summary>
        /// <value>Служебное имя базы данных (например, "IBIS").
        /// </value>
        [NotNull]
        public string Database
        {
            get => _database;
            set
            {
                Sure.NotNullNorEmpty(value, nameof(value));

                _database = value;
            }
        }

        /// <summary>
        /// Тип АРМ.
        /// </summary>
        /// <value>По умолчанию
        /// <see cref="IrbisWorkstation.Cataloger"/>.
        /// </value>
        public IrbisWorkstation Workstation
        {
            get => _workstation;
            set
            {
                ThrowIfConnected();
                _workstation = value;
            }
        }

        /// <summary>
        /// Идентификатор клиента.
        /// </summary>
        public int ClientID => _clientID;

        /// <summary>
        /// Номер команды.
        /// </summary>
        public int QueryID => _queryID;

        /// <summary>
        /// Executive engine.
        /// </summary>
        [NotNull]
        public ExecutionEngine Executive { get; private set; }

        /// <summary>
        /// Remote INI-file for the client.
        /// </summary>
        [CanBeNull]
        public IniFile IniFile => _iniFile;

        /// <summary>
        /// Server version.
        /// </summary>
        [CanBeNull]
        public IrbisVersion ServerVersion { get; internal set; }

        /// <summary>
        /// Статус подключения к серверу.
        /// </summary>
        /// <value>Устанавливается в true при успешном выполнении
        /// <see cref="Connect"/>, сбрасывается при выполнении
        /// <see cref="Dispose"/>.
        /// </value>
        public bool Connected => _connected;

        /// <summary>
        /// Флаг отключения.
        /// </summary>
        public bool Disposed => _disposed;

        /// <summary>
        /// Таймаут получения ответа от сервера в миллисекундах
        /// (для продвинутых функций).
        /// </summary>
        [DefaultValue(DefaultTimeout)]
        public int Timeout { get; set; }

        /// <summary>
        /// Признак: команда прервана.
        /// </summary>
        [DefaultValue(false)]
        public bool Interrupted { get; set; }

        /// <summary>
        /// Socket.
        /// </summary>
        [NotNull]
        public ClientSocket Socket { get; private set; }

        /// <summary>
        /// Extension point.
        /// </summary>
        [NotNull]
        public IServiceProvider Services { get; }

        /// <summary>
        /// Arbitrary user data.
        /// </summary>
        [CanBeNull]
        public object UserData { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisConnection ()
        {
            Log.Trace(nameof(IrbisConnection) + "::Constructor");

            Busy = new BusyState();

            Services = new ServiceContainer();
            Host = ConnectionSettings.DefaultHost;
            Port = ConnectionSettings.DefaultPort;
            Database = ConnectionSettings.DefaultDatabase;
            Username = "111";
            Password = "111";
            Workstation = ConnectionSettings.DefaultWorkstation;

            Executive = new ExecutionEngine(this);
            Socket = new SimpleClientSocket();
        }

        /// <summary>
        /// Конструктор с подключением.
        /// </summary>
        public IrbisConnection
            (
                [NotNull] string connectionString
            )
            : this()
        {
            Sure.NotNullNorEmpty(connectionString, nameof(connectionString));

            ParseConnectionString(connectionString);
            Connect();
        }

        #endregion

        #region Private members

        internal bool _connected;
        internal bool _disposed;
        private int _clientID;
        private int _queryID;

        private static Random _random = new Random();

        private readonly Stack<string> _databaseStack = new Stack<string>();

        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private string _database;
        private IrbisWorkstation _workstation;

        private IniFile _iniFile;

        internal void ThrowIfConnected()
        {
            if (Connected)
            {
                throw new IrbisException(Resources.IrbisConnection_AlreadyConnected);
            }
        }

        internal int GenerateClientId()
        {
            _clientID = _random.Next(1000000, 9999999);

            return _clientID;
        }

        internal int IncrementCommandNumber()
        {
            return ++_queryID;
        }

        internal void ResetCommandNumber()
        {
            _queryID = 0;
        }

        #endregion

        // =========================================================

        #region Public methods

        /// <inheritdoc cref="IIrbisConnection.ActualizeRecord" />
        public virtual void ActualizeRecord
            (
                string database,
                int mfn
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NonNegative(mfn, nameof(mfn));

            ActualizeRecordCommand command = new ActualizeRecordCommand
            {
                Database = database,
                Mfn = mfn
            };

            ExecuteCommand(command);
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.Clone()" />
        public virtual IrbisConnection Clone()
        {
            IrbisConnection result = Clone(Connected);

            return result;
        }

        /// <inheritdoc cref="IIrbisConnection.Clone(bool)" />
        public virtual IrbisConnection Clone
            (
                bool connect
            )
        {
            // TODO clone socket?

            IrbisConnection result = new IrbisConnection
            {
                Host = Host,
                Port = Port,
                Username = Username,
                Password = Password,
                Database = Database,
                Workstation = Workstation,
                Timeout = Timeout,
                // Socket = Socket.Clone ()
            };

            if (connect)
            {
                result.Connect();
            }

            return result;
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.Connect" />
        public virtual IniFile Connect()
        {
            // TODO use Executive

            if (!_connected)
            {
                Log.Trace(nameof(IrbisConnection) + "::" + nameof(Connect));

                ConnectCommand command = new ConnectCommand();
                ClientContext context = new ClientContext(this);
                command.Execute(context);
                _connected = true;

                string iniText = command.Configuration
                    .ThrowIfNull(nameof(command.Configuration));
                IniFile result = new IniFile();
                StringReader reader = new StringReader(iniText);
                result.Read(reader);
                _iniFile = result;

                if (!string.IsNullOrEmpty(command.ServerVersion))
                {
                    ServerVersion = new IrbisVersion
                    {
                        Version = command.ServerVersion
                    };
                }

                return result;
            }

            return _iniFile;
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.CorrectVirtualRecord(string,MarcRecord,GblStatement[])"/>
        public virtual MarcRecord CorrectVirtualRecord
            (
                string database,
                MarcRecord record,
                GblStatement[] statements
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(record, nameof(record));
            Sure.NotNull(statements, nameof(statements));

            GblVirtualCommand command = new GblVirtualCommand
            {
                Database = database,
                Record = record,
                Statements = statements
            };

            ExecuteCommand(command);

            return command.Result
                .ThrowIfNull(nameof(command.Result));
        }

        /// <inheritdoc cref="IIrbisConnection.CorrectVirtualRecord(string,MarcRecord,string)"/>
        public virtual MarcRecord CorrectVirtualRecord
            (
                string database,
                MarcRecord record,
                string filename
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(record, nameof(record));
            Sure.NotNullNorEmpty(filename, nameof(filename));

            GblVirtualCommand command = new GblVirtualCommand
            {
                Database = database,
                Record = record,
                FileName = filename
            };

            ExecuteCommand(command);

            return command.Result
                .ThrowIfNull(nameof(command.Result));
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.CreateDatabase" />
        public virtual void CreateDatabase
            (
                string databaseName,
                string description,
                bool readerAccess,
                string template
            )
        {
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));
            Sure.NotNullNorEmpty(description, nameof(description));

            CreateDatabaseCommand command = new CreateDatabaseCommand
            {
                Database = databaseName,
                Description = description,
                ReaderAccess = readerAccess,
                Template = template
            };

            ExecuteCommand(command);
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.CreateDictionary" />
        public virtual void CreateDictionary
            (
                string database
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));

            CreateDictionaryCommand command = new CreateDictionaryCommand
            {
                Database = database,
                RelaxResponse = true
            };

            ExecuteCommand(command);
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.DeleteDatabase" />
        public virtual void DeleteDatabase
            (
                string database
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));

            DeleteDatabaseCommand command = new DeleteDatabaseCommand
            {
                Database = database
            };

            ExecuteCommand(command);
        }

        // =========================================================

        #region ExecuteCommand

        /// <inheritdoc cref="IIrbisConnection.ExecuteCommand(ClientCommand)" />
        public virtual ServerResponse ExecuteCommand
            (
                ClientCommand command
            )
        {
            Sure.NotNull(command, nameof(command));

            Log.Trace(nameof(IrbisConnection) + "::" + nameof(ExecuteCommand));

            //RawClientRequest = null;
            //RawServerResponse = null;

            ClientContext context = new ClientContext(this)
            {
                Command = command
            };
            ServerResponse result = Executive.ExecuteCommand(context);

            //RawClientRequest = null;
            //RawServerResponse = null;

            return result;
        }

        /// <inheritdoc cref="IIrbisConnection.ExecuteCommand(string,object[])" />
        public virtual ServerResponse ExecuteCommand
            (
                string commandCode,
                params object[] arguments
            )
        {
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            UniversalCommand command = new UniversalCommand(commandCode, arguments);

            return ExecuteCommand(command);
        }

        #endregion

        // =========================================================

        #region FormatRecord

        /// <inheritdoc cref="IIrbisConnection.FormatRecord(string,int)" />
        public virtual string FormatRecord
            (
                string format,
                int mfn
            )
        {
            Sure.NotNull(format, nameof(format));
            Sure.Positive(mfn, nameof(mfn));

            FormatCommand command = new FormatCommand {FormatSpecification = format};
            command.MfnList.Add(mfn);

            ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty(nameof(command.FormatResult))
                [0];

            return result;
        }

        /// <inheritdoc cref="IIrbisConnection.FormatRecord(string,MarcRecord)" />
        public string FormatRecord
            (
                string format,
                MarcRecord record
            )
        {
            Sure.NotNull(format, nameof(format));
            Sure.NotNull(record, nameof(record));

            FormatCommand command = new FormatCommand
            {
                FormatSpecification = format,
                VirtualRecord = record
            };

            ExecuteCommand(command);

            string result = command.FormatResult
                .ThrowIfNullOrEmpty(nameof(command.FormatResult))
                [0];

            return result;
        }

        /// <inheritdoc cref="IIrbisConnection.FormatRecords" />
        public virtual string[] FormatRecords
            (
                string database,
                string format,
                IEnumerable<int> mfnList
            )
        {
            Sure.NotNull(mfnList, nameof(mfnList));
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(format, nameof(format));

            FormatCommand command = new FormatCommand
            {
                Database = database,
                FormatSpecification = format
            };
            command.MfnList.AddRange(mfnList);

            if (command.MfnList.Count == 0)
            {
                return StringUtility.EmptyArray;
            }

            ExecuteCommand(command);

            string[] result = command.FormatResult
                .ThrowIfNull(nameof(command.FormatResult));

            return result;
        }

        #endregion

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.GetDatabaseInfo" />
        public virtual DatabaseInfo GetDatabaseInfo
            (
                string databaseName
            )
        {
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));

            DatabaseInfoCommand command = new DatabaseInfoCommand
            {
                Database = databaseName
            };

            ExecuteCommand(command);
            DatabaseInfo result = command.Result.ThrowIfNull(nameof(command.Result));

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.GetDatabaseStat" />
        public virtual string GetDatabaseStat
            (
                StatDefinition definition
            )
        {
            Sure.NotNull(definition, nameof(definition));

            DatabaseStatCommand command = new DatabaseStatCommand
            {
                Definition = definition
            };

            ExecuteCommand(command);
            string result = command.Result.ThrowIfNull(nameof(command.Result));

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.GetMaxMfn()" />
        public virtual int GetMaxMfn()
        {
            return GetMaxMfn(Database);
        }

        /// <inheritdoc cref="IIrbisConnection.GetMaxMfn(string)" />
        public virtual int GetMaxMfn
            (
                string database
            )
        {
            database = database ?? Database;

            MaxMfnCommand command = new MaxMfnCommand
            {
                Database = database
            };

            ServerResponse response = ExecuteCommand(command);
            int result = response.ReturnCode;

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.GetServerStat" />
        public virtual ServerStat GetServerStat()
        {
            ServerStatCommand command = new ServerStatCommand();
            ExecuteCommand(command);

            ServerStat result = command.Result
                .ThrowIfNull(nameof(command.Result));

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.GetServerVersion" />
        public virtual IrbisVersion GetServerVersion()
        {
            ServerVersionCommand command = new ServerVersionCommand();
            ExecuteCommand(command);

            IrbisVersion result = command.Result.ThrowIfNull(nameof(command.Result));

            ServerVersion = result;

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.GlobalCorrection" />
        public virtual GblResult GlobalCorrection
            (
                GblSettings settings
            )
        {
            Sure.NotNull(settings, nameof(settings));

            if (string.IsNullOrEmpty(settings.Database))
            {
                settings.Database = Database;
            }

            GblCommand command = new GblCommand(this, settings);

            ExecuteCommand(command);

            return command.Result.ThrowIfNull(nameof(command.Result));
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.ListFiles(FileSpecification)" />
        public virtual string[] ListFiles
            (
                FileSpecification specification
            )
        {
            Sure.NotNull(specification, nameof(specification));

            specification.Verify(true);

            ListFilesCommand command = new ListFilesCommand();
            command.Specifications.Add(specification);

            ExecuteCommand(command);

            string[] result = command.Files
                .ThrowIfNull("command.Files");

            return result;
        }

        /// <inheritdoc cref="IIrbisConnection.ListFiles(FileSpecification[])" />
        public virtual string[] ListFiles
            (
                FileSpecification[] specifications
            )
        {
            Sure.NotNull(specifications, nameof(specifications));

            ListFilesCommand command = new ListFilesCommand();
            foreach (FileSpecification specification in specifications)
            {
                specification.Verify(true);
                command.Specifications.Add(specification);
            }

            ExecuteCommand(command);

            string[] result = command.Files
                .ThrowIfNull("command.Files");

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.ListProcesses" />
        public virtual IrbisProcessInfo[] ListProcesses()
        {
            ListProcessesCommand command = new ListProcessesCommand();
            ExecuteCommand(command);

            IrbisProcessInfo[] result = command.Result
                .ThrowIfNullOrEmpty(nameof(command.Result));

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.ListUsers" />
        public virtual UserInfo[] ListUsers()
        {
            ListUsersCommand command = new ListUsersCommand();
            ExecuteCommand(command);

            UserInfo[] result = command.Result
                .ThrowIfNull(nameof(command.Result));

            return result;
        }

        // =========================================================

        /// <inheritdoc cref="IIrbisConnection.NoOp" />
        public virtual void NoOp()
        {
            NopCommand command = new NopCommand();

            ExecuteCommand(command);
        }

        /// <inheritdoc cref="IIrbisConnection.ParseConnectionString" />
        public virtual void ParseConnectionString
            (
                string connectionString
            )
        {
            Sure.NotNull(connectionString, nameof(connectionString));

            ConnectionSettings settings = new ConnectionSettings();
            settings.ParseConnectionString(connectionString);
            settings.ApplyToConnection(this);
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.PopDatabase" />
        public virtual string PopDatabase()
        {
            string result = Database;

            if (_databaseStack.Count != 0)
            {
                Database = _databaseStack.Pop();
            }

            return result;
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.PrintTable" />
        public virtual string PrintTable
            (
                TableDefinition tableDefinition
            )
        {
            Sure.NotNull(tableDefinition, nameof(tableDefinition));

            PrintTableCommand command = new PrintTableCommand
            {
                Definition = tableDefinition
            };

            ExecuteCommand(command);

            return command.Result ?? string.Empty;
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.PushDatabase" />
        public virtual string PushDatabase
            (
                string newDatabase
            )
        {
            Sure.NotNullNorEmpty(newDatabase, nameof(newDatabase));

            string result = Database;
            _databaseStack.Push(Database);
            Database = newDatabase;

            return result;
        }

        // ========================================================

        /// <summary>
        /// Read binary file from server file system.
        /// </summary>
        [CanBeNull]
        public virtual byte[] ReadBinaryFile
            (
                FileSpecification file
            )
        {
            Sure.NotNull(file, nameof(file));

            ReadBinaryFileCommand command = new ReadBinaryFileCommand
            {
                File = file
            };

            ExecuteCommand(command);

            return command.Content;
        }

        // ========================================================

        /// <summary>
        /// Read term postings.
        /// </summary>
        [NotNull]
        public virtual TermPosting[] ReadPostings
            (
                PostingParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            ReadPostingsCommand command = new ReadPostingsCommand();
            command.ApplyParameters(parameters);

            ExecuteCommand(command);

            return command.Postings
                .ThrowIfNull(nameof(command.Postings));
        }

        // ========================================================

        #region ReadRecord

        /// <summary>
        /// Чтение, блокирование и расформатирование записи.
        /// </summary>
        [NotNull]
        public virtual MarcRecord ReadRecord
            (
                string database,
                int mfn,
                bool lockFlag,
                string format
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.Positive(mfn, nameof(mfn));

            ReadRecordCommand command = new ReadRecordCommand
            {
                Mfn = mfn,
                Database = database,
                Lock = lockFlag,
                Format = format
            };

            ExecuteCommand(command);

            return command.Record.ThrowIfNull(Resources.IrbisConnection_NoRecordRetrieved);
        }

        /// <summary>
        /// Чтение указанной версии и расформатирование записи.
        /// </summary>
        /// <remarks><c>null</c>означает, что затребованной
        /// версии записи нет.</remarks>
        [CanBeNull]
        public virtual MarcRecord ReadRecord
            (
                string database,
                int mfn,
                int versionNumber,
                string format
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.Positive(mfn, nameof(mfn));

            ReadRecordCommand command = new ReadRecordCommand
            {
                Mfn = mfn,
                Database = database,
                VersionNumber = versionNumber,
                Format = format
            };

            ExecuteCommand(command);

            return command.Record;
        }

        #endregion

        // ========================================================

        /// <summary>
        /// Read search terms from index.
        /// </summary>
        [NotNull]
        public virtual TermInfo[] ReadTerms
            (
                TermParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            ReadTermsCommand command = new ReadTermsCommand();
            command.ApplyParameters(parameters);

            ExecuteCommand(command);

            return command.Terms.ThrowIfNull(nameof(command.Terms));
        }

        // ========================================================

        /// <summary>
        /// Read text file from the server.
        /// </summary>
        [CanBeNull]
        public virtual string ReadTextFile
            (
                FileSpecification fileSpecification
            )
        {
            Sure.NotNull(fileSpecification, nameof(fileSpecification));

            ReadFileCommand command = new ReadFileCommand();
            command.Files.Add(fileSpecification);

            ExecuteCommand(command);

            string result = command.Result
                .ThrowIfNullOrEmpty(nameof(command.Result))
                [0];

            return result;
        }

        /// <summary>
        /// Чтение текстовых файлов с сервера.
        /// </summary>
        [NotNull]
        public virtual string[] ReadTextFiles
            (
                FileSpecification[] files
            )
        {
            Sure.NotNull(files, nameof(files));

            if (files.Length == 0)
            {
                return StringUtility.EmptyArray;
            }

            ReadFileCommand command = new ReadFileCommand();
            command.Files.AddRange(files);

            ExecuteCommand(command);

            string[] result = command.Result
                .ThrowIfNullOrEmpty(nameof(command.Result));

            return result;
        }

        // =========================================================

        /// <summary>
        /// Reconnect to the server.
        /// </summary>
        public virtual void Reconnect()
        {
            Log.Trace(nameof(IrbisConnection) + "::" + nameof(Reconnect));

            if (_connected)
            {
                DisconnectCommand command = new DisconnectCommand();

                ExecuteCommand(command);
                _connected = false;
            }

            if (!ReferenceEquals(_iniFile, null))
            {
                _iniFile.Dispose();
                _iniFile = null;
            }

            Connect();
        }

        // =========================================================

        /// <summary>
        /// Reload dictionary index for specified database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public virtual void ReloadDictionary
            (
                string databaseName
            )
        {
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));

            ReloadDictionaryCommand command = new ReloadDictionaryCommand
            {
                Database = databaseName
            };

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Reload master file for specified database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public virtual void ReloadMasterFile
            (
                string databaseName
            )
        {
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));

            ReloadMasterFileCommand command = new ReloadMasterFileCommand
            {
                Database = databaseName
            };

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Restart server.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public virtual void RestartServer()
        {
            RestartServerCommand command = new RestartServerCommand();

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Restore previously suspended connection.
        /// </summary>
        [NotNull]
        public static IrbisConnection Restore
            (
                [NotNull] string state
            )
        {
            Sure.NotNullNorEmpty(state, nameof(state));

            ConnectionSettings settings = ConnectionSettings.Decrypt(state);

            if (ReferenceEquals(settings, null))
            {
                throw new IrbisException(Resources.IrbisConnection_DecryptedStateIsNull);
            }

            IrbisConnection result = new IrbisConnection();
            settings.ApplyToConnection(result);

            return result;
        }

        // =========================================================

        /// <summary>
        /// Поиск записей.
        /// </summary>
        [NotNull]
        public virtual int[] Search
            (
                string expression
            )
        {
            Sure.NotNull(expression, nameof(expression));

            SearchCommand command = new SearchCommand
            {
                SearchExpression = expression
            };

            ExecuteCommand(command);

            int[] result = FoundItem.ConvertToMfn
                (
                    command.Found.ThrowIfNull(nameof(command.Found))
                );

            return result;
        }

        // =========================================================

        /// <summary>
        /// Sequential search.
        /// </summary>
        [NotNull]
        public virtual int[] SequentialSearch
            (
                SearchParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            SearchCommand command = new SearchCommand();
            command.ApplyParameters(parameters);

            ExecuteCommand(command);

            int[] result = FoundItem.ConvertToMfn
                (
                    command.Found.ThrowIfNull(nameof(command.Found))
                );

            return result;
        }

        // =========================================================

        /// <summary>
        /// Set execution engine.
        /// </summary>
        [NotNull]
        public virtual ExecutionEngine SetEngine
            (
                ExecutionEngine engine
            )
        {
            Sure.NotNull(engine, nameof(engine));

            ExecutionEngine previous = Executive;
            Executive = engine;

            return previous;
        }

        /// <summary>
        /// Set new <see cref="Executive"/>.
        /// </summary>
        /// <returns>Previous <see cref="Executive"/>.
        /// </returns>
        [NotNull]
        public virtual ExecutionEngine SetEngine
            (
                string typeName
            )
        {
            Sure.NotNull(typeName, nameof(typeName));

            Type type = Type.GetType(typeName, true);
            ExecutionEngine newEngine = (ExecutionEngine)Activator.CreateInstance
                (
                    type,
                    this
                );
            ExecutionEngine previous = SetEngine(newEngine);

            return previous;
        }

        // =========================================================

        /// <summary>
        /// Set logging socket, gather debug info to specified path.
        /// </summary>
        public virtual void SetNetworkLogging
            (
                string loggingPath
            )
        {
            Sure.NotNullNorEmpty(loggingPath, nameof(loggingPath));

            ClientSocket oldSocket = Socket;
            if (oldSocket is LoggingClientSocket)
            {
                return;
            }

            LoggingClientSocket newSocket = new LoggingClientSocket
                (
                    Socket,
                    loggingPath
                );

            SetSocket(newSocket);
        }

        // =========================================================

        /// <summary>
        ///
        /// </summary>
        public virtual void SetRetry
            (
                int retryCount,
                Func<Exception, bool> resolver
            )
        {
            RetryClientSocket oldSocket = ClientSocketUtility.FindSocket<RetryClientSocket>(this);

            if (retryCount <= 0)
            {
                if (!ReferenceEquals(oldSocket, null))
                {
                    SetSocket
                        (
                            oldSocket.InnerSocket
                            .ThrowIfNull(nameof(oldSocket.InnerSocket))
                        );
                }
            }
            else
            {
                RetryClientSocket newSocket = new RetryClientSocket
                    (
                        Socket,
                        new RetryManager(retryCount, resolver)
                    );

                if (ReferenceEquals(oldSocket, null))
                {
                    SetSocket(newSocket);
                }
            }
        }

        // =========================================================

        /// <summary>
        /// Set
        /// <see cref="T:ManagedIrbis.Network.Sockets.AbstractClientSocket"/>.
        /// </summary>
        public virtual void SetSocket
            (
                ClientSocket socket
            )
        {
            Sure.NotNull(socket, nameof(socket));

            if (Connected)
            {
                throw new IrbisException(Resources.IrbisConnection_CantSetSocketWhileConnected);
            }

            Socket = socket;
        }

        // =========================================================

        /// <summary>
        /// Temporary "shutdown" the connection for some reason.
        /// </summary>
        [NotNull]
        public virtual string Suspend()
        {
            ConnectionSettings settings = ConnectionSettings.FromConnection(this);
            string result = settings.Encrypt();

            _connected = false;

            return result;
        }

        // =========================================================

        /// <summary>
        /// Опустошение базы данных.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public virtual void TruncateDatabase
            (
                string databaseName
            )
        {
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));

            TruncateDatabaseCommand command = new TruncateDatabaseCommand
            {
                Database = databaseName
            };

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Unlock the specified database.
        /// </summary>
        /// <remarks>For Administrator only.</remarks>
        public virtual void UnlockDatabase
            (
                string databaseName
            )
        {
            Sure.NotNullNorEmpty(databaseName, nameof(databaseName));

            UnlockDatabaseCommand command = new UnlockDatabaseCommand
            {
                Database = databaseName
            };

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Unlock specified records.
        /// </summary>
        public virtual void UnlockRecords
            (
                string database,
                params int[] mfnList
            )
        {
            // TODO: write UnlockRecordsTest

            Sure.NotNullNorEmpty(database, nameof(database));

            if (mfnList.Length == 0)
            {
                return;
            }

            UnlockRecordsCommand command = new UnlockRecordsCommand
            {
                Database = database
            };
            command.Records.AddRange(mfnList);

            ExecuteCommand(command);
        }

        // =========================================================

        /// <summary>
        /// Update server INI-file for current client.
        /// </summary>
        public virtual void UpdateIniFile
            (
                string[] lines
            )
        {
            if (lines.IsNullOrEmpty())
            {
                return;
            }

            UpdateIniFileCommand command = new UpdateIniFileCommand
            {
                Lines = lines
            };

            ExecuteCommand(command);
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.UpdateUserList" />
        public virtual void UpdateUserList
            (
                UserInfo[] userList
            )
        {
            Sure.NotNull(userList, nameof(userList));

            UpdateUserListCommand command = new UpdateUserListCommand
            {
                UserList = userList
            };

            ExecuteCommand(command);
        }

        // ========================================================

        #region WriteRecord

        /// <inheritdoc cref="IIrbisConnection.WriteRecord(ManagedIrbis.MarcRecord,bool,bool,bool)" />
        public virtual MarcRecord WriteRecord
            (
                MarcRecord record,
                bool lockFlag,
                bool actualize,
                bool dontParseResponse
            )
        {
            Sure.NotNull(record, nameof(record));

            WriteRecordCommand command = new WriteRecordCommand()
            {
                Record = record,
                Actualize = actualize,
                Lock = lockFlag,
                DontParseResponse = dontParseResponse
            };

            ExecuteCommand(command);

            MarcRecord result = command.Record;

            if (ReferenceEquals(result, null))
            {
                throw new IrbisException(Resources.IrbisConnection_ResultRecordIsNull);
            }

            return result;
        }

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.WriteRecords" />
        public virtual MarcRecord[] WriteRecords
            (
                MarcRecord[] records,
                bool lockFlag,
                bool actualize
            )
        {
            Sure.NotNull(records, nameof(records));

            if (records.Length == 0)
            {
                return records;
            }

            WriteRecordsCommand command = new WriteRecordsCommand()
            {
                Actualize = actualize,
                Lock = lockFlag
            };
            foreach (MarcRecord record in records)
            {
                RecordReference reference = new RecordReference(record)
                {
                    HostName = Host,
                    Database = Database
                };
                command.References.Add(reference);
            }

            ExecuteCommand(command);

            return records;
        }

        #endregion

        // ========================================================

        /// <inheritdoc cref="IIrbisConnection.WriteTextFile" />
        public virtual void WriteTextFile
            (
                FileSpecification file
            )
        {
            Sure.NotNull(file, nameof(file));

            WriteFileCommand command = new WriteFileCommand();
            command.Files.Add(file);

            ExecuteCommand(command);
        }

        /// <inheritdoc cref="IIrbisConnection.WriteTextFiles" />
        public virtual void WriteTextFiles
            (
                params FileSpecification[] files
            )
        {
            WriteFileCommand command = new WriteFileCommand();
            command.Files.AddRange(files);

            ExecuteCommand(command);
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public virtual void Dispose()
        {
            Log.Trace(nameof(IrbisConnection) + "::" + nameof(Dispose));

            Disposing.Raise(this);

            if (_connected)
            {
                DisconnectCommand command = new DisconnectCommand();

                ExecuteCommand(command);
            }

            if (!ReferenceEquals(_iniFile, null))
            {
                _iniFile.Dispose();
                _iniFile = null;
            }

            _disposed = true;
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public virtual void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            Sure.NotNull(reader, nameof(reader));

            Host = reader.ReadString();
            Port = reader.ReadInt32();
            Username = reader.ReadString();
            Password = reader.ReadString();
            Database = reader.ReadString();
            Workstation = (IrbisWorkstation)reader.ReadPackedInt32();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public virtual void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer.Write(Host);
            writer.Write(Port);
            writer.Write(Username);
            writer.Write(Password);
            writer.Write(Database);
            writer.Write((int)Workstation);
        }

        #endregion
    }
}
