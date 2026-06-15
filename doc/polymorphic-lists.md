# Polymorphic lists

`AddList()` covers the common case of a list with a single element type. When a list can hold multiple concrete types behind a common base class or interface, use `AddPolymorphicList()` and chain `AddElement()` for each concrete type the user should be able to add.

## Modeling the data

The XML serializer needs to know which CLR types each list can contain. Annotate the list property with one `[XmlElement]` per concrete type, mapping the element name to the type:

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
    public List<PhoneNumber> PhoneNumbers { get; } = [];
}
```

The element name passed to `[XmlElement]` is the XML tag that will appear in the serialized output and inside the embedded text editor. It does not have to match the CLR type name, but doing so keeps things obvious.

> [!NOTE]
> Each concrete type must implement `IEquatable<TElement>`. The structure editor uses equality to decide whether an edit actually changed anything before recording an undo command. The sample uses [Generator.Equals](https://github.com/diegofrata/Generator.Equals) to generate these implementations, but a hand-written `Equals` works just as well.

## Describing the list

`AddPolymorphicList()` returns an <xref:NanoByte.StructureEditor.IListDescription`2> on which you call `AddElement()` once per concrete type. The user-facing name and factory you pass here populate the "Add" drop-down menu:

```csharp
editor.Describe<Contact>()
      .AddPolymorphicList(x => x.PhoneNumbers)
      .AddElement("Landline Number", () => new LandlineNumber())
      .AddElement("Mobile Number",   () => new MobileNumber());
```

Each `AddElement()` overload accepts an optional editor instance, the same way `AddProperty()` does. Pass a custom <xref:NanoByte.StructureEditor.INodeEditor`1> if the default `PropertyGrid` is not enough:

```csharp
.AddElement("Landline Number", () => new LandlineNumber(), new LandlineEditor())
.AddElement("Mobile Number",   () => new MobileNumber(),   new MobileEditor());
```

## Containers shared across multiple parents

If several types expose the same kind of child collection, factor it out into an interface and call `Describe<TInterface>()`. The structure editor dispatches by runtime type, so any object implementing the interface picks up the description:

```csharp
public interface IContactContainer
{
    List<Contact> Contacts { get; }
}

public class AddressBook : IContactContainer { /* ... */ }
public class Group       : IContactContainer { /* ... */ }

editor.Describe<IContactContainer>()
      .AddList("Contact", x => x.Contacts);
```

Both `AddressBook` nodes and `Group` nodes will now offer "Add → Contact" in the tree, without having to repeat the description.
