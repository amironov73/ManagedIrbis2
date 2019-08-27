// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ServerVersion.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class ServerVersion
    {
        /// <summary>
        ///
        /// </summary>
        public string? Organization { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int MaxClients { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int ConnectedClients { get; set; }

        /// <summary>
        ///
        /// </summary>
        public void Parse(string[] lines)
        {
            if (lines.Length >= 4)
            {
                Organization = lines[0];
                Version = lines[1];
                ConnectedClients = NumericUtility.ParseInt32(lines[2]);
                MaxClients = NumericUtility.ParseInt32(lines[3]);
            }
            else
            {
                Version = lines[0];
                ConnectedClients = NumericUtility.ParseInt32(lines[1]);
                MaxClients = NumericUtility.ParseInt32(lines[2]);
            }
        }

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format
                (
                    "Version: {0}, MaxClients: {1}, "
                    + "ConnectedClients: {2}, Organization: {3}",
                    Version.ToVisibleString(),
                    MaxClients,
                    ConnectedClients,
                    Organization.ToVisibleString()
                );
        }

        #endregion

    }
}
