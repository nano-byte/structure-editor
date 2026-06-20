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
        _descriptions.Add(new PropertyDescription<TContainer, TProperty, TEditor>(name, getPointer, factory, nullable: true));
        return this;
    }

    /// <inheritdoc/>
    public IContainerDescription<TContainer> AddRequiredProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, Func<TProperty> factory, TEditor editor)
        where TProperty : class, IEquatable<TProperty>
        where TEditor : INodeEditor<TProperty>, new()
    {
        _descriptions.Add(new PropertyDescription<TContainer, TProperty, TEditor>(name, AsNullable(getPointer), factory, nullable: false));
        return this;
    }

    /// <inheritdoc/>
    public IPropertyDescription<TContainer, TProperty> AddPolymorphicProperty<TProperty>(Func<TContainer, PropertyPointer<TProperty?>> getPointer)
        where TProperty : class
    {
        var propertyDescription = new PolymorphicPropertyDescription<TContainer, TProperty>(getPointer, nullable: true);
        _descriptions.Add(propertyDescription);
        return propertyDescription;
    }

    /// <inheritdoc/>
    public IPropertyDescription<TContainer, TProperty> AddRequiredPolymorphicProperty<TProperty>(Func<TContainer, PropertyPointer<TProperty>> getPointer)
        where TProperty : class
    {
        var propertyDescription = new PolymorphicPropertyDescription<TContainer, TProperty>(AsNullable(getPointer), nullable: false);
        _descriptions.Add(propertyDescription);
        return propertyDescription;
    }

    /// <summary>
    /// Widens a pointer to a non-nullable property into a pointer that exposes a nullable value, for use by the internal property descriptions.
    /// </summary>
    private static Func<TContainer, PropertyPointer<TProperty?>> AsNullable<TProperty>(Func<TContainer, PropertyPointer<TProperty>> getPointer)
        where TProperty : class
        => container =>
        {
            var pointer = getPointer(container);
            return new PropertyPointer<TProperty?>(() => pointer.Value, value => pointer.Value = value!);
        };

    /// <inheritdoc/>
    public IListDescription<TContainer, TList> AddPolymorphicList<TList>(Func<TContainer, IList<TList>> getList)
        where TList : class
    {
        var listDescription = new ListDescription<TContainer, TList>(getList);
        _descriptions.Add(listDescription);
        return listDescription;
    }

    /// <inheritdoc/>
    public IContainerDescription<TContainer> AddList<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, Func<TElement> factory, TEditor editor)
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
    public IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
        => _descriptions.SelectMany(description => description.GetCandidatesFor(container));
}
