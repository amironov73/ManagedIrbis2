﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BinaryAttachment.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

#endregion

// ReSharper disable VirtualMemberCallInConstructor

namespace AM
{
    /// <summary>
    /// Binary attachment.
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
        [ExcludeFromCodeCoverage]
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
            string result = string.Format
                (
                    "{0}: {1} bytes",
                    Name,
                    Content.Length
                );

            return result;
        }

        #endregion
    }
}
