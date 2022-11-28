// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes an element of the structure.
/// </summary>
/// <typeparam name="TContainer">The type of the container containing the element.</typeparam>
internal abstract class Description<TContainer>
    where TContainer : class
{
    /// <summary>
    /// Returns information about nodes of this type found in a specific instance of <typeparamref name="TContainer"/>.
    /// </summary>
    /// <param name="container">The container instance to look in to.</param>
    public abstract IEnumerable<Node> GetNodesIn(TContainer container);

    /// <summary>
    /// Returns information about possible new child nodes of this type for a specific instance of <typeparamref name="TContainer"/>.
    /// </summary>
    /// <param name="container">The container instance to look at.</param>
    public abstract IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container);
}
