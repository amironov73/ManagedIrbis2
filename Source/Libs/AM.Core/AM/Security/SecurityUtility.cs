// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SecurityUtility.cs -- useful routines for X509 certificates
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using AM.IO;

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace AM.Security
{
    /// <summary>
    /// Collection of useful routines for X509 certificates.
    /// </summary>
    [PublicAPI]
    public static class SecurityUtility
    {
        #region Public methods

        /// <summary>
        /// Get certificate for SslStream.
        /// </summary>
        public static X509Certificate GetSslCertificate()
        {

            Assembly assembly = typeof(SecurityUtility).Assembly;
            string resourceName = "AM.Core.ArsMagnaSslSocket.cer";
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (ReferenceEquals(stream, null))
            {
                throw new ArsMagnaException();
            }

            byte[] rawData = stream.ReadToEnd();
            X509Certificate result = new X509Certificate();
            result.Import(rawData);

            return result;
        }

        /// <summary>
        /// </summary>
        /// Get certificate by the subject.
        public static X509Certificate GetRootCertificate
            (
                [NotNull] string subject
            )
        {
            Sure.NotNullNorEmpty(subject, nameof(subject));

            using X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 certificate in store.Certificates)
            {
                if (string.Compare(certificate.Subject, subject,
                    StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return certificate;
                }
            }

            throw new ArsMagnaException();
        }

        /// <summary>
        /// Простое шифрование текста до неузнаваемости.
        /// </summary>
        /// <param name="secretText">Например, строка подключения,
        /// содержащая чувствительные данные.</param>
        /// <param name="password">Пароль.</param>
        /// <returns>Зашифрованный текст.</returns>
        public static string Encrypt
            (
                string secretText,
                string password
            )
        {
            var plainBytes = Encoding.UTF8.GetBytes(secretText);
            var symmetricAlgorithm = Rijndael.Create();
            var passwordBytes = new Rfc2898DeriveBytes(password, new byte[16])
                .GetBytes(16);
            // ReSharper disable IdentifierTypo
            var encryptor = symmetricAlgorithm.CreateEncryptor
                (
                    passwordBytes,
                    new byte[16]
                );
            // ReSharper restore IdentifierTypo
            var memory = new MemoryStream();
            var crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write);
            crypto.Write(plainBytes, 0, plainBytes.Length);
            crypto.FlushFinalBlock();
            var encryptedBytes = memory.ToArray();
            var result = Convert.ToBase64String(encryptedBytes);
            return result;
        }

        /// <summary>
        /// Расшифровка ранее зашифрованного текста.
        /// </summary>
        /// <param name="encryptedText">Текст, полученный от <see cref="Encrypt"/></param>
        /// <param name="password">Пароль.</param>
        /// <returns>Расшифрованный текст.</returns>
        public static string Decrypt
            (
                string encryptedText,
                string password
            )
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var symmetricAlgorithm = Rijndael.Create();
            var passwordBytes = new Rfc2898DeriveBytes(password, new byte[16])
                .GetBytes(16);
            // ReSharper disable IdentifierTypo
            var decryptor = symmetricAlgorithm.CreateDecryptor
                (
                    passwordBytes,
                    new byte[16]
                );
            // ReSharper restore IdentifierTypo
            var memory = new MemoryStream(encryptedBytes);
            var crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read);
            var reader = new BinaryReader(crypto);
            var decryptedBytes = reader.ReadBytes(encryptedBytes.Length);
            var result = Encoding.UTF8.GetString(decryptedBytes);
            return result;
        }

        /// <summary>
        /// Псевдошифрование в Base64.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptToBase64
            (
                string text
            )
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var result = Convert.ToBase64String(bytes);
            return result;
        }

        /// <summary>
        /// Псевдорасшифровка из Base64.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DecryptFromBase64
            (
                string text
            )
        {
            var bytes = Convert.FromBase64String(text);
            var result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        /// <summary>
        /// Вычисление хеша для указанной строки.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public static string ComputeMD5
        // ReSharper restore InconsistentNaming
            (
                string text
            )
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return ComputeMD5(bytes);
        }

        /// <summary>
        /// Вычисление хеша для указанных данных.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public static string ComputeMD5
        // ReSharper restore InconsistentNaming
            (
                byte[] bytes
            )
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(bytes);

            var builder = new StringBuilder(data.Length * 2);
            foreach (var b in data)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        #endregion
    }
}
