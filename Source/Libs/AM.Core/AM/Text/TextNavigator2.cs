// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TextNavigator.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.Text
{
    /// <summary>
    /// Навигатор по тексту.
    /// </summary>
    [PublicAPI]
    public sealed class TextNavigator2
    {
        #region Constants

        /// <summary>
        /// Признак конца текста.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const char EOF = '\0';

        #endregion

        #region Properties

        /// <summary>
        /// Текущая колонка текста. Нумерация с 1.
        /// </summary>
        public int Column => _column;

        /// <summary>
        /// Текст закончился?
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool IsEOF => _position >= _length;

        /// <summary>
        /// Длина текста.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Текущая строка текста. Нумерация с 1.
        /// </summary>
        public int Line => _line;

        /// <summary>
        /// Текущая позиция.
        /// </summary>
        public int Position => _position;

        ///// <summary>
        ///// Обрабатываемый текст.
        ///// </summary>
        //[NotNull]
        //public string Text => _text;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextNavigator2
            (
                Memory<char> text
            )
        {
            _text = text;
            _position = 0;
            _length = _text.Length;
            _line = 1;
            _column = 1;
        }

        #endregion

        #region Private members

        private Memory<char> _text;

        private int _position, _length, _line, _column;

        #endregion

        #region Public methods

        /// <summary>
        /// Clone the navigator.
        /// </summary>
        [Pure]
        public TextNavigator2 Clone()
        {
            TextNavigator2 result = new TextNavigator2(_text)
            {
                _column = _column,
                _length = _length,
                _line = _line,
                _position = _position
            };

            return result;
        }

        /// <summary>
        /// Навигатор по текстовому файлу.
        /// </summary>
        [NotNull]
        public static TextNavigator FromFile
            (
                [NotNull] string fileName,
                Encoding encoding = null
            )
        {
            Sure.FileExists(fileName, nameof(fileName));

            encoding = encoding ?? Encoding.UTF8;

            string text = File.ReadAllText(fileName, encoding);
            TextNavigator result = new TextNavigator(text);

            return result;
        }

        /// <summary>
        /// Выдать остаток текста.
        /// </summary>
        /// <returns><c>null</c>, если достигнут конец текста.
        /// </returns>
        [Pure]
        public Memory<char> GetRemainingText()
        {
            Memory<char> result = _text.Slice(_position, _length - _position);

            return result;
        }

        /// <summary>
        /// Управляющий символ?
        /// </summary>
        [Pure]
        public bool IsControl()
        {
            char c = PeekChar();

            return char.IsControl(c);
        }

        /// <summary>
        /// Цифра?
        /// </summary>
        [Pure]
        public bool IsDigit()
        {
            char c = PeekChar();

            return char.IsDigit(c);
        }

        /// <summary>
        /// Буква?
        /// </summary>
        [Pure]
        public bool IsLetter()
        {
            char c = PeekChar();

            return char.IsLetter(c);
        }

        /// <summary>
        /// Буква или цифра?
        /// </summary>
        [Pure]
        public bool IsLetterOrDigit()
        {
            char c = PeekChar();

            return char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Часть числа?
        /// </summary>
        [Pure]
        public bool IsNumber()
        {
            char c = PeekChar();

            return char.IsNumber(c);
        }

        /// <summary>
        /// Знак пунктуации?
        /// </summary>
        [Pure]
        public bool IsPunctuation()
        {
            char c = PeekChar();

            return char.IsPunctuation(c);
        }

        /// <summary>
        /// Разделитель?
        /// </summary>
        [Pure]
        public bool IsSeparator()
        {
            char c = PeekChar();

            return char.IsSeparator(c);
        }

        /// <summary>
        /// Символ?
        /// </summary>
        [Pure]
        public bool IsSymbol()
        {
            char c = PeekChar();

            return char.IsSymbol(c);
        }

        /// <summary>
        /// Пробельный символ?
        /// </summary>
        [Pure]
        public bool IsWhiteSpace()
        {
            char c = PeekChar();

            return char.IsWhiteSpace(c);
        }

        /// <summary>
        /// Заглядывание вперёд на 1 позицию.
        /// </summary>
        /// <remarks>Это на 1 позицию дальше,
        /// чем <see cref="PeekChar()"/>
        /// </remarks>
        [Pure]
        public char LookAhead()
        {
            int newPosition = _position + 1;
            if (newPosition >= _length)
            {
                return EOF;
            }

            return _text.Span[newPosition];
        }

        /// <summary>
        /// Заглядывание вперёд.
        /// </summary>
        [Pure]
        public char LookAhead
            (
                int distance
            )
        {
            Sure.NonNegative(distance, nameof(distance));

            int newPosition = _position + distance;
            if (newPosition >= _length)
            {
                return EOF;
            }

            return _text.Span[newPosition];
        }

        /// <summary>
        /// Заглядывание назад.
        /// </summary>
        [Pure]
        public char LookBehind()
        {
            if (_position == 0)
            {
                return EOF;
            }

            return _text.Span[_position - 1];
        }

        /// <summary>
        /// Заглядывание назад.
        /// </summary>
        [Pure]
        public char LookBehind
            (
                int distance
            )
        {
            Sure.Positive(distance, nameof(distance));

            if (_position < distance)
            {
                return EOF;
            }

            return _text.Span[_position - distance];
        }

        /// <summary>
        /// Смещение указателя.
        /// </summary>
        [NotNull]
        public TextNavigator2 Move
            (
                int distance
            )
        {
            // TODO Some checks

            _position += distance;
            _column += distance;

            return this;
        }

        /// <summary>
        /// Подглядывание текущего символа.
        /// </summary>
        [Pure]
        public char PeekChar()
        {
            if (_position >= _length)
            {
                return EOF;
            }

            return _text.Span[_position];
        }

        /// <summary>
        /// Подглядывание строки вплоть до указанной длины.
        /// </summary>
        public Memory<char> PeekString
            (
                int length
            )
        {
            Sure.Positive(length, nameof(length));

            int savePosition = _position, saveColumn = _column,
                saveLine = _line;
            int start = _position;
            for (int i = 0; i < length; i++)
            {
                char c = ReadChar();
                if (c == EOF)
                {
                    break;
                }
            }

            Memory<char> result = _text.Slice(start, _position - start);
            _position = savePosition;
            _column = saveColumn;
            _line = saveLine;

            return result;
        }

        /// <summary>
        /// Подглядывание вплоть до указанного символа
        /// (включая его).
        /// </summary>
        public Memory<char> PeekTo
            (
                char stopChar
            )
        {
            int savePosition = _position, saveColumn = _column,
                saveLine = _line;

            Memory<char> result = ReadTo(stopChar);

            _position = savePosition;
            _column = saveColumn;
            _line = saveLine;

            return result;
        }

        /// <summary>
        /// Подглядывание вплоть до указанных символов
        /// (включая их).
        /// </summary>
        public Memory<char> PeekTo
            (
                char[] stopChars
            )
        {
            int savePosition = _position, saveColumn = _column,
                saveLine = _line;

            Memory<char> result = ReadTo(stopChars);

            _position = savePosition;
            _column = saveColumn;
            _line = saveLine;

            return result;
        }

        /// <summary>
        /// Подглядывание вплоть до указанного символа.
        /// </summary>
        public Memory<char> PeekUntil
            (
                char stopChar
            )
        {
            int savePosition = _position, saveColumn = _column,
                saveLine = _line;

            Memory<char> result = ReadUntil(stopChar);

            _position = savePosition;
            _column = saveColumn;
            _line = saveLine;

            return result;
        }

        /// <summary>
        /// Подглядывание вплоть до указанных символов.
        /// </summary>
        public Memory<char> PeekUntil
            (
                char[] stopChars
            )
        {
            int savePosition = _position, saveColumn = _column,
                saveLine = _line;

            Memory<char> result = ReadUntil(stopChars);

            _position = savePosition;
            _column = saveColumn;
            _line = saveLine;

            return result;
        }

        /// <summary>
        /// Считывание символа.
        /// </summary>
        public char ReadChar()
        {
            if (_position >= _length)
            {
                return EOF;
            }

            char result = _text.Span[_position];
            _position++;
            if (result == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }

            return result;
        }

        /// <summary>
        /// Считывание экранированной строки вплоть до разделителя
        /// (не включая его).
        /// </summary>
        public string ReadEscapedUntil
            (
                char escapeChar,
                char stopChar
            )
        {
            StringBuilder result = new StringBuilder();
            while (true)
            {
                char c = ReadChar();
                if (c == EOF)
                {
                    break;
                }

                if (c == escapeChar)
                {
                    c = ReadChar();
                    if (c == EOF)
                    {
                        Log.Error
                            (
                                nameof(TextNavigator)
                                + "::"
                                + nameof(ReadEscapedUntil)
                                + ": "
                                + "unexpected end of stream"
                            );

                        throw new FormatException();
                    }
                    result.Append(c);
                }
                else if (c == stopChar)
                {
                    break;
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Считывание начиная с открывающего символа
        /// до закрывающего (включая их).
        /// </summary>
        public Memory<char> ReadFrom
            (
                char openChar,
                char closeChar
            )
        {
            if (IsEOF)
            {
                return null;
            }

            int savePosition = _position;

            char c = PeekChar();
            if (c != openChar)
            {
                return Memory<char>.Empty;
            }
            ReadChar();

            while (true)
            {
                c = ReadChar();
                if (c == EOF)
                {
                    return Memory<char>.Empty;
                }
                if (c == closeChar)
                {
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Считывание начиная с открывающего символа
        /// до закрывающего (включая их).
        /// </summary>
        public Memory<char> ReadFrom
            (
                char[] openChars,
                char[] closeChars
            )
        {
            int savePosition = _position;

            char c = PeekChar();
            if (Array.IndexOf(openChars, c) < 0)
            {
                return Memory<char>.Empty;
            }
            ReadChar();

            while (true)
            {
                c = ReadChar();
                if (c == EOF)
                {
                    return Memory<char>.Empty;
                }
                if (Array.IndexOf(closeChars, c) >= 0)
                {
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Чтение беззнакового целого.
        /// </summary>
        public Memory<char> ReadInteger()
        {
            if (!IsDigit())
            {
                return Memory<char>.Empty;
            }

            int savePosition = _position;

            while (IsDigit())
            {
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Чтение до конца строки.
        /// </summary>
        public Memory<char> ReadLine()
        {
            int start = _position;

            while (!IsEOF)
            {
                char c = PeekChar();
                if (c == '\r' || c == '\n')
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: start,
                    length: _position - start
                );

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

            return result;
        }

        /// <summary>
        /// Чтение строки вплоть до указанной длины.
        /// </summary>
        public Memory<char> ReadString
            (
                int length
            )
        {
            Sure.Positive(length, nameof(length));

            int start = _position;
            for (int i = 0; i < length; i++)
            {
                char c = ReadChar();
                if (c == EOF)
                {
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    start: start,
                    length: _position - start
                );

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанного символа
        /// (включая его).
        /// </summary>
        public Memory<char> ReadTo
            (
                char stopChar
            )
        {
            int start = _position;
            while (true)
            {
                char c = ReadChar();
                if (c == EOF || c == stopChar)
                {
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    start: start,
                    length: _position - start
                );

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанного разделителя
        /// (разделитель не помещается в возвращаемое значение,
        /// однако, считывается).
        /// </summary>
        public Memory<char> ReadTo
            (
                [NotNull] string stopString
            )
        {
            Sure.NotNullNorEmpty(stopString, nameof(stopString));

            int savePosition = _position;
            int length = 0;
            var span = _text.Span;

            while (true)
            {
            AGAIN:
                char c = ReadChar();
                if (c == EOF)
                {
                    _position = savePosition;
                    return null;
                }

                length++;
                if (length >= stopString.Length)
                {
                    int start = _position - stopString.Length;
                    for (int i = 0; i < stopString.Length; i++)
                    {
                        if (span[start + i] != stopString[i])
                        {
                            goto AGAIN;
                        }
                    }
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    savePosition,
                    _position - savePosition - stopString.Length
                );

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанного символа
        /// (включая один из них).
        /// </summary>
        public Memory<char> ReadTo
            (
                params char[] stopChars
            )
        {
            int savePosition = _position;

            while (true)
            {
                char c = ReadChar();
                if (c == EOF
                    || Array.IndexOf(stopChars, c) >= 0)
                {
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанного символа
        /// (не включая его).
        /// </summary>
        public Memory<char> ReadUntil
            (
                char stopChar
            )
        {
            int savePosition = _position;

            while (true)
            {
                char c = PeekChar();
                if (c == EOF || c == stopChar)
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанного разделителя
        /// (разделитель не помещается в возвращаемое значение
        /// и не считывается).
        /// </summary>
        public Memory<char> ReadUntil
            (
                [NotNull] string stopString
            )
        {
            Sure.NotNullNorEmpty(stopString, nameof(stopString));

            int savePosition = _position;
            int length = 0;
            var span = _text.Span;

            while (true)
            {
            AGAIN:
                char c = ReadChar();
                if (c == EOF)
                {
                    _position = savePosition;

                    return null;
                }

                length++;
                if (length >= stopString.Length)
                {
                    int start = _position - stopString.Length;
                    for (int i = 0; i < stopString.Length; i++)
                    {
                        if (span[start + i] != stopString[i])
                        {
                            goto AGAIN;
                        }
                    }
                    break;
                }
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition - stopString.Length
                );
            _position -= stopString.Length;

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанных символов
        /// (не включая их).
        /// </summary>
        public Memory<char> ReadUntil
            (
                params char[] stopChars
            )
        {
            int savePosition = _position;

            while (true)
            {
                char c = PeekChar();
                if (c == EOF
                    || Array.IndexOf(stopChars, c) >= 0)
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Считывание вплоть до указанных символов
        /// (не включая их).
        /// </summary>
        public Memory<char> ReadUntil
            (
                [NotNull] char[] openChars,
                [NotNull] char[] closeChars,
                [NotNull] char[] stopChars
            )
        {
            int savePosition = _position;
            int level = 0;

            while (true)
            {
                char c = PeekChar();

                if (c == EOF)
                {
                    _position = savePosition;
                    return null;
                }

                if (c.OneOf(openChars))
                {
                    level++;
                }
                else if (c.OneOf(closeChars))
                {
                    if (level == 0
                        && c.OneOf(stopChars))
                    {
                        break;
                    }
                    level--;
                }
                else if (c.OneOf(stopChars))
                {
                    if (level == 0)
                    {
                        break;
                    }
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    savePosition,
                    _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Считывание, пока встречается указанный символ.
        /// </summary>
        public Memory<char> ReadWhile
            (
                char goodChar
            )
        {
            int savePosition = _position;

            while (true)
            {
                char c = PeekChar();
                if (c == EOF || c != goodChar)
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Считывание, пока встречается указанные символы.
        /// </summary>
        public Memory<char> ReadWhile
            (
                params char[] goodChars
            )
        {
            int savePosition = _position;

            while (true)
            {
                char c = PeekChar();
                if (c == EOF
                    || Array.IndexOf(goodChars, c) < 0)
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Read word.
        /// </summary>
        public Memory<char> ReadWord()
        {
            int savePosition = _position;

            while (true)
            {
                char c = PeekChar();
                if (c == EOF
                    || !char.IsLetterOrDigit(c))
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    start: savePosition,
                    length: _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Read word.
        /// </summary>
        public Memory<char> ReadWord
            (
                params char[] additionalWordCharacters
            )
        {
            int savePosition = _position;

            while (true)
            {
                char c = PeekChar();
                if (c == EOF
                    || !char.IsLetterOrDigit(c)
                        && Array.IndexOf(additionalWordCharacters, c) < 0)
                {
                    break;
                }
                ReadChar();
            }

            Memory<char> result = _text.Slice
                (
                    savePosition,
                    _position - savePosition
                );

            return result;
        }

        /// <summary>
        /// Get recent text.
        /// </summary>
        [Pure]
        public Memory<char> RecentText
            (
                int length
            )
        {
            int start = _position - length;
            if (start < 0)
            {
                length += start;
                start = 0;
            }
            if (start >= _length)
            {
                length = 0;
                start = _length - 1;
            }
            if (length < 0)
            {
                length = 0;
            }

            return _text.Slice(start, length);
        }

        /// <summary>
        /// Restore previously saved position.
        /// </summary>
        public void RestorePosition
            (
                TextPosition saved
            )
        {
            _column = saved.Column;
            _line = saved.Line;
            _position = saved.Position;
        }

        /// <summary>
        /// Save current position.
        /// </summary>
        [Pure]
        public TextPosition SavePosition()
        {
            return new TextPosition(this);
        }

        /// <summary>
        /// Пропускает один символ, если он совпадает с указанным.
        /// </summary>
        /// <returns><c>true</c>, если символ был съеден успешно
        /// </returns>
        public bool SkipChar
            (
                char c
            )
        {
            if (PeekChar() == c)
            {
                ReadChar();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Пропускает указанное число символов.
        /// </summary>
        public bool SkipChar
            (
                int n
            )
        {
            for (int i = 0; i < n; i++)
            {
                ReadChar();
            }

            return !IsEOF;
        }

        /// <summary>
        /// Пропускает один символ, если он совпадает с любым
        /// из указанных.
        /// </summary>
        /// <returns><c>true</c>, если символ был съеден успешно
        /// </returns>
        public bool SkipChar
            (
                params char[] allowed
            )
        {
            if (Array.IndexOf(allowed, PeekChar()) >= 0)
            {
                ReadChar();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Пропускаем управляющие символы.
        /// </summary>
        public bool SkipControl()
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                if (IsControl())
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропускаем пунктуацию.
        /// </summary>
        public bool SkipPunctuation()
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                if (IsPunctuation())
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Skip non-word characters.
        /// </summary>
        public bool SkipNonWord()
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (!char.IsLetterOrDigit(c))
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Skip non-word characters.
        /// </summary>
        public bool SkipNonWord
            (
                params char[] additionalWordCharacters
            )
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (!char.IsLetterOrDigit(c)
                    && Array.LastIndexOf(additionalWordCharacters, c) < 0)
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропускаем произвольное количество символов
        /// из указанного диапазона.
        /// </summary>
        public bool SkipRange
            (
                char fromChar,
                char toChar
            )
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (c >= fromChar && c <= toChar)
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропустить указанный символ.
        /// </summary>
        public bool SkipWhile
            (
                char skipChar
            )
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (c == skipChar)
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропустить указанные символы.
        /// </summary>
        public bool SkipWhile
            (
                params char[] skipChars
            )
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (Array.IndexOf(skipChars, c) >= 0)
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропустить, пока не встретится указанный символ.
        /// Сам символ не считывается.
        /// </summary>
        public bool SkipTo
            (
                char stopChar
            )
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (c == stopChar)
                {
                    return true;
                }

                ReadChar();
            }
        }

        /// <summary>
        /// Пропустить, пока не встретятся указанные символы.
        /// </summary>
        public bool SkipWhileNot
            (
                params char[] goodChars
            )
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                char c = PeekChar();
                if (Array.IndexOf(goodChars, c) < 0)
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропускаем пробельные символы.
        /// </summary>
        public bool SkipWhitespace()
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                if (IsWhiteSpace())
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Пропускаем пробельные символы и пунктуацию.
        /// </summary>
        public bool SkipWhitespaceAndPunctuation()
        {
            while (true)
            {
                if (IsEOF)
                {
                    return false;
                }

                if (IsWhiteSpace() || IsPunctuation())
                {
                    ReadChar();
                }
                else
                {
                    return true;
                }
            }
        }

        ///// <summary>
        ///// Split text by given good characters.
        ///// </summary>
        //[NotNull]
        //[ItemNotNull]
        //public string[] SplitByGoodCharacters
        //    (
        //        params char[] goodCharacters
        //    )
        //{
        //    List<string> result = new List<string>();

        //    while (!IsEOF)
        //    {
        //        if (SkipWhileNot(goodCharacters))
        //        {
        //            string word = ReadWhile(goodCharacters);
        //            if (!string.IsNullOrEmpty(word))
        //            {
        //                result.Add(word);
        //            }
        //        }
        //    }

        //    return result.ToArray();
        //}

        ///// <summary>
        ///// Split the remaining text to array of words.
        ///// </summary>
        //[NotNull]
        //[ItemNotNull]
        //public string[] SplitToWords()
        //{
        //    List<string> result = new List<string>();

        //    while (true)
        //    {
        //        if (!SkipNonWord())
        //        {
        //            break;
        //        }

        //        string word = ReadWord();
        //        result.Add(word);
        //    }

        //    return result.ToArray();
        //}

        ///// <summary>
        ///// Split the remaining text to array of words.
        ///// </summary>
        //[NotNull]
        //[ItemNotNull]
        //public string[] SplitToWords
        //    (
        //        params char[] additionalWordCharacters
        //    )
        //{
        //    List<string> result = new List<string>();

        //    while (true)
        //    {
        //        if (!SkipNonWord(additionalWordCharacters))
        //        {
        //            break;
        //        }

        //        string word = ReadWord(additionalWordCharacters);
        //        result.Add(word);
        //    }

        //    return result.ToArray();
        //}

        /// <summary>
        /// Get substring.
        /// </summary>
        [Pure]
        public Memory<char> Substring
            (
                int offset,
                int length
            )
        {
            Memory<char> result = _text.Slice(offset, length);

            return result;
        }

        #endregion

    }
}
