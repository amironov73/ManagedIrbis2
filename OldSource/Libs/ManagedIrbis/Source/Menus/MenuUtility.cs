// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MenuUtility.cs -- MNU file extended handling.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using AM;
using AM.Collections;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Formatting = Newtonsoft.Json.Formatting;

#endregion

namespace ManagedIrbis.Menus
{
    /// <summary>
    /// MNU file extended handling.
    /// </summary>
    [PublicAPI]
    public static class MenuUtility
    {
        #region Private members

        private static IrbisTreeFile.Item _BuildItem
            (
                [NotNull] MenuEntry parent,
                [NotNull] MenuFile menu
            )
        {
            string value = string.Format
                (
                    "{0}{1}{2}",
                    parent.Code,
                    IrbisTreeFile.Item.Delimiter,
                    parent.Comment
                );
            IrbisTreeFile.Item result = new IrbisTreeFile.Item(value);
            MenuEntry[] children = menu.Entries
                .Where(v => ReferenceEquals(v.OtherEntry, parent))
                .OrderBy(v => v.Code)
                .ToArray();

            foreach (MenuEntry child in children)
            {
                IrbisTreeFile.Item subItem = _BuildItem(child, menu);
                result.Children.Add(subItem);
            }

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds the typed value with specified code.
        /// </summary>
        public static MenuFile Add<T>
            (
                [NotNull] this MenuFile menu,
                [NotNull] string code,
                [CanBeNull] T value
            )
        {
            Sure.NotNull(menu, nameof(menu));
            Sure.NotNull(code, nameof(code));

            if (ReferenceEquals(value, null))
            {
                menu.Add(code, string.Empty);
            }
            else
            {
                string textValue = ConversionUtility.ConvertTo<string>(value);
                menu.Add(code, textValue);
            }

            return menu;
        }

        /// <summary>
        /// Collects the comments for code.
        /// </summary>
        [NotNull]
        public static string[] CollectStrings
            (
            [NotNull] this MenuFile menu,
            [NotNull] string code
            )
        {
            return menu.Entries
                .Where
                    (
                        entry => entry.Code.SameString(code)
                                 || MenuFile.TrimCode(entry.Code.ThrowIfNull("entry.Code"))
                                     .SameString(code)
                    )
                .Select(entry => entry.Comment)
                .ToArray();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [CanBeNull]
        public static T GetValue<T>
            (
                [NotNull] this MenuFile menu,
                [NotNull] string code,
                [CanBeNull] T defaultValue
            )
        {
            Sure.NotNull(menu, nameof(menu));
            Sure.NotNull(code, nameof(code));

            MenuEntry found = menu.FindEntry(code);

            return found == null
                ? defaultValue
                : ConversionUtility.ConvertTo<T>(found.Comment);
        }

        /// <summary>
        /// Gets the value (case sensitive).
        /// </summary>
        [CanBeNull]
        public static T GetValueSensitive<T>
            (
                [NotNull] this MenuFile menu,
                [NotNull] string code,
                T defaultValue
            )
        {
            Sure.NotNull(menu, nameof(menu));
            Sure.NotNull(code, nameof(code));

            MenuEntry found = menu.FindEntrySensitive(code);

            return found == null
                ? defaultValue
                : ConversionUtility.ConvertTo<T>(found.Comment);
        }

        /// <summary>
        /// Converts the menu to JSON.
        /// </summary>
        [NotNull]
        public static string ToJson
            (
                [NotNull] this MenuFile menu
            )
        {
            Sure.NotNull(menu, nameof(menu));

            string result = JArray.FromObject(menu.Entries)
                .ToString(Formatting.None);

            return result;
        }

        /// <summary>
        /// Restores the menu from JSON.
        /// </summary>
        [NotNull]
        public static MenuFile FromJson
            (
                [NotNull] string text
            )
        {
            Sure.NotNullNorEmpty(text, nameof(text));

            NonNullCollection<MenuEntry> entries = JsonConvert
                .DeserializeObject<NonNullCollection<MenuEntry>>
                    (
                        text
                    );
            MenuFile result = new MenuFile(entries);

            return result;
        }

        /// <summary>
        /// Saves the menu to local JSON file.
        /// </summary>
        public static void SaveLocalJsonFile
            (
                [NotNull] this MenuFile menu,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(menu, nameof(menu));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            string contents = JArray.FromObject(menu.Entries)
                .ToString(Formatting.Indented);

            File.WriteAllText
                (
                    fileName, 
                    contents, 
                    IrbisEncoding.Utf8
                );
        }

        /// <summary>
        /// Parses the local json file.
        /// </summary>
        [NotNull]
        public static MenuFile ParseLocalJsonFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            string text = File.ReadAllText
                (
                    fileName,
                    IrbisEncoding.Utf8
                );
            MenuFile result = FromJson(text);

            return result;
        }

        /// <summary>
        /// Convert MNU to TRE.
        /// </summary>
        [NotNull]
        public static IrbisTreeFile ToTree
            (
                [NotNull] this MenuFile menu
            )
        {
            Sure.NotNull(menu, nameof(menu));

            foreach (MenuEntry entry in menu.Entries)
            {
                entry.Code = entry.Code.ThrowIfNull("entryCode").Trim();
            }

            foreach (MenuEntry first in menu.Entries)
            {
                foreach (MenuEntry second in menu.Entries)
                {
                    if (ReferenceEquals(first, second))
                    {
                        continue;
                    }
                    if (first.Code.SameString(second.Code))
                    {
                        continue;
                    }

                    if (first.Code.StartsWith(second.Code))
                    {
                        if (ReferenceEquals(first.OtherEntry, null))
                        {
                            first.OtherEntry = second;
                        }
                        else
                        {
                            if (second.Code.Length
                                > first.OtherEntry.Code.Length)
                            {
                                first.OtherEntry = second;
                            }
                        }
                    }
                }
            }

            MenuEntry[] roots = menu.Entries
                .Where(entry => ReferenceEquals(entry.OtherEntry, null))
                .OrderBy(entry => entry.Code)
                .ToArray();

            IrbisTreeFile result = new IrbisTreeFile();
            foreach (MenuEntry root in roots)
            {
                IrbisTreeFile.Item item = _BuildItem(root, menu);
                result.Roots.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Converts the menu to XML.
        /// </summary>
        [NotNull]
        public static string ToXml
            (
                [NotNull] this MenuFile menu
            )
        {
            Sure.NotNull(menu, nameof(menu));

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false,
                NewLineHandling = NewLineHandling.None
            };
            StringBuilder output = new StringBuilder();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MenuFile));
                serializer.Serialize(writer, menu, namespaces);
            }

            return output.ToString();
        }

        #endregion
    }
}
