// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IAttachmentContainer.cs -- container for the BinaryAttachment
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Container for the <see cref="BinaryAttachment"/>.
    /// </summary>
    [PublicAPI]
    public interface IAttachmentContainer
    {
        /// <summary>
        /// List attachments.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        BinaryAttachment[] ListAttachments();
    }
}
