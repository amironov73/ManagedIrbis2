﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SerializationUtility.cs -- serialization related helper methods
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.IO.Compression;

using AM.IO;

using JetBrains.Annotations;

#endregion

namespace AM.Runtime
{
    /// <summary>
    /// Serialization related helper methods.
    /// </summary>
    [PublicAPI]
    public static class SerializationUtility
    {
        #region Public methods

        /// <summary>
        /// Reads array from the stream.
        /// </summary>
        [ItemNotNull]
        public static T[] RestoreArray<T>
            (
                this BinaryReader reader
            )
            where T : IHandmadeSerializable, new()
        {
            var count = reader.ReadPackedInt32();
            var result = new T[count];
            for (int i = 0; i < count; i++)
            {
                var item = new T();
                item.RestoreFromStream(reader);
                result[i] = item;
            }

            return result;
        }

        /// <summary>
        /// Read array from the file.
        /// </summary>
        [ItemNotNull]
        public static T[] RestoreArrayFromFile<T>
            (
                string fileName
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.FileExists(fileName, nameof(fileName));

            using var stream = File.OpenRead(fileName);
            using var reader = new BinaryReader(stream);

            return reader.RestoreArray<T>();
        }

        /// <summary>
        /// Read array from the file.
        /// </summary>
        [ItemNotNull]
        public static T[] RestoreArrayFromZipFile<T>
            (
                string fileName
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.FileExists(fileName, nameof(fileName));

            using (Stream stream = File.OpenRead(fileName))
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Decompress
                ))
            using (BinaryReader reader = new BinaryReader(deflate))
            {
                return reader.RestoreArray<T>();
            }
        }

        /// <summary>
        /// Считывание массива из памяти.
        /// </summary>
        public static T[] RestoreArrayFromMemory<T>
            (
                [NotNull] this byte[] array
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.NotNull(array, nameof(array));

            using (Stream stream = new MemoryStream(array))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return reader.ReadArray<T>();
            }
        }

        /// <summary>
        /// Считывание массива из памяти.
        /// </summary>
        public static T[] RestoreArrayFromZipMemory<T>
            (
                [NotNull] this byte[] array
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.NotNull(array, nameof(array));

            using (Stream stream = new MemoryStream(array))
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Decompress
                ))
            using (BinaryReader reader = new BinaryReader(deflate))
            {
                return reader.ReadArray<T>();
            }
        }

        /// <summary>
        /// Read nullable object from the stream.
        /// </summary>
        public static T? RestoreNullable<T>
            (
                this BinaryReader reader
            )
            where T : class, IHandmadeSerializable, new()
        {
            var isNull = !reader.ReadBoolean();
            if (isNull)
            {
                return null;
            }

            var result = new T();
            result.RestoreFromStream(reader);

            return result;
        }

        /// <summary>
        /// Read non-nullable object from the file.
        /// </summary>
        public static T RestoreObjectFromFile<T>
            (
                string fileName
            )
            where T : IHandmadeSerializable, new()
        {
            using var stream = File.OpenRead(fileName);
            using var reader = new BinaryReader(stream);
            var result = new T();
            result.RestoreFromStream(reader);

            return result;
        }


        /// <summary>
        /// Считывание объекта из файла.
        /// </summary>
        [NotNull]
        public static T RestoreObjectFromZipFile<T>
            (
                [NotNull] string fileName
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.FileExists(fileName, nameof(fileName));

            using (Stream stream = File.OpenRead(fileName))
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Decompress
                ))
            using (BinaryReader reader = new BinaryReader(deflate))
            {
                T result = new T();
                result.RestoreFromStream(reader);

                return result;
            }
        }

        /// <summary>
        /// Read nullable object from the memory.
        /// </summary>
        public static T? RestoreObjectFromMemory<T>
            (
                this byte[] array
            )
            where T : class, IHandmadeSerializable, new()
        {
            using var stream = new MemoryStream(array);
            using var reader = new BinaryReader(stream);

            return reader.RestoreNullable<T>();
        }

        /// <summary>
        /// Считывание объекта из строки.
        /// </summary>
        public static T RestoreObjectFromString<T>
            (
                this string text
            )
            where T : class, IHandmadeSerializable, new()
        {
            var bytes = Convert.FromBase64String(text);
            var result = bytes.RestoreObjectFromZipMemory<T>();

            return result;
        }

        /// <summary>
        /// Считывание объекта из памяти.
        /// </summary>
        public static T RestoreObjectFromZipMemory<T>
            (
                [NotNull] this byte[] array
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(array, nameof(array));

            using (Stream stream = new MemoryStream(array))
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Decompress
                ))
            using (BinaryReader reader = new BinaryReader(deflate))
            {
                T result = new T();
                result.RestoreFromStream(reader);

                return result;
            }
        }

        /// <summary>
        /// Сохранение в поток массива элементов.
        /// </summary>
        public static void SaveToStream<T>
            (
                [NotNull][ItemNotNull] this T[] array,
                [NotNull] BinaryWriter writer
            )
            where T : IHandmadeSerializable
        {
            Sure.NotNull(array, nameof(array));
            Sure.NotNull(writer, nameof(writer));

            writer.WritePackedInt32(array.Length);
            foreach (T item in array)
            {
                item.SaveToStream(writer);
            }
        }

        /// <summary>
        /// Сохранение в файл объекта,
        /// умеющего сериализоваться вручную.
        /// </summary>
        public static void SaveToFile<T>
            (
                [NotNull] this T obj,
                [NotNull] string fileName
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(obj, nameof(obj));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using (Stream stream = File.Create(fileName))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                obj.SaveToStream(writer);
            }
        }

        /// <summary>
        /// Сохранение в файл объекта,
        /// умеющего сериализоваться вручную.
        /// </summary>
        public static void SaveToZipFile<T>
            (
                [NotNull] this T obj,
                [NotNull] string fileName
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(obj, nameof(obj));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using (Stream stream = File.Create(fileName))
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Compress
                ))
            using (BinaryWriter writer = new BinaryWriter(deflate))
            {
                obj.SaveToStream(writer);
            }
        }

        /// <summary>
        /// Сохранение в файл массива объектов,
        /// умеющих сериализоваться вручную.
        /// </summary>
        public static void SaveToFile<T>
            (
                [NotNull] [ItemNotNull] this T[] array,
                [NotNull] string fileName
            )
            where T : IHandmadeSerializable
        {
            Sure.NotNull(array, nameof(array));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using (Stream stream = File.Create(fileName))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                array.SaveToStream(writer);
            }
        }

        /// <summary>
        /// Сохранение объекта.
        /// </summary>
        public static byte[] SaveToMemory<T>
            (
                [NotNull] this T obj
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(obj, nameof(obj));

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.WriteNullable(obj);
                }
                return stream.ToArray();
            }
        }


        /// <summary>
        /// Сохранение массива объектов.
        /// </summary>
        public static byte[] SaveToMemory<T>
            (
                [NotNull][ItemNotNull] this T[] array
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.NotNull(array, nameof(array));

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.WriteArray(array);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Сохранение объекта в строке.
        /// </summary>
        public static string SaveToString<T>
            (
                [NotNull] this T obj
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(obj, nameof(obj));

            byte[] bytes = obj.SaveToZipMemory();
            string result = Convert.ToBase64String(bytes);

            return result;
        }

        /// <summary>
        /// Сохранение в файл массива объектов
        /// с одновременной упаковкой.
        /// </summary>
        public static void SaveToZipFile<T>
            (
                [NotNull] [ItemNotNull] this T[] array,
                [NotNull] string fileName
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.NotNull(array, nameof(array));
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            using (Stream stream = File.Create(fileName))
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Compress
                ))
            using (BinaryWriter writer = new BinaryWriter(deflate))
            {
                writer.WriteArray(array);
            }
        }

        /// <summary>
        /// Сохранение массива объектов.
        /// </summary>
        public static byte[] SaveToZipMemory<T>
            (
                [NotNull] this T obj
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(obj, nameof(obj));

            using (MemoryStream stream = new MemoryStream())
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Compress
                ))
            {
                using (BinaryWriter writer = new BinaryWriter(deflate))
                {
                    obj.SaveToStream(writer);
                    //writer.WriteNullable(obj);
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Сохранение массива объектов.
        /// </summary>
        public static byte[] SaveToZipMemory<T>
            (
                [NotNull][ItemNotNull] this T[] array
            )
            where T : IHandmadeSerializable, new()
        {
            Sure.NotNull(array, nameof(array));

            using (MemoryStream stream = new MemoryStream())
            using (DeflateStream deflate = new DeflateStream
                (
                    stream,
                    CompressionMode.Compress
                ))
            {
                using (BinaryWriter writer = new BinaryWriter(deflate))
                {
                    writer.WriteArray(array);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Сохранение в поток обнуляемого объекта.
        /// </summary>
        public static BinaryWriter WriteNullable<T>
            (
                [NotNull] this BinaryWriter writer,
                [CanBeNull] T obj
            )
            where T : class, IHandmadeSerializable, new()
        {
            Sure.NotNull(writer, nameof(writer));

            if (ReferenceEquals(obj, null))
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                obj.SaveToStream(writer);
            }

            return writer;
        }

        #endregion
    }
}
