// Copyright Bastian Eicher
// Licensed under the MIT License

using FluentAssertions;
using NanoByte.Common;
using NanoByte.StructureEditor.Sample.Controls;
using NanoByte.StructureEditor.WinForms;
using NanoByte.StructureEditor.Sample.Model;
using Xunit;

namespace NanoByte.StructureEditor
{
    public class ContainerDescriptionFacts
    {
        [Fact]
        public void GetPossibleChildrenFor()
        {
            var descriptor = new ContainerDescription<Contact>();
            descriptor
               .AddProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
               .AddProperty("Work Address", x => PropertyPointer.For(() => x.WorkAddress), new AddressEditor())
               .AddList(x => x.PhoneNumbers)
               .AddElement("Landline Number", new LandlineNumber())
               .AddElement("Mobile Number", new MobileNumber());

            var container = new Contact();
            descriptor.GetPossibleChildrenFor(container).Should().Equal(
                new ChildInfo("Home Address", null, null),
                new ChildInfo("Work Address", null, null),
                new ChildInfo("Landline Number", "A phone number for a landline.", null),
                new ChildInfo("Mobile Number", "A phone number for a mobile phone.", null));
        }

        [Fact]
        public void GetEntriesIn()
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
            descriptor.GetEntriesIn(container).Should().Equal(
                new EntryInfo("Home Address", null, container.HomeAddress, null, null, null, null),
                new EntryInfo("Work Address", null, container.WorkAddress, null, null, null, null),
                new EntryInfo("Landline Number", "A phone number for a landline.", container.PhoneNumbers[0], null, null, null, null),
                new EntryInfo("Mobile Number", "A phone number for a mobile phone.", container.PhoneNumbers[1], null, null, null, null));
        }
    }
}
