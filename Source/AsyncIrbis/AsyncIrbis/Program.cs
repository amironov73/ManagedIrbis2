using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

using static System.Console;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace AsyncIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public struct LocalList<T>
    {
        #region Private members

        private const int InitialCapacity = 4;

        private T[] _array;
        private int _size;

        private void _Extend(int newSize)
        {
            T[] newArray = new T[newSize];
            _array?.CopyTo(newArray, 0);

            _array = newArray;
        }

        private void _GrowAsNeeded()
        {
            if (_size >= _array.Length)
            {
                _Extend(_size * 2);
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalList(int capacity)
            : this()
        {
            _Extend(capacity);
        }

        #endregion

        #region Public methods

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                yield return _array[i];
            }
        }

        /// <inheritdoc cref="ICollection{T}.Add" />
        public void Add(T item)
        {
            if (ReferenceEquals(_array, null))
            {
                _Extend(InitialCapacity);
            }

            _GrowAsNeeded();
            _array[_size++] = item;
        }

        /// <summary>
        /// Add some items.
        /// </summary>
        public void AddRange
        (
            [NotNull] IEnumerable<T> items
        )
        {
            if (ReferenceEquals(_array, null))
            {
                _Extend(InitialCapacity);
            }

            foreach (T item in items)
            {
                _GrowAsNeeded();
                _array[_size++] = item;
            }
        }

        /// <inheritdoc cref="ICollection{T}.Clear" />
        public void Clear()
        {
            _size = 0;
        }

        /// <inheritdoc cref="ICollection{T}.Contains" />
        public bool Contains
        (
            T item
        )
        {
            if (ReferenceEquals(_array, null))
            {
                return false;
            }

            int index = Array.IndexOf(_array, item, 0, _size);
            return index >= 0;
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo" />
        public void CopyTo
        (
            T[] array,
            int arrayIndex
        )
        {
            if (!ReferenceEquals(_array, null))
            {
                Array.Copy(_array, 0, array, arrayIndex, _size);
            }
        }

        /// <inheritdoc cref="ICollection{T}.Remove" />
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);

                return true;
            }

            return false;
        }

        /// <inheritdoc cref="ICollection{T}.Count" />
        public int Count => _size;

        /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
        public bool IsReadOnly => false;

        /// <inheritdoc cref="IList{T}.IndexOf" />
        public int IndexOf(T item)
        {
            return ReferenceEquals(_array, null)
                ? -1
                : Array.IndexOf(_array, item, 0, _size);
        }

        /// <inheritdoc cref="IList{T}.Insert" />
        public void Insert(int index, T item)
        {
            if (ReferenceEquals(_array, null))
            {
                _Extend(InitialCapacity);
            }

            if (_size != 0 && index != _size - 1)
            {
                Array.Copy(_array, index, _array, index + 1, _size - index - 1);
            }

            _array[index] = item;
            _size++;
        }

        /// <inheritdoc cref="IList{T}.RemoveAt" />
        public void RemoveAt(int index)
        {
            if (!ReferenceEquals(_array, null))
            {
                if (index != _size - 1)
                {
                    Array.Copy(_array, index + 1, _array, index, _size - index - 1);
                }

                _size--;
            }
        }

        /// <inheritdoc cref="IList{T}.this" />
        public T this[int index]
        {
            get
            {
                if (ReferenceEquals(_array, null))
                {
                    throw new IndexOutOfRangeException();
                }

                return _array[index];
            }
            set
            {
                if (ReferenceEquals(_array, null))
                {
                    throw new IndexOutOfRangeException();
                }

                _array[index] = value;
            }
        }

        /// <summary>
        /// Convert the list to array.
        /// </summary>
        [NotNull]
        public T[] ToArray()
        {
            if (ReferenceEquals(_array, null) || _size == 0)
            {
                return Array.Empty<T>();
            }

            if (_size == _array.Length)
            {
                return _array;
            }

            T[] result = new T[_size];
            Array.Copy(_array, result, _size);

            return result;
        }

        /// <summary>
        /// Convert the list to <see cref="List{T}"/>.
        /// </summary>
        public List<T> ToList()
        {
            if (ReferenceEquals(_array, null) || _size == 0)
            {
                return new List<T>();
            }

            List<T> result = new List<T>(_size);
            for (int i = 0; i < _size; i++)
            {
                result.Add(_array[i]);
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// MARC record status
    /// </summary>
    [Flags]
    public enum RecordStatus
        : byte
    {
        /// <summary>
        /// Запись логически удалена
        /// </summary>
        LogicallyDeleted = 1,

        /// <summary>
        /// Запись физически удалена
        /// </summary>
        PhysicallyDeleted = 2,

        /// <summary>
        /// Запись отсутствует
        /// </summary>
        Absent = 4,

        /// <summary>
        /// Запись не актуализирована
        /// </summary>
        NonActualized = 8,

        /// <summary>
        /// Последний экземпляр записи
        /// </summary>
        Last = 32,

        /// <summary>
        /// Запись заблокирована
        /// </summary>
        Locked = 64
    }

    /// <summary>
    /// Простейший сборщик данных в виде байтовых чанков.
    /// </summary>
    public sealed class ChunkCollector
    {
        #region Properties

        /// <summary>
        /// Total length of the data.
        /// </summary>
        public int Length
        {
            get
            {
                var result = 0;
                foreach (var memory in _accumulator)
                {
                    result += memory.Count;
                }

                return result;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChunkCollector()
        {
            _accumulator = new List<ArraySegment<byte>>();
        }

        #endregion

        #region Private members

        private readonly List<ArraySegment<byte>> _accumulator;

        private static readonly byte[] _newLine = {10};

        #endregion

        #region Public methods

        /// <summary>
        /// Add the chunk.
        /// </summary>
        public ChunkCollector Add(ArraySegment<byte> chunk)
        {
            _accumulator.Add(chunk);

            return this;
        }

        /// <summary>
        /// Add the array.
        /// </summary>
        public ChunkCollector Add(byte[] array)
        {
            return Add(new ArraySegment<byte>(array, 0, array.Length));
        }

        /// <summary>
        /// Copy data from the stream.
        /// </summary>
        public async Task CopyFromAsync(Stream stream, int bufferSize)
        {
            while (true)
            {
                var buffer = new byte[bufferSize];
                var read = await stream.ReadAsync(buffer, 0, bufferSize);
                if (read <= 0)
                {
                    break;
                }

                var chunk = new ArraySegment<byte>(buffer, 0, read);
                Add(chunk);
            }
        }

        /// <summary>
        /// Debug print.
        /// </summary>
        public void Debug(TextWriter writer)
        {
            foreach (var memory in _accumulator)
            {
                foreach (var b in memory)
                {
                    writer.Write($" {b:X2}");
                }
            }
        }

        /// <summary>
        /// Get collected data.
        /// </summary>
        public ArraySegment<byte>[] GetChunks(int prefixLength = 0)
        {
            var length = _accumulator.Count;
            var result = new ArraySegment<byte>[length + prefixLength];
            for (int i = 0; i < length; i++)
            {
                result[prefixLength + i] = _accumulator[i];
            }

            return result;
        }

        /// <summary>
        /// Add new line symbol.
        /// </summary>
        public ChunkCollector NewLine()
        {
            return Add(_newLine);
        }

        #endregion

        #region Object members

        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder(Length);
            foreach (var memory in _accumulator)
            {
                result.Append(Encoding.Default.GetString(memory));
            }

            return result.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Чтение массива байтовых чанков.
    /// </summary>
    public sealed class ChunkReader
    {
        #region Properties

        public ChunkCollector Chunks { get; }

        public int Length { get; }

        public bool EOT { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChunkReader(ChunkCollector chunks)
        {
            Chunks = chunks;
            Length = chunks.Length;
            _memory = chunks.GetChunks();
            if (_memory.Length == 0)
            {
                EOT = true;
            }
            else
            {
                _currentChunk = _memory.FirstOrDefault();
                _currentIndex = 0;
                _currentOffset = 0;
            }
        }

        #endregion

        #region Private members

        private readonly ArraySegment<byte>[] _memory;
        private ArraySegment<byte> _currentChunk;
        private int _currentIndex, _currentOffset;

        #endregion

        #region Public methods

        public byte Peek()
        {
            if (EOT)
            {
                return 0;
            }

            return _currentChunk[_currentOffset];
        }

        public byte ReadByte()
        {
            if (EOT)
            {
                return 0;
            }

            if (_currentOffset >= _currentChunk.Count)
            {
                _currentOffset = 0;
                _currentIndex++;
                if (_currentIndex >= _memory.Length)
                {
                    EOT = true;
                    return 0;
                }

                _currentChunk = _memory[_currentIndex];
            }

            byte result = _currentChunk[_currentOffset];
            _currentOffset++;

            if (_currentOffset > _currentChunk.Count)
            {
                _currentOffset = 0;
                _currentIndex++;
                if (_currentIndex >= _memory.Length)
                {
                    EOT = true;
                }
                else
                {
                    _currentChunk = _memory[_currentIndex];
                }
            }

            return result;
        }

        public byte[] ReadLine()
        {
            var result = new MemoryStream();
            while (true)
            {
                var one = ReadByte();
                if (one == 0)
                {
                    break;
                }

                if (one == 13)
                {
                    if (Peek() == 10)
                    {
                        ReadByte();
                    }

                    break;
                }

                if (one == 10)
                {
                    break;
                }

                result.WriteByte(one);
            }

            return result.ToArray();
        }

        public string ReadLine(Encoding encoding)
        {
            byte[] bytes = ReadLine();
            if (bytes.Length == 0)
            {
                return string.Empty;
            }

            return encoding.GetString(bytes);
        }

        public byte[] RemainingBytes()
        {
            if (EOT)
            {
                return Array.Empty<byte>();
            }

            var length = _currentChunk.Count - _currentOffset;

            for (var i = _currentIndex + 1; i < _memory.Length; i++)
            {
                length += _memory[i].Count;
            }

            if (length == 0)
            {
                EOT = true;

                return Array.Empty<byte>();
            }

            var result = new byte[length];
            var offset = 0;
            _currentChunk.Slice(_currentOffset).CopyTo(result);
            offset += _currentChunk.Count - _currentOffset;
            for (var i = _currentIndex + 1; i < _memory.Length; i++)
            {
                var chunk = _memory[i];
                chunk.CopyTo(result, offset);
                offset += chunk.Count;
            }

            return result;
        }

        public string RemainingText(Encoding encoding)
        {
            var bytes = RemainingBytes();
            if (bytes.Length == 0)
            {
                return string.Empty;
            }

            return encoding.GetString(bytes);
        }

        #endregion
    }

    /// <summary>
    /// Асинхронно отрабатывает работу с TCP-сервером.
    /// </summary>
    public struct AsyncExecutor
    {
        #region Properties

        public string Host { get; }

        public int Port { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public AsyncExecutor(string host, int port)
        {
            Host = host;
            Port = port;
        }

        #endregion

        #region Public methods

        public async Task<ChunkCollector> Execute(ChunkCollector output)
        {
            // output.Debug(Out);

            using (var client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(Host, Port);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return null;
                }

                var socket = client.Client;
                var stream = client.GetStream();

                var length = output.Length;
                var prefix = Encoding.ASCII.GetBytes($"{length}\n");
                var chunks = output.GetChunks(1);
                chunks[0] = new ArraySegment<byte>(prefix);
                try
                {
                    await Task.Run(() => socket.Send(chunks));

//                    foreach (var chunk in chunks)
//                    {
//                        await stream.WriteAsync(chunk);
//                    }

                    // await stream.FlushAsync();
                    socket.Shutdown(SocketShutdown.Send);
                }
                catch (Exception exception)
                {
                    WriteLine(exception.Message);
                    Debug.WriteLine(exception.Message);
                    return null;
                }

                var result = new ChunkCollector();
                try
                {
                    await result.CopyFromAsync(stream, 2048);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return null;
                }

            // result.Debug(Out);

            return result;

            }
        }

        #endregion
    }

    public class IrbisException
        : Exception
    {
        public int Code { get; }

        public IrbisException()
        {
            Code = 0;
        }

        public IrbisException(int code)
        {
            Code = code;
        }

        public IrbisException(string message) : base(message)
        {
            Code = 0;
        }

        public IrbisException(int code, string message)
            : base(message)
        {
            Code = code;
        }

        public IrbisException(string message, Exception innerException) : base(message, innerException)
        {
            Code = 0;
        }
    }

    public sealed class SubField
    {
        public char Code { get; set; }
        public string Value { get; set; }

        public void Decode(string text)
        {
            Code = text[0];
            Value = text.Substring(1);
        }
    }

    public sealed class RecordField
    {
        /// <summary>
        /// Метка поля.
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// Значение поля до первого разделителя.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Список подполей.
        /// </summary>
        public List<SubField> Subfields { get; } = new List<SubField>();

        public RecordField Add(char code, string value)
        {
            var subfield = new SubField {Code = code, Value = value};
            Subfields.Add(subfield);

            return this;
        }

        public RecordField Clear()
        {
            Value = null;
            Subfields.Clear();

            return this;
        }

        public void Decode(string line)
        {
            var parts = line.Split('#', 2, StringSplitOptions.None);
            Tag = Utility.StringToInt(parts[0]);
            string body = parts[1];
            if (body[0] != '^')
            {
                int index = body.IndexOf('^');
                if (index < 0)
                {
                    Value = body;
                    return;
                }

                Value = body.Substring(0, index);
                body = body.Substring(index);
            }

            int offset = 1;
            bool flag = true;
            while (flag)
            {
                string one;
                int index = body.IndexOf('^', offset);
                if (index < 0)
                {
                    one = body.Substring(offset);
                    flag = false;
                }
                else
                {
                    one = body.Substring(offset, index - offset);
                    offset = index + 1;
                }

                var subfield = new SubField();
                subfield.Decode(one);
                Subfields.Add(subfield);
            }
        }
    }

    /// <summary>
    /// Библиографическая запись. Состоит из произвольного количества полей.
    /// </summary>
    public sealed class MarcRecord
    {
        /// <summary>
        /// Имя базы данных, в которой хранится запись.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// MFN записи.
        /// </summary>
        public int Mfn { get; set; }

        /// <summary>
        /// Версия записи.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Статус записи.
        /// </summary>
        public RecordStatus Status { get; set; }

        /// <summary>
        /// Список полей.
        /// </summary>
        public List<RecordField> Fields { get; } = new List<RecordField>();

        /// <summary>
        /// Добавление поля в запись.
        /// </summary>
        /// <returns>
        /// Свежедобавленное поле.
        /// </returns>
        public RecordField Add(int tag, string value = null)
        {
            var result = new RecordField {Tag = tag, Value = value};
            Fields.Add(result);

            return result;
        }

        /// <summary>
        /// Очистка записи (удаление всех полей).
        /// </summary>
        /// <returns>
        /// Очищенную запись.
        /// </returns>
        public MarcRecord Clear()
        {
            Fields.Clear();

            return this;
        }

        /// <summary>
        /// Декодирование ответа сервера.
        /// </summary>
        public void Decode(string[] lines)
        {
            var first = lines[0].Split('#');
            Mfn = Utility.StringToInt(first[0]);
            Status = (RecordStatus) Utility.StringToInt(first[1]);

            var second = lines[1].Split('#');
            Version = Utility.StringToInt(second[1]);

            for (int i = 2; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!string.IsNullOrEmpty(line))
                {
                    var field = new RecordField();
                    field.Decode(line);
                    Fields.Add(field);
                }
            }
        }
    }

    public static class Utility
    {
        private const string IrbisDelimiter = "\x001F\x001E";
        private const string ShortDelimiter = "\x001E";

        public static bool SameString
        (
            [CanBeNull] this string left,
            [CanBeNull] string right
        )
        {
            return string.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
        }

        [CanBeNull]
        public static string IrbisToDos
        (
            [CanBeNull] this string text
        )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text.Replace(IrbisDelimiter, "\n");
        }

        public static string[] SplitIrbis
        (
            [CanBeNull] this string text
        )
        {
            if (string.IsNullOrEmpty(text))
            {
                return Array.Empty<string>();
            }

            return text.Split(IrbisDelimiter);
        }

        public static Encoding AnsiEncoding()
        {
            return Encoding.GetEncoding(1251);
        }

        public static Encoding UtfEncoding()
        {
            return Encoding.UTF8;
        }

        public static string IntToString(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static int StringToInt(string value)
        {
            if (!int.TryParse(value, out int result))
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Remove comments from the format.
        /// </summary>
        [CanBeNull]
        public static string RemoveComments
        (
            [CanBeNull] string text
        )
        {
            const char zero = '\0';

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (!text.Contains("/*"))
            {
                return text;
            }

            int index = 0, length = text.Length;
            StringBuilder result = new StringBuilder(length);
            char state = zero;

            while (index < length)
            {
                char c = text[index];

                switch (state)
                {
                    case '\'':
                    case '"':
                    case '|':
                        if (c == state)
                        {
                            state = zero;
                        }

                        result.Append(c);
                        break;

                    default:
                        if (c == '/')
                        {
                            if (index + 1 < length && text[index + 1] == '*')
                            {
                                while (index < length)
                                {
                                    c = text[index];
                                    if (c == '\r' || c == '\n')
                                    {
                                        result.Append(c);
                                        break;
                                    }

                                    index++;
                                }
                            }
                            else
                            {
                                result.Append(c);
                            }
                        }
                        else if (c == '\'' || c == '"' || c == '|')
                        {
                            state = c;
                            result.Append(c);
                        }
                        else
                        {
                            result.Append(c);
                        }

                        break;
                }

                index++;
            }

            return result.ToString();
        }

        /// <summary>
        /// Prepare the dynamic format string.
        /// </summary>
        /// <remarks>Dynamic format string
        /// mustn't contains comments and
        /// string delimiters (no matter
        /// real or IRBIS).
        /// </remarks>
        [CanBeNull]
        public static string PrepareFormat
        (
            [CanBeNull] string text
        )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            text = RemoveComments(text);
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int length = text.Length;
            bool flag = false;
            for (int i = 0; i < length; i++)
            {
                if (text[i] < ' ')
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                return text;
            }

            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                char c = text[i];
                if (c >= ' ')
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }

    public sealed class ProcessInfo
    {
        public string Number { get; set; }
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public string ClienId { get; set; }
        public string Workstation { get; set; }
        public string Started { get; set; }
        public string LastCommand { get; set; }
        public string CommandNumber { get; set; }
        public string ProcessId { get; set; }
        public string State { get; set; }

        public static ProcessInfo[] Parse(string[] lines)
        {
            var result = new LocalList<ProcessInfo>();
            var processCount = Utility.StringToInt(lines[0]);
            var linesPerProcess = Utility.StringToInt(lines[1]);
            if (processCount == 0 || linesPerProcess == 0)
            {
                return result.ToArray();
            }

            for (int i = 2; i < lines.Length; i += linesPerProcess + 1)
            {
                if ((i + 9) > lines.Length)
                {
                    for (int j = i; j < lines.Length; j++)
                    {
                        WriteLine(lines[j]);
                    }

                    break;
                }

                var process = new ProcessInfo
                {
                    Number = lines[i + 0],
                    IpAddress = lines[i + 1],
                    Name = lines[i + 2],
                    ClienId = lines[i + 3],
                    Workstation = lines[i + 4],
                    Started = lines[i + 5],
                    LastCommand = lines[i + 6],
                    CommandNumber = lines[i + 7],
                    ProcessId = lines[i + 8],
                    State = lines[i + 9]
                };
                result.Add(process);
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            return $"{Number} {IpAddress} {Name}";
        }
    }

    public sealed class ServerVersion
    {
        public string Organization { get; set; }
        public string Version { get; set; }
        public int MaxClients { get; set; }
        public int ConnectedClient { get; set; }

        public void Parse(string[] lines)
        {
            if (lines.Length >= 4)
            {
                Organization = lines[0];
                Version = lines[1];
                ConnectedClient = Utility.StringToInt(lines[2]);
                MaxClients = Utility.StringToInt(lines[3]);
            }
            else
            {
                Version = lines[0];
                ConnectedClient = Utility.StringToInt(lines[1]);
                MaxClients = Utility.StringToInt(lines[2]);
            }
        }
    }

    public sealed class ClientQuery
    {
        [NotNull] public ChunkCollector Buffer { get; }

        [NotNull] private readonly Encoding _ansi;

        [NotNull] private readonly Encoding _utf;

        public ClientQuery
        (
            [NotNull] IrbisConnection connection,
            [NotNull] string command
        )
        {
            Buffer = new ChunkCollector();
            _ansi = Utility.AnsiEncoding();
            _utf = Utility.UtfEncoding();

            AddAnsi(command).NewLine();
            AddAnsi(connection.Workstation).NewLine();
            AddAnsi(command).NewLine();
            Add(connection.ClientId).NewLine();
            Add(connection.QueryId).NewLine();
            AddAnsi(connection.Password).NewLine();
            AddAnsi(connection.Username).NewLine();
            NewLine();
            NewLine();
            NewLine();
        }

        public ClientQuery Add(int value)
        {
            return AddAnsi(Utility.IntToString(value));
        }

        public ClientQuery AddAnsi<T>(T value)
        {
            byte[] converted = _ansi.GetBytes(value.ToString());
            Buffer.Add(converted);

            return this;
        }

        public ClientQuery AddUtf<T>(T value)
        {
            byte[] converted = _utf.GetBytes(value.ToString());
            Buffer.Add(converted);

            return this;
        }

        public ClientQuery NewLine()
        {
            Buffer.NewLine();

            return this;
        }
    }

    public sealed class ServerResponse
    {
        public string Command { get; private set; }
        public int ClientId { get; private set; }
        public int QueryId { get; private set; }
        public int ReturnCode { get; private set; }
        public int AnswerSize { get; private set; }
        public string ServerVersion { get; private set; }

        [NotNull] private readonly ChunkCollector _answer;
        [NotNull] private readonly ChunkReader _reader;

        [NotNull] private readonly Encoding _ansi;

        [NotNull] private readonly Encoding _utf;

        public ServerResponse([NotNull] ChunkCollector answer)
        {
            _answer = answer;
            _reader = new ChunkReader(_answer);
            _ansi = Utility.AnsiEncoding();
            _utf = Utility.UtfEncoding();

            Command = ReadAnsi();
            ClientId = ReadInteger();
            QueryId = ReadInteger();
            AnswerSize = ReadInteger();
            ServerVersion = ReadAnsi();
            ReadAnsi();
            ReadAnsi();
            ReadAnsi();
            ReadAnsi();
            ReadAnsi();
        }

        public bool CheckReturnCode(params int[] goodCodes)
        {
            if (GetReturnCode() < 0)
            {
                if (!goodCodes.Contains(ReturnCode))
                {
                    throw new IrbisException(ReturnCode);
                }
            }

            return true;
        }

        public void Debug() => _answer.Debug(Out);

        public string GetLine([NotNull] Encoding encoding) =>
            _reader.ReadLine(encoding);

        public int GetReturnCode()
        {
            ReturnCode = ReadInteger();

            return ReturnCode;
        }

        public string ReadAnsi() => GetLine(_ansi);

        public int ReadInteger()
        {
            return Utility.StringToInt(GetLine(_ansi));
        }

        public string[] ReadRemainingAnsiLines()
        {
            LocalList<string> result = new LocalList<string>();

            while (!_reader.EOT)
            {
                string line = _reader.ReadLine(_ansi);
                result.Add(line);
            }

            return result.ToArray();
        }

        public string ReadRemainingAnsiText() => _reader.RemainingText(_ansi);

        public string[] ReadRemainingUtfLines()
        {
            LocalList<string> result = new LocalList<string>();

            while (!_reader.EOT)
            {
                string line = _reader.ReadLine(_utf);
                result.Add(line);
            }

            return result.ToArray();
        }

        public string ReadRemainingUtfText() => _reader.RemainingText(_utf);

        public string ReadUtf() => GetLine(_utf);
    }

    public sealed class IrbisConnection
        : IDisposable
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6666;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Database { get; set; } = "IBIS";
        public string Workstation { get; set; } = "C";
        public int ClientId { get; private set; }
        public int QueryId { get; private set; }
        public string ServerVersion { get; private set; }
        public object IniFile { get; private set; }
        public int Interval { get; private set; }

        public bool Connected { get; private set; }
        private bool _debug = false;

        public async Task<bool> ActualizeRecord(string database, int mfn)
        {
            if (!Connected)
            {
                return false;
            }

            var query = new ClientQuery(this, "F");
            query.AddAnsi(database).NewLine();
            query.Add(mfn).NewLine();
            var response = await Execute(query);
            if (ReferenceEquals(response, null))
            {
                return false;
            }

            response.CheckReturnCode();

            return true;
        }

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

        public async Task<ServerResponse> Execute(ClientQuery query)
        {
            AsyncExecutor executor = new AsyncExecutor(Host, Port);
            ChunkCollector answer;
            try
            {
                if (_debug)
                {
                    query.Buffer.Debug(Out);
                }

                answer = await executor.Execute(query.Buffer);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }

            if (ReferenceEquals(answer, null))
            {
                return null;
            }

            if (_debug)
            {
                answer.Debug(Out);
            }

            var result = new ServerResponse(answer);
            QueryId++;

            return result;
        }

        public async Task<string> FormatRecord(string format, int mfn)
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "G");
            query.AddAnsi(Database).NewLine();
            string prepared = Utility.PrepareFormat(format);
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

            response.CheckReturnCode();

            return response.ReturnCode;
        }

        public async Task<ServerVersion> GetServerVersion()
        {
            if (!Connected)
            {
                return new ServerVersion();
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
                var files = line.SplitIrbis();
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

        public void ParseConnectionString([CanBeNull] string connectionString)
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
                        Port = Utility.StringToInt(value);
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

        [CanBeNull]
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

            var result = new MarcRecord();
            result.Database = Database;
            var lines = response.ReadRemainingUtfLines();
            result.Decode(lines);

            return result;
        }

        [CanBeNull]
        public async Task<string> ReadTextFile(string specification)
        {
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

            var result = response.ReadAnsi().IrbisToDos();

            return result;
        }

        public void Dispose()
        {
            Disconnect().Wait();
        }
    }

    class Program
    {
        static async Task AsyncMain()
        {
            using (var connection = new IrbisConnection())
            {
                connection.Username = "librarian";
                connection.Password = "secret";
                connection.Workstation = "A";

                if (!await connection.Connect())
                {
                    WriteLine("Не удалось подключиться!");
                    return;
                }

                var maxMfn = await connection.GetMaxMfn();
                WriteLine($"Max MFN: {maxMfn}");

                var formatted = await connection.FormatRecord("@brief", 123);
                WriteLine($"FORMATTED: {formatted}");

                var version = await connection.GetServerVersion();
                WriteLine($"{version.Organization} : {version.MaxClients}");

                var record = await connection.ReadRecord(123);
                WriteLine($"{record.Fields.Count}");

                var text = await connection.ReadTextFile("3.IBIS.WS.OPT");
                WriteLine(text);

                var files = await connection.ListFiles("3.IBIS.*.pft");
                WriteLine(string.Join(", ", files));

                var processes = await connection.ListProcesses();
                foreach (var process in processes)
                {
                    WriteLine(process);
                }

                await connection.Disconnect();
            }
        }

        static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            AsyncMain().Wait();
        }
    }
}