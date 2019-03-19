// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FileUtility.cs -- file manipulation routines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    /// File manipulation routines.
    /// </summary>
    [PublicAPI]
    public static class FileUtility
    {
        #region Public methods

        /// <summary>
        /// Побайтовое сравнение двух файлов.
        /// </summary>
        public static int Compare
            (
                [NotNull] string first,
                [NotNull] string second
            )
        {
            Sure.FileExists(first, nameof(first));
            Sure.FileExists(second, nameof(second));

            using
                (
                    FileStream firstStream = File.OpenRead(first),
                        secondStream = File.OpenRead(second)
                )
            {
                return StreamUtility.CompareTo
                    (
                        firstStream,
                        secondStream
                    );
            }
        }

        /// <summary>
        /// Copies the specified source file to the specified
        /// destination.
        /// </summary>
        public static void Copy
            (
                [NotNull] string sourceName,
                [NotNull] string targetName,
                bool overwrite
            )
        {
            Sure.NotNull(sourceName, nameof(sourceName));
            Sure.NotNull(targetName, nameof(targetName));

            File.Copy(sourceName, targetName, overwrite);
            DateTime creationTime = File.GetCreationTime(sourceName);
            File.SetCreationTime(targetName, creationTime);
            DateTime lastAccessTime = File.GetLastAccessTime(sourceName);
            File.SetLastAccessTime(targetName, lastAccessTime);
            DateTime lastWriteTime = File.GetLastWriteTime(sourceName);
            File.SetLastWriteTime(targetName, lastWriteTime);
            FileAttributes attributes = File.GetAttributes(sourceName);
            File.SetAttributes(targetName, attributes);
        }

        /// <summary>
        /// Copies given file only if source is newer than destination.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="backup">If set to <c>true</c>
        /// create backup copy of destination file.</param>
        /// <returns><c>true</c> if file copied; <c>false</c> otherwise.
        /// </returns>
        public static bool CopyNewer
            (
                [NotNull] string sourcePath,
                [NotNull] string targetPath,
                bool backup
            )
        {
            Sure.FileExists(sourcePath, nameof(sourcePath));
            Sure.NotNullNorEmpty(targetPath, nameof(targetPath));

            if (File.Exists(targetPath))
            {
                FileInfo sourceInfo = new FileInfo(sourcePath);
                FileInfo targetInfo = new FileInfo(targetPath);
                if (sourceInfo.LastWriteTime < targetInfo.LastWriteTime)
                {
                    return false;
                }
                if (backup)
                {
                    CreateBackup(targetPath, true);
                }
            }

            File.Copy(sourcePath, targetPath, true);

            return true;
        }

        /// <summary>
        /// Copies given file and creates backup copy of target file.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <returns>Name of backup file or <c>null</c>
        /// if no backup created.</returns>
        [CanBeNull]
        public static string CopyWithBackup
            (
                [NotNull] string sourcePath,
                [NotNull] string targetPath
            )
        {
            Sure.FileExists(sourcePath, nameof(sourcePath));
            Sure.NotNullNorEmpty(targetPath, nameof(targetPath));

            string result = null;
            if (File.Exists(targetPath))
            {
                result = CreateBackup(targetPath, true);
            }
            File.Copy(sourcePath, targetPath, false);

            return result;
        }

        /// <summary>
        /// Creates backup copy for given file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="rename">If set to <c>true</c>
        /// given file will be renamed; otherwise it will be copied.</param>
        /// <returns>Name of the backup file.</returns>
        [NotNull]
        public static string CreateBackup
            (
                [NotNull] string path,
                bool rename
            )
        {
            Sure.FileExists(path, nameof(path));

            string result = GetNotExistentFileName
                (
                    path,
                    "_backup_"
                );
            if (rename)
            {
                File.Move(path, result);
            }
            else
            {
                File.Copy(path, result, false);
            }

            return result;
        }

        /// <summary>
        /// Deletes specified file if it exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void DeleteIfExists
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Find file in path.
        /// </summary>
        [CanBeNull]
        public static string FindFileInPath
            (
                [NotNull] string fileName,
                [CanBeNull] string path,
                char elementDelimiter
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            if (ReferenceEquals(path, null) || path.Length == 0)
            {
                return null;
            }

            string[] elements = path.Split(elementDelimiter);
            foreach (string element in elements)
            {
                string fullPath = Path.Combine
                    (
                        element,
                        fileName
                    );
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the name of the not existent file.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>Name of not existent file.</returns>
        [NotNull]
        public static string GetNotExistentFileName
            (
                [NotNull] string original,
                [NotNull] string suffix
            )
        {
            Sure.NotNullNorEmpty(original, nameof(original));
            Sure.NotNullNorEmpty(suffix, nameof(suffix));

            string path = Path.GetDirectoryName(original) ?? string.Empty;
            string name = Path.GetFileNameWithoutExtension(original);
            string ext = Path.GetExtension(original);

            for (int i = 1; i < 10000; i++)
            {
                string result = Path.Combine
                    (
                        path,
                        name + suffix + i + ext
                    );
                if (!File.Exists(result) && !Directory.Exists(result))
                {
                    return result;
                }
            }

            // TODO diagnostics

            Log.Error
                (
                    nameof(FileUtility) + "::" + nameof(GetNotExistentFileName)
                    + ": giving up"
                );

            throw new ArsMagnaException();
        }

        /// <summary>
        /// Sets file modification date to current date.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <remarks>If no such file exists it will be created.</remarks>
        public static void Touch
            (
                [NotNull] string fileName
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            if (File.Exists(fileName))
            {
                File.SetLastWriteTime(fileName, DateTime.Now);
            }
            else
            {
                File.WriteAllBytes(fileName, new byte[0]);
            }
        }

        #endregion
    }
}
