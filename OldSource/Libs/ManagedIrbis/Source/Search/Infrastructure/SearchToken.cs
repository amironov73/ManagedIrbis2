﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SearchToken.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;

using AM;

using JetBrains.Annotations;

using Newtonsoft.Json;

#endregion

namespace ManagedIrbis.Search.Infrastructure
{
    /// <summary>
    /// Token.
    /// </summary>
    [DebuggerDisplay("{Kind} {Text} {Position}")]
    internal sealed class SearchToken
    {
        #region Properties

        /// <summary>
        /// Token kind.
        /// </summary>
        [JsonProperty("kind", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SearchTokenKind Kind { get; set; }

        /// <summary>
        /// Token position.
        /// </summary>
        [JsonProperty("position", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Position { get; set; }

        /// <summary>
        /// Token text.
        /// </summary>
        [CanBeNull]
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public SearchToken
            (
                SearchTokenKind kind,
                int position,
                [CanBeNull] string text
            )
        {
            Kind = kind;
            Position = position;
            Text = text;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return Text.ToVisibleString();
        }

        #endregion
    }
}
