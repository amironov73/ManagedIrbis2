// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BinaryAttachment.cs -- binary attachment for ArsMagnaException
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace AM
{
    /// <summary>
    /// Binary attachment (e. g. for <see cref="ArsMagnaException"/>).
    /// </summary>
    [PublicAPI]
    public class BinaryAttachment
    {
        #region Properties

        /// <summary>
        /// Name of the attachment.
        /// </summary>
        [NotNull]
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Content of the attachment.
        /// </summary>
        [NotNull]
        public virtual byte[] Content { get; protected set; }

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected BinaryAttachment()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the attachment.</param>
        /// <param name="content">Content of the attachment.</param>
        public BinaryAttachment
            (
                [NotNull] string name,
                [NotNull] byte[] content
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));
            Sure.NotNull(content, nameof(content));

            Name = name;
            Content = content;
        }

        #endregion

        #region Public methods

        /// <inheritdoc cref="object.ToString" />
        [Pure]
        public override string ToString()
        {
            string result = $"{Name}: {Content.Length} bytes";

            return result;
        }

        #endregion
    }
}
