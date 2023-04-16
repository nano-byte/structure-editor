using System;
using System.Collections.Generic;
using NanoByte.Common;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes an object that contains nodes (properties and/or lists). Provides information about how to edit this content.
/// </summary>
/// <typeparam name="TContainer">The type of the container to be described.</typeparam>
public interface IContainerDescription<TContainer> where TContainer : class
{
    /// <summary>
    /// Adds a property to the description.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
    /// <param name="name">The name of the property.</param>
    /// <param name="getPointer">A function to retrieve a pointer to property in the container.</param>
    /// <param name="factory">Callback to create a new instance of the property.</param>
    /// <param name="editor">Dummy element used for type inference of the editor class.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    IContainerDescription<TContainer> AddProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer, Func<TProperty> factory, TEditor editor)
        where TProperty : class, IEquatable<TProperty>
        where TEditor : INodeEditor<TProperty>, new();

    /// <summary>
    /// Adds a list to the description.
    /// </summary>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    /// <param name="getList">A function to retrieve the list from the container.</param>
    /// <returns>A list description, enabling you to specify explicit sub-types of <c>TList</c> allowed in the list.</returns>
    IListDescription<TContainer, TList> AddList<TList>(Func<TContainer, IList<TList>> getList)
        where TList : class;

    /// <summary>
    /// Adds a list with only one type of element to the description.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the list.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying this type of element.</typeparam>
    /// <param name="name">The name of the element type.</param>
    /// <param name="getList">A function to retrieve the list from the container.</param>
    /// <param name="factory">Callback to create a new instance of the element.</param>
    /// <param name="editor">Dummy element used for type inference of the editor class.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    IContainerDescription<TContainer> AddPlainList<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, Func<TElement> factory, TEditor editor)
        where TElement : class, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new();

    /// <summary>
    /// Returns information about nodes found in a specific instance of the container.
    /// </summary>
    /// <param name="container">The container instance to look in to.</param>
    IEnumerable<Node> GetNodesIn(TContainer container);

    /// <summary>
    /// Returns information about possible new child nodes for a specific instance of the container.
    /// </summary>
    /// <param name="container">The container instance to look at.</param>
    IEnumerable<NodeCandidate?> GetCandidatesFor(TContainer container);
}
