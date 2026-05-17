# Custom frontends

The library is split across two NuGet packages so the description model can be reused independently of WinForms.

## The two assemblies

### `NanoByte.StructureEditor`

UI-agnostic. Defines:

- <xref:NanoByte.StructureEditor.IStructureEditor`1>: the fluent entry point (`DescribeRoot`, `Describe<TContainer>`).
- <xref:NanoByte.StructureEditor.IContainerDescription`1> and <xref:NanoByte.StructureEditor.IListDescription`2>: the fluent surface for declaring properties, plain lists, and polymorphic lists.
- <xref:NanoByte.StructureEditor.Node> and its concrete subclasses: runtime nodes returned by `GetNodesIn(container)`, each carrying the methods needed to serialize, update, remove, and edit the underlying object.
- <xref:NanoByte.StructureEditor.NodeCandidate>: describes a node that *could* be created (drives "Add" menus), with `GetCreateCommand()` to materialize it.
- <xref:NanoByte.StructureEditor.INodeEditor> / <xref:NanoByte.StructureEditor.INodeEditor`1>: the toolkit-neutral contract a per-type editor implements.

The package targets `netstandard2.0`, `netstandard2.1`, and `net8.0`, so it can be referenced from non-Windows hosts.

### `NanoByte.StructureEditor.WinForms`

The reference frontend. Provides:

- <xref:NanoByte.StructureEditor.WinForms.StructureEditor`1>: the `UserControl` that hosts the tree, per-node editor panel, and XML text editor.
- <xref:NanoByte.StructureEditor.WinForms.PropertyGridNodeEditor`1>: default `INodeEditor<T>` based on `PropertyGrid`.
- <xref:NanoByte.StructureEditor.WinForms.NodeEditorBase`1>: base class for custom editors, with `Bind()` helpers that wire WinForms controls to the undo system.

## Runtime flow

1. **Description**: your code calls `DescribeRoot()` / `Describe<TContainer>()` and the fluent methods, populating a per-container-type list of node descriptions.
2. **Dispatch**: `StructureEditor<T>` stores these descriptions in an `AggregateDispatcher` keyed by container type. To build the tree, it walks the object graph and dispatches each object to the matching description list.
3. **Node materialization**: each description yields concrete <xref:NanoByte.StructureEditor.Node> instances. The tree control wraps them in `StructureTreeNode`s.
4. **Editing**: selecting a tree node calls `Node.GetEditorControl(executor)`, which returns an `INodeEditor` bound to the current `CommandManager`. Changes flow as `IUndoCommand`s onto the shared undo stack.
5. **Serialization**: the XML text editor mirrors `Node.GetSerialized()` for the selected node. Edits there round-trip via `Node.GetUpdateCommand(serializedValue)`, which replaces the target with the deserialized value as an undoable command.

## Building a non-WinForms frontend

Because `IStructureEditor<T>`, `Node`, and `INodeEditor` are toolkit-neutral, a host for Avalonia, WPF, Uno, etc. can be built without touching the core package. A minimal port needs to:

1. Implement `IStructureEditor<T>`: typically by mirroring `StructureEditor<T>`'s use of `AggregateDispatcher` to collect descriptions and dispatch by container type.
2. Render the node tree by walking `IContainerDescription<T>.GetNodesIn(container)` recursively (each `Node.Target` is itself a container for the next level).
3. Provide a toolkit-specific `INodeEditor<T>` base class (analogous to `NodeEditorBase<T>`) that adapts your toolkit's data-binding to <xref:NanoByte.Common.PropertyPointer`1> and <xref:NanoByte.Common.Undo.ICommandExecutor>.
4. Populate "Add" menus from `IContainerDescription<T>.GetCandidatesFor(container)` and execute `NodeCandidate.GetCreateCommand()` on the shared `ICommandManager<T>`.

The XML editor pane is optional. The rest of the model works the same whether or not you expose the serialized form.
