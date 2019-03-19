// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisClientQuery.cs -- client packet with query to the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Клиентский запрос.
    /// </summary>
    public sealed class ClientQuery
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClientQuery
            (
                [NotNull] IrbisConnection connection,
                [NotNull] string commandCode
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            _chunks = new List<byte[]>();
            AddAnsi(string.Empty);

            var header = commandCode + "\n"
                + connection.Workstation + "\n"
                + commandCode + "\n"
                + connection.ClientId.ToInvariantString() + "\n"
                + connection.QueryId.ToInvariantString() + "\n"
                + connection.Password + "\n"
                + connection.Username + "\n"
                + "\n\n\n";
            AddAnsi(header);
        }

        #endregion

        #region Private members

        private static readonly byte[] _newLine = {10};

        private readonly List<byte[]> _chunks;

        #endregion

        #region Public methods

        /// <summary>
        /// Добавление целого числа.
        /// </summary>
        public ClientQuery Add(int value)
        {
            return AddAnsi(value.ToInvariantString());
        }

        /// <summary>
        /// Добавление текста в кодировке ANSI.
        /// </summary>
        public ClientQuery AddAnsi<T>(T value)
        {
            if (ReferenceEquals(value, null))
            {
                return this;
            }

            byte[] converted = IrbisEncoding.Ansi.GetBytes(value.ToString());
            _chunks.Add(converted);

            return this;
        }

        /// <summary>
        /// Добавление текста в кодировке ANSI.
        /// </summary>
        public ClientQuery AddUtf<T>(T value)
        {
            if (ReferenceEquals(value, null))
            {
                return this;
            }

            byte[] converted = IrbisEncoding.Utf8.GetBytes(value.ToString());
            _chunks.Add(converted);

            return this;
        }

        /// <summary>
        /// Debug print.
        /// </summary>
        public void Debug(TextWriter writer)
        {
            foreach (var memory in _chunks)
            {
                foreach (var b in memory)
                {
                    writer.Write($" {b:X2}");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[][] GetChunks()
        {
            return _chunks.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetLength()
        {
            int result = 0;

            foreach (var chunk in _chunks)
            {
                result += chunk.Length;
            }

            return result;
        }

        /// <summary>
        /// Перевод строки.
        /// </summary>
        public ClientQuery NewLine()
        {
            _chunks.Add(_newLine);

            return this;
        }

        #endregion
    }
}
