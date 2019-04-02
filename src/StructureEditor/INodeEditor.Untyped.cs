// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Provides an interface to a control that edits a node in the structure.
    /// </summary>
    public interface INodeEditor
    {
        /// <summary>
        /// An optional undo system to use for editing.
        /// </summary>
        ICommandExecutor CommandExecutor { get; set; }
    }
}
