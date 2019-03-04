// Copyright Bastian Eicher
// Licensed under the MIT License

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
               .AddProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
               .AddProperty("Work Address", x => PropertyPointer.For(() => x.WorkAddress), new AddressEditor())
               .AddList(x => x.PhoneNumbers)
               .AddElement("Landline Number", new LandlineNumber())
               .AddElement("Mobile Number", new MobileNumber());

            var container = new Contact();
            descriptor.GetCandidatesFor(container).Should().Equal(
                new NodeCandidate("Home Address", null, null),
                new NodeCandidate("Work Address", null, null),
                new NodeCandidate("Landline Number", "A phone number for a landline.", null),
                new NodeCandidate("Mobile Number", "A phone number for a mobile phone.", null));
        }

        [Fact]
        public void GetNodeIn()
        {
            var descriptor = new ContainerDescription<Contact>();
            descriptor
               .AddProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
               .AddProperty("Work Address", x => PropertyPointer.For(() => x.WorkAddress), new AddressEditor())
               .AddList(x => x.PhoneNumbers)
               .AddElement("Landline Number", new LandlineNumber())
               .AddElement("Mobile Number", new MobileNumber());

            var container = new Contact
            {
                HomeAddress = new Address(),
                WorkAddress = new Address(),
                PhoneNumbers = {new LandlineNumber(), new MobileNumber()}
            };
            descriptor.GetNodesIn(container).Should().Equal(
                new Node("Home Address", null, container.HomeAddress, null, null, null, null),
                new Node("Work Address", null, container.WorkAddress, null, null, null, null),
                new Node("Landline Number", "A phone number for a landline.", container.PhoneNumbers[0], null, null, null, null),
                new Node("Mobile Number", "A phone number for a mobile phone.", container.PhoneNumbers[1], null, null, null, null));
        }
    }
}
