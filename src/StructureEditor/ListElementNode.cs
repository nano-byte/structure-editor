// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a node in the structure that points to an element in the list.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
/// <typeparam name="TList">The type of elements in the list.</typeparam>
/// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
/// <typeparam name="TEditor">An editor for modifying the content of the element.</typeparam>
/// <param name="name">The name of the element type.</param>
/// <param name="container">The container containing the <paramref name="list"/>.</param>
/// <param name="list">The list containing the <paramref name="element"/>.</param>
/// <param name="element">The element in the list.</param>
public class ListElementNode<TContainer, TList, TElement, TEditor>(string name, TContainer container, IList<TList> list, TElement element)
    : Node(name, GetDescription<TElement>(), element)
    where TContainer : class
    where TList : notnull
    where TElement : class, TList
    where TEditor : INodeEditor<TElement>, new()
{
    /// <inheritdoc/>
    public override string GetSerialized()
        => element.ToXmlString();

    /// <inheritdoc/>
    public override IValueCommand? GetUpdateCommand(string serializedValue)
    {
        var newValue = XmlStorage.FromXmlString<TElement>(serializedValue);
        return newValue.Equals(element) ? null : new ReplaceInList<TList>(list, element, newValue);
    }

    /// <inheritdoc/>
    public override IUndoCommand GetRemoveCommand()
        => new RemoveFromCollection<TList>(list, element);

    /// <inheritdoc/>
    public override INodeEditor GetEditorControl(ICommandExecutor executor)
    {
        var editor = new TEditor {Target = element, CommandExecutor = executor};

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (editor is ITargetContainerInject<TContainer> inject)
            inject.TargetContainer = container;

        return editor;
    }
}
