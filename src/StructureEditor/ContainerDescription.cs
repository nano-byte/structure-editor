// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.Common;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes an object that contains nodes (properties and/or lists). Provides information about how to edit this content.
/// </summary>
/// <typeparam name="TContainer">The type of the container to be described.</typeparam>
public class ContainerDescription<TContainer> : IContainerDescription<TContainer>
    where TContainer : class
{
    private readonly List<Description<TContainer>> _descriptions = [];

    /// <inheritdoc/>
    public IContainerDescription<TContainer> AddProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer, Func<TProperty> factory, TEditor editor)
        where TProperty : class, IEquatable<TProperty>
        where TEditor : INodeEditor<TProperty>, new()
    {
        _descriptions.Add(new PropertyDescription<TContainer, TProperty, TEditor>(name, getPointer, factory));
        return this;
    }

    /// <summary>
    /// Adds a list to the description.
    /// </summary>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    /// <param name="getList">A function to retrieve the list from the container.</param>
    /// <returns>A list description, enabling you to specify explicit sub-types of <typeparamref name="TList"/> allowed in the list.</returns>
    public IListDescription<TContainer, TList> AddList<TList>(Func<TContainer, IList<TList>> getList)
        where TList : class
    {
        var listDescription = new ListDescription<TContainer, TList>(getList);
        _descriptions.Add(listDescription);
        return listDescription;
    }

    /// <inheritdoc/>
    public IContainerDescription<TContainer> AddPlainList<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, Func<TElement> factory, TEditor editor)
        where TElement : class, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new()
    {
        var listDescription = new ListDescription<TContainer, TElement>(getList);
        listDescription.AddElement(name, factory, editor);
        _descriptions.Add(listDescription);
        return this;
    }

    /// <inheritdoc/>
    public IEnumerable<Node> GetNodesIn(TContainer container)
        => _descriptions.SelectMany(description => description.GetNodesIn(container));

    /// <inheritdoc/>
    public IEnumerable<NodeCandidate?> GetCandidatesFor(TContainer container)
        => _descriptions.SelectMany(description => description.GetCandidatesFor(container));
}
