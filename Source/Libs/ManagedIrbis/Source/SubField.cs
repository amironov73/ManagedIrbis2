// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SubField.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public sealed class SubField
    {
        /// <summary>
        /// 
        /// </summary>
        public char Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Decode(string text)
        {
            Code = text[0];
            Value = text.Substring(1);
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return "^" + Code + Value;
        }
    }
}
