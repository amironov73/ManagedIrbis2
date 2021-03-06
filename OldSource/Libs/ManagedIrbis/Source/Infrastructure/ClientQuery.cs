﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisClientQuery.cs -- client packet with query to the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using AM;
using AM.Text;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Client network packet with query to the server.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("{" + nameof(CommandCode) + "} {"
        + nameof(Workstation) + "} {" + nameof(ClientID) + "} {"
        + nameof(CommandNumber) + "}")]
    public sealed class ClientQuery
        : IVerifiable
    {
        #region Properties

        /// <summary>
        /// Command code.
        /// </summary>
        [CanBeNull]
        public string CommandCode { get; set; }

        /// <summary>
        /// Код АРМ.
        /// </summary>
        public IrbisWorkstation Workstation { get; set; }

        /// <summary>
        /// Client identifier.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int ClientID { get; set; }

        /// <summary>
        /// Sequential command number.
        /// </summary>
        public int CommandNumber { get; set; }

        /// <summary>
        /// User login.
        /// </summary>
        [CanBeNull]
        public string UserLogin { get; set; }

        /// <summary>
        /// User password.
        /// </summary>
        [CanBeNull]
        public string UserPassword { get; set; }

        /// <summary>
        /// Command arguments.
        /// </summary>
        /// <remarks>List can be empty.</remarks>
        [NotNull]
        [ItemCanBeNull]
        public List<object> Arguments { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClientQuery
            (
                [NotNull] string commandCode
            )
        {
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            CommandCode = commandCode;
            Arguments = new List<object>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add arbitrary argument.
        /// </summary>
        [NotNull]
        public ClientQuery Add
            (
                [CanBeNull] object argument
            )
        {
            Arguments.Add(argument);

            return this;
        }

        /// <summary>
        /// Add ANSI text line.
        /// </summary>
        public ClientQuery AddAnsi
            (
                [CanBeNull] string text
            )
        {
            Arguments.Add(new TextWithEncoding(text, IrbisEncoding.Ansi));

            return this;
        }

        /// <summary>
        /// Add UTF8 text line.
        /// </summary>
        public ClientQuery AddUtf8
            (
                [CanBeNull] string text
            )
        {
            Arguments.Add(new TextWithEncoding(text, IrbisEncoding.Utf8));

            return this;
        }

        /// <summary>
        /// Clear argument list.
        /// </summary>
        public ClientQuery Clear()
        {
            Arguments.Clear();

            return this;
        }

        /// <summary>
        /// Dump the query.
        /// </summary>
        public void Dump
            (
                [NotNull] TextWriter writer
            )
        {
            writer.WriteLine($"Command code: '{CommandCode}'");
            writer.WriteLine($"Workstation: '{(char)Workstation}'");
            writer.WriteLine($"Client ID: {ClientID}");
            writer.WriteLine($"Command number: {CommandNumber}");
            writer.WriteLine($"Login: '{UserLogin.ToVisibleString()}'");
            writer.WriteLine($"Password: '{UserPassword.ToVisibleString()}'");

            writer.WriteLine("Arguments:");
            foreach (object argument in Arguments)
            {
                if (ReferenceEquals(argument, null))
                {
                    writer.WriteLine("(null)");
                }
                else
                {
                    Type type = argument.GetType();
                    writer.WriteLine($"{type}: {argument.ToVisibleString()}");
                }
            }

            writer.WriteLine("------------------");
        }

        /// <summary>
        /// Build the packet.
        /// </summary>
        [NotNull]
        public byte[][] EncodePacket()
        {
            MemoryStream stream = MemoryManager.GetMemoryStream();

            // Query header: 7 lines
            stream
                .EncodeString(CommandCode).EncodeDelimiter()
                .EncodeWorkstation(Workstation).EncodeDelimiter()
                .EncodeString(CommandCode).EncodeDelimiter()
                .EncodeInt32(ClientID).EncodeDelimiter()
                .EncodeInt32(CommandNumber).EncodeDelimiter()
                .EncodeString(UserPassword).EncodeDelimiter()
                .EncodeString(UserLogin).EncodeDelimiter()

                // Three empty lines
                .EncodeDelimiter()
                .EncodeDelimiter()
                .EncodeDelimiter()

                // Total: 10 lines
                ;

            if (Arguments.Count != 0)
            {
                int countMinus1 = Arguments.Count - 1;
                for (int i = 0; i < countMinus1; i++)
                {
                    stream.EncodeAny(Arguments[i]);
                    stream.EncodeDelimiter();
                }
                for (int i = countMinus1; i < Arguments.Count; i++)
                {
                    stream.EncodeAny(Arguments[i]);
                    // DO NOT add delimiter to the last line!
                }
            }

            byte[] body = stream.ToArray();
            // TODO Get rid of the MemoryStream
            MemoryStream prefix = new MemoryStream();
            prefix
                .EncodeInt32(body.Length)
                .EncodeDelimiter();
            byte[][] result = { prefix.ToArray(), body };

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<ClientQuery> verifier
                = new Verifier<ClientQuery>(this, throwOnError);

            verifier
                .NotNullNorEmpty
                    (
                        CommandCode,
                        nameof(CommandCode)
                    )
                .Assert
                    (
                        Workstation != IrbisWorkstation.None,
                        nameof(Workstation)
                    )
                .Assert
                    (
                        ClientID != 0,
                        nameof(ClientID)
                    )
                .Assert
                    (
                        CommandNumber != 0,
                        nameof(CommandNumber)
                    );

            return verifier.Result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return $"CommandCode: {CommandCode}, Workstation: {Workstation}, "
                 + $"ClientID: {ClientID}, CommandNumber: {CommandNumber}, "
                 + $"UserLogin: {UserLogin}, UserPassword: {UserPassword}, "
                 + $"Arguments: {Arguments.Count}";
        }

        #endregion
    }
}
