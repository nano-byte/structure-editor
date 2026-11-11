# Custom node editors

When the default node editor is not a good fit (e.g., you want hint texts, validation, or a layout tailored to the type) supply your own editor by deriving from `NodeEditorBase<T>`.

|                               | WinForms                                                                    | Eto.Forms                                                             |
| ----------------------------- | --------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| Default node editor           | <xref:NanoByte.StructureEditor.WinForms.PropertyGridNodeEditor`1>           | <xref:NanoByte.StructureEditor.EtoForms.PropertyEditorNodeEditor`1>   |
| Base class for custom editors | <xref:NanoByte.StructureEditor.WinForms.NodeEditorBase`1> (a `UserControl`) | <xref:NanoByte.StructureEditor.EtoForms.NodeEditorBase`1> (a `Panel`) |

## Deriving from `NodeEditorBase<T>`

`NodeEditorBase<T>` implements <xref:NanoByte.StructureEditor.INodeEditor`1>. It exposes `Bind()` helpers that wire a control to a <xref:NanoByte.Common.PropertyPointer`1> through the editor's undo system. You typically build the control hierarchy in the constructor.

# [WinForms](#tab/winforms)

```csharp
public class AddressEditor : NodeEditorBase<Address>
{
    public AddressEditor()
    {
        var textBoxStreet  = new HintTextBox {HintText = "Street",  Dock = DockStyle.Top, Multiline = true, Height = 40};
        var textBoxCity    = new HintTextBox {HintText = "City",    Dock = DockStyle.Top};
        var textBoxCountry = new HintTextBox {HintText = "Country", Dock = DockStyle.Top};

        Bind(textBoxStreet,  PropertyPointer.ForNullable(() => Target!.Street));
        Bind(textBoxCity,    PropertyPointer.ForNullable(() => Target!.City));
        Bind(textBoxCountry, PropertyPointer.ForNullable(() => Target!.Country));

        // Reverse order for "Dock"
        Controls.Add(textBoxCountry);
        Controls.Add(textBoxCity);
        Controls.Add(textBoxStreet);
    }
}
```

# [Eto.Forms](#tab/etoforms)

The `Bind()` helpers **return the control**, so that you can place it directly in an Eto layout:

```csharp
public class AddressEditor : NodeEditorBase<Address>
{
    public AddressEditor()
    {
        Content = new TableLayout
        {
            Spacing = new(8, 4),
            Rows =
            {
                new TableRow(new Label {Text = "Street"}, new TableCell(Bind(new TextArea {Height = 50}, PropertyPointer.ForNullable(() => Target!.Street)), scaleWidth: true)),
                new TableRow(new Label {Text = "City"}, Bind(new TextBox(), PropertyPointer.ForNullable(() => Target!.City))),
                new TableRow(new Label {Text = "Country"}, Bind(new TextBox(), PropertyPointer.ForNullable(() => Target!.Country))),
                new TableRow {ScaleHeight = true}
            }
        };
    }
}
```

---

`Target!` is safe to dereference inside the `PropertyPointer` lambda. The pointer is evaluated lazily after `Target` has been assigned by the host editor.

## Available `Bind()` overloads

# [WinForms](#tab/winforms)

| Overload                                         | Use for                                                                          |
| ------------------------------------------------ | -------------------------------------------------------------------------------- |
| `Bind(Control, PropertyPointer<string?>)`        | Any `Control.Text`-based input (`TextBox`, `HintTextBox`, ...)                   |
| `Bind(ComboBox, PropertyPointer<string?>)`       | `ComboBox`, adds the current value to `Items` if missing                         |
| `Bind(UriTextBox, PropertyPointer<Uri?>)`        | `Uri` properties, with validation                                                |
| `Bind(CheckBox, PropertyPointer<bool>)`          | `bool` properties                                                                |
| `Bind<TControl, TChild>(TControl, Func<TChild>)` | Nested `INodeEditor<TChild>` controls, propagates `Target` and `CommandExecutor` |

# [Eto.Forms](#tab/etoforms)

| Overload                                             | Use for                                                                          |
| ---------------------------------------------------- | -------------------------------------------------------------------------------- |
| `Bind<TControl>(TControl, PropertyPointer<string?>)` | Any `TextControl` (`TextBox`, `TextArea`, ...)                                   |
| `Bind(ComboBox, PropertyPointer<string?>)`           | Editable `ComboBox`                                                              |
| `Bind(UriTextBox, PropertyPointer<Uri?>)`            | `Uri` properties, with validation                                                |
| `Bind(CheckBox, PropertyPointer<bool>)`              | `bool` properties                                                                |
| `Bind<TControl, TChild>(TControl, Func<TChild>)`     | Nested `INodeEditor<TChild>` controls, propagates `Target` and `CommandExecutor` |

Text input is applied when the control loses focus, when Enter is pressed in a single-line control, and when `CommitPendingChanges()` is called. The structure editor calls this before the selection changes.

---

Each binding writes through <xref:NanoByte.Common.Undo.ICommandExecutor> when present, so every change becomes an undoable command. If no executor is attached, the pointer is set directly.

## Plugging the editor in

Pass an instance to the relevant fluent-API method instead of relying on the default node editor:

```csharp
editor.Describe<Contact>()
      .AddProperty("Home Address", x => PropertyPointer.ForNullable(() => x.HomeAddress), new AddressEditor())
      .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor());
```

The `editor` parameter is a dummy instance used solely for type inference of the generic editor type; the structure editor instantiates a fresh editor per node via `new TEditor()`.

## Refresh and lifecycle hooks

`NodeEditorBase<T>` exposes `protected` events for advanced scenarios:

- `TargetChanged`: fires when a new `Target` is assigned. Use it to refresh control state that does not flow through `Bind()`.
- `CommandExecutorChanged`: fires when the undo system is (re)attached.
- `OnRefresh`: fires from `Refresh()`, both on target change and when external edits (e.g., the text editor) mutate the target.
- `OnCommitPendingChanges` (Eto.Forms only): fires from `CommitPendingChanges()`. Use it for controls that hold input of their own until they lose focus.

The `Bind()` helpers subscribe to these internally, so you usually only need them when integrating controls that have no matching overload.
