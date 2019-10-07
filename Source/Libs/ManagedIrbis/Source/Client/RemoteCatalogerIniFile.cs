﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RemoteCatalogerIniFile.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.IO;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Client
{
    /// <summary>
    /// INI-file for cataloger.
    /// </summary>
    [PublicAPI]
    public class RemoteCatalogerIniFile
    {
        #region Constants

        /// <summary>
        /// Display section name.
        /// </summary>
        public const string Display = "Display";

        /// <summary>
        /// Entry section name.
        /// </summary>
        public const string Entry = "Entry";

        /// <summary>
        /// Main section name.
        /// </summary>
        public const string Main = "Main";

        /// <summary>
        /// Private section name.
        /// </summary>
        public const string Private = "Private";

        #endregion

        #region Properties

        /// <summary>
        /// Имя файла пакетного задания для АВТОВВОДА.
        /// </summary>
        public string AutoinFile
        {
            get { return GetValue(Main, "AUTOINFILE", "autoin.gbl"); }
        }

        /// <summary>
        /// Разрешает (значение 1) или запрещает (значение 0)
        /// автоматическое слияние двух версий записи при корректировке
        /// (при получении сообщения о несовпадении версий – в ситуации,
        /// когда одну запись пытаются одновременно откорректировать
        /// два и более пользователей) Автоматическое слияние проводится
        /// по формальному алгоритму: неповторяющиеся поля заменяются,
        /// а оригинальные значения повторяющихся полей суммируются
        /// </summary>
        public bool AutoMerge
        {
            get { return GetBoolean(Main, "AUTOMERGE", "0"); }
        }

        /// <summary>
        /// Имя краткого (строкa) формата показа.
        /// </summary>
        public string BriefPft
        {
            get { return GetValue(Main, "BRIEFPFT", "brief.pft"); }
        }

        /// <summary>
        /// Интервал в мин., по истечении которого клиент посылает
        /// на сервер уведомление о том, что он «жив».
        /// </summary>
        public int ClientTimeLive
        {
            get { return GetValue(Main, "CLIENT_TIME_LIVE", 15); }
        }

        /// <summary>
        /// Имя файла-справочника со списком ТВП переформатирования
        /// для копирования.
        /// </summary>
        public string CopyMnu
        {
            get { return GetValue(Main, "COPYMNU", "fst.mnu"); }
        }

        /// <summary>
        /// Метка поля «количество выдач» в БД ЭК.
        /// </summary>
        public string CountTag
        {
            get { return GetValue(Main, "DBNTAGSPROS", "999"); }
        }

        /// <summary>
        /// Имя файла списка БД для АРМа Каталогизатора/Комплектатора.
        /// </summary>
        public string DatabaseList
        {
            get { return GetValue(Main, "DBNNAMECAT", "dbnam2.mnu"); }
        }

        /// <summary>
        /// Имя формата для ФЛК документа в целом.
        /// </summary>
        public string DbnFlc
        {
            get { return GetValue(Entry, "DBNFLC", "dbnflc.pft"); }
        }

        /// <summary>
        /// Имя базы данных по умолчанию.
        /// </summary>
        public string DefaultDb
        {
            get { return GetValue(Main, "DEFAULTDB", "IBIS"); }
        }

        /// <summary>
        /// Имя шаблона для создания новой БД.
        /// </summary>
        public string EmptyDbn
        {
            get { return GetValue(Main, "EMPTYDBN", "BLANK"); }
        }

        /// <summary>
        /// Метка поля «экземпляры» в БД ЭК.
        /// </summary>
        public string ExemplarTag
        {
            get { return GetValue(Main, "DBNTAGEKZ", "910"); }
        }

        /// <summary>
        /// Имя файла-справочника со списком ТВП переформатирования
        /// для экспорта.
        /// </summary>
        public string ExportMenu
        {
            get { return GetValue(Main, "EXPORTMNU", "export.mnu"); }
        }

        /// <summary>
        /// Имя файла-справочника со списком доступных РЛ.
        /// </summary>
        public string FormatMenu
        {
            get { return GetValue(Main, "FMTMNU", "fmt.mnu"); }
        }

        /// <summary>
        /// Имя БД, содержащей тематический рубрикатор ГРНТИ.
        /// </summary>
        public string HelpDbn
        {
            get { return GetValue(Main, "HELPDBN", "HELP"); }
        }

        /// <summary>
        /// Имя файла-справочника со списком ТВП переформатирования
        /// для импорта.
        /// </summary>
        public string ImportMenu
        {
            get { return GetValue(Main, "IMPORTMNU", "import.mnu"); }
        }

        /// <summary>
        /// Префикс инверсии для шифра документа в БД ЭК.
        /// </summary>
        public string IndexPrefix
        {
            get { return GetValue(Main, "DBNPREFSHIFR", "I="); }
        }

        /// <summary>
        /// Метка поля «шифр документа» в БД ЭК.
        /// </summary>
        public string IndexTag
        {
            get { return GetValue(Main, "DBNTAGSHIFR", "903"); }
        }

        /// <summary>
        /// INI-file.
        /// </summary>
        [NotNull]
        public IniFile Ini { get; private set; }

        /// <summary>
        /// Имя файла-справочника со списком постоянных запросов.
        /// </summary>
        public string IriMenu
        {
            get { return GetValue(Main, "IRIMNU", "iri.mnu"); }
        }

        /// <summary>
        /// Размер порции для показа кратких описаний.
        /// </summary>
        public int MaxBriefPortion
        {
            get { return GetValue(Main, "MAXBRIEFPORTION", 10); }
        }

        /// <summary>
        /// Максимальное количество отмеченных документов.
        /// </summary>
        public int MaxMarked
        {
            get { return GetValue(Main, "MAXMARKED", 10); }
        }

        /// <summary>
        /// Имя файла-справочника со списком доступных форматов
        /// показа документов.
        /// </summary>
        public string PftMenu
        {
            get { return GetValue(Main, "PFTMNU", "pft.mnu"); }
        }

        /// <summary>
        /// Имя оптимизационного файла, который определяет принцип
        /// формата ОПТИМИЗИРОВАННЫЙ (в АРМах Читатель и Каталогизатор).
        /// Для БД электронного каталога (IBIS) значение PFTW.OPT
        /// определяет в качестве оптимизированных  RTF-форматы,
        /// а значение PFTW_H.OPT – HTML-форматы
        /// </summary>
        public string PftOpt
        {
            get { return GetValue(Main, "PFTOPT", "pft.opt"); }
        }

        ///// <summary>
        ///// Определяет режим работы АРМа для «читателя»,
        ///// иначе преподавателя.
        ///// При значении параметра 1 в АРМе будут скрыты все режимы,
        ///// связанные с корректировкой, удалением, переносом данных,
        ///// т. е. только просмотр и вывод на печать. При старте будет
        ///// устанавливаться БД каталога, заданная в настройке
        ///// (параметр «dbn» секции «private»), или БД IBIS.
        ///// При значении параметра 2 в АРМе будут скрыты все режимы,
        ///// связанные с корректировкой, удалением данных, но останется
        ///// доступным режим переноса данных.
        ///// При значении параметра 3 – вариант 2 с добавлением режима
        ///// удаления данных.
        ///// </summary>
        //public int ReaderMode
        //{
        //    get { return GetValue(Main, "ReaderMode", 0); }
        //}

        /// <summary>
        /// Имя дополнительного INI-файла со сценариями поиска для БД.
        /// </summary>
        public string SearchIni
        {
            get { return GetValue(Main, "SEARCHINI", string.Empty); }
        }

        /// <summary>
        /// Имя эталонной БД Электронного каталога.
        /// </summary>
        public string StandardDbn
        {
            get { return GetValue(Main, "ETALONDBN", "IBIS"); }
        }

        /// <summary>
        /// Директория для сохранения временных (выходных) данных.
        /// </summary>
        public string WorkDirectory
        {
            get { return GetValue(Main, "WORKDIR", "/irbiswrk"); }
        }

        /// <summary>
        /// Имя файла оптимизации РЛ ввода.
        /// </summary>
        public string WsOpt
        {
            get { return GetValue(Main, "WSOPT", "ws.opt"); }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public RemoteCatalogerIniFile
            (
                IniFile iniFile
            )
        {
            Ini = iniFile;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get boolean value
        /// </summary>
        public bool GetBoolean
            (
                string sectionName,
                string keyName,
                string defaultValue
            )
        {
            Sure.NotNullNorEmpty(sectionName, nameof(sectionName));
            Sure.NotNullNorEmpty(keyName, nameof(keyName));
            Sure.NotNullNorEmpty(defaultValue, nameof(defaultValue));

            string text = Ini.GetValue
                (
                    sectionName,
                    keyName,
                    defaultValue
                )
                .ThrowIfNull("Ini.GetValue");

            return ConversionUtility.ToBoolean(text);
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

            string result = Ini.GetValue
                (
                    sectionName,
                    keyName,
                    defaultValue
                );

            return result;
        }

        /// <summary>
        /// Get value.
        /// </summary>
#nullable disable
        [CanBeNull]
        public T GetValue<T>
            (
                [NotNull] string sectionName,
                [NotNull] string keyName,
                [CanBeNull] T defaultValue
            )
        {
            Sure.NotNullNorEmpty(sectionName, nameof(sectionName));
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            T result = Ini.GetValue
                (
                    sectionName,
                    keyName,
                    defaultValue
                );

            return result;
        }
#nullable restore

        /// <summary>
        /// Set value.
        /// </summary>
        public RemoteCatalogerIniFile SetValue
            (
                string sectionName,
                string keyName,
                string? value
            )
        {
            Sure.NotNullNorEmpty(sectionName, nameof(sectionName));
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            Ini.SetValue
                (
                    sectionName,
                    keyName,
                    value
                );

            return this;
        }

        /// <summary>
        /// Set value.
        /// </summary>
#nullable disable
        public RemoteCatalogerIniFile SetValue<T>
            (
                [NotNull] string sectionName,
                [NotNull] string keyName,
                [CanBeNull] T value
            )
        {
            Sure.NotNullNorEmpty(sectionName, nameof(sectionName));
            Sure.NotNullNorEmpty(keyName, nameof(keyName));

            Ini.SetValue
                (
                    sectionName,
                    keyName,
                    value
                );

            return this;
        }
#nullable restore

        #endregion
    }
}
