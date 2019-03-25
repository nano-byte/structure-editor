// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor
{
    public partial class ContainerDescription<TContainer>
    {
        /// <summary>
        /// Adds a list to the description.
        /// </summary>
        /// <typeparam name="TList">The type of elements in the list.</typeparam>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <returns>A list description, enabling you to specify explicit sub-types of <typeparamref name="TList"/> allowed in the list.</returns>
        public IListDescription<TContainer, TList> AddList<TList>(Func<TContainer, IList<TList>> getList)
            where TList : class
        {
            var listDescription = new ListDescription<TList>(getList);
            _descriptions.Add(listDescription);
            return listDescription;
        }

        /// <inheritdoc/>
        public IContainerDescription<TContainer> AddPlainList<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, TEditor editor)
            where TElement : class, IEquatable<TElement>, new()
            where TEditor : INodeEditor<TElement>, new()
        {
            var listDescription = new ListDescription<TElement>(getList);
            listDescription.AddElement(name, new TElement(), editor);
            _descriptions.Add(listDescription);
            return this;
        }

        private partial class ListDescription<TList> : DescriptionBase, IListDescription<TContainer, TList>
            where TList : class
        {
            private readonly Func<TContainer, IList<TList>> _getList;
            private readonly List<IElementDescription> _descriptions = new List<IElementDescription>();

            public ListDescription(Func<TContainer, IList<TList>> getList) => _getList = getList;

            public override IEnumerable<Node> GetNodesIn(TContainer container) => _getList(container)
                .Select(element => _descriptions
                    .Select(x => x.TryGetNode(container, _getList(container), element))
                    .WhereNotNull()
                    .FirstOrDefault())
                .Where(node => node != null);

            public override IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
                => _descriptions.Select(description => description.GetCandidatesFor(_getList(container)));
        }
    }
}
