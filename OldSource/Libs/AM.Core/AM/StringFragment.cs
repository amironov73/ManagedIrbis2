// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* StringFragment.cs -- fragment of the System.String
 * Ars Magna project, http://arsmagna.ru 
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Fragment of the <see cref="string"/>.
    /// </summary>
    [PublicAPI]
    public struct StringFragment
    {
        #region Properties

        /// <summary>
        /// Original string.
        /// </summary>
        [NotNull]
        public string Original { get; }

        /// <summary>
        /// Offset.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Length.
        /// </summary>
        public int Length { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public StringFragment
            (
                [NotNull] string original,
                int offset,
                int length
            )
        {
            Sure.NotNull(original, nameof(original));
            Sure.NonNegative(offset, nameof(offset));
            Sure.NonNegative(length, nameof(length));

            Original = original;
            Offset = offset;
            Length = length;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Operator ==
        /// </summary>
        public static bool operator ==(StringFragment left, StringFragment right)
        {
            return left.ToString() == right.ToString();
        }

        /// <summary>
        /// Operator ==
        /// </summary>
        public static bool operator ==(StringFragment left, string right)
        {
            return left.ToString() == right;
        }

        /// <summary>
        /// Operator ==
        /// </summary>
        public static bool operator ==(string left, StringFragment right)
        {
            return left == right.ToString();
        }

        /// <summary>
        /// Operator !=
        /// </summary>
        public static bool operator !=(StringFragment left, StringFragment right)
        {
            return left.ToString() != right.ToString();
        }

        /// <summary>
        /// Operator !=
        /// </summary>
        public static bool operator !=(StringFragment left, string right)
        {
            return left.ToString() != right;
        }

        /// <summary>
        /// Operator !=
        /// </summary>
        public static bool operator !=(string left, StringFragment right)
        {
            return left != right.ToString();
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals
            (
                object obj
            )
        {
            if (obj.GetType() != typeof(StringFragment))
            {
                return false;
            }

            StringFragment other = (StringFragment)obj;

            return ToString() == other.ToString();
        }

        /// <inheritdoc cref="object.GetHashCode" />
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            return (Original.GetHashCode() * 17 + Offset) * 17 + Length;
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return Original.Substring(Offset, Length);
        }

        #endregion
    }
}
