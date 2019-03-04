// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using NanoByte.Common.Undo;
using ICommandExecutor = NanoByte.Common.Undo.ICommandExecutor;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Information and callbacks for a specific node in the structure.
    /// </summary>
    public class Node : IEquatable<Node>
    {
        public string Name { get; }
        public string Description { get; }
        public object Target { get; }
        public Func<ICommandExecutor, IEditorControl> GetEditorControl { get; }
        public Func<string> GetSerialized { get; }
        public Func<string, IValueCommand> GetUpdateCommand { get; }
        public IUndoCommand RemoveCommand { get; }

        public Node(string name, string description, object target, Func<ICommandExecutor, IEditorControl> getEditorControl, Func<string> getSerialized, Func<string, IValueCommand> getUpdateCommand, IUndoCommand removeCommand)
        {
            Name = name;
            Description = description;
            Target = target;
            GetEditorControl = getEditorControl;
            GetSerialized = getSerialized;
            GetUpdateCommand = getUpdateCommand;
            RemoveCommand = removeCommand;
        }

        public override string ToString() => $"{Name}: {Target}";

        public bool Equals(Node other)
            => other != null
            && Name == other.Name
            && Description == other.Description
            && Target == other.Target;

        public override bool Equals(object obj)
            => obj != null && obj is Node other && Equals(other);

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
