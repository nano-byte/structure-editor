---
title: Home
---

# NanoByte Structure Editor

NanoByte Structure Editor is a .NET library that helps you build split-screen editors for your data structures, consisting of:

1. a collapsible tree-view of the data structure,
2. a graphical editor for the currently selected node in the tree (auto-generated or [custom](custom-editors.md)) and
3. a text editor with a serialized ([XML or JSON](serialization-formats.md)) representation of the currently selected node.

This allows you to create an IDE-like experience for your users when editing complex domain specific languages, configuration files, etc..

![](screenshot.png)

## Choosing a frontend

The library is available for two GUI toolkits. Both share the same API for describing your data structure, storage and undo/redo. Add a reference to the NuGet package matching your application:

|                     | [NanoByte.StructureEditor.WinForms](https://www.nuget.org/packages/NanoByte.StructureEditor.WinForms/) | [NanoByte.StructureEditor.EtoForms](https://www.nuget.org/packages/NanoByte.StructureEditor.EtoForms/)                                   |
| ------------------- | ------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------------------------------------- |
| GUI toolkit         | Windows Forms                                                                                          | [Eto.Forms](https://github.com/picoe/Eto)                                                                                                |
| Platforms           | Windows                                                                                                | Windows, Linux, macOS                                                                                                                    |
| Frameworks          | .NET Framework 4.5+, .NET 10.0+                                                                        | .NET Framework 4.7.2+, .NET 10.0+                                                                                                        |
| Editor control      | <xref:NanoByte.StructureEditor.WinForms.StructureEditor`1>                                             | <xref:NanoByte.StructureEditor.EtoForms.StructureEditor`1>                                                                               |
| Default node editor | <xref:NanoByte.StructureEditor.WinForms.PropertyGridNodeEditor`1>                                      | <xref:NanoByte.StructureEditor.EtoForms.PropertyEditorNodeEditor`1>                                                                      |
| Text editor         | [ICSharpCode.TextEditor](https://github.com/nano-byte/ICSharpCode.TextEditor) with syntax highlighting | Plain monospaced text box, replaceable via [TextEditorFactory.Default](xref:NanoByte.StructureEditor.EtoForms.TextEditorFactory.Default) |
| Sample              | [Sample.WinForms](https://github.com/nano-byte/structure-editor/tree/master/src/Sample.WinForms)       | [Sample.EtoForms](https://github.com/nano-byte/structure-editor/tree/master/src/Sample.EtoForms)                                         |

An Eto.Forms application additionally needs an `Eto.Platform.*` package for each platform it runs on (e.g., `Eto.Platform.Windows`, `Eto.Platform.Gtk`, `Eto.Platform.Mac64`).

Code snippets in this documentation work with both packages unless shown in separate tabs or marked otherwise. Import the namespace matching your package (`NanoByte.StructureEditor.WinForms` or `NanoByte.StructureEditor.EtoForms`).

## Usage

### Initialization

Create an instance of `StructureEditor<T>` and add it to your Form:

# [WinForms](#tab/winforms)

```csharp
var editor = new StructureEditor<MyData>();
Controls.Add(editor);
```

# [Eto.Forms](#tab/etoforms)

```csharp
var editor = new StructureEditor<MyData>();
form.Content = editor;
```

---

Alternatively, you may want to derive your own class from `StructureEditor<T>`. This lets you keep the description of your data structure in one place and, with WinForms, use the graphical designer in Visual Studio (which does not support generic types) to place the editor on your Form.
```csharp
public class MyDataEditor : StructureEditor<MyData>
{}
```

You need to "describe" your data structure to the editor. You can do this directly after instantiating the editor or in the constructor of your derived class.
- Call [DescribeRoot()](xref:NanoByte.StructureEditor.IStructureEditor`1#NanoByte_StructureEditor_IStructureEditor_1_DescribeRoot_System_String_) and then use the fluent API provided as a return value to describe the properties on your main data type.
- Call [Describe<TContainer>()](xref:NanoByte.StructureEditor.IStructureEditor`1#NanoByte_StructureEditor_IStructureEditor_1_Describe__1) to describe the properties on a data type `TContainer` exposed by another property. You can use multiple calls with different type parameters to describe arbitrarily deep hierarchies.  

The fluent API provides the following methods:
- `.AddProperty()` describes a property.
- `.AddRequiredProperty()` describes a property that cannot be set to `null`.
- `.AddPolymorphicProperty()` describes a [polymorphic property](polymorphic-properties.md). After calling it you chain `.AddElement()` calls for each specific type the property can hold.
- `.AddRequiredPolymorphicProperty()` describes a polymorphic property that cannot be set to `null`.
- `.AddList()` describes a list.
- `.AddPolymorphicList()` describes a [polymorphic list](polymorphic-lists.md). After calling it you need to chain `.AddElement()` calls for each specific type of element the list can hold.

There are numerous overloads for each of these methods, e.g., allowing you to specify a [custom editor control](custom-editors.md) for a data type or to keep the auto-generated one.

```csharp
editor.DescribeRoot("Address Book")
      .AddList("Group", x => x.Groups);
editor.Describe<IContactContainer>()
      .AddList("Contact", x => x.Contacts);
editor.Describe<Contact>()
      .AddRequiredProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress))
      .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress))
      .AddPolymorphicList(x => x.PhoneNumbers)
      .AddElement("Landline Number", () => new LandlineNumber())
      .AddElement("Mobile Number", () => new MobileNumber());
```

### Storage

Use the `Open()` method to load an XML file into the editor:
```csharp
editor.Open(CommandManager<AddressBook>.Load(path));
```

Use the [Save()](xref:NanoByte.Common.Undo.ICommandManager`1#NanoByte_Common_Undo_ICommandManager_1_Save_System_String_) method on the `CommandManager` property to save the editor's content as an XML file:
```csharp
editor.CommitPendingChanges(); // Eto.Forms only
editor.CommandManager.Save(path);
```

With Eto.Forms, input in a control that still has the focus may not have been applied yet. Call `CommitPendingChanges()` before saving to make sure the latest input is included.

See [Undo, redo and persistence](undo-redo.md) for details.

### Serialization formats

The text editor uses XML by default. To use JSON instead, set the `Serializer` property:
```csharp
editor.Serializer = JsonStructureSerializer.Instance; // System.Text.Json, .NET 10.0+ only
```

To load and save files in the same format, use <xref:NanoByte.StructureEditor.SerializingCommandManager`1>. Passing one to `Open()` also sets the editor's `Serializer`:
```csharp
editor.Open(SerializingCommandManager<AddressBook>.Load(path, JsonStructureSerializer.Instance));
editor.CommandManager.Save(path);
```

For Newtonsoft.Json (including on .NET Framework), add a reference to the [NanoByte.StructureEditor.NewtonsoftJson](https://www.nuget.org/packages/NanoByte.StructureEditor.NewtonsoftJson/) NuGet package and use <xref:NanoByte.StructureEditor.NewtonsoftJson.NewtonsoftJsonStructureSerializer>.

Polymorphic lists and properties need extra attributes or settings to round-trip through JSON. See [Serialization formats](serialization-formats.md) for details.

### Samples

Take a look at the sample projects for a more complete setup, including custom node editors and undo/redo functionality. Both use the same data model from [Sample](https://github.com/nano-byte/structure-editor/tree/master/src/Sample):
- [Sample.WinForms](https://github.com/nano-byte/structure-editor/tree/master/src/Sample.WinForms)
- [Sample.EtoForms](https://github.com/nano-byte/structure-editor/tree/master/src/Sample.EtoForms)
