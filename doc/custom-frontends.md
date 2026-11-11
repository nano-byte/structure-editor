# Custom frontends

The library is split across multiple NuGet packages so the description model can be reused independently of the GUI toolkit. Two frontends are provided: WinForms and Eto.Forms. Both are built on the same toolkit-neutral core described below, which you can also use to build a frontend for another toolkit.

## The assemblies

### `NanoByte.StructureEditor`

UI-agnostic. Defines:

- <xref:NanoByte.StructureEditor.IStructureEditor`1>: the fluent entry point (`DescribeRoot`, `Describe<TContainer>`).
- <xref:NanoByte.StructureEditor.IContainerDescription`1> and <xref:NanoByte.StructureEditor.IListDescription`2>: the fluent surface for declaring properties, plain lists, and polymorphic lists.
- <xref:NanoByte.StructureEditor.Node> and its concrete subclasses: runtime nodes returned by `GetNodesIn(container)`, each carrying the methods needed to serialize, update, remove, and edit the underlying object.
- <xref:NanoByte.StructureEditor.NodeCandidate>: describes a node that *could* be created (drives "Add" menus), with `GetCreateCommand()` to materialize it.
- <xref:NanoByte.StructureEditor.INodeEditor> / <xref:NanoByte.StructureEditor.INodeEditor`1>: the toolkit-neutral contract a per-type editor implements.
- <xref:NanoByte.StructureEditor.StructureEditorPresenter`1>: the toolkit-neutral logic shared by all frontends (collecting descriptions, dispatching by container type, tracking the selection, add/remove and text round-tripping).

The package targets `net45` and `net10.0`. The non-Framework targets carry no Windows-specific dependencies, so it can be referenced from non-Windows hosts.

### `NanoByte.StructureEditor.WinForms`

The frontend for Windows Forms (Windows). Provides:

- <xref:NanoByte.StructureEditor.WinForms.StructureEditor`1>: the `UserControl` that hosts the tree, per-node editor panel, and text editor (see [Serialization formats](serialization-formats.md)).
- <xref:NanoByte.StructureEditor.WinForms.PropertyGridNodeEditor`1>: default `INodeEditor<T>` based on `PropertyGrid`.
- <xref:NanoByte.StructureEditor.WinForms.NodeEditorBase`1>: base class for custom editors, with `Bind()` helpers that wire WinForms controls to the undo system (see [Custom editors](custom-editors.md)).

### `NanoByte.StructureEditor.EtoForms`

The frontend for [Eto.Forms](https://github.com/picoe/Eto) (Windows, Linux, macOS). Provides:

- <xref:NanoByte.StructureEditor.EtoForms.StructureEditor`1>: the `Panel` that hosts the tree, per-node editor panel, and text editor.
- <xref:NanoByte.StructureEditor.EtoForms.PropertyEditorNodeEditor`1>: default `INodeEditor<T>` based on `PropertyEditor` from NanoByte.Common.EtoForms.
- <xref:NanoByte.StructureEditor.EtoForms.NodeEditorBase`1>: base class for custom editors, with `Bind()` helpers that wire Eto controls to the undo system (see [Custom editors](custom-editors.md)).
- <xref:NanoByte.StructureEditor.EtoForms.ITextEditor>: abstraction for the text pane. The default <xref:NanoByte.StructureEditor.EtoForms.PlainTextEditor> is a monospaced `TextArea`; set [TextEditorFactory.Default](xref:NanoByte.StructureEditor.EtoForms.TextEditorFactory.Default) during application startup, before creating any structure editors, to plug in an editor with syntax highlighting.

## Runtime flow

1. **Description**: your code calls `DescribeRoot()` / `Describe<TContainer>()` and the fluent methods, populating a per-container-type list of node descriptions.
2. **Dispatch**: `StructureEditorPresenter<T>` stores these descriptions in an `AggregateDispatcher` keyed by container type. To build the tree, the frontend walks the object graph and the presenter dispatches each object to the matching description list.
3. **Node materialization**: each description yields concrete <xref:NanoByte.StructureEditor.Node> instances. The frontend wraps them in tree items of its toolkit.
4. **Editing**: selecting a tree node calls `Node.GetEditorControl(executor)`, which returns an `INodeEditor` bound to the current `CommandManager`. Changes flow as `IUndoCommand`s onto the shared undo stack.
5. **Serialization**: the text editor mirrors `Node.GetSerialized(serializer)` for the selected node, using an <xref:NanoByte.StructureEditor.IStructureSerializer> such as <xref:NanoByte.StructureEditor.XmlStructureSerializer>. Edits there round-trip via `Node.GetUpdateCommand(serializedValue, serializer)`, which replaces the target with the deserialized value as an undoable command.

## Building another frontend

Because `IStructureEditor<T>`, `Node`, and `INodeEditor` are toolkit-neutral, a host for Avalonia, WPF, Uno, etc. can be built without touching the core package. The WinForms and Eto.Forms frontends are good references. A minimal port needs to:

1. Implement `IStructureEditor<T>` by delegating to a <xref:NanoByte.StructureEditor.StructureEditorPresenter`1>. Call its `Close()` method when your control is disposed.
2. Render the node tree by walking `GetRootNodes()` and `GetNodes(node.Target)` recursively. Report the selected node via `Select()`. Rebuild the tree after calling `Open()` and whenever `StructureChanged` is raised, using `ShouldSelect()` to restore the selection.
3. Provide a toolkit-specific `INodeEditor<T>` base class (analogous to `NodeEditorBase<T>`) that adapts your toolkit's data-binding to <xref:NanoByte.Common.PropertyPointer`1> and <xref:NanoByte.Common.Undo.ICommandExecutor>.
4. Populate "Add" menus from `GetCandidates()` and pass the chosen one to `Add()`. `null` entries mark where to place separators.

The text editor pane is optional. To provide one, show `GetSerialized()` for the selected node and pass edited text to `UpdateFromText()`. The rest of the model works the same whether or not you expose the serialized form.
