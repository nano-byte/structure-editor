// Copyright Bastian Eicher
// Licensed under the MIT License

using NanoByte.Common;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Describes a potential new node in the structure that points to a property.
/// </summary>
/// <typeparam name="TProperty">The type of the property.</typeparam>
public class PropertyNodeCandidate<TProperty> : NodeCandidate
    where TProperty : class, new()
{
    private readonly PropertyPointer<TProperty?> _pointer;

    /// <summary>
    /// Creates a new property node candidate.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="pointer">A pointer to the property.</param>
    public PropertyNodeCandidate(string name, PropertyPointer<TProperty?> pointer)
        : base(name, Node.GetDescription<TProperty>())
    {
        _pointer = pointer;
    }

    /// <inheritdoc />
    public override IValueCommand GetCreateCommand()
        => SetValueCommand.For(_pointer, new TProperty());
}
