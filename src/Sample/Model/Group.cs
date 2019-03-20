// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using NanoByte.Common.Collections;

namespace NanoByte.StructureEditor.Sample.Model
{
    /// <summary>
    /// A group of multiple contacts.
    /// </summary>
    [Description("A group of multiple contacts.")]
    public class Group : IEquatable<Group>, IContactContainer
    {
        /// <summary>
        /// The name of the group.
        /// </summary>
        [Description("The name of the group.")]
        [XmlAttribute]
        public string Name { get; set; }

        /// <inheritdoc/>
        [Browsable(false)]
        [XmlElement(nameof(Contact))]
        public List<Contact> Contacts { get; } = new List<Contact>();

        public override string ToString() => Name;

        public bool Equals(Group other)
            => other != null
            && Name == other.Name
            && Contacts.SequencedEquals(other.Contacts);

        public override bool Equals(object obj)
            => obj is Group other && Equals(other);

        public override int GetHashCode()
            => Name?.GetHashCode() ?? 0;
    }
}
