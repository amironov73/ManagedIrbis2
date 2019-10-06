// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CompressionUtility.cs -- useful routines that simplifies data compression
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.IO.Compression;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Useful routines that simplifies data compression/decompression.
    /// </summary>
    [PublicAPI]
    public static class CompressionUtility
    {
        #region Public methods

        /// <summary>
        /// Compress the data.
        /// </summary>
        public static byte[] Compress
            (
                byte[] data
            )
        {
            var memory = new MemoryStream();
            using (var compressor = new DeflateStream
                (
                    memory,
                    CompressionMode.Compress
                ))
            {
                compressor.Write(data, 0, data.Length);
            }

            return memory.ToArray();
        }

        /// <summary>
        /// Decompress the data.
        /// </summary>
        public static byte[] Decompress
            (
                byte[] data
            )
        {
            var memory = new MemoryStream(data);
            using var decompresser = new DeflateStream
                (
                    memory,
                    CompressionMode.Decompress
                );
            var result = new MemoryStream();
            StreamUtility.AppendTo
                (
                    decompresser,
                    result
                );
            decompresser.Dispose();

            return result.ToArray();
        }

        #endregion
    }
}

