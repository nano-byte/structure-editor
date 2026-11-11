// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using Eto.Forms;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// A control for editing the serialized text representation of a node in the structure.
/// </summary>
/// <remarks>Implement this to plug in a more capable text editor (e.g., with syntax highlighting) via <see cref="TextEditorFactory"/>.</remarks>
public interface ITextEditor
{
    /// <summary>
    /// The control to display.
    /// </summary>
    Control Control { get; }

    /// <summary>
    /// The text currently in the editor.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Raised when the <see cref="Text"/> has been changed.
    /// </summary>
    event EventHandler<EventArgs>? TextChanged;

    /// <summary>
    /// Replaces the text in the editor.
    /// </summary>
    /// <param name="text">The new text.</param>
    /// <param name="formatName">The <see cref="IStructureSerializer.FormatName"/> of the text (e.g., XML or JSON). May be used to determine syntax highlighting.</param>
    void SetContent(string text, string formatName);

    /// <summary>
    /// Marks a location in the text as erroneous, without moving the caret.
    /// </summary>
    /// <param name="line">The 1-based line number of the error.</param>
    /// <param name="column">The 1-based column number of the error.</param>
    /// <param name="message">A message describing the error.</param>
    void ShowError(int line, int column, string message);

    /// <summary>
    /// Removes any marker set by <see cref="ShowError"/>.
    /// </summary>
    void ClearError();

    /// <summary>
    /// Moves the caret to a location in the text and gives the editor the input focus.
    /// </summary>
    /// <param name="line">The 1-based line number.</param>
    /// <param name="column">The 1-based column number.</param>
    void NavigateTo(int line, int column);
}
