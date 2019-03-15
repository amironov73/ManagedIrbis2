using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using JetBrains.Annotations;

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
        public void Insert(int index,T item)
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
    /// Analog for <see cref="System.IO.MemoryStream"/> that uses
    /// small chunks to hold the data.
    /// </summary>
    [PublicAPI]
    public sealed class ChunkedBuffer
    {
        #region Constants

        /// <summary>
        /// Default chunk size.
        /// </summary>
        public const int DefaultChunkSize = 2048;

        #endregion

        #region Nested classes

        /// <summary>
        /// Chunk of bytes.
        /// </summary>
        class Chunk
        {
            public readonly byte[] Buffer;

            public Chunk Next;

            public Chunk (int size)
            {
                Buffer = new byte[size];
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Chunk size.
        /// </summary>
        public int ChunkSize => _chunkSize;

        /// <summary>
        /// End of data?
        /// </summary>
        public bool Eof
        {
            get
            {
                if (ReferenceEquals(_current, null))
                {
                    return true;
                }

                if (ReferenceEquals(_current, _last))
                {
                    return _read >= _position;
                }

                return false;
            }
        }

        /// <summary>
        /// Total length.
        /// </summary>
        public int Length
        {
            get
            {
                int result = 0;

                for (
                        Chunk chunk = _first;
                        !ReferenceEquals(chunk, null)
                        && !ReferenceEquals(chunk, _last);
                        chunk = chunk.Next
                    )
                {
                    result += _chunkSize;
                }

                result += _position;

                return result;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChunkedBuffer()
            : this(DefaultChunkSize)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ChunkedBuffer(int chunkSize)
        {
            _chunkSize = chunkSize;
        }

        #endregion

        #region Private members

        private Chunk _first, _current, _last;
        private readonly int _chunkSize;
        private int _position, _read;

        private bool _Advance()
        {
            if (ReferenceEquals(_current, _last))
            {
                return false;
            }

            _current = _current.Next;
            _read = 0;

            return true;
        }

        private void _AppendChunk()
        {
            Chunk newChunk = new Chunk(_chunkSize);
            if (ReferenceEquals(_first, null))
            {
                _first = newChunk;
                _current = newChunk;
            }
            else
            {
                _last.Next = newChunk;
            }
            _last = newChunk;
            _position = 0;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Copy data from the stream.
        /// </summary>
        public void CopyFrom
            (
                [NotNull] Stream stream,
                int bufferSize
            )
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Peek one byte.
        /// </summary>
        public int Peek()
        {
            if (ReferenceEquals(_current, null))
            {
                return -1;
            }

            if (ReferenceEquals(_current, _last))
            {
                if (_read >= _position)
                {
                    return -1;
                }
            }
            else
            {
                if (_read >= _chunkSize)
                {
                    _Advance();
                }
            }

            return _current.Buffer[_read];
        }

        /// <summary>
        /// Read array of bytes.
        /// </summary>
        public int Read
            (
                [NotNull] byte[] buffer
            )
        {
            return Read(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Read bytes.
        /// </summary>
        public int Read
            (
                byte[] buffer,
                int offset,
                int count
            )
        {
            if (count <= 0)
            {
                return 0;
            }

            if (ReferenceEquals(_current, null))
            {
                return 0;
            }

            int total = 0;
            do
            {
                int remaining = ReferenceEquals(_current, _last)
                    ? _position - _read
                    : _chunkSize - _read;

                if (remaining <= 0)
                {
                    if (!_Advance())
                    {
                        break;
                    }
                }

                int portion = Math.Min(count, remaining);
                Array.Copy
                    (
                        _current.Buffer,
                        _read,
                        buffer,
                        offset,
                        portion
                    );
                _read += portion;
                offset += portion;
                count -= portion;
                total += portion;
            } while (count > 0);

            return total;
        }

        /// <summary>
        /// Read one byte.
        /// </summary>
        public int ReadByte()
        {
            if (ReferenceEquals(_current, null))
            {
                return -1;
            }

            if (ReferenceEquals(_current, _last))
            {
                if (_read >= _position)
                {
                    return -1;
                }
            }
            else
            {
                if (_read >= _chunkSize)
                {
                    _Advance();
                }
            }

            return _current.Buffer[_read++];
        }

        /// <summary>
        /// Read one line from the current position.
        /// </summary>
        [CanBeNull]
        public string ReadLine
            (
                [NotNull] Encoding encoding
            )
        {
            if (Eof)
            {
                return null;
            }

            MemoryStream result = new MemoryStream();
            byte found = 0;
            while (found == 0)
            {
                byte[] buffer = _current.Buffer;
                int stop = ReferenceEquals(_current, _last)
                    ? _position
                    : _chunkSize;
                int head = _read;
                for (; head < stop; head++)
                {
                    byte c = buffer[head];
                    if (c == '\r' || c == '\n')
                    {
                        found = c;
                        break;
                    }
                }
                result.Write(buffer, _read, head - _read);
                _read = head;
                if (found != 0)
                {
                    _read++;
                }
                else
                {
                    if (!_Advance())
                    {
                        break;
                    }
                }
            }
            if (found == '\r')
            {
                if (Peek() == '\n')
                {
                    ReadByte();
                }
            }

            return encoding.GetString(result.ToArray());
        }

        /**
         * Получение непрочитанных байт одним большим куском. Позиция не сдвигается.
         */
        public byte[] ReadRemaining()
        {
            byte[] result = new byte[RemainingLength()];

            if (!ReferenceEquals(_current, null))
            {
                int offset=0, length;

                if (ReferenceEquals(_current, _last))
                {
                    length = _position - _read;
                    Array.Copy(_current.Buffer, _read, result, 0, length);
                }
                else
                {
                    length = _chunkSize - _read;
                    Array.Copy(_current.Buffer, _read, result, offset, length);

                    for
                        (
                            Chunk chunk = _current.Next;
                            !ReferenceEquals(chunk, _last);
                            chunk = chunk.Next
                        )
                    {
                        Array.Copy(chunk.Buffer, 0, result, offset, _chunkSize);
                        offset += _chunkSize;
                    }

                    length = _chunkSize - _position;
                    Array.Copy(_last.Buffer, 0, result, offset, length);
                }
            }

            return result;
        }

        /**
         * Количество непрочитанных байт.
         */
        public int RemainingLength()
        {
            int result = 0;

            if (!ReferenceEquals(_current, null))
            {
                if (ReferenceEquals(_current, _last))
                {
                    result = _position - _read;
                }
                else
                {
                    result += _chunkSize - _read;

                    for
                        (
                            Chunk chunk = _current.Next;
                            !ReferenceEquals(chunk, _last);
                            chunk = chunk.Next
                        )
                    {
                        result += _chunkSize;
                    }

                    result += _position - _read;
                }
            }

            return result;
        }

        /// <summary>
        /// Rewind to the beginning.
        /// </summary>
        public void Rewind()
        {
            _current = _first;
            _read = 0;
        }

        /// <summary>
        /// Get internal buffers.
        /// </summary>
        [NotNull]
        public byte[][] ToArrays
            (
                int prefix
            )
        {
            List<byte[]> result = new List<byte[]>();

            for (int i = 0; i < prefix; i++)
            {
                result.Add(Array.Empty<byte>());
            }

            for (
                    Chunk chunk = _first;
                    !ReferenceEquals(chunk, null)
                    && !ReferenceEquals(chunk, _last);
                    chunk = chunk.Next
                )
            {
                result.Add(chunk.Buffer);
            }

            if (_position != 0)
            {
                byte[] chunk = new byte[_position];
                Array.Copy(_last.Buffer, 0, chunk, 0, _position);
                result.Add(chunk);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get all data as one big array of bytes.
        /// </summary>
        [NotNull]
        public byte[] ToBigArray()
        {
            int total = Length;
            byte[] result = new byte[total];
            int offset = 0;
            for (
                    Chunk chunk = _first;
                    !ReferenceEquals(chunk, null)
                    && !ReferenceEquals(chunk, _last);
                    chunk = chunk.Next
                )
            {
                Array.Copy(chunk.Buffer, 0, result, offset, _chunkSize);
                offset += _chunkSize;
            }

            if (_position != 0)
            {
                Array.Copy(_last.Buffer, 0, result, offset, _position);
            }

            return result;
        }

        /// <summary>
        /// Write a block of bytes to the current stream
        /// using data read from a buffer.
        /// </summary>
        public void Write
            (
                [NotNull] byte[] buffer
            )
        {
            Write(buffer, 0, buffer.Length);
        }


        /// <summary>
        /// Write a block of bytes to the current stream
        /// using data read from a buffer.
        /// </summary>
        public void Write
            (
                [NotNull] byte[] buffer,
                int offset,
                int count
            )
        {
            if (count <= 0)
            {
                return;
            }

            if (ReferenceEquals(_first, null))
            {
                _AppendChunk();
            }

            do
            {
                int free = _chunkSize - _position;
                if (free == 0)
                {
                    _AppendChunk();
                    free = _chunkSize;
                }

                int portion = Math.Min(count, free);
                Array.Copy
                    (
                        buffer,
                        offset,
                        _last.Buffer,
                        _position,
                        portion
                    );

                _position += portion;
                count -= portion;
                offset += portion;
            } while (count > 0);
        }

        /// <summary>
        /// Write the text with encoding.
        /// </summary>
        public void Write
            (
                [NotNull] string text,
                [NotNull] Encoding encoding
            )
        {
            byte[] bytes = encoding.GetBytes(text);

            Write(bytes);
        }

        /// <summary>
        /// Write a byte to the current stream at the current position.
        /// </summary>
        public void WriteByte
            (
                byte value
            )
        {
            if (ReferenceEquals(_first, null))
            {
                _AppendChunk();
            }

            if (_position >= _chunkSize)
            {
                _AppendChunk();
            }

            _last.Buffer[_position++] = value;
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
    }

    public sealed class RecordField
    {
        public int Tag { get; set; }
        public string Value { get; set; }
        public List<SubField> Subfields { get; } = new List<SubField>();

        public RecordField Add(char code, string value)
        {
            var subfield = new SubField{Code = code, Value = value};
            Subfields.Add(subfield);

            return this;
        }

        public RecordField Clear()
        {
            Value = null;
            Subfields.Clear();

            return this;
        }
    }

    public sealed class MarcRecord
    {
        public string Database { get; set; }
        public int Mfn { get; set; }
        public int Version { get; set; }
        public RecordStatus Status { get; set; }
        public List<RecordField> Fields { get; } = new List<RecordField>();

        public RecordField Add(int tag, string value = null)
        {
            var result = new RecordField{Tag = tag, Value = value};
            Fields.Add(result);

            return result;
        }

        public MarcRecord Clear()
        {
            Fields.Clear();

            return this;
        }
    }

    public static class Utility
    {
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
    }

    public sealed class ClientQuery
    {
        [NotNull]
        private readonly ChunkedBuffer _buffer;

        [NotNull]
        private readonly Encoding _ansi;

        [NotNull]
        private readonly Encoding _utf;

        public ClientQuery
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string command
            )
        {
            _buffer = new ChunkedBuffer();
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
            _buffer.Write(converted);

            return this;
        }

        public ClientQuery AddUtf<T>(T value)
        {
            byte[] converted = _utf.GetBytes(value.ToString());
            _buffer.Write(converted);

            return this;
        }

        public byte[][] Encode()
        {
            int length = _buffer.Length;
            byte[][] result = _buffer.ToArrays(1);
            byte[] prefix = _ansi.GetBytes(Utility.IntToString(length) + "\n");
            result[0] = prefix;

            return result;
        }

        public ClientQuery NewLine()
        {
            _buffer.WriteByte(10);

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

        [NotNull]
        private readonly ChunkedBuffer _buffer;

        [NotNull]
        private readonly Encoding _ansi;

        [NotNull]
        private readonly Encoding _utf;

        public ServerResponse
            (
                [NotNull]Stream stream
            )
        {
            _buffer = new ChunkedBuffer();
            _buffer.CopyFrom(stream, _buffer.ChunkSize);
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

        public void CheckReturnCode(params int[] goodCodes)
        {
            if (GetReturnCode() < 0)
            {
                if (!goodCodes.Contains(ReturnCode))
                {
                    throw new IrbisException(ReturnCode);
                }
            }
        }

        public void Debug(bool debug)
        {
            // TODO implement
        }

        public string GetLine([NotNull] Encoding encoding) =>
            _buffer.ReadLine(encoding);

        public int GetReturnCode()
        {
            ReturnCode = ReadInteger();

            return ReturnCode;
        }

        public string ReadAnsi()
        {
            return GetLine(_ansi);
        }

        public int ReadInteger()
        {
            return Utility.StringToInt(GetLine(_ansi));
        }

        public string[] ReadRemainingAnsiLines()
        {
            LocalList<string> result = new LocalList<string>();

            while (!_buffer.Eof)
            {
                string line = _buffer.ReadLine(_ansi);
                result.Add(line);
            }

            return result.ToArray();
        }

        public string ReadRemainingAnsiText()
        {
            byte[] bytes = _buffer.ReadRemaining();
            string result = _ansi.GetString(bytes);

            return result;
        }

        public string[] ReadRemainingUtfLines()
        {
            LocalList<string> result = new LocalList<string>();

            while (!_buffer.Eof)
            {
                string line = _buffer.ReadLine(_utf);
                result.Add(line);
            }

            return result.ToArray();
        }

        public string ReadRemainingUtfText()
        {
            byte[] bytes = _buffer.ReadRemaining();
            string result = _utf.GetString(bytes);

            return result;
        }

        public string ReadUtf()
        {
            return GetLine(_utf);
        }
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

        private bool _connected = false;
        private bool _debug = false;

        public bool ActualizeRecord(string database, int mfn)
        {
            if (!_connected)
            {
                return false;
            }

            var query = new ClientQuery(this, "F");
            query.AddAnsi(database).NewLine();
            query.Add(mfn).NewLine();
            var response = Execute(query);
            response.CheckReturnCode();

            return true;
        }

        public bool Connect()
        {
            if (_connected)
            {
                return true;
            }

        AGAIN:
            ClientId = new Random().Next(100000, 999999);
            QueryId = 1;
            var query = new ClientQuery(this, "A");
            query.AddAnsi(Username).NewLine();
            query.AddAnsi(Password);

            var response = Execute(query);
            if (response.GetReturnCode() == -3337)
            {
                goto AGAIN;
            }

            if (response.ReturnCode < 0)
            {
                return false;
            }

            _connected = true;
            ServerVersion = response.ServerVersion;
            Interval = response.ReadInteger();
            // TODO Read INI-file

            return true;
        }

        public bool Disconnect()
        {
            if (!_connected)
            {
                return true;
            }

            var query = new ClientQuery(this, "B");
            query.AddAnsi(Username);
            Execute(query);
            _connected = false;

            return true;
        }

        public ServerResponse Execute(ClientQuery query)
        {
            TcpClient client = new TcpClient(Host, Port);
            var stream = client.GetStream();
            byte[][] packet = query.Encode();
            foreach (var one in packet)
            {
                stream.Write(one, 0, one.Length);
            }

            ServerResponse response = new ServerResponse(stream);
            QueryId++;

            return response;
        }

        public string FormatRecord(string format, int mfn)
        {
            if (!_connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "G");
            query.AddAnsi(Database).NewLine();
            // TODO Prepare format
            query.AddAnsi(format).NewLine();
            query.Add(1).NewLine();
            query.Add(mfn).NewLine();
            var response = Execute(query);
            response.CheckReturnCode();
            string result = response.ReadRemainingUtfText();

            return result;
        }

        public int GetMaxMfn(string database = null)
        {
            if (!_connected)
            {
                return 0;
            }

            database = database ?? Database;
            var query = new ClientQuery(this, "O");
            query.AddAnsi(database);
            var response = Execute(query);
            response.CheckReturnCode();

            return response.ReturnCode;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var connection = new IrbisConnection())
            {
                connection.Username = "librarian";
                connection.Password = "secret";

                if (!connection.Connect())
                {
                    Console.WriteLine("Не удалось подключиться!");
                    return;
                }

                int maxMfn = connection.GetMaxMfn();
                Console.WriteLine($"Max MFN: {maxMfn}");
            }
        }
    }
}
