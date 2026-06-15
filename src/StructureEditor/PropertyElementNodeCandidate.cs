// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a potential new node in the structure that sets a polymorphic single-value property to a specific type of value.
/// </summary>
/// <typeparam name="TProperty">The base type of the property.</typeparam>
/// <typeparam name="TElement">The specific type of value to set the property to.</typeparam>
/// <param name="name">The name of the value type.</param>
/// <param name="pointer">A pointer to the property.</param>
/// <param name="factory">Callback to create a new instance of <typeparamref name="TElement"/>.</param>
public class PropertyElementNodeCandidate<TProperty, TElement>(string name, PropertyPointer<TProperty?> pointer, Func<TElement> factory)
    : NodeCandidate(name, Node.GetDescription<TElement>())
    where TProperty : class
    where TElement : class, TProperty
{
    /// <inheritdoc />
    public override IValueCommand GetCreateCommand()
        => SetValueCommand.For(pointer, factory());
}
