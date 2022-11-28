// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using NanoByte.Common;

namespace NanoByte.StructureEditor.WinForms;

/// <summary>
/// Provides WinForms-specific extensions methods for <see cref="IContainerDescription{TContainer}"/>.
/// </summary>
public static class ContainerDescriptionExtensions
{
    /// <summary>
    /// Adds a property to the description using <see cref="PropertyGridNodeEditor{T}"/>.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="getPointer">A function to retrieve a pointer to property in the container.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public static IContainerDescription<TContainer> AddProperty<TContainer, TProperty>(this IContainerDescription<TContainer> description, string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer)
        where TContainer : class
        where TProperty : class, IEquatable<TProperty>, new()
        => description.AddProperty(name, getPointer, new PropertyGridNodeEditor<TProperty>());

    /// <summary>
    /// Adds a list with only one type of element to the description using <see cref="PropertyGridNodeEditor{T}"/>.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TElement">The type of elements in the list.</typeparam>
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <param name="name">The name of the element type.</param>
    /// <param name="getList">A function to retrieve the list from the container.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public static IContainerDescription<TContainer> AddPlainList<TContainer, TElement>(this IContainerDescription<TContainer> description, string name, Func<TContainer, IList<TElement>> getList)
        where TContainer : class
        where TElement : class, IEquatable<TElement>, new()
        => description.AddPlainList(name, getList, new PropertyGridNodeEditor<TElement>());

    /// <summary>
    /// Adds a list element type to the description using <see cref="PropertyGridNodeEditor{T}"/>.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <param name="name">The name of the element type.</param>
    /// <param name="element">Dummy element used for type inference of <typeparamref name="TElement"/>.</param>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    public static IListDescription<TContainer, TList> AddElement<TContainer, TList, TElement>(this IListDescription<TContainer, TList> description, string name, TElement element)
        where TContainer : class
        where TList : class
        where TElement : class, TList, IEquatable<TElement>, new()
        => description.AddElement(name, element, new PropertyGridNodeEditor<TElement>());
}
