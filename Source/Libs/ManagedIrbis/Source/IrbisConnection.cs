// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisConnection.cs -- connection to IRBIS-server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.IO;
using AM.Logging;
using AM.Threading.Tasks;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Sockets;
using ManagedIrbis.Properties;
using ManagedIrbis.Search;

#endregion

// ReSharper disable CommentTypo

namespace ManagedIrbis
{
    /// <summary>
    /// Подключение к серверу ИРБИС64.
    /// </summary>
    [PublicAPI]
    public sealed class IrbisConnection
        : IDisposable
    {
        #region Events

        /// <summary>
        /// Fired when <see cref="Busy"/> changed.
        /// </summary>
        public event EventHandler? BusyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        ///
        /// </summary>
        public int Port { get; set; } = 6666;

        /// <summary>
        ///
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public string Database { get; set; } = "IBIS";

        /// <summary>
        ///
        /// </summary>
        public string Workstation { get; set; } = "C";

        /// <summary>
        ///
        /// </summary>
        public int ClientId { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public int QueryId { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string? ServerVersion { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public IniFile? IniFile { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public int Interval { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Socket.
        /// </summary>
        public ClientSocket Socket { get; private set; }

        /// <summary>
        /// Busy?
        /// </summary>
        public bool Busy { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public CancellationToken Cancellation { get; }

        /// <summary>
        /// Last error code.
        /// </summary>
        public int LastError { get; internal set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisConnection()
        {
            Socket = new PlainTcp4Socket(this);
            _cancellation = new CancellationTokenSource();
            Cancellation = _cancellation.Token;
        }

        #endregion

        #region Private members

        private static readonly int[] _goodCodesForReadRecord = { -201, -600, -602, -603 };
        private static readonly int[] _goodCodesForReadTerms = { -202, -203, -204 };

        private CancellationTokenSource _cancellation;

        private bool _debug = false;

        private void SetBusy(bool busy)
        {
            Busy = busy;
            BusyChanged?.Invoke(this, EventArgs.Empty);
        } // method SetBusy

        private bool CheckConnection()
        {
            if (!Connected)
            {
                LastError = -100500;
            }

            return Connected;
        } // method CheckConnection

        #endregion

        #region Public methods

        /// <summary>
        /// Актуализация всех неактуализированных записей
        /// в указанной базе данных.
        /// </summary>
        /// <param name="database">Имя базы данных.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> ActualizeDatabaseAsync
            (
                string? database = default
            )
        {
            var result = await ActualizeRecordAsync(database, 0);
            return result;
        } // method ActualizeDatabase

        /// <summary>
        /// Актуализация записи с указанным MFN.
        /// </summary>
        /// <param name="database">Имя базы данных.</param>
        /// <param name="mfn">MFN, подлежащий актуализации.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> ActualizeRecordAsync
            (
                string? database,
                int mfn
            )
        {
            database ??= Database;
            var response = await ExecuteAsync("F", database, mfn);
            return !ReferenceEquals(response, null);
        } // method ActualizeRecordAsync

        /// <summary>
        /// Cancel the current operation.
        /// </summary>
        public void CancelOperation()
        {
            _cancellation.Cancel();
        } // method CancelOperation

        /// <summary>
        ///
        /// </summary>
        public async Task<IniFile?> ConnectAsync()
        {
            if (Connected)
            {
                return IniFile;
            }

            AGAIN:
            ClientId = new Random().Next(100000, 999999);
            QueryId = 1;
            var query = new ClientQuery(this, "A");
            query.AddAnsi(Username).NewLine();
            query.AddAnsi(Password);

            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            if (response.GetReturnCode() == -3337)
            {
                goto AGAIN;
            }

            if (response.ReturnCode < 0)
            {
                return null;
            }

            Connected = true;
            ServerVersion = response.ServerVersion;
            Interval = response.ReadInteger();
            // TODO Read INI-file
            IniFile = new IniFile();

            return IniFile;
        } // method ConnectAsync

        /// <summary>
        /// Создание базы данных.
        /// </summary>
        /// <param name="database">Имя создаваемой базы.</param>
        /// <param name="description">Описание в свободной форме.</param>
        /// <param name="readerAccess">Читатель будет иметь доступ?</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> CreateDatabaseAsync
            (
                string database,
                string description,
                bool readerAccess = true
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            var query = new ClientQuery(this, "T");
            query.AddAnsi(database).NewLine();
            query.AddAnsi(description).NewLine();
            query.Add(readerAccess ? 1 : 0).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            if (!response.CheckReturnCode())
            {
                return false;
            }

            return true;
        } // method CreateDatabaseAsync


        /// <summary>
        /// Создание словаря в указанной базе данных.
        /// </summary>
        /// <param name="database">Имя базы данных.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> CreateDictionaryAsync
            (
                string? database = default
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            database ??= Database;
            var query = new ClientQuery(this, "Z");
            query.AddAnsi(database).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            if (!response.CheckReturnCode())
            {
                return false;
            }

            return true;
        } // method CreateDictionaryAsync

        /// <summary>
        /// Удаление указанной базы данных.
        /// </summary>
        /// <param name="database">Имя удаляемой базы данных.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> DeleteDatabaseAsync
            (
                string? database = default
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            database ??= Database;
            var query = new ClientQuery(this, "W");
            query.AddAnsi(database).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            if (!response.CheckReturnCode())
            {
                return false;
            }

            return true;

        } // method DeleteDatabaseAsync

        /// <summary>
        /// Отключение от сервера.
        /// </summary>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> DisconnectAsync()
        {
            if (Connected)
            {
                var query = new ClientQuery(this, "B");
                query.AddAnsi(Username);
                try
                {
                    await ExecuteAsync(query);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }

                Connected = false;
            }

            return true;
        } // method DisconnectAsync

        /// <summary>
        /// Отправка клиентского запроса на сервер
        /// и получение ответа от него.
        /// </summary>
        /// <param name="query">Клиентский запрос.</param>
        /// <returns>Ответ от сервера.</returns>
        public async Task<ServerResponse?> ExecuteAsync
            (
                ClientQuery query
            )
        {
            SetBusy(true);
            try
            {
                if (_cancellation.IsCancellationRequested)
                {
                    _cancellation = new CancellationTokenSource();
                }

                ServerResponse? result;
                try
                {
                    if (_debug)
                    {
                        query.Debug(Console.Out);
                    }

                    result = await Socket.TransactAsync(query);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return null;
                }

                if (ReferenceEquals(result, null))
                {
                    return null;
                }

                if (_debug)
                {
                    result.Debug(Console.Out);
                }

                result.Parse();
                QueryId++;

                return result;
            }
            finally
            {
                SetBusy(false);
            }
        } // method ExecuteAsync

        /// <summary>
        /// Отправка запроса на сервер по упрощённой схеме.
        /// </summary>
        /// <param name="command">Код команды.</param>
        /// <param name="args">Опциональные параметры команды
        /// (в кодировке ANSI).</param>
        /// <returns>Ответ сервера.</returns>
        public async Task<ServerResponse?> ExecuteAsync
            (
                string command,
                params object[] args
            )
        {
            if (!CheckConnection())
            {
                return null;
            }

            var query = new ClientQuery(this, command);
            foreach (var arg in args)
            {
                query.AddAnsi(arg?.ToString()).NewLine();
            }

            var result = await ExecuteAsync(query);

            return result;
        } // method ExecuteAsync

        /// <summary>
        /// Форматирование записи с указанием её MFN.
        /// </summary>
        /// <param name="format">Спецификация формата.</param>
        /// <param name="mfn">MFN записи.</param>
        /// <returns>Результат расформатирования.</returns>
        public async Task<string?> FormatRecordAsync
            (
                string format,
                int mfn
            )
        {
            if (!CheckConnection())
            {
                return null;
            }

            var query = new ClientQuery(this, "G");
            query.AddAnsi(Database).NewLine();
            var prepared = IrbisFormat.PrepareFormat(format);
            query.AddAnsi(prepared).NewLine();
            query.Add(1).NewLine();
            query.Add(mfn).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            response.CheckReturnCode();
            string result = response.ReadRemainingUtfText();
            if (!string.IsNullOrEmpty(result))
            {
                result = result.TrimEnd();
            }

            return result;
        } // method FormatRecordAsync

        /// <summary>
        /// Получение максимального MFN для указанной базы данных.
        /// </summary>
        /// <param name="database">Опциональное имя базы данных
        /// (<c>null</c> означает текущую базу данных).</param>
        /// <returns>Макисмальный MFN.</returns>
        public async Task<int> GetMaxMfnAsync
            (
                string? database = default
            )
        {
            if (!CheckConnection())
            {
                return 0;
            }

            database ??= Database;
            var query = new ClientQuery(this, "O");
            query.AddAnsi(database).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return 0;
            }

            if (!response.CheckReturnCode())
            {
                return 0;
            }

            return response.ReturnCode;
        } // method GetMaxMfnAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<ServerVersion?> GetServerVersionAsync()
        {
            if (!CheckConnection())
            {
                return null;
            }

            var query = new ClientQuery(this, "1");
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            response.CheckReturnCode();
            ServerVersion result = new ServerVersion();
            var lines = response.ReadRemainingAnsiLines();
            result.Parse(lines);

            return result;
        } // method GetServerVersionAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<DatabaseInfo[]> ListDatabasesAsync
            (
                string fileName
            )
        {
            await Task.Delay(0, Cancellation);
            throw new NotImplementedException("ListDatabasesAsync");
        }

        /// <summary>
        ///
        /// </summary>
        public async Task<string[]> ListFilesAsync
            (
                string specification
            )
        {
            if (!CheckConnection() || string.IsNullOrEmpty(specification))
            {
                return Array.Empty<string>();
            }

            var query = new ClientQuery(this, "!");
            query.AddAnsi(specification).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<string>();
            }

            var lines = response.ReadRemainingAnsiLines();
            var result = new LocalList<string>();
            foreach (var line in lines)
            {
                var files = IrbisText.SplitIrbisToLines(line);
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        result.Add(file);
                    }
                }
            }

            return result.ToArray();
        } // method ListFilesAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<ProcessInfo[]> ListProcessesAsync()
        {
            if (!CheckConnection())
            {
                return Array.Empty<ProcessInfo>();
            }

            var query = new ClientQuery(this, "+3");
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<ProcessInfo>();
            }

            response.CheckReturnCode();
            var lines = response.ReadRemainingAnsiLines();
            var result = ProcessInfo.Parse(lines);

            return result;
        } // method ListProcessesAsync

        /// <summary>
        ///
        /// </summary>
        public void ParseConnectionString
            (
                [CanBeNull] string connectionString
            )
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }

            var pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                if (!pair.Contains('='))
                {
                    continue;
                }

                var parts = pair.Split('=', 2);
                var name = parts[0].Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var value = parts[1].Trim();

                switch (name)
                {
                    case "host":
                    case "server":
                    case "address":
                        Host = value;
                        break;

                    case "port":
                        Port = NumericUtility.ParseInt32(value);
                        break;

                    case "user":
                    case "username":
                    case "name":
                    case "login":
                    case "account":
                        Username = value;
                        break;

                    case "password":
                    case "pwd":
                    case "secret":
                        Password = value;
                        break;

                    case "db":
                    case "database":
                    case "base":
                    case "catalog":
                        Database = value;
                        break;

                    case "arm":
                    case "workstation":
                        Workstation = value;
                        break;

                    case "debug":
                        _debug = true;
                        break;

                    default:
                        throw new IrbisException($"Unknown key {name}");
                }
            }
        } // method ParseConnectionString

        /// <summary>
        ///
        /// </summary>
        public async Task<MarcRecord?> ReadRecordAsync
            (
                int mfn
            )
        {
            if (!CheckConnection())
            {
                return null;
            }

            var query = new ClientQuery(this, "C");
            query.AddAnsi(Database).NewLine();
            query.Add(mfn).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            if (!response.CheckReturnCode(_goodCodesForReadRecord))
            {
                return null;
            }

            var result = new MarcRecord
            {
                Database = Database
            };
            var lines = response.ReadRemainingUtfLines();
            result.Decode(lines);

            return result;
        } // method ReadRecordAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<TermInfo[]> ReadAllTermsAsync
            (
                string prefix
            )
        {
            if (!CheckConnection())
            {
                return Array.Empty<TermInfo>();
            }

            prefix = prefix.ToUpperInvariant();
            var result = new LocalList<TermInfo>();
            var startTerm = prefix;
            var flag = true;
            while (flag)
            {
                var terms = await ReadTermsAsync(startTerm, 1024);
                if (terms.Length == 0)
                {
                    break;
                }

                int startIndex = 0;
                if (result.Count != 0)
                {
                    var lastTerm = result[result.Count - 1];
                    var firstTerm = terms[0];
                    if (firstTerm.Text == lastTerm.Text)
                    {
                        startIndex = 1;
                    }
                }

                for (var i = startIndex; i < terms.Length; i++)
                {
                    var term = terms[i];
                    var text = term.Text;
                    if (string.IsNullOrEmpty(text))
                    {
                        break;
                    }

                    if (!text.StartsWith(prefix))
                    {
                        flag = false;
                        break;
                    }
                    else
                    {
                        result.Add(term);
                    }
                }
            }

            return result.ToArray();
        } // method ReadAllTermsAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<TermInfo[]> ReadTermsAsync
            (
                TermParameters parameters
            )
        {
            if (!CheckConnection())
            {
                return Array.Empty<TermInfo>();
            }

            var command = parameters.ReverseOrder ? "P" : "H";
            var database = parameters.Database ?? Database;
            var query = new ClientQuery(this, command);
            query.AddAnsi(database).NewLine();
            query.AddUtf(parameters.StartTerm).NewLine();
            query.Add(parameters.NumberOfTerms).NewLine();
            var prepared = IrbisFormat.PrepareFormat(parameters.Format);
            query.AddAnsi(prepared).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<TermInfo>();
            }
            if (!response.CheckReturnCode(_goodCodesForReadTerms))
            {
                return Array.Empty<TermInfo>();
            }

            var lines = response.EnumRemainingNonNullUtfLines();
            var result = TermInfo.Parse(lines);

            return result;
        } // method ReadTermsAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<TermInfo[]> ReadTermsAsync
            (
                string? startTerm,
                int numberOfTerms = 100
            )
        {
            TermParameters parameters = new TermParameters
            {
                StartTerm = startTerm,
                NumberOfTerms = numberOfTerms
            };
            var result = await ReadTermsAsync(parameters);

            return result;
        } // method ReadTermsAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<string?> ReadTextFileAsync
            (
                string? specification
            )
        {
            if (!CheckConnection())
            {
                return null;
            }

            if (string.IsNullOrEmpty(specification))
            {
                return null;
            }

            var query = new ClientQuery(this, "L");
            query.AddAnsi(specification).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            var result = IrbisText.IrbisToWindows(response.ReadAnsi());

            return result;
        } // method ReadTextFileAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<FoundItem[]> SearchAsync
            (
                SearchParameters parameters
            )
        {
            if (!CheckConnection())
            {
                return Array.Empty<FoundItem>();
            }

            var database = parameters.Database ?? Database;
            var query = new ClientQuery(this, "K");
            query.AddAnsi(database).NewLine();
            query.AddUtf(parameters.SearchExpression).NewLine();
            query.Add(parameters.NumberOfRecords).NewLine();
            query.Add(parameters.FirstRecord).NewLine();
            var prepared = IrbisFormat.PrepareFormat(parameters.FormatSpecification);
            query.AddAnsi(prepared).NewLine();
            query.Add(parameters.MinMfn).NewLine();
            query.Add(parameters.MaxMfn).NewLine();
            query.AddAnsi(parameters.SequentialSpecification).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<FoundItem>();
            }
            if (!response.CheckReturnCode())
            {
                return Array.Empty<FoundItem>();
            }

            var count = response.ReadInteger(); // Число найденных записей
            var lines = response.EnumRemainingUtfLines();
            var result = FoundItem.ParseServerResponse(lines, count);

            return result;
        } // method SearchAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<int[]> SearchAsync
            (
                string? expression
            )
        {
            if (!CheckConnection())
            {
                return Array.Empty<int>();
            }

            if (string.IsNullOrEmpty(expression))
            {
                return Array.Empty<int>();
            }

            var query = new ClientQuery(this, "K");
            query.AddAnsi(Database).NewLine();
            query.AddUtf(expression).NewLine();
            query.Add(0).NewLine();
            query.Add(1).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<int>();
            }
            if (!response.CheckReturnCode())
            {
                return Array.Empty<int>();
            }

            int count = response.ReadInteger(); // Число найденных записей
            var result = new LocalList<int>(Math.Max(count, 2));
            foreach (var line in response.EnumRemainingBinaryLines())
            {
                int mfn = FastNumber.ParseInt32(line, 0, line.Length);
                if (mfn != 0)
                {
                    result.Add(mfn);
                }
            }

            return result.ToArray();
        } // method SearchAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<int> SearchCountAsync
            (
                string? expression
            )
        {
            if (!CheckConnection())
            {
                return 0;
            }

            if (string.IsNullOrEmpty(expression))
            {
                return 0;
            }

            var query = new ClientQuery(this, "K");
            query.AddAnsi(Database).NewLine();
            query.AddUtf(expression).NewLine();
            query.Add(0).NewLine();
            query.Add(1).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return 0;
            }
            if (!response.CheckReturnCode())
            {
                return 0;
            }

            int result = response.ReadInteger(); // Число найденных записей

            return result;
        } // method SearchCountAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<MarcRecord[]> SearchReadAsync
            (
                string? expression,
                int limit = 0
            )
        {
            if (!CheckConnection())
            {
                return Array.Empty<MarcRecord>();
            }

            if (string.IsNullOrEmpty(expression)
                || limit < 0)
            {
                return Array.Empty<MarcRecord>();
            }

            var query = new ClientQuery(this, "K");
            query.AddAnsi(Database).NewLine();
            query.AddUtf(expression).NewLine();
            query.Add(limit).NewLine();
            query.Add(1).NewLine();
            query.AddAnsi(IrbisFormat.All).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<MarcRecord>();
            }
            if (!response.CheckReturnCode())
            {
                return Array.Empty<MarcRecord>();
            }
            int count = response.ReadInteger(); // Число найденных записей
            if (count <= 0)
            {
                return Array.Empty<MarcRecord>();
            }

            var result = new LocalList<MarcRecord>(count);
            foreach (var line in response.EnumRemainingUtfLines())
            {
                var parts = line.Split('#', 2);
                if (parts.Length == 2)
                {
                    var lines = parts[1].Split('\x1F').Skip(1).ToArray();
                    var record = new MarcRecord();
                    record.Decode(lines);
                    record.Database = Database;
                    result.Add(record);
                }
            }

            return result.ToArray();
        } // method SearchReadAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<MarcRecord> SearchReadOneRecordAsync
            (
                string? expression
            )
        {
            var found = await SearchReadAsync(expression, 1);
            var result = found.FirstOrDefault();

            return result;
        } // method SearchReadOneRecordAsync

        /// <summary>
        /// Полнотекстовый поиск.
        /// </summary>
        /// <param name="parameters"></param>
        public void SearchText
            (
                TextParameters parameters
            )
        {
            // TODO implement
        }

        /// <summary>
        ///
        /// </summary>
        public string ToConnectionString()
        {
            return $"host={Host};port={Port};username={Username};password={Password};database={Database};arm={Workstation};";
        } // method ToConnectionString

        /// <summary>
        ///
        /// </summary>
        public async Task<bool> TruncateDatabaseAsync
            (
                string? database = null
            )
        {
            database ??= Database;
            var response = await ExecuteAsync("S", database);

            return !ReferenceEquals(response, null);
        } // method TruncateDartabaseAsync

        /// <summary>
        /// Разблокирование указанной базы данных.
        /// </summary>
        /// <param name="database">Имя базы данных.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> UnlockDatabaseAsync
            (
                string? database = null
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            database ??=  Database;
            var response = await ExecuteAsync("U", database);

            return !ReferenceEquals(response, null);
        } // method UnlockDatabaseAsync

        /// <summary>
        /// Разблокирование записей на сервере.
        /// </summary>
        /// <param name="database">База данных.</param>
        /// <param name="mfnList">Массив MFN.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> UnlockRecordsAsync
            (
                string? database,
                IList<int> mfnList
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            if (mfnList.Count == 0)
            {
                return true;
            }

            database ??= Database;
            var list = string.Join("\n", mfnList.Select(NumericUtility.ToInvariantString));
            var response = await ExecuteAsync("Q", database, list);

            return !ReferenceEquals(response, null);
        } // method UnlockRecordsAsync

        /// <summary>
        /// Обновление строк серверного INI-файла
        /// для текущего пользователя.
        /// </summary>
        /// <param name="lines">Изменённые строки</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> UpdateIniFileAsync
            (
                IList<string>? lines
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            if (ReferenceEquals(lines, null))
            {
                return true;
            }

            var text = string.Join("\n", lines);
            var response = await ExecuteAsync("+7", text);

            return !ReferenceEquals(response, null);
        } // method UpdateIniFileAsync

        /// <summary>
        /// Обновление списка пользователей на сервере.
        /// </summary>
        /// <param name="users">Список пользователей.</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> UpdateUserListAsync
            (
                IEnumerable<UserInfo> users
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            var query = new ClientQuery(this, "+7");
            foreach (var user in users)
            {
                query.AddAnsi(user.Encode()).NewLine();
            }

            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            return true;
        } // method UpdateUserList

        /// <summary>
        ///
        /// </summary>
        public async Task<int> WriteRecordAsync
            (
                MarcRecord? record,
                bool lockFlag = false,
                bool actualize = true,
                bool dontParse = false
            )
        {
            if (!CheckConnection() || ReferenceEquals(record, null))
            {
                return 0;
            }

            var database = record.Database ?? Database;
            var query = new ClientQuery(this, "D");
            query.AddAnsi(database).NewLine();
            query.Add(Convert.ToInt32(lockFlag)).NewLine();
            query.Add(Convert.ToInt32(actualize)).NewLine();
            query.AddUtf(record.Encode()).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return 0;
            }
            if (!response.CheckReturnCode())
            {
                return 0;
            }

            if (!dontParse)
            {
                record.Clear();
                var lines = new LocalList<string>();
                lines.Add(response.ReadUtf());
                lines.AddRange(IrbisText.SplitIrbisToLines(response.ReadUtf()));
                record.Decode(lines.ToArray());
                record.Database = database;
            }

            return response.ReturnCode;
        } // method WriteRecordAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<int> WriteRecordAsync
            (
                RawRecord record,
                bool lockFlag = false,
                bool actualize = true
            )
        {
            if (!CheckConnection())
            {
                return 0;
            }

            var database = record.Database ?? Database;
            var query = new ClientQuery(this, "D");
            query.AddAnsi(database).NewLine();
            query.Add(Convert.ToInt32(lockFlag)).NewLine();
            query.Add(Convert.ToInt32(actualize)).NewLine();
            query.AddUtf(record.EncodeRecord()).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null))
            {
                return 0;
            }
            if (!response.CheckReturnCode())
            {
                return 0;
            }

            return response.ReturnCode;
        } // method WriteRecordAsync

        /// <summary>
        ///
        /// </summary>
        public async Task<bool> WriteRecordsAsync
            (
                IEnumerable<MarcRecord>? records
            )
        {
            if (!CheckConnection() || ReferenceEquals(records, null))
            {
                return false;
            }

            // TODO implement

            await Task.Delay(10, Cancellation);

            return true;
        } // method WriteRecordsAsync

        /// <summary>
        /// Сохранение текстового файла на сервере.
        /// </summary>
        /// <param name="specification">Спецификация (включая текст для сохранения).</param>
        /// <returns>Признак успешности операции.</returns>
        public async Task<bool> WriteTextFile
            (
                FileSpecification specification
            )
        {
            if (!CheckConnection())
            {
                return false;
            }

            var query = new ClientQuery(this, "L");
            query.AddAnsi(specification.ToString()).NewLine();
            var response = await ExecuteAsync(query);
            if (ReferenceEquals(response, null) || !response.CheckReturnCode())
            {
                return false;
            }

            return true;
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            DisconnectAsync().WaitWithoutException(Cancellation);
        } // method Dispose

        #endregion
    }
}
