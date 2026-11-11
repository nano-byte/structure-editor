// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using Eto.Drawing;
using Eto.Forms;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// A plain monospaced text editor without syntax highlighting. Works on all platforms.
/// </summary>
public class PlainTextEditor : ITextEditor
{
    private readonly TextArea _textArea = new()
    {
        Wrap = false,
        AcceptsTab = true,
        // Small preferred size, otherwise long content may grow the containing window
        Size = new(100, 100)
    };

    /// <summary>
    /// Creates a new plain text editor.
    /// </summary>
    public PlainTextEditor()
    {
        _textArea.Font = Fonts.Monospace(SystemFonts.Default().Size);
        if (_textArea.SpellCheckIsSupported) _textArea.SpellCheck = false;
        _textArea.TextChanged += (_, e) => TextChanged?.Invoke(this, e);
    }

    /// <inheritdoc/>
    public Control Control => _textArea;

    /// <inheritdoc/>
    public string Text => _textArea.Text ?? "";

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? TextChanged;

    /// <inheritdoc/>
    public void SetContent(string text, string formatName)
        => _textArea.Text = text ?? "";

    /// <inheritdoc/>
    public void ShowError(int line, int column, string message) {}

    /// <inheritdoc/>
    public void ClearError() {}

    /// <inheritdoc/>
    public void NavigateTo(int line, int column)
    {
        _textArea.CaretIndex = GetOffset(Text, line, column);
        _textArea.Focus();
    }

    /// <summary>
    /// Converts a line and column number to a character offset.
    /// </summary>
    /// <param name="text">The text to navigate in.</param>
    /// <param name="line">The 1-based line number.</param>
    /// <param name="column">The 1-based column number.</param>
    /// <returns>The 0-based character offset. Clamped to the bounds of the line and text.</returns>
    public static int GetOffset(string text, int line, int column)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        int lineStart = 0;
        for (int i = 1; i < line; i++)
        {
            int lineBreak = text.IndexOf('\n', lineStart);
            if (lineBreak == -1) break;
            lineStart = lineBreak + 1;
        }

        int lineEnd = text.IndexOf('\n', lineStart);
        if (lineEnd == -1) lineEnd = text.Length;
        else if (lineEnd > lineStart && text[lineEnd - 1] == '\r') lineEnd--;

        return Math.Min(lineStart + Math.Max(0, column - 1), lineEnd);
    }
}
