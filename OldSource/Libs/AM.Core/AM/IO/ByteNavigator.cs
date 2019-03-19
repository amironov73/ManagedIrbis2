﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ByteNavigator.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Навигатор по байтовому массиву.
    /// </summary>
    [PublicAPI]
    public sealed class ByteNavigator
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
        public ByteNavigator
            (
                [NotNull] byte[] data
            )
        {
            Sure.NotNull(data, nameof(data));

            _data = data;
            Length = data.Length;
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ByteNavigator
            (
                [NotNull] byte[] data,
                int length
            )
        {
            Sure.NotNull(data, nameof(data));
            Sure.NonNegative(length, nameof(length));

            if (length > data.Length)
            {
                length = data.Length;
            }

            _data = data;
            Length = length;
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ByteNavigator
            (
                [NotNull] byte[] data,
                int length,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(data, nameof(data));
            Sure.NonNegative(length, nameof(length));
            Sure.NotNull(encoding, nameof(encoding));

            if (length > data.Length)
            {
                length = data.Length;
            }

            _data = data;
            Length = length;
            Encoding = encoding;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ByteNavigator
            (
                [NotNull] byte[] data,
                int offset,
                int length,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(data, nameof(data));
            Sure.NonNegative(offset, nameof(offset));
            Sure.NonNegative(length, nameof(length));
            Sure.NotNull(encoding, nameof(encoding));

            if (length > data.Length)
            {
                length = data.Length;
            }

            _data = data;
            Position = offset;
            Length = length;
            Encoding = encoding;
        }

        #endregion

        #region Private members

        private readonly byte[] _data;

        #endregion

        #region Public methods

        /// <summary>
        /// Clone the navigator.
        /// </summary>
        [NotNull]
        public ByteNavigator Clone()
        {
            ByteNavigator result = new ByteNavigator
                (
                    _data,
                    Length
                )
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
        public static ByteNavigator FromFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            byte[] data = File.ReadAllBytes(fileName);
            ByteNavigator result = new ByteNavigator(data);

            return result;
        }

        /// <summary>
        /// Выдать остаток данных.
        /// </summary>
        [CanBeNull]
        public byte[] GetRemainingData()
        {
            if (IsEOF)
            {
                return null;
            }

            byte[] result = _data.GetSpan(Position);

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
                : _data[Position];
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

            char result = (char)_data[Position];

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

            int result = _data[Position];
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

            char result = (char)_data[Position];
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
        [CanBeNull]
        public string ReadLine()
        {
            if (IsEOF)
            {
                return null;
            }

            StringBuilder result = new StringBuilder();

            while (!IsEOF)
            {
                char c = PeekChar();
                if (c == '\r' || c == '\n')
                {
                    break;
                }
                c = ReadChar();
                result.Append(c);
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

            return result.ToString();
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
