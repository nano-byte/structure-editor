// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using NanoByte.Common;

namespace NanoByte.StructureEditor;

/// <summary>
/// Provides extensions methods for <see cref="IContainerDescription{TContainer}"/>.
/// </summary>
public static class ContainerDescriptionExtensions
{
    /// <summary>
    /// Adds a property to the description.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="getPointer">A function to retrieve a pointer to property in the container.</param>
    /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public static IContainerDescription<TContainer> AddProperty<TContainer, TProperty, TEditor>(this IContainerDescription<TContainer> description, string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer, TEditor editor)
        where TContainer : class
        where TProperty : class, IEquatable<TProperty>, new()
        where TEditor : INodeEditor<TProperty>, new()
        => description.AddProperty(name, getPointer, () => new TProperty(), editor);

    /// <summary>
    /// Adds a list with only one type of element to the description.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TElement">The type of elements in the list.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <param name="name">The name of the element type.</param>
    /// <param name="getList">A function to retrieve the list from the container.</param>
    /// <param name="editor"></param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public static IContainerDescription<TContainer> AddPlainList<TContainer, TElement, TEditor>(this IContainerDescription<TContainer> description, string name, Func<TContainer, IList<TElement>> getList, TEditor editor)
        where TContainer : class
        where TElement : class, IEquatable<TElement>, new()
        where TEditor : INodeEditor<TElement>, new()
        => description.AddPlainList(name, getList, () => new TElement(), editor);
}
