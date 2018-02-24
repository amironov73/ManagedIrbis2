﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* JsonUtility.cs -- helper routines for JSON
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace AM.Json
{
    /// <summary>
    /// Helper routines for JSON.
    /// </summary>
    [PublicAPI]
    public static class JsonUtility
    {
        #region Public methods

        /// <summary>
        /// Expand $type's.
        /// </summary>
        public static void ExpandTypes
            (
                [NotNull] JObject obj,
                [NotNull] string nameSpace,
                [NotNull] string assembly
            )
        {
            Sure.NotNull(obj, "obj");
            Sure.NotNullNorEmpty(nameSpace, "nameSpace");
            Sure.NotNullNorEmpty(assembly, "assembly");

#if !WINMOBILE && !PocketPC

            IEnumerable<JValue> values = obj
                .SelectTokens("$..$type")
                .OfType<JValue>();
            foreach (JValue value in values)
            {
                string typeName = value.Value.ToString();
                if (!typeName.Contains('.'))
                {
                    typeName = nameSpace
                               + "."
                               + typeName
                               + ", "
                               + assembly;
                    value.Value = typeName;
                }
            }

#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Include
            (
                [NotNull] JObject obj,
                [NotNull] Action<JProperty> resolver
            )
        {
            Sure.NotNull(obj, "obj");
            Sure.NotNull(resolver, "resolver");

#if !WINMOBILE && !PocketPC

            JValue[] values = obj
                .SelectTokens("$..$include")
                .OfType<JValue>()
                .ToArray();

            foreach (JValue value in values)
            {
                JProperty property = (JProperty) value.Parent;
                resolver(property);
            }

#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Include
            (
                [NotNull] JObject obj
            )
        {
            Sure.NotNull(obj, "obj");

#if !WINMOBILE && !PocketPC

            JToken[] tokens = obj
                .SelectTokens("$..$include")
                .ToArray();

            foreach (JToken token in tokens)
            {
                JProperty property = (JProperty)token.Parent;
                Resolve(property);
            }

#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Include
            (
                [NotNull] JObject obj,
                [NotNull] string newName
            )
        {
            Sure.NotNull(obj, "obj");
            Sure.NotNullNorEmpty(newName, "newName");

            Action<JProperty> resolver = prop =>
            {
                Resolve(prop, newName);
            };
            Include(obj, resolver);
        }

        /// <summary>
        /// Read <see cref="JArray"/> from specified
        /// local file.
        /// </summary>
        [NotNull]
        public static JArray ReadArrayFromFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, "fileName");

#if WINMOBILE || PocketPC

            throw new NotImplementedException();

#else

            string text = File.ReadAllText(fileName);
            JArray result = JArray.Parse(text);

            return result;

#endif
        }

        /// <summary>
        /// Read <see cref="JObject"/> from specified
        /// local JSON file.
        /// </summary>
        [NotNull]
        public static JObject ReadObjectFromFile
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, "fileName");

#if WINMOBILE || PocketPC

            throw new NotImplementedException();

#else

            string text = File.ReadAllText(fileName);
            JObject result = JObject.Parse(text);

            return result;

#endif
        }

        /// <summary>
        /// Read arbitrary object from specified
        /// local JSON file.
        /// </summary>
        [NotNull]
        public static T ReadObjectFromFile<T>
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, "fileName");

#if WINMOBILE || PocketPC

            throw new NotImplementedException();

#else

            string text = File.ReadAllText(fileName);
            T result = JsonConvert.DeserializeObject<T>(text);

            return result;

#endif
        }

        /// <summary>
        /// Save the <see cref="JArray"/>
        /// to the specified local file.
        /// </summary>
        public static void SaveArrayToFile
            (
                [NotNull] JArray array,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(array, "array");
            Sure.NotNullNorEmpty(fileName, "fileName");

#if !WINMOBILE && !PocketPC

            string text = array.ToString(Formatting.Indented);
            File.WriteAllText(fileName, text);

#endif
        }

        /// <summary>
        /// Save object to the specified local JSON file.
        /// </summary>
        public static void SaveObjectToFile
            (
                [NotNull] JObject obj,
                [NotNull] string fileName
            )
        {
            Sure.NotNull(obj, "obj");
            Sure.NotNullNorEmpty(fileName, "fileName");

#if !WINMOBILE && !PocketPC

            string text = obj.ToString(Formatting.Indented);
            File.WriteAllText(fileName, text);

#endif
        }

        /// <summary>
        /// Save object to the specified local JSON file.
        /// </summary>
        public static void SaveObjectToFile
            (
                [NotNull] object obj,
                [NotNull] string fileName
            )
        {
#if !WINMOBILE && !PocketPC

            JObject json = JObject.FromObject(obj);

            SaveObjectToFile(json, fileName);

#endif
        }

        /// <summary>
        /// Resolver for <see cref="Include(JObject,Action{JProperty})"/>.
        /// </summary>
        public static void Resolve
            (
                [NotNull] JProperty property,
                [NotNull] string newName
            )
        {
            Sure.NotNull(property, "property");
            Sure.NotNull(newName, "newName");

#if !WINMOBILE && !PocketPC

            // TODO use path for searching

            string fileName = property.Value.ToString();
            string text = File.ReadAllText(fileName);
            JObject value = JObject.Parse(text);
            JProperty newProperty = new JProperty(newName, value);
            property.Replace(newProperty);

#endif
        }

        /// <summary>
        /// Resolver for <see cref="Include(JObject,Action{JProperty})"/>.
        /// </summary>
        public static void Resolve
            (
                [NotNull] JProperty property
            )
        {
            Sure.NotNull(property, "property");

#if !WINMOBILE && !PocketPC

            // TODO use path for searching

            JObject obj = (JObject) property.Value;
            string newName = obj["name"].Value<string>();
            string fileName = obj["file"].Value<string>();
            string text = File.ReadAllText(fileName);
            JObject value = JObject.Parse(text);
            JProperty newProperty = new JProperty(newName, value);
            property.Replace(newProperty);

#endif
        }

            /// <summary>
            /// Serialize to short string.
            /// </summary>
        [NotNull]
        public static string SerializeShort
            (
                [NotNull] object obj
            )
        {
            Sure.NotNull(obj, "obj");

#if WINMOBILE || PocketPC

            throw new NotImplementedException();

#else

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            StringWriter textWriter = new StringWriter();
            JsonWriter jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.None,
                QuoteChar = '\''
            };
            serializer.Serialize(jsonWriter, obj);

            return textWriter.ToString();

#endif
        }

        #endregion
    }
}
