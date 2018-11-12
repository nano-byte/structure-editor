// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Provides an interface to a control that edits a single object and has a reference to the containing object.
    /// </summary>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    /// <typeparam name="TContainer">The type of the container of <typeparamref name="T"/>.</typeparam>
    public interface IEditorControlContainerRef<T, TContainer> : IEditorControl<T>
    {
        /// <summary>
        /// The <see cref="IEditorControl{T}.Target"/>'s container.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        TContainer ContainerRef { get; set; }
    }
}
