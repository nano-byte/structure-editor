// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Generator.Equals;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// A group of multiple contacts.
/// </summary>
[Description("A group of multiple contacts.")]
[Equatable]
public partial class  Group : IContactContainer
{
    /// <summary>
    /// The name of the group.
    /// </summary>
    [Description("The name of the group.")]
    [XmlAttribute]
    public string? Name { get; set; }

    /// <inheritdoc/>
    [Browsable(false)]
    [XmlElement(nameof(Contact))]
    public List<Contact> Contacts { get; } = new();

    public override string ToString() => Name ?? "";
}
