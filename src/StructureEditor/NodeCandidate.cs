// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes a potential new node in the structure.
    /// </summary>
    public abstract class NodeCandidate
    {
        /// <summary>
        /// The name of the node type.
        /// </summary>
        public string NodeType { get; }

        /// <summary>
        /// A description of the node type.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Creates a new node candidate.
        /// </summary>
        /// <param name="nodeType">The name of the node type.</param>
        /// <param name="description">A description of the node type.</param>
        protected NodeCandidate(string nodeType, string? description)
        {
            NodeType = nodeType;
            Description = description;
        }

        /// <summary>
        /// Gets a command for creating the new node in the structure.
        /// </summary>
        public abstract IValueCommand GetCreateCommand();

        public override string ToString()
            => string.IsNullOrEmpty(Description) ? NodeType : NodeType + " (" + Description + ")";
    }
}
