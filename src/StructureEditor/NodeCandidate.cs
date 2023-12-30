// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a potential new node in the structure.
/// </summary>
/// <param name="nodeType">The name of the node type.</param>
/// <param name="description">A description of the node type.</param>
public abstract class NodeCandidate(string nodeType, string? description)
{
    /// <summary>
    /// The name of the node type.
    /// </summary>
    public string NodeType { get; } = nodeType;

    /// <summary>
    /// A description of the node type.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// Gets a command for creating the new node in the structure.
    /// </summary>
    public abstract IValueCommand GetCreateCommand();

    public override string ToString()
        => string.IsNullOrEmpty(Description) ? NodeType : NodeType + " (" + Description + ")";
}
