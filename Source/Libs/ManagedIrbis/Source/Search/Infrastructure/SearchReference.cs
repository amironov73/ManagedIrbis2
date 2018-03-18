﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SearchReference.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Search.Infrastructure
{
    /// <summary>
    /// #N
    /// </summary>
    sealed class SearchReference
        : ISearchTree
    {
        #region Properties

        /// <inheritdoc cref="ISearchTree.Parent"/>
        public ISearchTree Parent { get; set; }

        /// <summary>
        /// Number.
        /// </summary>
        [CanBeNull]
        public string Number { get; set; }

        #endregion

        #region ISearchTree members

        public ISearchTree[] Children
        {
            get { return SearchUtility.EmptyArray; }
        }

        public string Value { get { return Number; } }

        public TermLink[] Find
            (
                SearchContext context
            )
        {
            TermLink[] result = TermLink.EmptyArray;

            int number = Number.SafeToInt32(-1);
            if (number > 0)
            {
                var history = context.Manager.SearchHistory;
                if (number <= history.Count)
                {
                    SearchResult previous = history[number - 1];


                    int[] found = context.Provider.Search(previous.Query);
                    result = TermLink.FromMfn(found);
                }
            }

            return result;
        }

        /// <inheritdoc cref="ISearchTree.ReplaceChild"/>
        public void ReplaceChild
            (
                ISearchTree fromChild,
                ISearchTree toChild
            )
        {
            Log.Error
                (
                    "SearchReference::ReplaceChild: "
                    + "not implemented"
                );

            throw new NotImplementedException();
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return "#" + Number;
        }

        #endregion
    }
}
