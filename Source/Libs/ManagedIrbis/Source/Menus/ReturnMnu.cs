// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReturnMnu.cs -- wrapper for RETURN.MNU
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.Client;
using ManagedIrbis.Infrastructure;

#endregion

namespace ManagedIrbis.Menus
{
    /// <summary>
    /// Wrapper for RETURN.MNU.
    /// </summary>
    [PublicAPI]
    public sealed class ReturnMnu
    {
        #region Constants

        /// <summary>
        /// Default file name.
        /// </summary>
        public const string DefaultFileName = "RETURN.MNU";

        #endregion

        #region Nested classes

        /// <summary>
        /// Item.
        /// </summary>
        public sealed class Item
        {
            #region Properties

            /// <summary>
            /// Date.
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Comment.
            /// </summary>
            [CanBeNull]
            public string Comment { get; set; }

            #endregion

            #region Construction

            /// <summary>
            /// Constructor.
            /// </summary>
            public Item()
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            public Item
                (
                    [NotNull] MenuEntry entry
                )
            {
                Sure.NotNull(entry, nameof(entry));

                string code = entry.Code.ThrowIfNull(nameof(entry.Code));
                Comment = entry.Comment;
                if (code.StartsWith("@"))
                {
                    Date = DateTime.ParseExact
                        (
                            code.Substring(1),
                            "dd.MM.yyyy",
                            CultureInfo.InvariantCulture
                        );
                }
                else
                {
                    Date = DateTime.Today.AddDays(NumericUtility.ParseInt32(code));
                }
            }

            #endregion

            #region Object members

            /// <inheritdoc cref="object.ToString" />
            public override string ToString()
            {
                return $"{Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)} {Comment}";
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// Items.
        /// </summary>
        [NotNull]
        public List<Item> Items { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReturnMnu
            (
                [NotNull] MenuFile menu
            )
        {
            Sure.NotNull(menu, nameof(menu));

            Items = new List<Item>();
            foreach (MenuEntry entry in menu.Entries)
            {
                Items.Add(new Item(entry));
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Read RETURN.MNU from server connection.
        /// </summary>
        [NotNull]
        public static ReturnMnu FromConnection
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string fileName = DefaultFileName
            )
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            FileSpecification specification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    StandardDatabases.Readers
                );
            MenuFile menu = MenuFile.ReadFromServer(connection, specification)
                .ThrowIfNull(nameof(menu));
            ReturnMnu result = new ReturnMnu(menu);

            return result;
        }

        /// <summary>
        /// Read RETURN.MNU from the local file.
        /// </summary>
        [NotNull]
        public static ReturnMnu FromFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            MenuFile menu = MenuFile.ParseLocalFile(fileName);
            ReturnMnu result = new ReturnMnu(menu);

            return result;
        }

        /// <summary>
        /// Read RETURN.MNU from the provider.
        /// </summary>
        [NotNull]
        public static ReturnMnu FromProvider
            (
                [NotNull] IrbisProvider provider,
                [NotNull] string fileName = DefaultFileName
            )
        {
            Sure.NotNull(provider, nameof(provider));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            FileSpecification specification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    StandardDatabases.Readers,
                    fileName
                );
            MenuFile menu = provider.ReadMenuFile(specification)
                .ThrowIfNull(nameof(menu));
            ReturnMnu result = new ReturnMnu(menu);

            return result;
        }

        #endregion
    }
}
