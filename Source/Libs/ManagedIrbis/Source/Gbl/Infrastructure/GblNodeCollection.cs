// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GblNodeCollection.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.ObjectModel;
using System.Text;

using AM;
using AM.Collections;
using AM.Text;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Gbl.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public sealed class GblNodeCollection
        : NonNullCollection<GblNode>
    {
        #region Properties

        /// <summary>
        /// Parent node.
        /// </summary>
        [CanBeNull]
        public GblNode Parent { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public GblNodeCollection
            (
                [CanBeNull] GblNode parent
            )
        {
            Parent = parent;
        }

        #endregion

        #region Collection<T> members

        /// <inheritdoc cref="Collection{T}.ClearItems" />
        protected override void ClearItems()
        {
            foreach (GblNode node in this)
            {
                node.Parent = null;
            }

            base.ClearItems();
        }

        /// <inheritdoc cref="NonNullCollection{T}.InsertItem" />
        protected override void InsertItem
            (
                int index,
                GblNode item
            )
        {
            Sure.NotNull(item, nameof(item));

            if (!ReferenceEquals(item.Parent, null))
            {
                if (!ReferenceEquals(item.Parent, Parent))
                {
                    throw new IrbisException();
                }
            }

            item.Parent = Parent;
            base.InsertItem(index, item);
        }

        /// <inheritdoc cref="NonNullCollection{T}.SetItem" />
        protected override void SetItem
            (
                int index,
                GblNode item
            )
        {
            Sure.NotNull(item, nameof(item));

            if (!ReferenceEquals(item.Parent, null))
            {
                if (!ReferenceEquals(item.Parent, Parent))
                {
                    throw new IrbisException();
                }
            }

            if (index < Count)
            {
                GblNode previousItem = this[index];
                if (!ReferenceEquals(previousItem, item))
                {
                    previousItem.Parent = null;
                }
            }

            item.Parent = Parent;
            base.SetItem(index, item);
        }

        /// <inheritdoc cref="Collection{T}.RemoveItem" />
        protected override void RemoveItem
            (
                int index
            )
        {
            GblNode node = this[index];
            node.Parent = null;

            base.RemoveItem(index);
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            GblUtility.NodesToText(result, this);

            return result.ToString();
        }

        #endregion
    }
}
