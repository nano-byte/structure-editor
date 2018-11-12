// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor.Sample.Model
{
    [Description("Contact details of a person.")]
    public class Contact : IEquatable<Contact>
    {
        [Description("The first name of the contact.")]
        [XmlAttribute]
        public string FirstName { get; set; }

        [Description("The last name of the contact.")]
        [XmlAttribute]
        public string LastName { get; set; }

        [Browsable(false)]
        public Address HomeAddress { get; set; }

        [Browsable(false)]
        public Address WorkAddress { get; set; }

        [Browsable(false)]
        [XmlElement("Landline", typeof(LandlineNumber))]
        [XmlElement("Mobile", typeof(MobileNumber))]
        public List<PhoneNumber> PhoneNumbers { get; } = new List<PhoneNumber>();

        public override string ToString() => FirstName + " " + LastName;

        public bool Equals(Contact other)
            => other != null
            && FirstName == other.FirstName
            && LastName == other.LastName
            && Equals(WorkAddress, other.WorkAddress)
            && Equals(HomeAddress, other.HomeAddress)
            && PhoneNumbers.SequencedEquals(other.PhoneNumbers);

        public override bool Equals(object obj)
            => obj is Contact other && Equals(other);

        public override int GetHashCode()
            => 0;
    }
}
