// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Undo;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Information and callbacks for a specific entry in the structure.
    /// </summary>
    public class EntryInfo : IEquatable<EntryInfo>
    {
        public string Name { get; }
        public string Description { get; }
        public object Target { get; }
        public Func<ICommandExecutor, IEditorControl> GetEditorControl { get; }
        public Func<string> ToXmlString { get; }
        public Func<string, IValueCommand> FromXmlString { get; }
        public IUndoCommand RemoveCommand { get; }

        public EntryInfo(string name, string description, object target, Func<ICommandExecutor, IEditorControl> getEditorControl, Func<string> toXmlString, Func<string, IValueCommand> fromXmlString, IUndoCommand removeCommand)
        {
            Name = name;
            Description = description;
            Target = target;
            GetEditorControl = getEditorControl;
            ToXmlString = toXmlString;
            FromXmlString = fromXmlString;
            RemoveCommand = removeCommand;
        }

        public override string ToString() => $"{Name}: {Target}";

        public bool Equals(EntryInfo other)
            => other != null
            && Name == other.Name
            && Description == other.Description
            && Target == other.Target;

        public override bool Equals(object obj)
            => obj != null && obj is EntryInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Target?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
