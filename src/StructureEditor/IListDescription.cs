// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.StructureEditor;

/// <summary>
/// Exposes methods for configuring a list in a <see cref="ContainerDescription{TContainer}"/> in a Fluent API style.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the list.</typeparam>
/// <typeparam name="TList">The type of elements in the list.</typeparam>
public interface IListDescription<TContainer, TList>
    where TContainer : class
    where TList : class
{
    /// <summary>
    /// Adds a list element type to the description.
    /// </summary>
    /// <param name="name">The name of the element type.</param>
    /// <param name="element">Dummy element used for type inference of <typeparamref name="TElement"/>.</param>
    /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
    /// <typeparam name="TElement">The type of a specific element type in the list.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying this type of element.</typeparam>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
    IListDescription<TContainer, TList> AddElement<TElement, TEditor>(string name, TElement element, TEditor editor)
        where TElement : class, TList, IEquatable<TElement>, new()
        where TEditor : INodeEditor<TElement>, new();
}
