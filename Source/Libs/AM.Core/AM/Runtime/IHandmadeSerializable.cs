// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IHandmadeSerializable.cs -- object can be stored to a stream
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;

using JetBrains.Annotations;

#endregion

namespace AM.Runtime
{
    /// <summary>
    /// The object can be stored to a stream and restored back.
    /// </summary>
    public interface IHandmadeSerializable
    {
        /// <summary>
        /// Restore the object from a stream.
        /// </summary>
        void RestoreFromStream
            (
                [NotNull] BinaryReader reader
            );

        /// <summary>
        /// Store the object to a stream.
        /// </summary>
        void SaveToStream
            (
                [NotNull] BinaryWriter writer
            );
    }
}
