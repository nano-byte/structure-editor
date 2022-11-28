// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// An object that contains <see cref="Contact"/>s.
/// </summary>
public interface IContactContainer
{
    /// <summary>
    /// Contact details of individuals.
    /// </summary>
    List<Contact> Contacts { get; }
}