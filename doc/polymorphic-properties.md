# Polymorphic properties

`AddProperty()` covers the common case of a single-value property with a fixed type. When a property can hold one of several concrete types behind a common base class or interface, use `AddPolymorphicProperty()` and chain `AddElement()` for each concrete type the user should be able to set.

## Modeling the data

The XML serializer needs to know which CLR types the property can hold. Annotate the property with one `[XmlElement]` per concrete type, mapping the element name to the type:

```csharp
public abstract class PhoneNumber
{
    public string? CountryCode { get; set; }
    public string? AreaCode    { get; set; }
    public string? LocalNumber { get; set; }
}

public class LandlineNumber : PhoneNumber, IEquatable<LandlineNumber> { /* ... */ }
public class MobileNumber   : PhoneNumber, IEquatable<MobileNumber>   { /* ... */ }

public class Contact
{
    [XmlElement(nameof(LandlineNumber), typeof(LandlineNumber))]
    [XmlElement(nameof(MobileNumber),   typeof(MobileNumber))]
    public PhoneNumber? PrimaryNumber { get; set; }
}
```

The element name passed to `[XmlElement]` is the XML tag that will appear in the serialized output and inside the embedded text editor. It does not have to match the CLR type name, but doing so keeps things obvious.

> [!NOTE]
> Each concrete type must implement `IEquatable<TElement>`. The structure editor uses equality to decide whether an edit actually changed anything before recording an undo command. The sample uses [Generator.Equals](https://github.com/diegofrata/Generator.Equals) to generate these implementations, but a hand-written `Equals` works just as well.

## Describing the property

`AddPolymorphicProperty()` returns an <xref:NanoByte.StructureEditor.IPropertyDescription`2> on which you call `AddElement()` once per concrete type. The user-facing name and factory you pass here populate the "Add" drop-down menu:

```csharp
editor.Describe<Contact>()
      .AddPolymorphicProperty(x => PropertyPointer.ForNullable(() => x.PrimaryNumber))
      .AddElement("Primary Landline Number", () => new LandlineNumber())
      .AddElement("Primary Mobile Number",   () => new MobileNumber());
```

Because the property holds at most one value, the editor offers the configured types in the "Add" menu only while the property is unset, and setting one replaces any previous value.

Each `AddElement()` overload accepts an optional editor instance, the same way `AddProperty()` does. Pass a custom <xref:NanoByte.StructureEditor.INodeEditor`1> if the default `PropertyGrid` is not enough:

```csharp
.AddElement("Primary Landline Number", () => new LandlineNumber(), new LandlineEditor())
.AddElement("Primary Mobile Number",   () => new MobileNumber(),   new MobileEditor());
```
