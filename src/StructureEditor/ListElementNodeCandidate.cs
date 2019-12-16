// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes a potential new node in the structure that points to an element in the list.
    /// </summary>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    /// <typeparam name="TElement">The type of a specific element type to add to the list.</typeparam>
    public class ListElementNodeCandidate<TList, TElement> : NodeCandidate
        where TList : notnull
        where TElement : TList, new()
    {
        private readonly IList<TList> _list;

        /// <summary>
        /// Creates a new list element node candidate.
        /// </summary>
        /// <param name="name">The name of the element type.</param>
        /// <param name="list">The list to add the element to.</param>
        public ListElementNodeCandidate(string name, IList<TList> list)
            : base(name, Node.GetDescription<TElement>())
        {
            _list = list;
        }

        /// <inheritdoc />
        public override IValueCommand GetCreateCommand()
            => new AddToCollection<TList>(_list, new TElement());
    }
}
