// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a potential new node in the structure that points to a property.
/// </summary>
/// <typeparam name="TProperty">The type of the property.</typeparam>
/// <param name="name">The name of the property.</param>
/// <param name="pointer">A pointer to the property.</param>
/// <param name="factory">Callback to create a new instance of <typeparamref name="TProperty"/>.</param>
public class PropertyNodeCandidate<TProperty>(string name, PropertyPointer<TProperty?> pointer, Func<TProperty> factory)
    : NodeCandidate(name, Node.GetDescription<TProperty>())
    where TProperty : class
{
    /// <inheritdoc />
    public override IValueCommand GetCreateCommand()
        => SetValueCommand.For(pointer, factory());
}
