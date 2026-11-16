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
    /// <param name="description">Describes an object that contains nodes (properties and/or lists).</param>
    /// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
    extension<TContainer>(IContainerDescription<TContainer> description) where TContainer : class
    {
        /// <summary>
        /// Adds a property to the description.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to the property in the container.</param>
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty?>> getPointer, TEditor editor) where TProperty : class, new()
            where TEditor : INodeEditor<TProperty>, new()
            => description.AddProperty(name, getPointer, () => new TProperty(), editor);

        /// <summary>
        /// Adds a required (non-nullable) property to the description.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to the property in the container.</param>
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddRequiredProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, TEditor editor) where TProperty : class, new()
            where TEditor : INodeEditor<TProperty>, new()
            => description.AddRequiredProperty(name, getPointer, () => new TProperty(), editor);

        /// <summary>
        /// Adds a list to the description.
        /// </summary>
        /// <typeparam name="TElement">The type of elements in the list.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying the content of the element.</typeparam>
        /// <param name="name">The name of the element type.</param>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        public IContainerDescription<TContainer> AddList<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, TEditor editor) where TElement : class, new()
            where TEditor : INodeEditor<TElement>, new()
            => description.AddList(name, getList, () => new TElement(), editor);
    }
}
