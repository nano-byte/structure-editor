// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NanoByte.Common;
using NanoByte.StructureEditor.Sample.Controls;
using NanoByte.StructureEditor.Sample.Model;
using NanoByte.StructureEditor.WinForms;
using Xunit;

namespace NanoByte.StructureEditor;

public class ContainerDescriptionFacts
{
    [Fact]
    public void GetCandidatesFor()
    {
        var descriptor = new ContainerDescription<Contact>();
        descriptor
           .AddRequiredProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
           .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor());
        descriptor
           .AddPolymorphicProperty(x => PropertyPointer.ForNullable(() => x.PrimaryNumber))
           .AddElement("Primary Landline Number", () => new LandlineNumber())
           .AddElement("Primary Mobile Number", () => new MobileNumber());
        descriptor
           .AddPolymorphicList(x => x.PhoneNumbers)
           .AddElement("Landline Number", () => new LandlineNumber())
           .AddElement("Mobile Number", () => new MobileNumber());

        var container = new Contact();

        var candidates = descriptor.GetCandidatesFor(container).ToArray();
        ShouldBe(candidates,
            ("Home Address", "A postal address."),
            ("Work Address", "A postal address."),
            ("Primary Landline Number", "A phone number for a landline."),
            ("Primary Mobile Number", "A phone number for a mobile phone."),
            ("Landline Number", "A phone number for a landline."),
            ("Mobile Number", "A phone number for a mobile phone."));
    }

    [Fact]
    public void GetNodeIn()
    {
        var descriptor = new ContainerDescription<Contact>();
        descriptor
           .AddRequiredProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
           .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor());
        descriptor
           .AddPolymorphicProperty(x => PropertyPointer.ForNullable(() => x.PrimaryNumber))
           .AddElement("Primary Landline Number", () => new LandlineNumber())
           .AddElement("Primary Mobile Number", () => new MobileNumber());
        descriptor
           .AddPolymorphicList(x => x.PhoneNumbers)
           .AddElement("Landline Number", () => new LandlineNumber())
           .AddElement("Mobile Number", () => new MobileNumber());

        var primaryNumber = new MobileNumber();
        var landlineNumber = new LandlineNumber();
        var mobileNumber = new MobileNumber();
        var container = new Contact
        {
            HomeAddress = new Address(),
            WorkAddress = new Address(),
            PrimaryNumber = primaryNumber,
            PhoneNumbers = {landlineNumber, mobileNumber}
        };

        var nodes = descriptor.GetNodesIn(container).ToArray();
        ShouldBe(nodes,
            ("Home Address",  container.HomeAddress),
            ("Work Address", container.WorkAddress),
            ("Primary Mobile Number", primaryNumber),
            ("Landline Number", landlineNumber),
            ("Mobile Number", mobileNumber));
    }

    [Fact]
    public void RequiredPropertiesHaveNoRemoveCommand()
    {
        PhoneNumber requiredNumber = new MobileNumber();

        var descriptor = new ContainerDescription<Contact>();
        descriptor
           .AddRequiredProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
           .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor());
        descriptor
           .AddPolymorphicProperty(x => PropertyPointer.ForNullable(() => x.PrimaryNumber))
           .AddElement("Primary Landline Number", () => new LandlineNumber())
           .AddElement("Primary Mobile Number", () => new MobileNumber());
        descriptor
           .AddRequiredPolymorphicProperty(_ => PropertyPointer.For(() => requiredNumber))
           .AddElement("Required Landline Number", () => new LandlineNumber())
           .AddElement("Required Mobile Number", () => new MobileNumber());

        var container = new Contact
        {
            HomeAddress = new Address(),
            WorkAddress = new Address(),
            PrimaryNumber = new MobileNumber()
        };

        var nodes = descriptor.GetNodesIn(container).ToArray();

        // Required properties cannot be removed
        NodeFor(nodes, "Home Address").GetRemoveCommand().Should().BeNull();
        NodeFor(nodes, "Required Mobile Number").GetRemoveCommand().Should().BeNull();

        // Nullable properties can be removed
        NodeFor(nodes, "Work Address").GetRemoveCommand().Should().NotBeNull();
        NodeFor(nodes, "Primary Mobile Number").GetRemoveCommand().Should().NotBeNull();
    }

    private static Node NodeFor(IEnumerable<Node> nodes, string nodeType)
        => nodes.First(node => node.NodeType == nodeType);

    private static void ShouldBe(IReadOnlyList<NodeCandidate?> nodes, params (string name, string description)[] expectedValues)
    {
        nodes.Should().HaveCount(expectedValues.Length);
        for (int i = 0; i < expectedValues.Length; i++)
        {
            var node = nodes[i];
            Debug.Assert(node != null);
            node.NodeType.Should().Be(expectedValues[i].name);
            node.Description.Should().Be(expectedValues[i].description);
        }
    }

    private static void ShouldBe(IReadOnlyList<Node> nodes, params (string nodeType, object target)[] expectedValues)
    {
        nodes.Should().HaveCount(expectedValues.Length);
        for (int i = 0; i < expectedValues.Length; i++)
        {
            var node = nodes[i];
            node.NodeType.Should().Be(expectedValues[i].nodeType);
            node.Target.Should().BeSameAs(expectedValues[i].target);
        }
    }
}
