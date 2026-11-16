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
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
    extension<TContainer>(IContainerDescription<TContainer> description) where TContainer : class
    {
        /// <summary>
        /// Adds a property to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to the property in the container.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddProperty<TProperty>(string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer) where TProperty : class, new()
            => description.AddProperty(name, getPointer, new PropertyGridNodeEditor<TProperty>());

        /// <summary>
        /// Adds a property to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to the property in the container.</param>
        /// <param name="factory">Callback to create a new instance of <typeparamref name="TProperty"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddProperty<TProperty>(string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer, Func<TProperty> factory) where TProperty : class
            => description.AddProperty(name, getPointer, factory, new PropertyGridNodeEditor<TProperty>());

        /// <summary>
        /// Adds a required (non-nullable) property to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to the property in the container.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddRequiredProperty<TProperty>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer) where TProperty : class, new()
            => description.AddRequiredProperty(name, getPointer, new PropertyGridNodeEditor<TProperty>());

        /// <summary>
        /// Adds a required (non-nullable) property to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to the property in the container.</param>
        /// <param name="factory">Callback to create a new instance of <typeparamref name="TProperty"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddRequiredProperty<TProperty>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, Func<TProperty> factory) where TProperty : class
            => description.AddRequiredProperty(name, getPointer, factory, new PropertyGridNodeEditor<TProperty>());

        /// <summary>
        /// Adds a list to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TElement">The type of elements in the list.</typeparam>
        /// <param name="name">The name of the element type.</param>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddList<TElement>(string name, Func<TContainer, IList<TElement>> getList) where TElement : class, new()
            => description.AddList(name, getList, new PropertyGridNodeEditor<TElement>());

        /// <summary>
        /// Adds a list to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TElement">The type of elements in the list.</typeparam>
        /// <param name="name">The name of the element type.</param>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <param name="factory">Callback to create a new instance of <typeparamref name="TElement"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddList<TElement>(string name, Func<TContainer, IList<TElement>> getList, Func<TElement> factory) where TElement : class
            => description.AddList(name, getList, factory, new PropertyGridNodeEditor<TElement>());
    }

    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
    /// <typeparam name="TList">The type of elements in the list.</typeparam>
    extension<TContainer, TList>(IListDescription<TContainer, TList> description) where TContainer : class where TList : class
    {
        /// <summary>
        /// Adds a list element type to the description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
        /// <param name="name">The name of the element type.</param>
        /// <param name="factory">Callback to create a new instance of <typeparamref name="TElement"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IListDescription<TContainer, TList> AddElement<TElement>(string name, Func<TElement> factory) where TElement : class, TList
            => description.AddElement(name, factory, new PropertyGridNodeEditor<TElement>());
    }

    /// <param name="description">Describes a polymorphic single-value property.</param>
    /// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
    /// <typeparam name="TProperty">The base type of the property.</typeparam>
    extension<TContainer, TProperty>(IPropertyDescription<TContainer, TProperty> description) where TContainer : class where TProperty : class
    {
        /// <summary>
        /// Adds a value type to a polymorphic single-value property description using <see cref="PropertyGridNodeEditor{T}"/>.
        /// </summary>
        /// <typeparam name="TElement">A specific type the property value can have.</typeparam>
        /// <param name="name">The name of the value type.</param>
        /// <param name="factory">Callback to create a new instance of <typeparamref name="TElement"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IPropertyDescription<TContainer, TProperty> AddElement<TElement>(string name, Func<TElement> factory) where TElement : class, TProperty
            => description.AddElement(name, factory, new PropertyGridNodeEditor<TElement>());
    }
}
