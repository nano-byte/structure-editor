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
public class PropertyNodeCandidate<TProperty> : NodeCandidate
    where TProperty : class
{
    private readonly PropertyPointer<TProperty?> _pointer;
    private readonly Func<TProperty> _factory;

    /// <summary>
    /// Creates a new property node candidate.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="pointer">A pointer to the property.</param>
    /// <param name="factory">Callback to create a new instance of <typeparamref name="TProperty"/>.</param>
    public PropertyNodeCandidate(string name, PropertyPointer<TProperty?> pointer, Func<TProperty> factory)
        : base(name, Node.GetDescription<TProperty>())
    {
        _pointer = pointer;
        _factory = factory;
    }

    /// <inheritdoc />
    public override IValueCommand GetCreateCommand()
        => SetValueCommand.For(_pointer, _factory());
}
