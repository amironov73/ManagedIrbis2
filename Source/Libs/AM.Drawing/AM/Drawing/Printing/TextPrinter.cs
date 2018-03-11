// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TextPrinter.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 */

#region Using directives

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;

using JetBrains.Annotations;

#endregion

namespace AM.Drawing.Printing
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    // ReSharper disable once RedundantNameQualifier
    [System.ComponentModel.DesignerCategory("Code")]
    public abstract class TextPrinter
        : Component
    {
        #region Events

        /// <summary>
        ///
        /// </summary>
        public event PrintEventHandler BeginPrint;

        /// <summary>
        ///
        /// </summary>
        public event PrintEventHandler EndPrint;

        /// <summary>
        ///
        /// </summary>
        public event PrintPageEventHandler PrintPage;

        /// <summary>
        ///
        /// </summary>
        public event QueryPageSettingsEventHandler QueryPageSettings;

        #endregion

        #region Properties

        private RectangleF _borders;

        /// <summary>
        /// Gets or sets the borders.
        /// </summary>
        /// <value>The borders.</value>
        public virtual RectangleF Borders
        {
            [DebuggerStepThrough]
            get
            {
                return _borders;
            }
            [DebuggerStepThrough]
            set
            {
                _borders = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public virtual string DocumentName { get; set; }

        private int _pageNumber;

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public virtual int PageNumber
        {
            get
            {
                return _pageNumber;
            }
        }

        private PageSettings _pageSettings;

        /// <summary>
        /// Gets or sets the page settings.
        /// </summary>
        /// <value>The page settings.</value>
        public virtual PageSettings PageSettings
        {
            [DebuggerStepThrough]
            get
            {
                return _pageSettings;
            }
            [DebuggerStepThrough]
            set
            {
                _pageSettings = value;
            }
        }

        private PrintController _printController;

        /// <summary>
        /// Gets or sets the print controller.
        /// </summary>
        /// <value>The print controller.</value>
        public virtual PrintController PrintController
        {
            [DebuggerStepThrough]
            get
            {
                return _printController;
            }
            [DebuggerStepThrough]
            set
            {
                _printController = value;
            }
        }

        private PrinterSettings _printerSettings;

        /// <summary>
        /// Gets or sets the printer settings.
        /// </summary>
        /// <value>The printer settings.</value>
        public virtual PrinterSettings PrinterSettings
        {
            [DebuggerStepThrough]
            get
            {
                return _printerSettings;
            }
            [DebuggerStepThrough]
            set
            {
                _printerSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public virtual Color TextColor { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public virtual Font TextFont { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TextPrinter"/> class.
        /// </summary>
        protected TextPrinter()
        {
            _borders = new RectangleF(10f, 10f, 10f, 10f);
            DocumentName = "Text document";
            TextColor = Color.Black;
            TextFont = new Font(FontFamily.GenericSerif, 12f);
        }

        #endregion

        #region Private members

        /// <summary>
        /// Called when [begin print].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The
        /// <see cref="T:System.Drawing.Printing.PrintEventArgs"/>
        /// instance containing the event data.</param>
        protected virtual void OnBeginPrint
            (
                object sender,
                PrintEventArgs e
            )
        {
            PrintEventHandler handler = BeginPrint;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [end print].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The
        /// <see cref="T:System.Drawing.Printing.PrintEventArgs"/>
        /// instance containing the event data.</param>
        protected virtual void OnEndPrint
            (
                object sender,
                PrintEventArgs e
            )
        {
            PrintEventHandler handler = EndPrint;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Called when [print page].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="ea">The
        /// <see cref="T:System.Drawing.Printing.PrintPageEventArgs"/>
        /// instance containing the event data.</param>
        protected virtual void OnPrintPage
            (
                object sender,
                PrintPageEventArgs ea
            )
        {
            PrintPageEventHandler handler = PrintPage;
            if (handler != null)
            {
                handler(this, ea);
            }
        }

        /// <summary>
        /// Called when [query page settings].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The
        /// <see cref="T:System.Drawing.Printing.QueryPageSettingsEventArgs"/>
        /// instance containing the event data.</param>
        protected virtual void OnQueryPageSettings
            (
                object sender,
                QueryPageSettingsEventArgs e
            )
        {
            ++_pageNumber;
            QueryPageSettingsEventHandler handler = QueryPageSettings;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Prints the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public virtual bool Print
            (
                string text
            )
        {
            Sure.NotNull(text, nameof(text));

            using (PrintDocument document = new PrintDocument())
            {
                document.DocumentName = DocumentName;
                document.OriginAtMargins = false; // ???
                if (PageSettings != null)
                {
                    document.DefaultPageSettings = PageSettings;
                }
                if (PrintController != null)
                {
                    document.PrintController = PrintController;
                }
                if (PrinterSettings != null)
                {
                    document.PrinterSettings = PrinterSettings;
                }
                document.BeginPrint += OnBeginPrint;
                document.EndPrint += OnEndPrint;
                document.PrintPage += OnPrintPage;
                document.QueryPageSettings += OnQueryPageSettings;
                _pageNumber = 1;
                document.Print();

                return true;
            }
        }

        #endregion
    }
}