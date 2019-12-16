// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes a node in the structure that points to a property.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
    public class PropertyNode<TContainer, TProperty, TEditor> : Node
        where TProperty : class
        where TEditor : INodeEditor<TProperty>, new()
    {
        private readonly TContainer _container;
        private readonly PropertyPointer<TProperty?> _pointer;

        /// <summary>
        /// Creates a new property node.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="container">The container containing the property.</param>
        /// <param name="pointer">A pointer to the property.</param>
        public PropertyNode(string name, TContainer container, PropertyPointer<TProperty?> pointer)
            : base(name, GetDescription<TProperty>(), pointer.Value)
        {
            _pointer = pointer;
            _container = container;
        }

        /// <inheritdoc/>
        public override string GetSerialized()
            => _pointer.Value.ToXmlString();

        /// <inheritdoc/>
        public override IValueCommand? GetUpdateCommand(string serializedValue)
        {
            var newValue = XmlStorage.FromXmlString<TProperty>(serializedValue);
            return newValue.Equals(_pointer.Value) ? null : SetValueCommand.For(_pointer, newValue);
        }

        /// <inheritdoc/>
        public override IUndoCommand GetRemoveCommand() => SetValueCommand.For(_pointer, null);

        /// <inheritdoc/>
        public override INodeEditor GetEditorControl(ICommandExecutor executor)
        {
            var editor = new TEditor {Target = _pointer.Value, CommandExecutor = executor};

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (editor is ITargetContainerInject<TContainer> inject)
                inject.TargetContainer = _container;

            return editor;
        }
    }
}
