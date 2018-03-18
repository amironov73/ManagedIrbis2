﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ServerIniFile.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Net;

using AM;
using AM.IO;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Server
{
    /// <summary>
    /// irbis_server.ini
    /// </summary>
    [PublicAPI]
    public sealed class ServerIniFile
        : IDisposable
    {
        #region Constants

        /// <summary>
        /// Name of the server INI-file.
        /// </summary>
        public const string FileName = "irbis_server.ini";

        /// <summary>
        /// Main section name.
        /// </summary>
        public const string Main = "Main";

        /// <summary>
        /// REDIRECT section name.
        /// </summary>
        public const string Redirect = "REDIRECT";

        #endregion

        #region Properties

        /// <summary>
        /// Путь на таблицу isisacw.
        /// </summary>
        /// <remarks>
        /// C:\IRBIS64\isisacw
        /// </remarks>
        [CanBeNull]
        public string AlphabetTablePath => GetValue("ACTABPATH");

        /// <summary>
        /// Имя файла со списком клиентов с паролями для доступа к серверу.
        /// </summary>
        /// <remarks>
        /// client_m.mnu
        /// </remarks>
        [CanBeNull]
        public string ClientList => GetValue("CLIENTLIST");

        /// <summary>
        /// Время жизни клиента без подтверждения (в мин.).
        /// По умочанию 0 – режим отключен.
        /// </summary>
        /// <remarks>
        /// 30
        /// </remarks>
        public int ClientTimeLive => GetValue<int>("CLIENT_TIME_LIVE");

        /// <summary>
        /// Путь к системным меню и параметрическим файлам БД.
        /// </summary>
        /// <remarks>
        /// C:\IRBIS64\DATAI\
        /// </remarks>
        [CanBeNull]
        public string DataPath => GetValue("DATAPATH");

        /// <summary>
        /// Шифровать профили клиентов.
        /// </summary>
        /// <remarks>
        /// 0
        /// </remarks>
        public bool EncryptPasswords => Convert.ToBoolean(GetValue<int>("ENCRYPT_PASSWORDS"));

        /// <summary>
        /// Включение кэширования при форматировании.
        /// Значение по умолчанию – 1.
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool FormatsCacheable => Convert.ToBoolean(GetValue<int>("FORMAT_CASHABLE"));

        /// <summary>
        /// INI-file.
        /// </summary>
        [NotNull]
        public IniFile Ini { get; private set; }

        /// <summary>
        /// IP адрес сервера используется только для показа в таблице описателей.
        /// </summary>
        /// <remarks>
        /// 127.0.0.1
        /// </remarks>
        [CanBeNull]
        // ReSharper disable once InconsistentNaming
        public IPAddress IPAddress
        {
            get
            {
                string address = GetValue("IP_ADDRESS")
                    .ThrowIfNull("IP_ADDRESS");
                IPAddress result = IPAddress.Parse(address);

                return result;
            }
        }

        /// <summary>
        /// IP порт сервера.
        /// </summary>
        /// <remarks>
        /// 6666
        /// </remarks>
        // ReSharper disable once InconsistentNaming
        public int IPPort => GetValue<int>("IP_PORT");

        /// <summary>
        /// Флаг разрешает серверу использовать процесс обработки многократно.
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool KeepProcessAlive => Convert.ToBoolean(GetValue<int>("KEEP_PROCESS_ALIVE"));

        /// <summary>
        /// Cигнал окончания процесса обработки посылается
        /// через TCP на порт 7778, а не как сообщение Windows.
        /// В этом случае RegisterWindowMessage игнорируется.
        /// </summary>
        /// <remarks>
        /// 7778
        /// </remarks>
        // ReSharper disable once InconsistentNaming
        public int LocalIPPort => GetValue<int>("IP_PORT_LOCAL");

        /// <summary>
        /// Размер log-файла, байты.
        /// </summary>
        /// <remarks>
        /// 1000000
        /// </remarks>
        public int MaxLogFileSize => GetValue<int>("MaxLogFileSize");

        /// <summary>
        /// Максимально возможное число процессов обработки,
        /// если превышено - возвращается ошибка SERVER_OVERLOAD.
        /// По умолчанию = 20.
        /// </summary>
        /// <remarks>
        /// 100
        /// </remarks>
        public int MaxProcessCount => GetValue<int>("MAX_PROCESS_COUNT");

        /// <summary>
        /// Максимально возможное число запросов
        /// к долгоживущему процессу обработки,
        /// после чего процесс автоматически прерывается.
        /// По умолчанию = 100.
        /// </summary>
        /// <remarks>
        /// 1000
        /// </remarks>
        public int MaxProcessRequests => GetValue<int>("MAX_PROCESS_REQUESTS");

        /// <summary>
        /// Максимальное число процессов обработки,
        /// которые сервер использует многократно
        /// (только если KEEP_PROCESS_ALIVE = 1).
        /// </summary>
        /// <remarks>
        /// 30
        /// </remarks>
        public int MaxServers => GetValue<int>("MAX_SERVERS");

        /// <summary>
        /// Время мониторинга в сек. процессов и потоков
        /// на соответствие друг другу. Если 0 – режим отключен.
        /// 10 сек по умолчанию.
        /// </summary>
        /// <remarks>
        /// 60
        /// </remarks>
        public int ProcessThreadsMonitor => GetValue<int>("PROCESS_THREADS_MONITOR");

        /// <summary>
        /// Максимальное время обработки запроса (в мин.)
        /// По умочанию 0 – режим отключен.
        /// </summary>
        /// <remarks>
        /// 15
        /// </remarks>
        public int ProcessTimeLive => GetValue<int>("PROCESS_TIME_LIVE");

        /// <summary>
        /// Разрешать (определять) адрес машины клиента при регистрации.
        /// </summary>
        /// <remarks>
        /// 0
        /// </remarks>
        public bool RecognizeClientAddress => Convert.ToBoolean(GetValue<int>("RECOGNIZE_CLIENT_ADDRESS"));

        /// <summary>
        /// Сигнал обмена сообщениями между сервером и процессами обработки
        /// регистрируется в системе WINDOWS и получает уникальный идентификатор.
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool RegisterWindowsMessage => Convert.ToBoolean(GetValue<int>("RegisterWindowMessage"));

        /// <summary>
        /// (Не используется) Имя запускаемого сервисом WINDOWS
        /// сервера ИРБИС64. Это либо irbis_server.exe,
        /// либо server_64_console.exe.
        /// </summary>
        /// <remarks>
        /// irbis_server.exe
        /// </remarks>
        [CanBeNull]
        public string ServiceName => GetValue("SERVICE_NAME");

        /// <summary>
        /// Не выводить windows-сообщения о непредвиденных ошибках
        /// в процессах обработки server_64.exe. Этот параметр рекомендуется
        /// использовать, если во время эксплуатации сервера выводятся
        /// сообщения об ошибках в server_64.exe.
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool SuppressExceptions => Convert.ToBoolean(GetValue<int>("SUPPRESS_EXEPTIONS"));

        /// <summary>
        /// Путь к системным INI-файлам.
        /// </summary>
        /// <remarks>
        /// C:\IRBIS64\
        /// </remarks>
        [CanBeNull]
        public string SystemPath => GetValue("SYSPATH");

        /// <summary>
        /// Сервер может работать в режиме параллельной обработки
        /// чтения-записи запросов клиентов в многопотоковом режиме.
        /// Режим включается данным параметром.
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool ThreadsAvailable => Convert.ToBoolean(GetValue<int>("THREADS_AVAILABLE"));

        /// <summary>
        /// Читать запрос от клиента в потоке (по умолчанию 0).
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool ThreadsAvailableRead => Convert.ToBoolean(GetValue<int>("THREADS_AVAILABLE_READ"));

        /// <summary>
        /// Послать ответ клиенту в потоке (по умолчанию 0).
        /// </summary>
        /// <remarks>
        /// 1
        /// </remarks>
        public bool ThreadsAvailableWrite => Convert.ToBoolean(GetValue<int>("THREADS_AVAILABLE_WRITE"));

        /// <summary>
        /// Путь и имя таблицы для перевода в верхний регистр.
        /// Значение по умолчанию – isisucw.
        /// </summary>
        /// <remarks>
        /// C:\IRBIS64\isisucw
        /// </remarks>
        [CanBeNull]
        public string UpperCaseTable => GetValue("UCTABPATH");

        /// <summary>
        /// Директория для сохранения временных файлов,
        /// используемых для межпроцессорного взаимодействия
        /// сервера и процессов обработки.
        /// </summary>
        /// <remarks>
        /// C:\IRBIS64\workdir
        /// Если на сервере установить флаг ОТЛАДКА, временные файлы
        /// не уничтожаются (следите за объемом физической памяти
        /// на диске и количеством файлов в директории).
        /// </remarks>
        [CanBeNull]
        public string WorkDir => GetValue("WORKDIR");

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerIniFile
            (
                [NotNull] IniFile iniFile
            )
        {
            Sure.NotNull(iniFile, nameof(iniFile));

            Ini = iniFile;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get value.
        /// </summary>
        [CanBeNull]
        public string GetValue
            (
                [NotNull] string keyName,
                [CanBeNull] string defaultValue
            )
        {
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            string result = Ini.GetValue
                (
                    Main,
                    keyName,
                    defaultValue
                );

            return result;
        }

        /// <summary>
        /// Get value.
        /// </summary>
        [CanBeNull]
        public string GetValue
            (
                [NotNull] string keyName
            )
        {
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            string result = Ini.GetValue
                (
                    Main,
                    keyName,
                    null
                );

            return result;
        }

        /// <summary>
        /// Get value.
        /// </summary>
        [CanBeNull]
        public T GetValue<T>
            (
                [NotNull] string keyName,
                [CanBeNull] T defaultValue
            )
        {
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            T result = Ini.GetValue
                (
                    Main,
                    keyName,
                    defaultValue
                );

            return result;
        }

        /// <summary>
        /// Get value.
        /// </summary>
        [CanBeNull]
        public T GetValue<T>
            (
                [NotNull] string keyName
            )
        {
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            T result = Ini.GetValue
                (
                    Main,
                    keyName,
                    default(T)
                );

            return result;
        }

        /// <summary>
        /// Set value.
        /// </summary>
        public ServerIniFile SetValue
            (
                [NotNull] string keyName,
                [CanBeNull] string value
            )
        {
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            Ini.SetValue
                (
                    Main,
                    keyName,
                    value
                );

            return this;
        }

        /// <summary>
        /// Set value.
        /// </summary>
        public ServerIniFile SetValue<T>
            (
                [NotNull] string keyName,
                [CanBeNull] T value
            )
        {
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            Ini.SetValue
                (
                    Main,
                    keyName,
                    value
                );

            return this;
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            Ini.Dispose();
        }

        #endregion
    }
}

