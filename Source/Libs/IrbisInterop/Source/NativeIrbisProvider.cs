﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NativeIrbisProvider.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AM;
using AM.Collections;
using AM.IO;
using AM.Parameters;
using AM.Runtime;
using AM.Text;
using AM.Threading;

using JetBrains.Annotations;

using ManagedIrbis;
using ManagedIrbis.Client;
using ManagedIrbis.Infrastructure;
using ManagedIrbis.Menus;
using ManagedIrbis.Pft;
using ManagedIrbis.Search;
using ManagedIrbis.Server;

#endregion

namespace IrbisInterop
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class NativeIrbisProvider
        : IrbisProvider
    {
        #region Constants

        /// <summary>
        /// Provider name.
        /// </summary>
        public const string ProviderName = "Native";

        #endregion

        #region Properties

        /// <inheritdoc cref="IrbisProvider.Database" />
        public override string Database
        {
            get { return base.Database; }
            set
            {
                Sure.NotNullNorEmpty(value, nameof(value));

                // ReSharper disable ConditionIsAlwaysTrueOrFalse

                if (!ReferenceEquals(Irbis64, null))
                {
                    Irbis64.UseDatabase(value);
                }
                base.Database = value;

                // ReSharper restore ConditionIsAlwaysTrueOrFalse
            }
        }

        /// <summary>
        /// IRBIS64.DLL wrapper.
        /// </summary>
        [NotNull]
        public Irbis64Dll Irbis64 { get; private set; }

        /// <inheritdoc cref="IrbisProvider.BusyState" />
        public override BusyState BusyState
        {
            get { return Irbis64.Busy; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public NativeIrbisProvider()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            Irbis64 = null;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NativeIrbisProvider
            (
                [NotNull] Irbis64Dll irbis64
            )
        {
            Sure.NotNull(irbis64, nameof(irbis64));

            Irbis64 = irbis64;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NativeIrbisProvider
            (
                [NotNull] ServerConfiguration configuration
            )
        {
            Sure.NotNull(configuration, nameof(configuration));

            Irbis64 = new Irbis64Dll(configuration);
            _ownIrbis64 = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NativeIrbisProvider
            (
                [NotNull] string serverIniPath
            )
            : this (ServerConfiguration.FromIniFile(serverIniPath))
        {
        }

        #endregion

        #region Private members

        private bool _ownIrbis64;

        #endregion

        #region Public methods

        /// <summary>
        /// Register the provider.
        /// </summary>
        public static void Register()
        {
            ProviderManager.Registry.Add
                (
                    ProviderName,
                    typeof(NativeIrbisProvider)
                );
        }

        #endregion

        #region IrbisProvider members

        /// <inheritdoc cref="IrbisProvider.AcquireFormatter" />
        public override IPftFormatter AcquireFormatter()
        {
            return new NativeIrbisFormatter(this);
        }

        /// <inheritdoc cref="IrbisProvider.Configure" />
        public override void Configure
            (
                string configurationString
            )
        {
            Sure.NotNullNorEmpty(configurationString, nameof(configurationString));

            Parameter[] parameters = ParameterUtility.ParseString
                (
                    configurationString
                );

            foreach (Parameter parameter in parameters)
            {
                string name = parameter.Name
                    .ThrowIfNull("parameter.Name")
                    .ToLower();
                string value = parameter.Value
                    .ThrowIfNull("parameter.Value");

                ServerConfiguration configuration;

                switch (name)
                {
                    case "ini":
                        configuration
                            = ServerConfiguration.FromIniFile(value);
                        Irbis64 = new Irbis64Dll(configuration);
                        _ownIrbis64 = true;
                        break;

                    case "path":
                    case "root":
                        configuration
                            = ServerConfiguration.FromPath(value);
                        Irbis64 = new Irbis64Dll(configuration);
                        _ownIrbis64 = true;
                        break;

                    case "layout":
                    case "ver":
                    case "version":
                        SpaceLayout layout;
                        switch (value)
                        {
                            case "2012":
                                layout = SpaceLayout.Version2012();
                                break;

                            case "2014":
                                layout = SpaceLayout.Version2014();
                                break;

                            case "2016":
                                layout = SpaceLayout.Version2016();
                                break;

                            default:
                                throw new IrbisException();
                        }
                        Irbis64.Layout = layout;
                        break;

                    case "db":
                    case "database":
                        Database = value;
                        break;

                    case "provider": // pass through
                    case "assembly":
                    case "assemblies":
                    case "register":
                    case "type":
                        break;

                    default:
                        throw new IrbisException();
                }
            }
        }

        /// <inheritdoc cref="IrbisProvider.ExactSearchLinks"/>
        public override TermLink[] ExactSearchLinks
            (
                string term
            )
        {
            Sure.NotNull(term, nameof(term));

            return Irbis64.ExactSearchLinks(term);
        }

        /// <inheritdoc cref="IrbisProvider.ExactSearchTrimLinks"/>
        public override TermLink[] ExactSearchTrimLinks
            (
                string term,
                int limit
            )
        {
            Sure.NotNull(term, nameof(term));
            Sure.Positive(limit, nameof(limit));

            return Irbis64.ExactSearchTrimLinks(term, limit);
        }

        /// <inheritdoc cref="IrbisProvider.FileExist" />
        public override bool FileExist
            (
                FileSpecification specification
            )
        {
            string result = Irbis64.ExpandSpecification(specification);
            return !string.IsNullOrEmpty(result);
        }

        /// <inheritdoc cref="IrbisProvider.FormatRecord" />
        public override string FormatRecord
            (
                MarcRecord record,
                string format
            )
        {
            Irbis64.SetFormat(format);
            NativeRecord native = NativeRecord.FromMarcRecord(record);
            Irbis64.SetRecord(native);
            string result = Irbis64.FormatRecord();

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.FormatRecords" />
        public override string[] FormatRecords
            (
                int[] mfns,
                string format
            )
        {
            List<string> result = new List<string>(mfns.Length);

            Irbis64.SetFormat(format);
            foreach (int mfn in mfns)
            {
                string text = Irbis64.FormatRecord(mfn);
                result.Add(text);
            }

            return result.ToArray();
        }

        /// <inheritdoc cref="IrbisProvider.GetFileSearchPath" />
        public override string[] GetFileSearchPath()
        {
            return Irbis64.PftSearchPath;
        }

        /// <inheritdoc cref="IrbisProvider.GetMaxMfn" />
        public override int GetMaxMfn()
        {
            return Irbis64.GetMaxMfn();
        }

        /// <inheritdoc cref="IrbisProvider.GetStopWords" />
        public override IrbisStopWords GetStopWords()
        {
            string fileName = Database + ".stw";
            FileSpecification specification = new FileSpecification
                (
                    IrbisPath.MasterFile,
                    Database,
                    fileName
                );
            string content = ReadFile(specification) ?? string.Empty;
            IrbisStopWords result = IrbisStopWords.ParseText
                (
                    fileName,
                    content
                );

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReadFile" />
        public override string ReadFile
            (
                FileSpecification fileSpecification
            )
        {
            Sure.NotNull(fileSpecification, nameof(fileSpecification));

            string path = Irbis64.ExpandSpecification(fileSpecification);
            string result = string.IsNullOrEmpty(path)
                ? null
                : File.ReadAllText(path, IrbisEncoding.Ansi);

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReadIniFile" />
        public override IniFile ReadIniFile
            (
                FileSpecification fileSpecification
            )
        {
            Sure.NotNull(fileSpecification, nameof(fileSpecification));

            string path = Irbis64.ExpandSpecification(fileSpecification);
            IniFile result = string.IsNullOrEmpty(path)
                ? null
                : new IniFile(path, IrbisEncoding.Ansi, false);

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReadMenuFile" />
        public override MenuFile ReadMenuFile
            (
                FileSpecification fileSpecification
            )
        {
            Sure.NotNull(fileSpecification, nameof(fileSpecification));

            string path = Irbis64.ExpandSpecification(fileSpecification);
            MenuFile result = string.IsNullOrEmpty(path)
                ? null
                : MenuFile.ParseLocalFile(path, IrbisEncoding.Ansi);

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReadRecord" />
        public override MarcRecord ReadRecord
            (
                int mfn
            )
        {
            Sure.Positive(mfn, nameof(mfn));

            Irbis64.ReadRecord(mfn);
            NativeRecord native = Irbis64.GetRecord();
            MarcRecord result = native.ToMarcRecord();

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReadTerms" />
        public override TermInfo[] ReadTerms
            (
                TermParameters parameters
            )
        {
            Sure.NotNull(parameters, nameof(parameters));

            string startTerm = parameters.StartTerm
                .ThrowIfNull("parameters.StartTerm");
            int number = parameters.NumberOfTerms;
            TermInfo[] result = parameters.ReverseOrder
                ? Irbis64.ListTermsReverse(startTerm, number)
                : Irbis64.ListTerms(startTerm, number);

            return result;
        }

        /// <inheritdoc cref="IrbisProvider.ReleaseFormatter" />
        public override void ReleaseFormatter
            (
                IPftFormatter formatter
            )
        {
            // Nothing to do here
        }

        /// <inheritdoc cref="IrbisProvider.Search" />
        public override int[] Search
            (
                string expression
            )
        {
            if (string.IsNullOrEmpty(expression))
            {
                return new int[0];
            }

            return Irbis64.Search(expression);
        }

        /// <inheritdoc cref="IrbisProvider.WriteRecord" />
        public override void WriteRecord
            (
                MarcRecord record
            )
        {
            Sure.NotNull(record, nameof(record));

            // TODO update record

            NativeRecord native = NativeRecord.FromMarcRecord(record);
            Irbis64.SetRecord(native);
            Irbis64.WriteRecord(true, false);
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IrbisProvider.Dispose" />
        public override void Dispose()
        {
            if (_ownIrbis64)
            {
                Irbis64.Dispose();
            }
            base.Dispose();
        }

        #endregion
    }
}
