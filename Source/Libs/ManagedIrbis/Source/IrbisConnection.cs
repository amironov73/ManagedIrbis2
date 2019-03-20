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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;
using ManagedIrbis.Infrastructure.Sockets;
using ManagedIrbis.Properties;
using ManagedIrbis.Search;

#endregion

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
        public event EventHandler BusyChanged;

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
        public string ServerVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object IniFile { get; private set; }

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
        [NotNull]
        public IrbisSocket Socket { get; private set; }

        /// <summary>
        /// Busy?
        /// </summary>
        public bool Busy { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken Cancellation { get; }

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

        private CancellationTokenSource _cancellation;

        private bool _debug = false;

        private void SetBusy(bool busy)
        {
            Busy = busy;
            BusyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> ActualizeRecord(string database, int mfn)
        {
            var response = await Execute("F", database, mfn);
            return !ReferenceEquals(response, null);
        }

        /// <summary>
        /// Cancel the current operation.
        /// </summary>
        public void CancelOperation()
        {
            _cancellation.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> Connect()
        {
            if (Connected)
            {
                return true;
            }

        AGAIN:
            ClientId = new Random().Next(100000, 999999);
            QueryId = 1;
            var query = new ClientQuery(this, "A");
            query.AddAnsi(Username).NewLine();
            query.AddAnsi(Password);

            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            if (response.GetReturnCode() == -3337)
            {
                goto AGAIN;
            }

            if (response.ReturnCode < 0)
            {
                return false;
            }

            Connected = true;
            ServerVersion = response.ServerVersion;
            Interval = response.ReadInteger();
            // TODO Read INI-file

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> Disconnect()
        {
            if (!Connected)
            {
                return true;
            }

            var query = new ClientQuery(this, "B");
            query.AddAnsi(Username);
            try
            {
                await Execute(query);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            Connected = false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ServerResponse> Execute
            (
                [NotNull] ClientQuery query
            )
        {
            Sure.NotNull(query, nameof(query));

            SetBusy(true);
            try
            {
                if (_cancellation.IsCancellationRequested)
                {
                    _cancellation = new CancellationTokenSource();
                }

                ServerResponse result;
                try
                {
                    if (_debug)
                    {
                        query.Debug(Console.Out);
                    }

                    result = await Socket.Transact(query);
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
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ServerResponse> Execute
            (
                [NotNull] string command,
                params object[] args
            )
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, command);
            foreach (var arg in args)
            {
                query.AddAnsi(arg).NewLine();
            }

            var result = await Execute(query);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<string> FormatRecord(string format, int mfn)
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "G");
            query.AddAnsi(Database).NewLine();
            string prepared = IrbisFormat.PrepareFormat(format);
            query.AddAnsi(prepared).NewLine();
            query.Add(1).NewLine();
            query.Add(mfn).NewLine();
            var response = await Execute(query);
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
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<int> GetMaxMfn(string database = null)
        {
            if (!Connected)
            {
                return 0;
            }

            database = database ?? Database;
            var query = new ClientQuery(this, "O");
            query.AddAnsi(database).NewLine();
            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return 0;
            }

            if (!response.CheckReturnCode())
            {
                return 0;
            }

            return response.ReturnCode;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ServerVersion> GetServerVersion()
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "1");
            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            response.CheckReturnCode();
            ServerVersion result = new ServerVersion();
            var lines = response.ReadRemainingAnsiLines();
            result.Parse(lines);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<string[]> ListFiles(string specification)
        {
            if (!Connected || string.IsNullOrEmpty(specification))
            {
                return Array.Empty<string>();
            }

            var query = new ClientQuery(this, "!");
            query.AddAnsi(specification).NewLine();
            var response = await Execute(query);
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
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ProcessInfo[]> ListProcesses()
        {
            if (!Connected)
            {
                return Array.Empty<ProcessInfo>();
            }

            var query = new ClientQuery(this, "+3");
            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return Array.Empty<ProcessInfo>();
            }

            response.CheckReturnCode();
            var lines = response.ReadRemainingAnsiLines();
            var result = ProcessInfo.Parse(lines);

            return result;
        }

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
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<MarcRecord> ReadRecord(int mfn)
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "C");
            query.AddAnsi(Database).NewLine();
            query.Add(mfn).NewLine();
            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            var code = response.GetReturnCode();
            if (code < 0)
            {
                // TODO add good codes
                return null;
            }

            var result = new MarcRecord
            {
                Database = Database
            };
            var lines = response.ReadRemainingUtfLines();
            result.Decode(lines);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<TermInfo[]> ReadAllTerms
            (
                [NotNull] string prefix
            )
        {
            Sure.NotNullNorEmpty(prefix, nameof(prefix));

            if (!Connected)
            {
                return Array.Empty<TermInfo>();
            }

            prefix = prefix.ToUpperInvariant();
            var result = new LocalList<TermInfo>();
            var startTerm = prefix;
            var flag = true;
            while (flag)
            {
                var terms = await ReadTerms(startTerm, 1024);
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
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<TermInfo[]> ReadTerms
            (
                [NotNull] TermParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            if (!Connected)
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
            var response = await Execute(query);
            if (!response.CheckReturnCode(-202, -203, -204))
            {
                return Array.Empty<TermInfo>();
            }

            var lines = response.EnumRemainingNonNullUtfLines();
            var result = TermInfo.Parse(lines);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<TermInfo[]> ReadTerms
            (
                [CanBeNull] string startTerm,
                int numberOfTerms = 100
            )
        {
            TermParameters parameters = new TermParameters
            {
                StartTerm = startTerm,
                NumberOfTerms = numberOfTerms
            };
            var result = await ReadTerms(parameters);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<string> ReadTextFile
            (
                [CanBeNull] string specification
            )
        {
            if (string.IsNullOrEmpty(specification))
            {
                return null;
            }

            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "L");
            query.AddAnsi(specification).NewLine();
            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return null;
            }

            var result = IrbisText.IrbisToWindows(response.ReadAnsi());

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<FoundItem[]> Search
            (
                [NotNull] SearchParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            if (!Connected)
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
            var response = await Execute(query);
            if (!response.CheckReturnCode())
            {
                return Array.Empty<FoundItem>();
            }

            var count = response.ReadInteger(); // Число найденных записей
            var lines = response.EnumRemainingUtfLines();
            var result = FoundItem.ParseServerResponse(lines, count);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<int[]> Search
            (
                [CanBeNull] string expression
            )
        {
            if (string.IsNullOrEmpty(expression))
            {
                return Array.Empty<int>();
            }

            var query = new ClientQuery(this, "K");
            query.AddAnsi(Database).NewLine();
            query.AddUtf(expression).NewLine();
            query.Add(0).NewLine();
            query.Add(1).NewLine();
            var response = await Execute(query);
            if (!response.CheckReturnCode())
            {
                return Array.Empty<int>();
            }

            int count = response.ReadInteger(); // Число найденных записей
            var result = new LocalList<int>(count);
            foreach (var line in response.EnumRemainingBinaryLines())
            {
                int mfn = FastNumber.ParseInt32(line, 0, line.Length);
                if (mfn != 0)
                {
                    result.Add(mfn);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<int> SearchCount
            (
                [CanBeNull] string expression
            )
        {
            if (string.IsNullOrEmpty(expression))
            {
                return 0;
            }

            var query = new ClientQuery(this, "K");
            query.AddAnsi(Database).NewLine();
            query.AddUtf(expression).NewLine();
            query.Add(0).NewLine();
            query.Add(1).NewLine();
            var response = await Execute(query);
            if (!response.CheckReturnCode())
            {
                return 0;
            }

            int result = response.ReadInteger(); // Число найденных записей

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<MarcRecord[]> SearchRead
            (
                [CanBeNull] string expression,
                int limit = 0
            )
        {
            if (string.IsNullOrEmpty(expression)
                || limit < 0)
            {
                return Array.Empty<MarcRecord>();
            }

            if (!Connected)
            {
                return Array.Empty<MarcRecord>();
            }

            var query = new ClientQuery(this, "K");
            query.AddAnsi(Database).NewLine();
            query.AddUtf(expression).NewLine();
            query.Add(limit).NewLine();
            query.Add(1).NewLine();
            query.AddAnsi(IrbisFormat.All).NewLine();
            var response = await Execute(query);
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
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<MarcRecord> SearchReadOneRecord
            (
                [CanBeNull] string expression
            )
        {
            var found = await SearchRead(expression, 1);
            var result = found.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public string ToConnectionString()
        {
            return $"host={Host};port={Port};username={Username};password={Password};database={Database};arm={Workstation};";
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> TruncateDatabase
            (
                [CanBeNull] string database = null
            )
        {
            database = database ?? Database;
            var response = await Execute("S", database);

            return !ReferenceEquals(response, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> UnlockDatabase
            (
                [CanBeNull] string database = null
            )
        {
            database = database ?? Database;
            var response = await Execute("U", database);

            return !ReferenceEquals(response, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> UnlockRecords
            (
                [CanBeNull] string database,
                [NotNull] IList<int> mfnList
            )
        {
            database = database ?? Database;
            var list = string.Join("\n", mfnList.Select(NumericUtility.ToInvariantString));
            var response = await Execute("Q", database, list);

            return !ReferenceEquals(response, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> UpdateIniFile
            (
                [CanBeNull] IList<string> lines
            )
        {
            if (ReferenceEquals(lines, null))
            {
                return true;
            }

            var text = string.Join("\n", lines);
            var response = await Execute("+7", text);

            return !ReferenceEquals(response, null);
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            Disconnect().Wait(Cancellation);
        }

        #endregion
    }
}
