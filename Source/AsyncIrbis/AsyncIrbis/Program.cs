using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

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
            var result = new RecordField{Tag = tag, Value = value};
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
                var process = new ProcessInfo
                {
                    Number        = lines[i + 0],
                    IpAddress     = lines[i + 1],
                    Name          = lines[i + 2],
                    ClienId       = lines[i + 3],
                    Workstation   = lines[i + 4],
                    Started       = lines[i + 5],
                    LastCommand   = lines[i + 6],
                    CommandNumber = lines[i + 7],
                    ProcessId     = lines[i + 8],
                    State         = lines[i + 9]
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
            if (lines.Length == 4)
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

        public bool Connected { get; private set; }
        private bool _debug = false;

        public bool ActualizeRecord(string database, int mfn)
        {
            if (!Connected)
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

            var response = Execute(query);
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

        public bool Disconnect()
        {
            if (!Connected)
            {
                return true;
            }

            var query = new ClientQuery(this, "B");
            query.AddAnsi(Username);
            Execute(query);
            Connected = false;

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
            var response = Execute(query);
            response.CheckReturnCode();
            string result = response.ReadRemainingUtfText();

            return result;
        }

        public int GetMaxMfn(string database = null)
        {
            if (!Connected)
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

        public ServerVersion GetServerVersion()
        {
            if (!Connected)
            {
                return new ServerVersion();
            }

            var query = new ClientQuery(this, "1");
            var response = Execute(query);
            response.CheckReturnCode();
            ServerVersion result = new ServerVersion();
            var lines = response.ReadRemainingAnsiLines();
            result.Parse(lines);

            return result;
        }

        public string[] ListFiles(string specification)
        {
            if (!Connected || string.IsNullOrEmpty(specification))
            {
                return Array.Empty<string>();
            }

            var query = new ClientQuery(this, "!");
            query.AddAnsi(specification).NewLine();
            var response = Execute(query);
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

        public ProcessInfo[] ListProcesses()
        {
            if (!Connected)
            {
                return Array.Empty<ProcessInfo>();
            }

            var query = new ClientQuery(this, "+3");
            var response = Execute(query);
            response.CheckReturnCode();
            var lines = response.ReadRemainingAnsiLines();
            var result = ProcessInfo.Parse(lines);

            return result;
        }

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
        public MarcRecord ReadRecord(int mfn)
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "C");
            query.AddAnsi(Database).NewLine();
            query.Add(mfn).NewLine();
            var response = Execute(query);
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
        public string ReadTextFile(string specification)
        {
            if (!Connected)
            {
                return null;
            }

            var query = new ClientQuery(this, "L");
            query.AddAnsi(specification).NewLine();
            var response = Execute(query);
            var result = response.ReadAnsi().IrbisToDos();

            return result;
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
                    WriteLine("Не удалось подключиться!");
                    return;
                }

                var maxMfn = connection.GetMaxMfn();
                WriteLine($"Max MFN: {maxMfn}");

                var version = connection.GetServerVersion();
                WriteLine($"{version.Organization} : {version.MaxClients}");

                var record = connection.ReadRecord(123);
                WriteLine($"{record.Fields.Count}");

                var text = connection.ReadTextFile("3.IBIS.WS.OPT");
                WriteLine(text);

                var files = connection.ListFiles("3.IBIS.*.pft");
                WriteLine(string.Join(", ", files));

                var processes = connection.ListProcesses();
                foreach (var process in processes)
                {
                    WriteLine(process);
                }
            }
        }
    }
}
