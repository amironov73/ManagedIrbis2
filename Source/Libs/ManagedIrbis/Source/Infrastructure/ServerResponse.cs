// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ServerResponse.cs -- server response network packet
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AM;
using AM.Collections;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Server response network packet.
    /// </summary>
    public sealed class ServerResponse
    {
        #region Properties

        /// <summary>
        ///
        /// </summary>
        public string Command { get; private set; }

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
        public int ReturnCode { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public int AnswerSize { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public string ServerVersion { get; private set; }

        /// <summary>
        /// End of text?
        /// </summary>
        public bool EOT { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerResponse()
        {
            _memory = new List<ArraySegment<byte>>();
        }

        #endregion

        #region Private members

        [NotNull] private readonly List<ArraySegment<byte>> _memory;
        private ArraySegment<byte> _currentChunk;
        private int _currentIndex, _currentOffset;

        #endregion

        /// <summary>
        ///
        /// </summary>
        public bool CheckReturnCode(params int[] goodCodes)
        {
            if (GetReturnCode() < 0)
            {
                if (Array.IndexOf(goodCodes, ReturnCode) < 0)
                {
                    // throw new IrbisException(ReturnCode);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Pull the data from the stream.
        /// </summary>
        public async Task PullDataAsync
            (
                [NotNull] Stream stream,
                int bufferSize,
                CancellationToken token
            )
        {
            Sure.NotNull(stream, nameof(stream));

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                var buffer = new byte[bufferSize];
                var read = await stream.ReadAsync(buffer, 0, bufferSize, token);
                if (read <= 0)
                {
                    break;
                }

                var chunk = new ArraySegment<byte>(buffer, 0, read);
                _memory.Add(chunk);
            }
        }

        /// <summary>
        /// Parse the answer.
        /// </summary>
        public void Parse()
        {
            if (_memory.Count == 0)
            {
                EOT = true;
            }
            else
            {
                EOT = false;
                _currentChunk = _memory.FirstOrDefault();
                _currentIndex = 0;
                _currentOffset = 0;

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
        }

        /// <summary>
        ///
        /// </summary>
        public byte Peek()
        {
            if (EOT)
            {
                return 0;
            }

            if (_currentOffset >= _currentChunk.Count)
            {
                _currentOffset = 0;
                _currentIndex++;
                if (_currentIndex >= _memory.Count)
                {
                    EOT = true;
                    return 0;
                }

                _currentChunk = _memory[_currentIndex];
            }

            return _currentChunk[_currentOffset];
        }

        /// <summary>
        ///
        /// </summary>
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
                if (_currentIndex >= _memory.Count)
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
                if (_currentIndex >= _memory.Count)
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

        /// <summary>
        ///
        /// </summary>
        [NotNull]
        public byte[] ReadLine()
        {
            using (var result = new MemoryStream())
            {
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
        }

        /// <summary>
        ///
        /// </summary>
        public string ReadLine(Encoding encoding)
        {
            byte[] bytes = ReadLine();
            if (bytes.Length == 0)
            {
                return string.Empty;
            }

            return encoding.GetString(bytes);
        }

        /// <summary>
        ///
        /// </summary>
        public byte[] RemainingBytes()
        {
            if (EOT)
            {
                return Array.Empty<byte>();
            }

            var length = _currentChunk.Count - _currentOffset;

            for (var i = _currentIndex + 1; i < _memory.Count; i++)
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
            for (var i = _currentIndex + 1; i < _memory.Count; i++)
            {
                var chunk = _memory[i];
                chunk.CopyTo(result, offset);
                offset += chunk.Count;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        public string RemainingText(Encoding encoding)
        {
            var bytes = RemainingBytes();
            if (bytes.Length == 0)
            {
                return string.Empty;
            }

            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Debug dump.
        /// </summary>
        public void Debug([CanBeNull] TextWriter writer)
        {
            if (!ReferenceEquals(writer, null))
            {
                foreach (var memory in _memory)
                {
                    foreach (var b in memory)
                    {
                        writer.Write($" {b:X2}");
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public int GetReturnCode()
        {
            ReturnCode = ReadInteger();

            return ReturnCode;
        }

        /// <summary>
        ///
        /// </summary>
        public string ReadAnsi() => ReadLine(IrbisEncoding.Ansi);

        /// <summary>
        ///
        /// </summary>
        public int ReadInteger() => ReadLine(IrbisEncoding.Ansi).SafeToInt32();

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<string> EnumRemainingAnsiLines()
        {
            while (!EOT)
            {
                string line = null;
                try
                {
                    line = ReadAnsi();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    continue;
                }

                if (!ReferenceEquals(line, null))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<string> EnumRemainingNonNullAnsiLines()
        {
            while (!EOT)
            {
                string line = null;
                try
                {
                    line = ReadAnsi();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    continue;
                }

                if (!string.IsNullOrEmpty(line))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<string> EnumRemainingUtfLines()
        {
            while (!EOT)
            {
                string line = null;
                try
                {
                    line = ReadUtf();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    continue;
                }

                if (!ReferenceEquals(line, null))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<string> EnumRemainingNonNullUtfLines()
        {
            while (!EOT)
            {
                string line = null;
                try
                {
                    line = ReadUtf();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    continue;
                }

                if (!string.IsNullOrEmpty(line))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerable<byte[]> EnumRemainingBinaryLines()
        {
            while (!EOT)
            {
                byte[] line = null;
                try
                {
                    line = ReadLine();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    continue;
                }

                if (!ReferenceEquals(line, null))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string[] ReadRemainingAnsiLines()
        {
            LocalList<string> result = new LocalList<string>();

            while (!EOT)
            {
                var line = ReadAnsi();
                result.Add(line);
            }

            return result.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        public string ReadRemainingAnsiText() => RemainingText(IrbisEncoding.Ansi);

        /// <summary>
        ///
        /// </summary>
        public string[] ReadRemainingUtfLines()
        {
            LocalList<string> result = new LocalList<string>();

            while (!EOT)
            {
                string line = ReadLine(IrbisEncoding.Utf8);
                result.Add(line);
            }

            return result.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        public string ReadRemainingUtfText() => RemainingText(IrbisEncoding.Utf8);

        /// <summary>
        ///
        /// </summary>
        public string ReadUtf() => ReadLine(IrbisEncoding.Utf8);
    }
}
