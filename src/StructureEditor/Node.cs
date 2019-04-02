// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common.Undo;
using NanoByte.Common.Values;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes a specific node in the structure.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// The name of the node type.
        /// </summary>
        public string NodeType { get; }

        /// <summary>
        /// A description of the node type.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The object the node represents.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="nodeType">The name of the node type.</param>
        /// <param name="description">A description of the node type.</param>
        /// <param name="target">The object the node represents.</param>
        protected Node(string nodeType, string description, object target)
        {
            NodeType = nodeType;
            Description = description;
            Target = target;
        }

        public override string ToString()
            => $"{NodeType}: {Target}";

        /// <summary>
        /// Returns a serialized representation of the <see cref="Target"/>.
        /// </summary>
        public abstract string GetSerialized();

        /// <summary>
        /// Gets a command for updating the node's target with a new value.
        /// </summary>
        /// <param name="serializedValue">A serialized representation of the new value.</param>
        public abstract IValueCommand GetUpdateCommand(string serializedValue);

        /// <summary>
        /// Gets a command for removing the node's target from the structure.
        /// </summary>
        public abstract IUndoCommand GetRemoveCommand();

        /// <summary>
        /// Gets a GUI control for editing the node's target.
        /// </summary>
        /// <param name="executor">Used to perform undo/redo operations.</param>
        public abstract INodeEditor GetEditorControl(ICommandExecutor executor);

        /// <summary>
        /// Gets the <see cref="DescriptionAttribute.Description"/> of <typeparamref name="T"/>, if any.
        /// </summary>
        [CanBeNull]
        public static string GetDescription<T>()
            => AttributeUtils.GetAttributes<DescriptionAttribute, T>().FirstOrDefault()?.Description;
    }
}
