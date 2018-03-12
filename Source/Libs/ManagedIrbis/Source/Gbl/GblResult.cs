﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GblResult.cs -- result of GBL execution
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Linq;

using AM;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure;

#endregion

namespace ManagedIrbis.Gbl
{
    /// <summary>
    /// Result of GBL execution.
    /// </summary>
    [PublicAPI]
    public sealed class GblResult
    {
        #region Properties

        /// <summary>
        /// Момент начала обработки.
        /// </summary>
        public DateTime TimeStarted { get; set; }

        /// <summary>
        /// Всего времени затрачено (с момента начала обработки).
        /// </summary>
        public TimeSpan TimeElapsed { get; set; }

        /// <summary>
        /// Отменено пользователем.
        /// </summary>
        // TODO implement
        public bool Canceled { get; set; }

        /// <summary>
        /// Исключение (если возникло).
        /// </summary>
        // TODO implement
        public Exception Exception { get; set; }

        /// <summary>
        /// Предполагалось обработать записей.
        /// </summary>
        // TODO implement
        public int RecordsSupposed { get; set; }

        /// <summary>
        /// Обработано записей.
        /// </summary>
        public int RecordsProcessed { get; set; }

        /// <summary>
        /// Успешно обработано записей.
        /// </summary>
        public int RecordsSucceeded { get; set; }

        /// <summary>
        /// Ошибок при обработке записей.
        /// </summary>
        public int RecordsFailed { get; set; }

        /// <summary>
        /// Результаты для каждой записи.
        /// </summary>
        public ProtocolLine[] Protocol { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Get empty result.
        /// </summary>
        [NotNull]
        public static GblResult GetEmptyResult()
        {
            GblResult result = new GblResult
            {
                TimeStarted = DateTime.Now,
                TimeElapsed = new TimeSpan(0)
            };

            return result;
        }

        /// <summary>
        /// Merge result.
        /// </summary>
        public void MergeResult
            (
                [NotNull] GblResult intermediateResult
            )
        {
            Sure.NotNull(intermediateResult, nameof(intermediateResult));

            if (intermediateResult.Canceled)
            {
                Canceled = intermediateResult.Canceled;
            }
            if (!ReferenceEquals(intermediateResult.Exception, null))
            {
                Exception = intermediateResult.Exception;
            }
            RecordsProcessed += intermediateResult.RecordsProcessed;
            RecordsFailed += intermediateResult.RecordsFailed;
            RecordsSucceeded += intermediateResult.RecordsSucceeded;

            if (ReferenceEquals(Protocol, null))
            {
                Protocol = new ProtocolLine[0];
            }

            ProtocolLine[] otherLines = intermediateResult.Protocol;
            if (ReferenceEquals(otherLines, null))
            {
                otherLines = new ProtocolLine[0];
            }

            Protocol = ArrayUtility.Merge
                (
                    Protocol,
                    otherLines
                );
        }

        /// <summary>
        /// Parse server response.
        /// </summary>
        public void Parse
            (
                [NotNull] ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            Protocol = ProtocolLine.Parse(response);
            RecordsProcessed = Protocol.Length;
            RecordsSucceeded = Protocol.Count(line => line.Success);
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" />
        /// that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" />
        /// that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format
                (
                    "Records processed: {0}, Canceled: {1}",
                    RecordsProcessed,
                    Canceled
                );
        }

        #endregion
    }
}
