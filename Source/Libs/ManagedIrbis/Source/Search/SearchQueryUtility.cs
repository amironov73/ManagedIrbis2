﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SearchQueryUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Linq;

using AM;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Search.Infrastructure
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class SearchQueryUtility
    {
        #region Private members

        internal static List<ISearchTree> GetDescendants
            (
                ISearchTree node
            )
        {
            List<ISearchTree> result = new List<ISearchTree>
            {
                node
            };

            foreach (ISearchTree child in node.Children)
            {
                List<ISearchTree> descendants
                    = GetDescendants(child);
                result.AddRange(descendants);
            }

            return result;
        }

        /// <summary>
        /// Require syntax element.
        /// </summary>
        internal static string RequireSyntax
            (
                [CanBeNull] this string element,
                [NotNull] string message
            )
        {
            if (ReferenceEquals(element, null))
            {
                Log.Error
                    (
                        "SearchQueryUtility::RequireSyntax: "
                        + "required element missing: "
                        + message
                    );

                throw new SearchSyntaxException(message);
            }

            return element;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Extract search terms from the query.
        /// </summary>
        [NotNull]
        public static SearchTerm[] ExtractTerms
            (
                [NotNull] SearchProgram program
            )
        {
            Sure.NotNull(program, nameof(program));

            List<ISearchTree> nodes = GetDescendants(program);
            SearchTerm[] result = nodes
                .OfType<SearchTerm>()
                .ToArray();

            return result;
        }

        #endregion
    }
}
