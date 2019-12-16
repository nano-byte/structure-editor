// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Provides an interface to a control that edits a node in the structure.
    /// </summary>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    public interface INodeEditor<T> : INodeEditor
        where T : class
    {
        /// <summary>
        /// The element to be edited.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        T? Target { get; set; }
    }
}
