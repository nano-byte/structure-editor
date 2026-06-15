// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a node in the structure that points to a specific type of value of a polymorphic single-value property.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
/// <typeparam name="TProperty">The base type of the property.</typeparam>
/// <typeparam name="TElement">The specific type the property value currently has.</typeparam>
/// <typeparam name="TEditor">An editor for modifying the content of the value.</typeparam>
/// <param name="name">The name of the value type.</param>
/// <param name="container">The container containing the property.</param>
/// <param name="pointer">A pointer to the property.</param>
/// <param name="element">The current value of the property.</param>
public class PropertyElementNode<TContainer, TProperty, TElement, TEditor>(string name, TContainer container, PropertyPointer<TProperty?> pointer, TElement element)
    : Node(name, GetDescription<TElement>(), element)
    where TContainer : class
    where TProperty : class
    where TElement : class, TProperty
    where TEditor : INodeEditor<TElement>, new()
{
    /// <inheritdoc/>
    public override string GetSerialized()
        => element.ToXmlString();

    /// <inheritdoc/>
    public override IValueCommand? GetUpdateCommand(string serializedValue)
    {
        var newValue = XmlStorage.FromXmlString<TElement>(serializedValue);
        return Equals(newValue, element) ? null : SetValueCommand.For(pointer, newValue);
    }

    /// <inheritdoc/>
    public override IUndoCommand GetRemoveCommand() => SetValueCommand.For(pointer, null);

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
