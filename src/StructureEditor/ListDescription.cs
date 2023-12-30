// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a type of list in the structure.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
/// <typeparam name="TList">The type of elements in the list.</typeparam>
/// <param name="getList">A callback for retrieving the list from a container.</param>
internal class ListDescription<TContainer, TList>(Func<TContainer, IList<TList>> getList)
    : Description<TContainer>, IListDescription<TContainer, TList>
    where TContainer : class
    where TList : class
{
    private readonly List<IElementDescription> _descriptions = [];

    public override IEnumerable<Node> GetNodesIn(TContainer container)
    {
        foreach (var element in getList(container))
        {
            var node = _descriptions
                      .Select(x => x.TryGetNode(container, getList(container), element))
                      .WhereNotNull()
                      .FirstOrDefault();
            if (node != null) yield return node;
        }
    }

    public override IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
        => _descriptions.Select(description => description.GetCandidateFor(getList(container)));

    /// <inheritdoc/>
    public IListDescription<TContainer, TList> AddElement<TElement, TEditor>(string name, Func<TElement> factory, TEditor editor)
        where TElement : class, TList, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new()
    {
        _descriptions.Add(new ElementDescription<TElement, TEditor>(name, factory));
        return this;
    }

    private interface IElementDescription
    {
        Node? TryGetNode(TContainer container, IList<TList> list, TList candidate);

        NodeCandidate GetCandidateFor(IList<TList> list);
    }

    private class ElementDescription<TElement, TEditor> : IElementDescription
        where TElement : class, TList, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new()
    {
        private readonly string _name;
        private readonly Func<TElement> _factory;

        public ElementDescription(string name, Func<TElement> factory)
        {
            _name = name;
            _factory = factory;
        }

        public Node? TryGetNode(TContainer container, IList<TList> list, TList candidate)
        {
            return (candidate is TElement element)
                ? new ListElementNode<TContainer, TList, TElement, TEditor>(_name, container, list, element)
                : null;
        }

        public NodeCandidate GetCandidateFor(IList<TList> list)
            => new ListElementNodeCandidate<TList, TElement>(_name, list, _factory);
    }
}
