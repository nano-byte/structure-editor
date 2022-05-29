// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using NanoByte.Common;
using NanoByte.Common.Native;
using NanoByte.StructureEditor.WinForms.Resources;

namespace NanoByte.StructureEditor.WinForms
{
    /// <summary>
    /// A text editor that automatically validates changes using an external callback after a short period of no input.
    /// </summary>
    public class ValidatingTextEditor : UserControl
    {
        /// <summary>
        /// Raised when changes have accumulated after a short period of no input.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Cannot rename System.Action<T>")]
        [Description("Raised when changes have accumulated after a short period of no input.")]
        public event Action<string>? ContentChanged;

        private readonly StatusStrip _statusStrip = new()
        {
            Location = new Point(0, 128),
            SizingGrip = false
        };
        private readonly ToolStripStatusLabel _labelStatus = new();
        private readonly Timer _timer = new() {Interval = 250};

        /// <summary>
        /// The text editor control used internally.
        /// </summary>
        public TextEditorControl TextEditor { get; private set; } = new();

        public ValidatingTextEditor()
        {
            SuspendLayout();
            SetupControls();
            ResumeLayout(performLayout: false);
        }

        private void SetupControls()
        {
            _timer.Tick += timer_Tick;

            _statusStrip.Items.Add(_labelStatus);
            Controls.Add(_statusStrip);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    _timer.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Set content
        /// <summary>
        /// Sets a new text to be edited.
        /// </summary>
        /// <param name="text">The text to set.</param>
        /// <param name="format">The format named used to determine the highlighting scheme (e.g. XML).</param>
        public void SetContent(string text, string format)
        {
            var textEditor = new TextEditorControl
            {
                Location = new Point(0, 0),
                Size = Size - new Size(0, _statusStrip.Height),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                TabIndex = 0,
                ShowVRuler = false,
                Document =
                {
                    TextContent = text,
                    HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(format)
                }
            };
            textEditor.TextChanged += TextEditor_TextChanged;
            textEditor.Validating += TextEditor_Validating;

            Controls.Add(textEditor);
            Controls.Remove(TextEditor);
            TextEditor.Dispose();
            TextEditor = textEditor;

            SetStatus(Images.Info, "OK");
        }
        #endregion

        //--------------------//

        #region Event handlers
        private void TextEditor_TextChanged(object? sender, EventArgs e)
        {
            TextEditor.Document.MarkerStrategy.RemoveAll(_ => true);

            if (_timer.Enabled) _timer.Stop();
            else SetStatus(null, "Changed...");
            _timer.Start();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Invalid user input may cause arbitrary exceptions.")]
        private void TextEditor_Validating(object? sender, CancelEventArgs e)
        {
            if (_timer.Enabled)
            { // Ensure pending validation is not lost
                try
                {
                    ValidateContent();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                    e.Cancel = true;
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Invalid user input may cause arbitrary exceptions.")]
        private void timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                ValidateContent();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }
        #endregion

        #region Validation
        private void ValidateContent()
        {
            _timer.Stop();
            if (ContentChanged == null) return;

            Log.Handler += HandleLogEntry;
            ContentChanged(TextEditor.Text);
            Log.Handler -= HandleLogEntry;

            string? warning = null;

            void HandleLogEntry(LogSeverity severity, string message, Exception? exception)
            {
                if (severity >= LogSeverity.Warn) warning = message;
            }

            if (warning == null) SetStatus(Images.Info, "OK");
            else SetStatus(Images.Error, warning);
        }

        private void HandleError(Exception ex)
        {
            if (!UnixUtils.IsUnix && ex is InvalidDataException && ex.Source == "System.Xml" && ex.InnerException != null)
            { // Parse XML exception message for position of the error
                int lineStart = ex.Message.LastIndexOf('(') + 1;
                int lineLength = ex.Message.LastIndexOf(',') - lineStart;
                int charStart = ex.Message.LastIndexOf(' ') + 1;
                int charLength = ex.Message.LastIndexOf(')') - charStart;
                if (int.TryParse(ex.Message.Substring(lineStart, lineLength), out int lineNumber) && int.TryParse(ex.Message.Substring(charStart, charLength), out int charNumber))
                {
                    int lineOffset = TextEditor.Document.GetLineSegment(lineNumber - 1).Offset;
                    TextEditor.Document.MarkerStrategy.AddMarker(
                        new TextMarker(lineOffset + charNumber - 1, 10, TextMarkerType.WaveLine) {ToolTip = ex.InnerException.Message});
                    TextEditor.Refresh();
                }

                SetStatus(Images.Error, ex.InnerException.Message);
            }
            else
            {
                SetStatus(Images.Error, ex.InnerException == null
                    ? ex.Message
                    : ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }

        private void SetStatus(Image? image, string message)
        {
            _labelStatus.Image = image;
            _labelStatus.Text = message;
        }
        #endregion

        #region Undo control
        /// <summary>
        /// Clears the undo stack of the underlying text editor control.
        /// </summary>
        public void ClearUndoStack() => TextEditor.Document.UndoStack.ClearAll();

        /// <summary>
        /// Indicates whether <see cref="Undo"/> is currently available.
        /// </summary>
        public bool EnableUndo => TextEditor.EnableUndo;

        /// <summary>
        /// Indicates whether <see cref="Redo"/> is currently available.
        /// </summary>
        public bool EnableRedo => TextEditor.EnableRedo;

        /// <summary>
        /// Executes the undo method of the underlying text editor control.
        /// </summary>
        public void Undo() => TextEditor.Undo();

        /// <summary>
        /// Executes the redo method of the underlying text editor control.
        /// </summary>
        public void Redo() => TextEditor.Redo();
        #endregion
    }
}
