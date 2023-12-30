// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Generator.Equals;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// Contact details of an individuals.
/// </summary>
[Description("Contact details of an individuals.")]
[Equatable]
public partial class Contact
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
    [OrderedEquality]
    public List<PhoneNumber> PhoneNumbers { get; } = [];

    public override string ToString() => FirstName + " " + LastName;
}
