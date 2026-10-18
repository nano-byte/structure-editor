# Serialization formats

The text editor shows a serialized representation of the selected node. By default this is XML, but you can switch to JSON or plug in your own format.

| Serializer                                                                                         | Package                                                                                                            | Frameworks                     |
| -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ | ------------------------------ |
| <xref:NanoByte.StructureEditor.XmlStructureSerializer> (default)                                   | [NanoByte.StructureEditor](https://www.nuget.org/packages/NanoByte.StructureEditor/)                               | .NET Framework 4.5+, .NET 8.0+ |
| <xref:NanoByte.StructureEditor.JsonStructureSerializer> (System.Text.Json)                         | [NanoByte.StructureEditor](https://www.nuget.org/packages/NanoByte.StructureEditor/)                               | .NET 8.0+                      |
| <xref:NanoByte.StructureEditor.NewtonsoftJson.NewtonsoftJsonStructureSerializer> (Newtonsoft.Json) | [NanoByte.StructureEditor.NewtonsoftJson](https://www.nuget.org/packages/NanoByte.StructureEditor.NewtonsoftJson/) | .NET Framework 4.5+, .NET 8.0+ |

## Choosing a serializer

Set the [Serializer](xref:NanoByte.StructureEditor.WinForms.StructureEditor`1#NanoByte_StructureEditor_WinForms_StructureEditor_1_Serializer) property to change the format of the text editor:

```csharp
editor.Serializer = JsonStructureSerializer.Instance;
```

To load and save files in the same format, use <xref:NanoByte.StructureEditor.SerializingCommandManager`1> instead of `CommandManager<T>` (which always uses XML). Passing one to `Open()` also sets the editor's `Serializer`:

```csharp
editor.Open(SerializingCommandManager<AddressBook>.Load(path, JsonStructureSerializer.Instance));
// ...
editor.CommandManager.Save(path);
```

Both JSON serializers accept options/settings in their constructors. Start from `JsonStructureSerializer.DefaultOptions` or `NewtonsoftJsonStructureSerializer.DefaultSettings` and adjust as needed.

## Modeling the data for JSON

XML serialization attributes such as `[XmlAttribute]`, `[XmlElement]` and `[XmlIgnore]` have no effect on JSON. Use the JSON library's own attributes (e.g., `[JsonIgnore]`, `[JsonPropertyName]`) where needed.

### Get-only collections

Lists exposed as get-only properties, such as `public List<Contact> Contacts { get; } = [];`, need to be populated in place rather than replaced:

- `JsonStructureSerializer.DefaultOptions` sets `PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate` for this. Keep it if you provide your own options.
- Newtonsoft.Json populates existing collections by default.

### Polymorphism

[Polymorphic lists](polymorphic-lists.md) and [polymorphic properties](polymorphic-properties.md) need a type discriminator in JSON, so the deserializer knows which concrete type to create.

**System.Text.Json:** annotate the base type with `[JsonPolymorphic]` and one `[JsonDerivedType]` per concrete type:

```csharp
[JsonPolymorphic]
[JsonDerivedType(typeof(LandlineNumber), nameof(LandlineNumber))]
[JsonDerivedType(typeof(MobileNumber), nameof(MobileNumber))]
public abstract class PhoneNumber { /* ... */ }
```

The discriminator is written as a `$type` property. By default, it must be the first property of the object. On .NET 9+ you can set `AllowOutOfOrderMetadataProperties = true` to relax this.

**Newtonsoft.Json:** enable `TypeNameHandling.Auto`. Always combine it with a `SerializationBinder` that only allows your own types. Otherwise, loading untrusted JSON can instantiate arbitrary types.

```csharp
editor.Serializer = new NewtonsoftJsonStructureSerializer(new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    TypeNameHandling = TypeNameHandling.Auto,
    SerializationBinder = new KnownTypesBinder(typeof(LandlineNumber), typeof(MobileNumber))
});

class KnownTypesBinder(params Type[] knownTypes) : ISerializationBinder
{
    public Type BindToType(string? assemblyName, string typeName)
        => knownTypes.FirstOrDefault(x => x.Name == typeName)
        ?? throw new JsonSerializationException($"Unknown type: {typeName}");

    public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }
}
```

When you select a single list element or property value in the tree, the text editor serializes it as its concrete type. So the discriminator only appears when you view the containing object.

## Custom formats

Implement <xref:NanoByte.StructureEditor.IStructureSerializer> to support other formats (e.g., YAML):

- `FormatName` selects the syntax highlighting of the text editor (e.g., `XML`, `JSON`).
- `Serialize()` and `Deserialize()` convert between objects and text. `Deserialize()` should throw on invalid input.
- `TryGetErrorLocation()` extracts a line and column from exceptions thrown by `Deserialize()`. The text editor uses this to underline the error.
