﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StreamUtility.cs -- stream manipulation routines.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System;
using System.IO;
using System.Net;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo

namespace AM.IO
{
    /// <summary>
    /// Stream manipulation routines.
    /// </summary>
    [PublicAPI]
    public static class StreamUtility
    {
        #region Public methods

        /// <summary>
        /// Appends one's stream contents (starting from current position)
        /// to another stream.
        /// </summary>
        public static unsafe void AppendTo
            (
                Stream sourceStream,
                Stream destinationStream,
                int chunkSize = 0
            )
        {
            if (chunkSize <= 0)
            {
                chunkSize = 4 * 1024;
            }

            var buffer = stackalloc byte[chunkSize];
            var span = new Span<byte>(buffer, chunkSize);
            destinationStream.Seek(0, SeekOrigin.End);
            while (true)
            {
                var read = sourceStream.Read(span);
                if (read <= 0)
                {
                    break;
                }

                destinationStream.Write(span.Slice(0, read));
            }
        }

        /// <summary>
        /// Compares two <see cref="Stream"/>'s from current position.
        /// </summary>
        public static unsafe int CompareTo
            (
                Stream firstStream,
                Stream secondStream
            )
        {
            const int bufferSize = 1024;
            var firstArray = stackalloc byte[bufferSize];
            var firstBuffer = new Span<byte>(firstArray, bufferSize);
            var secondArray = stackalloc byte[bufferSize];
            var secondBuffer = new Span<byte>(secondArray, bufferSize);
            while (true)
            {
                var firstRead = firstStream.Read(firstBuffer);
                var secondRead = secondStream.Read(secondBuffer);
                var difference = firstRead - secondRead;
                if (difference != 0)
                {
                    return difference;
                }

                if (firstRead == 0)
                {
                    return 0;
                }

                for (var i = 0; i < firstRead; i++)
                {
                    difference = firstBuffer[i] - secondBuffer[i];
                    if (difference != 0)
                    {
                        return difference;
                    }
                }
            }
        }

        /// <summary>
        /// Reads <see cref="Boolean"/> value from the <see cref="Stream"/>.
        /// </summary>
        public static bool ReadBoolean
            (
                Stream stream
            )
        {
            return stream.ReadByte() switch
            {
                0 => false,
                -1 => throw new IOException(),
                _ => true
            };
        }

        /// <summary>
        /// Read some bytes from the stream.
        /// </summary>
        public static byte[]? ReadBytes
            (
                Stream stream,
                int count
            )
        {
            Sure.Positive(count, nameof(count));

            var result = new byte[count];
            var read = stream.Read(result, 0, count);
            if (read <= 0)
            {
                return null;
            }

            if (read != count)
            {
                Array.Resize(ref result, read);
            }

            return result;
        }

        /// <summary>
        /// Reads <see cref="Int16"/> value from the <see cref="Stream"/>.
        /// </summary>
        public static unsafe short ReadInt16
            (
                Stream stream
            )
        {
            short value = default;
            var span = new Span<byte>((byte*)&value, sizeof(short));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="UInt16"/> value from the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe ushort ReadUInt16
            (
                Stream stream
            )
        {
            ushort value = default;
            var span = new Span<byte>((byte*)&value, sizeof(ushort));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="Int32"/> value from the <see cref="Stream"/>.
        /// </summary>
        public static unsafe int ReadInt32
            (
                Stream stream
            )
        {
            int value = default;
            var span = new Span<byte>((byte*)&value, sizeof(int));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="UInt32"/> value from the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe uint ReadUInt32
            (
                Stream stream
            )
        {
            uint value = default;
            var span = new Span<byte>((byte*)&value, sizeof(uint));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="Int64"/> value from the <see cref="Stream"/>.
        /// </summary>
        public static unsafe long ReadInt64
            (
                Stream stream
            )
        {
            long value = default;
            var span = new Span<byte>((byte*)&value, sizeof(long));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="UInt64"/> value from the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe ulong ReadUInt64
            (
                Stream stream
            )
        {
            ulong value = default;
            var span = new Span<byte>((byte*)&value, sizeof(ulong));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="Single"/> value from the <see cref="Stream"/>.
        /// </summary>
        public static unsafe float ReadSingle
            (
                Stream stream
            )
        {
            float value = default;
            var span = new Span<byte>((byte*)&value, sizeof(float));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="Double"/> value from the <see cref="Stream"/>.
        /// </summary>
        public static unsafe double ReadDouble
            (
                Stream stream
            )
        {
            double value = default;
            var span = new Span<byte>((byte*)&value, sizeof(double));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads <see cref="String"/> value from the <see cref="Stream"/>
        /// using specified <see cref="Encoding"/>.
        /// </summary>
        /// <seealso cref="Write(Stream,string,Encoding)"/>
        public static string ReadString
            (
                Stream stream,
                Encoding encoding
            )
        {
            var count = ReadInt32(stream);
            var bytes = ReadExact(stream, count);
            var result = encoding.GetString(bytes);

            return result;
        }

        /// <summary>
        /// Reads <see cref="Boolean"/> value from the <see cref="Stream"/>
        /// using UTF-8 <see cref="Encoding"/>.
        /// </summary>
        public static string ReadString
            (
                Stream stream
            )
        {
            return ReadString(stream, Encoding.UTF8);
        }

        /// <summary>
        /// Reads array of <see cref="Int16"/> values from the
        /// <see cref="Stream"/>.
        /// </summary>
        public static short[] ReadInt16Array
            (
                Stream stream
            )
        {
            var length = ReadInt32(stream);
            var result = new short[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadInt16(stream);
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="UInt16"/> values from the
        /// <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static ushort[] ReadUInt16Array
            (
                Stream stream
            )
        {
            var length = ReadInt32(stream);
            var result = new ushort[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadUInt16(stream);
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="Int32"/> values from the
        /// <see cref="Stream"/>.
        /// </summary>
        public static int[] ReadInt32Array
            (
                Stream stream
            )
        {
            var length = ReadInt32(stream);
            var result = new int[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadInt32(stream);
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="UInt32"/> values from the
        /// <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static uint[] ReadUInt32Array
            (
                Stream stream
            )
        {
            var length = ReadInt32(stream);
            var result = new uint[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadUInt32(stream);
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="String"/>'s from the given stream until the end
        /// of the stream using specified <see cref="Encoding"/>.
        /// </summary>
        public static string[] ReadStringArray
            (
                Stream stream,
                Encoding encoding
            )
        {
            var length = ReadInt32(stream);
            var result = new string[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadString(stream, encoding);
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="String"/>'s from the <see cref="Stream"/>
        /// using UTF-8 <see cref="Encoding"/>.
        /// </summary>
        public static string[] ReadStringArray
            (
                Stream stream
            )
        {
            return ReadStringArray(stream, Encoding.UTF8);
        }

        /// <summary>
        /// Reads the <see cref="Decimal"/> from the specified
        /// <see cref="Stream"/>.
        /// </summary>
        public static unsafe decimal ReadDecimal
            (
                Stream stream
            )
        {
            decimal value = default;
            var span = new Span<byte>((byte*)&value, sizeof(decimal));
            ReadExact(stream, span);
            return value;
        }

        /// <summary>
        /// Reads the date time.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public static DateTime ReadDateTime
            (
                Stream stream
            )
        {
            var binary = ReadInt64(stream);

            return DateTime.FromBinary(binary);
        }

        /// <summary>
        /// Чтение точного числа байт.
        /// </summary>
        public static byte[] ReadExact
            (
                Stream stream,
                int length
            )
        {
            var buffer = new byte[length];
            if (length != 0 &&
                stream.Read(buffer, 0, length) != length)
            {
                Log.Error
                    (
                        "StreamUtility::_Read: "
                        + "unexpected end of stream"
                    );

                throw new IOException("Unexpected end of stream");
            }

            return buffer;
        }

        /// <summary>
        /// Чтение точного числа байт.
        /// </summary>
        public static void ReadExact
            (
                Stream stream,
                Span<byte> span
            )
        {
            if (!span.IsEmpty &&
                stream.Read(span) != span.Length)
            {
                Log.Error
                    (
                        "StreamUtility::_Read: "
                        + "unexpected end of stream"
                    );

                throw new IOException("Unexpected end of stream");
            }
        }

        /// <summary>
        /// Writes the <see cref="Boolean"/> value to the <see cref="Stream"/>.
        /// </summary>
        public static void Write
            (
                Stream stream,
                bool value
            )
        {
            stream.WriteByte
                (
                    value
                    ? (byte)1
                    : (byte)0
                );
        }

        /// <summary>
        /// Writes the <see cref="Int16"/> value to the <see cref="Stream"/>.
        /// </summary>
        public static unsafe void Write
            (
                Stream stream,
                short value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(short));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="UInt16"/> value to the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void Write
            (
                Stream stream,
                ushort value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(ushort));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="Int32"/> to the <see cref="Stream"/>.
        /// </summary>
        public static unsafe void Write
            (
                Stream stream,
                int value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(int));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="UInt32"/> to the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void Write
            (
                Stream stream,
                uint value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(uint));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="Int64"/> to the <see cref="Stream"/>.
        /// </summary>
        public static unsafe void Write
            (
                Stream stream,
                long value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(long));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="UInt64"/> to the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void Write
            (
                Stream stream,
                ulong value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(ulong));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="Single"/> to the <see cref="Stream"/>.
        /// </summary>
        public static unsafe void Write
            (
                Stream stream,
                float value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(float));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="Double"/> to the <see cref="Stream"/>.
        /// </summary>
        public static unsafe void Write
            (
                Stream stream,
                double value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(double));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="String"/> to the <see cref="Stream"/>
        /// using specified <see cref="Encoding"/>.
        /// </summary>
        public static void Write
            (
                Stream stream,
                string value,
                Encoding encoding
            )
        {
            var bytes = encoding.GetBytes(value);
            Write(stream, bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes the <see cref="String"/> to the <see cref="Stream"/>
        /// using UTF-8 <see cref="Encoding"/>.
        /// </summary>
        public static void Write
            (
                Stream stream,
                string value
            )
        {
            Write(stream, value, Encoding.UTF8);
        }

        /// <summary>
        /// Writes the array of <see cref="Int16"/> to the <see cref="Stream"/>.
        /// </summary>
        public static void Write
            (
                Stream stream,
                short[] values
            )
        {
            Write(stream, values.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                Write(stream, values[i]);
            }
        }

        /// <summary>
        /// Writes the array of <see cref="UInt16"/> to the <see cref="Stream"/>.
        /// </summary>
        [CLSCompliant(false)]
        public static void Write
            (
                Stream stream,
                ushort[] values
            )
        {
            Write(stream, values.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                Write(stream, values[i]);
            }
        }

        /// <summary>
        /// Writes the array of <see cref="Int32"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of signed integer numbers.</param>
        /// <remarks>Value can be readed with
        /// <see cref="ReadInt32Array"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <see cref="Write(Stream,uint[])"/>
        /// <see cref="ReadInt32Array"/>
        public static void Write
            (
                Stream stream,
                int[] values
            )
        {
            Write(stream, values.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                Write(stream, values[i]);
            }
        }

        /// <summary>
        /// Writes the array of <see cref="UInt32"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of unsigned integer numbers.</param>
        /// <remarks>Value can be readed with
        /// <see cref="ReadUInt32Array"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,int[])"/>
        /// <see cref="ReadUInt32Array"/>
        [CLSCompliant(false)]
        public static void Write
            (
                Stream stream,
                uint[] values
            )
        {
            Write(stream, values.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                Write(stream, values[i]);
            }
        }

        /// <summary>
        /// Writes the array of <see cref="String"/> to the <see cref="Stream"/>
        /// using specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of strings to write.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <remarks>Value can be readed with
        /// <see cref="ReadStringArray(Stream,Encoding)"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,string[])"/>
        /// <see cref="ReadStringArray(Stream,Encoding)"/>
        public static void Write
            (
                Stream stream,
                string[] values,
                Encoding encoding
            )
        {
            Write(stream, values.Length);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                Write(stream, values[i], encoding);
            }
        }

        /// <summary>
        /// Writes the array of <see cref="String"/> to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of strings to write.</param>
        /// <remarks>Value can be readed with
        /// <see cref="ReadStringArray(Stream)"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,string[],Encoding)"/>
        /// <seealso cref="ReadStringArray(Stream)"/>
        public static void Write
            (
                Stream stream,
                string[] values
            )
        {
            Write(stream, values, Encoding.UTF8);
        }

        /// <summary>
        /// Writes the <see cref="Decimal"/> to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Value can be readed with <see cref="ReadDecimal"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">Error during stream output
        /// happens.</exception>
        /// <seealso cref="ReadDecimal"/>
        public static unsafe void Write
            (
                Stream stream,
                decimal value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(decimal));
            stream.Write(span);
        }

        /// <summary>
        /// Writes the <see cref="DateTime"/> to the specified
        /// <see cref="Stream"/>.
        /// </summary>
        public static unsafe void Write
            (
                Stream stream,
                DateTime value
            )
        {
            var span = new Span<byte>((byte*)&value, sizeof(decimal));
            stream.Write(span);
        }

        /// <summary>
        /// Network to host byte conversion.
        /// </summary>
        public static void NetworkToHost16
            (
                byte[] array,
                int offset
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp = array[offset];
                array[offset] = array[offset + 1];
                array[offset + 1] = temp;
            }
        }

        /// <summary>
        /// Network to host byte conversion.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void NetworkToHost16
            (
                byte* ptr
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp = *ptr;
                *ptr = ptr[1];
                ptr[1] = temp;
            }
        }

        /// <summary>
        /// Network to host byte conversion.
        /// </summary>
        public static void NetworkToHost32
            (
                byte[] array,
                int offset
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp1 = array[offset];
                var temp2 = array[offset + 1];
                array[offset] = array[offset + 3];
                array[offset + 1] = array[offset + 2];
                array[offset + 3] = temp1;
                array[offset + 2] = temp2;
            }
        }

        /// <summary>
        /// Network to host byte conversion.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void NetworkToHost32
            (
                byte* ptr
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp1 = *ptr;
                var temp2 = ptr[1];
                *ptr = ptr[3];
                ptr[1] = ptr[2];
                ptr[3] = temp1;
                ptr[2] = temp2;
            }
        }

        /// <summary>
        /// Network to host byte conversion.
        /// </summary>
        /// <remarks>IRBIS64-oriented!</remarks>
        public static void NetworkToHost64
            (
                byte[] array,
                int offset
            )
        {
            NetworkToHost32(array, offset);
            NetworkToHost32(array, offset + 4);
        }

        /// <summary>
        /// Network to host byte conversion.
        /// </summary>
        /// <remarks>IRBIS64-oriented!</remarks>
        [CLSCompliant(false)]
        public static unsafe void NetworkToHost64
            (
                byte* ptr
            )
        {
            NetworkToHost32(ptr);
            NetworkToHost32(ptr + 4);
        }

        /// <summary>
        /// Host to network byte conversion.
        /// </summary>
        public static void HostToNetwork16
            (
                byte[] array,
                int offset
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp = array[offset];
                array[offset] = array[offset + 1];
                array[offset + 1] = temp;
            }
        }

        /// <summary>
        /// Host to network byte conversion.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void HostToNetwork16
            (
                byte* ptr
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp = *ptr;
                *ptr = ptr[1];
                ptr[1] = temp;
            }

        }

        /// <summary>
        /// Host to network byte conversion.
        /// </summary>
        public static void HostToNetwork32
            (
                byte[] array,
                int offset
            )
        {
            var temp1 = array[offset];
            var temp2 = array[offset + 1];
            array[offset] = array[offset + 3];
            array[offset + 1] = array[offset + 2];
            array[offset + 3] = temp1;
            array[offset + 2] = temp2;
        }

        /// <summary>
        /// Host to network byte conversion.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void HostToNetwork32
            (
                byte* ptr
            )
        {
            if (BitConverter.IsLittleEndian)
            {
                var temp1 = *ptr;
                var temp2 = ptr[1];
                *ptr = ptr[3];
                ptr[1] = ptr[2];
                ptr[3] = temp1;
                ptr[2] = temp2;
            }
        }

        /// <summary>
        /// Host to network byte conversion.
        /// </summary>
        /// <remarks>IRBIS64-oriented!</remarks>
        public static void HostToNetwork64
            (
                byte[] array,
                int offset
            )
        {
            HostToNetwork32(array, offset);
            HostToNetwork32(array, offset + 4);
        }

        /// <summary>
        /// Host to network byte conversion.
        /// </summary>
        /// <remarks>IRBIS64-oriented!</remarks>
        [CLSCompliant(false)]
        public static unsafe void HostToNetwork64
            (
                byte* ptr
            )
        {
            HostToNetwork32(ptr);
            HostToNetwork32(ptr + 4);
        }

        /// <summary>
        /// Read integer in network byte order.
        /// </summary>
        public static unsafe short ReadInt16Network
            (
                this Stream stream
            )
        {
            var buffer = stackalloc byte[sizeof(short)];
            var span = new Span<byte>(buffer, sizeof(short));
            ReadExact(stream, span);
            return IPAddress.NetworkToHostOrder(*(short*) buffer);
        }

        /// <summary>
        /// Read integer in host byte order.
        /// </summary>
        public static unsafe short ReadInt16Host
            (
                this Stream stream
            )
        {
            var buffer = stackalloc byte[sizeof(short)];
            var span = new Span<byte>(buffer, sizeof(short));
            ReadExact(stream, span);
            return *(short*) buffer;
        }

        /// <summary>
        /// Read integer in network byte order.
        /// </summary>
        public static unsafe int ReadInt32Network
            (
                this Stream stream
            )
        {
            var buffer = stackalloc byte[sizeof(int)];
            var span = new Span<byte>(buffer, sizeof(int));
            ReadExact(stream, span);
            return IPAddress.NetworkToHostOrder(*(int*) buffer);
        }

        /// <summary>
        /// Read integer in host byte order.
        /// </summary>
        public static unsafe int ReadInt32Host
            (
                this Stream stream
            )
        {
            var buffer = stackalloc byte[sizeof(int)];
            var span = new Span<byte>(buffer, sizeof(int));
            ReadExact(stream, span);
            return *(int*) buffer;
        }

        /// <summary>
        /// Read integer in network byte order.
        /// </summary>
        public static unsafe long ReadInt64Network
            (
                this Stream stream
            )
        {
            var buffer = stackalloc byte[sizeof(long)];
            var span = new Span<byte>(buffer, sizeof(long));
            ReadExact(stream, span);
            NetworkToHost64(buffer);
            return *(long*) buffer;
        }

        /// <summary>
        /// Read integer in host byte order.
        /// </summary>
        public static unsafe long ReadInt64Host
            (
                this Stream stream
            )
        {
            var buffer = stackalloc byte[sizeof(long)];
            var span = new Span<byte>(buffer, sizeof(long));
            ReadExact(stream, span);
            return *(long*) buffer;
        }

        /// <summary>
        /// Считывает из потока максимально возможное число байт.
        /// </summary>
        /// <remarks>Полезно для считывания из сети (сервер высылает
        /// ответ, после чего закрывает соединение).</remarks>
        /// <param name="stream">Поток для чтения.</param>
        /// <returns>Массив считанных байт.</returns>
        public static byte[] ReadToEnd
            (
                this Stream stream
            )
        {
            var result = new MemoryStream(); //-V3114

            while (true)
            {
                var buffer = new byte[50 * 1024];
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    break;
                }
                result.Write(buffer, 0, read);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Write 16-bit integer to the stream in network byte order.
        /// </summary>
        public static unsafe void WriteInt16Network
            (
                this Stream stream,
                short value
            )
        {
            value = IPAddress.HostToNetworkOrder(value);
            var ptr = (byte*)&value;
            var span = new Span<byte>(ptr, sizeof(short));
            stream.Write(span);
        }

        /// <summary>
        /// Write 16-bit integer to the stream in network byte order.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void WriteUInt16Network
            (
                this Stream stream,
                ushort value
            )
        {
            unchecked
            {
                value = (ushort) IPAddress.HostToNetworkOrder((short) value);
                var ptr = (byte*) &value;
                var span = new Span<byte>(ptr, sizeof(ushort));
                stream.Write(span);
            }
        }

        /// <summary>
        /// Write 32-bit integer to the stream in network byte order.
        /// </summary>
        public static unsafe void WriteInt32Network
            (
                this Stream stream,
                int value
            )
        {
            value = IPAddress.HostToNetworkOrder(value);
            var ptr = (byte*)&value;
            var span = new Span<byte>(ptr, sizeof(int));
            stream.Write(span);
        }

        /// <summary>
        /// Write 32-bit integer to the stream in network byte order.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void WriteUInt32Network
            (
                this Stream stream,
                uint value
            )
        {
            unchecked
            {
                value = (uint) IPAddress.HostToNetworkOrder((int)value);
                var ptr = (byte*) &value;
                var span = new Span<byte>(ptr, sizeof(uint));
                stream.Write(span);
            }
        }

        /// <summary>
        /// Write 64-bit integer to the stream in network byte order.
        /// </summary>
        public static unsafe void WriteInt64Network
            (
                this Stream stream,
                long value
            )
        {
            var ptr = (byte*)&value;
            HostToNetwork64(ptr);
            var span = new Span<byte>(ptr, sizeof(long));
            stream.Write(span);
        }

        /// <summary>
        /// Write 64-bit integer to the stream in network byte order.
        /// </summary>
        [CLSCompliant(false)]
        public static unsafe void WriteUInt64Network
            (
                this Stream stream,
                ulong value
            )
        {
            var ptr = (byte*)&value;
            HostToNetwork64(ptr);
            var span = new Span<byte>(ptr, sizeof(ulong));
            stream.Write(span);
        }

        #endregion
    }
}
