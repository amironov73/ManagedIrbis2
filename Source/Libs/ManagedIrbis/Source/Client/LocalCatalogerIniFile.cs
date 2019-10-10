// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* LocalCatalogerIniFile.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;
using AM.IO;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

#endregion

namespace ManagedIrbis.Client
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// INI-file for cataloger.
    /// </summary>
    [PublicAPI]
    public class LocalCatalogerIniFile
    {
        #region Constants

        #endregion

        #region Properties

        /// <summary>
        /// INI-file.
        /// </summary>
        public IniFile Ini { get; private set; }

        /// <summary>
        /// Context section.
        /// </summary>
        public ContextIniSection Context
        {
            get { return _contextIniSection; }
        }

        /// <summary>
        /// Desktop section.
        /// </summary>
        private DesktopIniSection Desktop
        {
            get { return _desktopIniSection; }
        }


        /// <summary>
        /// Magna section.
        /// </summary>
        public IniFile.Section MagnaSection
        {
            get
            {
                var ini = Ini;
                var result = ini.GetOrCreateSection("Magna");

                return result;
            }
        }

        /// <summary>
        /// Main section.
        /// </summary>
        public IniFile.Section Main
        {
            get
            {
                var ini = Ini;
                var result = ini.GetOrCreateSection("Main");

                return result;
            }
        }

        /// <summary>
        /// Организация, на которую куплен ИРБИС.
        /// </summary>
        public string? Organization => Main["User"];

        /// <summary>
        /// IP адрес ИРБИС сервера.
        /// </summary>
        public string ServerIP => Main["ServerIP"] ?? "127.0.0.1";

        /// <summary>
        /// Port number of the IRBIS server.
        /// </summary>
        public int ServerPort
        {
            get
            {
                // coverity[dereference]
                var result = Convert.ToInt32
                    (
                        Main["ServerPort"] ?? "6666"
                    );

                return result;
            }
        }

        /// <summary>
        /// User login.
        /// </summary>
        public string? UserName
        {
            get
            {
                const string Login = "UserName";
                return Context.UserName ?? MagnaSection[Login];
            }
        }

        /// <summary>
        /// User password.
        /// </summary>
        public string? UserPassword
        {
            get
            {
                const string Password = "UserPassword";
                return Context.Password ?? MagnaSection[Password];
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalCatalogerIniFile
            (
                IniFile iniFile
            )
        {
            Ini = iniFile;
            _contextIniSection = new ContextIniSection(iniFile);
            _desktopIniSection = new DesktopIniSection(iniFile);
        }

        #endregion

        #region Private members

        private readonly  ContextIniSection _contextIniSection;
        private readonly DesktopIniSection _desktopIniSection;

        #endregion

        #region Public methods

        /// <summary>
        /// Build connection string.
        /// </summary>
        public string BuildConnectionString()
        {
            var settings = new ConnectionSettings
            {
                Host = ServerIP,
                Port = ServerPort,
                Username = UserName.EmptyToNull(),
                Password = UserPassword.EmptyToNull()
            };

            return settings.ToString();
        }

        /// <summary>
        /// Get value.
        /// </summary>
        public string? GetValue
            (
                string sectionName,
                string keyName,
                string? defaultValue
            )
        {
            Sure.NotNullNorEmpty(sectionName, nameof(sectionName));
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            var result = Ini.GetValue
                (
                    sectionName,
                    keyName,
                    defaultValue
                );

            return result;
        }


        /// <summary>
        /// Load from specified file.
        /// </summary>
        public static LocalCatalogerIniFile Load
            (
                string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, "fileName");

            var iniFile = new IniFile();
            iniFile.Read(fileName, IrbisEncoding.Ansi);
            var result = new LocalCatalogerIniFile(iniFile);

            return result;
        }

        #endregion
    }
}
