# Undo, redo and persistence

The structure editor routes every edit (tree changes, property changes, and raw text edits) through a single <xref:NanoByte.Common.Undo.ICommandManager`1>. The manager owns the root object, records reversible commands, and persists to disk.

## The `CommandManager` property

`StructureEditor<T>.CommandManager` is the seam between the editor and your data. It is created automatically (wrapping a new `T` from the factory) and replaced when you call `Open()`:

```csharp
editor.Open(CommandManager<AddressBook>.Load(path));      // load from XML
editor.Open(CommandManager.For(SampleData.AddressBook));  // wrap an in-memory object
```

`CommandManager.Target` is the live root object. Reads and writes are safe at any time. Replacing it raises `TargetUpdated`, which the editor uses to rebuild the tree asynchronously.

## Saving

`CommandManager<T>` serializes the target to XML. Use <xref:NanoByte.StructureEditor.SerializingCommandManager`1> to save in another format, such as JSON (see [Serialization formats](serialization-formats.md)):

```csharp
editor.CommandManager.Save(path);
```

After `Save()`, `CommandManager.UndoEnabled` becomes `false` and `Path` is updated, so you can build a "dirty" indicator and a Save/Save As flow:

```csharp
public bool Save()
{
    CommitPendingChanges(); // Eto.Forms only
    if (string.IsNullOrEmpty(CommandManager.Path)) return SaveAs();
    CommandManager.Save(CommandManager.Path);
    return true;
}
```

With Eto.Forms, input in a control that still has the focus may not have been applied yet. Call `CommitPendingChanges()` before reading or saving `CommandManager.Target` to make sure the latest input is included.

## Wiring undo/redo into your UI

`StructureEditor<T>` exposes `Undo()` and `Redo()` methods. They delegate to `CommandManager.Undo()` / `Redo()`, reversing the last structural or property change.

# [WinForms](#tab/winforms)

If the embedded text editor has its own undo stack populated (i.e., the user is typing), `Undo()` and `Redo()` undo/redo characters there instead.

Bind them to menu items or hot-keys:

```csharp
new ToolStripMenuItem("&Undo") {ShortcutKeys = Keys.Control | Keys.Z}
    .Click += (_, _) => editor.Undo();
```

# [Eto.Forms](#tab/etoforms)

`Undo()` and `Redo()` first apply any pending input, so that it can be undone as well.

Bind them to menu items or hot-keys:

```csharp
var item = new ButtonMenuItem {Text = "&Undo", Shortcut = Application.Instance.CommonModifier | Keys.Z};
item.Click += (_, _) => editor.Undo();
```

---

## Confirming unsaved changes on close

Check `CommandManager.UndoEnabled` to detect pending changes:

```csharp
public bool Closing()
{
    CommitPendingChanges(); // Eto.Forms only
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

Inside a `NodeEditorBase<T>` (<xref:NanoByte.StructureEditor.WinForms.NodeEditorBase`1> or <xref:NanoByte.StructureEditor.EtoForms.NodeEditorBase`1>), the `Bind()` helpers route changes through <xref:NanoByte.Common.Undo.ICommandExecutor> (`CommandExecutor`), which is the same instance as the structure editor's `CommandManager`. You generally do not need to interact with it directly. If you build a control without a matching `Bind()` overload, pass the new value to the `protected` `SetValue()` helper:

```csharp
SetValue(pointer, newValue);
```

It executes a `SetValueCommand` on the `CommandExecutor`. If no executor is attached, it sets the pointer directly. This is equivalent to:

```csharp
if (CommandExecutor == null) pointer.Value = newValue;
else CommandExecutor.Execute(SetValueCommand.For(pointer, newValue));
```

Use this longer form if you need to execute a different kind of command.

This keeps property edits, tree add/remove, and raw text edits on one undo stack, so a single Ctrl+Z reverses whichever happened last.
