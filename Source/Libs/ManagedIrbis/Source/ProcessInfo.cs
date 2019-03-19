// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ProcessInfo.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;

using AM;
using AM.Collections;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public sealed class ProcessInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClienId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Workstation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Started { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CommandNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static ProcessInfo[] Parse(string[] lines)
        {
            var result = new LocalList<ProcessInfo>();
            var processCount = NumericUtility.ParseInt32(lines[0]);
            var linesPerProcess = NumericUtility.ParseInt32(lines[1]);
            if (processCount == 0 || linesPerProcess == 0)
            {
                return result.ToArray();
            }

            for (int i = 2; i < lines.Length; i += linesPerProcess + 1)
            {
                if ((i + linesPerProcess) > lines.Length)
                {
                    break;
                }

                var process = new ProcessInfo
                {
                    Number = lines.GetItem(i + 0),
                    IpAddress = lines.GetItem(i + 1),
                    Name = lines.GetItem(i + 2),
                    ClienId = lines.GetItem(i + 3),
                    Workstation = lines.GetItem(i + 4),
                    Started = lines.GetItem(i + 5),
                    LastCommand = lines.GetItem(i + 6),
                    CommandNumber = lines.GetItem(i + 7),
                    ProcessId = lines.GetItem(i + 8),
                    State = lines.GetItem(i + 9)
                };
                result.Add(process);
            }

            return result.ToArray();
        }

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return $"{Number} {IpAddress} {Name}";
        }
    }
}
