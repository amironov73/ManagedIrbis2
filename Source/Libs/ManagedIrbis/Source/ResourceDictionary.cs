﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ResourceDictionary.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AM;
using AM.Collections;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// Resource dictionary.
    /// </summary>
    [PublicAPI]
    public sealed class ResourceDictionary<T>
        : IEnumerable<IrbisResource<T>>
    {
        #region Properties

        /// <summary>
        /// Get resource count.
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceDictionary()
        {
            _dictionary = new CaseInsensitiveDictionary<IrbisResource<T>>();
        }

        #endregion

        #region Private members

        private readonly CaseInsensitiveDictionary<IrbisResource<T>> _dictionary;

        #endregion

        #region Public methods

        /// <summary>
        /// Add resource.
        /// </summary>
#nullable disable
        public ResourceDictionary<T> Add
            (
                string name,
                [CanBeNull] T content
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            _dictionary.Add(name, new IrbisResource<T>(name, content));

            return this;
        }
#nullable restore

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public ResourceDictionary<T> Clear()
        {
            _dictionary.Clear();

            return this;
        }

        /// <summary>
        /// Get resource content for name.
        /// </summary>
#nullable disable
        public T Get
            (
                string name
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            if (!_dictionary.TryGetValue(name, out var resource))
            {
                return default;
            }

            return resource.Content;
        }
#nullable restore

        /// <summary>
        /// Determines whether we have the resource with given name.
        /// </summary>
        public bool Have
            (
                string name
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            return _dictionary.ContainsKey(name);
        }

        /// <summary>
        /// Put the content for resource with given name.
        /// </summary>
#nullable disable
        public ResourceDictionary<T> Put
            (
                string name,
                [CanBeNull] T content
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            IrbisResource<T> resource = new IrbisResource<T>(name, content);
            _dictionary[name] = resource;

            return this;
        }
#nullable restore

        /// <summary>
        /// Remove resource with given name from the dictionary.
        /// </summary>
        public ResourceDictionary<T> Remove
            (
                string name
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            _dictionary.Remove(name);

            return this;
        }

        /// <summary>
        /// Get all resources as array.
        /// </summary>
        public IrbisResource<T>[] ToArray()
        {
            return _dictionary.Values.ToArray();
        }

        #endregion

        #region IEnumerable members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<IrbisResource<T>> GetEnumerator()
        {
            foreach (KeyValuePair<string, IrbisResource<T>> pair in _dictionary)
            {
                yield return pair.Value;
            }
        }

        #endregion
    }
}
