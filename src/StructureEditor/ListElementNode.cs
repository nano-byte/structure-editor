// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes a node in the structure that points to an element in the list.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying the content of the element.</typeparam>
    public class ListElementNode<TContainer, TList, TElement, TEditor> : Node
        where TElement : TList
        where TEditor : INodeEditor<TElement>, new()
    {
        private readonly TContainer _container;
        private readonly IList<TList> _list;
        private readonly TElement _element;

        /// <summary>
        /// Creates a new list element node.
        /// </summary>
        /// <param name="name">The name of the element type.</param>
        /// <param name="container">The container containing the <paramref name="list"/>.</param>
        /// <param name="list">The list containing the <paramref name="element"/>.</param>
        /// <param name="element">The element in the list.</param>
        public ListElementNode(string name, TContainer container, IList<TList> list, TElement element)
            : base(name, GetDescription<TElement>(), element)
        {
            _list = list;
            _element = element;
            _container = container;
        }

        /// <inheritdoc/>
        public override string GetSerialized()
            => _element.ToXmlString();

        /// <inheritdoc/>
        public override IValueCommand GetUpdateCommand(string serializedValue)
        {
            var newValue = XmlStorage.FromXmlString<TElement>(serializedValue);
            return newValue.Equals(_element) ? null : new ReplaceInList<TList>(_list, _element, newValue);
        }

        /// <inheritdoc/>
        public override IUndoCommand GetRemoveCommand()
            => new RemoveFromCollection<TList>(_list, _element);

        /// <inheritdoc/>
        public override IEditorControl GetEditorControl(ICommandExecutor executor)
        {
            var editor = new TEditor {Target = _element, CommandExecutor = executor};

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (editor is ITargetContainerInject<TContainer> inject)
                inject.TargetContainer = _container;

            return editor;
        }
    }
}
