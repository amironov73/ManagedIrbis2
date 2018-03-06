/* UnmanagedLibrary.cs -- helper for calling unmanaged methods from managed code
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

#endregion

namespace AM.Win32
{
    /// <summary>
    /// Helper for calling unmanaged methods from
    /// managed code.
    /// </summary>
    /// <remarks>
    /// Inspired by Mike Stall's.Net Debugging Blog
    /// http://blogs.msdn.com/jmstall/default.aspx
    /// </remarks>
    /// <example>
    /// <para>Sample usage may be:</para>
    /// <code>
    /// using ( Unmanaged library lib = new UnmanagedLibrary ( "kernel32" ) )
    /// {
    ///	Action&lt;string&gt; function 
    ///		= lib.GetFunction&lt;Action&lt;string&gt;&gt;("DeleteFile");
    ///	function ( "c:\\tmp.dat" );
    /// }
    /// </code>
    /// </example>
    [PublicAPI]
    public class UnmanagedLibrary
        : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the handle.
        /// </summary>
        public SafeLibraryHandle Handle { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="UnmanagedLibrary"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public UnmanagedLibrary
            (
                [NotNull] string name
            )
        {
            Sure.NotNullNorEmpty(name, nameof(name));

            Name = name;
            Handle = new SafeLibraryHandle(Kernel32.LoadLibrary(name));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        public T GetFunction<T>(string functionName)
            where T : class
        {
            Sure.NotNullNorEmpty(functionName, nameof(functionName));

            IntPtr ptr = Kernel32.GetProcAddress
                (
                    Handle.DangerousGetHandle(),
                    functionName
                );

            Delegate function = Marshal.GetDelegateForFunctionPointer
                (
                    ptr,
                    typeof(T)
                );

            return (T)(object)function;
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            if (!Handle.IsClosed)
            {
                Handle.Close();
            }
        }

        #endregion
    }
}