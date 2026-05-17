# Undo, redo and persistence

The structure editor routes every edit (tree changes, property changes, and raw XML edits) through a single <xref:NanoByte.Common.Undo.ICommandManager`1>. The manager owns the root object, records reversible commands, and persists to disk.

## The `CommandManager` property

`StructureEditor<T>.CommandManager` is the seam between the editor and your data. It is created automatically (wrapping a new `T` from the factory) and replaced when you call `Open()`:

```csharp
editor.Open(CommandManager<AddressBook>.Load(path));      // load from XML
editor.Open(CommandManager.For(SampleData.AddressBook));  // wrap an in-memory object
```

`CommandManager.Target` is the live root object. Reads and writes are safe at any time. Replacing it raises `TargetUpdated`, which the editor uses to rebuild the tree on the next idle.

## Saving

`ICommandManager<T>` serializes the target to XML via the same machinery the inline editor uses:

```csharp
editor.CommandManager.Save(path);
```

After `Save()`, `CommandManager.UndoEnabled` becomes `false` and `Path` is updated, so you can build a "dirty" indicator and a Save/Save As flow:

```csharp
public bool Save()
{
    if (string.IsNullOrEmpty(CommandManager.Path)) return SaveAs();
    CommandManager.Save(CommandManager.Path);
    return true;
}
```

## Wiring undo/redo into your UI

`StructureEditor<T>` exposes `Undo()` and `Redo()` methods that already do the right thing in two contexts:

- If the embedded XML text editor has its own undo stack populated (i.e., the user is typing), they undo/redo characters there.
- Otherwise they delegate to `CommandManager.Undo()` / `Redo()`, reversing the last structural or property change.

Bind them to menu items or hot-keys:

```csharp
new ToolStripMenuItem("&Undo") {ShortcutKeys = Keys.Control | Keys.Z}
    .Click += (_, _) => editor.Undo();
```

## Confirming unsaved changes on close

Check `CommandManager.UndoEnabled` to detect pending changes:

```csharp
public bool Closing()
{
    if (!CommandManager.UndoEnabled) return true;

    return Msg.YesNoCancel(this, "Save changes?", MsgSeverity.Warn) switch
    {
        DialogResult.Yes => Save(),
        DialogResult.No  => true,
        _                => false
    };
}
```

## Custom editors and the command system

Inside a <xref:NanoByte.StructureEditor.WinForms.NodeEditorBase`1>, the `Bind()` helpers route changes through <xref:NanoByte.Common.Undo.ICommandExecutor> (`CommandExecutor`), which is the same instance as the structure editor's `CommandManager`. You generally do not need to interact with it directly. If you build a control without a matching `Bind()` overload, follow the same pattern:

```csharp
if (CommandExecutor == null) pointer.Value = newValue;
else CommandExecutor.Execute(SetValueCommand.For(pointer, newValue));
```

This keeps property edits, tree add/remove, and raw XML edits on one undo stack, so a single Ctrl+Z reverses whichever happened last.
