using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Generator.Equals;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// An address book.
/// </summary>
[Description("An address book.")]
[Equatable]
public partial class AddressBook : IContactContainer
{
    /// <summary>
    /// The name of the address book.
    /// </summary>
    [Description("The name of the address book.")]
    [XmlAttribute]
    public string? Name { get; set; }

    /// <summary>
    /// Groups of multiple contacts.
    /// </summary>
    [Browsable(false)]
    [XmlElement(nameof(Group))]
    [OrderedEquality]
    public List<Group> Groups { get; } = [];

    /// <inheritdoc/>
    [Browsable(false)]
    [XmlElement(nameof(Contact))]
    [OrderedEquality]
    public List<Contact> Contacts { get; } = [];

    public override string ToString() => Name ?? "";
}
