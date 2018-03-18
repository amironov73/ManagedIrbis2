// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ProviderManager.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;

using AM;
using AM.Configuration;
using AM.Logging;
using AM.Parameters;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Client
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class ProviderManager
    {
        #region Constants

        /// <summary>
        /// Connected client (<see cref="IrbisConnection"/>).
        /// </summary>
        public const string Connected = "Connected";

        /// <summary>
        /// Default provider (<see cref="IrbisConnection"/>).
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// Local provider.
        /// </summary>
        public const string Local = "Local";

        /// <summary>
        /// Null provider.
        /// </summary>
        public const string Null = "Null";

        ///// <summary>
        ///// Connected client with some local functionality:
        ///// <see cref="SemiConnectedClient"/>.
        ///// </summary>
        //public const string SemiConnected = "SemiConnected";

        #endregion

        #region Properties

        /// <summary>
        /// Registry.
        /// </summary>
        [NotNull]
        public static Dictionary<string, Type> Registry
        {
            get; private set;
        }

        #endregion

        #region Construction

        static ProviderManager()
        {
            Registry = new Dictionary<string, Type>
            {
                {Null, typeof(NullProvider)},
                //{Local, typeof(LocalProvider)},
                {Connected, typeof(ConnectedClient)},
                //{SemiConnected, typeof(SemiConnectedClient)},
                {Default, typeof(ConnectedClient)}
            };
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get <see cref="IrbisProvider" /> and configure it.
        /// </summary>
        [NotNull]
        public static IrbisProvider GetAndConfigureProvider
            (
                [NotNull] string configurationString
            )
        {
            Sure.NotNullNorEmpty(configurationString, nameof(configurationString));

            Parameter[] parameters = ParameterUtility.ParseString(configurationString);
            string name = parameters.GetParameter("Provider", null);
            if (ReferenceEquals(name, null) || name.Length == 0)
            {
                Log.Warn
                    (
                        nameof(ProviderManager) + "::" + nameof(GetAndConfigureProvider)
                        + ": provider name not specified"
                    );

                name = Default;
            }

            name = name.ThrowIfNull("provider name");

            string assemblyParameter = parameters.GetParameter("Assembly", null)
                ?? parameters.GetParameter("Assemblies", null);
            if (!ReferenceEquals(assemblyParameter, null) && assemblyParameter.Length != 0)
            {
                string[] assemblies = assemblyParameter.Split('|');
                foreach (string assembly in assemblies)
                {
                    Assembly.Load(assembly);
                }
            }

            string typeName = parameters.GetParameter("Register", null)
                  ?? parameters.GetParameter("Type", null);
            if (!string.IsNullOrEmpty(typeName))
            {
                Type type = Type.GetType(typeName, true);
                string shortName = type.Name;
                if (!Registry.ContainsKey(shortName))
                {
                    Registry.Add(shortName, type);
                }
            }

            IrbisProvider result = GetProvider(name, true)
                .ThrowIfNull();
            result.Configure(configurationString);

            return result;
        }

        /// <summary>
        /// Get <see cref="IrbisProvider"/> by name.
        /// </summary>
        [CanBeNull]
        public static IrbisProvider GetProvider
            (
                [NotNull] string name,
                bool throwOnError
            )
        {
            Sure.NotNull(name, nameof(name));

            if (!Registry.TryGetValue(name, out Type type))
            {
                Log.Error
                    (
                        nameof(ProviderManager) + "::" + nameof(GetProvider)
                        + ": provider not found: "
                        + name
                    );

                if (throwOnError)
                {
                    throw new IrbisException("Provider not found: " + name);
                }

                return null;
            }

            if (ReferenceEquals(type, null))
            {
                Log.Error
                    (
                        nameof(ProviderManager) + "::" + nameof(GetProvider)
                        + ": can't find type: "
                        + name
                    );

                if (throwOnError)
                {
                    throw new IrbisException("Can't find type: " + name);
                }

                return null;
            }

            IrbisProvider result = (IrbisProvider)Activator.CreateInstance(type);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        [NotNull]
        public static IrbisProvider GetPreconfiguredProvider()
        {
            string configurationString = ConfigurationUtility.GetString("IrbisProvider");
            if (ReferenceEquals(configurationString, null) || configurationString.Length == 0)
            {
                Log.Error
                    (
                        nameof(ProviderManager) + "::" + nameof(GetPreconfiguredProvider)
                        + ": IrbisProvider configuration key not specified"
                    );

                throw new IrbisException("IrbisProvider configuration key not specified");
            }

            IrbisProvider result = GetAndConfigureProvider(configurationString);

            return result;
        }

        #endregion
    }
}
