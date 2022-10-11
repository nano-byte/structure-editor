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

namespace NanoByte.StructureEditor
{
    public class ContainerDescriptionFacts
    {
        [Fact]
        public void GetCandidatesFor()
        {
            var descriptor = new ContainerDescription<Contact>();
            descriptor
               .AddProperty("Home Address", x => PropertyPointer.ForNullable(() => x.HomeAddress), new AddressEditor())
               .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor())
               .AddList(x => x.PhoneNumbers)
               .AddElement("Landline Number", new LandlineNumber())
               .AddElement("Mobile Number", new MobileNumber());

            var container = new Contact();

            var candidates = descriptor.GetCandidatesFor(container).ToArray();
            ShouldBe(candidates,
                ("Home Address", "A postal address."),
                ("Work Address", "A postal address."),
                ("Landline Number", "A phone number for a landline."),
                ("Mobile Number", "A phone number for a mobile phone."));
        }

        [Fact]
        public void GetNodeIn()
        {
            var descriptor = new ContainerDescription<Contact>();
            descriptor
               .AddProperty("Home Address", x => PropertyPointer.ForNullable(() => x.HomeAddress), new AddressEditor())
               .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor())
               .AddList(x => x.PhoneNumbers)
               .AddElement("Landline Number", new LandlineNumber())
               .AddElement("Mobile Number", new MobileNumber());

            var landlineNumber = new LandlineNumber();
            var mobileNumber = new MobileNumber();
            var container = new Contact
            {
                HomeAddress = new Address(),
                WorkAddress = new Address(),
                PhoneNumbers = {landlineNumber, mobileNumber}
            };

            var nodes = descriptor.GetNodesIn(container).ToArray();
            ShouldBe(nodes,
                ("Home Address",  container.HomeAddress),
                ("Work Address", container.WorkAddress),
                ("Landline Number", landlineNumber),
                ("Mobile Number", mobileNumber));
        }

        private static void ShouldBe(IReadOnlyList<NodeCandidate?> nodes, params (string name, string description)[] expectedValues)
        {
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
            for (int i = 0; i < expectedValues.Length; i++)
            {
                var node = nodes[i];
                node.NodeType.Should().Be(expectedValues[i].nodeType);
                node.Target.Should().BeSameAs(expectedValues[i].target);
            }
        }
    }
}
