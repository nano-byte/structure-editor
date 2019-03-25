using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace NanoByte.StructureEditor.Sample.Model
{
    /// <summary>
    /// An address book.
    /// </summary>
    [Description("An address book.")]
    public class AddressBook : IContactContainer, IEquatable<AddressBook>
    {
        /// <summary>
        /// The name of the address book.
        /// </summary>
        [Description("The name of the address book.")]
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Groups of multiple contacts.
        /// </summary>
        [Browsable(false)]
        [XmlElement(nameof(Group))]
        public List<Group> Groups { get; } = new List<Group>();

        /// <inheritdoc/>
        [Browsable(false)]
        [XmlElement(nameof(Contact))]
        public List<Contact> Contacts { get; } = new List<Contact>();

        public override string ToString() => Name;

        public bool Equals(AddressBook other)
            => other != null
            && Name == other.Name
            && Groups.SequenceEqual(other.Groups)
            && Contacts.SequenceEqual(other.Contacts);

        public override bool Equals(object obj)
            => obj is AddressBook other && Equals(other);

        public override int GetHashCode()
            => Name?.GetHashCode() ?? 0;
    }
}
