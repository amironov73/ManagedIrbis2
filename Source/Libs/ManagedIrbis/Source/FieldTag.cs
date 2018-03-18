// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FieldTag.cs -- field tag related routines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 * TODO check tag length?
 */

#region Using directives

using System.Diagnostics.CodeAnalysis;

using AM;
using AM.Collections;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Field tag related routines.
    /// </summary>
    [PublicAPI]
    public static class FieldTag
    {
        #region Properties

        /// <summary>
        /// Бросать исключения при валидации?
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static bool ThrowOnValidate { get; set; }

        #endregion

        #region Construction

        static FieldTag()
        {
            GoodCharacters = new CharSet().AddRange('0', '9');
        }

        #endregion

        #region Private members

        private static readonly CharSet GoodCharacters;

        #endregion

        #region Public methods

        /// <summary>
        /// Whether given tag is valid?
        /// </summary>
        public static bool IsValidTag
            (
                [CanBeNull] string tag
            )
        {
            if (ReferenceEquals(tag, null) || tag.Length == 0)
            {
                return false;
            }

            bool result = GoodCharacters.CheckText(tag)
                && Normalize(tag) != "0"
                && tag.Length < 6; // ???

            return result;
        }

        /// <summary>
        /// Normalization.
        /// </summary>
        public static string Normalize
            (
                [CanBeNull] string tag
            )
        {
            if (ReferenceEquals(tag, null) || tag.Length == 0)
            {
                return tag;
            }

            string result = tag;
            while (result.Length > 1 && result.StartsWith("0"))
            {
                result = result.Substring(1);
            }

            return result;
        }

        /// <summary>
        /// Verify the tag value.
        /// </summary>
        public static bool Verify
            (
                [CanBeNull] string tag,
                bool throwOnError
            )
        {
            bool result = IsValidTag(tag);

            if (!result)
            {
                Log.Error
                    (
                        "FieldTag::Verify: "
                        + "bad tag="
                        + tag.ToVisibleString()
                    );

                if (throwOnError)
                {
                    throw new VerificationException
                        (
                            "bad tag="
                            + tag.ToVisibleString()
                        );
                }
            }

            return result;
        }

        /// <summary>
        /// Verify the tag value.
        /// </summary>
        public static bool Verify
            (
                int tag,
                bool throwOnError
            )
        {
            bool result = tag > 0;

            if (!result)
            {
                Log.Error
                    (
                        "FieldTag::Verify: "
                        + "bad tag="
                        + tag.ToInvariantString()
                    );

                if (throwOnError)
                {
                    throw new VerificationException
                    (
                        "bad tag="
                        + tag.ToInvariantString()
                    );
                }
            }

            return result;
        }

        /// <summary>
        /// Verify the tag value.
        /// </summary>
        public static bool Verify
            (
                int tag
            )
        {
            return Verify
                (
                    tag,
                    ThrowOnValidate
                );
        }

        /// <summary>
        /// Verify the tag value.
        /// </summary>
        public static bool Verify
            (
                [CanBeNull] string tag
            )
        {
            return Verify
                (
                    tag,
                    ThrowOnValidate
                );
        }

        #endregion
    }
}
