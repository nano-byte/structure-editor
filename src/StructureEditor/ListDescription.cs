// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes a type of list in the structure.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    internal class ListDescription<TContainer, TList> : Description<TContainer>, IListDescription<TContainer, TList>
        where TContainer : class
        where TList : class
    {
        private readonly Func<TContainer, IList<TList>> _getList;
        private readonly List<IElementDescription> _descriptions = new List<IElementDescription>();

        /// <summary>
        /// Creates a new list description.
        /// </summary>
        /// <param name="getList">A callback for retrieving the list from a container.</param>
        public ListDescription(Func<TContainer, IList<TList>> getList)
        {
            _getList = getList;
        }

        public override IEnumerable<Node> GetNodesIn(TContainer container)
            => _getList(container)
              .Select(element => _descriptions
                                .Select(x => x.TryGetNode(container, _getList(container), element))
                                .WhereNotNull()
                                .FirstOrDefault())
              .Where(node => node != null);

        public override IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
            => _descriptions.Select(description => description.GetCandidateFor(_getList(container)));

        /// <inheritdoc/>
        public IListDescription<TContainer, TList> AddElement<TElement, TEditor>(string name, TElement element, TEditor editor)
            where TElement : class, TList, IEquatable<TElement>, new()
            where TEditor : INodeEditor<TElement>, new()
        {
            _descriptions.Add(new ElementDescription<TElement, TEditor>(name));
            return this;
        }

        private interface IElementDescription
        {
            [CanBeNull]
            Node TryGetNode(TContainer container, IList<TList> list, TList candidate);

            NodeCandidate GetCandidateFor(IList<TList> list);
        }

        private class ElementDescription<TElement, TEditor> : IElementDescription
            where TElement : class, TList, IEquatable<TElement>, new()
            where TEditor : INodeEditor<TElement>, new()
        {
            private readonly string _name;

            public ElementDescription(string name) => _name = name;

            public Node TryGetNode(TContainer container, IList<TList> list, TList candidate)
            {
                return (candidate is TElement element)
                    ? new ListElementNode<TContainer, TList, TElement, TEditor>(_name, container, list, element)
                    : null;
            }

            public NodeCandidate GetCandidateFor(IList<TList> list)
                => new ListElementNodeCandidate<TList, TElement>(_name, list);
        }
    }
}