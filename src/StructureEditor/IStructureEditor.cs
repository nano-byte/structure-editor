using System.Diagnostics.CodeAnalysis;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// An editor for hierarchical structures.
    /// </summary>
    /// <typeparam name="T">The type of object to edit.</typeparam>
    public interface IStructureEditor<T>
        where T : class
    {
        /// <summary>
        /// Adds a <see cref="ContainerDescription{TContainer}"/> used to describe the structure of the data being editing.
        /// </summary>
        /// <typeparam name="TContainer">The type of the container to describe.</typeparam>
        /// <returns>The <see cref="ContainerDescription{TContainer}"/> for use in a "Fluent API" style.</returns>
        IContainerDescription<TContainer> Describe<TContainer>()
            where TContainer : class;

        /// <summary>
        /// Set up handling for the root element with a generic editor.
        /// </summary>
        /// <param name="name">The name of the root element.</param>
        IContainerDescription<T> DescribeRoot(string name);

        /// <summary>
        /// Set up handling for the root element with a custom editor.
        /// </summary>
        /// <typeparam name="TEditor">An editor for modifying the content of the root.</typeparam>
        /// <param name="name">The name of the root element.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
        IContainerDescription<T> DescribeRoot<TEditor>(string name)
            where TEditor : IEditorControl<T>, new();
    }
}
