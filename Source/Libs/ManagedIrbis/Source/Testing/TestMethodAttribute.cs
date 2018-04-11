﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TestClassAttribute.cs -- attribute for test method(s)
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
    /// Attribute for test method(s).
    /// </summary>
    [PublicAPI]
    public sealed class TestMethodAttribute
        : Attribute
    {
    }
}
