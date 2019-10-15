// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisStopWords.cs -- STW file handling
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Linq;

using AM;
using AM.Collections;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

#endregion

namespace ManagedIrbis
{
    //
    // STW file example:
    //
    // A
    // ABOUT
    // AFTER
    // AGAINST
    // ALL
    // ALS
    // AN
    // AND
    // AS
    // AT
    // AUF
    // AUS
    // AUX
    // B
    // BIJ
    // BY
    //

    /// <summary>
    /// STW file handling.
    /// </summary>
    [PublicAPI]
    public sealed class IrbisStopWords
    {
        #region Properties

        /// <summary>
        /// File name (for identification only).
        /// </summary>
        public string? FileName { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public IrbisStopWords()
        {
            _dictionary = new CaseInsensitiveDictionary<object?>();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="IrbisStopWords"/> class.
        /// </summary>
        /// <param name="fileName">The name.</param>
        public IrbisStopWords
            (
                string? fileName
            )
            : this()
        {
            FileName = fileName;
        }

        #endregion

        #region Private members

        private readonly CaseInsensitiveDictionary<object?> _dictionary;

        #endregion

        #region Public methods

//        /// <summary>
//        /// Load stopword list from server.
//        /// </summary>
//        public static IrbisStopWords FromServer
//            (
//                IrbisConnection connection
//            )
//        {
//            string database = connection.Database
//                .ThrowIfNull(Resources.DatabaseNotSet);
//            string fileName = database + ".stw";
//
//            return FromServer(connection, database, fileName);
//        }

//        /// <summary>
//        /// Load stopword list from server.
//        /// </summary>
//        public static IrbisStopWords FromServer
//            (
//                IrbisConnection connection,
//                string database,
//                string fileName
//            )
//        {
//            Sure.NotNullNorEmpty(database, nameof(database));
//            Sure.NotNullNorEmpty(fileName, nameof(fileName));
//
//            FileSpecification specification = new FileSpecification
//                (
//                    path: IrbisPath.MasterFile,
//                    database: database,
//                    fileName: fileName
//                );
//
//            string? text = connection
//                .ReadTextFileAsync(specification.ToString()).Result;
//            if (string.IsNullOrEmpty(text))
//            {
//                text = string.Empty;
//            }
//
//            IrbisStopWords result = ParseText(fileName, text);
//
//            return result;
//        }

        /// <summary>
        /// Is given word is stopword?
        /// </summary>
        public bool IsStopWord
            (
                string? word
            )
        {
            if (string.IsNullOrEmpty(word))
            {
                return true;
            }

            word = word.Trim();
            if (string.IsNullOrEmpty(word))
            {
                return true;
            }

            return _dictionary.ContainsKey(word);
        }

        /// <summary>
        /// Parse array of plain text lines.
        /// </summary>
        public static IrbisStopWords ParseLines
            (
                string? name,
                [ItemNotNull] string[] lines
            )
        {
            var result = new IrbisStopWords(name);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    result._dictionary[trimmed] = null;
                }
            }

            return result;
        }

//        /// <summary>
//        /// Parse plain text.
//        /// </summary>
//        public static IrbisStopWords ParseText
//            (
//                string? name,
//                string text
//            )
//        {
//            string[] lines = text.SplitLines();
//
//            return ParseLines(name, lines);
//        }

        /// <summary>
        /// Parse the text file.
        /// </summary>
        public static IrbisStopWords ParseFile
            (
                string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            var name = Path.GetFileNameWithoutExtension(fileName);
            var lines = File.ReadAllLines
                (
                    path: fileName,
                    encoding: IrbisEncoding.Ansi
                );

            return ParseLines(name, lines);
        }

        /// <summary>
        /// Convert <see cref="IrbisStopWords"/> to array
        /// of text lines.
        /// </summary>
        [ItemNotNull]
        public string[] ToLines()
        {
            var result = _dictionary.Keys.ToArray();
            Array.Sort(result);

            return result;
        }

        /// <summary>
        /// Convert <see cref="IrbisStopWords"/> to plain text.
        /// </summary>
        public string ToText()
        {
            return string.Join
                (
                    separator: Environment.NewLine,
                    value: ToLines()
                );
        }

        #endregion
    }
}

