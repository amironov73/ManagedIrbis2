// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ByteNavigator2.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Навигатор по байтовому массиву with <see cref="Memory{T}"/> support.
    /// </summary>
    [PublicAPI]
    public sealed class ByteNavigator2
    {
        #region Constants

        /// <summary>
        /// Признак конца данных.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const int EOF = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Используемая кодировка.
        /// </summary>
        [NotNull]
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Достигнут конец данных?
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool IsEOF => Position >= Length;

        /// <summary>
        /// Длина массива.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Текущая позиция.
        /// </summary>
        public int Position { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ByteNavigator2
            (
                Memory<byte> data,
                [CanBeNull] Encoding encoding = null
            )
        {
            encoding = encoding ?? Encoding.Default;

            _data = data;
            Position = 0;
            Length = data.Length;
            Encoding = encoding;
        }

        #endregion

        #region Private members

        private Memory<byte> _data;

        #endregion

        #region Public methods

        /// <summary>
        /// Clone the navigator.
        /// </summary>
        [NotNull]
        public ByteNavigator2 Clone()
        {
            ByteNavigator2 result = new ByteNavigator2(_data, Encoding)
            {
                Encoding = Encoding,
                Position = Position
            };

            return result;
        }

        /// <summary>
        /// Навигатор по двоичному файлу.
        /// </summary>
        [NotNull]
        public static ByteNavigator2 FromFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            byte[] data = File.ReadAllBytes(fileName);
            ByteNavigator2 result = new ByteNavigator2(data);

            return result;
        }

        /// <summary>
        /// Выдать остаток данных.
        /// </summary>
        public Memory<byte> GetRemainingData()
        {
            Memory<byte> result = _data.Slice(Position);

            return result;
        }

        /// <summary>
        /// Управляющий символ?
        /// </summary>
        public bool IsControl()
        {
            char c = PeekChar();
            return char.IsControl(c);
        }

        /// <summary>
        /// Цифра?
        /// </summary>
        public bool IsDigit()
        {
            char c = PeekChar();
            return char.IsDigit(c);
        }

        /// <summary>
        /// Буква?
        /// </summary>
        public bool IsLetter()
        {
            char c = PeekChar();
            return char.IsLetter(c);
        }

        /// <summary>
        /// Буква или цифра?
        /// </summary>
        public bool IsLetterOrDigit()
        {
            char c = PeekChar();
            return char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Часть числа?
        /// </summary>
        public bool IsNumber()
        {
            char c = PeekChar();
            return char.IsNumber(c);
        }

        /// <summary>
        /// Знак пунктуации?
        /// </summary>
        public bool IsPunctuation()
        {
            char c = PeekChar();
            return char.IsPunctuation(c);
        }

        /// <summary>
        /// Разделитель?
        /// </summary>
        public bool IsSeparator()
        {
            char c = PeekChar();
            return char.IsSeparator(c);
        }

        /// <summary>
        /// Суррогат?
        /// </summary>
        public bool IsSurrogate()
        {
            char c = PeekChar();
            return char.IsSurrogate(c);
        }

        /// <summary>
        /// Символ?
        /// </summary>
        public bool IsSymbol()
        {
            char c = PeekChar();
            return char.IsSymbol(c);
        }

        /// <summary>
        /// Пробельный символ?
        /// </summary>
        public bool IsWhiteSpace()
        {
            char c = PeekChar();
            return char.IsWhiteSpace(c);
        }

        /// <summary>
        /// Абсолютное перемещение.
        /// </summary>
        public void MoveAbsolute
            (
                int position
            )
        {
            if (position > Length)
            {
                position = Length;
            }
            if (position < 0)
            {
                position = 0;
            }
            Position = position;
        }

        /// <summary>
        /// Относительное перемещение.
        /// </summary>
        public void MoveRelative
            (
                int delta
            )
        {
            Position += delta;
            if (Position > Length)
            {
                Position = Length;
            }
            if (Position < 0)
            {
                Position = 0;
            }
        }

        /// <summary>
        /// Подсмотреть один байт.
        /// </summary>
        public int PeekByte()
        {
            return Position >= Length
                ? EOF
                : _data.Span[Position];
        }

        /// <summary>
        /// Peek one char.
        /// </summary>
        public char PeekChar()
        {
            if (Position >= Length)
            {
                return '\0';
            }

            char result = (char)_data.Span[Position];

            return result;

            // TODO implement properly
        }

        /// <summary>
        /// Прочитать один байт
        /// (текущая позиция продвигается).
        /// </summary>
        public int ReadByte()
        {
            if (Position >= Length)
            {
                return -1;
            }

            int result = _data.Span[Position];
            Position++;

            return result;
        }

        /// <summary>
        /// Read one char.
        /// </summary>
        public char ReadChar()
        {
            if (Position >= Length)
            {
                return '\0';
            }

            char result = (char)_data.Span[Position];
            Position++;

            return result;

            // TODO implement properly

            //byte[] bytes = new byte[Encoding.GetMaxByteCount(1)];
            //bytes[0] = _data[Position];
            //Position++;
            //int count = 1;

            //Decoder decoder = Encoding.GetDecoder();
            //decoder.Reset();

            //char[] chars = new char[1];
            //int bytesUsed, charsUsed;
            //bool completed;
            //decoder.Convert(bytes, 0, count, chars, 0, 1,
            //    false, out bytesUsed, out charsUsed,
            //    out completed);
            //while (charsUsed != 1)
            //{
            //    if (count == bytes.Length)
            //    {
            //        return '\0';
            //    }

            //    if (Position >= Length)
            //    {
            //        return '\0';
            //    }

            //    bytes[count] = _data[Position];
            //    count++;
            //    Position++;

            //    decoder.Convert(bytes, 0, count, chars, 0, 1,
            //        false, out bytesUsed, out charsUsed,
            //        out completed);
            //}

            //return chars[0];
        }

        /// <summary>
        /// Чтение до конца строки.
        /// </summary>
        public string ReadLine()
        {
            if (IsEOF)
            {
                return null;
            }

            int start = Position;

            while (!IsEOF)
            {
                char c = PeekChar();
                if (c == '\r' || c == '\n')
                {
                    break;
                }

                Position++;
            }

            int length = Position - start;

            if (!IsEOF)
            {
                char c = PeekChar();

                if (c == '\r')
                {
                    ReadChar();
                    c = PeekChar();
                }
                if (c == '\n')
                {
                    ReadChar();
                }
            }

            // TODO get rid of memory allocation

            var span = _data.Span.Slice(start, length);
            byte[] bytes = span.ToArray();
            string result = Encoding.GetString(bytes);

            return result;
        }

        /// <summary>
        /// Пропускаем строку
        /// </summary>
        public void SkipLine()
        {
            if (IsEOF)
            {
                return;
            }

            while (!IsEOF)
            {
                char c = PeekChar();
                if (c == '\r' || c == '\n')
                {
                    break;
                }
                ReadChar();
            }

            if (!IsEOF)
            {
                char c = PeekChar();

                if (c == '\r')
                {
                    ReadChar();
                    c = PeekChar();
                }
                if (c == '\n')
                {
                    ReadChar();
                }
            }
        }

        #endregion
    }
}
