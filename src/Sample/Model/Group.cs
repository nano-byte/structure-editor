// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor.Sample.Model
{
    [Description("A group of multiple contacts.")]
    public class Group : IEquatable<Group>, IContactContainer
    {
        [Description("The name of the address book.")]
        [XmlAttribute]
        public string Name { get; set; }

        [Browsable(false)]
        [XmlElement("Contact")]
        public List<Contact> Contacts { get; } = new List<Contact>();

        public override string ToString() => Name;

        public bool Equals(Group other)
            => Name == other.Name
            && Contacts.SequencedEquals(other.Contacts);

        public override bool Equals(object obj)
            => obj is Group other && Equals(other);

        public override int GetHashCode()
            => 0;
    }
}
