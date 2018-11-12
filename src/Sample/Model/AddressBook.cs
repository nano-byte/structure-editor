using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NanoByte.StructureEditor.Sample.Model
{
    [Description("An address book.")]
    public class AddressBook : IContactContainer, IEquatable<AddressBook>
    {
        [Description("The name of the address book.")]
        [XmlAttribute]
        public string Name { get; set; }

        [Browsable(false)]
        [XmlElement("Group")]
        public List<Group> Groups { get; } = new List<Group>();

        [Browsable(false)]
        [XmlElement("Contact")]
        public List<Contact> Contacts { get; } = new List<Contact>();

        public override string ToString() => Name;

        public bool Equals(AddressBook other)
            => false;

        public override bool Equals(object obj)
            => obj is AddressBook other && Equals(other);

        public override int GetHashCode()
            => 0;
    }
}
