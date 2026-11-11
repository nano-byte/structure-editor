// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;
using Eto.Drawing;
using Eto.Forms;
using NanoByte.Common;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// A text editor that automatically validates changes using an external callback after a short period of no input.
/// </summary>
public class ValidatingTextEditor : Panel, IPendingChanges
{
    /// <summary>
    /// Raised when changes have accumulated after a short period of no input.
    /// </summary>
    /// <remarks>Handlers should throw an exception if the text is invalid. It is then displayed as an error.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Cannot rename System.Action<T>")]
    public event Action<string>? ContentChanged;

    /// <summary>
    /// The text editor control used internally.
    /// </summary>
    public ITextEditor TextEditor { get; }

    private readonly Label _labelStatus = new() {Wrap = WrapMode.None};
    private readonly Color _defaultTextColor;
    private readonly UITimer _timer = new() {Interval = 0.25};

    private IStructureSerializer? _serializer;

    /// <summary>Suppresses <see cref="ITextEditor.TextChanged"/> while the content is set programmatically.</summary>
    private bool _settingContent;

    /// <summary>The location of the error currently displayed in <see cref="_labelStatus"/>, if any.</summary>
    private (int line, int column)? _errorLocation;

    /// <summary>
    /// Creates a new validating text editor.
    /// </summary>
    /// <param name="textEditor">The text editor control to use internally.</param>
    public ValidatingTextEditor(ITextEditor textEditor)
    {
        TextEditor = textEditor ?? throw new ArgumentNullException(nameof(textEditor));
        TextEditor.TextChanged += TextEditor_TextChanged;
        TextEditor.Control.LostFocus += delegate { CommitPendingChanges(); };

        _defaultTextColor = _labelStatus.TextColor;
        _labelStatus.MouseDown += delegate
        {
            if (_errorLocation is (var line, var column)) TextEditor.NavigateTo(line, column);
        };
        _timer.Elapsed += delegate { CommitPendingChanges(); };

        Content = new TableLayout
        {
            Spacing = new(0, 4),
            Rows =
            {
                new TableRow(TextEditor.Control) {ScaleHeight = true},
                new TableRow(_labelStatus)
            }
        };
    }

    /// <summary>
    /// Creates a new validating text editor using <see cref="TextEditorFactory.Default"/>.
    /// </summary>
    public ValidatingTextEditor() : this(TextEditorFactory.Default()) {}

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>
    /// Sets a new text to be edited.
    /// </summary>
    /// <param name="text">The text to set.</param>
    /// <param name="serializer">The serializer that produced the text. Determines the highlighting scheme and is used to locate errors.</param>
    public void SetContent(string text, IStructureSerializer serializer)
    {
        // Discard any validation pending against the previous content
        _timer.Stop();

        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        _settingContent = true;
        try
        {
            TextEditor.ClearError();
            TextEditor.SetContent(text, serializer.FormatName);
        }
        finally
        {
            _settingContent = false;
        }

        SetStatus("OK");
    }

    private void TextEditor_TextChanged(object? sender, EventArgs e)
    {
        if (_settingContent) return;

        TextEditor.ClearError();

        if (_timer.Started) _timer.Stop();
        else SetStatus("Changed...");
        _timer.Start();
    }

    /// <summary>
    /// Validates any changes immediately instead of waiting for a period of no input.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Invalid user input may cause arbitrary exceptions.")]
    public void CommitPendingChanges()
    {
        if (!_timer.Started) return;
        _timer.Stop();

        try
        {
            ValidateContent();
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    private void ValidateContent()
    {
        if (ContentChanged == null) return;

        string? warning = null;

        void HandleLogEntry(LogSeverity severity, string? message, Exception? exception)
        {
            if (severity >= LogSeverity.Warn)
                warning = message ?? exception?.GetMessageWithInner();
        }

        Log.Handler += HandleLogEntry;
        try
        {
            ContentChanged(TextEditor.Text);
        }
        finally
        {
            Log.Handler -= HandleLogEntry;
        }

        if (warning == null) SetStatus("OK");
        else SetStatus(warning, isError: true);
    }

    private void HandleError(Exception ex)
    {
        if (_serializer != null && _serializer.TryGetErrorLocation(ex, out int line, out int column, out string message))
        {
            TextEditor.ShowError(line, column, message);
            SetStatus($"Line {line}, column {column}: {message}", isError: true, errorLocation: (line, column));
        }
        else
            SetStatus(ex.GetMessageWithInner(), isError: true);
    }

    private void SetStatus(string message, bool isError = false, (int line, int column)? errorLocation = null)
    {
        _errorLocation = errorLocation;

        // Only show the first line, to keep the height of the status line constant
        int lineBreak = message.IndexOfAny(['\r', '\n']);
        _labelStatus.Text = lineBreak == -1 ? message : message.Substring(0, lineBreak);
        _labelStatus.ToolTip = errorLocation == null ? message : message + Environment.NewLine + "(Click to go to error)";
        _labelStatus.TextColor = isError ? Colors.Red : _defaultTextColor;
        _labelStatus.Cursor = errorLocation == null ? Cursors.Default : Cursors.Pointer;
    }
}
