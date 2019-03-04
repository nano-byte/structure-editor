using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NanoByte.Common;

namespace NanoByte.StructureEditor
{
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
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        IContainerDescription<TContainer> AddProperty<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, TEditor editor)
            where TProperty : class, IEquatable<TProperty>, new()
            where TEditor : INodeEditor<TProperty>, new();

        /// <summary>
        /// Adds a property to the description. Gives the <typeparamref name="TEditor"/> access to the <typeparamref name="TContainer"/>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying the content of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getPointer">A function to retrieve a pointer to property in the container.</param>
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        IContainerDescription<TContainer> AddPropertyContainerRef<TProperty, TEditor>(string name, Func<TContainer, PropertyPointer<TProperty>> getPointer, TEditor editor)
            where TProperty : class, IEquatable<TProperty>, new()
            where TEditor : INodeEditorContainerRef<TProperty, TContainer>, new();

        /// <summary>
        /// Adds a list to the description.
        /// </summary>
        /// <typeparam name="TList">The type of elements in the list.</typeparam>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <returns>A list description, enabling you to specify explicit sub-types of <typeparamref name="TList"/> allowed in the list.</returns>
        [NotNull]
        IListDescription<TContainer, TList> AddList<TList>(Func<TContainer, IList<TList>> getList)
            where TList : class;

        /// <summary>
        /// Adds a list with only one type of element to the description.
        /// </summary>
        /// <typeparam name="TElement">The type of elements in the list.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying this type of element.</typeparam>
        /// <param name="name">The name of the element type.</param>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        IContainerDescription<TContainer> AddPlainList<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, TEditor editor)
            where TElement : class, IEquatable<TElement>, new()
            where TEditor : INodeEditor<TElement>, new();

        /// <summary>
        /// Adds a list with only one type of element to the description. Gives the <typeparamref name="TEditor"/> access to the <typeparamref name="TContainer"/>.
        /// </summary>
        /// <typeparam name="TElement">The type of elements in the list.</typeparam>
        /// <typeparam name="TEditor">An editor for modifying this type of element.</typeparam>
        /// <param name="name">The name of the element type.</param>
        /// <param name="getList">A function to retrieve the list from the container.</param>
        /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
        /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
        [NotNull]
        IContainerDescription<TContainer> AddPlainListContainerRef<TElement, TEditor>(string name, Func<TContainer, IList<TElement>> getList, TEditor editor)
            where TElement : class, IEquatable<TElement>, new()
            where TEditor : INodeEditorContainerRef<TElement, TContainer>, new();

        /// <summary>
        /// Returns information about nodes found in a specific instance of <typeparamref name="TContainer"/>.
        /// </summary>
        /// <param name="container">The container instance to look in to.</param>
        [NotNull, ItemNotNull]
        IEnumerable<Node> GetNodesIn(TContainer container);

        /// <summary>
        /// Returns information about possible new child nodes for a specific instance of <typeparamref name="TContainer"/>.
        /// </summary>
        /// <param name="container">The container instance to look at.</param>
        [NotNull, ItemNotNull]
        IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container);
    }
}
