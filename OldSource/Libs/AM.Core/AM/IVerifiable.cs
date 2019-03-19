// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IVerifiable.cs -- object state verification
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

namespace AM
{
    /// <summary>
    /// Object state verification.
    /// </summary>
    public interface IVerifiable
    {
        /// <summary>
        /// Verify the object state.
        /// </summary>
        bool Verify
            (
                bool throwOnError
            );
    }
}
