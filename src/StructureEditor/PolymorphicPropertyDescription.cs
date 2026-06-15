// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a polymorphic single-value property in the structure, i.e. a property whose value can be one of several types.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
/// <typeparam name="TProperty">The base type of the property.</typeparam>
/// <param name="getPointer">A callback for retrieving a pointer to the property in a container.</param>
internal class PolymorphicPropertyDescription<TContainer, TProperty>(Func<TContainer, PropertyPointer<TProperty?>> getPointer)
    : Description<TContainer>, IPropertyDescription<TContainer, TProperty>
    where TContainer : class
    where TProperty : class
{
    private readonly List<IElementDescription> _descriptions = [];

    public override IEnumerable<Node> GetNodesIn(TContainer container)
    {
        var pointer = getPointer(container);
        if (pointer.Value == null) yield break;

        var node = _descriptions
                  .Select(x => x.TryGetNode(container, pointer))
                  .WhereNotNull()
                  .FirstOrDefault();
        if (node != null) yield return node;
    }

    public override IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
        => _descriptions.Select(description => description.GetCandidateFor(getPointer(container)));

    /// <inheritdoc/>
    public IPropertyDescription<TContainer, TProperty> AddElement<TElement, TEditor>(string name, Func<TElement> factory, TEditor editor)
        where TElement : class, TProperty, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new()
    {
        _descriptions.Add(new ElementDescription<TElement, TEditor>(name, factory));
        return this;
    }

    private interface IElementDescription
    {
        Node? TryGetNode(TContainer container, PropertyPointer<TProperty?> pointer);

        NodeCandidate GetCandidateFor(PropertyPointer<TProperty?> pointer);
    }

    private class ElementDescription<TElement, TEditor>(string name, Func<TElement> factory) : IElementDescription
        where TElement : class, TProperty, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new()
    {
        public Node? TryGetNode(TContainer container, PropertyPointer<TProperty?> pointer)
            => pointer.Value is TElement element
                ? new PropertyElementNode<TContainer, TProperty, TElement, TEditor>(name, container, pointer, element)
                : null;

        public NodeCandidate GetCandidateFor(PropertyPointer<TProperty?> pointer)
            => new PropertyElementNodeCandidate<TProperty, TElement>(name, pointer, factory);
    }
}
