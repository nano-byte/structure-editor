// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// Controls which <see cref="ITextEditor"/> implementation is used by <see cref="StructureEditor{T}"/>.
/// </summary>
public static class TextEditorFactory
{
    private static Func<ITextEditor> _default = () => new PlainTextEditor();

    /// <summary>
    /// Creates the <see cref="ITextEditor"/> for new <see cref="StructureEditor{T}"/> instances. Defaults to <see cref="PlainTextEditor"/>.
    /// </summary>
    /// <remarks>Set this during application startup to use a more capable text editor (e.g., with syntax highlighting).</remarks>
    public static Func<ITextEditor> Default
    {
        get => _default;
        set => _default = value ?? throw new ArgumentNullException(nameof(value));
    }
}
