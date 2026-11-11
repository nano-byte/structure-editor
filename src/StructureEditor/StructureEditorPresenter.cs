// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using NanoByte.Common;
using NanoByte.Common.Collections;
using NanoByte.Common.Dispatch;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// Toolkit-neutral logic for an editor for hierarchical structures. Used by GUI frontends to implement <see cref="IStructureEditor{T}"/>.
/// </summary>
/// <remarks>
/// Holds the descriptions of the structure, the object being edited and the current selection.
/// The frontend is responsible for displaying <see cref="GetRootNodes"/>/<see cref="GetNodes"/> as a tree, reporting the selected node via <see cref="Select"/> and rebuilding the tree when <see cref="StructureChanged"/> is raised.
/// </remarks>
/// <typeparam name="T">The type of object to edit.</typeparam>
public class StructureEditorPresenter<T>
    where T : class
{
    private readonly Func<T> _factory;

    /// <summary>
    /// Creates a new structure editor presenter.
    /// </summary>
    /// <param name="factory">Callback to create a new instance of <typeparamref name="T"/>.</param>
    public StructureEditorPresenter(Func<T> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        CommandManager = new CommandManager<T>(factory());
    }

    #region Describe
    private readonly AggregateDispatcher<object, Node?> _getNodes = new();

    private readonly AggregateDispatcher<object, NodeCandidate?> _getCandidates = new();

    /// <inheritdoc cref="IStructureEditor{T}.Describe{TContainer}"/>
    public IContainerDescription<TContainer> Describe<TContainer>()
        where TContainer : class
    {
        var description = new ContainerDescription<TContainer>();
        _getNodes.Add<TContainer>(container => description.GetNodesIn(container).ToList());
        // null candidates mark the boundaries between the descriptions of different container types
        _getCandidates.Add<TContainer>(container => description.GetCandidatesFor(container).Concat(new NodeCandidate?[] {null}).ToList());
        return description;
    }

    /// <inheritdoc cref="IStructureEditor{T}.DescribeRoot{TEditor}"/>
    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Generics used as type-safe reflection replacement.")]
    public IContainerDescription<T> DescribeRoot<TEditor>(string name)
        where TEditor : INodeEditor<T>, new()
    {
        // Use CommandManager as root rather than Target, to allow the entire Target to be replaced during editing.
        // Described as required, so that the root can neither be removed nor re-created.
        Describe<ICommandManager<T>>()
           .AddRequiredProperty(name, _ => PropertyPointer.For(() => CommandManager.Target!, value => CommandManager.Target = value), _factory, new TEditor());

        return Describe<T>();
    }
    #endregion

    #region Target
    /// <summary>
    /// Holds the object being edited and manages undo/redo operations on it.
    /// </summary>
    public ICommandManager<T> CommandManager { get; private set; }

    private IStructureSerializer _serializer = XmlStructureSerializer.Instance;

    /// <summary>
    /// The serializer used for the text representation of the selected node. Defaults to <see cref="XmlStructureSerializer"/>.
    /// </summary>
    /// <remarks>Set automatically by <see cref="Open"/> when passed a <see cref="SerializingCommandManager{T}"/>.</remarks>
    public IStructureSerializer Serializer
    {
        get => _serializer;
        set => _serializer = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Raised when the structure was modified and the tree needs to be rebuilt.
    /// </summary>
    /// <remarks>May be raised multiple times in quick succession. Frontends should coalesce these into a single rebuild.</remarks>
    public event Action? StructureChanged;

    private void OnTargetUpdated(IUndoCommand command) => StructureChanged?.Invoke();

    /// <summary>
    /// Opens an object for editing using the specified <see cref="ICommandManager{T}"/>.
    /// </summary>
    /// <param name="commandManager">Holds the object being edited and manages undo/redo operations on it. If this is a <see cref="SerializingCommandManager{T}"/> its <see cref="SerializingCommandManager{T}.Serializer"/> is also applied to <see cref="Serializer"/>.</param>
    /// <remarks>Does not raise <see cref="StructureChanged"/>. The frontend needs to rebuild its tree after calling this.</remarks>
    public void Open(ICommandManager<T> commandManager)
    {
        if (commandManager == null) throw new ArgumentNullException(nameof(commandManager));

        CommandManager.TargetUpdated -= OnTargetUpdated;
        CommandManager = commandManager;
        CommandManager.TargetUpdated += OnTargetUpdated;

        if (commandManager is SerializingCommandManager<T> {Serializer: var serializer}) Serializer = serializer;
    }

    /// <summary>
    /// Stops listening for changes to the <see cref="CommandManager"/>. Call this when the frontend is disposed.
    /// </summary>
    public void Close()
        => CommandManager.TargetUpdated -= OnTargetUpdated;
    #endregion

    #region Nodes
    /// <summary>
    /// Returns the top-level nodes of the structure.
    /// </summary>
    public IReadOnlyList<Node> GetRootNodes()
        => GetNodes(CommandManager);

    /// <summary>
    /// Returns the child nodes of a <see cref="Node.Target"/>.
    /// </summary>
    /// <param name="target">The <see cref="Node.Target"/> to look into; can be <c>null</c>.</param>
    public IReadOnlyList<Node> GetNodes(object? target)
        => target == null
            ? new List<Node>()
            : _getNodes.Dispatch(target).WhereNotNull().ToList();

    /// <summary>
    /// Compares <see cref="Node.Target"/>s by reference, so that they can be tracked across tree rebuilds (e.g., to restore expansion state).
    /// </summary>
    public static IEqualityComparer<object> TargetComparer { get; } = new ReferenceComparer();

    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
    #endregion

    #region Selection
    private object? _selectedTarget;
    private object? _editingTarget;
    private object? _serializedTarget;

    /// <summary>
    /// The currently selected node; <c>null</c> if nothing is selected.
    /// </summary>
    public Node? SelectedNode { get; private set; }

    /// <summary>
    /// The target object of the currently selected node; <c>null</c> if nothing is selected.
    /// </summary>
    public object? SelectedTarget => SelectedNode?.Target;

    /// <summary>
    /// Determines whether a node should be selected after rebuilding the tree, in order to retain the previous selection or to select a newly created node.
    /// </summary>
    public bool ShouldSelect(Node node)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));
        return node.Target == _selectedTarget;
    }

    /// <summary>
    /// Reports that a different node has been selected in the tree.
    /// </summary>
    /// <param name="node">The newly selected node; <c>null</c> if nothing is selected.</param>
    /// <param name="editorChanged"><c>true</c> if the editor control needs to be replaced with <see cref="GetEditorControl"/>; <c>false</c> if the existing editor control is still valid and only needs to be refreshed.</param>
    /// <param name="textChanged"><c>true</c> if the text representation needs to be replaced with <see cref="GetSerialized"/>; <c>false</c> if the text currently shown already represents the node.</param>
    public void Select(Node? node, out bool editorChanged, out bool textChanged)
    {
        SelectedNode = node;
        _selectedTarget = node?.Target;

        editorChanged = _selectedTarget != _editingTarget;
        _editingTarget = _selectedTarget;

        textChanged = _selectedTarget != _serializedTarget;
        _serializedTarget = null;
    }

    /// <summary>
    /// Gets a GUI control for editing the currently selected node; <c>null</c> if nothing is selected.
    /// </summary>
    public INodeEditor? GetEditorControl()
        => SelectedNode?.GetEditorControl(CommandManager);
    #endregion

    #region Add/remove
    /// <summary>
    /// Returns the nodes that can be added as children of the currently selected node.
    /// </summary>
    /// <returns>A list of candidates. <c>null</c> entries mark the boundaries between groups (e.g., for separators in a menu); there are no leading, trailing or consecutive <c>null</c>s.</returns>
    public IReadOnlyList<NodeCandidate?> GetCandidates()
    {
        var result = new List<NodeCandidate?>();
        if (SelectedNode?.Target is {} target)
        {
            foreach (var candidate in _getCandidates.Dispatch(target))
            {
                if (candidate != null) result.Add(candidate);
                // Suppress leading and consecutive separators
                else if (result.Count != 0 && result[result.Count - 1] != null) result.Add(null);
            }

            // Suppress trailing separator
            if (result.Count != 0 && result[result.Count - 1] == null) result.RemoveAt(result.Count - 1);
        }
        return result;
    }

    /// <summary>
    /// Adds a new node to the structure and marks it for selection once the tree has been rebuilt.
    /// </summary>
    /// <param name="candidate">One of the candidates returned by <see cref="GetCandidates"/>.</param>
    public void Add(NodeCandidate candidate)
    {
        if (candidate == null) throw new ArgumentNullException(nameof(candidate));

        var command = candidate.GetCreateCommand();
        _selectedTarget = command.Value;
        CommandManager.Execute(command);
    }

    /// <summary>
    /// Gets a command for removing the currently selected node from the structure; <c>null</c> if nothing is selected or the node cannot be removed.
    /// </summary>
    public IUndoCommand? GetRemoveCommand()
        => SelectedNode?.GetRemoveCommand();
    #endregion

    #region Text
    /// <summary>
    /// Returns the serialized representation of the currently selected node; empty if nothing is selected.
    /// </summary>
    public string GetSerialized()
        => SelectedNode?.GetSerialized(Serializer) ?? "";

    /// <summary>
    /// Replaces the currently selected node with a new value entered as text.
    /// </summary>
    /// <param name="serializedValue">A serialized representation of the new value.</param>
    /// <returns><c>true</c> if the structure was modified; <c>false</c> if nothing is selected or the new value is equivalent to the current one.</returns>
    /// <exception cref="Exception">The <paramref name="serializedValue"/> is not a valid serialized representation of the node.</exception>
    public bool UpdateFromText(string serializedValue)
    {
        var command = SelectedNode?.GetUpdateCommand(serializedValue, Serializer);
        if (command == null) return false;

        // The text already represents the new value and must not be replaced while the user is typing
        _serializedTarget = _selectedTarget = command.Value;
        CommandManager.Execute(command);
        return true;
    }
    #endregion
}
