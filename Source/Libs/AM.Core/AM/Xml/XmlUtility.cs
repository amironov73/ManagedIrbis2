// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* XmlUtility.cs -- useful routines for XML manipulations
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using JetBrains.Annotations;

#endregion

namespace AM.Xml
{
    /// <summary>
    /// Collection of useful routines for XML manipulations.
    /// </summary>
    [PublicAPI]
    public static class XmlUtility
    {
        #region Public methods

        /// <summary>
        /// Deserialize object from file.
        /// </summary>
        public static T Deserialize<T>
            (
                [NotNull] string fileName
            )
        {
            Sure.FileExists(fileName, nameof(fileName));

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (Stream stream = File.OpenRead(fileName))
            {
                return (T) serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Deserialize the string.
        /// </summary>
        public static T DeserializeString<T>
            (
                [NotNull] string xmlText
            )
        {
            Sure.NotNullNorEmpty(xmlText, "xmlText");

            StringReader reader = new StringReader(xmlText);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        /// Serialize object to file.
        /// </summary>
        public static void Serialize<T>
            (
                [NotNull] string fileName,
                T obj
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using (StreamWriter output = new StreamWriter(fileName))
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj, namespaces);
            }
        }

        /// <summary>
        /// Serialize to string without standard
        /// XML header and namespaces.
        /// </summary>
        [NotNull]
        public static string SerializeShort
            (
                [NotNull] object obj
            )
        {
            Sure.NotNull(obj, "obj");

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
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj, namespaces);
            }

            return output.ToString();
        }

        #endregion
    }
}
