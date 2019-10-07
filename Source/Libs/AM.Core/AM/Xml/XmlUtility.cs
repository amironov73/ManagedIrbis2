// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* XmlUtility.cs -- useful routines for XML manipulations
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
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
                string fileName
            )
        {
            Sure.FileExists(fileName, nameof(fileName));

            var serializer = new XmlSerializer(typeof(T));
            using Stream stream = File.OpenRead(fileName);

            return (T) serializer.Deserialize(stream);
        }

        /// <summary>
        /// Deserialize the string.
        /// </summary>
        public static T DeserializeString<T>
            (
                string xmlText
            )
        {
            Sure.NotNullNorEmpty(xmlText, nameof(xmlText));

            var reader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        /// Serialize object to file.
        /// </summary>
        public static void Serialize<T>
            (
                string fileName,
                T obj
            )
            where T: class
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using var output = new StreamWriter(fileName);
            using var writer = XmlWriter.Create(output, settings);
            var objType = obj.GetType();
            var serializer = new XmlSerializer(objType);
            serializer.Serialize(writer, obj, namespaces);
        }

        /// <summary>
        /// Serialize to string without standard
        /// XML header and namespaces.
        /// </summary>
        public static string SerializeShort
            (
                object obj
            )
        {
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false,
                NewLineHandling = NewLineHandling.None
            };
            var output = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using (var writer = XmlWriter.Create(output, settings))
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj, namespaces);
            }

            return output.ToString();
        }

        #endregion
    }
}
