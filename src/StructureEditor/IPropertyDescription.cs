// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Diagnostics.CodeAnalysis;

namespace NanoByte.StructureEditor;

/// <summary>
/// Exposes methods for configuring a polymorphic single-value property in a <see cref="ContainerDescription{TContainer}"/> in a Fluent API style.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the property.</typeparam>
/// <typeparam name="TProperty">The base type of the property.</typeparam>
public interface IPropertyDescription<TContainer, TProperty>
    where TContainer : class
    where TProperty : class
{
    /// <summary>
    /// Adds a property value type to the description.
    /// </summary>
    /// <param name="name">The name of the value type.</param>
    /// <param name="factory">Callback to create a new instance of <typeparamref name="TElement"/>.</param>
    /// <param name="editor">Dummy element used for type inference of <typeparamref name="TEditor"/>.</param>
    /// <typeparam name="TElement">A specific type the property value can have.</typeparam>
    /// <typeparam name="TEditor">An editor for modifying this type of value.</typeparam>
    /// <returns>The "this" pointer for use in a "Fluent API" style.</returns>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
    IPropertyDescription<TContainer, TProperty> AddElement<TElement, TEditor>(string name, Func<TElement> factory, TEditor editor)
        where TElement : class, TProperty, IEquatable<TElement>
        where TEditor : INodeEditor<TElement>, new();
}
