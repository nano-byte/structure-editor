// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using NanoByte.Common;
using NanoByte.StructureEditor.NewtonsoftJson;
using NanoByte.StructureEditor.Sample;
using NanoByte.StructureEditor.Sample.Controls;
using NanoByte.StructureEditor.Sample.Model;
using NanoByte.StructureEditor.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace NanoByte.StructureEditor;

public abstract class SerializerFacts
{
    protected abstract IStructureSerializer Serializer { get; }

    /// <summary>
    /// A serialized <see cref="Address"/> that is malformed in line 4.
    /// </summary>
    protected abstract string MalformedAddress { get; }

    [Fact]
    public void RoundTrip()
    {
        var original = SampleData.AddressBook;
        original.Contacts[0].PrimaryNumber = new MobileNumber {CountryCode = "+1", AreaCode = "555", LocalNumber = "0104"};

        string serialized = Serializer.Serialize(original);
        var deserialized = Serializer.Deserialize<AddressBook>(serialized);

        Serializer.Serialize(deserialized).Should().Be(serialized);
        deserialized.Contacts.SelectMany(x => x.PhoneNumbers).Select(x => x.GetType())
                    .Should().Equal(original.Contacts.SelectMany(x => x.PhoneNumbers).Select(x => x.GetType()));
        deserialized.Contacts.Select(x => x.PrimaryNumber?.GetType())
                    .Should().Equal(original.Contacts.Select(x => x.PrimaryNumber?.GetType()));
    }

    [Fact]
    public void ErrorLocation()
    {
        var exception = Assert.ThrowsAny<Exception>(() => Serializer.Deserialize<Address>(MalformedAddress));

        Serializer.TryGetErrorLocation(exception, out int line, out int column, out string message).Should().BeTrue();
        line.Should().Be(4);
        column.Should().BePositive();
        message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void NodeUpdate()
    {
        var descriptor = new ContainerDescription<Contact>();
        descriptor
           .AddPolymorphicList(x => x.PhoneNumbers)
           .AddElement("Landline Number", () => new LandlineNumber())
           .AddElement("Mobile Number", () => new MobileNumber());
        var container = new Contact {PhoneNumbers = {new LandlineNumber {LocalNumber = "0101"}}};
        var node = descriptor.GetNodesIn(container).Single();

        string serialized = node.GetSerialized(Serializer);
        node.GetUpdateCommand(serialized, Serializer).Should().BeNull(because: "unchanged text should not produce a command");

        var command = node.GetUpdateCommand(serialized.Replace("0101", "9999"), Serializer);
        command.Should().NotBeNull();
        command!.Execute();
        container.PhoneNumbers.Should().ContainSingle()
                 .Which.Should().BeOfType<LandlineNumber>()
                 .Which.LocalNumber.Should().Be("9999");
    }

    [Fact]
    public void PropertyNodeUpdate()
    {
        var descriptor = new ContainerDescription<Contact>();
        descriptor.AddRequiredProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor());
        var container = new Contact {HomeAddress = {City = "Paris"}};
        var node = descriptor.GetNodesIn(container).Single();

        var command = node.GetUpdateCommand(node.GetSerialized(Serializer).Replace("Paris", "Berlin"), Serializer);
        command.Should().NotBeNull();
        command!.Execute();
        container.HomeAddress.City.Should().Be("Berlin");
    }
}

public class XmlSerializerFacts : SerializerFacts
{
    protected override IStructureSerializer Serializer => XmlStructureSerializer.Instance;

    protected override string MalformedAddress => """
        <Address>
          <Street>1 Main Street</Street>
          <City>Paris</City>
        </Adress>
        """;

    [Fact]
    public void OmitsXmlDeclaration()
        => Serializer.Serialize(new Address()).Should().NotContain("<?xml");
}

#if NET
public class JsonSerializerFacts : SerializerFacts
{
    protected override IStructureSerializer Serializer => JsonStructureSerializer.Instance;

    protected override string MalformedAddress => """
        {
          "Street": "1 Main Street",
          "City": "Paris"
          "Country": "France"
        }
        """;
}
#endif

public class NewtonsoftJsonSerializerFacts : SerializerFacts
{
    protected override IStructureSerializer Serializer { get; } = new NewtonsoftJsonStructureSerializer(new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        SerializationBinder = new KnownTypesBinder(typeof(LandlineNumber), typeof(MobileNumber))
    });

    protected override string MalformedAddress => """
        {
          "Street": "1 Main Street",
          "City": "Paris"
          "Country": "France"
        }
        """;

    private class KnownTypesBinder(params Type[] knownTypes) : ISerializationBinder
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
}
