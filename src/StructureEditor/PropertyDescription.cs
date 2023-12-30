// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using NanoByte.Common;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a type of property in the structure.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
/// <typeparam name="TProperty">The type of the property.</typeparam>
/// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
internal class PropertyDescription<TContainer, TProperty, TEditor> : Description<TContainer>
    where TContainer : class
    where TProperty : class, IEquatable<TProperty>
    where TEditor : INodeEditor<TProperty>, new()
{
    private readonly string _name;
    private readonly Func<TContainer, PropertyPointer<TProperty?>> _getPointer;
    private readonly Func<TProperty> _factory;

    /// <summary>
    /// Creates a new property description.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="getPointer">A callback for retrieving a pointer to the property in a container.</param>
    /// <param name="factory">Callback to create a new instance of <typeparamref name="TProperty"/>.</param>
    public PropertyDescription(string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer, Func<TProperty> factory)
    {
        _name = name;
        _getPointer = getPointer;
        _factory = factory;
    }

    /// <inheritdoc/>
    public override IEnumerable<Node> GetNodesIn(TContainer container)
    {
        var pointer = _getPointer(container);
        if (pointer.Value != null)
            yield return new PropertyNode<TContainer, TProperty, TEditor>(_name, container, pointer);
    }

    /// <inheritdoc/>
    public override IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
        => [new PropertyNodeCandidate<TProperty>(name, getPointer(container), factory)];
}
