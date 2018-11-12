// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Information and callbacks for a potential new child node in the structure.
    /// </summary>
    public class ChildInfo : IEquatable<ChildInfo>
    {
        public string Name { get; }
        public string Description { get; }
        public Func<IValueCommand> Create { get; }

        public ChildInfo(string name, string description, Func<IValueCommand> create)
        {
            Name = name;
            Description = description;
            Create = create;
        }

        public override string ToString()
            => string.IsNullOrEmpty(Description) ? Name : Name + " (" + Description + ")";

        public bool Equals(ChildInfo other)
            => other != null
            && Name == other.Name
            && Description == other.Description;

        public override bool Equals(object obj)
            => obj != null && obj is ChildInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ (Description?.GetHashCode() ?? 0);
            }
        }
    }
}
