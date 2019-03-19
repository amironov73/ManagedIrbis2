// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisTestClassAttribute.cs -- attribute for class(es) with test(s)
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Testing
{
    /// <summary>
    /// Attribute for class(es) with test(s).
    /// </summary>
    [PublicAPI]
    public sealed class IrbisTestClassAttribute
        : Attribute
    {
    }
}
