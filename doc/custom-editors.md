# Custom node editors

When the default <xref:NanoByte.StructureEditor.WinForms.PropertyGridNodeEditor`1> is not a good fit (e.g., you want hint texts, validation, or a layout tailored to the type) supply your own editor by deriving from <xref:NanoByte.StructureEditor.WinForms.NodeEditorBase`1>.

## Deriving from `NodeEditorBase<T>`

`NodeEditorBase<T>` is a `UserControl` that implements <xref:NanoByte.StructureEditor.INodeEditor`1>. It exposes `Bind()` helpers that wire a WinForms control to a <xref:NanoByte.Common.PropertyPointer`1> through the editor's undo system. You typically build the control hierarchy in the constructor.

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

`Target!` is safe to dereference inside the `PropertyPointer` lambda. The pointer is evaluated lazily after `Target` has been assigned by the host editor.

## Available `Bind()` overloads

| Overload | Use for |
|---|---|
| `Bind(Control, PropertyPointer<string?>)` | Any `Control.Text`-based input (`TextBox`, `HintTextBox`, …) |
| `Bind(ComboBox, PropertyPointer<string?>)` | `ComboBox`, adds the current value to `Items` if missing |
| `Bind(UriTextBox, PropertyPointer<Uri?>)` | `Uri` properties, with validation |
| `Bind(CheckBox, PropertyPointer<bool>)` | `bool` properties |
| `Bind<TControl, TChild>(TControl, Func<TChild>)` | Nested `INodeEditor<TChild>` controls, propagates `Target` and `CommandExecutor` |

Each binding writes through <xref:NanoByte.Common.Undo.ICommandExecutor> when present, so every change becomes an undoable command. If no executor is attached, the pointer is set directly.

## Plugging the editor in

Pass an instance to the relevant fluent-API method instead of relying on the default `PropertyGrid`:

```csharp
editor.Describe<Contact>()
      .AddProperty("Home Address", x => PropertyPointer.ForNullable(() => x.HomeAddress), new AddressEditor())
      .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor());
```

The `editor` parameter is a dummy instance used solely for type inference of the generic editor type; the structure editor instantiates a fresh editor per node via `new TEditor()`.

## Refresh and lifecycle hooks

`NodeEditorBase<T>` exposes three `protected` events for advanced scenarios:

- `TargetChanged`: fires when a new `Target` is assigned. Use it to refresh control state that does not flow through `Bind()`.
- `CommandExecutorChanged`: fires when the undo system is (re)attached.
- `OnRefresh`: fires from `Refresh()`, both on target change and when external edits (e.g., the XML text editor) mutate the target.

The `Bind()` helpers subscribe to these internally, so you usually only need them when integrating controls that have no matching overload.
