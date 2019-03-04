// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Information and callbacks for a potential new node in the structure.
    /// </summary>
    public class NodeCandidate : IEquatable<NodeCandidate>
    {
        public string Name { get; }
        public string Description { get; }
        public Func<IValueCommand> GetCreateCommand { get; }

        public NodeCandidate(string name, string description, Func<IValueCommand> getCreateCommand)
        {
            Name = name;
            Description = description;
            GetCreateCommand = getCreateCommand;
        }

        public override string ToString()
            => string.IsNullOrEmpty(Description) ? Name : Name + " (" + Description + ")";

        public bool Equals(NodeCandidate other)
            => other != null
            && Name == other.Name
            && Description == other.Description;

        public override bool Equals(object obj)
            => obj != null && obj is NodeCandidate other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ (Description?.GetHashCode() ?? 0);
            }
        }
    }
}
