// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Provides an interface to a control that edits a single object.
    /// </summary>
    public interface IEditorControl
    {
        /// <summary>
        /// An optional undo system to use for editing.
        /// </summary>
        ICommandExecutor CommandExecutor { get; set; }
    }

    /// <summary>
    /// Provides an interface to a control that edits a single object.
    /// </summary>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    public interface IEditorControl<T> : IEditorControl
    {
        /// <summary>
        /// The element to be edited.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        T Target { get; set; }
    }
}
