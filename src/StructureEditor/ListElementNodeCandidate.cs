// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a potential new node in the structure that points to an element in the list.
/// </summary>
/// <typeparam name="TList">The type of elements in the list.</typeparam>
/// <typeparam name="TElement">The type of a specific element type to add to the list.</typeparam>
public class ListElementNodeCandidate<TList, TElement> : NodeCandidate
    where TList : notnull
    where TElement : TList
{
    private readonly IList<TList> _list;
    private readonly Func<TElement> _factory;

    /// <summary>
    /// Creates a new list element node candidate.
    /// </summary>
    /// <param name="name">The name of the element type.</param>
    /// <param name="list">The list to add the element to.</param>
    /// <param name="factory">Callback to create a new instance of <typeparamref name="TElement"/>.</param>
    public ListElementNodeCandidate(string name, IList<TList> list, Func<TElement> factory)
        : base(name, Node.GetDescription<TElement>())
    {
        _list = list;
        _factory = factory;
    }

    /// <inheritdoc />
    public override IValueCommand GetCreateCommand()
        => new AddToCollection<TList>(_list, _factory());
}
