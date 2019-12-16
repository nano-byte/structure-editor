// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace NanoByte.StructureEditor.Sample.Model
{
    /// <summary>
    /// Contact details of an individuals.
    /// </summary>
    [Description("Contact details of an individuals.")]
    public class Contact : IEquatable<Contact>
    {
        /// <summary>
        /// The first name of the contact.
        /// </summary>
        [Description("The first name of the contact.")]
        [XmlAttribute]
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the contact.
        /// </summary>
        [Description("The last name of the contact.")]
        [XmlAttribute]
        public string? LastName { get; set; }

        [Browsable(false)]
        public Address? HomeAddress { get; set; }

        [Browsable(false)]
        public Address? WorkAddress { get; set; }

        [Browsable(false)]
        [XmlElement(nameof(LandlineNumber), typeof(LandlineNumber))]
        [XmlElement(nameof(MobileNumber), typeof(MobileNumber))]
        public List<PhoneNumber> PhoneNumbers { get; } = new List<PhoneNumber>();

        public override string ToString() => FirstName + " " + LastName;

        public bool Equals(Contact other)
            => other != null
            && FirstName == other.FirstName
            && LastName == other.LastName
            && Equals(WorkAddress, other.WorkAddress)
            && Equals(HomeAddress, other.HomeAddress)
            && PhoneNumbers.SequenceEqual(other.PhoneNumbers);

        public override bool Equals(object? obj)
            => obj is Contact other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FirstName?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (LastName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (WorkAddress?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (HomeAddress?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
